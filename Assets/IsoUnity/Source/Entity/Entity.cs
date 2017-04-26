using UnityEngine;
using System.Collections.Generic;
using IsoUnity;

namespace IsoUnity.Entities
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Mover))]
    public class Entity : MonoBehaviour
    {

        /// <summary>
        /// Face used for dialogs
        /// </summary>
        public Texture2D face;

        [SerializeField]
        private Cell position;
        public Cell Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                this.transform.parent = position.transform;
                //my_transform.position = position.transform.position + new Vector3(0, position.WalkingHeight + my_transform.localScale.y/2f, 0);
            }
        }

        public void tick()
        {
            foreach (EntityScript es in this.GetComponents<EntityScript>())
                es.tick();
        }

        public void eventHappened(IGameEvent ge)
        {
            EntityScript[] scripts = this.GetComponents<EntityScript>();

            //TODO Preference system

            foreach (EntityScript es in scripts)
                es.eventHappened(ge);
        }

        public Option[] getOptions()
        {
            EntityScript[] scripts = this.GetComponents<EntityScript>();
            List<Option> options = new List<Option>();

            foreach (EntityScript es in scripts)
                options.AddRange(es.getOptions() ?? new Option[0]);

            return options.ToArray() as Option[];
        }

        // Use this for initialization
        void Start()
        {

        }


        Transform my_transform;
        public Decoration decoration
        {
            get
            {
                return this.GetComponent<Decoration>();
            }
        }

        public Mover mover
        {
            get
            {
                return this.GetComponent<Mover>();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (my_transform == null)
                my_transform = this.transform;

            if (!Application.isPlaying && Application.isEditor)
            {

                Transform parent = my_transform.parent;
                Transform actual = null;
                if (position != null)
                    actual = position.transform;

                if (parent != actual)
                {
                    Cell probablyParent = parent.GetComponent<Cell>();
                    if (probablyParent != null)
                        position = probablyParent;
                    else if (actual != null)
                        my_transform.parent = actual;

                }

                if (this.position != null)
                {
                    //my_transform.position = position.transform.position + new Vector3(0, position.WalkingHeight + my_transform.localScale.y/2f, 0);
                }
            }


        }

        public void OnDestroy()
        {
            if (this.Position != null)
            {
                this.Position.Map.unRegisterEntity(this);
            }
        }
    }
}