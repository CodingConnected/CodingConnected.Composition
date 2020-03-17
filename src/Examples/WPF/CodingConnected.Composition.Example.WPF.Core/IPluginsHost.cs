using CodingConnected.Composition.Example.WPF.IPlugins;
using System.Collections.Generic;

namespace CodingConnected.Composition.Example.WPF.Core
{
    public interface IPluginsHost
    {
        List<IPlugin> Plugins { get; }
    }
}