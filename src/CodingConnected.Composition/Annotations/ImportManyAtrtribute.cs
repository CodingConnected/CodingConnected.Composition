using System;

namespace CodingConnected.Composition.Annotations
{
    /// <summary>
    /// When tagged, a property will be set to a <code>List</code> or
    /// <code>IEnumerable</code> of type <code>ImportedType</code>. Each
    /// exported type that is exported as this type will be added to the
    /// list.
    /// </summary>
    public class ImportManyAttribute : Attribute
    {
        public Type ImportedType { get; }

        public ImportManyAttribute(Type importedType)
        {
            ImportedType = importedType;
        }
    }
}
