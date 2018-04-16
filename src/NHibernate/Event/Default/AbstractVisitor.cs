using NHibernate.Persister.Entity;
using NHibernate.Type;

namespace NHibernate.Event.Default
{
	/// <summary> 
	/// Abstract superclass of algorithms that walk a tree of property values of an entity, and
	/// perform specific functionality for collections, components and associated entities. 
	/// </summary>
	public abstract partial class AbstractVisitor
	{
		private readonly IEventSource session;

		public AbstractVisitor(IEventSource session)
		{
			this.session = session;
		}

		public IEventSource Session
		{
			get { return session; }
		}

		/// <summary> Dispatch each property value to ProcessValue(). </summary>
		/// <param name="values"> </param>
		/// <param name="types"> </param>
		internal void ProcessValues(object[] values, IType[] types)
		{
			for (int i = 0; i < types.Length; i++)
			{
				if (IncludeProperty(values, i))
					ProcessValue(i, values, types);
			}
		}

		internal virtual void ProcessValue(int i, object[] values, IType[] types)
		{
			ProcessValue(values[i], types[i]);
		}

		/// <summary> 
		/// Visit a property value. Dispatch to the correct handler for the property type.
		/// </summary>
		/// <param name="value"> </param>
		/// <param name="type"> </param>
		internal object ProcessValue(object value, IType type)
		{
			if (type.IsCollectionType)
			{
				//even process null collections
				return ProcessCollection(value, (CollectionType)type);
			}
			else if (type.IsEntityType)
			{
				return ProcessEntity(value, (EntityType)type);
			}
			else if (type.IsComponentType)
			{
				return ProcessComponent(value, (IAbstractComponentType)type);
			}
			else
			{
				return null;
			} 
		}

		/// <summary>
		/// Visit a component. Dispatch each property to <see cref="ProcessValues"/>
		/// </summary>
		/// <param name="component"></param>
		/// <param name="componentType"></param>
		/// <returns></returns>
		internal virtual object ProcessComponent(object component, IAbstractComponentType componentType)
		{
			if (component != null)
			{
				ProcessValues(componentType.GetPropertyValues(component, session), componentType.Subtypes);
			}
			return null;
		}

		/// <summary>
		///  Visit a many-to-one or one-to-one associated entity. Default superclass implementation is a no-op.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="entityType"></param>
		/// <returns></returns>
		internal virtual object ProcessEntity(object value, EntityType entityType)
		{
			return null;
		}

		/// <summary>
		/// Visit a collection. Default superclass implementation is a no-op.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="collectionType"></param>
		/// <returns></returns>
		internal virtual object ProcessCollection(object value, CollectionType collectionType)
		{
			return null;
		}

		/// <summary>
		/// Walk the tree starting from the given entity.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="persister"></param>
		internal virtual void Process(object obj, IEntityPersister persister)
		{
			ProcessEntityPropertyValues(persister.GetPropertyValues(obj), persister.PropertyTypes);
		}

		public void ProcessEntityPropertyValues(object[] values, IType[] types)
		{
			for (int i = 0; i < types.Length; i++)
			{
				if (IncludeEntityProperty(values, i))
				{
					ProcessValue(i, values, types);
				}
			}
		}

		internal virtual bool IncludeEntityProperty(object[] values, int i)
		{
			return IncludeProperty(values, i);
		}

		internal bool IncludeProperty(object[] values, int i)
		{
			return !Equals(Intercept.LazyPropertyInitializer.UnfetchedProperty, values[i]);
		}
	}
}
