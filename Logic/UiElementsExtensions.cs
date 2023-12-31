using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using iOverlay.Logic.WidgetLogic;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;
// ReSharper disable CompareOfFloatsByEqualityOperator

namespace iOverlay.Logic
{
    internal static class UiElementsExtensions
    {
        public static void ApplyDraggable(this Window window)
        {
            window.MouseDown += (_, mouseButtonEventArgs) =>
            {
                if (mouseButtonEventArgs.ChangedButton == MouseButton.Left)
                    window.DragMove();
            };
        }

        public static void SafeSet(this UIElement controlObject, dynamic value)
        {
            switch (controlObject)
            {
                case Label label:
                    label.Dispatcher.Invoke(() => { label.Content = value; });
                    break;
            }
        }


        public static void AnimateProgress(this ProgressRing bar, long newProgress)
        {
            Task.Run(() =>
            {
                bar.Dispatcher.Invoke(async () =>
                {
                    bool increment = newProgress >= bar.Progress;

                    while (true)
                    {
                        if (Math.Floor(bar.Progress) == newProgress) break;

                        bar.Progress += increment ? 1 : -1;
                        bar.Progress = bar.Progress > 0 ? bar.Progress : 0;
                        bar.Foreground = InternalValorantLogic.PercentToColour[(int)bar.Progress].ToBrush();
                        await Task.Delay(20);
                    }
                });
            });
        }

        public static void AnimateSessionRankRating(this Label label, long rating)
        {
            Task.Run(() =>
            {
                label.Dispatcher.Invoke(async () =>
                {
                    int currentProgress = int.Parse(label.Content?.ToString()?.Replace("-", "").Replace("+", "").Trim() ?? "0");
                    bool increment = rating >= currentProgress;

                    while (true)
                    {
                        int currentValue = int.Parse(label.Content?.ToString()?.Replace("-", "").Replace("+", "").Trim() ?? "0");
                        int newValue = increment ? 1 : -1;
                        int newRating = currentValue + newValue;

                        if (currentValue == rating) break;

                        label.Content = newRating < 0 ? $"-{newRating}" : $"+{newRating}";
                        label.Foreground = newRating < 0
                            ? new SolidColorBrush(Color.FromRgb(255, 29, 0))
                            : new SolidColorBrush(Color.FromRgb(101, 245, 100));

                        await Task.Delay(20);
                    }
                });
            });
        }

        public static void AnimateRankRating(this Label label, long rating)
        {
            Task.Run(() =>
            {
                label.Dispatcher.Invoke(async () =>
                {
                    int currentProgress = int.Parse(label.Content?.ToString()?.Replace("RR", "").Trim() ?? "0");
                    bool increment = rating >= currentProgress;

                    while (true)
                    {
                        int currentValue = int.Parse(label.Content?.ToString()?.Replace("RR", "").Trim() ?? "0");
                        int newValue = increment ? 1 : -1;

                        if (currentValue == rating) break;

                        label.Content = $"{currentValue + newValue} RR";
                        int colourIndex = currentValue + newValue > 0 ? currentValue + newValue : 0;
                        label.Foreground = InternalValorantLogic.PercentToColour[colourIndex].ToBrush();

                        await Task.Delay(20);
                    }
                });
            });
        }


    }
}
