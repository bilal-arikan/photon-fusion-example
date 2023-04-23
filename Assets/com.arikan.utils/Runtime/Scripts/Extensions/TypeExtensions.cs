using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class TypeExtensions
{
    /// <summary>
    /// Return all interfaces implemented by the incoming type as well as the type itself if it is an interface.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static IEnumerable<Type> GetInterfacesAndSelf(this Type type)
    {
        if (type == null)
            throw new ArgumentNullException();
        if (type.IsInterface)
            return new[] { type }.Concat(type.GetInterfaces());
        else
            return type.GetInterfaces();
    }

    public static IEnumerable<KeyValuePair<Type, Type>> GetDictionaryKeyValueTypes(this Type type)
    {
        foreach (Type intType in type.GetInterfacesAndSelf())
        {
            if (intType.IsGenericType
                && intType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
            {
                var args = intType.GetGenericArguments();
                if (args.Length == 2)
                    yield return new KeyValuePair<Type, Type>(args[0], args[1]);
            }
        }
    }

    public static IEnumerable<Type> FindDerivedTypes(this Type type, bool inheritedInterfaces = true)
    {
        IEnumerable<Type> first = Enumerable.Empty<Type>();
        if (type.BaseType != null)
        {
            first = first.Concat(type.BaseType.Yield());
        }
        return first.Concat(GetInterfaces(type, inheritedInterfaces));
    }

    public static IEnumerable<Type> GetInterfaces(this Type type, bool includeInherited)
    {
        if (includeInherited || type.BaseType == null)
        {
            return type.GetInterfaces();
        }
        return type.GetInterfaces().Except(type.BaseType.GetInterfaces());
    }
}
