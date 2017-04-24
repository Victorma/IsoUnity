using UnityEngine;
using NCalc;
using System;

namespace IsoUnity.Sequences {
	[NodeContent("Switches/Formula setter")]
	public class FormulaSetter : ScriptableObject, ISimpleContent {

	    public string iswitch;
	    [SerializeField]
	    private string formula;

	    public static FormulaSetter Create(string iswitch, string formula)
	    {
	        var r = ScriptableObject.CreateInstance<FormulaSetter>();
	        r.iswitch = iswitch;
	        r.Formula = formula;
	        return r;
	    }
	    
	    public string Formula
	    {
	        get { return formula; }
	        set
	        {
	            this.name = value;
	            this.formula = value;
	            SequenceFormula.Formula = this.formula;
	        }
	    }
	    
	    private string paramError;
	    
	    public SequenceFormula SequenceFormula { get; private set; }

	    void Awake()
	    {
	        SequenceFormula = new SequenceFormula();
	    }

	    void OnEnable()
	    {
	        SequenceFormula = new SequenceFormula();
	        SequenceFormula.Formula = formula;
	    }

	    public override string ToString()
	    {
	        return this.formula;
	    }

	    public int Execute()
	    {
	        var result = SequenceFormula.Evaluate();
	        if(SequenceFormula.IsValidExpression && result != null)
	        {
				if (Sequence.current.ContainsVariable (iswitch)) {
					// Save as local
					Sequence.current.SetVariable (iswitch, result);
				} else if (IsoSwitchesManager.getInstance ().getIsoSwitches ().containsSwitch (iswitch)) {
					// Save as global
					IsoSwitchesManager.getInstance ().getIsoSwitches ().getSwitch (iswitch).State = result;
				} else {
					// If it doesnt exist, store it as local variable
					Sequence.current.SetVariable (iswitch, result);
				}
	        }

	        return 0;
	    }
	}
}