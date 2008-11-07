using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using Castle.Core.Interceptor;
using Castle.DynamicProxy;
using NHibernate.Bytecode;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Proxy;
using NHibernate.Type;
using NUnit.Framework;
using Environment=NHibernate.Cfg.Environment;

namespace NHibernate.ByteCode.Castle.Tests.ProxyInterface
{
	[TestFixture]
	public class CustomProxyFixture : TestCase
	{
		protected override IList Mappings
		{
			get { return new[] { "ProxyInterface.Mappings.hbm.xml" }; }
		}

		protected override void Configure(Configuration configuration)
		{
			configuration.Properties[Environment.ProxyFactoryFactoryClass] =
				typeof (CustomProxyFactoryFactory).AssemblyQualifiedName;
		}

		[Test]
		public void CanImplementNotifyPropertyChanged()
		{
			using (ISession s = OpenSession())
			{
				s.Save(new Blog("blah"));
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				var blog = (Blog) s.Load(typeof (Blog), 1);
				var propertyChanged = (INotifyPropertyChanged) blog;
				string propChanged = null;
				propertyChanged.PropertyChanged +=
					delegate(object sender, PropertyChangedEventArgs e) { propChanged = e.PropertyName; };

				blog.BlogName = "foo";
				Assert.AreEqual("BlogName", propChanged);
			}

			using (ISession s = OpenSession())
			{
				s.Delete("from Blog");
				s.Flush();
			}
		}
	}

	public class CustomProxyFactoryFactory : IProxyFactoryFactory
	{
		#region IProxyFactoryFactory Members

		public IProxyFactory BuildProxyFactory()
		{
			return new DataBindingProxyFactory();
		}

		public IProxyValidator ProxyValidator
		{
			get { return new DynProxyTypeValidator(); }
		}

		#endregion
	}

	public class DataBindingProxyFactory : ProxyFactory
	{
		public override INHibernateProxy GetProxy(object id, ISessionImplementor session)
		{
			try
			{
				LazyInitializer initializer = new DataBindingInterceptor(EntityName, PersistentClass, id, GetIdentifierMethod,
				                                                         SetIdentifierMethod, ComponentIdType, session);

				object generatedProxy;

				var list = new ArrayList(Interfaces);
				list.Add(typeof (INotifyPropertyChanged));
				var interfaces = (System.Type[]) list.ToArray(typeof (System.Type));
				if (IsClassProxy)
				{
					generatedProxy = DefaultProxyGenerator.CreateClassProxy(PersistentClass, interfaces, ProxyGenerationOptions.Default,
					                                                        initializer);
				}
				else
				{
					generatedProxy = DefaultProxyGenerator.CreateInterfaceProxyWithoutTarget(interfaces[0], interfaces, initializer);
				}

				initializer._constructed = true;
				return (INHibernateProxy) generatedProxy;
			}
			catch (Exception e)
			{
				log.Error("Creating a proxy instance failed", e);
				throw new HibernateException("Creating a proxy instance failed", e);
			}
		}
	}

	public class DataBindingInterceptor : LazyInitializer
	{
		private PropertyChangedEventHandler subscribers = delegate { };

		public DataBindingInterceptor(string entityName, System.Type persistentClass, object id,
		                              MethodInfo getIdentifierMethod, MethodInfo setIdentifierMethod,
		                              IAbstractComponentType componentIdType, ISessionImplementor session)
			: base(entityName, persistentClass, id, getIdentifierMethod, setIdentifierMethod, componentIdType, session) {}

		//public DataBindingInterceptor(System.Type persistentClass, object id, MethodInfo getIdentifierMethod, MethodInfo setIdentifierMethod, ISessionImplementor session)
		//  : base(persistentClass, id, getIdentifierMethod, setIdentifierMethod, session)
		//{
		//}

		public override void Intercept(IInvocation invocation)
		{
			object result;
			if (invocation.Method.DeclaringType == typeof (INotifyPropertyChanged))
			{
				var propertyChangedEventHandler = (PropertyChangedEventHandler) invocation.GetArgumentValue(0);
				if (invocation.Method.Name.StartsWith("add_"))
				{
					subscribers += propertyChangedEventHandler;
				}
				else
				{
					subscribers -= propertyChangedEventHandler;
				}
				return;
			}
			base.Intercept(invocation);
			result = invocation.ReturnValue;
			if (invocation.Method.Name.StartsWith("set_"))
			{
				subscribers(this, new PropertyChangedEventArgs(invocation.Method.Name.Substring(4)));
			}
			invocation.ReturnValue = result;
		}
	}
}