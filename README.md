# CodingConnected.Composition
CodingConnected.Composition is a minuscule parts composition 
library written in C#. 

This library works much like MEF, only with almost 100% fewer 
features. On the bright side, the library is written as a .NET 
standard 2.0 class library, and is thus compatible with
both .NET Framework and .NET Core.

It is not my intent to recreate MEF, but rather to have a very 
compact and easy to use library to add plugin and composition 
functionality to WPF and .NET Core apps I am working on. 

The library currently supports:

- Exporting types from an assembly
- Loading exported types from one or more assemblies
- Importing types:
  - When <code>[Import]</code> is applied to a property, it will 
    be set to an instance of its type. An exception will be raised
    if more than one option is available. The property must be
    public and have a setter.
  - When <code>[ImportMany]</code> is applied to a property, all
    matching types will be instantied and put in a <code>List\<T\></code>,
    to which the given property will then be set. The property must be
    public, have a setter and be of a generic type (<code>List\<T\></code>
    or <code>IEnumerable\<T\></code>).
  - Note: recursive composition is supported, however, only for properties that
    are set by composition themselves and for generic types. A regular object 
    will not be queried for properties marked for import. It is of course 
    possible to manually call <code>Composer.Compose()</code> on any object.

This is a limited set of possibilities; and this is so by design. The goal 
is to provide some options for composition in relatively simple settings. 

<u>An important limitation to note</u>: any libraries that an external class
might need, have to be available in the main application. Thoughts on 
dynamically loading dependencies are very welcome.

The project has two example projects that outline potential ways in
which its capabilities might be leveraged. 

- A .NET framework console application, that basically follows along 
  the same lines as the example in 
[this MEF tutorial](https://docs.microsoft.com/en-us/dotnet/framework/mef/).
- A .NET core WPF application illustrating a way in which one could build
  a simple application that can dynamically load tabs (or other controls), 
  menu items and toolbars from plugins.

Contributions are welcome. (We need a test suite!)