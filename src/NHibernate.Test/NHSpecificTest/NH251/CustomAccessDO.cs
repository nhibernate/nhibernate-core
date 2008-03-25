using System.Collections;
using System.Reflection;
using NHibernate.Engine;
using NHibernate.Properties;

namespace NHibernate.Test.NHSpecificTest.NH251
{
	/// <summary>
	/// The DictionaryAccessor access strategy uses this interface to
	/// access properties
	/// </summary>
	public interface IDynamicFieldContainer
	{
		IDictionary Fields { get; }
	}

	// A component class
	public class Name
	{
		private string first;
		private string last;
	}

	/// <summary>
	/// A domain object for the test case
	/// </summary>
	public class CustomAccessDO : IDynamicFieldContainer
	{
		public IDictionary dynamicFields = new Hashtable(); // may contain components
		public int id;

		public IDictionary Fields
		{
			get { return dynamicFields; }
		}
	}


	/// <summary>
	/// Custom access strategy that uses IDynamicFieldContainer to get/set property values
	/// </summary>
	public class DictionaryAccessor : IPropertyAccessor
	{
		public IGetter GetGetter(System.Type theClass, string propertyName)
		{
			return new CustomGetter(theClass, propertyName);
		}

		public ISetter GetSetter(System.Type theClass, string propertyName)
		{
			return new CustomSetter(propertyName);
		}

		public class CustomGetter : IGetter
		{
			private System.Type theClass;
			private string propertyName;

			public CustomGetter(System.Type theClass, string propertyName)
			{
				this.theClass = theClass;
				this.propertyName = propertyName;
			}

			public object Get(object target)
			{
				IDynamicFieldContainer container = (IDynamicFieldContainer) target;
				return container.Fields[propertyName];
			}

			public System.Type ReturnType
			{
				get { return theClass; }
			}

			public string PropertyName
			{
				get { return propertyName; }
			}

			public MethodInfo Method
			{
				get { return null; }
			}

			public object GetForInsert(object owner, IDictionary mergeMap, ISessionImplementor session)
			{
				return Get(owner);
			}

// Optional operation (return null)
		}

		public class CustomSetter : ISetter
		{
			private string propertyName;

			public CustomSetter(string propertyName)
			{
				this.propertyName = propertyName;
			}

			public void Set(object target, object value)
			{
				IDynamicFieldContainer container = (IDynamicFieldContainer) target;
				container.Fields[propertyName] = value;
			}

			public string PropertyName
			{
				get { return propertyName; }
			}

			public MethodInfo Method
			{
				get { return null; }
			} // Optional operation
		}
	}
}