﻿using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace OrigindLauncher.UI.Code
{
    public static class Fly
    {
        private static Storyboard CreateFlyOutStoryboard(Window window, TimeSpan duration)
        {
            var anime = new DoubleAnimation
            {
                Duration = duration,
                EasingFunction = new CircleEase {EasingMode = EasingMode.EaseIn},
                From = double.IsNaN(window.Top) ? 0 : window.Top,
                To = double.IsNaN(window.Top + SystemParameters.PrimaryScreenHeight)
                    ? 2000
                    : window.Top + SystemParameters.PrimaryScreenHeight
            };

            var sb = new Storyboard
            {
                Children = {anime}
            };

            Storyboard.SetTargetProperty(anime, new PropertyPath(Window.TopProperty));

            return sb;
        }


        public static void FlyOutWindow(Window window, TimeSpan? aDuration = null, Action callback = null)
        {
            var duration = aDuration ?? TimeSpan.FromMilliseconds(400);
            var sb = CreateFlyOutStoryboard(window, duration);
            sb.Completed += (sender, args) => { callback?.Invoke(); };
            sb.Begin(window);
        }
    }

    public static class WindowFlyHelper
    {
        public static void Flyout(this Window window, Action callback = null, TimeSpan? aDuration = null)
        {
            Fly.FlyOutWindow(window, aDuration, callback);
        }

        public static void FlyoutAndClose(this Window window, Action callback = null)
        {
            Fly.FlyOutWindow(window, null, () =>
            {
                window.Dispatcher.Invoke(window.Close);
                callback?.Invoke();
            });
        }
    }
}