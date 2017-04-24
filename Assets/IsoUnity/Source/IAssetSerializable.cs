using UnityEngine;
using System.Collections;

namespace IsoUnity.Sequences {
	public interface IAssetSerializable {

	    void SerializeInside(Object assetObject);

	}
}