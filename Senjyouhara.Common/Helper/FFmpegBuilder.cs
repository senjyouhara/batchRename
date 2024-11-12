using System;

namespace Senjyouhara.Common.Helper;

public class FFmpegBuilder
{

    public enum PresetEnum
    {
        veryslow,
        slower,
        slow,
        medium,
        fast,
        faster,
        veryfast,
    }
    public enum ColorSpaceEnum
    {
        yuv420p,
        yuv422p,
    }
    public enum AudioEncoderEnum
    {
        aac,
        flac,
        copy
    }
    public enum VideoEncoderEnum
    {
        libx264,
        libx265,
        mpeg4video,
        copy
    }
    public enum VideoEncoderProfileEnum
    {
        high,
        main,
        baseline
    }
    
    public FFmpegConfig config = new ();
    
    public FFmpegBuilder Input(string filePath)
    {
        config.Input = $"-i {filePath}";
        return this;
    }    
    
    public FFmpegBuilder Output(string filePath)
    {
        config.Output = $"{filePath}";
        return this;
    }    
    
    public FFmpegBuilder Filter(string filter)
    {
        config.Filter = $"-filter_complex {filter}";
        return this;
    }
    
    public FFmpegBuilder Preset(PresetEnum preset)
    {
        config.Preset = $"-preset {Enum.GetName(typeof(PresetEnum), preset)}";
        return this;
    }
    
    public FFmpegBuilder VideoEncoder(string videoEncoder)
    {
        config.VideoEncoder = $"-codec:v {videoEncoder}";
        return this;
    }
    
    public FFmpegBuilder VideoEncoder(VideoEncoderProfileEnum videoEncoderProfile)
    {
        config.VideoEncoderProfile = $"-profile:v {Enum.GetName(typeof(VideoEncoderProfileEnum), videoEncoderProfile)}";
        return this;
    }
    
    public FFmpegBuilder AudioEncoder(AudioEncoderEnum audioEncoder)
    {
        config.AudioEncoder = $"-codec:a {Enum.GetName(typeof(AudioEncoderEnum), audioEncoder)}";
        return this;
    }
    
    public FFmpegBuilder AudioCodeRate(string audioCodeRate)
    {
        config.AudioCodeRate = $"-b:a {audioCodeRate}";
        return this;
    }
    
    public FFmpegBuilder Level(string level = "4.1")
    {
        config.Level = $"-level:v {level}";
        return this;
    }
    
    public FFmpegBuilder Crf(int crf)
    {
        config.Crf = $" -crf {crf}";
        return this;
    }
    
    public FFmpegBuilder ColorSpace(ColorSpaceEnum colorSpace)
    {
        config.ColorSpace = $"-pix_fmt {Enum.GetName(typeof(ColorSpaceEnum), colorSpace)}";
        return this;
    }
    
    
    
}