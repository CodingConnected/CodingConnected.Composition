using System.Windows;
using System.Windows.Input;
using CodingConnected.Composition.Example.WPF.MVVM;

namespace CodingConnected.Composition.Example.WPF.Core.Plugin
{
    public class ExternalPluginToolBarViewModel : ViewModelBase
    {
        #region Commands

        public ICommand ToolBarButtonCommand { get; } = new RelayCommand(x =>
        {
            MessageBox.Show("External example plugin toolbar item clicked");
        });

        #endregion // Commands
    }
}
