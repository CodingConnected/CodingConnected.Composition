using System;

namespace CodingConnected.Composition.Annotations
{
    /// <summary>
    /// Marks a type as exported, which makes it available for import
    /// </summary>
    public class ExportAttribute : Attribute
    {
        public Type ExportedType { get; }

        public ExportAttribute(Type exportedType)
        {
            ExportedType = exportedType;
        }
    }
}
