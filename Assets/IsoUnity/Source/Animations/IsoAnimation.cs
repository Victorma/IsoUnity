using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IsoUnity 
{
	[System.Serializable]
	[CreateAssetMenu(fileName = "new IsoAnimation", menuName = "IsoUnity/IsoAnimation")]
	public class IsoAnimation : ScriptableObject {

		[System.Serializable]
		public struct Frame {
			public int column;
			public float duration;
		}

		[SerializeField]
		public bool loop = false;
		[SerializeField]
		public List<Frame> frames = new List<Frame>();
		[SerializeField]
		public string sheet;
		[SerializeField]
		public IsoDecoration overrideSheet;
	}
}

