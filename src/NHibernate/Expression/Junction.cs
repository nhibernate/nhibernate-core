using System;
using System.Collections;
using NHibernate.Engine;
using NHibernate.SqlCommand;

namespace NHibernate.Expression
{
	/// <summary>
	/// A sequence of logical expressions combined by some associative
	/// logical operator.
	/// </summary>
	public abstract class Junction : Expression
	{
		private IList expressions = new ArrayList();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public Junction Add( Expression expression )
		{
			expressions.Add( expression );
			return this;
		}

		/// <summary></summary>
		protected abstract String Op { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sessionFactory"></param>
		/// <param name="persistentClass"></param>
		/// <returns></returns>
		public override TypedValue[ ] GetTypedValues( ISessionFactoryImplementor sessionFactory, System.Type persistentClass )
		{
			ArrayList typedValues = new ArrayList();

			foreach( Expression expression in expressions )
			{
				TypedValue[ ] subvalues = expression.GetTypedValues( sessionFactory, persistentClass );
				for( int i = 0; i < subvalues.Length; i++ )
				{
					typedValues.Add( subvalues[ i ] );
				}
			}

			return ( TypedValue[ ] ) typedValues.ToArray( typeof( TypedValue ) );

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="persistentClass"></param>
		/// <param name="alias"></param>
		/// <returns></returns>
		public override SqlString ToSqlString( ISessionFactoryImplementor factory, System.Type persistentClass, string alias )
		{
			//TODO: add default capacity
			SqlStringBuilder sqlBuilder = new SqlStringBuilder();

			if( expressions.Count == 0 )
			{
				return new SqlString( new object[ ] {"1=1"} );
			}


			sqlBuilder.Add( "(" );

			for( int i = 0; i < expressions.Count - 1; i++ )
			{
				sqlBuilder.Add(
					( ( Expression ) expressions[ i ] ).ToSqlString( factory, persistentClass, alias ) );
				sqlBuilder.Add( Op );
			}

			sqlBuilder.Add(
				( ( Expression ) expressions[ expressions.Count - 1 ] ).ToSqlString( factory, persistentClass, alias ) );


			sqlBuilder.Add( ")" );

			return sqlBuilder.ToSqlString();
		}

		/// <summary></summary>
		public override string ToString()
		{
			return '(' + String.Join( Op, ( string[ ] ) expressions ) + ')';
		}
	}
}