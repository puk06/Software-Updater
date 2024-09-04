using System.Diagnostics;
using Octokit;

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
                releaseFiles = releaseFiles.Where(x => !x.Contains("archive/refs")).ToArray();
                if (releaseFiles.Length == 0)
                {
                    Console.WriteLine("アップデートファイルの取得に失敗しました。");
                    Thread.Sleep(3000);
                    return;
                }

                var releaseFile = releaseFiles[0];

                var updater = new Classes.Updater(releaseFile, softwareName);

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

        private static async Task<string[]> GetReleaseFiles(string version, string author, string softwareName)
        {
            var githubClient = new GitHubClient(new ProductHeaderValue(softwareName));
            var releases = await githubClient.Repository.Release.GetAll(author, softwareName);
            foreach (var release in releases)
            {
                if (release.TagName == version)
                {
                    return release.Assets.Select(x => x.BrowserDownloadUrl).ToArray();
                }
            }

            return null;
        }
    }
}