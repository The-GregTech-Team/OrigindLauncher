using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using OrigindLauncher.Resources.FileSystem;
using OrigindLauncher.Resources.Json;
using OrigindLauncher.Resources.Server;
using OrigindLauncher.Resources.Server.Data;
using OrigindLauncher.Resources.Web;

namespace OrigindLauncher.Resources.Client
{
    public static class ClientManager
    {
        public const string GameStorageDirectory = @".minecraft";
        private const string GameStoragePath = @".minecraft\";

        public const string AssetsDownloadPath = "assets.zip";
        private static ClientInfo _currentInfo;

        static ClientManager()
        {
            if (File.Exists(Definitions.ClientJsonPath))
                _currentInfo = File.ReadAllText(Definitions.ClientJsonPath).JsonCast<ClientInfo>();
        }

        private static string GetGameStorageDirectory(string name)
        {
            return $@"{GameStoragePath}\{name}";
        }

        public static bool CheckUpdate()
        {
            if (_currentInfo == null) return true;
            return ClientInfoGetter.Get().Version != _currentInfo.Version;
        }

        private static void DownloadAssetsFileAndPut(DownloadManager.CompletedEventHandler downloadFileCompleted,
            DownloadManager.ProgressChangedEventHandler downloadProgressChanged,
            DownloadManager.OnErrorEventHandler onError)
        {
            var clientInfo = ClientInfoGetter.Get();

            var downloadman = new DownloadManager(new[]
                {new DownloadInfo($"{clientInfo.BaseUrl}{AssetsDownloadPath}", AssetsDownloadPath)});
            downloadman.DownloadFileCompleted += downloadFileCompleted;
            downloadman.DownloadProgressChanged += downloadProgressChanged;
            downloadman.OnError += onError;
            downloadman.DownloadFileCompleted += args =>
            {
                ZipFile.ExtractToDirectory(AssetsDownloadPath, GameStoragePath);
                File.Delete(AssetsDownloadPath);
            };

            downloadman.Start();
        }

        public static DownloadStatusInfo Update(DownloadManager.CompletedEventHandler downloadFileCompleted,
            DownloadManager.ProgressChangedEventHandler downloadProgressChanged,
            DownloadManager.OnErrorEventHandler onError, Action allDone)
        {
            DirectoryHelper.EnsureDirectoryExists(GameStorageDirectory);

            var dsi = new DownloadStatusInfo();
            if (!Directory.Exists(GetGameStorageDirectory("assets")))
            {
                DownloadAssetsFileAndPut(downloadFileCompleted, downloadProgressChanged, onError);
                dsi.FileNameList.Add(AssetsDownloadPath);
            }

            var onlineInfo = ClientInfoGetter.Get();
            var updateInfo = GetUpdateInfo();

            foreach (var deletes in updateInfo.FilsToDelete)
                File.Delete(GameStoragePath+deletes.Path);

            dsi.FileNameList.AddRange(updateInfo.FilsToDownload.Select(a => a.Path));

            _currentInfo = onlineInfo;
            File.WriteAllText(Definitions.ClientJsonPath, _currentInfo.ToJsonString());

            var downloadman = new DownloadManager(updateInfo.FilsToDownload);

            downloadman.addDownloadPath = GameStoragePath;
            downloadman.DownloadFileCompleted += downloadFileCompleted;
            downloadman.DownloadProgressChanged += downloadProgressChanged;
            downloadman.OnError += onError;
            downloadman.AllDone += allDone;
            downloadman.Start();
            //TODO


            return dsi;
        }

        private static UpdateInfo GetUpdateInfo()
        {
            if (_currentInfo == null) _currentInfo = new ClientInfo();
            var onlineInfo = ClientInfoGetter.Get();

            var updateInfo = new UpdateInfo();

            var filesInOnlineInfo = onlineInfo.Files;
            var filesInCurrentInfoDictionary = _currentInfo.Files.ToDictionary(e => e.Path);
            var filesInOnlineInfoDictionary = filesInOnlineInfo.ToDictionary(e => e.Path);

            foreach (var onlineInfoFile in filesInOnlineInfo)
                if (filesInCurrentInfoDictionary.ContainsKey(onlineInfoFile.Path))
                {
                    // 要更新的文件
                    if (filesInCurrentInfoDictionary[onlineInfoFile.Path].Hash == onlineInfoFile.Hash) continue;

                    updateInfo.FilsToDelete.Add(onlineInfoFile);
                    updateInfo.FilsToDownload.Add(new DownloadInfo($"{onlineInfo.BaseUrl}{onlineInfoFile.Hash}",
                        onlineInfoFile.Path));
                }
                else
                {
                    // 要添加的文件
                    updateInfo.FilsToDownload.Add(new DownloadInfo($"{onlineInfo.BaseUrl}{onlineInfoFile.Hash}",
                        onlineInfoFile.Path));
                }

            // 要删除的文件
            foreach (var localInfoFile in _currentInfo.Files)
                if (!filesInOnlineInfoDictionary.ContainsKey(localInfoFile.Path))
                    updateInfo.FilsToDelete.Add(localInfoFile);

            return updateInfo;
        }
    }

    public class UpdateInfo
    {
        public List<FileEntry> FilsToDelete = new List<FileEntry>();
        public List<DownloadInfo> FilsToDownload = new List<DownloadInfo>();
    }

    public class DownloadStatusInfo
    {
        public List<string> FileNameList = new List<string>();
    }
}