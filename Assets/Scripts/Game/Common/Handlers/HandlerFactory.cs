using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Game.Utils;
using VContainer;
using VContainer.Unity;

namespace Game.Common.Handlers
{
    public class HandlerFactory<TInterface> : IInitializable
    {
        [Inject]
        private readonly IObjectResolver _resolver = null!;

        private Dictionary<string, Type> _handlerTypes = null!;

        public void Initialize()
        {
            HashSet<Assembly> assemblies = new() {
                    Assembly.GetExecutingAssembly()
            };

            try {
                assemblies.Add(Assembly.Load("Assembly-CSharp"));
            } catch {
                // ignored
            }

            _handlerTypes = ReflectionUtils
                            .FindAllTypesWithAttributeAndInterface<HandlerAttribute, TInterface>(assemblies.Select(a => a.FullName).ToList())
                            .ToDictionary(t => t.Item2.Name, t => t.Item1);
        }

        public TInterface Create(string name)
        {
            Type handlerType = _handlerTypes.Require(name);
            TInterface behaviour = (TInterface) Activator.CreateInstance(handlerType);
            _resolver.Inject(behaviour);
            return behaviour;
        }
    }
}