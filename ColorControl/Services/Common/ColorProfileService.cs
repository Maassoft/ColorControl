using ColorControl.Shared.Common;
using ColorControl.Shared.Contracts.DisplayInfo;
using ColorControl.Shared.Native;
using ColorControl.Shared.Services;
using EDIDParser;
using MHC2Gen;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WindowsDisplayAPI;
using static ColorControl.Shared.Native.CCD;

namespace ColorControl.Services.Common;

public class ColorProfileService
{
	private readonly GlobalContext _globalContext;
	private readonly RpcClientService _rpcClientService;
	private readonly WinApiService _winApiService;
	private readonly ServiceManager _serviceManager;
	private readonly WinApiAdminService _winApiAdminService;

	public ColorProfileService(GlobalContext globalContext, RpcClientService rpcClientService, WinApiService winApiService, ServiceManager serviceManager, WinApiAdminService winApiAdminService)
	{
		_globalContext = globalContext;
		_rpcClientService = rpcClientService;
		_winApiService = winApiService;
		_serviceManager = serviceManager;
		_winApiAdminService = winApiAdminService;

		_rpcClientService.Name = nameof(ColorProfileService);
	}

	public bool InstallColorProfile(string profileName)
	{
		return CCD.InstallColorProfile(profileName);
	}

	public bool UninstallColorProfile(string profileName)
	{
		return CCD.UninstallColorProfile(profileName);
	}

	public bool AddDisplayColorProfile(string displayName, string profileName, bool setAsDefault = true, bool isHdrColorProfile = false)
	{
		return CCD.AddDisplayColorProfile(displayName, profileName, setAsDefault, isHdrColorProfile);
	}


	public bool RemoveDisplayColorProfile(string displayName, string profileName, bool isHdrColorProfile = false)
	{
		return CCD.RemoveDisplayColorProfile(displayName, profileName, isHdrColorProfile);
	}

	public bool SetDisplayDefaultColorProfile(string displayName, string profileName, bool fixMaxTML = true, bool isHdrColorProfile = false)
	{
		return CCD.SetDisplayDefaultColorProfile(displayName, profileName, fixMaxTML, isHdrColorProfile);
	}

	public string GetDisplayDefaultColorProfile(string displayName, bool isHdrColorProfile)
	{
		return CCD.GetDisplayDefaultColorProfile(displayName, isHdrColorProfile);
	}

	public List<string> GetDisplayColorProfiles(string displayName, bool isHdrColorProfile = false)
	{
		if (displayName != null && !displayName.StartsWith("\\\\"))
		{
			displayName = GetDisplayNameByDisplayId(displayName);
		}

		return CCD.GetDisplayColorProfiles(displayName, isHdrColorProfile);
	}

	public List<ColorProfileDto> GetAllDisplayColorProfiles(string displayName)
	{
		if (displayName != null && !displayName.StartsWith("\\\\"))
		{
			displayName = GetDisplayNameByDisplayId(displayName);
		}

		var display = GetDisplayByGdiName(displayName);

		if (display == null)
		{
			return [];
		}

		var advancedColor = CCD.GetAdvancedColorSupportedAndEnabled(display.DisplayName);

		return GetAllDisplayColorProfiles(display, advancedColor.isSupported);
	}

	public List<ColorProfileDto> GetAllDisplayColorProfiles(Display display, bool isHdrSupported = false)
	{
		var colorProfiles = GetDisplayColorProfiles(display.DisplayName, false).Select(p => new ColorProfileDto { Name = p, IsHDRProfile = false }).ToList();
		if (isHdrSupported)
		{
			colorProfiles = colorProfiles.Union(GetDisplayColorProfiles(display.DisplayName, true).Select(p => new ColorProfileDto { Name = p, IsHDRProfile = true })).ToList();
		}

		return colorProfiles;
	}

	public List<string> GetAllColorProfileNames()
	{
		if (!_winApiService.IsAdministrator())
		{
			return _rpcClientService.Call<List<string>>(nameof(GetAllColorProfileNames));
		}

		return CCD.GetAllColorProfileNames();
	}

	private string GetDisplayNameByDisplayId(string displayName)
	{
		var deviceName = default(string);

		Display.GetDisplays().FirstOrDefault(d =>
		{
			var ccdDisplayInfo = CCD.GetDisplayInfo(d.DisplayName);

			if (ccdDisplayInfo.DisplayId == displayName)
			{
				deviceName = ccdDisplayInfo.DisplayName;

				return true;
			}
			return false;
		});

		return deviceName;
	}

	public Display GetDisplayByGdiName(string displayName)
	{
		return Display.GetDisplays().FirstOrDefault(d => d.DisplayName == displayName);
	}

	public bool GetUsePerUserDisplayProfiles(string displayName)
	{
		return CCD.GetUsePerUserDisplayProfiles(displayName);
	}

	public bool SetUsePerUserDisplayProfiles(string displayName, bool usePerUserProfiles)
	{
		return CCD.SetUsePerUserDisplayProfiles(displayName, usePerUserProfiles);
	}

	public bool DeleteColorProfile(string name)
	{
		return CCD.UninstallColorProfile(name);
	}

	public ColorProfileDto GetColorProfile(string name, string displayName = null)
	{
		var isAssociatedAsHdr = CCD.IsColorProfileAssociatedAsHdr(displayName, name);

		var data = MHC2Wrapper.LoadProfile(name, isAssociatedAsHdr);

		return new ColorProfileDto
		{
			Name = name,
			HasExtraInfo = data.HasExtraInfo,
			BlackLuminance = data.BlackLuminance,
			ColorGamut = data.ColorGamut,
			Description = data.Description,
			DevicePrimaries = new RgbPrimariesDto(data.DevicePrimaries),
			Gamma = data.Gamma,
			IsHDRProfile = data.IsHDRProfile,
			MaxCLL = data.MaxCLL,
			MinCLL = data.MinCLL,
			SDRBrightnessBoost = data.SDRBrightnessBoost,
			SDRMaxBrightness = data.SDRMaxBrightness,
			SDRMinBrightness = data.SDRMinBrightness,
			SDRTransferFunction = data.SDRTransferFunction,
			ShadowDetailBoost = data.ShadowDetailBoost,
			WhiteLuminance = data.WhiteLuminance,
		};
	}

	public bool UpdateColorProfile(ColorProfileDto colorProfile, string displayName = null)
	{
		var command = new GenerateProfileCommand
		{
			Description = colorProfile.Description,
			IsHDRProfile = colorProfile.IsHDRProfile,
			MinCLL = colorProfile.MinCLL,
			MaxCLL = colorProfile.MaxCLL,
			BlackLuminance = colorProfile.BlackLuminance,
			WhiteLuminance = colorProfile.WhiteLuminance,
			SDRMinBrightness = colorProfile.SDRMinBrightness,
			SDRMaxBrightness = colorProfile.SDRMaxBrightness,
			SDRTransferFunction = colorProfile.SDRTransferFunction,
			SDRBrightnessBoost = colorProfile.SDRBrightnessBoost,
			ShadowDetailBoost = colorProfile.ShadowDetailBoost,
			ColorGamut = colorProfile.ColorGamut,
			Gamma = colorProfile.Gamma,
			DevicePrimaries = colorProfile.DevicePrimaries.ToInternal(),
			
			ToneMappingFromLuminance = colorProfile.ToneMappingFromLuminance,
			ToneMappingToLuminance = colorProfile.ToneMappingToLuminance,
			HdrBrightnessMultiplier = colorProfile.HdrBrightnessMultiplier,
			HdrGammaMultiplier = colorProfile.HdrGammaMultiplier,
		};

		var bytes = MHC2Wrapper.GenerateSdrAcmProfile(command);

		var tempFilename = Path.GetFullPath(colorProfile.Name, Path.GetTempPath());

		if (File.Exists(tempFilename))
		{
			File.Delete(tempFilename);
		}

		File.WriteAllBytes(tempFilename, bytes);

		_winApiAdminService.UninstallColorProfile(tempFilename);

		var result = CCD.InstallColorProfile(tempFilename);

		if (result && displayName != null)
		{
			CCD.AddDisplayColorProfile(displayName, colorProfile.Name, false, command.IsHDRProfile);
		}

		return result;
	}

	public bool InstallAndActivateColorProfile(ColorProfileDto colorProfile, string displayName)
	{
		var installed = UpdateColorProfile(colorProfile);

		if (!installed)
		{
			return false;
		}

		var profileName = colorProfile.Name;

		return CCD.SetDisplayDefaultColorProfile(displayName, profileName, _globalContext.Config.SetMinTmlAndMaxTml, colorProfile.IsHDRProfile);
	}

	public List<DisplayInfoDto> GetDisplayInfos(bool? onlyHdrEnabled = null)
	{
		return Display.GetDisplays().Select(d =>
		{
			var ccdDisplayInfo = CCD.GetDisplayInfo(d.DisplayName);
			var advancedColor = CCD.GetAdvancedColorSupportedAndEnabled(d.DisplayName);
			var colorParams = CCD.GetColorParams(d.DisplayName);

			var displayColors = new List<DisplayColorInfo>
			{
				new DisplayColorInfo
				{
					DisplayPrimariesSource = MHC2Gen.DisplayPrimariesSource.Custom,
					CustomName = "sRGB",
					RgbPrimaries = new RgbPrimariesDto(RgbPrimaries.sRGB),
					BlackLuminance = 0,
					WhiteLuminance = 100,
					MinCLL = 0,
					MaxCLL = 100
				}
			};

			if (!colorParams.Equals(default(ColorParams)))
			{
				displayColors.Add(new DisplayColorInfo
				{
					DisplayPrimariesSource = MHC2Gen.DisplayPrimariesSource.Windows,
					RgbPrimaries = ColorParamsToRgbPrimaries(colorParams),
					BlackLuminance = (double)colorParams.MinLuminance / 10000,
					WhiteLuminance = (double)colorParams.MaxLuminance / 10000,
					MinCLL = (double)colorParams.MinLuminance / 10000,
					MaxCLL = (double)colorParams.MaxLuminance / 10000
				});
			}

			var edid = GetEDIDInternal(d.DevicePath);
			if (edid != null)
			{
				displayColors.Add(new DisplayColorInfo
				{
					DisplayPrimariesSource = MHC2Gen.DisplayPrimariesSource.EDID,
					RgbPrimaries = EdidToRgbPrimaries(edid),
					BlackLuminance = (double)colorParams.MinLuminance / 10000,
					WhiteLuminance = (double)colorParams.MaxLuminance / 10000,
					MinCLL = (double)colorParams.MinLuminance / 10000,
					MaxCLL = (double)colorParams.MaxLuminance / 10000
				});
			}

			var colorProfiles = GetAllDisplayColorProfiles(d, advancedColor.isSupported);

			return new DisplayInfoDto
			{
				DisplayName = ccdDisplayInfo.ExtendedName,
				DevicePath = ccdDisplayInfo.DevicePath,
				DisplayId = ccdDisplayInfo.DisplayId,
				IsActive = true,
				DeviceName = d.DisplayName,
				AdapterName = d.Adapter.DeviceName,
				IsAdvancedColorEnabled = advancedColor.isEnabled,
				IsAdvancedColorSupported = advancedColor.isSupported,
				DisplayColors = displayColors,
				ColorProfiles = colorProfiles,
				DefaultSdrColorProfile = GetDisplayDefaultColorProfile(d.DisplayName, false),
				DefaultHdrColorProfile = GetDisplayDefaultColorProfile(d.DisplayName, true),
			};
		})
			.Where(d => !onlyHdrEnabled.HasValue || onlyHdrEnabled == d.IsHdrSupportedAndEnabled)
			.ToList();
	}

	public RgbPrimariesDto ColorParamsToRgbPrimaries(ColorParams colorParams)
	{
		var divider = colorParams.RedPointX <= 1 << 10 ? 1 << 10 : 1 << 20;

		return new RgbPrimariesDto
		{
			Red = new ColorPoint
			{
				X = (double)colorParams.RedPointX / divider,
				Y = (double)colorParams.RedPointY / divider
			},
			Green = new ColorPoint
			{
				X = (double)colorParams.GreenPointX / divider,
				Y = (double)colorParams.GreenPointY / divider
			},
			Blue = new ColorPoint
			{
				X = (double)colorParams.BluePointX / divider,
				Y = (double)colorParams.BluePointY / divider
			},
			White = new ColorPoint
			{
				X = (double)colorParams.WhitePointX / divider,
				Y = (double)colorParams.WhitePointY / divider
			}
		};
	}

	public RgbPrimariesDto EdidToRgbPrimaries(EDID edid)
	{
		var coords = edid.DisplayParameters.ChromaticityCoordinates;

		return new RgbPrimariesDto
		{
			Red = new ColorPoint
			{
				X = coords.RedX,
				Y = coords.RedY
			},
			Green = new ColorPoint
			{
				X = coords.GreenX,
				Y = coords.GreenY
			},
			Blue = new ColorPoint
			{
				X = coords.BlueX,
				Y = coords.BlueY
			},
			White = new ColorPoint
			{
				X = coords.WhiteX,
				Y = coords.WhiteY
			}
		};
	}

	public static EDID GetEDIDInternal(string path)
	{
		try
		{
			var registryPath = "HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Enum\\DISPLAY\\";
			registryPath += string.Join("\\", path.Split('#').Skip(1).Take(2));
			return new EDID((byte[])Registry.GetValue(registryPath + "\\Device Parameters", "EDID", null));
		}
		catch
		{
			return null;
		}
	}
}

