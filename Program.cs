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
                if (args.Length != 3)
                {
                    Console.WriteLine("アップデートに必要な情報の取得に失敗しました。");
                    Thread.Sleep(3000);
                    return;
                }

                var tag = args[0];
                var author = args[1];
                var softwareName = args[2];

                if (string.IsNullOrEmpty(tag))
                {
                    Console.WriteLine("バージョン情報が取得できませんでした。");
                    Thread.Sleep(3000);
                    return;
                }

                Console.WriteLine("アップデートを行います。もし大丈夫な場合はEnterを押してください。");
                Console.ReadLine();
                Console.WriteLine($"{softwareName}関係のソフトをすべて終了します。");

                var processes = Process.GetProcessesByName(softwareName);
                foreach (var process in processes)
                {
                    process.Kill();
                }

                Console.WriteLine($"{softwareName}を終了しました。アップデートを開始します。");

                var releaseFiles = await GetReleaseFiles(tag, author, softwareName);
                var downloadFile = releaseFiles[0];
                if (releaseFiles.Length == 0)
                {
                    Console.WriteLine("アップデートファイルの取得に失敗しました。");
                    Thread.Sleep(3000);
                    return;
                }

                int selectedIndex = 0;

                if (releaseFiles.Length > 1)
                {
                    while (true)
                    {
                        Console.Clear();

                        Console.WriteLine("アップデートファイルが複数見つかりました。どのアップデートをダウンロードしますか？\n");

                        for (int i = 0; i < releaseFiles.Length; i++)
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

                        ConsoleKeyInfo keyInfo = Console.ReadKey();

                        if (keyInfo.Key == ConsoleKey.UpArrow)
                        {
                            if (selectedIndex > 0)
                            {
                                selectedIndex--;
                            }
                        }
                        else if (keyInfo.Key == ConsoleKey.DownArrow)
                        {
                            if (selectedIndex < releaseFiles.Length - 1)
                            {
                                selectedIndex++;
                            }
                        }
                        else if (keyInfo.Key == ConsoleKey.Enter)
                        {
                            downloadFile = releaseFiles[selectedIndex];
                            break;
                        }
                    }
                }
                else if (releaseFiles.Length == 1)
                {
                    downloadFile = releaseFiles[0];
                }
                else
                {
                    Console.WriteLine("アップデートファイルが見つかりませんでした。");
                }

                var updater = new Updater(downloadFile.DownloadUrl, softwareName);

                await updater.Update();

                Console.WriteLine("アップデートが完了しました！ソフトを使ってくれてありがとうございます！");
                Thread.Sleep(3000);
            }
            catch (Exception e)
            {
                Console.WriteLine("アップデート中にエラーが発生しました: " + e.Message);
                Thread.Sleep(3000);
            }
        }

        private static async Task<Releases[]> GetReleaseFiles(string version, string author, string softwareName)
        {
            var githubClient = new GitHubClient(new ProductHeaderValue(softwareName));
            var releases = await githubClient.Repository.Release.GetAll(author, softwareName);
            var releaseFiles = new List<Releases>();
            foreach (var release in releases)
            {
                if (release.TagName == version)
                {
                    foreach (var asset in release.Assets)
                    {
                        releaseFiles.Add(new Releases
                        {
                            Filename = asset.Name,
                            DownloadUrl = asset.BrowserDownloadUrl
                        });
                    }
                }
            }

            return releaseFiles.ToArray();
        }
    }
}