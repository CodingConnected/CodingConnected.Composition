using System.Windows;
using System.Windows.Media;

namespace CodingConnected.Composition.Example.WPF.IPlugins
{
    public interface ITabItem
    {
        string DisplayName { get; }
        ImageSource Icon { get; }
        DataTemplate ContentDataTemplate { get; }
        bool IsTabItemEnabled { get; set; }
    }
}