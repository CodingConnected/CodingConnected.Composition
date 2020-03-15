using System;

namespace CodingConnected.Composition.Annotations
{
    /// <summary>
    /// Marks a type as exported, which makes it available for import.
    /// </summary>
    /// <remarks>Types marked by <code>Export</code> will be instantiated once.
    /// To have a type instatiated anew for each import, use <code>ExportMany</code></remarks>
    public class ExportAttribute : Attribute
    {
        public Type ExportedType { get; }

        public ExportAttribute(Type exportedType)
        {
            ExportedType = exportedType;
        }
    }
}
