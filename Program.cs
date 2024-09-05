using System.Diagnostics;
using Octokit;
using Software_Updater.Classes;

namespace Software_Updater
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            try
            {
                if (!ValidateArgs(args, out var tag, out var author, out var softwareName))
                    return;

                if (!ConfirmUpdate())
                    return;

                await TerminateSoftwareProcesses(softwareName);

                var releaseFiles = await GetReleaseFiles(tag, author, softwareName);

                if (releaseFiles.Length == 0)
                {
                    Console.WriteLine("アップデートファイルの取得に失敗しました。");
                    Thread.Sleep(3000);
                    return;
                }

                var downloadFile = SelectReleaseFile(releaseFiles);

                if (downloadFile == null)
                {
                    Console.WriteLine("アップデートファイルが見つかりませんでした。");
                    return;
                }

                await PerformUpdate(downloadFile.DownloadUrl, softwareName);

                Console.WriteLine("アップデートが完了しました！ソフトを使ってくれてありがとうございます！");
                Thread.Sleep(3000);
            }
            catch (Exception e)
            {
                Console.WriteLine("アップデート中にエラーが発生しました: " + e.Message);
                Thread.Sleep(3000);
            }
        }

        private static bool ValidateArgs(string[] args, out string tag, out string author, out string softwareName)
        {
            tag = args.Length > 0 ? args[0] : null;
            author = args.Length > 1 ? args[1] : null;
            softwareName = args.Length > 2 ? args[2] : null;

            if (args.Length == 3 && !string.IsNullOrEmpty(tag)) return true;
            Console.WriteLine("アップデートに必要な情報の取得に失敗しました。");
            Thread.Sleep(3000);
            return false;

        }

        private static bool ConfirmUpdate()
        {
            Console.WriteLine("アップデートを行います。もし大丈夫な場合はEnterを押してください。");
            Console.ReadLine();
            return true;
        }

        private static Task TerminateSoftwareProcesses(string softwareName)
        {
            Console.WriteLine($"{softwareName}関係のソフトをすべて終了します。");

            var processes = Process.GetProcessesByName(softwareName);
            foreach (var process in processes)
            {
                process.Kill();
            }

            Console.WriteLine($"{softwareName}を終了しました。アップデートを開始します。");
            return Task.CompletedTask;
        }

        private static ReleaseFile SelectReleaseFile(IReadOnlyList<ReleaseFile> releaseFiles)
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

        private static async Task PerformUpdate(string downloadUrl, string softwareName)
        {
            var updater = new Updater(downloadUrl, softwareName);
            await updater.Update();
        }

        private static async Task<ReleaseFile[]> GetReleaseFiles(string version, string author, string softwareName)
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