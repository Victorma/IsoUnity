using UnityEngine;
using System.Collections.Generic;

namespace IsoUnity.Sequences {
	[System.Serializable]
	public class SequenceNode : ScriptableObject {
		[SerializeField]
		private SequenceNode[] childs;
	    [SerializeField]
	    protected bool isUnityObject;
		[SerializeField]
	    protected Object objectContent = null;
	    [SerializeField]
	    private object content = null;
	    [SerializeField]
	    protected Sequence secuence = null;
	    [SerializeField]
	    private Rect position = new Rect(0, 0, 300, 0);
	    [SerializeField]
	    private bool collapsed = false;

	    private NodeContentAttribute contentAttribute;

	    public string ShortDescription
	    {
	        get
	        {
	            return isUnityObject ? objectContent.name : content.ToString();
	        }
	    }

	    public Rect Position
	    {
	        get {
	            if (collapsed) return new Rect(position.x, position.y, GUI.skin.button.CalcSize(new GUIContent(ShortDescription)).x + 80, 30 * ChildSlots);
	            else           return position; 
	        }
	        set {
	            if (collapsed) position = new Rect(value.x, value.y, position.width, position.height);
	            else           position = value; 
	        }
	    }

	    public bool Collapsed
	    {
	        get { return collapsed; }
	        set { collapsed = value; }
	    }

		public void init(Sequence s){
			childs = new SequenceNode[0];
	        this.secuence = s;
			DontDestroyOnLoad (this);
		}
		
		public SequenceNode[] Childs {
			get
	        {
	            UpdateChildSlots();
	            return childs;
	        }
		}

	    public SequenceNode this[int child]
	    {
	        get { return Childs[child]; }
	        set { Childs[child] = value; }
	    }

	    public string Name{
			get{ return name;} 
			set{ name = value;}
		}
		
		public virtual object Content{
			get
	        {
	            return isUnityObject ? objectContent : content;
	        }
			set
	        {
	            if(value == null)
	            {
	                content = objectContent = null;
	                return;
	            }

	            var attrs = System.Attribute.GetCustomAttributes(value.GetType(), typeof(NodeContentAttribute), true);
	            contentAttribute = attrs.Length > 0 ? attrs[0] as NodeContentAttribute : null;

	            isUnityObject = value is Object;
	            if (isUnityObject)
	            {
	                content = null;
	                objectContent = value as Object;
	            }
	            else
	            {
	                objectContent = null;
	                content = value;
	            }
	        }
		}
		
		public void clearChilds(){
	        var aux = ChildSlots;
	        ChildSlots = 0;
	        ChildSlots = aux;
		}

	    public int ChildSlots
	    {
	        set
	        {
	            if (this.childs.Length != value)
	            {
	                var newChilds = new SequenceNode[value];
	                move<SequenceNode>(this.childs, newChilds, null);
	                this.childs = newChilds;
	            }
	        }
	        get
	        {
	            return Childs.Length;
	        }
	    }
		
		/*public SequenceNode addNewChild(Object content = null, int childSlots = 0){
	        this.ChildSlots++;
	        var r = this.childs[this.ChildSlots - 1] = secuence.createChild(content, childSlots);
	        return r;
		}
		
		public void removeChild(int index){
	        for (int i = index; i < this.childs.Length - 1; i++)
	        {
	            this.childs[i] = this.childs[i + 1];
	        }
	        this.ChildSlots--;
		}
		
		public void removeChild(SequenceNode child){
	        for (int i = 0; i < childs.Length; i++)
	            if (child == childs[i])
	            {
	                this.removeChild(i);
	                break;
	            }
		}*/

	    private void UpdateChildSlots()
	    {        
	        if(Content is NodeContent)          ChildSlots = (Content as NodeContent).ChildSlots;
	        else if(contentAttribute != null)   ChildSlots = contentAttribute.Slots;
	    }

	    /* AUX */
	    
	    private int move<T>(T[] from, T[] to, T empty)
	    {
	        int l = Mathf.Min(from.Length, to.Length);
	        for (int i = 0; i < l; i++) to[i] = from[i];
	        for (int i = l; i < to.Length; i++) to[i] = empty;
	        return l;
	    }

		public virtual SequenceNode Clone(){
			var clone = ScriptableObject.CreateInstance<SequenceNode>();

			clone.isUnityObject = isUnityObject;
			clone.objectContent = objectContent;
			clone.content = content;
			clone.position = position;
			clone.collapsed = collapsed;

			clone.childs = new SequenceNode[this.ChildSlots];
			for (int i = 0; i < ChildSlots; i++)
				clone.childs [i] = childs [i];

			if (Content != null && Content is System.ICloneable) 
				clone.Content = (Content as System.ICloneable).Clone ();

			return clone;
		}
	}
}