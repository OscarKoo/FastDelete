using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FastDelete
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Console.WriteLine("Please choose the folder to delete");
                return;
            }

            var folder = args.First();

            if (!Directory.Exists(folder))
            {
                Console.WriteLine("Folder does not exists");
                return;
            }

            var subFolders = Directory.GetDirectories(folder);
            var total = subFolders.Length;

            var files = Directory.GetFiles(folder);
            total += files.Length;

            sw.Start();

            if (files.Length > 0)
            {
                Parallel.ForEach(subFolders, f =>
                {
                    File.Delete(f);
                    ShowProgress($"Deleting {folder}: ", total);
                });
            }

            if (subFolders.Length > 0)
            {
                Parallel.ForEach(subFolders, f =>
                {
                    Directory.Delete(f, true);
                    ShowProgress($"Deleting {folder}: ", total);
                });
            }

            sw.Stop();
        }

        static readonly object syncObj = new object();
        static int count;
        static readonly Stopwatch sw = new Stopwatch();

        static void ShowProgress(string message, int total)
        {
            lock (syncObj)
            {
                var current = sw.ElapsedMilliseconds;
                ClearLine();
                count++;
                var left = current * (total - count) / (double)count;
                var ts = TimeSpan.FromMilliseconds(left);
                Console.Write($"{message}({count}/{total} - {count * 100f / total}%, Estimated Time: {ts:dd\\.hh\\:mm\\:ss})");
                if (total == count)
                    Console.WriteLine();
            }
        }

        static void ClearLine()
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.CursorTop - 1);
        }
    }
}