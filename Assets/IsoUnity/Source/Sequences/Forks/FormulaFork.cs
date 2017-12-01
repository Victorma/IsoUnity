using UnityEngine;
using NCalc;
using System.Reflection;

namespace IsoUnity.Sequences {
	[NodeContent("Fork/Single/Formula fork", 2)]
	public class FormulaFork : Checkable {

	    public static FormulaFork Create(string formula)
	    {
	        var r = ScriptableObject.CreateInstance<FormulaFork>();
	        r.Formula = formula;
	        return r;
	    }

	    [SerializeField]
	    private string formula = "";
	    public string Formula
	    {
	        get { return formula; }
	        set
	        {
	            name = value;
	            formula = value;
	            SequenceFormula.Formula = formula;
	        }
	    }

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


	    public override bool check()
	    {
	        var r = SequenceFormula.Evaluate();
	        return r is bool ? (bool)r : false;
	    }

	    public override string ToString()
	    {
	        return this.formula;
	    }
	}
}