using System.Collections;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Loader;
using NHibernate.Util;

namespace NHibernate.Type
{
	/*
	/// <summary>
	/// Handles "dynamic" components, represented as <tt>Map</tt>s
	/// </summary>
	public class DynamicComponentType : AbstractType, IAbstractComponentType
	{
		private string[] propertyNames;
		private IType[] propertyTypes;
		private int propertySpan;
		private Cascades.CascadeStyle[] cascade;
		private OuterJoinFetchStrategy[] joinedFetch;

		public DynamicComponentType(
			string[] propertyNames,
			IType[] propertyTypes,
			int propertySpan,
			Cascades.CascadeStyle[] cascade,
			OuterJoinFetchStrategy[] joinedFetch
			)
		{
			this.propertyNames = propertyNames;
			this.propertyTypes = propertyTypes;
			this.propertySpan = propertySpan;
			this.cascade = cascade;
			this.joinedFetch = joinedFetch;
		}

		#region IAbstractComponentType Members

		public IType[] Subtypes
		{
			get	{ return propertyTypes;	} 
		}

		public string[] PropertyNames
		{
			get
			{
				// TODO:  Add DynamicComponentType.PropertyNames getter implementation
				return null;
			}
		}

		public object[] GetPropertyValues(object component, ISessionImplementor session)
		{
			// TODO:  Add DynamicComponentType.GetPropertyValues implementation
			return null;
		}

		public void SetPropertyValues(object component, object[] values)
		{
			// TODO:  Add DynamicComponentType.SetPropertyValues implementation
		}

		public object GetPropertyValue(object component, int i, ISessionImplementor session)
		{
			// TODO:  Add DynamicComponentType.GetPropertyValue implementation
			return null;
		}

		public NHibernate.Engine.Cascades.CascadeStyle Cascade(int i)
		{
			// TODO:  Add DynamicComponentType.Cascade implementation
			return null;
		}

		public NHibernate.Loader.OuterJoinFetchStrategy EnableJoinedFetch(int i)
		{
			// TODO:  Add DynamicComponentType.EnableJoinedFetch implementation
			return null;
		}

		#endregion

		#region IType Members

		public bool IsAssociationType
		{
			get
			{
				// TODO:  Add DynamicComponentType.IsAssociationType getter implementation
				return false;
			}
		}

		public bool IsPersistentCollectionType
		{
			get
			{
				// TODO:  Add DynamicComponentType.IsPersistentCollectionType getter implementation
				return false;
			}
		}

		public bool IsComponentType
		{
			get
			{
				// TODO:  Add DynamicComponentType.IsComponentType getter implementation
				return false;
			}
		}

		public bool IsEntityType
		{
			get
			{
				// TODO:  Add DynamicComponentType.IsEntityType getter implementation
				return false;
			}
		}

		public bool IsObjectType
		{
			get
			{
				// TODO:  Add DynamicComponentType.IsObjectType getter implementation
				return false;
			}
		}

		public NHibernate.SqlTypes.SqlType[] SqlTypes(IMapping mapping)
		{
			// TODO:  Add DynamicComponentType.SqlTypes implementation
			return null;
		}

		public int GetColumnSpan(IMapping mapping)
		{
			// TODO:  Add DynamicComponentType.GetColumnSpan implementation
			return 0;
		}

		public System.Type ReturnedClass
		{
			get
			{
				// TODO:  Add DynamicComponentType.ReturnedClass getter implementation
				return null;
			}
		}

		public bool Equals(object x, object y)
		{
			// TODO:  Add DynamicComponentType.Equals implementation
			return false;
		}

		public bool IsDirty(object old, object current, ISessionImplementor session)
		{
			// TODO:  Add DynamicComponentType.IsDirty implementation
			return false;
		}

		public object NullSafeGet(System.Data.IDataReader rs, string[] names, ISessionImplementor session, object owner)
		{
			// TODO:  Add DynamicComponentType.NullSafeGet implementation
			return null;
		}

		object NHibernate.Type.IType.NullSafeGet(System.Data.IDataReader rs, string name, ISessionImplementor session, System.Object owner)
		{
			// TODO:  Add DynamicComponentType.NHibernate.Type.IType.NullSafeGet implementation
			return null;
		}

		public void NullSafeSet(System.Data.IDbCommand st, object value, int index, ISessionImplementor session)
		{
			// TODO:  Add DynamicComponentType.NullSafeSet implementation
		}

		public string ToXML(object value, ISessionFactoryImplementor factory)
		{
			// TODO:  Add DynamicComponentType.ToXML implementation
			return null;
		}

		public string Name
		{
			get
			{
				// TODO:  Add DynamicComponentType.Name getter implementation
				return null;
			}
		}

		public object DeepCopy(object val)
		{
			// TODO:  Add DynamicComponentType.DeepCopy implementation
			return null;
		}

		public bool IsMutable
		{
			get
			{
				// TODO:  Add DynamicComponentType.IsMutable getter implementation
				return false;
			}
		}

		public object Disassemble(object value, ISessionImplementor session)
		{
			// TODO:  Add DynamicComponentType.Disassemble implementation
			return null;
		}

		public object Assemble(object cached, ISessionImplementor session, object owner)
		{
			// TODO:  Add DynamicComponentType.Assemble implementation
			return null;
		}

		public bool HasNiceEquals
		{
			get
			{
				// TODO:  Add DynamicComponentType.HasNiceEquals getter implementation
				return false;
			}
		}

		public object Hydrate(System.Data.IDataReader rs, string[] names, ISessionImplementor session, object owner)
		{
			// TODO:  Add DynamicComponentType.Hydrate implementation
			return null;
		}

		public object ResolveIdentifier(object value, ISessionImplementor session, object owner)
		{
			// TODO:  Add DynamicComponentType.ResolveIdentifier implementation
			return null;
		}

		#endregion
	}
	*/
}
