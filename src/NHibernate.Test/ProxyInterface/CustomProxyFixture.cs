using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using Castle.DynamicProxy;
using NHibernate.Engine;
using NHibernate.Proxy;
using NHibernate.Test.ExpressionTest.SubQueries;
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

		protected override void BuildSessionFactory()
		{
			cfg.SetProxyFactoryClass(typeof(DataBindingProxyFactory));
			base.BuildSessionFactory();
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

	public class DataBindingProxyFactory : CastleProxyFactory
	{
		public override INHibernateProxy GetProxy(object id, ISessionImplementor session)
		{
			try
			{
				CastleLazyInitializer initializer = new DataBindingInterceptor(_persistentClass, id,
																			  _getIdentifierMethod, _setIdentifierMethod, session);

				object generatedProxy = null;

				ArrayList list = new ArrayList(_interfaces);
				list.Add(typeof(INotifyPropertyChanged));
				System.Type[] interfaces = (System.Type[])list.ToArray(typeof(System.Type));
				if (IsClassProxy)
				{
					generatedProxy = _proxyGenerator.CreateClassProxy(_persistentClass, interfaces, initializer, false);
				}
				else
				{
					generatedProxy = _proxyGenerator.CreateProxy(interfaces, initializer, new object());
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

		public DataBindingInterceptor(System.Type persistentClass, object id, MethodInfo getIdentifierMethod, MethodInfo setIdentifierMethod, ISessionImplementor session)
			: base(persistentClass, id, getIdentifierMethod, setIdentifierMethod, session)
		{
		}

		public override object Intercept(IInvocation invocation, params object[] args)
		{
			if (invocation.Method.DeclaringType == typeof(INotifyPropertyChanged))
			{
				PropertyChangedEventHandler propertyChangedEventHandler = (PropertyChangedEventHandler)args[0];
				if (invocation.Method.Name.StartsWith("add_"))
				{
					subscribers += propertyChangedEventHandler;
				}
				else
				{
					subscribers -= propertyChangedEventHandler;
				}
				return null;
			}
			object result = base.Intercept(invocation, args);
			if (invocation.Method.Name.StartsWith("set_"))
			{
				subscribers(this, new PropertyChangedEventArgs(invocation.Method.Name.Substring(4)));
			}
			return result;
		}
	}
}
