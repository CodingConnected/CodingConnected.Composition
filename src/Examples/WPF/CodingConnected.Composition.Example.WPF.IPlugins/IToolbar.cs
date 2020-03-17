using System.Windows.Controls;

namespace CodingConnected.Composition.Example.WPF.IPlugins
{
    public interface IToolBar
    {
        UserControl ToolBarView { get; }
        bool IsToolBarEnabled { get; set; }
    }
}