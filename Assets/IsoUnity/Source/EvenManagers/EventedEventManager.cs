using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using System.Collections;

namespace IsoUnity.Entities
{
    public abstract class EventedEventManager : EventManager
    {
        private bool inited = false;
        private Dictionary<GameEventConfig, MethodInfo> calls;
        private Dictionary<MethodInfo, GameEventAttribute> attrInfo;

        public override void Tick()
        {
            if (!inited)
            {
                EventedEntityScript.Init(this.GetType(), ref calls, ref attrInfo);
                inited = true;
            }
        }

        public override void ReceiveEvent(IGameEvent ge)
        {
            Current = ge;
            EventedEntityScript.EventHappened(this, calls, attrInfo, ge);
            Current = null;
        }

        private IEnumerator CoroutineController(IGameEvent ge, IEnumerator toRun)
        {
            // We wrap the coroutine
            while (toRun.MoveNext())
                yield return toRun.Current;

            // And when it finishes, we finish the event
            Game.main.eventFinished(ge);
        }

        protected IGameEvent Current { get; private set; }
    }
}