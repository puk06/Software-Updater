﻿using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Text;

namespace Software_Updater.Classes
{
    public class Updater(string downloadUrl, string softwarename, IEnumerable<string> ignoreFiles, string language)
    {
        public async Task Update()
        {
            var tempPath = Path.GetTempPath();
            var tempFile = Path.Combine(tempPath, softwarename + ".zip");
            var extractPath = Path.Combine(tempPath, softwarename + ".Temp.zip");

            Console.WriteLine(language == "English" ? "Downloading file..." : "ファイルのダウンロードを開始しています...");
            if (File.Exists(tempFile)) File.Delete(tempFile);
            if (Directory.Exists(extractPath)) Directory.Delete(extractPath, true);

            using var client = new WebClient();

            client.DownloadProgressChanged += (_, e) =>
            {
                Console.Write(language == "English" ? $"\rDownloading file... {e.ProgressPercentage}% complete" : $"\rファイルのダウンロード中です... {e.ProgressPercentage}% 完了");
            };

            await client.DownloadFileTaskAsync(new Uri(downloadUrl), tempFile);

            Console.WriteLine(language == "English" ? "\nDownload completed!" : "ダウンロードが完了しました！");
            Console.WriteLine(language == "English" ? "Extracting file..." : "ファイルの展開中です...");

            ZipFile.ExtractToDirectory(tempFile, extractPath, Encoding.UTF8, true);
            File.Delete(tempFile);

            var folders = Directory.GetDirectories(extractPath);
            folders = folders.Where(x => !x.Contains("Updater")).ToArray();
            var files = Directory.GetFiles(extractPath);

            var currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (currentPath == null)
            {
                Console.WriteLine(language == "English" ? "Failed to get current folder." : "カレントフォルダの取得に失敗しました。");
                Thread.Sleep(3000);
                return;
            }

            var softwarePath = Directory.GetParent(currentPath)?.FullName;
            if (softwarePath == null)
            {
                Console.WriteLine(language == "English" ? "Failed to get software folder." : "ソフトウェアのフォルダの取得に失敗しました。");
                Thread.Sleep(3000);
                return;
            }

            for (int i = 0; i < files.Length; i++)
            {
                var file = files[i];
                var fileName = Path.GetFileName(file);
                if (ignoreFiles.Contains(fileName)) continue;
                var currentFile = Path.Combine(softwarePath, fileName);
                Console.WriteLine(language == "English" ? $"Copying file... {i + 1}/{files.Length}: {fileName}" : $"ファイルのコピー中です... {i + 1}/{files.Length}: {fileName}");
                File.Copy(file, currentFile, true);
            }

            for (int i = 0; i < folders.Length; i++)
            {
                var folder = folders[i];
                var folderName = Path.GetFileName(folder);
                var currentFolder = Path.Combine(softwarePath, folderName);
                if (!Directory.Exists(currentFolder)) Directory.CreateDirectory(currentFolder);
                Console.WriteLine(language == "English" ? $"Copying folder... {i + 1}/{folders.Length}: {folderName}" : $"フォルダのコピー中です... {i + 1}/{folders.Length}: {folderName}");
                DirectoryCopy(folder, currentFolder, true);
            }

            Directory.Delete(extractPath, true);
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            var dir = new DirectoryInfo(sourceDirName);
            var dirs = dir.GetDirectories();

            if (!dir.Exists) throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName);
            if (!Directory.Exists(destDirName)) Directory.CreateDirectory(destDirName);


            var files = dir.GetFiles();
            foreach (var file in files)
            {
                var tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, true);
            }

            if (!copySubDirs) return;
            foreach (var subdir in dirs)
            {
                var tempPath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, tempPath, true);
            }
        }
    }
}
