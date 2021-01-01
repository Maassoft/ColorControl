# ColorControl
Easily change NVIDIA display settings and/or control LG TV's

If you own a NVIDIA graphics card, this app allows you not to only adjust basic display settings, but some hidden settings as well:

![Screenshot1](https://github.com/Maassoft/ColorControl/blob/master/images/NvPresets.png)

And if you own a recent LG TV, you can control your TV through the app (no NVIDIA graphics card needed):

![Screenshot2](https://github.com/Maassoft/ColorControl/blob/master/images/LgController.png)

For this to work correctly:
Check if the TV is listed in Windows Device Manager (Win+X -> Device Manager) under Digital Media Devices. If not then add the TV using Settings (Win+I) -> "Devices" -> "Add Bluetooth or other device" -> "Everything Else", then select your TV by name. It should now appear in Device Manager. (If your TV is not shown when adding devices then your PC is unable to see the TV on the network, check your network settings on both the PC & TV)
NOTE: You may have to add the TV as a device more than once before it appears in Device Manager, as Windows can detect the TV as multiple devices.

If you receive WinPcap errors download and install Npcap (https://nmap.org/npcap/#download) in WinPcap compatibility mode. WinPcap is depreciated under windows 10.

On the Options-tabpage you can finetune some parameters and/or enable some settings:

![Screenshot3](https://github.com/Maassoft/ColorControl/blob/master/images/Options.png)
