using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IsoUnity.Sequences {
	public abstract class DialogEventManager : EventManager {
	    
	    
	    private IGameEvent gameEvent;
	    

	    public override void ReceiveEvent(IGameEvent ev)
	    {
	        if(ev.Name == "show dialog fragment")
	        {
	            gameEvent = ev;
	            DoFragment(ev.getParameter("fragment") as Fragment);
	            doing = DOING_FRAGMENT;
	        }

	        if(ev.Name == "show dialog options")
	        {
	            gameEvent = ev;
	            DoOptions(ev.getParameter("message") as string, ev.getParameter("options") as List<Option>);
	            doing = DOING_OPTIONS;
	        }
	    }

	    private const int DOING_FRAGMENT = 1;
	    private const int DOING_OPTIONS = 2;
	    private int doing = -1;
	    protected abstract void DoFragment(Fragment fragment);
	    protected abstract void DoOptions(string question, List<Option> options);
	    protected abstract bool IsFragmentFinised();
	    protected abstract int IsOptionSelected();

	    public override void Tick()
	    {
	        if (doing == DOING_FRAGMENT && IsFragmentFinised())
	        {
	            Game.main.eventFinished(gameEvent);
	            doing = -1;
	        }

	        if(doing == DOING_OPTIONS)
	        {
	            var s = IsOptionSelected();
	            if(s != -1)
	            {
	                var extraParams = new Dictionary<string, object>();
	                extraParams.Add("option", s);
	                Game.main.eventFinished(gameEvent, extraParams);
	                doing = -1;
	            }
	        }
	    }

	}
}