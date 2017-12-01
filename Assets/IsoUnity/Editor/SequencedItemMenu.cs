using UnityEditor;

namespace IsoUnity.Sequences {
    public class SequencedItemMEnu {

        [MenuItem("Assets/Create/Item with sequence")]
        public static void createIsoTextureAsset()
        {
            var item = IsoAssetsManager.CreateAssetInCurrentPathOf("SequencedItem") as SequencedItem;
            item.Init();
        }
    }
}