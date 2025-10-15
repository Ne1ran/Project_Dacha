using System;
using System.Collections.Generic;
using System.Reflection;
using Core.Attributes;
using Core.Console.Service;
using Core.Descriptors.Service;
using Core.EntryPoints;
using Core.Notifications.Service;
using Core.Resources.Service;
using Core.SceneManagement.Service;
using Core.Serialization;
using Core.UI.Service;
using Game.Utils;
using MessagePipe;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Object = System.Object;

namespace Core.Scopes
{
    public class ApplicationScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            AppContext.ApplicationScope = this;
            AppContext.CurrentScope = this;
            
            builder.Register<AddressablesManager>(Lifetime.Singleton);
            builder.Register<PrefabBinderManager>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();;
            builder.Register<IResourceService, AddressablesResourceService>(Lifetime.Singleton);
            builder.Register<SceneService>(Lifetime.Singleton);
            builder.Register<ConsoleService>(Lifetime.Singleton);
            builder.Register<IDescriptorService, ResourcesDescriptorsService>(Lifetime.Singleton);
            builder.RegisterMessagePipe();
            
            builder.Register<UIService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<ISerializer, JsonSerializer>(Lifetime.Singleton);
            builder.Register<NotificationManager>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            RegisterDescriptors(builder);
            builder.RegisterComponentInHierarchy<ApplicationEntryPoint>();
        }

        private void RegisterDescriptors(IContainerBuilder builder)
        {
            List<Type> typesWithDescriptor = new();

            HashSet<Assembly> assemblies = new() {
                    Assembly.GetExecutingAssembly()
            };

            try {
                assemblies.Add(Assembly.Load("Assembly-CSharp"));
            } catch {
                // ignored
            }

            foreach (Assembly assembly in assemblies) {
                foreach (Type type in assembly.GetTypes()) {
                    DescriptorAttribute attribute = type.GetCustomAttribute<DescriptorAttribute>();
                    if (attribute == null) {
                        continue;
                    }

                    typesWithDescriptor.Add(type);
                }
            }

            foreach (Type type in typesWithDescriptor) {
                DescriptorAttribute descriptorAttribute = ReflectionUtils.RequireAttribute<DescriptorAttribute>(type);
                Object? loadDesc = UnityEngine.Resources.Load(descriptorAttribute.DescriptorPath, type);
                if (loadDesc == null) {
                    throw new ArgumentException($"Descriptor not found on path={descriptorAttribute.DescriptorPath}");
                }

                ScriptableObject? descriptor = loadDesc as ScriptableObject;
                if (descriptor == null) {
                    throw new ArgumentException($"Descriptor is not castable on path={descriptorAttribute.DescriptorPath}");
                }
                
                builder.RegisterInstance(descriptor).As(type).AsSelf();
            }
        }
    } 
}