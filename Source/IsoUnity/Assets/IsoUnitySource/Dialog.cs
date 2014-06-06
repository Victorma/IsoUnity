using UnityEngine;
using System.Collections;

[System.Serializable]
public class Dialog : ScriptableObject
{
	[System.Serializable]
	public class Fragment{
		[SerializeField]
		private Texture2D face;
		[SerializeField]
		private string name;
		[SerializeField]
		private string msg;
		[SerializeField]
		private Entity entity;
		[SerializeField]
		private bool isEntityFragment;

		public Texture2D Face {
			get {
				if(this.face == null && this.Entity != null){
					return Entity.face;
				}
				return face; 
			}
			set { 
				if(this.Entity == null) 
					this.face = value;
				else if(this.Entity != null && this.Entity.face != value)
					this.face = value;
				else
					this.face = null;
			}
		}

		public string Name {
			get {
				if(this.name == "" && this.Entity != null){
					return Entity.name;
				}
				return name; 
			}
			set { 
				if(this.Entity == null) 
					this.name = value;
				else if(this.Entity != null && this.Entity.name != value)
					this.name = value;
				else
					this.name = "";
			}
		}

		public Entity Entity {
			get { return (isEntityFragment)?entity:null; }
			set { this.entity = value; }
		}

		public bool IsEntityFragment {
			get { return isEntityFragment; }
			set { isEntityFragment = value; }
		}

		public string Msg{
			get { return msg; }
			set { msg = value; }
		}


		public Fragment(){
			this.isEntityFragment = false;
			this.name = "";
			this.msg = "";
			this.entity = null;
		}
	}
	[System.Serializable]
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


