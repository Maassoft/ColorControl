using NvAPIWrapper.Display;
using NvAPIWrapper.Native.Display;
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace ColorControl
{
    public class ColorDataConverter : JavaScriptConverter
    {
        public override IEnumerable<Type> SupportedTypes
        {
            get { return new List<Type>() { typeof(ColorData) }; }
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            if (obj == null) return result;
            return result;
        }

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            var format = ColorDataFormat.RGB;
            var dynamicRange = ColorDataDynamicRange.VESA;
            var colorDepth = ColorDataDepth.BPC8;
            var colorimetry = ColorDataColorimetry.Auto;
            var selectionPolicy = ColorDataSelectionPolicy.User;
            object value;
            if (dictionary.TryGetValue("ColorFormat", out value))
            {
                format = (ColorDataFormat)Enum.ToObject(typeof(ColorDataFormat), value);
            }
            if (dictionary.TryGetValue("ColorDepth", out value))
            {
                colorDepth = (ColorDataDepth)Enum.ToObject(typeof(ColorDataDepth), value);
            }
            if (dictionary.TryGetValue("Colorimetry", out value))
            {
                colorimetry = (ColorDataColorimetry)Enum.ToObject(typeof(ColorDataColorimetry), value);
            }
            if (dictionary.TryGetValue("DynamicRange", out value))
            {
                dynamicRange = (ColorDataDynamicRange)Enum.ToObject(typeof(ColorDataDynamicRange), value);
            }
            if (dictionary.TryGetValue("SelectionPolicy", out value))
            {
                selectionPolicy = (ColorDataSelectionPolicy)Enum.ToObject(typeof(ColorDataSelectionPolicy), value);
            }
            return new ColorData(format, dynamicRange: dynamicRange, colorimetry: colorimetry, colorDepth: colorDepth, colorSelectionPolicy: selectionPolicy);
        }
    }
}
