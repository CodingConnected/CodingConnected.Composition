using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
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
        /// Loads exported types from an assembly as a given location
        /// </summary>
        /// <param name="assemblyPath">The path to the assembly to search for exported types</param>
        /// <param name="signedOnly">Only load exported types from this assembly if the assembly has been digitally signed</param>
        /// <param name="actionIfUnsigned">If only signed assemblies should be loaded, and this assembly is not signed, load
        /// exported types nonetheless if this action returns true</param>
        public static void LoadExports(string assemblyPath, bool signedOnly = false, Func<Assembly, bool> actionIfUnsigned = null)
        {
            LoadExports(Assembly.LoadFrom(assemblyPath), signedOnly, actionIfUnsigned);
        }

        /// <summary>
        /// Loads exported types from a given assembly
        /// </summary>
        /// <param name="assembly">The assembly to search for exported types</param>
        /// <param name="signedOnly">Only load exported types from this assembly if the assembly has been digitally signed</param>
        /// <param name="actionIfUnsigned">If only signed assemblies should be loaded, and this assembly is not signed, load
        /// exported types nonetheless if this action returns true</param>
        public static void LoadExports(Assembly assembly, bool signedOnly = false, Func<Assembly, bool> actionIfUnsigned = null)
        {
            if (signedOnly)
            {
                try
                {
                    X509Certificate.CreateFromSignedFile(assembly.Location);
                }
                catch (CryptographicException e)
                {
                    if (actionIfUnsigned == null || !actionIfUnsigned(assembly))
                    {
                        return;
                    }
                }
            }

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

        public static void AddSingleton(Type exportedAsType, object singleton)
        {
            if (ExportedTypes.Any(x => x.ExposedType == exportedAsType))
            {
                throw new Exception($"Type {exportedAsType.FullName} has already been exported");
            }
            ExportedTypes.Add(new ExportedType(typeof(object), exportedAsType, false));
            SingletonInstances.Add(typeof(object), singleton);
        }

        public static void AddExportedType(Type actualType, Type exportedAsType, bool exportMany = false)
        {
            if (ExportedTypes.Any(x => x.ExposedType == exportedAsType))
            {
                throw new Exception($"Type {exportedAsType.FullName} has already been exported");
            }
            ExportedTypes.Add(new ExportedType(actualType, exportedAsType, exportMany));
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
        /// Create and compose an object: an object of type T will be created
        /// using the first available public constructor.
        /// During creation, all generic parameters of the constructor that are
        /// not provided as arguments to this method, will be resolved by the
        /// composer.
        /// After creation the object will be composed: all properties of
        /// the object decorated with an Import or ImportMany attribute,
        /// will be instantiated accordingly.
        /// </summary>
        /// <typeparam name="T">The type of which an instance is to be constructed and composed</typeparam>
        /// <param name="nonComposedParams">Any non-generic arguments that need to be supplied to the constructor, and/or
        /// generic arguments that should not be set via composition</param>
        /// <returns></returns>
        public static T CreateAndCompose<T>(params object[] nonComposedParams)
        {
            var type = typeof(T);
            return (T) CreateAndCompose(type, nonComposedParams);
        }

        public static object CreateAndCompose(Type type, params object[] nonComposedParams)
        {
            var nonComposedParamsList = nonComposedParams.ToList();
            var constructor = type.GetConstructors().FirstOrDefault(x => x.IsPublic);
            if (constructor == null)
            {
                throw new NullReferenceException($"Type {type.FullName} contains no public constructor");
            }
            var constructorArgs = new List<object>();
            var i = 0;
            foreach (var prmInfo in constructor.GetParameters())
            {
                var prm = nonComposedParamsList.FirstOrDefault(x => x.GetType() == prmInfo.ParameterType);
                if (prm == null)
                {
                    foreach (var p in nonComposedParams)
                    {
                        var pt = p.GetType();
                        var interfaces = pt.GetInterfaces();
                        if (interfaces.Any(x => x == prmInfo.ParameterType))
                        {
                            prm = p;
                            break;
                        }
                    }
                }
                if (prm != null)
                {
                    nonComposedParamsList.Remove(prm);
                    constructorArgs.Add(prm);
                }
                else if (prmInfo.ParameterType.IsInterface)
                {
                    var singletonType = ExportedTypes.FirstOrDefault(x => x.ExposedType == prmInfo.ParameterType)?.ActualType;
                    if (singletonType == null) throw new NullReferenceException($"No type {prmInfo.ParameterType.FullName} could be matched with any exported type");
                    var singleton = GetSingleton(singletonType);
                    constructorArgs.Add(singleton);
                }
                else
                {
                    throw new Exception($"Found no matching argument for parameter {prmInfo.Name} in constructor for type {type.FullName}");
                }
            }

            var o = constructor.Invoke(constructorArgs.ToArray());

            Compose(o);

            return o;
        }

        public static object GetExportedTypeInstance(Type type)
        {
            var t = ExportedTypes.FirstOrDefault(x => x.ExposedType == type);
            if (t == null)
            {
                throw new NullReferenceException($"No type {type.FullName} has been exported");
            }
            switch (t.Many)
            {
                case true:
                    return CreateAndCompose(t.ActualType);
                default:
                    return GetSingleton(t.ActualType);
            }
        }

        /// <summary>
        /// Compose an object: all properties of the object decorated with an
        /// Import or ImportMany attribute, will be instantiated accordingly.
        /// Alternatively, if a Type if provided, it will be scanned for public
        /// static properties marked for import.
        /// </summary>
        /// <param name="root">The object to compose, or a type if composing must be done for static properties of a class</param>
        public static void Compose(object root)
        {
            if (root == null) return;

            PropertyInfo[] infos;
            object obj;

            if (root is Type rootType)
            {
                obj = null;
                infos = rootType.GetProperties(
                    BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            }
            else
            {
                obj = root;
                infos = root.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            }

            foreach (var prop in infos)
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
                                    instance = CreateAndCompose(exportedTypes[0].ActualType);
                                    break;
                                default:
                                    instance = GetSingleton(exportedTypes[0].ActualType);
                                    break;
                            }
                            prop.SetValue(obj, instance);
                            break;
                        case ImportManyAttribute ima:
                            if (!prop.PropertyType.IsGenericType)
                            {
                                throw new Exception("ImportMany can only be applied on generic types");
                            }

                            exportedTypes = ExportedTypes.Where(x => x.ExposedType == ima.ImportedType).ToArray();
                            var constructedListType = typeof(List<>).MakeGenericType(ima.ImportedType);
                            var instanceList = (IList)Activator.CreateInstance(constructedListType);
                            foreach (var t in exportedTypes)
                            {
                                var listItem = instance = CreateAndCompose(t.ActualType);
                                instanceList.Add(listItem);
                            }

                            try
                            {
                                prop.SetValue(obj, instanceList);
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
