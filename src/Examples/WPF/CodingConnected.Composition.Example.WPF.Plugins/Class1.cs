using System;
using System.Collections.Generic;

namespace CodingConnected.Composition.Example.WPF.Plugins
{
    public interface IPlugin
    {
        IEnumerable<Type> GetPluginTypes();
    }

    public interface ITabItem
    {

    }

    public interface IToolbar
    {

    }

    public interface IMenuItem
    {
        
    }
}
