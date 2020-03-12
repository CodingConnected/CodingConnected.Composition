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
        internal static Dictionary<Type, object> SingletonInstances { get; } = new Dictionary<Type, object>();

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

        internal static object GetSingleton(Type type)
        {
            if (!SingletonInstances.ContainsKey(type))
            {
                SingletonInstances.Add(type, Activator.CreateInstance(type));
            }

            return SingletonInstances[type];
        }

        public static void Compose(object root)
        {
            if (root == null) return;

            foreach (var prop in root.GetType().GetProperties())
            {
                var attribs = prop.GetCustomAttributes().ToArray();
                if (attribs.Length == 0 && !prop.PropertyType.IsValueType && prop.PropertyType != typeof(string))
                {
                    Compose(prop.GetValue(root));
                }
                foreach (var attrib in attribs)
                {
                    switch (attrib)
                    {
                        case ImportAttribute ia:
                        {
                            var exT = ExportedTypes.Where(x => x.ExposedType == prop.PropertyType).ToArray();
                            if (exT.Length == 0)
                            {
                                throw new Exception($"No exported type matched imported type {prop.PropertyType.FullName}");
                            }
                            if (exT.Length > 1)
                            {
                                throw new AmbiguousMatchException($"More than one exported types match imported type {prop.PropertyType.FullName}");
                            }
                            var instance = GetSingleton(exT[0].ActualType);
                            prop.SetValue(root, instance);
                            Compose(instance);
                            break;
                        }
                        case ImportManyAttribute ima:
                        {
                            var exT = ExportedTypes.Where(x => x.ExposedType == ima.ImportedType).ToArray();
                            if (exT.Length == 0)
                            {
                                throw new Exception($"No exported type matched imported type {ima.ImportedType.FullName}");
                            }
                            var constructedListType = typeof(List<>).MakeGenericType(ima.ImportedType);
                            var instance = (IList)Activator.CreateInstance(constructedListType);
                            foreach (var t in exT)
                            {
                                instance.Add(Activator.CreateInstance(t.ActualType));
                            }
                            prop.SetValue(root, instance);
                            Compose(instance);
                            break;
                        }
                    }
                }
            }
        }
    }
}
