using System.Collections;
using NHibernate.Engine;
using NHibernate.Persister;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Expression
{
	/// <summary>
	/// Summary description for AbstractCriterion.
	/// </summary>
	public abstract class AbstractCriterion : ICriterion
	{
		/// <summary>
		/// 
		/// </summary>
		protected AbstractCriterion()
		{
		}

		private static IQueryable GetPropertyMapping( System.Type persistentClass, ISessionFactoryImplementor sessionFactory )
		{
			return (IQueryable) sessionFactory.GetPersister( persistentClass );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="persistentClass"></param>
		/// <param name="property"></param>
		/// <param name="alias"></param>
		/// <param name="aliasClasses"></param>
		/// <returns></returns>
		protected static string[] GetColumns( ISessionFactoryImplementor factory, System.Type persistentClass, string property, string alias, IDictionary aliasClasses )
		{
			if ( property.IndexOf( '.' ) > 0 )
			{
				string root = StringHelper.Root( property );
				System.Type clazz = (System.Type) aliasClasses[ root ];
				if ( clazz != null )
				{
					persistentClass = clazz;
					alias = root;
					property = property.Substring( root.Length + 1 );
				}
			}

			return GetPropertyMapping( persistentClass, factory ).ToColumns( alias, property );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="persistentClass"></param>
		/// <param name="property"></param>
		/// <param name="aliasClasses"></param>
		/// <returns></returns>
		protected static IType GetType( ISessionFactoryImplementor factory, System.Type persistentClass, string property, IDictionary aliasClasses )
		{
			if ( property.IndexOf( '.' ) > 0 )
			{
				string root = StringHelper.Root( property );
				System.Type clazz = (System.Type) aliasClasses[ root ];
				if ( clazz != null )
				{
					persistentClass = clazz;
					property = property.Substring( root.Length + 1 );
				}
			}

			return GetPropertyMapping( persistentClass, factory ).ToType( property );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="persistentClass"></param>
		/// <param name="property"></param>
		/// <param name="value"></param>
		/// <param name="aliasClasses"></param>
		/// <returns></returns>
		protected static TypedValue GetTypedValue( ISessionFactoryImplementor factory, System.Type persistentClass, string property, object value, IDictionary aliasClasses )
		{
			return new TypedValue( GetType( factory, persistentClass, property, aliasClasses), value );
		}

		#region ICriterion Members

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sessionFactory"></param>
		/// <param name="persistentClass"></param>
		/// <param name="alias"></param>
		/// <param name="aliasClasses"></param>
		/// <returns></returns>
		public abstract SqlString ToSqlString(ISessionFactoryImplementor sessionFactory, System.Type persistentClass, string alias, IDictionary aliasClasses);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sessionFactory"></param>
		/// <param name="persistentClass"></param>
		/// <param name="aliasClasses"></param>
		/// <returns></returns>
		public abstract TypedValue[] GetTypedValues(ISessionFactoryImplementor sessionFactory, System.Type persistentClass, IDictionary aliasClasses);

		#endregion
	}
}
