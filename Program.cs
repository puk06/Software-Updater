using static Software_Updater.Classes.Helper;

namespace Software_Updater
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            try
            {
                if (!ValidateArgs(args, out var tag, out var author, out var softwareName, out var executableName, out var ignoreFiles))
                    return;

                if (!ConfirmUpdate(tag, author, softwareName))
                    return;

                await TerminateSoftwareProcesses(softwareName, executableName);

                var releaseFiles = await GetReleaseFiles(tag, author, softwareName);

                if (releaseFiles.Length == 0)
                {
                    Console.WriteLine("アップデートファイルの取得に失敗しました。");
                    Console.WriteLine("Failed to get update files.");
                    Thread.Sleep(3000);
                    return;
                }

                var language = SelectLanguage();

                var downloadFile = SelectReleaseFile(releaseFiles);

                if (downloadFile == null)
                {
                    Console.WriteLine(language == "English" ? "Failed to find update file." : "アップデートファイルが見つかりませんでした。");
                    return;
                }

                await PerformUpdate(downloadFile.DownloadUrl, softwareName, ignoreFiles, language);

                //Console.WriteLine("アップデートが完了しました！ソフトを使ってくれてありがとうございます！");
                Console.WriteLine(language == "English" ? "Update completed! Thank you for using my software!" : "アップデートが完了しました！ソフトを使ってくれてありがとうございます！");
                Thread.Sleep(3000);
            }
            catch (Exception e)
            {
                Console.WriteLine("アップデート中にエラーが発生しました");
                Console.WriteLine("An error occurred during the update");
                Console.WriteLine(e.Message);
                Thread.Sleep(3000);
            }
        }
    }
}