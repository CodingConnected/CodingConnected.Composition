using System;

namespace CodingConnected.Composition.Annotations
{
    /// <summary>
    /// When tagged, a property will be set to a <c>List</c> or
    /// <c>IEnumerable</c> of type <c>ImportedType</c>. Each exported
    /// type that is exported as this type will be added to the list.
    /// <para>
    /// <remarks><b>Note:</b> <c>ImportedType</c> must be a generic type</remarks>
    /// </para>
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
