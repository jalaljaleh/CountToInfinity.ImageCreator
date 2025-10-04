using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace CountToInfinity
{
    internal class Program
    {
        // ------------------------------
        // Configuration (defaults)
        // ------------------------------

        /// <summary>Target pixel size for each digit sprite.</summary>
        private const int DigitSize = 76;

        /// <summary>Folder that contains clear.png and digit sprites 0.png..9.png (can be overridden by --resources).</summary>
        private static string ResourceFolder = "resources";

        /// <summary>Template/background image filename in ResourceFolder.</summary>
        private static string ClearTemplate = "clear.png";

        /// <summary>Destination folder for generated PNGs (fixed).</summary>
        private const string OutFolder = "out";

        /// <summary>Horizontal spacing between digits in pixels.</summary>
        private const int DigitGap = 26;

        // ------------------------------
        // Runtime state
        // ------------------------------

        /// <summary>Cache for preloaded & resized digit images (0..9).</summary>
        private static readonly Dictionary<char, Image<Rgba32>> DigitCache = new();

        static void Main(string[] args)
        {
            PrintHeader();

            // If args provided, run non-interactively once and exit
            if (TryParseArgs(args, out var fromArg, out var toArg, out var resArg))
            {
                if (resArg != null) ResourceFolder = resArg!;
                try
                {
                    RunBatch(fromArg, toArg);
                }
                finally
                {
                    DisposeDigitCache();
                }
                return; // auto close
            }

            // Interactive loop
            while (true)
            {
                try
                {
                    RunOnceInteractive();
                }
                catch (Exception ex)
                {
                    LogError($"Unhandled error: {ex.Message}");
                    Console.WriteLine();
                    AskContinue();
                }
                finally
                {
                    DisposeDigitCache();
                }
            }
        }

        // ------------------------------
        // UI / Header
        // ------------------------------

        /// <summary>Draws a simple header banner.</summary>
        private static void PrintHeader()
        {
            Console.Clear();
            WriteBannerLine();
            WriteInfo("                         - Welcome -                              ");
            WriteInfo("              This Program Made by jalaljaleh                     ");
            WriteInfo("     https://github.com/jalaljaleh/CountToInfinity.ImageCreator   ");
            WriteBannerLine();
            Console.WriteLine();
        }

        private static void WriteBannerLine()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(new string('*', 66));
            Console.ResetColor();
        }

        // ------------------------------
        // Non-interactive batch (auto-exit)
        // ------------------------------

        /// <summary>Generates images for the inclusive range [from, to] without prompts.</summary>
        private static void RunBatch(int from, int to)
        {
            if (to < from)
            {
                LogError($"Invalid range: to({to}) < from({from})");
                return;
            }

            EnsureDirectory(OutFolder);
            LoadDigitCacheOrThrow();

            var sw = Stopwatch.StartNew();
            int maxDigits = to.ToString().Length;
            int total = to - from + 1;
            LogInfo($"Generating {total} image(s) from {from} to {to} into '{Path.GetFullPath(OutFolder)}' ...");

            for (int i = from; i <= to; i++)
            {
                try
                {
                    CreateImage(i, maxDigits);
                }
                catch (Exception ex)
                {
                    LogError($"Failed creating image for {i}: {ex.Message}");
                }
            }

            sw.Stop();
            LogSuccess($"Finished: {total} files in {sw.Elapsed.TotalSeconds:0.##}s");
            // No prompt, no open — auto exit
        }

        // ------------------------------
        // Interactive flow
        // ------------------------------

        /// <summary>Reads range from the console and generates the images.</summary>
        private static void RunOnceInteractive()
        {
            int from = ReadIntPrompt("From (start number):");
            int to = ReadIntPrompt("To (end number):");

            if (to < from)
            {
                LogWarning("The 'to' value must be greater than or equal to 'from'. Aborting this run.");
                AskContinue();
                return;
            }

            EnsureDirectory(OutFolder);
            LoadDigitCacheOrThrow();

            var sw = Stopwatch.StartNew();
            int maxDigits = to.ToString().Length;
            int total = to - from + 1;
            LogInfo($"Generating {total} image(s) from {from} to {to} ...");

            for (int i = from; i <= to; i++)
            {
                try
                {
                    CreateImage(i, maxDigits);
                }
                catch (Exception ex)
                {
                    LogError($"Failed creating image for {i}: {ex.Message}");
                }
            }

            sw.Stop();
            LogSuccess($"Finished: {total} files in {sw.Elapsed.TotalSeconds:0.##}s");
            AskContinue();
        }

        // ------------------------------
        // Input helpers
        // ------------------------------

        /// <summary>Reads a valid int from the console with a colored prompt.</summary>
        private static int ReadIntPrompt(string prompt)
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write(prompt + " ");
                Console.ResetColor();

                var line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                {
                    LogWarning("Empty input. Please enter a number.");
                    continue;
                }

                if (int.TryParse(line.Trim(), out var value))
                    return value;

                LogWarning($"Invalid number: '{line}'. Try again.");
            }
        }

        /// <summary>Pauses after a run in interactive mode.</summary>
        private static void AskContinue()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine();
            Console.WriteLine();
            Console.Write("Press any key to continue or Ctrl+C to exit...");
            Console.WriteLine();
            Console.ResetColor();
            Console.ReadKey(intercept: true);
        }

        // ------------------------------
        // Filesystem / resources
        // ------------------------------

        /// <summary>Ensures a directory exists (creates if missing).</summary>
        private static void EnsureDirectory(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        /// <summary>Loads and resizes digit images once. Throws if template is missing.</summary>
        private static void LoadDigitCacheOrThrow()
        {
            if (DigitCache.Count == 0)
            {
                // Load each digit sprite 0..9
                for (char c = '0'; c <= '9'; c++)
                {
                    var file = Path.Combine(ResourceFolder, $"{c}.png");
                    if (!File.Exists(file))
                    {
                        LogWarning($"Digit resource missing: {file} (skipping '{c}')");
                        continue;
                    }

                    var img = Image.Load<Rgba32>(file);
                    img.Mutate(x => x.Resize(DigitSize, DigitSize));
                    DigitCache[c] = img;
                }
            }

            // Ensure template exists up front
            var templatePath = Path.Combine(ResourceFolder, ClearTemplate);
            if (!File.Exists(templatePath))
                throw new FileNotFoundException($"Template image not found: {templatePath}");
        }

        /// <summary>Disposes cached digit images to release memory.</summary>
        private static void DisposeDigitCache()
        {
            foreach (var kv in DigitCache)
                kv.Value.Dispose();
            DigitCache.Clear();
        }

        // ------------------------------
        // Image generation
        // ------------------------------

        /// <summary>
        /// Creates a single image for the given number using the template and cached digits.
        /// The composed digits are centered horizontally and vertically.
        /// </summary>
        private static void CreateImage(int number, int maxDigits)
        {
            LogInfo($"Creating image {number} ...");

            var templatePath = Path.Combine(ResourceFolder, ClearTemplate);

            using (var parent = Image.Load<Rgba32>(templatePath))
            {
                var s = number.ToString();
                var chars = s.ToCharArray();

                // Total width of the composed digit block: digits + gaps
                int totalWidth = (chars.Length * DigitSize) + ((chars.Length - 1) * DigitGap);

                // Center horizontally and vertically
                int startX = (parent.Width - totalWidth) / 2;
                int y = (parent.Height - DigitSize) / 2;

                // Place each digit left-to-right
                for (int i = 0; i < chars.Length; i++)
                {
                    char ch = chars[i];
                    if (!DigitCache.TryGetValue(ch, out var cached))
                    {
                        LogWarning($"Missing digit image for '{ch}', skipping.");
                        continue;
                    }

                    int x = startX + (i * (DigitSize + DigitGap));

                    // Clone before drawing to avoid mutating the cached instance
                    using (var clone = cached.Clone())
                    {
                        parent.Mutate(ctx => ctx.DrawImage(clone, new Point(x, y), 1f));
                    }
                }

                // Save to out/<number>.png
                string outPath = Path.Combine(OutFolder, $"{s}.png");
                parent.SaveAsPng(outPath);
                LogSuccess($"Saved: {outPath}");
            }
        }

        // ------------------------------
        // Arg parsing
        // ------------------------------

        // Supported args:
        //   --from <int>
        //   --to <int>
        //   --resources <path>   (optional; overrides default "resources" folder)
        private static bool TryParseArgs(string[] args, out int from, out int to, out string? resFolder)
        {
            from = 0; to = -1; resFolder = null;
            if (args == null || args.Length == 0) return false;

            for (int i = 0; i < args.Length; i++)
            {
                var a = args[i];
                switch (a)
                {
                    case "--from":
                        if (i + 1 < args.Length && int.TryParse(args[i + 1], out var f)) { from = f; i++; }
                        break;
                    case "--to":
                        if (i + 1 < args.Length && int.TryParse(args[i + 1], out var t)) { to = t; i++; }
                        break;
                    case "--resources":
                        if (i + 1 < args.Length) { resFolder = args[i + 1]; i++; }
                        break;
                }
            }

            // Require a valid inclusive range for non-interactive run
            return to >= from && to >= 0;
        }

        // ------------------------------
        // Logging (colored)
        // ------------------------------

        private static void LogInfo(string text)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{Timestamp()} [INFO]  {text}");
            Console.ResetColor();
        }

        private static void LogWarning(string text)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{Timestamp()} [WARN]  {text}");
            Console.ResetColor();
        }

        private static void LogError(string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{Timestamp()} [ERROR] {text}");
            Console.ResetColor();
        }

        private static void LogSuccess(string text)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{Timestamp()} [OK]    {text}");
            Console.ResetColor();
        }

        private static void WriteInfo(string text)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        private static string Timestamp() => DateTime.UtcNow.ToString("HH:mm:ss");
    }
}
