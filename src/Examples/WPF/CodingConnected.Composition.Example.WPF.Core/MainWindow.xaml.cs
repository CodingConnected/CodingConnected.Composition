using CodingConnected.Composition.Example.WPF.IPlugins;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace CodingConnected.Composition.Example.WPF.Core
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Composer.LoadExports(Assembly.GetExecutingAssembly());
            if (File.Exists("..\\..\\..\\..\\CodingConnected.Composition.Example.WPF.Core.Plugin\\bin\\Debug\\netcoreapp3.0\\CodingConnected.Composition.Example.WPF.Core.Plugin.dll"))
            {
                Composer.LoadExports(Assembly.LoadFrom("..\\..\\..\\..\\CodingConnected.Composition.Example.WPF.Core.Plugin\\bin\\Debug\\netcoreapp3.0\\CodingConnected.Composition.Example.WPF.Core.Plugin.dll"));
            }
            Composer.Compose(PluginsHost.Default);

            // Load toolbars; this must be done here because one cannot 
            // bind a collection of toolbars to a toolbartray.
            MainToolBarTray.DataContextChanged += (s, e) =>
            {
                if (!(e.NewValue is MainWindowViewModel vm)) return;
                foreach (var pl in PluginsHost.Default.Plugins)
                {
                    if (pl is IToolBar itb)
                    {
                        var tb = new ToolBar();
                        tb.Items.Add(itb.ToolBarView);
                        MainToolBarTray.ToolBars.Add(tb);
                    }
                }
            };

            this.DataContext = new MainWindowViewModel();
        }
    }
}
