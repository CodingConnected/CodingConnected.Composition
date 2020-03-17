using System;
using System.Collections.Generic;

namespace CodingConnected.Composition.Example.WPF.IPlugins
{
    public interface IPlugin
    {
        IEnumerable<Type> GetPluginTypes();
    }
}
