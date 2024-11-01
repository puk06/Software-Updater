using Octokit;
using System.Diagnostics;

namespace Software_Updater.Classes
{
    internal class Helper
    {
        public static bool ValidateArgs(string[] args, out string tag, out string author, out string softwareName, out string executableName, out string[] ignoreFiles, string language)
        {
            tag = args.Length > 0 ? args[0] : null;
            author = args.Length > 1 ? args[1] : null;
            softwareName = args.Length > 2 ? args[2] : null;
            executableName = args.Length > 3 ? args[3] : null;
            ignoreFiles = args.Length > 4 ? args.Skip(4).ToArray() : Array.Empty<string>();
            if (args.Length >= 4 && !string.IsNullOrEmpty(tag)) return true;
            Console.WriteLine(language == "English" ? "Failed to get necessary information for the update." : "アップデートに必要な情報の取得に失敗しました。");
            Thread.Sleep(3000);
            return false;
        }

        public static bool ConfirmUpdate(string version, string author, string softwareName, string language)
        {
            if (language == "English")
            {
                Console.WriteLine($"Software name: {softwareName} ({author}/{softwareName})");
                Console.WriteLine($"New version: {version}");
                Console.WriteLine("Please press Enter if the above information is correct and you want to update.");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine($"ソフト名: {softwareName} ({author}/{softwareName})");
                Console.WriteLine($"アップデート後のバージョン: {version}");
                Console.WriteLine("上記の情報を元にアップデートを行います。もし大丈夫な場合はEnterを押してください。");
                Console.ReadLine();
            }

            return true;
        }

        public static Task TerminateSoftwareProcesses(string softwareName, string executableName, string language)
        {
            Console.WriteLine(language == "English" ? $"Closing all {softwareName} related software." : $"{softwareName}関係のソフトをすべて終了します。");

            var processes = Process.GetProcessesByName(executableName);
            foreach (var process in processes)
            {
                process.Kill();
            }

            Console.WriteLine(language == "English" ? $"Closed {softwareName}. Starting the update." : $"{softwareName}を終了しました。アップデートを開始します。");
            return Task.CompletedTask;
        }

        public static ReleaseFile SelectReleaseFile(IReadOnlyList<ReleaseFile> releaseFiles, string language)
        {
            if (releaseFiles.Count == 1)
                return releaseFiles[0];

            int selectedIndex = 0;
            while (true)
            {
                Console.Clear();
                Console.WriteLine(language == "English" ? "Multiple update files found. Which update do you want to download?\n" : "アップデートファイルが複数見つかりました。どのアップデートをダウンロードしますか？\n");

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
                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow when selectedIndex > 0:
                        selectedIndex--;
                        break;
                    case ConsoleKey.DownArrow when selectedIndex < releaseFiles.Count - 1:
                        selectedIndex++;
                        break;
                    case ConsoleKey.Enter:
                        return releaseFiles[selectedIndex];
                }
            }
        }

        public static string SelectLanguage()
        {
            string[] languages = { "English", "日本語" };
            int selectedIndex = 0;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("言語を選択してください / Select a language\n");

                for (int i = 0; i < languages.Length; i++)
                {
                    if (i == selectedIndex)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"> {i + 1}: {languages[i]}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine($"{i + 1}: {languages[i]}");
                    }
                }

                var keyInfo = Console.ReadKey();
                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow when selectedIndex > 0:
                        selectedIndex--;
                        break;
                    case ConsoleKey.DownArrow when selectedIndex < languages.Length - 1:
                        selectedIndex++;
                        break;
                    case ConsoleKey.Enter:
                        return languages[selectedIndex];
                }
            }
        }

        public static async Task PerformUpdate(string downloadUrl, string softwareName, string[] ignoreFiles, string language)
        {
            var updater = new Updater(downloadUrl, softwareName, ignoreFiles, language);
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
