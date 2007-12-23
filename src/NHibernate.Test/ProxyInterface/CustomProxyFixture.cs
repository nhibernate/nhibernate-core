using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using Castle.DynamicProxy;
using NHibernate.Bytecode;
using NHibernate.Engine;
using NHibernate.Proxy;
using NHibernate.Proxy.Poco.Castle;
using NHibernate.Test.ExpressionTest.SubQueries;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.ProxyInterface
{
	[TestFixture]
	public class CustomProxyFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new string[] { "ExpressionTest.SubQueries.Mappings.hbm.xml" }; }
		}

		protected override void Configure(Cfg.Configuration configuration)
		{
			configuration.Properties[NHibernate.Cfg.Environment.ProxyFactoryFactoryClass] =
				typeof(CustomProxyFactoryFactory).AssemblyQualifiedName;
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
				Blog blog = (Blog)s.Load(typeof(Blog), 1);
				INotifyPropertyChanged propertyChanged = (INotifyPropertyChanged)blog;
				string propChanged = null;
				propertyChanged.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
				{
					propChanged = e.PropertyName;
				};

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

		#endregion
	}

	public class DataBindingProxyFactory : CastleProxyFactory
	{
		public override INHibernateProxy GetProxy(object id, ISessionImplementor session)
		{
			try
			{
				CastleLazyInitializer initializer = new DataBindingInterceptor(EntityName, PersistentClass, id,
																				GetIdentifierMethod, SetIdentifierMethod, ComponentIdType, session);

				object generatedProxy = null;

				ArrayList list = new ArrayList(Interfaces);
				list.Add(typeof(INotifyPropertyChanged));
				System.Type[] interfaces = (System.Type[])list.ToArray(typeof(System.Type));
				if (IsClassProxy)
				{
					generatedProxy = DefaultProxyGenerator.CreateClassProxy(PersistentClass, interfaces, ProxyGenerationOptions.Default, initializer);
				}
				else
				{
					generatedProxy = DefaultProxyGenerator.CreateInterfaceProxyWithoutTarget(interfaces[0], interfaces, initializer);
				}

				initializer._constructed = true;
				return (INHibernateProxy)generatedProxy;
			}
			catch (Exception e)
			{
				log.Error("Creating a proxy instance failed", e);
				throw new HibernateException("Creating a proxy instance failed", e);
			}
		}
	}

	public class DataBindingInterceptor : CastleLazyInitializer
	{
		private PropertyChangedEventHandler subscribers = delegate { };
		public DataBindingInterceptor(string entityName, System.Type persistentClass, object id, 
			MethodInfo getIdentifierMethod, MethodInfo setIdentifierMethod, IAbstractComponentType componentIdType, ISessionImplementor session) 
			: base(entityName, persistentClass, id, getIdentifierMethod, setIdentifierMethod, componentIdType, session) {}

		//public DataBindingInterceptor(System.Type persistentClass, object id, MethodInfo getIdentifierMethod, MethodInfo setIdentifierMethod, ISessionImplementor session)
		//  : base(persistentClass, id, getIdentifierMethod, setIdentifierMethod, session)
		//{
		//}

		public override void Intercept(Castle.Core.Interceptor.IInvocation invocation)
		{
			object result = null;
			if (invocation.Method.DeclaringType == typeof(INotifyPropertyChanged))
			{
				PropertyChangedEventHandler propertyChangedEventHandler = (PropertyChangedEventHandler)invocation.GetArgumentValue(0);
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
