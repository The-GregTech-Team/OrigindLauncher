using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;

namespace OrigindLauncher.UI.Code
{
    public static class ProgressBarAnime
    {
        public static void AnimeAddAsPercent(this ProgressBar progressBar, double value)
        {
            var toValue = progressBar.Maximum * value + progressBar.Value;
            if (toValue > progressBar.Maximum) throw new InvalidOperationException("无法更改 ProgressBar.Value, 这是一个代码bug");

            var anime = new DoubleAnimation
            {
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = new SineEase {EasingMode = EasingMode.EaseInOut},
                From = progressBar.Value,
                To = toValue
            };

            var sb = new Storyboard
            {
                Children = {anime}
            };

            Storyboard.SetTargetProperty(anime, new PropertyPath(RangeBase.ValueProperty));

            sb.Begin(progressBar);
        }

        public static void AnimeSubstractAsPercent(this ProgressBar progressBar, double value)
        {
            var toValue = progressBar.Maximum - progressBar.Maximum * value;
            if (toValue > progressBar.Maximum) throw new InvalidOperationException("无法更改 ProgressBar.Value, 这是一个代码bug");

            var anime = new DoubleAnimation
            {
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = new SineEase {EasingMode = EasingMode.EaseInOut},
                From = progressBar.Value,
                To = toValue
            };

            var sb = new Storyboard
            {
                Children = {anime}
            };

            Storyboard.SetTargetProperty(anime, new PropertyPath(RangeBase.ValueProperty));

            sb.Begin(progressBar);
        }

        public static void AnimeToValueAsPercent(this ProgressBar progressBar, double value)
        {
            var toValue = progressBar.Maximum * value;
            if (Math.Abs(toValue - progressBar.Value) < 0.01) return;

            if (toValue > progressBar.Value)
                AnimeAddAsPercent(progressBar, toValue - progressBar.Value);
            else if (toValue < progressBar.Value)
                AnimeSubstractAsPercent(progressBar, progressBar.Value - toValue);
        }
    }
}