using System.Windows;
using System.Windows.Input;
using CodingConnected.Composition.Example.WPF.MVVM;

namespace CodingConnected.Composition.Example.WPF.Core.InternalPlugins
{
    public class ExamplePluginToolBarViewModel : ViewModelBase
    {
        #region Commands

        public ICommand ToolBarButtonCommand { get; } = new RelayCommand(x =>
        {
            MessageBox.Show("Internal example plugin toolbar item clicked");
        });

        #endregion // Commands
    }
}
