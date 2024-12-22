using System.ComponentModel;

namespace ColorControl.Shared.Contracts.LG;

public enum ButtonType
{
	HOME,
	BACK,
	ENTER,
	EXIT,
	UP,
	DOWN,
	LEFT,
	RIGHT,
	RED,
	GREEN,
	YELLOW,
	BLUE,
	[Description("1")]
	_1,
	[Description("2")]
	_2,
	[Description("3")]
	_3,
	[Description("4")]
	_4,
	[Description("5")]
	_5,
	[Description("6")]
	_6,
	[Description("7")]
	_7,
	[Description("8")]
	_8,
	[Description("9")]
	_9,
	[Description("0")]
	_0,
	POWER,
	VOLUMEUP,
	VOLUMEDOWN,
	MUTE,
	MENU,
	CC,
	DASH,
	CHANNELUP,
	CHANNELDOWN,
	LIST,
	AD,
	SAP,
	PROGRAM,
	PLAY,
	PAUSE,
	STOP,
	REWIND,
	FASTFORWARD,
	GUIDE,
	AMAZON,
	NETFLIX,
	MAGNIFIER_ZOOM,
	LIVE_ZOOM,
	[Description("3D Mode")]
	_3D_MODE,
	ASPECT_RATIO,
	RECENT,
	RECORD,
	SCREEN_REMOTE,
	MYAPPS
}

public enum ControlButtons
{
	Back, Down, Left, Right,
	OK,
	Exit
}

public enum PictureMode
{
	cinema, eco, expert1, expert2, game, normal, photo, sports, technicolor, vivid, hdrEffect, filmMaker,

	hdrCinema, hdrCinemaBright, hdrExternal, hdrGame, hdrStandard, hdrTechnicolor, hdrVivid, hdrFilmMaker,

	dolbyHdrCinema, dolbyHdrCinemaBright, dolbyHdrDarkAmazon, dolbyHdrGame, dolbyHdrStandard, dolbyHdrVivid, dolbyStandard
}

public enum DynamicRange
{
	sdr, hdr, technicolorHdr, dolbyHdr,
	sdrALLM, hdrALLM, technicolorHdrALLM, dolbyHdrALLM
}

public enum OffToHigh
{
	off,
	low,
	medium,
	high
}

public enum LowToAuto
{
	low,
	medium,
	high,
	auto
}

public enum OffToOn
{
	off,
	on
}

public enum OffToAuto2
{
	off,
	on,
	auto
}

public enum OffToAuto
{
	off,
	on,
	low,
	medium,
	high,
	auto
}

public enum ColorGamut
{
	auto,
	extended,
	wide,
	srgb,
	native
}

public enum EnergySaving
{
	auto,
	off,
	min,
	med,
	max,
	screen_off
}

public enum DynamicTonemapping
{
	on,
	off,
	HGIG,
}

public enum TruMotionMode
{
	off,
	smooth,
	clear,
	clearPlus,
	cinemaClear,
	natural,
	user
}

public enum WhiteBalanceColorTemperature
{
	cool,
	medium,
	warm1,
	warm2,
	warm3
}

public enum FalseToTrue
{
	[Description("Disabled")]
	false_,
	[Description("Enabled")]
	true_
}

public enum BoolFalseToTrue
{
	[Description("Disabled")]
	bool_false,
	[Description("Enabled")]
	bool_true
}

public enum WhiteBalanceMethod
{
	[Description("2-points")]
	_2,
	[Description("20-points")]
	_10,
	[Description("22-points")]
	_22
}

public enum GammaExp
{
	low,
	medium,
	high1,
	high2
}

public enum BlackLevel
{
	low,
	medium,
	high,
	limited,
	full,
	auto
}

public enum AspectRatio
{
	_21x9,
	_16x9,
	_4x3,
	_14x9,
	_32x9,
	_32x12,
	just_scan,
	original,
	full_wide,
	limited,
	zoom,
	zoom2,
	cinema_zoom,
	vertZoom,
	allDirZoom,
	twinZoom
}

public enum InputOptimization
{
	auto,
	on,
	standard,
	boost
}

public enum OffToLevel2
{
	off,
	level1,
	level2
}

public enum HdmiIcon
{
	[Description("HDMI")]
	hdmigeneric,
	[Description("Satellite")]
	satellite,
	[Description("Set-Top Box")]
	settopbox,
	[Description("DVD Player")]
	dvd,
	[Description("Blu-ray Player")]
	bluray,
	[Description("Home Theater")]
	hometheater,
	[Description("Game Console")]
	gameconsole,
	[Description("Streaming Box")]
	streamingbox,
	[Description("Generic Camera")]
	camera,
	[Description("PC")]
	pc,
	[Description("Mobile Device")]
	mobile
}

public enum SoundMode
{
	[Description("AI Sound Pro")]
	aiSoundPlus,
	[Description("AI Sound")]
	aiSound,
	[Description("Standard")]
	standard,
	[Description("Clear Voice")]
	news,
	[Description("Music")]
	music,
	[Description("Cinema")]
	movie,
	[Description("Sports")]
	sports,
	[Description("Game Optimizer")]
	game,
	[Description("Pagode")]
	pagode,
	[Description("Serta Wego")]
	sertaWego,
	[Description("Brazilian Punk")]
	brazilianPunk,
	[Description("ASC")]
	asc,
	[Description("Bass Boost")]
	bass,
}

public enum SoundOutput
{
	[Description("TV Speaker")]
	tv_speaker,
	[Description("HDMI(ARC) Device")]
	external_arc,
	[Description("Optical Out Device")]
	external_optical,
	[Description("Bluetooth Device")]
	bt_soundbar,
	[Description("Mobile Device")]
	mobile_phone,
	[Description("Audio Out Device")]
	lineout,
	[Description("Wired Headphones")]
	headphone,
	[Description("Bluetooth Device + TV Speaker")]
	tv_speaker_bluetooth,
	[Description("Optical Out Device + TV Speaker")]
	tv_external_speaker,
	[Description("Wired Headphones + TV Speaker")]
	tv_speaker_headphone,
	[Description("WiSA Speakers")]
	wisa_speaker,
}

public enum MasterLuminanceLevel
{
	[Description("540 nits")]
	_540nit,
	[Description("1000 nits")]
	_1000nit,
	[Description("4000 nits")]
	_4000nit
}

public enum MasteringColor
{
	auto,
	[Description("0")]
	_0,
	p3D65,
	bt2020D65,
	bt709D65,
}

public enum MasteringNits
{
	auto,
	[Description("0")]
	_0,
	[Description("400")]
	_400,
	[Description("540")]
	_540,
	[Description("700")]
	_700,
	[Description("1000")]
	_1000,
	[Description("2000")]
	_2000,
	[Description("3000")]
	_3000,
	[Description("4000")]
	_4000,
	[Description("10000")]
	_10000,
}

public enum LogoLuminance
{
	off,
	[Description("Low")]
	light,
	[Description("High")]
	strong
}