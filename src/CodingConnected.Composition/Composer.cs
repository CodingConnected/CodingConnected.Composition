using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodingConnected.Composition.Annotations;

namespace CodingConnected.Composition
{
    internal class ExportedType
    {
        public Type ActualType { get; }
        public Type ExposedType { get; }

        public ExportedType(Type actualType, Type exposedType)
        {
            ActualType = actualType;
            ExposedType = exposedType;
        }
    }

    public static class Composer
    {
        internal static List<ExportedType> ExportedTypes { get; } = new List<ExportedType>();

        public static void LoadExports(Assembly assembly)
        {
            foreach (var type in assembly.GetExportedTypes())
            {
                var custom = type.GetCustomAttributes();
                foreach (var attrib in custom.Where(x => x is ExportAttribute))
                {
                    ExportedTypes.Add(new ExportedType(type, ((ExportAttribute)attrib).ExportedType));
                }
            }
        }

        public static void Compose(object root)
        {
            foreach (var prop in root.GetType().GetProperties())
            {
                var attribs = prop.GetCustomAttributes();
                foreach (var attrib in attribs)
                {
                    if (attrib is ImportAttribute ia)
                    {
                        var exT = ExportedTypes.FirstOrDefault(x => x.ExposedType == prop.PropertyType);
                        if (exT == null)
                        {
                            throw new Exception();
                        }
                        var instance = Activator.CreateInstance(exT.ActualType);
                        prop.SetValue(root, instance);
                        Compose(instance);
                    }
                    if (attrib is ImportManyAttribute ima)
                    {
                        var exT = ExportedTypes.Where(x => x.ExposedType == ima.ImportedType);
                        if (!exT.Any())
                        {
                            throw new Exception();
                        }
                        var listType = typeof(List<>);
                        var constructedListType = listType.MakeGenericType(ima.ImportedType);
                        var instance = (IList)Activator.CreateInstance(constructedListType);
                        foreach (var t in exT)
                        {
                            instance.Add(Activator.CreateInstance(t.ActualType));
                        }
                        prop.SetValue(root, instance);
                        Compose(instance);
                    }
                }
            }
        }
    }
}
