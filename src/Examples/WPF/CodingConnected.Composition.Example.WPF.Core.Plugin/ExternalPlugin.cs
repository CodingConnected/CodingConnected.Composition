using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CodingConnected.Composition.Annotations;
using CodingConnected.Composition.Example.WPF.IPlugins;
using CodingConnected.Composition.Example.WPF.MVVM;

namespace CodingConnected.Composition.Example.WPF.Core.Plugin
{
    [Export(typeof(IPlugin))]
    public class ExternalPlugin : IPlugin, ITabItem, IMenuItem, IToolBar
    {
        #region Fields

        private DataTemplate _contentDataTemplate;
        private MenuItem _menu;
        private ImageSource _icon;

        #endregion // Fields

        #region IPlugin

        public IEnumerable<Type> GetPluginTypes()
        {
            return new List<Type> { typeof(ITabItem), typeof(IMenuItem) };
        }

        #endregion // IPlugin

        #region ITabItem

        public string DisplayName => "External";

        public ImageSource Icon
        {
            get
            {
                if (_icon == null)
                {
                    var dict = new ResourceDictionary();
                    var u = new Uri("pack://application:,,,/" +
                                    System.Reflection.Assembly.GetExecutingAssembly().GetName().Name +
                                    ";component/" + "Resources/Icons.xaml");
                    dict.Source = u;
                    _icon = (DrawingImage) dict["ExternalDrawingImage"];
                }

                return _icon;
            }
        }

        public DataTemplate ContentDataTemplate
        {
            get
            {
                if (_contentDataTemplate == null)
                {
                    _contentDataTemplate = new DataTemplate();
                    var tab = new FrameworkElementFactory(typeof(ExternalPluginTabView));
                    tab.SetValue(FrameworkElement.DataContextProperty, new ExternalPluginTabViewModel());
                    _contentDataTemplate.VisualTree = tab;
                }

                return _contentDataTemplate;
            }
        }

        public bool IsTabItemEnabled { get; set; } = true;

        #endregion // ITabItem

        #region IMenuItem
        
        public MenuItem Menu => _menu ??= new MenuItem
        {
            Header = "External plugin",
            Icon = new Image{Source = Icon},
            Command = new RelayCommand(x =>
            {
                MessageBox.Show("External example plugin menu item clicked");
            })
        };

        #endregion // IMenuItem

        #region IToolBar

        public UserControl ToolBarView => new ExternalPluginToolBarView
        {
            DataContext = new ExternalPluginToolBarViewModel()
        };

        public bool IsToolBarEnabled { get; set; } = true;
        
        #endregion // IToolBar
    }
}
