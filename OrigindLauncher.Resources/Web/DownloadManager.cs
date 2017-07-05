using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using OrigindLauncher.Resources.FileSystem;
using OrigindLauncher.Resources.String;

namespace OrigindLauncher.Resources.Web
{
    public class DownloadManager
    {
        public delegate void CompletedEventHandler(CompletedEventArgs completedEventArgs);

        public delegate void OnErrorEventHandler(OnErrorEventArgs onErrorEventArgs);

        public delegate void ProgressChangedEventHandler(
            DownloadProgressChangeEventArgs downloadProgressChangeEventArgs);

        private const int MaxThread = 8;

        private readonly IEnumerable<DownloadInfo> _infos;
        private int _completedCount;

        public DownloadManager(IEnumerable<DownloadInfo> infos)
        {
            _infos = infos;
        }

        public int RetryTimes { get; set; } = 5;
        public string AddDownloadPath { get; set; } = "";

        public bool Downloading { get; set; } = true;

        public event CompletedEventHandler DownloadFileCompleted;
        public event ProgressChangedEventHandler DownloadProgressChanged;
        public event OnErrorEventHandler OnError;
        public event Action AllDone;

        public void Start()
        {
            Task.Run(() =>
            {
                Parallel.ForEach(_infos, new ParallelOptions {MaxDegreeOfParallelism = MaxThread}, info =>
                {
                    var wc = new WebClient();
                    wc.DownloadProgressChanged += (sender, args) =>
                    {
                        DownloadProgressChanged?.Invoke(new DownloadProgressChangeEventArgs
                        {
                            BytesReceived = args.BytesReceived,
                            TotalBytesToReceive = args.TotalBytesToReceive,
                            FileLocation = info.Path
                        });
                    };


                    var trytimes = RetryTimes;

                    while (trytimes-- != 0 && Downloading)
                        try
                        {
                            var downloadPath = AddDownloadPath + info.Path;
                            var directoryName = Path.GetDirectoryName(downloadPath);

                            if (!string.IsNullOrWhiteSpace(directoryName))
                                DirectoryHelper.EnsureDirectoryExists(directoryName);

                            if (File.Exists(downloadPath))
                            {
                                string hash;
                                using (var sfile =
                                    File.Open(downloadPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                                {
                                    hash = SHA128Helper.Compute(sfile);
                                }
                                if (hash == info.Hash)
                                {
                                    _completedCount++;
                                    DownloadFileCompleted?.Invoke(new CompletedEventArgs {FileLocation = info.Path});
                                    if (_completedCount == _infos.Count())
                                        AllDone?.Invoke();
                                    break;
                                }
                            }

                            wc.DownloadFileTaskAsync(info.Url, downloadPath).Wait();

                            DownloadFileCompleted?.Invoke(new CompletedEventArgs {FileLocation = info.Path});
                            _completedCount++;
                            break;
                        }
                        catch (Exception e)
                        {
                            OnError?.Invoke(new OnErrorEventArgs
                            {
                                Exception = e,
                                FileLocation = info.Path,
                                IsFinal = trytimes == 0
                            });
                        }


                    if (_completedCount == _infos.Count())
                        AllDone?.Invoke();
                });
                if (_completedCount == _infos.Count())
                    AllDone?.Invoke();
            });
        }
    }


    public class CompletedEventArgs
    {
        public string FileLocation;
    }

    public class DownloadProgressChangeEventArgs
    {
        public long BytesReceived;
        public string FileLocation;
        public long TotalBytesToReceive;
    }

    public class OnErrorEventArgs
    {
        public Exception Exception;
        public string FileLocation;
        public bool IsFinal;
    }

    public class DownloadInfo
    {
        public string Hash;
        public string Path;

        public string Url;

        public DownloadInfo(string url, string path, string hash)
        {
            Url = url;
            Path = path;
            Hash = hash;
        }
    }
}