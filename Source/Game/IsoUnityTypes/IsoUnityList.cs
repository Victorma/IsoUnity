using UnityEngine;
using System.Collections.Generic;

public class IsoUnityList : ScriptableObject, IList<object> {

    [SerializeField]
    private List<Object> listaIsometrica;

    void OnEnable()
    {
        Debug.Log("Lista creada");
        this.listaIsometrica = new List<Object>();
    }

    void OnDestroy()
    {
        Debug.Log("Lista Destruida");
        this.Clear();
    }

    public List<Object> getSerializable()
    {
        return listaIsometrica;
    }

    private static Object getObjectIn(object o, List<Object> listaIsometrica)
    {
        foreach(var i in listaIsometrica)
            if (i is IsoUnityBasicType)
            {
                var b = i as IsoUnityBasicType;
                if (o == b.Value)
                    return i;
            }
            else
            {
                if (o == i)
                    return i;
            }

        return null;
    }

    private static Object getObjectFor(object o)
    {
        if (o == null)
            return null;

        Object uo = null;

        if (o is Object)
        {
            uo = o as Object;
        }
        else
        {
            uo = IsoUnityTypeFactory.Instance.getIsoUnityType(o);
        }
        return uo;
    }

    private static object getSystemObjectFor(Object o)
    {
        object so = null;
        if (o is IsoUnityType)
        {
            so = (o as IsoUnityType).Value;
        }
        else
        {
            so = o;
        }

        return so;
    }

    public int IndexOf(object item)
    {
        return listaIsometrica.IndexOf(getObjectIn(item, listaIsometrica));
    }

    public void Insert(int index, object item)
    {
        if (listaIsometrica[index] is IsoUnityType)
        {
            IsoUnityTypeFactory.Instance.Destroy(listaIsometrica[index] as IsoUnityType);
        }

        listaIsometrica.Insert(index, getObjectFor(item));
    }

    public void RemoveAt(int index)
    {
        if (listaIsometrica[index] is IsoUnityType)
        {
            IsoUnityTypeFactory.Instance.Destroy(listaIsometrica[index] as IsoUnityType);
        }

        listaIsometrica.RemoveAt(index);
    }

    public object this[int index]
    {
        get
        {
            return getSystemObjectFor(listaIsometrica[index]);
        }
        set
        {
            this.Insert(index, value);
        }
    }

    public System.Collections.IEnumerator GetEnumerator()
    {
        return myEnumerator(); 
    }

    public void Add(object item)
    {
        this.listaIsometrica.Add(getObjectFor(item));
    }

    public void Clear()
    {
        for (int i = listaIsometrica.Count - 1; i >= 0; i--)
        {
            this.RemoveAt(i);
        }
    }

    public bool Contains(object item)
    {
        return this.listaIsometrica.Contains(getObjectIn(item, listaIsometrica));
    }

    public void CopyTo(object[] array, int arrayIndex)
    {
        if (arrayIndex != this.Count)
            return;

        int i = 0;
        foreach (var o in this)
        {
            array[i] = o; i++;
        }
    }

    public int Count
    {
        get { return listaIsometrica.Count; }
    }

    public bool IsReadOnly
    {
        get { return false; }
    }

    public bool Remove(object item)
    {
        return this.listaIsometrica.Remove(getObjectIn(item, listaIsometrica));
    }

    IEnumerator<object> IEnumerable<object>.GetEnumerator()
    {
        return myEnumerator();
    }

    private IEnumerator<object> myEnumerator()
    {
        return new MyEnumerator(listaIsometrica);
    }

    private class MyEnumerator : IEnumerator<object>
    {
        private IEnumerator<Object> OEnumerator;
        public MyEnumerator(List<Object> lista)
        {
            OEnumerator = lista.GetEnumerator();
        }

        public object Current
        {
            get { return IsoUnityList.getSystemObjectFor(OEnumerator.Current); }
        }


        public bool MoveNext()
        {
            return OEnumerator.MoveNext();
        }

        public void Reset()
        {
            OEnumerator.Reset();
        }

        public void Dispose()
        {
            OEnumerator.Dispose();
        }
    }
}
