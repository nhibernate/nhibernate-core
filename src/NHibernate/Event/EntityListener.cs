using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using NHibernate.Persister.Entity;
using NHibernate.Util;

namespace NHibernate.Event
{
    internal class EventConfig
    {
        public object Entity { get; set; }

        public IEntityPersister Persister { get; set; }

        public IEventSource Session { get; set; }

        public object[] State { get; set; }

        public bool ShouldUpdateState
        {
            get { return State != null; }
        }
    }

    [Serializable]
    public class EntityListener : IPreUpdateEventListener, IPreInsertEventListener, IPreDeleteEventListener,
        IFlushEntityEventListener, IPostUpdateEventListener, IPostInsertEventListener, IPostDeleteEventListener
    {
        [NonSerialized]
        private Dictionary<System.Type, Dictionary<System.Type, IEnumerable<MethodInfo>>>
            _methodsWithListenerAttributeCache;

        public EntityListener()
        {
            ResetCache();
        }

        [OnDeserialized]
        private void ResetCacheOnDeserialize(StreamingContext context)
        {
            ResetCache();
        }

        private void ResetCache()
        {
            _methodsWithListenerAttributeCache =
                new Dictionary<System.Type, Dictionary<System.Type, IEnumerable<MethodInfo>>>();
        }

        public void OnFlushEntity(FlushEntityEvent @event)
        {
        }

        public void OnPostDelete(PostDeleteEvent @event)
        {
            InvokeMethodsWithAttribute(new EventConfig
            {
                Entity = @event.Entity,
                Persister = @event.Persister,
                Session = @event.Session
            }, typeof (PostDeleteAttribute));
        }

        public void OnPostInsert(PostInsertEvent @event)
        {
            InvokeMethodsWithAttribute(new EventConfig
            {
                Entity = @event.Entity,
                Persister = @event.Persister,
                Session = @event.Session
            }, typeof (PostInsertAttribute));
        }

        public void OnPostUpdate(PostUpdateEvent @event)
        {
            InvokeMethodsWithAttribute(new EventConfig
            {
                Entity = @event.Entity,
                Persister = @event.Persister,
                Session = @event.Session
            }, typeof (PostUpdateAttribute));
        }

        public bool OnPreDelete(PreDeleteEvent @event)
        {
            InvokeMethodsWithAttribute(new EventConfig
            {
                Entity = @event.Entity,
                Persister = @event.Persister,
                Session = @event.Session
            }, typeof (PreDeleteAttribute));
            return false;
        }

        public bool OnPreInsert(PreInsertEvent @event)
        {
            InvokeMethodsWithAttribute(new EventConfig
            {
                Entity = @event.Entity,
                Persister = @event.Persister,
                Session = @event.Session,
                State = @event.State
            }, typeof (PreInsertAttribute));
            return false;
        }

        public bool OnPreUpdate(PreUpdateEvent @event)
        {
            InvokeMethodsWithAttribute(new EventConfig
            {
                Entity = @event.Entity,
                Persister = @event.Persister,
                Session = @event.Session,
                State = @event.State
            }, typeof (PreUpdateAttribute));
            return false;
        }

        private static void InvokeAndApplyDirtyFields(EventConfig config, MethodInfo method)
        {
            Dictionary<string, object> initialFieldValues = null;
            if (config.ShouldUpdateState)
            {
                initialFieldValues = GetFieldValues(config.Entity.GetType(), config.Entity);
            }

            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length == 0)
            {
                method.Invoke(config.Entity, new object[] {});
            }
            else if (parameters.Length == 1 && parameters[0].ParameterType == typeof (ISession))
            {
                method.Invoke(config.Entity, new object[] {config.Session});
            }
            else
            {
                throw new ArgumentException(
                    "Error while invoking attribute event method, only 0 or 1 params (ISession) is supported!");
            }

            if (config.ShouldUpdateState)
            {
                UpdateDirtyFieldValuesInState(config, initialFieldValues);
            }
        }

        private void InvokeMethodsWithAttribute(EventConfig config, System.Type attributeType)
        {
            var entityType = config.Entity.GetType();
            if (!_methodsWithListenerAttributeCache.ContainsKey(entityType))
            {
                _methodsWithListenerAttributeCache.Add(entityType,
                                                       new Dictionary<System.Type, IEnumerable<MethodInfo>>
                                                       {
                                                           {
                                                               attributeType,
                                                               GetMethodsWithListenerAttribute(attributeType, entityType)
                                                           }
                                                       });
            }

            Dictionary<System.Type, IEnumerable<MethodInfo>> cacheDict = _methodsWithListenerAttributeCache[entityType];
            if (!cacheDict.ContainsKey(attributeType))
            {
                cacheDict.Add(attributeType, GetMethodsWithListenerAttribute(attributeType, entityType));
            }

            cacheDict[attributeType].ForEach(method => InvokeAndApplyDirtyFields(config, method));
        }

        private static IEnumerable<MethodInfo> GetMethodsWithListenerAttribute(System.Type attributeType,
                                                                               System.Type entityType)
        {
            return entityType
                .GetMethods()
                .Where(method => method.GetCustomAttributes(attributeType, false).Length > 0);
        }

        // WHY should you update presistent state AND the object itself?
        // http: //ayende.com/blog/3987/nhibernate-ipreupdateeventlistener-ipreinserteventlistener
        private static void Set(IEntityPersister persister, object[] state, string propertyName, object value)
        {
            int index = Array.IndexOf(persister.PropertyNames, propertyName);
            if (index == -1)
                return;
            state[index] = value;
        }

        private static Dictionary<string, object> GetFieldValues(System.Type type, object obj)
        {
            return type.GetProperties().ToDictionary(
                property => property.Name,
                property => property.GetValue(obj, null));
        }

        private static void UpdateDirtyFieldValuesInState(EventConfig config,
                                                          Dictionary<string, object> initialFieldValues)
        {
            foreach (var actualValue in GetFieldValues(config.Entity.GetType(), config.Entity))
            {
                if (!Equals(initialFieldValues[actualValue.Key], actualValue.Value))
                {
                    Set(config.Persister, config.State, actualValue.Key, actualValue.Value);
                }
            }
        }
    }
}