using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace DailyCheckInJournal
{
    internal static class CheckIn
    {
        private static int Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            if (args.Length == 0 || HasFlag(args, "--help") || HasFlag(args, "-h"))
            {
                PrintUsage();
                return args.Length == 0 ? 1 : 0;
            }

            string note = "";
            var checkInParts = new List<string>();

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--note" || args[i] == "-n")
                {
                    if (i + 1 >= args.Length)
                    {
                        Console.Error.WriteLine("Missing note text after " + args[i]);
                        return 1;
                    }

                    note = args[++i];
                    continue;
                }

                checkInParts.Add(args[i]);
            }

            string checkIn = string.Join(" ", checkInParts).Trim();

            if (string.IsNullOrWhiteSpace(checkIn))
            {
                Console.Error.WriteLine("Check-in text is required.");
                return 1;
            }

            string root = FindProjectRoot();
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string logsDir = Path.Combine(root, "logs");
            string filePath = Path.Combine(logsDir, date + ".md");

            Directory.CreateDirectory(logsDir);

            if (File.Exists(filePath))
            {
                Console.Error.WriteLine("A check-in already exists for " + date + ": " + filePath);
                return 1;
            }

            var lines = new List<string>
            {
                "# " + date,
                "",
                "- Check-in: " + checkIn
            };

            if (!string.IsNullOrWhiteSpace(note))
            {
                lines.Add("- Note: " + note.Trim());
            }

            File.WriteAllLines(filePath, lines.ToArray(), new UTF8Encoding(false));
            Console.WriteLine("Created " + filePath);
            Console.WriteLine("Commit with: git add logs/" + date + ".md && git commit -m \"Add check-in for " + date + "\"");
            return 0;
        }

        private static bool HasFlag(string[] args, string flag)
        {
            foreach (string arg in args)
            {
                if (arg == flag)
                {
                    return true;
                }
            }

            return false;
        }

        private static string FindProjectRoot()
        {
            string current = Directory.GetCurrentDirectory();
            string fromCurrent = WalkForRoot(current);

            if (fromCurrent != null)
            {
                return fromCurrent;
            }

            string exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string fromExe = WalkForRoot(exeDir);

            return fromExe ?? current;
        }

        private static string WalkForRoot(string start)
        {
            var dir = new DirectoryInfo(start);

            while (dir != null)
            {
                bool hasReadme = File.Exists(Path.Combine(dir.FullName, "README.md"));
                bool hasGit = File.Exists(Path.Combine(dir.FullName, ".git")) || Directory.Exists(Path.Combine(dir.FullName, ".git"));

                if (hasReadme && hasGit)
                {
                    return dir.FullName;
                }

                dir = dir.Parent;
            }

            return null;
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Daily Check-In Journal");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("  checkin.exe \"오늘 한 줄 메모\"");
            Console.WriteLine("  checkin.exe \"오늘 한 줄 메모\" --note \"선택 메모\"");
        }
    }
}
