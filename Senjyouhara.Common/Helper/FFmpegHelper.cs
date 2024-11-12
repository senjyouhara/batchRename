using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Senjyouhara.Common.Helper;

public class FFmpegHelper
{
    public static string CommandPath { private get; set; } = "";

    public static List<string> Builder(FFmpegBuilder builder)
    {
        List<string> commands = new();
        return commands;
    }
    
    public static void Builder(FFmpegBuilder builder, Action<string> action)
    {
        List<string> commands = new();
        var param = string.Join(" ", commands);
        var filepath = (CommandPath + " " +  param );
        
        System.Diagnostics.Process process = new System.Diagnostics.Process();
        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
        startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
        startInfo.FileName = "cmd.exe";
        startInfo.Arguments = filepath;
        process.StartInfo = startInfo;
        process.Start();
        process.OutputDataReceived += (sender, args) =>
        {
            action(args.Data);
        };
        process.WaitForExit();
    }

}