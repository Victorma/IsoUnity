using UnityEngine;
using UnityEditor;
using System.Collections;

public class Dialog : ScriptableObject
{
	public class Fragment{
		[SerializeField]
		public Texture2D face;
		[SerializeField]
		public string name;
		[SerializeField]
		public string msg;
		[SerializeField]
		public bool reset;

		public Fragment(){
			this.name = "";
			this.msg = "";
			this.reset = false;
		}
	}

	public class DialogOption{

		public string tag;
		[SerializeField]
		public string text;
		
		public DialogOption(){
			this.tag = "";
			this.text = "";
		}
	}
		[SerializeField]
		public string id;
		[SerializeField]
		private Fragment[] fragments;
		[SerializeField]
		private DialogOption[] options;

		public Dialog (){
				//this.fragments = null;
				//this.options = null;
		}

		public Fragment[] getFragments(){
				return this.fragments;
		}
		
		public void addFragment(){
			if (fragments != null) {
				Fragment[] tmp = new Fragment [fragments.Length + 1];
				for (int i=0; i<fragments.Length; i++) {
						tmp [i] = fragments [i];
				}
				tmp [fragments.Length] = new Fragment ();
				this.fragments = tmp;
			} else {
				fragments = new Fragment[1];
				fragments[0] = new Fragment();
			}
		}

		public void removeFragment(Fragment frg){
			int k = 0;
			Fragment[] tmp = new Fragment [fragments.Length - 1];
			for (int i=0; i<fragments.Length-1; i++) {
				if(frg == fragments[i]) k = 1;
				tmp[i] = fragments[i+k];
			}
			fragments = tmp;
		}

		public DialogOption[] getOptions(){
			return this.options;
		}

		public void addOption(){
			if (options != null) {
				DialogOption[] tmp = new DialogOption [options.Length + 1];
				for (int i=0; i<options.Length; i++) {
					tmp [i] = options [i];
				}
				tmp [options.Length] = new DialogOption ();
				this.options = tmp;
			} else {
				options = new DialogOption[1];
				options[0] = new DialogOption();
			}
		}

		public void removeOption(DialogOption dlo){
			int k = 0;
			DialogOption[] tmp = new DialogOption [options.Length - 1];
			for (int i=0; i<options.Length-1; i++) {
				if(dlo == options[i]) k = 1;
				tmp[i] = options[i+k];
			}
			options = tmp;
		}

}


