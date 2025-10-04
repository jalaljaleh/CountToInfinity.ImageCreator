using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CountToInfinity
{
    internal class FileManager
    {
        public static string ToResourcePath(string path)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), $@"Resources/{path}");
        }

        public static void OpenOutputFolder(string folderPath)
        {
            try
            {
                var full = Path.GetFullPath(folderPath);

                if (!Directory.Exists(full))
                {
                    Console.WriteLine($"Output folder does not exist: {full}");
                    return;
                }

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    // Explorer
                    Process.Start(new ProcessStartInfo("explorer", $"\"{full}\"") { UseShellExecute = true });
                    return;
                }

                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    // macOS Finder
                    Process.Start("open", full);
                    return;
                }

                // Linux (most distros)
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", full);
                    return;
                }

                // Fallback: try UseShellExecute on the folder path (may work on some platforms)
                Process.Start(new ProcessStartInfo(full) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to open folder: {ex.Message}");
            }
        }

    }
}
