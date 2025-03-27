using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using NAudio.CoreAudioApi;

class DiscordOutputMuter
{
    [DllImport("user32.dll")] static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    [DllImport("kernel32.dll")] static extern IntPtr GetConsoleWindow();
    const int SW_HIDE = 0;

    static void Main()
    {
        ShowWindow(GetConsoleWindow(), SW_HIDE);

        string[] targetDevices =
        {
            "Headphones (Arctis Nova Pro)",
            "SteelSeries Sonar - Microphone (SteelSeries Sonar Virtual Audio Device)"
        };

        bool hMuted = false;
        bool mMuted = false;

        while (!hMuted || !mMuted)
        {
            Thread.Sleep(1000); 

            try
            {
                using (MMDeviceEnumerator enumerator = new MMDeviceEnumerator())
                {
                    if (!hMuted)
                    {
                        var headphoneDevice = GetDevice(enumerator, targetDevices[0], DataFlow.Render);
                        if (headphoneDevice != null && MuteDiscord(headphoneDevice))
                        {
                            hMuted = true;
                        }
                    }

                    if (!mMuted)
                    {
                        var micDevice = GetDevice(enumerator, targetDevices[1], DataFlow.Render);
                        if (micDevice != null && MuteDiscord(micDevice))
                        {
                            mMuted = true;
                        }
                    }
                }
            }
            catch {  }
        }
    }

    static bool MuteDiscord(MMDevice device)
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
                catch {  }
            }
        }
        catch {  }
        return muted;
    }

    static MMDevice GetDevice(MMDeviceEnumerator enumerator, string deviceName, DataFlow flow)
    {
        try
        {
            foreach (MMDevice device in enumerator.EnumerateAudioEndPoints(flow, DeviceState.Active))
            {
                if (device.FriendlyName.Trim().Equals(deviceName, StringComparison.OrdinalIgnoreCase))
                {
                    return device;
                }
            }
        }
        catch { }
        return null;
    }
}
