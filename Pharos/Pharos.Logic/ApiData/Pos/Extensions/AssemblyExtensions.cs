﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Pharos.Logic.ApiData.Pos.Extensions
{
    public static class AssemblyExtensions
    {
        public static IEnumerable<TBaseInterface> GetImplementedObjectsByInterface<TBaseInterface>(this Assembly assembly, Func<Type, bool> filter = null)
         where TBaseInterface : class
        {
            return GetImplementedObjectsByInterface<TBaseInterface>(assembly, typeof(TBaseInterface), filter);
        }

        /// <summary>
        /// Gets the implemented objects by interface.
        /// </summary>
        /// <typeparam name="TBaseInterface">The type of the base interface.</typeparam>
        /// <param name="assembly">The assembly.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <returns></returns>
        public static IEnumerable<TBaseInterface> GetImplementedObjectsByInterface<TBaseInterface>(this Assembly assembly, Type targetType, Func<Type, bool> filter = null)
            where TBaseInterface : class
        {
            Type[] arrType = assembly.GetExportedTypes();

            var result = new List<TBaseInterface>();

            for (int i = 0; i < arrType.Length; i++)
            {
                var currentImplementType = arrType[i];

                if (currentImplementType.IsAbstract)
                    continue;

                if (!targetType.IsAssignableFrom(currentImplementType))
                    continue;
                if (filter != null && filter(currentImplementType))
                    continue;
                result.Add((TBaseInterface)Activator.CreateInstance(currentImplementType));
            }

            return result;
        }

        public static TBaseInterface GetImplementedObjectByInterface<TBaseInterface>(this IEnumerable<Assembly> assemblies, Func<Type, bool> filter = null)
         where TBaseInterface : class
        {
            List<TBaseInterface> tBaseInterfaces = new List<TBaseInterface>();
            foreach (var assembly in assemblies)
            {
                try
                {

                    tBaseInterfaces.AddRange(assembly.GetImplementedObjectsByInterface<TBaseInterface>(filter));
                }
                catch (Exception exc)
                {
                    throw new Exception(string.Format("加载程序集失败，程序集： {0}!", assembly.FullName), exc);
                }
            }
            return tBaseInterfaces.FirstOrDefault();
        }
        public static IEnumerable<TBaseInterface> GetImplementedObjectsByInterface<TBaseInterface>(this IEnumerable<Assembly> assemblies, Func<Type, bool> filter = null)
         where TBaseInterface : class
        {
            List<TBaseInterface> tBaseInterfaces = new List<TBaseInterface>();
            foreach (var assembly in assemblies)
            {
                try
                {
                    tBaseInterfaces.AddRange(assembly.GetImplementedObjectsByInterface<TBaseInterface>(filter));
                }
                catch (Exception exc)
                {
                    throw new Exception(string.Format("加载程序集失败，程序集： {0}!", assembly.FullName), exc);
                }
            }
            return tBaseInterfaces;
        }
    }
}