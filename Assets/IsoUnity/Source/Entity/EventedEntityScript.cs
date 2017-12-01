using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using System.Collections;

namespace IsoUnity.Entities
{
    public abstract class EventedEntityScript : EntityScript
    {
        private bool inited = false;
        private Dictionary<GameEventConfig, MethodInfo> calls;
        private Dictionary<MethodInfo, GameEventAttribute> attrInfo;

        public override void eventHappened(IGameEvent ge)
        {
            Current = ge;

            EventHappened(this, calls, attrInfo, ge);

            Current = null;
        }

        private static IEnumerator CoroutineController(IGameEvent ge, IEnumerator toRun)
        {
            // We wrap the coroutine
            while (toRun.MoveNext())
                yield return toRun.Current;

            // And when it finishes, we finish the event
            Game.main.eventFinished(ge);
        }

        public override Option[] getOptions()
        {
            return null;
        }

        protected IGameEvent Current { get; private set; }

        public override void tick()
        {
            if (!inited)
            {
                Init(this.GetType(), ref calls, ref attrInfo);
                inited = true;
            }
        }

        internal static void Init(Type type, ref Dictionary<GameEventConfig, MethodInfo> calls, ref Dictionary<MethodInfo, GameEventAttribute> attrInfo)
        {
            calls = new Dictionary<GameEventConfig, MethodInfo>();
            attrInfo = new Dictionary<MethodInfo, GameEventAttribute>();

            foreach (var method in type.GetMethods().ToList()
                    .FindAll(m => m.GetCustomAttributes(typeof(GameEventAttribute), true).Length > 0)
                    .ToArray())
            {
                var attr = ((GameEventAttribute)method.GetCustomAttributes(typeof(GameEventAttribute), true)[0]);
                attrInfo.Add(method, attr);
                calls.Add(new GameEventConfig(type, method), method);
            }
        }

        internal static void EventHappened(MonoBehaviour reference, Dictionary<GameEventConfig, MethodInfo> calls, Dictionary<MethodInfo, GameEventAttribute> attrInfo, IGameEvent ge)
        {
            if (calls != null && calls.Count > 0)
            {
                var config = new GameEventConfig(ge);
                if (calls.ContainsKey(config))
                {
                    var call = calls[config];
                    
                    if (!attrInfo[call].RequiresReference || ge.belongsTo(reference, reference.GetType().Name))
                    {
                        List<object> parameters = new List<object>();
                        foreach (var p in call.GetParameters())
                            if (ge.Params.Contains(p.Name.ToLower()))
                                parameters.Add(ge.getParameter(p.Name));
                            else parameters.Add(p.DefaultValue);

                        var output = call.Invoke(reference, parameters.ToArray());

                        if (output is IEnumerator)
                            // If we want to autofinish it we use the controller, else just launch it
                            reference.StartCoroutine(attrInfo[call].AutoFinish ? CoroutineController(ge, output as IEnumerator) : output as IEnumerator);
                        else if (attrInfo[call].AutoFinish)
                            // If is not a coroutine and we have to auto finish it, we just do it
                            Game.main.eventFinished(ge);

                    }
                }
            }
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class GameEventAttribute : System.Attribute
    {
        public bool AutoFinish { get; private set; }
        public bool RequiresReference { get; private set; }
        public GameEventAttribute(bool autoFinish = true, bool requiresReference = true)
        {
            AutoFinish = autoFinish;
            RequiresReference = requiresReference;
        }
    }

    public class GameEventConfig 
    {
        public string Name { get; private set; }
        public MethodInfo Methid { get; private set; }
        public Dictionary<string, System.Type> ParameterConfig { get; private set; }

        public Dictionary<string, bool> ParameterHasDefault { get; private set; }
        public Dictionary<string, object> DefaultValue { get; private set; }

        public bool RequiresReference { get; private set; }

        public GameEventConfig(Type t, MethodInfo m)
        { 
            Name = SplitCamelCase(m.Name).ToLower();
            ParameterConfig = new Dictionary<string, Type>();
            ParameterHasDefault = new Dictionary<string, bool>();
            DefaultValue = new Dictionary<string, object>();

            var attr = ((GameEventAttribute)m.GetCustomAttributes(typeof(GameEventAttribute), true)[0]);

            if (attr.RequiresReference)
            {
                ParameterConfig.Add(t.Name.ToLower(), t);
                ParameterHasDefault.Add(t.Name.ToLower(), false);
            }

            RequiresReference = attr.RequiresReference;

            foreach (var parameter in m.GetParameters())
            {
                ParameterConfig.Add(parameter.Name.ToLower(), parameter.ParameterType);
                ParameterHasDefault.Add(parameter.Name.ToLower(), parameter.IsOptional);
                if (parameter.IsOptional)
                    DefaultValue.Add(parameter.Name.ToLower(), parameter.DefaultValue);
            }
        }

        public GameEventConfig(string name, Dictionary<string, System.Type> parameterConfig, Dictionary<string, bool> parameterHasDefault)
        {
            Name = name;
            ParameterConfig = parameterConfig;
            ParameterHasDefault = parameterHasDefault;
        }

        public GameEventConfig(IGameEvent gameEvent)
        {
            Name = gameEvent.Name;
            ParameterConfig = new Dictionary<string, Type>();
            foreach (var p in gameEvent.Params)
                if(p != "synchronous")
                    ParameterConfig.Add(p, gameEvent.getParameter(p) != null ? gameEvent.getParameter(p).GetType() : typeof(object));
        }

        public override bool Equals(object other)
        {
            if(!(other is GameEventConfig))
                return false;

            var o = other as GameEventConfig;

            // Yo soy el método
            // Él es el evento

            if (other == null)
                return false;

            // Nuestro nombre tiene que ser igual
            if (o.Name.Equals(Name))
            {
                // Busco en mis parámetros
                foreach (var kvp in ParameterConfig)
                {
                    var otherHasParam = o.ParameterConfig.ContainsKey(kvp.Key);
                    // Si el otro no tiene parámetro pero yo tengo un valor por defecto
                    if ((!otherHasParam && !ParameterHasDefault[kvp.Key])
                        // Si el evento tiene un parámetro pero yo no me lo puedo asignar ó sea object que podría ser cualquier cosa
                        || (otherHasParam && !kvp.Value.IsAssignableFrom(o.ParameterConfig[kvp.Key])))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }


        public static string SplitCamelCase(string input)
        {
            return System.Text.RegularExpressions.Regex.Replace(input, "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Multiline).Trim();
        }
    }
}