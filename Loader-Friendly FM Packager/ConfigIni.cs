using System.IO;
using System.Reflection;

namespace Loader_Friendly_FM_Packager;

internal static class ConfigIni
{
    internal static void ReadConfigData()
    {
        if (!File.Exists(Paths.ConfigFile))
        {
            return;
        }

        const BindingFlags _bFlagsEnum =
        BindingFlags.Instance |
        BindingFlags.Static |
        BindingFlags.Public |
        BindingFlags.NonPublic;

        string[] lines = File.ReadAllLines(Paths.ConfigFile);
        for (int i = 0; i < lines.Length; i++)
        {
            string lineT = lines[i].Trim();
            if (lineT.TryGetValueO("CompressionLevel=", out string value))
            {
                if (int.TryParse(value, out int result))
                {
                    Config.CompressionLevel = result;
                }
            }
            else if (lineT.TryGetValueO("CompressionMethod=", out value))
            {
                FieldInfo? field = typeof(CompressionMethod).GetField(value, _bFlagsEnum);
                if (field != null)
                {
                    Config.CompressionMethod = (CompressionMethod)field.GetValue(null);
                }
            }
            else if (lineT.TryGetValueO("Threads=", out value))
            {
                if (int.TryParse(value, out int result))
                {
                    Config.Threads = result;
                }
            }
            else if (lineT.TryGetValueO("DictionarySize=", out value))
            {
                if (long.TryParse(value, out long result))
                {
                    Config.DictionarySize = result;
                }
            }
            else if (lineT.TryGetValueO("MemoryUsageForCompressing=", out value))
            {
                bool isPercent = false;
                if (value.Length > 0 && value[^1] == '%')
                {
                    isPercent = true;
                    value = value.TrimEnd('%');
                }
                if (long.TryParse(value, out long result))
                {
                    Config.MemoryUseForCompression = new MemoryUseItem(result, isPercent);
                }
            }
        }

        return;

        static bool? GetNullableBoolValue(string value)
        {
            if (value == bool.TrueString)
            {
                return true;
            }
            else if (value == bool.FalseString)
            {
                return false;
            }
            else
            {
                return null;
            }
        }
    }

    internal static void WriteConfigData()
    {
        using var sw = new StreamWriter(Paths.ConfigFile);
        sw.WriteLine("CompressionLevel=" + Config.CompressionLevel.ToStrInv());
        sw.WriteLine("CompressionMethod=" + Config.CompressionMethod);
        sw.WriteLine("Threads=" + Config.Threads.ToStrInv());
        sw.WriteLine("DictionarySize=" + Config.DictionarySize.ToStrInv());
        sw.WriteLine("MemoryUsageForCompressing=" +
                     (Config.MemoryUseForCompression.IsPercent && Config.MemoryUseForCompression.Value != -1
                         ? Config.MemoryUseForCompression.Value + "%"
                         : Config.MemoryUseForCompression.Value));
        return;

        static string GetNullableBoolValue(bool? value) => value switch
        {
            true => bool.TrueString,
            false => bool.FalseString,
            _ => "",
        };
    }
}
