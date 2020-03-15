﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CodingConnected.Composition.Annotations;
using CodingConnected.Composition.Example.WPF.MVVM;
using CodingConnected.Composition.Example.WPF.Plugins;

namespace CodingConnected.Composition.Example.WPF.Core.OwnPlugins
{
    [Export(typeof(IPlugin))]
    public class ExamplePlugin : IPlugin, ITabItem, IMenuItem
    {
        #region Fields

        private DataTemplate _contentDataTemplate;
        private MenuItem _menu;

        #endregion // Fields

        #region IPlugin

        public IEnumerable<Type> GetPluginTypes()
        {
            return new List<Type> { typeof(ITabItem), typeof(IMenuItem) };
        }

        #endregion // IPlugin

        #region ITabItem

        public string DisplayName => "Internal";

        public ImageSource Icon => null;

        public DataTemplate ContentDataTemplate
        {
            get
            {
                if (_contentDataTemplate == null)
                {
                    _contentDataTemplate = new DataTemplate();
                    var tab = new FrameworkElementFactory(typeof(ExamplePluginTabView));
                    tab.SetValue(FrameworkElement.DataContextProperty, new ExamplePluginTabViewModel());
                    _contentDataTemplate.VisualTree = tab;
                }
                return _contentDataTemplate;
            }
        }

        public bool IsEnabled { get; set; } = true;

        #endregion // ITabItem

        #region IMenuItem
        
        public MenuItem Menu => _menu ??
            (_menu = new MenuItem
            {
                Header = "Internal plugin",
                Command = new RelayCommand(x =>
                {
                    MessageBox.Show("Internal example plugin menu item clicked");
                })
            });

        #endregion // IMenuItem
    }
}
