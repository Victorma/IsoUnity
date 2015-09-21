using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SecuenceNode : ScriptableObject {
	[SerializeField]
	private SecuenceNode[] childs;
	[SerializeField]
	private Object content = null;
    [SerializeField]
    private Secuence secuence = null;

	public void init(Secuence s){
		childs = new SecuenceNode[0];
        this.secuence = s;
		DontDestroyOnLoad (this);
	}
	
	public SecuenceNode[] Childs {
		get{ return childs; }
	}
	
	public string Name{
		get{ return name;} 
		set{ name = value;}
	}
	
	public Object Content{
		get{ return content;}
		set{ content = value;}
	}
	
	public void clearChilds(){
        var aux = ChildSlots;
        ChildSlots = 0;
        ChildSlots = aux;
	}

    private int move<T>(T[] from, T[] to, T empty)
    {
        int l = Mathf.Min(from.Length, to.Length);
        for (int i = 0; i < l; i++)          to[i] = from[i];
        for (int i = l; i < to.Length; i++)  to[i] = empty;
        return l;
    }

    public int ChildSlots
    {
        set
        {
            if (this.childs.Length != value)
            {
                var newChilds = new SecuenceNode[value];
                var max = move<SecuenceNode>(this.childs, newChilds, null);
                //for (int i = max; i < newChilds.Length; i++)
                //    newChilds[i] = secuence.createChild();

                this.childs = newChilds;
            }
        }
        get
        {
            return this.childs.Length;
        }
    }
	
	public SecuenceNode addNewChild(){
        this.ChildSlots++;
		return this.childs[this.ChildSlots-1];
	}
	
	public void removeChild(int i){
        this.childs[i] = null;
	}
	
	public void removeChild(SecuenceNode child){
        for (int i = 0; i < childs.Length; i++)
            if (child == childs[i])
            {
                this.removeChild(i);
                break;
            }
	}

}