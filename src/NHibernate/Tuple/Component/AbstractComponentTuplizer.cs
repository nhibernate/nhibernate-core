using System;

using NHibernate.Engine;
using NHibernate.Properties;

namespace NHibernate.Tuple.Component
{
	/// <summary> Support for tuplizers relating to components. </summary>
	[Serializable]
	public abstract class AbstractComponentTuplizer : IComponentTuplizer
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(AbstractComponentTuplizer));

		protected internal int propertySpan;
		protected internal IGetter[] getters;
		protected internal ISetter[] setters;
		protected internal IInstantiator instantiator;
		protected internal bool hasCustomAccessors;

		protected internal AbstractComponentTuplizer(Mapping.Component component)
		{
			propertySpan = component.PropertySpan;
			getters = new IGetter[propertySpan];
			setters = new ISetter[propertySpan];

			bool foundCustomAccessor = false;
			int i = 0;
			foreach (Mapping.Property prop in component.PropertyIterator)
			{
				getters[i] = BuildGetter(component, prop);
				setters[i] = BuildSetter(component, prop);
				if (!prop.IsBasicPropertyAccessor)
				{
					foundCustomAccessor = true;
				}
				i++;
			}
			if (log.IsDebugEnabled)
			{
				log.DebugFormat("{0} accessors found for component: {1}", foundCustomAccessor ? "Custom" : "No custom",
								component.ComponentClassName);
			}
			hasCustomAccessors = foundCustomAccessor;

			// Only to be secure that we can access to every things
			string[] getterNames = new string[propertySpan];
			string[] setterNames = new string[propertySpan];
			System.Type[] propTypes = new System.Type[propertySpan];
			for (int j = 0; j < propertySpan; j++)
			{
				getterNames[j] = getters[j].PropertyName;
				setterNames[j] = setters[j].PropertyName;
				propTypes[j] = getters[j].ReturnType;
			}

			// Fix for NH-3119
			//instantiator = BuildInstantiator(component);
		}

		#region IComponentTuplizer Members

		public virtual object GetParent(object component)
		{
			return null;
		}

		public virtual void SetParent(object component, object parent, ISessionFactoryImplementor factory)
		{
			throw new NotSupportedException();
		}

		public virtual bool HasParentProperty
		{
			get { return false; }
		}

		#endregion

		#region ITuplizer Members

		public abstract System.Type MappedClass { get; }

		public virtual object[] GetPropertyValues(object component)
		{
			object[] values = new object[propertySpan];
			// NH Different behavior : for NH-1101
			if (component != null)
				for (int i = 0; i < propertySpan; i++)
					values[i] = GetPropertyValue(component, i);

			return values;
		}

		public virtual void SetPropertyValues(object component, object[] values)
		{
			for (int i = 0; i < propertySpan; i++)
			{
				setters[i].Set(component, values[i]);
			}
		}

		public virtual object GetPropertyValue(object component, int i)
		{
			return getters[i].Get(component);
		}

		/// <summary> This method does not populate the component parent</summary>
		public virtual object Instantiate()
		{
			return instantiator.Instantiate();
		}

		public virtual bool IsInstance(object obj)
		{
			return instantiator.IsInstance(obj);
		}

		#endregion

		protected internal abstract IInstantiator BuildInstantiator(Mapping.Component component);
		protected internal abstract IGetter BuildGetter(Mapping.Component component, Mapping.Property prop);
		protected internal abstract ISetter BuildSetter(Mapping.Component component, Mapping.Property prop);

	}
}
