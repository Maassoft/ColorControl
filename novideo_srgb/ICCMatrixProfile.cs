using System;
using System.IO;

namespace novideo_srgb
{
    public class ICCMatrixProfile
    {
        public Matrix matrix = Matrix.Zero3x3();
        public ToneCurve[] trcs = new ToneCurve[3];
        public ToneCurve[] vcgt;

        private ICCMatrixProfile()
        {
        }

        public static ICCMatrixProfile FromFile(string path)
        {
            var result = new ICCMatrixProfile();

            using (var reader = new ICCBinaryReader(File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                var stream = reader.BaseStream;

                {
                    stream.Seek(0x24, SeekOrigin.Begin);
                    var magic = new string(reader.ReadChars(4));
                    if (magic != "acsp")
                    {
                        throw new ICCProfileException("Not an ICC profile");
                    }
                }

                {
                    stream.Seek(0xC, SeekOrigin.Begin);
                    var type = new string(reader.ReadChars(4));
                    if (type != "mntr")
                    {
                        throw new ICCProfileException("Not a display device profile");
                    }
                }

                {
                    stream.Seek(0x10, SeekOrigin.Begin);
                    var spaces = new string(reader.ReadChars(8));
                    if (spaces != "RGB XYZ ")
                    {
                        throw new ICCProfileException("Not an RGB profile with XYZ PCS");
                    }
                }

                stream.Seek(0x80, SeekOrigin.Begin);

                var tagCount = reader.ReadUInt32();

                var seenTags = 0;

                var useCLUT = false;
                for (uint i = 0; i < tagCount; i++)
                {
                    stream.Seek(0x80 + 4 + 12 * i, SeekOrigin.Begin);
                    var tagSig = new string(reader.ReadChars(4));

                    var offset = reader.ReadUInt32();
                    var size = reader.ReadUInt32();

                    reader.BaseStream.Seek(offset, SeekOrigin.Begin);

                    var index = Array.IndexOf(new[] { 'r', 'g', 'b' }, tagSig[0]);

                    if (tagSig == "A2B1")
                    {
                        useCLUT = true;
                        var typeSig = new string(reader.ReadChars(4));
                        if (typeSig != "mft2")
                        {
                            throw new ICCProfileException(tagSig + " is not of lut16Type");
                        }

                        reader.ReadUInt32();

                        var inputChannels = reader.ReadByte();
                        if (inputChannels != 3)
                        {
                            throw new ICCProfileException(tagSig + " must have 3 input channels");
                        }

                        var outputChannels = reader.ReadByte();
                        if (outputChannels != 3)
                        {
                            throw new ICCProfileException(tagSig + " must have 3 output channels");
                        }

                        var lutPoints = reader.ReadByte();

                        reader.ReadByte();
                        for (var j = 0; j < 9; j++)
                        {
                            reader.ReadS15Fixed16();
                        }

                        var inputEntries = reader.ReadUInt16();
                        var outputEntries = reader.ReadUInt16();

                        var input = new ToneCurve[3];
                        for (var j = 0; j < 3; j++)
                        {
                            var table = new ushort[inputEntries];
                            for (var k = 0; k < inputEntries; k++)
                            {
                                table[k] = reader.ReadUInt16();
                            }

                            input[j] = new LutToneCurve(table);
                        }

                        var clut = new ushort[lutPoints, lutPoints, lutPoints, 3];
                        for (var r = 0; r < lutPoints; r++)
                        {
                            for (var g = 0; g < lutPoints; g++)
                            {
                                for (var b = 0; b < lutPoints; b++)
                                {
                                    for (var j = 0; j < 3; j++)
                                    {
                                        clut[r, g, b, j] = reader.ReadUInt16();
                                    }
                                }
                            }
                        }

                        var output = new LutToneCurve[3];
                        for (var j = 0; j < 3; j++)
                        {
                            var table = new ushort[outputEntries];
                            for (var k = 0; k < outputEntries; k++)
                            {
                                table[k] = reader.ReadUInt16();
                            }

                            output[j] = new LutToneCurve(table, 32768);
                        }

                        var lut16 = new Lut16(input, clut, output);
                        var black = lut16.SampleGrayscaleAt(0);

                        var primaries = new[]
                        {
                            lut16.SampleAt(1, 0, 0),
                            lut16.SampleAt(0, 1, 0),
                            lut16.SampleAt(0, 0, 1)
                        };

                        var Mprime = Matrix.Zero3x3();
                        for (var j = 0; j < 3; j++)
                        {
                            var purePrimary = primaries[j] - black;
                            for (var k = 0; k < 3; k++)
                            {
                                Mprime[k, j] = purePrimary[k] / purePrimary[1];
                            }
                        }

                        var M = Mprime * Matrix.FromDiagonal(Mprime.Inverse() * Colorimetry.D50);
                        var Minv = M.Inverse();
                        result.matrix = M;

                        const int trcSize = 4096;
                        var trcs = new double[3][];
                        for (var j = 0; j < 3; j++)
                        {
                            trcs[j] = new double[trcSize];
                        }

                        for (var j = 0; j < trcSize - 1; j++)
                        {
                            var values = lut16.SampleGrayscaleAt(j / (double)(trcSize - 1));

                            var toneResponse = Minv * values;
                            for (var k = 0; k < 3; k++)
                            {
                                trcs[k][j] = Math.Min(Math.Max(toneResponse[k], 0), 1);
                            }
                        }

                        for (var j = 0; j < 3; j++)
                        {
                            trcs[j][trcSize - 1] = 1;
                            result.trcs[j] = new DoubleToneCurve(trcs[j]);
                        }
                    }
                    else if (tagSig.EndsWith("TRC") && !useCLUT)
                    {
                        var typeSig = new string(reader.ReadChars(4));
                        if (typeSig != "curv")
                        {
                            throw new ICCProfileException(tagSig + " is not of curveType");
                        }

                        reader.ReadUInt32();

                        var numEntries = reader.ReadUInt32();

                        ToneCurve curve;
                        if (numEntries == 1)
                        {
                            var gamma = reader.ReadU8Fixed8();
                            curve = new GammaToneCurve(gamma);
                        }
                        else
                        {
                            var entries = new ushort[numEntries];
                            for (uint j = 0; j < numEntries; j++)
                            {
                                entries[j] = reader.ReadUInt16();
                            }

                            curve = new LutToneCurve(entries);
                        }

                        result.trcs[index] = curve;

                        seenTags++;
                    }
                    else if (tagSig.EndsWith("XYZ") && !useCLUT)
                    {
                        reader.ReadUInt32();
                        reader.ReadUInt32();

                        for (var j = 0; j < 3; j++)
                        {
                            result.matrix[j, index] = reader.ReadS15Fixed16();
                        }

                        seenTags++;
                    }
                    else if (tagSig == "vcgt")
                    {
                        reader.ReadChars(4);
                        reader.ReadUInt32();
                        var type = reader.ReadUInt32();
                        if (type != 0) throw new ICCProfileException("Only VCGT type 0 is supported");

                        var numChannels = reader.ReadUInt16();
                        var numEntries = reader.ReadUInt16();
                        var entrySize = reader.ReadUInt16();

                        if (numChannels != 3) throw new ICCProfileException("Only VCGT with 3 channels is supported");

                        result.vcgt = new ToneCurve[3];
                        for (var j = 0; j < 3; j++)
                        {
                            var values = new ushort[numEntries];
                            for (var k = 0; k < numEntries; k++)
                            {
                                switch (entrySize)
                                {
                                    case 1:
                                        values[k] = (ushort)(reader.ReadByte() * ushort.MaxValue / byte.MaxValue);
                                        break;
                                    case 2:
                                        values[k] = reader.ReadUInt16();
                                        break;
                                    default:
                                        throw new ICCProfileException("Only 8 and 16 bit VCGT is supported");
                                }
                            }

                            result.vcgt[j] = new LutToneCurve(values);
                        }
                    }
                }

                if (!useCLUT)
                {
                    if (seenTags != 6)
                    {
                        throw new ICCProfileException("Missing required tags for curves + matrix profile");
                    }

                    result.matrix = Colorimetry.XYZScaleToD50(result.matrix);
                }
            }

            return result;
        }
    }
}