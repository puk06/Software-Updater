﻿using Octokit;
using System.Diagnostics;

namespace Software_Updater.Classes
{
    internal class Helper
    {
        public static bool ValidateArgs(string[] args, out string tag, out string author, out string softwareName, out string executableName)
        {
            tag = args.Length > 0 ? args[0] : null;
            author = args.Length > 1 ? args[1] : null;
            softwareName = args.Length > 2 ? args[2] : null;
            executableName = args.Length > 3 ? args[3] : null;

            if (args.Length == 4 && !string.IsNullOrEmpty(tag)) return true;
            Console.WriteLine("アップデートに必要な情報の取得に失敗しました。");
            Thread.Sleep(3000);
            return false;

        }

        public static bool ConfirmUpdate(string softwareName, string tag)
        {
            Console.WriteLine($"ソフト名: {softwareName}");
            Console.WriteLine($"アップデート後のバージョン: {tag}");
            Console.WriteLine("上記のアップデートを行います。もし大丈夫な場合はEnterを押してください。");
            Console.ReadLine();
            return true;
        }

        public static Task TerminateSoftwareProcesses(string softwareName, string executableName)
        {
            Console.WriteLine($"{softwareName}関係のソフトをすべて終了します。");

            var processes = Process.GetProcessesByName(executableName);
            foreach (var process in processes)
            {
                process.Kill();
            }

            Console.WriteLine($"{softwareName}を終了しました。アップデートを開始します。");
            return Task.CompletedTask;
        }

        public static ReleaseFile SelectReleaseFile(IReadOnlyList<ReleaseFile> releaseFiles)
        {
            if (releaseFiles.Count == 1)
                return releaseFiles[0];

            int selectedIndex = 0;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("アップデートファイルが複数見つかりました。どのアップデートをダウンロードしますか？\n");

                for (int i = 0; i < releaseFiles.Count; i++)
                {
                    if (i == selectedIndex)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"> {i + 1}: {releaseFiles[i].Filename}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine($"{i + 1}: {releaseFiles[i].Filename}");
                    }
                }

                var keyInfo = Console.ReadKey();
                if (keyInfo.Key == ConsoleKey.UpArrow && selectedIndex > 0)
                    selectedIndex--;
                else if (keyInfo.Key == ConsoleKey.DownArrow && selectedIndex < releaseFiles.Count - 1)
                    selectedIndex++;
                else if (keyInfo.Key == ConsoleKey.Enter)
                    return releaseFiles[selectedIndex];
            }
        }

        public static async Task PerformUpdate(string downloadUrl, string softwareName)
        {
            var updater = new Updater(downloadUrl, softwareName);
            await updater.Update();
        }

        public static async Task<ReleaseFile[]> GetReleaseFiles(string version, string author, string softwareName)
        {
            var githubClient = new GitHubClient(new ProductHeaderValue(softwareName));
            var releases = await githubClient.Repository.Release.GetAll(author, softwareName);
            var releaseFiles = new List<ReleaseFile>();
            foreach (var release in releases)
            {
                if (release.TagName != version) continue;
                foreach (var asset in release.Assets)
                {
                    releaseFiles.Add(new ReleaseFile
                    {
                        Filename = asset.Name,
                        DownloadUrl = asset.BrowserDownloadUrl
                    });
                }
            }

            return releaseFiles.ToArray();
        }
    }
}
