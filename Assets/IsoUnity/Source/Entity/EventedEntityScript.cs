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
        private Dictionary<MethodInfo, bool> autofinish;

        public override void eventHappened(IGameEvent ge)
        {
            Current = ge;

            if(calls != null && calls.Count > 0)
            {
                var config = new GameEventConfig(ge);

                if (ge.belongsTo(this, this.GetType().Name) && calls.ContainsKey(config))
                {
                    List<object> parameters = new List<object>();
                    var call = calls[config];
                    foreach (var p in call.GetParameters())
                        parameters.Add(ge.getParameter(p.Name));
                    
                    var output = call.Invoke(this, parameters.ToArray());

                    if(output is IEnumerator)
                        // If we want to autofinish it we use the controller, else just launch it
                        StartCoroutine(autofinish[call] ? CoroutineController(ge, output as IEnumerator) : output as IEnumerator);
                    else if (autofinish[call])
                        // If is not a coroutine and we have to auto finish it, we just do it
                        Game.main.eventFinished(ge);

                }
            }

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

        public override Option[] getOptions()
        {
            return null;
        }

        protected IGameEvent Current { get; private set; }

        public override void tick()
        {
            if (!inited)
            {
                calls = new Dictionary<GameEventConfig, MethodInfo>();
                autofinish = new Dictionary<MethodInfo, bool>();

                foreach (var method in this.GetType().GetMethods().ToList()
                      .FindAll(m => m.GetCustomAttributes(typeof(GameEventAttribute), true).Length > 0)
                      .ToArray())
                {
                    
                    autofinish.Add(method, ((GameEventAttribute)method.GetCustomAttributes(typeof(GameEventAttribute), true)[0]).AutoFinish);
                    calls.Add(new GameEventConfig(this.GetType(), method), method);
                }

                inited = true;
            }
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class GameEventAttribute : System.Attribute
    {
        public bool AutoFinish { get; private set; }
        public GameEventAttribute(bool autoFinish = true)
        {
            AutoFinish = autoFinish;
        }
    }

    public class GameEventConfig 
    {
        public string Name { get; private set; }
        public MethodInfo Methid { get; private set; }
        public Dictionary<string, System.Type> ParameterConfig { get; private set; }

        public Dictionary<string, bool> ParameterHasDefault { get; private set; }
        public Dictionary<string, object> DefaultValue { get; private set; }

        public GameEventConfig(Type t, MethodInfo m)
        { 
            Name = SplitCamelCase(m.Name).ToLower();
            ParameterConfig = new Dictionary<string, Type>();
            ParameterHasDefault = new Dictionary<string, bool>();
            DefaultValue = new Dictionary<string, object>();

            ParameterConfig.Add(t.Name.ToLower(), t);
            ParameterHasDefault.Add(t.Name.ToLower(), false);

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