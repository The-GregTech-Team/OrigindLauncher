using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using OrigindLauncher.Resources.Configs;

namespace OrigindLauncher.Resources.Core
{
    public static class DebugHelper
    {
        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        private const string LogoBase64 =
                "/9j/4AAQSkZJRgABAQEAYABgAAD/4QCSRXhpZgAATU0AKgAAAAgABQE+AAUAAAACAAAASgE/AAUAAAAGAAAAWlEQAAEAAAABAQAAAFERAAQAAAABAAAOxFESAAQAAAABAAAOxAAAAAAAAHolAAGGoAAAgIMAAYagAAD5/wABhqAAAIDpAAGGoAAAdTAAAYagAADqYAABhqAAADqYAAGGoAAAF28AAYag/9sAQwAIBgYHBgUIBwcHCQkICgwUDQwLCwwZEhMPFB0aHx4dGhwcICQuJyAiLCMcHCg3KSwwMTQ0NB8nOT04MjwuMzQy/9sAQwEJCQkMCwwYDQ0YMiEcITIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIy/8AAEQgAIAAwAwEiAAIRAQMRAf/EAB8AAAEFAQEBAQEBAAAAAAAAAAABAgMEBQYHCAkKC//EALUQAAIBAwMCBAMFBQQEAAABfQECAwAEEQUSITFBBhNRYQcicRQygZGhCCNCscEVUtHwJDNicoIJChYXGBkaJSYnKCkqNDU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6g4SFhoeIiYqSk5SVlpeYmZqio6Slpqeoqaqys7S1tre4ubrCw8TFxsfIycrS09TV1tfY2drh4uPk5ebn6Onq8fLz9PX29/j5+v/EAB8BAAMBAQEBAQEBAQEAAAAAAAABAgMEBQYHCAkKC//EALURAAIBAgQEAwQHBQQEAAECdwABAgMRBAUhMQYSQVEHYXETIjKBCBRCkaGxwQkjM1LwFWJy0QoWJDThJfEXGBkaJicoKSo1Njc4OTpDREVGR0hJSlNUVVZXWFlaY2RlZmdoaWpzdHV2d3h5eoKDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uLj5OXm5+jp6vLz9PX29/j5+v/aAAwDAQACEQMRAD8A9/rwKNHlkWONGd3IVVUZJJ6ACvfa88+GiIZNSkKKXURqGxyAd2Rn3wPyFetl1f2FKrUte1v1PBzfDfWa9Cje1+b8kzGg8Nw2Not74huHson/ANVbxgGaX5c/8B7dR7HHFE/huG+tGvfD1w97En+tt5ABNF8uf+Bd+g9hnmtLxF4J1FZ5Lyzmlv0IBYSvum4HP+90GMc8gY4zR4d8E6i08d5eTS2CAEqIn2zcjj/d6nOeeCMc5r0PrUfZ+29rr26elt/nf8NDyvqU/a/V/YO3e+vrfb5W/HU4yRHikaORGR0JVlYYII6givfa88+JaIJNNkCKHYSKWxyQNuBn2yfzNeh15+Y1/b0qVS1r3/Q9XKMN9Wr16N725fybCvPPhpIgk1KMuodhGwXPJA3ZOPbI/MV6HXgUcjxSLJG7I6EMrKcEEdCDRl1D29KrTva9v1DN8T9Wr0K1r25vySPe6K810Px9cWaiDVFe6iGNsq48xQB0P97tyTnryaNc8fXF4pg0tXtYjndK2PMYEdB/d78g56cisv7LxHtOS2nfob/23hPZe0vr26/5Fn4lyIZNNjDqXUSMVzyAduDj3wfyNeh14FJI8sjSSOzu5LMzHJJPUk177WuY0PYUqVO97X/QwyjE/Wa9eta1+X8mj//Z"
            ;

        public static void EnableDebug()
        {
            AllocConsole();

            Trace.Listeners.Add(new ConsoleTraceListener());
            Trace.Listeners.Add(new TextWriterTraceListener(File.Open("Debug.log", FileMode.Append)));
            Trace.AutoFlush = true;
            
            Colorful.Console.WriteWithGradient($"==WELCOME TO ORIGIND {Config.LauncherVersion} DEBUGGER==", Color.Yellow, Color.Fuchsia, 14);
            //PrintFromBase64(LogoBase64);
            Trace.WriteLine("");
            Trace.WriteLine("");
        }
        private static void PrintFromBase64(string base64)
        {
            using (var ms = new MemoryStream(Convert.FromBase64String(base64)))
            {
                var bm = new Bitmap(ms);
                ConsoleWriteImage(bm);
            }
        }
        // https://stackoverflow.com/questions/33538527/display-a-image-in-a-console-application
        public static void ConsoleWriteImage(Bitmap bmpSrc)
        {
            int sMax = 39;
            decimal percent = Math.Min(decimal.Divide(sMax, bmpSrc.Width), decimal.Divide(sMax, bmpSrc.Height));
            Size resSize = new Size((int)(bmpSrc.Width * percent), (int)(bmpSrc.Height * percent));
            Func<System.Drawing.Color, int> ToConsoleColor = c =>
            {
                int index = ((c.R > 128) | (c.G > 128) | (c.B > 128)) ? 8 : 0;
                index |= (c.R > 64) ? 4 : 0;
                index |= (c.G > 64) ? 2 : 0;
                index |= (c.B > 64) ? 1 : 0;
                return index;
            };
            Bitmap bmpMin = new Bitmap(bmpSrc, resSize);
            for (int i = 0; i < resSize.Height; i++)
            {
                for (int j = 0; j < resSize.Width; j++)
                {
                    Console.ForegroundColor = (ConsoleColor)ToConsoleColor(bmpMin.GetPixel(j, i));
                    Console.Write("██");
                }
                System.Console.WriteLine();
            }
        }
    }
}
