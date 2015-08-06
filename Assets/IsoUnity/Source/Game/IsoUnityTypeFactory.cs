using UnityEngine;
using System.Collections.Generic;

public abstract class IsoUnityTypeFactory 
{
    private static IsoUnityTypeFactory instance;
    public static IsoUnityTypeFactory Instance
    {
        get
        {
            if (instance == null)
                instance = new IsoUnityTypeFactoryImp();
            return instance;
        }
    }
    public abstract void Destroy(IsoUnityType i);
    public abstract IsoUnityType getIsoUnityType(object c);

    private class IsoUnityTypeFactoryImp : IsoUnityTypeFactory
    {
        private List<IsoUnityType> types;

        public IsoUnityTypeFactoryImp()
        {
            types = new List<IsoUnityType>();
            types.Add(ScriptableObject.CreateInstance<IsoUnityBasicType>());
            types.Add(ScriptableObject.CreateInstance<IsoUnityCollectionType>());
            types.Add(ScriptableObject.CreateInstance<IsoUnityEnumType>());
        }

        public override void Destroy(IsoUnityType i)
        {
            IsoUnityType.DestroyImmediate(i);
        }

        public override IsoUnityType getIsoUnityType(object c)
        {
            IsoUnityType r = null;
            foreach (IsoUnityType t in types)
            {
                if (t.canHandle(c))
                {
                    r = t.clone();
                    r.Value = c;
                    break;
                }
            }
            return r;
        }
    }

}
