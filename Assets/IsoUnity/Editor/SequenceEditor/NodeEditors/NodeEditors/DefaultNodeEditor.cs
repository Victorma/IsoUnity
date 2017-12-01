using System.Linq;
using UnityEngine;
using UnityEditor;

namespace IsoUnity.Sequences {
	public class DefaultNodeEditor : NodeEditor {

	    // Private variables
	    private Editor editor;
	    private SequenceNode node;
	    private NodeContentAttribute attr;
	    private string[] defaultContent = new string[] { "default" };

	    // Public attributes
	    public string[] ChildNames { get { return node.Content is NodeContent ? (node.Content as NodeContent).ChildNames : defaultContent; } }
	    public string NodeName { get { return "Default"; } }
	    public SequenceNode Result { get { return node; } }

	    // NodeEditor behaviour
	    public NodeEditor clone()
	    {
	        return new DefaultNodeEditor();
	    }

	    public bool manages(SequenceNode c)
	    {
	        return c.Content != null && c.Content is Object;
	    }

	    public void useNode(SequenceNode c)
	    {
	        node = c;
	        attr = c.Content.GetType().GetCustomAttributes(true).ToList().Find(a => a is NodeContentAttribute) as NodeContentAttribute;
	        node.Name = attr != null ? attr.Name : node.Content.GetType().ToString();
	        editor = Editor.CreateEditor(c.Content as Object);
	    }
	    
	    // Drawing
	    public void draw()
	    {
	        if(editor != null)
	        {
	            editor.OnInspectorGUI();
	            node.ChildSlots = 
	                node.Content is NodeContent 
	                ? (node.Content as NodeContent).ChildSlots 
	                : (attr != null) 
	                    ? attr.Slots 
	                    : 1;
	        }
	    }
	}
}