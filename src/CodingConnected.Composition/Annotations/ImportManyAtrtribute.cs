﻿using System;

namespace CodingConnected.Composition.Annotations
{
    public class ImportManyAttribute : Attribute
    {
        public Type ImportedType { get; }

        public ImportManyAttribute(Type importedType)
        {
            ImportedType = importedType;
        }
    }
}
