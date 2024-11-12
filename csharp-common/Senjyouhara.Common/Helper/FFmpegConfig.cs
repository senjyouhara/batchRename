namespace Senjyouhara.Common.Helper;


public class FFmpegConfig
{
    // 输入文件
    public string Input { get; set; }
    // 输出文件
    public string Output { get; set; }
    // 过滤器
    public string Filter { get; set; }
    // 预置处理
    public string Preset { get; set; }
    // 视频编码器
    public string VideoEncoder { get; set; }
    
    // 指定编码器配置，主要和压缩比有关
    public string VideoEncoderProfile { get; set; }
    // 音频编码器
    public string AudioEncoder { get; set; }
    // 音频码率
    public string AudioCodeRate { get; set; }
    
    // 对编码器配置的限制
    public string Level { get; set; }
    // 码率控制 
    public string Crf { get; set; }
    // 色彩空间
    public string ColorSpace { get; set; }
}