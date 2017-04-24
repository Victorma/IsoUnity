using UnityEngine;
using UnityEditor;
using System.Collections;

using IsoUnity.Entities;

namespace IsoUnity.Sequences
{

    public class MoveEventEditor : EventEditor
    {

        private SerializableGameEvent ge;
        public MoveEventEditor()
        {
            this.ge = ScriptableObject.CreateInstance<SerializableGameEvent>();
            ge.Name = this.EventName;
            ge.setParameter("entity", null);
            ge.setParameter("cell", null);
        }

        public SerializableGameEvent Result
        {
            get { return ge; }
        }
        public string EventName
        {
            get { return "Move"; }
        }
        public EventEditor clone()
        {
            return new MoveEventEditor();
        }

        public void useEvent(SerializableGameEvent ge)
        {
            this.ge = ge;
            this.ge.Name = this.EventName;
        }

        public void draw()
        {

            ge.setParameter("entity", EditorGUILayout.ObjectField("Entity", (Object)ge.getParameter("entity"), typeof(Entity), true));
            ge.setParameter("cell", EditorGUILayout.ObjectField("Cell", (Object)ge.getParameter("cell"), typeof(Cell), true));
        }

        public void detachEvent(SerializableGameEvent ge)
        {
            if (ge.getParameter("entity") == null) ge.removeParameter("entity");
            if (ge.getParameter("cell") == null) ge.removeParameter("cell");
        }

    }
}
