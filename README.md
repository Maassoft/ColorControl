# ColorControl
Easily change NVIDIA and AMD display settings and/or control LG TV's

## NVIDIA/AMD controller

If you own a NVIDIA or AMD graphics card, this app allows you not to only adjust basic display settings, but some hidden settings as well.
For both NVIDIA and AMD graphics cards, you can configure your own presets to change the color depth (6 to 16 bpc), color format (RGB/YUV), refresh rate, dithering and HDR setting. You can assign a global keyboard shortcut to each preset to change the display settings (and HDR!) with just a couple of key presses.
The NVIDIA controller even has some more options:
* Dynamic range: VESA (Full RGB) or CEA (Limited RGB/YUV)
* Color space: to change the color space, but most may not be supported by your tv
* Dithering: you can define the dithering mode (Temporal or Spatial) and the dithering bit depth

Notes:
* For a specific setting to be applied, you must include it within the preset. Just click the "Include" menu item and once it is checked, you're good to go.

Screenshot:
![image](https://user-images.githubusercontent.com/70057942/132135723-688a6177-1906-4941-b92e-e456d71594b0.png)

## LG controller

If you own a recent LG TV (2018 or newer, older might work), you can control your TV through the app (no NVIDIA or AMD graphics card needed).
At startup of the application it will automatically detect your tv's (see below) if they are on the same network as your pc. If a tv is powered on, it will show a popup by which you can allow ColorControl to send commands to you tv. This will only happen the first time or whenever there's a change in the required permissions. A new version of ColorControl might need this.
It is also possible to add a tv manually by using the "Add" button. A name and ip address are required, the MAC address is only necessary for Wake-On-Lan.
You can configure as well when to automatically power your tv on or off:
* Power on after startup of pc
* Power on after resume
* Power off on shutdown
* Power off on standby
* Power off on screensaver and on when screensaver deactivates

### Presets

With the presets you can peform actions on your tv you would normally do via the remote control. Properties of a preset:
* Name: fill in your own name/description
* Device: select the tv to perform the action on. Defaults to "Globally selected device", which is the selected device in the top devices drop down.
* App: select the app to launch that is installed on your tv (optional)
* Shortcut: enter the global shortcut to execute this preset
* Steps: steps to execute sequentially. These steps can be:
  * Remote control buttons: like RIGHT, LEFT, ENTER, etc.
  * Actions: directly change picture settings like backlight, contrast, pictureMode, etc. In a dialog you have to specify the value.
  * NVIDIA/AMD presets: add NVIDIA or AMD presets here that have to execute as well

Furthermore, you can add a trigger to a preset which means it will execute automatically when a process on your pc is running. See for more information: https://github.com/Maassoft/ColorControl/releases/tag/v4.0.0.0

Screenshot:
![image](https://user-images.githubusercontent.com/70057942/132136067-1a2c205d-a241-4bf2-8d77-550b31606727.png)

### Auto detecting your tv
Check if the TV is listed in Windows Device Manager (Win+X -> Device Manager) under Digital Media Devices. If not then add the TV using Settings (Win+I) -> "Devices" -> "Add Bluetooth or other device" -> "Everything Else", then select your TV by name. It should now appear in Device Manager. (If your TV is not shown when adding devices then your PC is unable to see the TV on the network, check your network settings on both the PC & TV)
NOTE: You may have to add the TV as a device more than once before it appears in Device Manager, as Windows can detect the TV as multiple devices.

WinPcap is no longer used by default, but if you receive WinPcap errors, download and install Npcap (https://nmap.org/npcap/#download) in WinPcap compatibility mode. WinPcap is depreciated under windows 10.

On the Options-tabpage you can finetune some parameters and/or enable some settings:

![Screenshot3](https://github.com/Maassoft/ColorControl/blob/master/images/Options.png)
