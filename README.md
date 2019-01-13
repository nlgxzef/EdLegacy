Download: https://github.com/nlgzrgn/Ed/releases/
------------------------------------------------------------------------------------------------------------
Description:

Ed is a tool which makes adding cars easier for your favorite racing game!
Ed only supports Need for Speed: Carbon for now. Please don't ask support for other games. They may or may not be added over time.

Note: To make your add-on cars work properly, you will need either one of these .asi mods:
- NFSC Unlimiter: https://nfsmods.xyz/mod/846
- NFSC Car Array Patch by sparx: https://nfsmods.xyz/mod/834
! Don't use both mods at the same time. Only use the one which works better for you.

The tool is pretty straightforward and it's made for last user. So, anyone who knows English can easily use it.

Folders:
Config\<Game Name>: The folder used to store .ini config files. Copy your config files here.
Resources\<Game Name>\FrontEnd\FrontEndTextures\8D08770D: Textures folder for FrontB.lzc. Copy the textures (CARSELECT_MANUFACTURER_x and SECONDARY_LOGO_x) you want to add into FrontB.lzc here.
Temp\<Game Name>: Temporary folder which will get created when Ed is working. Use Keep Temporary Files option to prevent Ed from deleting them.
! It's not suggested to make changes on other files in these folders unless you're a professional.

------------------------------------------------------------------------------------------------------------
Installation and usage:

! Before installation, make sure that you have installed .NET Framework 4.5 or higher.

Now you can use Ed. Just follow these steps:

1) Copy the files anywhere on your PC.
2) Run "Ed.exe" as administrator.
3) Press Ctrl+O or click Main > Open to select your game installation directory.
Brotip: It's usually in C:\Program Files(x86)\Electronic Arts\Need for Speed Carbon.
4) If the game is detected, you will see the folder path under the Config List view.
5) Config files for the game will be listed on the screen. If you don't see any, press Ctrl+B or click Main > Browse Config Folder... to copy them in. Then, press F5 to refresh the view.
Brotip: Also see the Example Config File.ini.
6) Now press Alt+A, click "Add Cars" button or click Tools > Add Cars from Config File(s).
7) Ed now should do everything required to add the cars you want.
8) If the "New cars added successfully." dialog appears, it means that Ed completed his job.
9) Now add the VLT entries required using nfsu360's NFS-VltEd. (Author of the car should have added a .nfsms with it, or you can do it manually.)
10) Run the game, create a new save game and enjoy your new car!

IMPORTANT: This is a pre-release! If you find any issues, feel free to report them on #ed-issues channel (including your Ed.log file) on Extra Options Discord Server! (https://discord.gg/EfgtTQb)  
MORE IMPORTANT: It's suggested to use Tools > Unlock Game Files For Modding if it's your first time to mod your game, or if you have any issues.  

------------------------------------------------------------------------------------------------------------
Changelog: (+ Addition, * Change, ! Attention, - Deletion)

v1.0.0.403 (Build 1; Rev.00 BETA) :
* Fixed an issue with Car Part Info when users try to add more than 1 car.

v1.0.0.401 (Build 1; Rev.00 BETA) :
* Fixed an issue with Car Info Array when users try to add more than 1 car.
* Fixed an issue which makes added textures invisible.

v1.0.0.400 (Build 1; Rev.00 BETA) :
+ Initial release.
------------------------------------------------------------------------------------------------------------
Credits:

Coded by:
- nlgzrgn

Thanks to:
- ArturoPlayerOne and 379Felipe for their successful attempts on adding cars.
- MWInside for ReCompiler and inspiration.
- 379Felipe for testing, resources and tutorials.
- heyitsleo for resources and their great help.
- Xanvier for XNFSTPKTool.
- Improvement Mod Team (379Felipe, GXP-10, Neon, RaTT, sparx, SpeedyHeart and Xanvier) for their great help.
- And anyone I forgot to write here...
------------------------------------------------------------------------------------------------------------

See ya!
©2019 nlgzrgn @ ExOpts Team - No rights reserved. ;)
