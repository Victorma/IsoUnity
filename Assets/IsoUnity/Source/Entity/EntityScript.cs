using UnityEngine;
using System.Collections;
using IsoUnity;

namespace IsoUnity.Entities
{
    public abstract class EntityScript : MonoBehaviour
    {
        public Entity Entity
        {
            get { return this.GetComponent<Entity>(); }
        }
        // Use this for initialization
        /*public virtual void Start()
        {

        }*/

        //Abstract Methods
        public abstract void eventHappened(IGameEvent ge);
        public abstract void tick();
        public abstract void Update();
        public abstract Option[] getOptions();
    }
}