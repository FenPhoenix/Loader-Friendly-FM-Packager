﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using JetBrains.Annotations;

namespace Loader_Friendly_FM_Packager;

public static class Fen7z
{
    [PublicAPI]
    public sealed class ProgressReport
    {
        public int PercentOfBytes;
        public int PercentOfEntries;
        public int CompressPercent;
        public bool Canceling;
    }

    public enum SevenZipExitCode
    {
        NoError = 0,
        /// <summary>
        /// Warning (Non fatal error(s)). For example, one or more files were locked by some other application,
        /// so they were not compressed.
        /// </summary>
        Warning = 1,
        FatalError = 2,
        CommandLineError = 7,
        /// <summary>
        /// Not enough memory for the operation.
        /// </summary>
        NotEnoughMemory = 8,
        /// <summary>
        /// User stopped the process.
        /// </summary>
        UserStopped = 255,
        Unknown = int.MaxValue,
    }

    [PublicAPI]
    public sealed class Result
    {
        public bool ErrorOccurred =>
            !Canceled &&
            (Exception != null
             || (ExitCode != SevenZipExitCode.NoError && ExitCode != SevenZipExitCode.UserStopped)
             || (ExitCodeInt != null && ExitCodeInt != 0 && ExitCodeInt != 255));

        public readonly string ErrorText;
        public readonly Exception? Exception;
        public readonly SevenZipExitCode ExitCode;
        public readonly int? ExitCodeInt;
        public readonly bool Canceled;

        public Result(Exception exception, string errorText)
        {
            Exception = exception;
            ErrorText = errorText;
            ExitCode = SevenZipExitCode.NoError;
        }

        public Result(Exception exception, string errorText, SevenZipExitCode exitCode, bool canceled)
        {
            Exception = exception;
            ErrorText = errorText;
            ExitCode = exitCode;
            Canceled = canceled;
        }

        public Result(Exception? exception, string errorText, SevenZipExitCode exitCode, int? exitCodeInt, bool canceled)
        {
            Exception = exception;
            ErrorText = errorText;
            ExitCode = exitCode;
            ExitCodeInt = exitCodeInt;
            Canceled = canceled;
        }

        public override string ToString() =>
            ErrorOccurred
                ? $"Error in 7z.exe extraction:{NL}"
                  + ErrorText + $"{NL}"
                  + (Exception?.ToString() ?? "") + $"{NL}"
                  + "ExitCode: " + ExitCode + $"{NL}"
                  + "ExitCodeInt: " + (ExitCodeInt?.ToString() ?? "")
                : $"No error.{NL}"
                  + "Canceled: " + Canceled + $"{NL}"
                  + "ExitCode: " + ExitCode + $"{NL}"
                  + "ExitCodeInt: " + (ExitCodeInt?.ToString() ?? "");
    }

    /// <summary>
    /// Extract a .7z file wholly or partially.
    /// </summary>
    /// <param name="sevenZipPathAndExe"></param>
    /// <param name="archivePath"></param>
    /// <param name="outputPath"></param>
    /// <param name="entriesCount">Only used if <paramref name="progress"/> is provided (non-null).</param>
    /// <param name="listFile"></param>
    /// <param name="fileNamesList"></param>
    /// <param name="progress"></param>
    /// <returns></returns>
    public static Result Extract(
        string sevenZipPathAndExe,
        string archivePath,
        string outputPath,
        int entriesCount = 0,
        string listFile = "",
        List<string>? fileNamesList = null,
        IProgress<ProgressReport>? progress = null)
    {
        return Extract(
            sevenZipPathAndExe: sevenZipPathAndExe,
            archivePath: archivePath,
            outputPath: outputPath,
            cancellationToken: CancellationToken.None,
            entriesCount: entriesCount,
            listFile: listFile,
            fileNamesList: fileNamesList,
            progress: progress);
    }

    /// <summary>
    /// Extract a .7z file wholly or partially.
    /// </summary>
    /// <param name="sevenZipPathAndExe"></param>
    /// <param name="archivePath"></param>
    /// <param name="outputPath"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="entriesCount">Only used if <paramref name="progress"/> is provided (non-null).</param>
    /// <param name="listFile"></param>
    /// <param name="fileNamesList"></param>
    /// <param name="progress"></param>
    /// <returns></returns>
    public static Result Extract(
        string sevenZipPathAndExe,
        string archivePath,
        string outputPath,
        CancellationToken cancellationToken,
        int entriesCount = 0,
        string listFile = "",
        List<string>? fileNamesList = null,
        IProgress<ProgressReport>? progress = null)
    {
        bool selectiveFiles = !listFile.IsWhiteSpace() && fileNamesList?.Count > 0;

        if (selectiveFiles && fileNamesList != null)
        {
            try
            {
                File.WriteAllLines(listFile, fileNamesList);
            }
            catch (Exception ex)
            {
                return new Result
                (
                    exception: ex,
                    errorText: "Exception trying to write the 7z.exe list file: " + listFile
                );
            }
        }

        bool canceled = false;

        string errorText = "";

        var report = new ProgressReport();

        var p = new Process { EnableRaisingEvents = true };
        try
        {
            p.StartInfo.FileName = sevenZipPathAndExe;
            p.StartInfo.WorkingDirectory = outputPath;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            // x     = Extract with full paths
            // -aoa  = Overwrite all existing files without prompt
            // -y    = Say yes to all prompts automatically
            // -bsp1 = Redirect progress information to stdout stream
            // -bb1  = Show names of processed files in log (needed for smooth reporting of entries done count)
            p.StartInfo.Arguments =
                "x \"" + archivePath + "\" -o\"" + outputPath + "\" "
                + (selectiveFiles ? "@\"" + listFile + "\" " : "")
                + "-aoa -y -bsp1 -bb1";
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;

            p.OutputDataReceived += (sender, e) =>
            {
                var proc = (Process)sender;
                if (!canceled && cancellationToken.IsCancellationRequested)
                {
                    canceled = true;

                    report.Canceling = true;
                    progress?.Report(report);
                    try
                    {
                        proc.CancelErrorRead();
                        proc.CancelOutputRead();
                        /*
                        We should be sending Ctrl+C to it, but since that's apparently deep-level black
                        magic on Windows, we just kill it. We expect the caller to understand that the
                        extracted files will be in an indeterminate state, and to delete them or do whatever
                        it deems fit.
                        */
                        proc.Kill();
                    }
                    catch
                    {
                        // Ignore, it's going to throw but work anyway (even on non-admin, tested)
                    }
                    return;
                }

                if (e.Data.IsEmpty() || report.Canceling || progress == null) return;
                try
                {
                    string lineT = e.Data.Trim();

                    int pi = lineT.IndexOf('%');
                    if (pi > -1)
                    {
                        int di;
                        if (entriesCount > 0 &&
                            Utils.Int_TryParseInv((di = lineT.IndexOf('-', pi + 1)) > -1
                                ? lineT.Substring(pi + 1, di)
                                : lineT.Substring(pi + 1), out int entriesDone))
                        {
                            report.PercentOfEntries = Utils.GetPercentFromValue_Int(entriesDone, entriesCount).Clamp(0, 100);
                        }

                        if (Utils.Int_TryParseInv(lineT.Substring(0, pi), out int bytesPercent))
                        {
                            report.PercentOfBytes = bytesPercent;
                        }

                        progress.Report(report);
                    }
                }
                catch
                {
                    // ignore, it just means we won't report progress... meh
                }
            };

            p.ErrorDataReceived += (_, e) =>
            {
                if (!e.Data.IsWhiteSpace()) errorText += $"{NL}---" + e.Data;
            };

            p.Start();
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();

            p.WaitForExit();

            (SevenZipExitCode exitCode, int? exitCodeInt, Exception? exception) = GetExitCode(p);

            return new Result(exception, errorText, exitCode, exitCodeInt, canceled);
        }
        catch (Exception ex)
        {
            return new Result(ex, errorText, SevenZipExitCode.Unknown, canceled);
        }
        finally
        {
            EndProcess(p, selectiveFiles, listFile);
        }
    }

    public static Result Compress(
        string sevenZipPathAndExe,
        string sourcePath,
        string outputArchive,
        string args,
        CancellationToken cancellationToken,
        string listFile = "",
        IProgress<ProgressReport>? progress = null)
    {
        bool selectiveFiles = !listFile.IsWhiteSpace();

        bool canceled = false;

        string errorText = "";

        var report = new ProgressReport();

        var p = new Process { EnableRaisingEvents = true };
        try
        {
            p.StartInfo.FileName = sevenZipPathAndExe;
            p.StartInfo.WorkingDirectory = sourcePath;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.Arguments =
                "a " + args + " \"" +
                outputArchive + "\" " +
                (!listFile.IsEmpty()
                    ? "@\"" + listFile + "\""
                    // We have to say eg. "C:\FM_dir\*" not "C:\fm_dir", because the latter creates a subfolder
                    // within the archive.
                    : "\"" + sourcePath.TrimEnd('/', '\\') + "\\*\"");
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;

            p.OutputDataReceived += (sender, e) =>
            {
                var proc = (Process)sender;
                if (!canceled && cancellationToken.IsCancellationRequested)
                {
                    canceled = true;

                    report.Canceling = true;
                    progress?.Report(report);
                    try
                    {
                        proc.CancelErrorRead();
                        proc.CancelOutputRead();
                        /*
                        We should be sending Ctrl+C to it, but since that's apparently deep-level black
                        magic on Windows, we just kill it. We expect the caller to understand that the
                        extracted files will be in an indeterminate state, and to delete them or do whatever
                        it deems fit.
                        */
                        proc.Kill();
                    }
                    catch
                    {
                        // Ignore, it's going to throw but work anyway (even on non-admin, tested)
                    }
                    return;
                }

                if (e.Data.IsEmpty() || report.Canceling || progress == null) return;
                try
                {
                    string lineT = e.Data.Trim();

                    if (!lineT.StartsWithO("+"))
                    {
                        int pi = lineT.IndexOf('%');
                        if (pi > -1)
                        {
                            string percentStr = lineT.Substring(0, pi).Trim();
                            if (Utils.Int_TryParseInv(percentStr, out int percent))
                            {
                                report.CompressPercent = percent;
                                progress.Report(report);
                            }
                        }
                    }
                }
                catch
                {
                    // ignore, it just means we won't report progress... meh
                }
            };

            p.ErrorDataReceived += (_, e) =>
            {
                if (!e.Data.IsWhiteSpace()) errorText += $"{NL}---" + e.Data;
            };

            p.Start();
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();

            p.WaitForExit();

            (SevenZipExitCode exitCode, int? exitCodeInt, Exception? exception) = GetExitCode(p);

            return new Result(exception, errorText, exitCode, exitCodeInt, canceled);
        }
        catch (Exception ex)
        {
            return new Result(ex, errorText, SevenZipExitCode.Unknown, canceled);
        }
        finally
        {
            EndProcess(p, selectiveFiles, listFile);
        }
    }

    public static Result Rename(
        string sevenZipPathAndExe,
        string sourcePath,
        string outputArchive,
        string originalFileName,
        string newFileName,
        string args,
        CancellationToken cancellationToken,
        IProgress<ProgressReport>? progress = null)
    {
        bool canceled = false;

        string errorText = "";

        var report = new ProgressReport();

        var p = new Process { EnableRaisingEvents = true };
        try
        {
            p.StartInfo.FileName = sevenZipPathAndExe;
            p.StartInfo.WorkingDirectory = sourcePath;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.Arguments =
                // -scc = console i/o charset
                // Tested with 1252 "Rustung-u" and also Unicode copyright symbol, both get put into the archive
                // with correct encoding and renamed correctly.
                "rn " + args + " \"" + outputArchive + "\" " +
                "\"" + originalFileName + "\" \"" + newFileName + "\"";
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;

            p.OutputDataReceived += (sender, e) =>
            {
                var proc = (Process)sender;
                if (!canceled && cancellationToken.IsCancellationRequested)
                {
                    canceled = true;

                    report.Canceling = true;
                    progress?.Report(report);
                    try
                    {
                        proc.CancelErrorRead();
                        proc.CancelOutputRead();
                        /*
                        We should be sending Ctrl+C to it, but since that's apparently deep-level black
                        magic on Windows, we just kill it. We expect the caller to understand that the
                        extracted files will be in an indeterminate state, and to delete them or do whatever
                        it deems fit.
                        */
                        proc.Kill();
                    }
                    catch
                    {
                        // Ignore, it's going to throw but work anyway (even on non-admin, tested)
                    }
                }
            };

            p.ErrorDataReceived += (_, e) =>
            {
                if (!e.Data.IsWhiteSpace()) errorText += $"{NL}---" + e.Data;
            };

            p.Start();
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();

            p.WaitForExit();

            (SevenZipExitCode exitCode, int? exitCodeInt, Exception? exception) = GetExitCode(p);

            return new Result(exception, errorText, exitCode, exitCodeInt, canceled);
        }
        catch (Exception ex)
        {
            return new Result(ex, errorText, SevenZipExitCode.Unknown, canceled);
        }
        finally
        {
            EndProcess(p, false, "");
        }
    }

    private static void EndProcess(Process p, bool selectiveFiles, string listFile)
    {
        try
        {
            if (!p.HasExited)
            {
                p.Kill();
            }
        }
        catch
        {
            // ignore
        }
        finally
        {
            try
            {
                if (!p.HasExited)
                {
                    p.WaitForExit();
                }
            }
            catch
            {
                // ignore...
            }

            p.Dispose();
        }
        if (selectiveFiles && !listFile.IsEmpty())
        {
            try
            {
                File.Delete(listFile);
            }
            catch
            {
                // ignore
            }
        }
    }

    private static (SevenZipExitCode ExitCode, int? ExitCodeInt, Exception? ex)
    GetExitCode(Process p)
    {
        try
        {
            int exitCode = p.ExitCode;
            SevenZipExitCode sevenZipExitCode = exitCode switch
            {
                (int)SevenZipExitCode.NoError => SevenZipExitCode.NoError,
                (int)SevenZipExitCode.Warning => SevenZipExitCode.Warning,
                (int)SevenZipExitCode.FatalError => SevenZipExitCode.FatalError,
                (int)SevenZipExitCode.CommandLineError => SevenZipExitCode.CommandLineError,
                (int)SevenZipExitCode.NotEnoughMemory => SevenZipExitCode.NotEnoughMemory,
                (int)SevenZipExitCode.UserStopped => SevenZipExitCode.UserStopped,
                _ => SevenZipExitCode.Unknown,
            };
            return (sevenZipExitCode, exitCode, null);
        }
        catch (InvalidOperationException ex)
        {
            return (SevenZipExitCode.Unknown, null, ex);
        }
        catch (NotSupportedException ex)
        {
            return (SevenZipExitCode.Unknown, null, ex);
        }
    }
}
