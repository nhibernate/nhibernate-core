using System;

using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Expression {

	/// <summary>
	/// SQLExpression
	/// </summary>
	public class SQLExpression : Expression {

		private string sql;
		private TypedValue[] typedValues;

		internal SQLExpression(string sql, object[] values, IType[] types) {
			typedValues = new TypedValue[values.Length];
			for ( int i=0; i<typedValues.Length; i++ ) {
				typedValues[i] = new TypedValue( types[i], values[i] );
			}
		}

		public override string ToSqlString(ISessionFactoryImplementor sessionFactory, System.Type persistentClass, string alias) {
			return StringHelper.Replace(sql, "$alias", alias);
		}

		public override TypedValue[] GetTypedValues(ISessionFactoryImplementor sessionFactory, System.Type persistentClass) {
			return typedValues;
		}

		public override string ToString() {
			return sql;
		}
	}
}

