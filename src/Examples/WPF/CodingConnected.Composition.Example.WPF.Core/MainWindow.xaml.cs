using CodingConnected.Composition.Example.WPF.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
                    if (pl is IToolbar itb)
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
