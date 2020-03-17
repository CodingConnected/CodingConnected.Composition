using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using CodingConnected.Composition.Annotations;

namespace CodingConnected.Composition
{
    internal class ExportedType
    {
        public Type ActualType { get; }
        public Type ExposedType { get; }
        public bool Many { get; }

        public ExportedType(Type actualType, Type exposedType, bool many)
        {
            ActualType = actualType;
            ExposedType = exposedType;
            Many = many;
        }
    }

    public static class Composer
    {
        internal static List<ExportedType> ExportedTypes { get; } = new List<ExportedType>();
        internal static Dictionary<Type, object> SingletonInstances { get; } = new Dictionary<Type, object>();

        /// <summary>
        /// Loads exported types from a given assembly
        /// </summary>
        /// <param name="assembly">The assembly to search for exported types</param>
        public static void LoadExports(Assembly assembly)
        {
            foreach (var type in assembly.GetExportedTypes())
            {
                var custom = type.GetCustomAttributes();
                foreach (var attrib in custom)
                {
                    switch (attrib)
                    {
                        case ExportAttribute ea:
                            ExportedTypes.Add(new ExportedType(type, ea.ExportedType, false));
                            break;
                        case ExportManyAttribute ema:
                            ExportedTypes.Add(new ExportedType(type, ema.ExportedType, true));
                            break;
                    }
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

        /// <summary>
        /// Compose an object: all properties of the object decorated with an
        /// Import or ImportMany attribute, will be instantiated accordingly
        /// </summary>
        /// <param name="root">The object to compose</param>
        public static void Compose(object root)
        {
            if (root == null) return;

            foreach (var prop in root.GetType().GetProperties())
            {
                var browsable = prop.GetCustomAttribute<BrowsableAttribute>();
                if (browsable != null && !browsable.Browsable) continue;

                var attribs = prop.GetCustomAttributes().ToArray();
                if (attribs.Length == 0 && !prop.PropertyType.IsValueType && prop.PropertyType != typeof(string))
                {
                    // Recursive composition is only allowed for generic types for now
                    if (prop.PropertyType.IsGenericType)
                    {
                        Compose(prop.GetValue(root));
                    }
                }

                foreach (var attrib in attribs)
                {
                    ExportedType[] exportedTypes;
                    switch (attrib)
                    {
                        case ImportAttribute ia:
                            exportedTypes = ExportedTypes.Where(x => x.ExposedType == prop.PropertyType).ToArray();
                            if (exportedTypes.Length == 0)
                            {
                                throw new Exception($"No exported type matched imported type {prop.PropertyType.FullName}");
                            }
                            if (exportedTypes.Length > 1)
                            {
                                throw new AmbiguousMatchException($"More than one exported types match imported type {prop.PropertyType.FullName}");
                            }
                            object instance;
                            switch (exportedTypes[0].Many)
                            {
                                case true:
                                    instance = Activator.CreateInstance(exportedTypes[0].ActualType);
                                    break;
                                default:
                                    instance = GetSingleton(exportedTypes[0].ActualType);
                                    break;
                            }
                            prop.SetValue(root, instance);
                            Compose(instance);
                            break;
                        case ImportManyAttribute ima:
                            if (!prop.PropertyType.IsGenericType)
                            {
                                throw new Exception("ImportMany can only be applied on generic types");
                            }

                            exportedTypes = ExportedTypes.Where(x => x.ExposedType == ima.ImportedType).ToArray();
                            if (exportedTypes.Length == 0)
                            {
                                throw new Exception($"No exported type matched imported type {ima.ImportedType.FullName}");
                            }
                            var constructedListType = typeof(List<>).MakeGenericType(ima.ImportedType);
                            var instanceList = (IList)Activator.CreateInstance(constructedListType);
                            foreach (var t in exportedTypes)
                            {
                                var listItem = Activator.CreateInstance(t.ActualType);
                                Compose(listItem);
                                instanceList.Add(listItem);
                            }

                            try
                            {
                                prop.SetValue(root, instanceList);
                            }
                            catch (ArgumentException)
                            {
                                throw new Exception("No setter found for property " + prop.Name + ", or wrong type");
                            }
                            break;
                    }
                }
            }
        }
    }
}
