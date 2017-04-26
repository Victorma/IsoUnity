using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class AttributesUtil {
    
    public static IEnumerable<Type> GetTypesWith<TAttribute>(bool inherit)
                          where TAttribute : System.Attribute
    {
        return from a in AppDomain.CurrentDomain.GetAssemblies()
               from t in a.GetTypes()
               where System.Attribute.GetCustomAttributes(t, typeof(TAttribute), true).Length > 0 && !t.IsAbstract
               select t;
    }

    public static IEnumerable<KeyValuePair<Type, MethodInfo>> GetMethodsWith<TAttribute>(Type type, bool inherit)
                          where TAttribute : System.Attribute
    {
        return from a in AppDomain.CurrentDomain.GetAssemblies()
               from t in a.GetTypes().Where(ta => type.IsAssignableFrom(ta))
               from m in t.GetMethods()
               where System.Attribute.GetCustomAttributes(m, typeof(TAttribute), inherit).Length > 0 && !m.IsAbstract
               select new KeyValuePair<Type, MethodInfo>(t,m);
    }
}
