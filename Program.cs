using Software_Updater.Classes;
using static Software_Updater.Classes.Helper;

namespace Software_Updater
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            try
            {
                if (!ValidateArgs(args, out var tag, out var author, out var softwareName, out var executableName))
                    return;

                if (!ConfirmUpdate(softwareName, tag))
                    return;

                await TerminateSoftwareProcesses(softwareName, executableName);

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
    }
}