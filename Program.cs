using static Software_Updater.Classes.Helper;

namespace Software_Updater
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            try
            {
                var language = SelectLanguage();

                if (!ValidateArgs(args, out var tag, out var author, out var softwareName, out var executableName, out var ignoreFiles, language))
                    return;

                if (!ConfirmUpdate(tag, author, softwareName, language))
                    return;

                await TerminateSoftwareProcesses(softwareName, executableName, language);

                var releaseFiles = await GetReleaseFiles(tag, author, softwareName);

                if (releaseFiles.Length == 0)
                {
                    Console.WriteLine(language == "English" ? "Failed to get update files." : "アップデートファイルの取得に失敗しました。");
                    Thread.Sleep(3000);
                    return;
                }


                var downloadFile = SelectReleaseFile(releaseFiles, language);

                if (downloadFile == null)
                {
                    Console.WriteLine(language == "English" ? "Failed to find update file." : "アップデートファイルが見つかりませんでした。");
                    return;
                }

                await PerformUpdate(downloadFile.DownloadUrl, softwareName, ignoreFiles, language);

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