using IsoUnity.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace IsoUnity.Sequences
{
    public class AttributeEventEditor : EventEditor
    {
        private GameEventConfig config;
        private SerializableGameEvent ge;


        public SerializableGameEvent Result { get; private set; }
        public string EventName { get; private set; }

        public AttributeEventEditor(GameEventConfig config)
        {
            this.config = config;
            EventName = config.Name;
        }

        public EventEditor clone()
        {
            return this.MemberwiseClone() as EventEditor;
        }

        public void detachEvent(SerializableGameEvent ge)
        {
            foreach (var parameterConfig in config.ParameterConfig)
            {
                ge.removeParameter(parameterConfig.Key);
            }
        }

        public void draw()
        {
            foreach (var parameterConfig in config.ParameterConfig)
            {
                if (parameterConfig.Value == typeof(bool)) ge.setParameter(parameterConfig.Key, EditorGUILayout.Toggle(parameterConfig.Key, (bool)ge.getParameter(parameterConfig.Key)));
                if (parameterConfig.Value == typeof(float)) ge.setParameter(parameterConfig.Key, EditorGUILayout.FloatField(parameterConfig.Key, (float)ge.getParameter(parameterConfig.Key)));
                if (parameterConfig.Value == typeof(int)) ge.setParameter(parameterConfig.Key, EditorGUILayout.IntField(parameterConfig.Key, (int)ge.getParameter(parameterConfig.Key)));
                if (parameterConfig.Value == typeof(Vector2)) ge.setParameter(parameterConfig.Key, EditorGUILayout.Vector2Field(parameterConfig.Key, (Vector2)ge.getParameter(parameterConfig.Key)));
                if (parameterConfig.Value == typeof(Vector3)) ge.setParameter(parameterConfig.Key, EditorGUILayout.Vector3Field(parameterConfig.Key, (Vector3)ge.getParameter(parameterConfig.Key)));
                if (parameterConfig.Value == typeof(Vector4)) ge.setParameter(parameterConfig.Key, EditorGUILayout.Vector4Field(parameterConfig.Key, (Vector4)ge.getParameter(parameterConfig.Key)));
                if (parameterConfig.Value == typeof(Rect)) ge.setParameter(parameterConfig.Key, EditorGUILayout.RectField(parameterConfig.Key, (Rect)ge.getParameter(parameterConfig.Key)));
                if (typeof(UnityEngine.Object).IsAssignableFrom(parameterConfig.Value)) ge.setParameter(parameterConfig.Key, EditorGUILayout.ObjectField(parameterConfig.Key, (UnityEngine.Object)ge.getParameter(parameterConfig.Key), parameterConfig.Value, true));
                if (parameterConfig.Value == typeof(Color)) ge.setParameter(parameterConfig.Key, EditorGUILayout.ColorField(parameterConfig.Key, (Color)ge.getParameter(parameterConfig.Key)));
                if (parameterConfig.Value == typeof(Bounds)) ge.setParameter(parameterConfig.Key, EditorGUILayout.BoundsField(parameterConfig.Key, (Bounds)ge.getParameter(parameterConfig.Key)));
                if (parameterConfig.Value == typeof(AnimationCurve)) ge.setParameter(parameterConfig.Key, EditorGUILayout.CurveField(parameterConfig.Key, (AnimationCurve)ge.getParameter(parameterConfig.Key)));
                if (typeof(Enum).IsAssignableFrom(parameterConfig.Value)) ge.setParameter(parameterConfig.Key, EditorGUILayout.EnumPopup(parameterConfig.Key, (Enum)ge.getParameter(parameterConfig.Key)));
                if (parameterConfig.Value == typeof(string)) ge.setParameter(parameterConfig.Key, EditorGUILayout.TextField(parameterConfig.Key, (string)ge.getParameter(parameterConfig.Key)));
                if (parameterConfig.Value == typeof(long)) ge.setParameter(parameterConfig.Key, EditorGUILayout.LongField(parameterConfig.Key, (long)ge.getParameter(parameterConfig.Key)));
                if (parameterConfig.Value == typeof(double)) ge.setParameter(parameterConfig.Key, (double) EditorGUILayout.FloatField(parameterConfig.Key, (float)ge.getParameter(parameterConfig.Key)));
            }
        }

        public void useEvent(SerializableGameEvent ge)
        {
            this.ge = ge;
            this.ge.Name = config.Name;
            foreach (var parameterConfig in config.ParameterConfig)
            {
                if (ge.getParameter(parameterConfig.Key) == null || !parameterConfig.Value.IsAssignableFrom(ge.getParameter(parameterConfig.Key).GetType()))
                {
                    object value = config.ParameterHasDefault[parameterConfig.Key] ? config.DefaultValue[parameterConfig.Key] : GetDefault(parameterConfig.Value);
                    ge.setParameter(parameterConfig.Key, value);
                }
            }
        }

        public object GetDefault(Type t)
        {
            return this.GetType().GetMethod("GetDefaultGeneric").MakeGenericMethod(t).Invoke(this, null);
        }

        public T GetDefaultGeneric<T>()
        {
            return default(T);
        }
    }
}