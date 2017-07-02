using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using OrigindLauncher.Resources.FileSystem;

namespace OrigindLauncher.Resources.Web
{
    public class DownloadManager
    {
        public delegate void CompletedEventHandler(CompletedEventArgs completedEventArgs);

        public delegate void OnErrorEventHandler(OnErrorEventArgs onErrorEventArgs);

        public delegate void ProgressChangedEventHandler(
            DownloadProgressChangeEventArgs downloadProgressChangeEventArgs);

        private const int MaxThread = 8;
        private int completedCount;

        public IEnumerable<DownloadInfo> Infos;

        public DownloadManager(IEnumerable<DownloadInfo> infos)
        {
            Infos = infos;
        }

        public int RetryTimes { get; set; } = 5;
        public string addDownloadPath { get; set; } = "";

        public event CompletedEventHandler DownloadFileCompleted;
        public event ProgressChangedEventHandler DownloadProgressChanged;
        public event OnErrorEventHandler OnError;
        public event Action AllDone;

        public void Start()
        {
            Task.Run(() =>
            {
                Parallel.ForEach(Infos, new ParallelOptions {MaxDegreeOfParallelism = MaxThread}, info =>
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

                    while (trytimes-- != 0)
                        try
                        {
                            var directoryName = Path.GetDirectoryName(addDownloadPath + info.Path);
                            if (!string.IsNullOrWhiteSpace(directoryName))
                                DirectoryHelper.EnsureDirectoryExists(directoryName);
                            if (File.Exists(addDownloadPath + info.Path))
                            {
                                File.Delete(addDownloadPath + info.Path);
                            }

                            wc.DownloadFileTaskAsync(info.Url, addDownloadPath + info.Path).Wait();

                            DownloadFileCompleted?.Invoke(new CompletedEventArgs {FileLocation = info.Path});
                            completedCount++;
                            break;
                        }
                        catch (WebException e)
                        {
                            OnError?.Invoke(new OnErrorEventArgs
                            {
                                Exception = e,
                                FileLocation = info.Path,
                                IsFinal = trytimes == 0
                            });
                            // TODO Logger
                        }

                    if (completedCount == Infos.Count())
                        AllDone?.Invoke();
                });
                if (completedCount == Infos.Count())
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
        public WebException Exception;
        public string FileLocation;
        public bool IsFinal;
    }

    public class DownloadInfo
    {
        public string Path;

        public string Url;

        public DownloadInfo(string url, string path)
        {
            Url = url;
            Path = path;
        }
    }
}