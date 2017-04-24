using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
}
