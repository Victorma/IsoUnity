using UnityEngine;
using UnityEditor;
using System.Collections;

public abstract class CellFactory
{

    private static CellFactory inst;

    public static CellFactory Instance
    {
        get {
            if (inst == null)
                inst = new CellFactoryImp();
            return inst;
        }
    }

    public abstract Object getCellPrefabFor(GameObject cell);

    private class CellFactoryImp : CellFactory
    {
        public CellFactoryImp()
        {
            if (!AssetDatabase.IsValidFolder("Assets/CellPrefabs"))
                AssetDatabase.CreateFolder("Assets", "CellPrefabs");
        }

        public override Object getCellPrefabFor(GameObject cell)
        {
            PrefabType type = PrefabUtility.GetPrefabType(cell);
            if (type == PrefabType.PrefabInstance)
            {
                return PrefabUtility.GetPrefabObject(cell);
            }

            Cell c = cell.GetComponent<Cell>();

            string path = "Assets/CellPrefabs/"+Mathf.CeilToInt(c.Properties.height)+"/";

            Object prefab = null;

            if (AssetDatabase.IsValidFolder(path))
            {
                string[] prefabs = AssetDatabase.FindAssets(c.Properties.ToString(), new string[] { path });
                if (prefabs != null && prefabs.Length>0)
                    prefab = AssetDatabase.LoadAssetAtPath(prefabs[0], typeof(Object));
            }else
                AssetDatabase.CreateFolder("Assets/CellPrefabs", Mathf.CeilToInt(c.Properties.height) + "");

            if (prefab == null)
            {
                prefab = PrefabUtility.CreateEmptyPrefab(path);

                CellProperties properties = c.Properties;

                /*Material myMat = new Material(Shader.Find("Transparent/Cutout/Diffuse"));
                texture.filterMode = FilterMode.Point;
                myMat.SetTexture("_MainTex", texture);
                this.GetComponent<Renderer>().sharedMaterial = myMat;*/
            }


            return prefab;
        }
    }
}
