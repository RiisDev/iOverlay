using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Pipes;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;
using MessageBox = System.Windows.MessageBox;

namespace iOverlay.Logic;

public static class MiscLogic
{
    public static Dictionary<int, Color> PercentToColour { get; } = new()
    {
        { 0, Color.FromRgb(255, 0, 0) },
        { 1, Color.FromRgb(255, 3, 0) },
        { 2, Color.FromRgb(255, 6, 0) },
        { 3, Color.FromRgb(255, 9, 0) },
        { 4, Color.FromRgb(255, 13, 0) },
        { 5, Color.FromRgb(255, 16, 0) },
        { 6, Color.FromRgb(255, 19, 0) },
        { 7, Color.FromRgb(255, 23, 0) },
        { 8, Color.FromRgb(255, 26, 0) },
        { 9, Color.FromRgb(255, 29, 0) },
        { 10, Color.FromRgb(255, 33, 0) },
        { 11, Color.FromRgb(255, 36, 0) },
        { 12, Color.FromRgb(255, 39, 0) },
        { 13, Color.FromRgb(255, 42, 0) },
        { 14, Color.FromRgb(255, 46, 0) },
        { 15, Color.FromRgb(255, 49, 0) },
        { 16, Color.FromRgb(255, 52, 0) },
        { 17, Color.FromRgb(255, 56, 0) },
        { 18, Color.FromRgb(255, 59, 0) },
        { 19, Color.FromRgb(255, 62, 0) },
        { 20, Color.FromRgb(255, 66, 0) },
        { 21, Color.FromRgb(255, 69, 0) },
        { 22, Color.FromRgb(255, 72, 0) },
        { 23, Color.FromRgb(255, 75, 0) },
        { 24, Color.FromRgb(255, 79, 0) },
        { 25, Color.FromRgb(255, 82, 0) },
        { 26, Color.FromRgb(255, 85, 0) },
        { 27, Color.FromRgb(255, 89, 0) },
        { 28, Color.FromRgb(255, 92, 0) },
        { 29, Color.FromRgb(255, 95, 0) },
        { 30, Color.FromRgb(255, 99, 0) },
        { 31, Color.FromRgb(255, 102, 0) },
        { 32, Color.FromRgb(255, 105, 0) },
        { 33, Color.FromRgb(255, 108, 0) },
        { 34, Color.FromRgb(255, 112, 0) },
        { 35, Color.FromRgb(255, 115, 0) },
        { 36, Color.FromRgb(255, 118, 0) },
        { 37, Color.FromRgb(255, 122, 0) },
        { 38, Color.FromRgb(255, 125, 0) },
        { 39, Color.FromRgb(255, 128, 0) },
        { 40, Color.FromRgb(255, 132, 0) },
        { 41, Color.FromRgb(255, 135, 0) },
        { 42, Color.FromRgb(255, 138, 0) },
        { 43, Color.FromRgb(255, 141, 0) },
        { 44, Color.FromRgb(255, 145, 0) },
        { 45, Color.FromRgb(255, 148, 0) },
        { 46, Color.FromRgb(255, 151, 0) },
        { 47, Color.FromRgb(255, 155, 0) },
        { 48, Color.FromRgb(255, 158, 0) },
        { 49, Color.FromRgb(255, 161, 0) },
        { 50, Color.FromRgb(255, 165, 0) },
        { 51, Color.FromRgb(255, 170, 6) },
        { 52, Color.FromRgb(255, 174, 11) },
        { 53, Color.FromRgb(254, 179, 17) },
        { 54, Color.FromRgb(254, 183, 23) },
        { 55, Color.FromRgb(254, 188, 29) },
        { 56, Color.FromRgb(254, 192, 34) },
        { 57, Color.FromRgb(254, 197, 40) },
        { 58, Color.FromRgb(254, 201, 46) },
        { 59, Color.FromRgb(253, 206, 52) },
        { 60, Color.FromRgb(253, 210, 57) },
        { 61, Color.FromRgb(253, 213, 61) },
        { 62, Color.FromRgb(253, 214, 62) },
        { 63, Color.FromRgb(253, 215, 63) },
        { 64, Color.FromRgb(249, 216, 64) },
        { 65, Color.FromRgb(245, 217, 65) },
        { 66, Color.FromRgb(241, 217, 66) },
        { 67, Color.FromRgb(237, 218, 67) },
        { 68, Color.FromRgb(233, 219, 68) },
        { 69, Color.FromRgb(229, 220, 69) },
        { 70, Color.FromRgb(224, 221, 70) },
        { 71, Color.FromRgb(220, 221, 71) },
        { 72, Color.FromRgb(216, 222, 72) },
        { 73, Color.FromRgb(212, 223, 73) },
        { 74, Color.FromRgb(208, 224, 74) },
        { 75, Color.FromRgb(204, 225, 75) },
        { 76, Color.FromRgb(200, 225, 76) },
        { 77, Color.FromRgb(196, 226, 77) },
        { 78, Color.FromRgb(192, 227, 78) },
        { 79, Color.FromRgb(188, 228, 79) },
        { 80, Color.FromRgb(183, 229, 80) },
        { 81, Color.FromRgb(179, 229, 81) },
        { 82, Color.FromRgb(175, 230, 82) },
        { 83, Color.FromRgb(171, 231, 83) },
        { 84, Color.FromRgb(167, 232, 84) },
        { 85, Color.FromRgb(163, 233, 85) },
        { 86, Color.FromRgb(159, 233, 86) },
        { 87, Color.FromRgb(155, 234, 87) },
        { 88, Color.FromRgb(151, 235, 88) },
        { 89, Color.FromRgb(147, 236, 89) },
        { 90, Color.FromRgb(142, 237, 90) },
        { 91, Color.FromRgb(138, 237, 91) },
        { 92, Color.FromRgb(134, 238, 92) },
        { 93, Color.FromRgb(130, 239, 93) },
        { 94, Color.FromRgb(126, 240, 94) },
        { 95, Color.FromRgb(122, 241, 95) },
        { 96, Color.FromRgb(118, 241, 96) },
        { 97, Color.FromRgb(114, 242, 97) },
        { 98, Color.FromRgb(110, 243, 98) },
        { 99, Color.FromRgb(106, 244, 99) },
        { 100, Color.FromRgb(101, 245, 100) }
    };

    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    public static void AnimateProgress(this ProgressRing bar, int newProgress)
    {
        new Task(() =>
        {
            bar.Dispatcher.Invoke(async () =>
            {
                bool increment = newProgress >= bar.Progress;

                while (true)
                {
                    if (Math.Floor(bar.Progress) == newProgress) break;

                    bar.Progress += increment ? 1 : -1;
                    bar.Foreground = PercentToColour[(int)bar.Progress].ToBrush();
                    await Task.Delay(20);
                }
            });
        }).Start();
    }

    public static void AnimateSessionRankRating(this Label label, int rating)
    {
        new Task(() =>
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
        }).Start();
    }

    public static void AnimateRankRating(this Label label, int rating)
    {
        new Task(() =>
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
                    label.Foreground = PercentToColour[currentValue + newValue].ToBrush();

                    await Task.Delay(20);
                }
            });
        }).Start();
    }

    public static void ApplyDraggable(this Window window)
    {
        window.MouseDown += (_, mouseButtonEventArgs) =>
        {
            if (mouseButtonEventArgs.ChangedButton == MouseButton.Left)
                window.DragMove();
        };
    }

    public static JsonElement? GetPropertyNullable(this JsonElement jsonElement, string propertyName)
    {
        try
        {
            if (jsonElement.TryGetProperty(propertyName, out JsonElement returnedElement)) return returnedElement;

            return null;
        }
        catch { return null; }
    }

    public static async Task<string?> GetTrackerJson(string url)
    {
        Process botProcess = new();
        string cachedData;

        return await Task.Run(() =>
        {
            botProcess.StartInfo = new ProcessStartInfo
            {
                FileName = "Assets\\BypassCF.exe",
                Arguments = $"\"{url}\"",
                CreateNoWindow = true
            };
            botProcess.Start();
            // Not changing my old pipe name sorry!
            using NamedPipeServerStream pipeServer = new("ECAC_BOT_PIPE", PipeDirection.In,
                NamedPipeServerStream.MaxAllowedServerInstances);
            pipeServer.WaitForConnection();
            using StreamReader reader = new(pipeServer);
            cachedData = reader.ReadToEnd();
            botProcess.Dispose();
            pipeServer.Dispose();
            return cachedData;
        });
    }
}