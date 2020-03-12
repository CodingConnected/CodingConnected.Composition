# CodingConnected.Composition
CodingConnected.Composition is a minuscule parts composition 
library written in C#. 

This library works much like MEF, only with almost 100% fewer 
features. On the bright side, the library is written as a .NET 
standard 2.0 class library, and is thus compatible with
both .NET Framework and .NET Core.

This is project in the making and will be expanded in the 
unforeseeable future. Having said that, the future is unforeseeable
by definition. It is not my intent to recreate MEF, but
rather to have a very compact and easy to use library to add
plugin and composition functionality to WPF and .NET Core apps
I am working on. 

The library currently facilitates mostly a single use case:

- Allowing an application to dynamically load classes, either 
into a single property or into a list. When it is a single 
property, a singleton is currently assumed. Currently, 
exported classes should not have depencies not shared with 
the main application.

For now, see the example application for, well, an example. It 
basically follows along the same lines as the example in 
[this MEF tutorial](https://docs.microsoft.com/en-us/dotnet/framework/mef/).

Contributions are welcome.