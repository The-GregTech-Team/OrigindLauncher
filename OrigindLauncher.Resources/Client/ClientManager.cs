using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using MaterialDesignThemes.Wpf;
using OrigindLauncher.Resources.FileSystem;
using OrigindLauncher.Resources.Json;
using OrigindLauncher.Resources.Server;
using OrigindLauncher.Resources.Server.Data;
using OrigindLauncher.Resources.String;
using OrigindLauncher.UI.Dialogs;

namespace OrigindLauncher.Resources.Client
{
    public static class ClientManager
    {
        public const string GameStorageDirectory = @".minecraft";
        private const string GameStoragePath = @".minecraft\";

        static ClientManager()
        {
            if (File.Exists(Definitions.ClientJsonPath))
                CurrentInfo = File.ReadAllText(Definitions.ClientJsonPath).JsonCast<ClientInfo>();
        }

        public static ClientInfo CurrentInfo { get; set; }

        public static string GetGameStorageDirectory(string name)
        {
            return $@"{GameStoragePath}{name}";
        }

        public static bool CheckUpdate()
        {
            if (CurrentInfo == null) return true;
            return ClientInfoGetter.Get().Version != CurrentInfo.Version;
        }

        public static ClientInfo MakeClientInfo()
        {
            var basePath = Directory.GetFiles(GameStoragePath, "*", SearchOption.AllDirectories);
            var clientInfo = new ClientInfo();
            var files = new List<FileEntry>();

            Parallel.ForEach(basePath, new ParallelOptions {MaxDegreeOfParallelism = 16}, (file, state) =>
            {
                var path = file.Substring(GameStoragePath.Length);
                // var fileData = File.ReadAllText(file);

                string hash;
                using (var sfile = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    hash = SHA128Computer.Compute(sfile);
                }

                Trace.WriteLine($"计算哈希完成: {path}.");
                files.Add(new FileEntry(path, hash));
            });

            clientInfo.Files = files;

            return clientInfo;
        }

        public static async Task<bool> UpdateAsync()
        {
            DirectoryHelper.EnsureDirectoryExists(GameStorageDirectory);

            var onlineInfo = ClientInfoGetter.Get();
            var updateInfo = GetUpdateInfo(onlineInfo);
            var result = false;

            await Application.Current.Dispatcher.Invoke(async () =>
            {
                var gud = new GameUpdatingDialog(updateInfo);
                await DialogHost.Show(gud, "RootDialog");

                if (gud.Result)
                    CurrentInfo.Version = onlineInfo.Version;
                Save();
                result = gud.Result;
            });

            return result;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Save()
        {
            File.WriteAllText(Definitions.ClientJsonPath, CurrentInfo.ToJsonString());
        }

        public static UpdateInfo GetUpdateInfo(ClientInfo clientInfo)
        {
            if (CurrentInfo == null) CurrentInfo = new ClientInfo();

            var updateInfo = new UpdateInfo();

            var filesInOnlineInfo = clientInfo.Files;

            CurrentInfo.Files.RemoveAll(r => r == null);
            var filesInCurrentInfoDictionary = new HashSet<FileEntry>(CurrentInfo.Files).ToDictionary(e => e.Path);
            var filesInOnlineInfoDictionary = filesInOnlineInfo.ToDictionary(e => e.Path);

            foreach (var onlineInfoFile in filesInOnlineInfo)
                if (filesInCurrentInfoDictionary.ContainsKey(onlineInfoFile.Path))
                {
                    // 要更新的文件
                    if (filesInCurrentInfoDictionary[onlineInfoFile.Path].Hash == onlineInfoFile.Hash) continue;

                    updateInfo.FilesToDelete.Add(onlineInfoFile);
                    updateInfo.FilesToDownload.Add(new DownloadInfo($"{clientInfo.BaseUrl}{onlineInfoFile.Hash}",
                        onlineInfoFile.Path, onlineInfoFile.Hash));
                }
                else
                {
                    // 要添加的文件
                    updateInfo.FilesToDownload.Add(new DownloadInfo($"{clientInfo.BaseUrl}{onlineInfoFile.Hash}",
                        onlineInfoFile.Path, onlineInfoFile.Hash));
                }

            // 要删除的文件
            foreach (var localInfoFile in CurrentInfo.Files.Where(
                localInfoFile => !filesInOnlineInfoDictionary.ContainsKey(localInfoFile.Path)))
                updateInfo.FilesToDelete.Add(localInfoFile);

            return updateInfo;
        }
    }

    public class UpdateInfo
    {
        public List<FileEntry> FilesToDelete { get; } = new List<FileEntry>();
        public List<DownloadInfo> FilesToDownload { get; } = new List<DownloadInfo>();
    }

    public class DownloadInfo
    {
        public readonly string Hash;
        public readonly string Path;
        public readonly string Url;

        public DownloadInfo(string url, string path, string hash)
        {
            Url = url;
            Path = path;
            Hash = hash;
        }

        public override int GetHashCode()
        {
            return Path.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return ((DownloadInfo) obj)?.Path == Path;
        }
    }
}