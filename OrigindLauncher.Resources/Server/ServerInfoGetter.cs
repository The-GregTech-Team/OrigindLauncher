using System;
using System.Threading.Tasks;
using GoodTimeStudio.ServerPinger;

namespace OrigindLauncher
{
    public static class ServerInfoGetter
    {
        /// <exception cref="AggregateException">
        ///     The task was canceled. The
        ///     <see cref="P:System.AggregateException.InnerExceptions" /> collection contains a
        ///     <see cref="T:System.Threading.Tasks.TaskCanceledException" /> object. -or-An exception was thrown during the
        ///     execution of the task. The <see cref="P:System.AggregateException.InnerExceptions" /> collection contains
        ///     information about the exception or exceptions.
        /// </exception>
        public static async Task<ServerStatus> GetServerInfoAsync()
        {
            var pinger = new ServerPinger("Origind", "d1.natapp.cc", 25333, PingVersion.MC_Current);
            var result = await pinger.GetStatus();
            return result;
        }
    }
}