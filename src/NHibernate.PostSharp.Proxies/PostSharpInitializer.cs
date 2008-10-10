using System;
using System.Reflection;
using NHibernate.Engine;
using NHibernate.Proxy;
using PostSharp.Laos;

namespace NHibernate.PostSharp.Proxies
{
    public class PostSharpInitializer : ILazyInitializer
    {
        private readonly System.Type persistentClass;
        private readonly MethodInfo getIdentifierMethod;
        private readonly MethodInfo setIdentifierMethod;
        private object implementation;

        public PostSharpInitializer(string entityName,
            System.Type persistentClass,
            object identifier,
            MethodInfo getIdentifierMethod,
            MethodInfo setIdentifierMethod,
            ISessionImplementor session)
        {
            EntityName = entityName;
            Identifier = identifier;
            Session = session;
            this.persistentClass = persistentClass;
            this.getIdentifierMethod = getIdentifierMethod;
            this.setIdentifierMethod = setIdentifierMethod;
            IsUninitialized = true;
        }

        public void Initialize()
        {
            implementation = Session.ImmediateLoad(EntityName, Identifier);
            IsUninitialized = false;
        }

        public object Identifier { get; set; }

        public string EntityName { get; private set; }

        public System.Type PersistentClass
        {
            get { return persistentClass; }
        }

        public bool IsUninitialized { get; private set; }
        public bool Unwrap { get; set; }
        public ISessionImplementor Session { get; set; }
        public object GetImplementation()
        {
            return this;
        }

        public object GetImplementation(ISessionImplementor s)
        {
            return this;
        }

        public void SetImplementation(object target)
        {
            throw new System.NotImplementedException();
        }

        public void InitializeIfNeeded()
        {
            if (IsUninitialized)
            {
                Initialize();
            }
        }

        public void Intercept(MethodInvocationEventArgs eventArgs)
        {
            //TODO: ugly hack to compare the post sharp generated method
            // to what NH thinks as the identifier method. Need to think
            // of a more elegant way of doing this
            string cleanedUpMethodName = eventArgs.Delegate.Method.Name.Substring(1);
            if (cleanedUpMethodName ==
                getIdentifierMethod.Name)
            {
                eventArgs.ReturnValue = Identifier;
                return;
            }
            object[] array = eventArgs.GetArgumentArray();

            if (cleanedUpMethodName ==
                setIdentifierMethod.Name)
            {
                Identifier = array[0];
            }
            InitializeIfNeeded();

            eventArgs.ReturnValue = eventArgs.Delegate.Method.Invoke(implementation, array);
        }
    }
}
