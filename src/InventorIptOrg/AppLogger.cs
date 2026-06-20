using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace InventorIptOrg
{
    internal static class AppBuild
    {
        public const string ProductName = "STEP Organizer";
        public const string Version = "v0.5.17";
        public const string BuildName = "CANVAS_ONLY_GITHUB_PREP";
        public const string BuildToken = "STEP_ORGANIZER_v0.5.16_CANVAS_ONLY_GITHUB_PREP_2026-06-20";
        public const string WindowTitle = ProductName + " — " + Version + " — " + BuildName;
    }

    internal static class AppLogger
    {
        private static readonly object Sync = new object();
        private static readonly Stopwatch AppStopwatch = Stopwatch.StartNew();
        private static long scopeCounter;
        private static string logFilePath;
        private static volatile bool suppressHighFrequencyViewerLogs;

        public static bool SuppressHighFrequencyViewerLogs
        {
            get { return suppressHighFrequencyViewerLogs; }
            set { suppressHighFrequencyViewerLogs = value; }
        }
        static AppLogger()
        {
        }

        public static string LogFilePath
        {
            get
            {
                Initialize();
                return logFilePath;
            }
        }

        public static void Initialize()
        {
            if (!string.IsNullOrEmpty(logFilePath))
            {
                return;
            }

            try
            {
                string baseDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "InventorIptOrganizer",
                    "logs",
                    AppBuild.Version + "_" + AppBuild.BuildName);

                Directory.CreateDirectory(baseDir);

                string fileName = "inventor_ipt_organizer_" +
                    AppBuild.Version.Replace('.', '_') + "_" +
                    DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") + ".log";

                logFilePath = Path.Combine(baseDir, fileName);

                File.AppendAllText(
                    logFilePath,
                    "timestamp\tsince_start_us\tthread\tbuild_token\tevent\tmember\telapsed_us\tmessage" + Environment.NewLine,
                    Encoding.UTF8);

                Log("APP_START", "AppLogger.Initialize", "LogFilePath=" + logFilePath + "; Product=" + AppBuild.ProductName + "; Version=" + AppBuild.Version + "; BuildName=" + AppBuild.BuildName);
            }
            catch
            {
                logFilePath = Path.Combine(Path.GetTempPath(), "inventor_ipt_organizer_v0_4_28_startup_fallback.log");
            }
        }

        public static IDisposable Scope(string memberName)
        {
            return new LogScope(memberName);
        }

        public static void Log(string eventName, string memberName, string message = null, long? elapsedMicroseconds = null)
        {
            try
            {
                if (suppressHighFrequencyViewerLogs && IsHighFrequencyViewerEvent(eventName))
                {
                    return;
                }

                Initialize();

                string line = string.Join("\t", new string[]
                {
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"),
                    ToMicroseconds(AppStopwatch.ElapsedTicks).ToString(),
                    Thread.CurrentThread.ManagedThreadId.ToString(),
                    Escape(AppBuild.BuildToken),
                    Escape(eventName),
                    Escape(memberName),
                    elapsedMicroseconds.HasValue ? elapsedMicroseconds.Value.ToString() : string.Empty,
                    Escape(message)
                });

                lock (Sync)
                {
                    File.AppendAllText(logFilePath, line + Environment.NewLine, Encoding.UTF8);
                }
            }
            catch
            {
                // Логирование никогда не должно ломать CAD-автоматизацию.
            }
        }

        public static void LogException(string memberName, Exception ex)
        {
            Log("EXCEPTION", memberName, ex == null ? null : ex.ToString());
        }

        public static void LogUserMessageShown(string text, string caption, MessageBoxButtons? buttons, MessageBoxIcon? icon)
        {
            string message = "caption=" + (caption ?? string.Empty) +
                "; buttons=" + (buttons.HasValue ? buttons.Value.ToString() : string.Empty) +
                "; icon=" + (icon.HasValue ? icon.Value.ToString() : string.Empty) +
                "; text=" + (text ?? string.Empty);

            Log("USER_MESSAGE_SHOW", "MessageBox.Show", message);
        }

        public static void LogUserMessageClosed(string text, string caption, DialogResult result)
        {
            string message = "caption=" + (caption ?? string.Empty) +
                "; result=" + result.ToString() +
                "; text=" + (text ?? string.Empty);

            Log("USER_MESSAGE_RESULT", "MessageBox.Show", message);
        }

        private static bool IsHighFrequencyViewerEvent(string eventName)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                return false;
            }

            return eventName.StartsWith("STEP_ZBUFFER_", StringComparison.Ordinal) ||
                   string.Equals(eventName, "STEP_SURFACE_SHELL_STATS", StringComparison.Ordinal) ||
                   string.Equals(eventName, "STEP_LOCAL_DRAW_SECONDS", StringComparison.Ordinal);
        }

        private static long ToMicroseconds(long stopwatchTicks)
        {
            return stopwatchTicks * 1000000L / Stopwatch.Frequency;
        }

        private static string Escape(string value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return value
                .Replace("\\", "\\\\")
                .Replace("\r", "\\r")
                .Replace("\n", "\\n")
                .Replace("\t", "\\t");
        }

        private sealed class LogScope : IDisposable
        {
            private readonly string memberName;
            private readonly Stopwatch stopwatch;
            private readonly long id;
            private bool disposed;

            public LogScope(string memberName)
            {
                this.memberName = memberName;
                this.id = Interlocked.Increment(ref scopeCounter);
                this.stopwatch = Stopwatch.StartNew();
                Log("ENTER", this.memberName, "scope_id=" + this.id.ToString());
            }

            public void Dispose()
            {
                if (disposed)
                {
                    return;
                }

                disposed = true;
                stopwatch.Stop();
                Log("EXIT", memberName, "scope_id=" + id.ToString(), ToMicroseconds(stopwatch.ElapsedTicks));
            }
        }
    }

    internal static class LoggedMessageBox
    {
        public static DialogResult Show(string text)
        {
            return ShowCore(text, string.Empty, null, null);
        }

        public static DialogResult Show(string text, string caption)
        {
            return ShowCore(text, caption, null, null);
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons)
        {
            return ShowCore(text, caption, buttons, null);
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            return ShowCore(text, caption, buttons, icon);
        }

        private static DialogResult ShowCore(string text, string caption, MessageBoxButtons? buttons, MessageBoxIcon? icon)
        {
            AppLogger.LogUserMessageShown(text, caption, buttons, icon);

            DialogResult result;

            if (buttons.HasValue && icon.HasValue)
            {
                result = MessageBox.Show(text, caption, buttons.Value, icon.Value);
            }
            else if (buttons.HasValue)
            {
                result = MessageBox.Show(text, caption, buttons.Value);
            }
            else if (!string.IsNullOrEmpty(caption))
            {
                result = MessageBox.Show(text, caption);
            }
            else
            {
                result = MessageBox.Show(text);
            }

            AppLogger.LogUserMessageClosed(text, caption, result);
            return result;
        }
    }
}
