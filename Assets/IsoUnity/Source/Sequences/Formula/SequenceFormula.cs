using UnityEngine;
using NCalc;
using System.Reflection;

namespace IsoUnity.Sequences {
	public class SequenceFormula {

	    private Expression expression;
	    private string formula;
	    private string paramError;
	    private string functionError;
	    private object expresionResult;

	    public SequenceFormula() : this(string.Empty) { }
	    public SequenceFormula(string formula)
	    {
	        this.Formula = formula;
	    }

	    public string Formula
	    {
	        get
	        {
	            return formula;
	        }
	        set
	        {
	            formula = value;
	            RegenerateExpression();
	        }
	    }

	    private System.Type desiredReturnType;
	    public System.Type DesiredReturnType
	    {
	        get
	        {
	            return desiredReturnType;
	        }
	        set
	        {
	            desiredReturnType = value;
	            RegenerateExpression();
	        }
	    }

	    private void RegenerateExpression()
	    {
	        paramError = string.Empty;
	        if (!string.IsNullOrEmpty(formula))
	        {
	            try
	            {
	                expression = new Expression(this.formula);
	                expression.EvaluateParameter += CheckParameter;
	                expression.EvaluateFunction += EvaluateFunction;
	                expresionResult = expression.Evaluate();
	            }
	            catch { }
	        }
	    }

	    public bool IsValidExpression
	    {
	        get
	        {
	            return !string.IsNullOrEmpty(formula) && string.IsNullOrEmpty(paramError) && string.IsNullOrEmpty(functionError) && !expression.HasErrors() && (desiredReturnType == null || expresionResult != null && expresionResult.GetType().Equals(desiredReturnType));
	        }
	    }


	    public string Error
	    {
	        get
	        {
	            return
	                string.IsNullOrEmpty(formula)
	                    ? "The formula can't be empty"
	                    : !string.IsNullOrEmpty(paramError)
	                        ? paramError
	                        : !string.IsNullOrEmpty(functionError)
	                            ? functionError
	                            : desiredReturnType != null && !(expresionResult.GetType().Equals(desiredReturnType))
	                                ? "The formula doesn't result in a " + desiredReturnType.ToString() + " value."
	                                : expression.Error;
	        }
	    }

	    private void CheckParameter(string param, ParameterArgs args)
	    {
			if (Sequence.current.ContainsVariable (param)) 
			{
				args.HasResult = true;
				args.Result = Sequence.current.GetVariable (param);
			} 
			else if (IsoSwitchesManager.getInstance().getIsoSwitches().containsSwitch(param))
			{
				args.HasResult = true;
				args.Result = IsoSwitchesManager.getInstance().getIsoSwitches().consultSwitch(param);
	        }
	        else
			{
				args.HasResult = false;
				paramError = "Missing parameter \"" + param + "\"";
	        }
	    }

	    private void EvaluateFunction(string name, FunctionArgs args)
	    {
	        functionError = "";

	        switch (name)
	        {
	            case "var":
	                {
	                    if (args.Parameters.Length < 3)
	                    {
	                        functionError = "Function 'var' requires 3 arguments.";
	                        return;
	                    }

	                    var gameObject = (string)args.Parameters[0].Evaluate();
	                    if (!(gameObject is string))
	                    {
	                        functionError = "Function 'var' 1st parameter is not string.";
	                        return;
	                    }
	                    var component = (string)args.Parameters[1].Evaluate();
	                    if (!(component is string))
	                    {
	                        functionError = "Function 'var' 2nd parameter is not string.";
	                        return;
	                    }
	                    var property = (string)args.Parameters[2].Evaluate();
	                    if (!(property is string))
	                    {
	                        functionError = "Function 'var' 3rd parameter is not string.";
	                        return;
	                    }

	                    if (Application.isPlaying) // Only real check runtime
	                    {
	                        GameObject go = GameObject.Find(gameObject);
	                        Component co = null;
	                        PropertyInfo p = null;

	                        if (go) co = go.GetComponent(component);
	                        if (co) p = co.GetType().GetProperty(property);

	                        // Result
	                        args.HasResult = go != null && co != null && p != null;
	                        if (args.HasResult) args.Result = p.GetValue(co, null);
	                        else
	                        {
	                            if (go == null)
	                                functionError = "Formula '" + name + "' function error: GameObject \"" + gameObject + "\" not found in scene.";
	                            else if (co == null)
	                                functionError = "Formula '" + name + "' function error: Component \"" + component + "\" not found in \"" + gameObject + "\" gameObject.";
	                            else if (p == null)
	                                functionError = "Formula '" + name + "' function error: Property \"" + property + "\" not found in \"" + component + "\" component.";
	                        }
	                    }
	                }

	                break;

	            case "varObject":
	            case "objectVar":
	                {
	                    if (args.Parameters.Length < 2)
	                    {
	                        functionError = "Function '" + name + "' requires 2 arguments.";
	                        return;
	                    } 

	                    var objectName = (string)args.Parameters[0].Evaluate();
	                    if (!(objectName is string))
	                    {
	                        functionError = "Function '" + name + "' 1st parameter is not string.";
	                        return;
	                    }
	                    var property = (string)args.Parameters[1].Evaluate();
	                    if (!(property is string))
	                    {
	                        functionError = "Function '" + name + "' 2nd parameter is not string.";
	                        return;
	                    }

	                    if (Application.isPlaying) // Only real check runtime
	                    {
	                        object o = Sequence.current ? Sequence.current.GetObject(objectName) : null;
	                        PropertyInfo p = null;

	                        if (o != null) p = o.GetType().GetProperty(property);

	                        args.HasResult = o != null && p != null;
	                        if (args.HasResult) args.Result = p.GetValue(o, null);
	                        else
	                        {
	                            if (o == null)
	                                functionError = "Formula '" + name + "' function error: Object \"" + objectName + "\" not found in the current sequence.";
	                            else if (p == null)
	                                functionError = "Formula '" + name + "' function error: Property \"" + property + "\" not found in \"" + objectName + "\" scene object.";

	                        }
	                    }
	                }
	                break;
	            /*default:
	                functionError = "Missing function \"" + name + "\"";
	                break;*/
	        }
	    }

	    public object Evaluate()
	    {
	        return expression.Evaluate();
	    }
	}

	public class FormulaException : System.Exception
	{

	    public FormulaException(string message) : base(message)
	    {
	    }

	}
}