using System;

using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Expression 
{
	/// <summary>
	/// Creates a SQLExpression
	/// </summary>
	/// <remarks>
	/// This allows for database specific Expressions at the cost of needing to 
	/// write a correct <see cref="SqlString"/>.
	/// </remarks>
	public class SQLExpression : Expression
	{

		private readonly SqlString sql;
		private readonly TypedValue[] typedValues;

		internal SQLExpression(SqlString sql, object[] values, IType[] types) 
		{
			this.sql = sql;
			typedValues = new TypedValue[values.Length];
			for ( int i=0; i<typedValues.Length; i++ ) 
			{
				typedValues[i] = new TypedValue( types[i], values[i] );
			}
		}


		public override SqlString ToSqlString(ISessionFactoryImplementor factory, System.Type persistentClass, string alias) 
		{
			return sql.Replace( "$alias", alias );
		}

		public override TypedValue[] GetTypedValues(ISessionFactoryImplementor sessionFactory, System.Type persistentClass) 
		{
			return typedValues;
		}

		public override string ToString() 
		{
			return sql.ToString();
		}
	}
}

