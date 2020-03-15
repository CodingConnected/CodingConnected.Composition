using CodingConnected.Composition.Example.WPF.MVVM;
using CodingConnected.Composition.Example.WPF.Plugins;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CodingConnected.Composition.Example.WPF.Core
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Fields

        private ITabItem selectedTab;
        private RelayCommand _exitApplicationCommand;

        #endregion // Fields

        #region Properties

        public ObservableCollection<ITabItem> TabItems { get; } = new ObservableCollection<ITabItem>();

        public ITabItem SelectedTab
        {
            get => selectedTab;
            set
            {
                selectedTab = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<MenuItem> MenuItems { get; } = new ObservableCollection<MenuItem>();

        #endregion // Properties

        #region Commands

        public ICommand ExitApplicationCommand => _exitApplicationCommand ?? (_exitApplicationCommand = new RelayCommand(x => Application.Current.Shutdown()));

        #endregion // Commands

        #region Private Methods

        private void LoadPlugins()
        {
            foreach (var plugin in PluginsHost.Default.Plugins)
            {
                var pluginTypes = plugin.GetPluginTypes();
                foreach (var pluginType in pluginTypes)
                {
                    try
                    {
                        switch (pluginType.Name)
                        {
                            case "ITabItem":
                                TabItems.Add((ITabItem)plugin);
                                break;
                            case "IMenuItem":
                                MenuItems.Add(((IMenuItem)plugin).Menu);
                                break;
                        }
                    }
                    catch
                    {
                        throw new Exception($"Plugin {plugin.GetType().FullName} does not implement {pluginType.Name}");
                    }
                }
            }
        }

        #endregion // Private Methods

        #region Constructor

        public MainWindowViewModel()
        {
            LoadPlugins();
            SelectedTab = TabItems.FirstOrDefault();
        }
        
        #endregion // Constructor
    }
}
