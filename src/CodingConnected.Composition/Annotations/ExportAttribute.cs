using System;

namespace CodingConnected.Composition.Annotations
{
    public class ExportAttribute : Attribute
    {
        public Type ExportedType { get; }

        public ExportAttribute(Type exportedType)
        {
            ExportedType = exportedType;
        }
    }
}
