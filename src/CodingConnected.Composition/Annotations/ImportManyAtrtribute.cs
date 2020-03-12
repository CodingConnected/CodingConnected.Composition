using System;

namespace CodingConnected.Composition.Annotations
{
    /// <summary>
    /// When tagged, 
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
