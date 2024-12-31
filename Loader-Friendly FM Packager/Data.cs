using System.Text;
using JetBrains.Annotations;

namespace Loader_Friendly_FM_Packager;

[PublicAPI]
public enum CompressionMethod
{
    LZMA,
    LZMA2,
}

public enum SevenZipApp
{
    Internal,
    External,
}

public sealed class ConfigData
{
    private int _compressionLevel = 9;
    public int CompressionLevel
    {
        get => _compressionLevel;
        set => _compressionLevel = value.Clamp(0, 9);
    }

    public CompressionMethod CompressionMethod = CompressionMethod.LZMA2;

    public SevenZipApp SevenZipApp = SevenZipApp.Internal;

    public string SevenZipExternalAppPath = "";
}

public static class Global
{
    public static readonly UTF8Encoding UTF8NoBOM = new(false, true);

    // TODO: We should probably just always use LZMA2, I don't see any disadvantage...
    public static readonly string[] CompressionMethodArgStrings =
    {
        "LZMA2",
        "LZMA",
    };

    public static readonly ConfigData Config = new();
}
