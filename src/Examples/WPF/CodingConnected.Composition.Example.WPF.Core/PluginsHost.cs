using CodingConnected.Composition.Annotations;
using CodingConnected.Composition.Example.WPF.Plugins;
using System.Collections.Generic;
using System.ComponentModel;

namespace CodingConnected.Composition.Example.WPF.Core
{
    public class PluginsHost : IPluginsHost
    {
        private static IPluginsHost _default;

        [Browsable(false)]
        public static IPluginsHost Default => _default ?? (_default = new PluginsHost());

        [ImportMany(typeof(IPlugin))]
        public List<IPlugin> Plugins { get; private set; }
    }
}
