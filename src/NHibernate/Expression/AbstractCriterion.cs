using System;
using System.Collections;

using NHibernate.Engine;
using NHibernate.Persister;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Expression
{
	/// <summary>
	/// Base class for <see cref="ICriterion"/> implementations.
	/// </summary>
	public abstract class AbstractCriterion : ICriterion
	{
		/// <summary>
		/// Gets a string representation of the <see cref="AbstractCriterion"/>.  
		/// </summary>
		/// <returns>
		/// A String that shows the contents of the <see cref="AbstractCriterion"/>.
		/// </returns>
		/// <remarks>
		/// This is not a well formed Sql fragment.  It is useful for logging what the <see cref="AbstractCriterion"/>
		/// looks like.
		/// </remarks>
		public abstract override string ToString();

		private static IQueryable GetPropertyMapping( System.Type persistentClass, ISessionFactoryImplementor sessionFactory )
		{
			return ( IQueryable ) sessionFactory.GetPersister( persistentClass );
		}

		protected internal static string[ ] GetColumns( ISessionFactoryImplementor factory, System.Type persistentClass, string property, string alias, IDictionary aliasClasses )
		{
			if( property.IndexOf( '.' ) > 0 )
			{
				string root = StringHelper.Root( property );
				System.Type clazz = aliasClasses[ root ] as System.Type;
				if( clazz != null )
				{
					persistentClass = clazz;
					alias = root;
					property = property.Substring( root.Length + 1 );
				}
			}

			return GetPropertyMapping( persistentClass, factory ).ToColumns( alias, property );
		}

		/// <summary>
		/// Get the a typed value for the given property value.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="persistentClass"></param>
		/// <param name="property"></param>
		/// <param name="aliasClasses"></param>
		/// <returns></returns>
		protected static IType GetType(
			ISessionFactoryImplementor factory,
			System.Type persistentClass,
			string property,
			IDictionary aliasClasses )
		{
			if( property.IndexOf( '.' ) > 0 )
			{
				string root = StringHelper.Root( property );
				System.Type clazz = ( System.Type ) aliasClasses[ root ];
				if( clazz != null )
				{
					persistentClass = clazz;
					property = property.Substring( root.Length + 1 );
				}
			}

			return GetPropertyMapping( persistentClass, factory ).ToType( property );
		}

		/// <summary>
		/// Get the <see cref="TypedValue"/> for the given property value.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="persistentClass"></param>
		/// <param name="property"></param>
		/// <param name="value"></param>
		/// <param name="aliasClasses"></param>
		/// <returns></returns>
		protected static TypedValue GetTypedValue(
			ISessionFactoryImplementor factory,
			System.Type persistentClass,
			string property,
			object value,
			IDictionary aliasClasses )
		{
			return new TypedValue( GetType( factory, persistentClass, property, aliasClasses ), value );
		}

		#region ICriterion Members

		/// <summary>
		/// Render a SqlString for the expression.
		/// </summary>
		/// <param name="factory">The ISessionFactory that contains the mapping for the Type.</param>
		/// <param name="persistentClass">The Class the Expression is being built for.</param>
		/// <param name="alias">The alias to use for the table.</param>
		/// <param name="aliasClasses"></param>
		/// <returns>A SqlString that contains a valid Sql fragment.</returns>
		public abstract SqlString ToSqlString( ISessionFactoryImplementor factory, System.Type persistentClass, string alias, IDictionary aliasClasses );

		/// <summary>
		/// Return typed values for all parameters in the rendered SQL fragment
		/// </summary>
		/// <param name="sessionFactory">The ISessionFactory that contains the mapping for the Type.</param>
		/// <param name="persistentClass">The Class the Expression is being built for.</param>
		/// <param name="aliasClasses"></param>
		/// <returns>An array of TypedValues for the Expression.</returns>
		public abstract TypedValue[ ] GetTypedValues( ISessionFactoryImplementor sessionFactory, System.Type persistentClass, IDictionary aliasClasses );

		#endregion
	}
}