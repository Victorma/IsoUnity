using UnityEngine;
using System.Collections;

public interface NodeEditor {
	void draw();
	SecuenceNode Result { get; }
	string NodeName{ get; }
	NodeEditor clone();
	bool manages(SecuenceNode c);
	void useNode(SecuenceNode c);
}
