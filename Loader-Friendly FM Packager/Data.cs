using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace Loader_Friendly_FM_Packager;

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

    public const long DefaultDictionarySize = ByteSize.MB * 256;

    private int _threads;
    public int Threads
    {
        get => _threads;
        set
        {
            if (value == -1)
            {
                _threads = -1;
            }
            else
            {
                int max = CompressionMethod == CompressionMethod.LZMA2 ? CPUThreads * 2 : Math.Min(CPUThreads, 2);
                _threads = value.Clamp(0, max);
            }
        }
    }

    private long _dictionarySize;
    public long DictionarySize
    {
        get => _dictionarySize;
        set =>
            _dictionarySize = value == -1
                ? -1
                : DictionarySizeItems.FirstOrDefault_PastFirstIndex(x =>
                    x.BackingValue == value)?.BackingValue ?? DefaultDictionarySize;
    }

    private MemoryUseItem _memoryUseForCompression = MemoryUseItem.Default;
    public MemoryUseItem MemoryUseForCompression
    {
        get => _memoryUseForCompression;
        set => _memoryUseForCompression = value.Value == -1 ? MemoryUseItem.Default : value;
    }

    public bool? StoreCreationTime;
    public bool? StoreLastAccessTime;
    public bool? SetArchiveTimeToLatestTileTime;
    public bool DoNotChangeSourceFilesLastAccessTime;
}

public readonly struct MemoryUseItem
{
    public readonly long Value;
    public readonly bool IsPercent;

    public static MemoryUseItem Default => new(-1, true);

    public MemoryUseItem(long value, bool isPercent)
    {
        Value = value;
        IsPercent = isPercent;
    }
}

public sealed class FriendlyStringAndBackingValue<TBacking>
{
    public readonly string FriendlyString;
    public readonly TBacking BackingValue;

    public FriendlyStringAndBackingValue(string friendlyString, TBacking backingValue)
    {
        FriendlyString = friendlyString;
        BackingValue = backingValue;
    }
}

public static class Global
{
    public static readonly int CPUThreads = Environment.ProcessorCount;

    public static object[] ToFriendlyStrings<T>(this FriendlyStringAndBackingValue<T>[] items)
    {
        object[] ret = new object[items.Length];
        for (int i = 0; i < items.Length; i++)
        {
            ret[i] = items[i].FriendlyString;
        }
        return ret;
    }

    public static class ByteSize
    {
        public const long KB = 1024;
        public const long MB = KB * 1024;
        public const long GB = MB * 1024;
    }

    [PublicAPI]
    public enum CompressionMethod
    {
        LZMA2,
        LZMA,
    }

    public enum SevenZipApp
    {
        Internal,
        External,
    }

    public static readonly UTF8Encoding UTF8NoBOM = new(false, true);

    // TODO: We should probably just always use LZMA2, I don't see any disadvantage...
    public static readonly string[] CompressionMethodArgStrings =
    {
        "LZMA2",
        "LZMA",
    };

    public static readonly ConfigData Config = new();

    private static FriendlyStringAndBackingValue<int>[] FillLzma2ThreadItems()
    {
        if (CPUThreads < 1)
        {
            return Array.Empty<FriendlyStringAndBackingValue<int>>();
        }

        int count = (CPUThreads * 2) + 1;

        FriendlyStringAndBackingValue<int>[] ret = new FriendlyStringAndBackingValue<int>[count];
        ret[0] = new FriendlyStringAndBackingValue<int>("* " + CPUThreads.ToStrInv(), CPUThreads);
        for (int i = 1; i < count; i++)
        {
            ret[i] = new FriendlyStringAndBackingValue<int>(i.ToStrInv(), i);
        }

        Trace.WriteLine(nameof(FillLzma2ThreadItems));
        for (int i = 0; i < ret.Length; i++)
        {
            var item = ret[i];
            Trace.WriteLine(item?.FriendlyString ?? "<null at " + i + ">");
        }

        return ret;
    }

    private static FriendlyStringAndBackingValue<int>[] FillLzmaThreadItems()
    {
        if (CPUThreads < 1)
        {
            return Array.Empty<FriendlyStringAndBackingValue<int>>();
        }

        int threads = Math.Min(CPUThreads, 2);

        int count = threads + 1;

        FriendlyStringAndBackingValue<int>[] ret = new FriendlyStringAndBackingValue<int>[count];
        ret[0] = new FriendlyStringAndBackingValue<int>("* " + threads, CPUThreads);
        for (int i = 1; i < count; i++)
        {
            ret[i] = new FriendlyStringAndBackingValue<int>(i.ToStrInv(), i);
        }

        Trace.WriteLine(nameof(FillLzmaThreadItems));
        for (int i = 0; i < ret.Length; i++)
        {
            var item = ret[i];
            Trace.WriteLine(item?.FriendlyString ?? "<null at " + i + ">");
        }

        return ret;
    }

    public static readonly FriendlyStringAndBackingValue<int>[] Lzma2ThreadItems = FillLzma2ThreadItems();
    public static readonly FriendlyStringAndBackingValue<int>[] LzmaThreadItems = FillLzmaThreadItems();

    public static readonly FriendlyStringAndBackingValue<long>[] DictionarySizeItems =
    {
        new("* 256 MB", ByteSize.MB * 256),
        new("64 KB", ByteSize.KB * 64),
        new("256 KB", ByteSize.KB * 256),
        new("1 MB", ByteSize.MB * 1),
        new("2 MB", ByteSize.MB * 2),
        new("3 MB", ByteSize.MB * 3),
        new("4 MB", ByteSize.MB * 4),
        new("5 MB", ByteSize.MB * 5),
        new("6 MB", ByteSize.MB * 6),
        new("8 MB", ByteSize.MB * 8),
        new("12 MB", ByteSize.MB * 12),
        new("16 MB", ByteSize.MB * 16),
        new("24 MB", ByteSize.MB * 24),
        new("32 MB", ByteSize.MB * 32),
        new("48 MB", ByteSize.MB * 48),
        new("64 MB", ByteSize.MB * 64),
        new("96 MB", ByteSize.MB * 96),
        new("128 MB", ByteSize.MB * 128),
        new("192 MB", ByteSize.MB * 192),
        new("256 MB", ByteSize.MB * 256),
        new("384 MB", ByteSize.MB * 384),
        new("512 MB", ByteSize.MB * 512),
        new("768 MB", ByteSize.MB * 768),
        new("1024 MB", ByteSize.MB * 1024),
        new("1536 MB", ByteSize.MB * 1536),
        new("2048 MB", ByteSize.MB * 2048),
        new("3840 MB", ByteSize.MB * 3840),
    };

    public static readonly FriendlyStringAndBackingValue<MemoryUseItem>[] MemoryUseItems =
    {
        new("* 80%", new MemoryUseItem(80, true)),
        new("10%", new MemoryUseItem(10, true)),
        new("20%", new MemoryUseItem(20, true)),
        new("30%", new MemoryUseItem(30, true)),
        new("40%", new MemoryUseItem(40, true)),
        new("50%", new MemoryUseItem(50, true)),
        new("60%", new MemoryUseItem(60, true)),
        new("70%", new MemoryUseItem(70, true)),
        new("80%", new MemoryUseItem(80, true)),
        new("90%", new MemoryUseItem(90, true)),
        new("100%", new MemoryUseItem(100, true)),
        new("256 MB", new MemoryUseItem(ByteSize.MB * 256, false)),
        new("384 MB", new MemoryUseItem(ByteSize.MB * 384, false)),
        new("512 MB", new MemoryUseItem(ByteSize.MB * 512, false)),
        new("768 MB", new MemoryUseItem(ByteSize.MB * 768, false)),
        new("1024 MB", new MemoryUseItem(ByteSize.MB * 1024, false)),
        new("1536 MB", new MemoryUseItem(ByteSize.MB * 1536, false)),
        new("2 GB", new MemoryUseItem(ByteSize.GB * 2, false)),
        new("3 GB", new MemoryUseItem(ByteSize.GB * 3, false)),
        new("4 GB", new MemoryUseItem(ByteSize.GB * 4, false)),
        new("6 GB", new MemoryUseItem(ByteSize.GB * 6, false)),
        new("8 GB", new MemoryUseItem(ByteSize.GB * 8, false)),
        new("12 GB", new MemoryUseItem(ByteSize.GB * 12, false)),
        new("16 GB", new MemoryUseItem(ByteSize.GB * 16, false)),
        new("24 GB", new MemoryUseItem(ByteSize.GB * 24, false)),
        new("32 GB", new MemoryUseItem(ByteSize.GB * 32, false)),
        new("48 GB", new MemoryUseItem(ByteSize.GB * 48, false)),
        new("64 GB", new MemoryUseItem(ByteSize.GB * 64, false)),
        new("96 GB", new MemoryUseItem(ByteSize.GB * 96, false)),
        new("128 GB", new MemoryUseItem(ByteSize.GB * 128, false)),
        new("192 GB", new MemoryUseItem(ByteSize.GB * 192, false)),
        new("256 GB", new MemoryUseItem(ByteSize.GB * 256, false)),
        new("384 GB", new MemoryUseItem(ByteSize.GB * 384, false)),
        new("512 GB", new MemoryUseItem(ByteSize.GB * 512, false)),
        new("768 GB", new MemoryUseItem(ByteSize.GB * 768, false)),
        new("1024 GB", new MemoryUseItem(ByteSize.GB * 1024, false)),
        new("1536 GB", new MemoryUseItem(ByteSize.GB * 1536, false)),
        new("2048 GB", new MemoryUseItem(ByteSize.GB * 2048, false)),
        new("3072 GB", new MemoryUseItem(ByteSize.GB * 3072, false)),
        new("4096 GB", new MemoryUseItem(ByteSize.GB * 4096, false)),
        new("6144 GB", new MemoryUseItem(ByteSize.GB * 6144, false)),
        new("8192 GB", new MemoryUseItem(ByteSize.GB * 8192, false)),
        new("12288 GB", new MemoryUseItem(ByteSize.GB * 12288, false)),
        new("16384 GB", new MemoryUseItem(ByteSize.GB * 16384, false)),
    };

    public static readonly FriendlyStringAndBackingValue<CompressionMethod>[] CompressionMethodItems =
    {
        new("* LZMA2", CompressionMethod.LZMA2),
        new("LZMA", CompressionMethod.LZMA),
    };
}
