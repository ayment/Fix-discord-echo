using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using NAudio.CoreAudioApi;

class DiscordOutputMuter
{
    [DllImport("user32.dll")] static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    [DllImport("kernel32.dll")] static extern IntPtr GetConsoleWindow();
    const int SW_HIDE = 0;

    static void Main()
    {
        ShowWindow(GetConsoleWindow(), SW_HIDE);

        string[] Devices =
        {
            "Headphones (Arctis Nova Pro)",
            "SteelSeries Sonar - Microphone (SteelSeries Sonar Virtual Audio Device)"
        };

        try
        {
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            bool muted = false;

            foreach (var deviceName in Devices)
            {
                MMDevice device = Devicething(enumerator, deviceName);
                if (device != null)
                {
                    if (Mute(device)) muted = true;
                }
            }

            if (muted) Environment.Exit(0);
        }
        catch { /*idk*/ }
    }

    static bool Mute(MMDevice device)
    {
        bool muted = false;
        try
        {
            var sessions = device.AudioSessionManager.Sessions;
            for (int i = 0; i < sessions.Count; i++)
            {
                var session = sessions[i];
                if (session.GetProcessID == 0) continue;

                try
                {
                    using Process process = Process.GetProcessById((int)session.GetProcessID);
                    if (process.ProcessName.Equals("Discord", StringComparison.OrdinalIgnoreCase))
                    {
                        session.SimpleAudioVolume.Mute = true;
                        muted = true;
                    }
                }
                catch { /*idk*/ }
            }
        }
        catch { /*idk*/ }
        return muted;
    }

    static MMDevice Devicething(MMDeviceEnumerator enumerator, string targetName)
    {
        try
        {
            foreach (MMDevice device in enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
            {
                if (device.FriendlyName.Trim().Equals(targetName, StringComparison.OrdinalIgnoreCase))
                {
                    return device;
                }
            }
        }
        catch { /*idk*/ }
        return null;
    }
}