# Fix-discord-echo
This tool fixes the echo issue that occurs when using SteelSeries Sonar during Discord screen sharing.


# How It Works:
The app monitors the Windows Volume Mixer.

When Discord appears while using the specified devices, it automatically mutes Discord.

The script closes itself after muting, keeping your system clean.

# Installation:
Download the executable or compile the C# code using Visual Studio.

(Optional) Run on Startup:

Press Win + R, type shell:startup.

Create a shortcut and set the path to the .exe file.


# Requirements to Build
.NET 8.0 SDK (Might work on older versions, but not tested)

CoreAudio.dll (Included with the NAudio package)

Install via NuGet: Install-Package NAudio
