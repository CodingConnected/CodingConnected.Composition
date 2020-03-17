using System;

namespace CodingConnected.Composition.Annotations
{
    /// <summary>
    /// When tagged, a property will be set with an instance of a singleton of this type
    /// If multiple classes match this import, and exception will be raised
    /// <para>
    /// <remarks><b>Note:</b> the decorated property must have a generic type</remarks>
    /// </para>
    /// </summary>
    public class ImportAttribute : Attribute
    {
    }
}
