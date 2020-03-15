using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CodingConnected.Composition.Example.WPF.Plugins
{
    public interface IPlugin
    {
        IEnumerable<Type> GetPluginTypes();
    }

    public interface ITabItem
    {
        string DisplayName { get; }
        ImageSource Icon { get; }
        DataTemplate ContentDataTemplate { get; }
        bool IsEnabled { get; set; }
    }

    public interface IToolbar
    {
        UserControl ToolBarView { get; }
        bool IsEnabled { get; set; }
    }

    public interface IMenuItem
    {
        MenuItem Menu { get; }
    }
}
