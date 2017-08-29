using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace OrigindLauncher.UI.Code
{
    public static class Fade
    {
        public static void FadeOut(this UIElement element)
        {
            var da = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(200)));

            element.BeginAnimation(UIElement.OpacityProperty, da);

        }

        public static void FadeIn(this UIElement element)
        {
            var da = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(500)));

            element.BeginAnimation(UIElement.OpacityProperty, da);

        }
    }
}
