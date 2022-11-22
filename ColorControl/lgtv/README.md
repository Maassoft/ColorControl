# lgtv.net
LG TV WebOS API for .NET

For now, there is only API, example app will be added in the future.

## inspiration: 
* https://github.com/msloth/lgtv.js/blob/master/lgtv.js

## references:
* https://github.com/ConnectSDK/Connect-SDK-Android-Core
* https://github.com/CODeRUS/harbour-lgremote-webos
* https://github.com/openwebos
* https://mym.hackpad.com/ep/pad/static/rLlshKkzdNj

## Usage
```C#
 // Initalization
    var _instance =  new LgTvApi(ip,new LgTvApiCore(), new ClientKeyStore(ip));
    await _instance.Connect();
    await _instance.MakeHandShake();
    await _instance.GetMouse();

//control
    await _instance.VolumeDown();
    await _instance.TurnOff();
    ......
    (await _instance.GetMouse()).SendButton(ButtonType.RED);
    (await _instance.GetMouse()).SendButton(ButtonType.LEFT);
    ..
```
