using System.Windows;
using iOverlay.Logic.SaveLoad;

namespace iOverlay;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static SaveLogic.ValorantSettings? ValorantSettings { get; set; }
    public static SaveLogic.SpotifySettings? SpotifySettings { get; set; }
}