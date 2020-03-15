using System;

namespace CodingConnected.Composition.Annotations
{
    /// <summary>
    /// Marks a type as exported, which makes it available for import.
    /// </summary>
    /// <remarks>Types marked by <code>ExportMany</code> will be instantiated 
    /// once each time they are imported. To have a type instatiated only once,
    /// use <code>Export</code></remarks>
    public class ExportManyAttribute : Attribute
    {
        public Type ExportedType { get; }

        public ExportManyAttribute(Type exportedType)
        {
            ExportedType = exportedType;
        }
    }
}
