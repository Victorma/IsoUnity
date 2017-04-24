using UnityEngine;
using System.Collections;

namespace IsoUnity
{
    public abstract class EventManager : MonoBehaviour
    {

        void OnEnable()
        {
            if (Game.main != null)
                Game.main.RegisterEventManager(this);
        }

        void OnDisable()
        {
            if(Game.main != null)
                Game.main.DeRegisterEventManager(this);
        }

        public abstract void ReceiveEvent(IGameEvent ev);
        public abstract void Tick();

    }
}