using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace OrigindLauncher.Resources.UI
{
    public static class ImageHelper
    {
        public static ImageSource GetImageSource(string source)
        {
            return new BitmapImage(new Uri(source, UriKind.RelativeOrAbsolute));
        }
    }
}