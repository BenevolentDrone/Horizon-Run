using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

namespace HereticalSolutions
{
    /// <summary>
    /// Provides extension methods for the <see cref="Type"/> class.
    /// </summary>
    public static class TypeHelpers
    {
        public static void GetTypesWithAttribute<TAttribute>(
            out Type[] result)
            where TAttribute : System.Attribute
        {
            List<Type> resultList = new List<Type>();

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.GetCustomAttribute<TAttribute>(false) != null)
                    {
                        resultList.Add(type);
                    }
                }
            }

            result = resultList.ToArray();
        }
        
        public static void GetTypesWithAttribute<TAttribute>(
            out Type[] result,
            out IReadOnlyRepository<int, Type> hashToType,
            out IReadOnlyRepository<Type, int> typeToHash)
            where TAttribute : System.Attribute
        {
            hashToType = RepositoriesFactory.BuildDictionaryRepository<int, Type>();

            typeToHash = RepositoriesFactory.BuildDictionaryRepository<Type, int>();

            List<Type> resultList = new List<Type>();

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.GetCustomAttribute<TAttribute>(false) != null)
                    {
                        resultList.Add(type);
                    }
                }
            }

            foreach (Type type in resultList)
            {
                string typeFullString = type.ToString();

                int typeHash = typeFullString.GetHashCode();
                
                ((IRepository<int, Type>)hashToType).AddOrUpdate(typeHash, type);
                
                ((IRepository<Type, int>)typeToHash).AddOrUpdate(type, typeHash);
            }

            result = resultList.ToArray();
        }
    }
}