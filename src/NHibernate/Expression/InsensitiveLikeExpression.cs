using System;
using NHibernate.Engine;
using NHibernate.Dialect;

namespace NHibernate.Expression
{
	/// <summary>
	/// Summary description for InsensitiveLikeExpression.
	/// </summary>
	public class InsensitiveLikeExpression: Expression {

		private readonly string _propertyName;
		private readonly object _value;

		internal InsensitiveLikeExpression(string propertyName, object value) {		
			this._propertyName = propertyName;
			this._value = value;
		}

		public override string ToSqlString(ISessionFactoryImplementor sessionFactory, System.Type persistentClass, string alias) {
			Dialect.Dialect dialect = sessionFactory.Dialect;

			string[] columns = Expression.GetColumns(sessionFactory, persistentClass, _propertyName, alias);

			if (columns.Length!=1) throw new HibernateException("InsentiveLike may only be used with single-column properties");

			if ( dialect is PostgreSQLDialect ) {
				return columns[0] + " ilike ?";
			}
			else {
				return dialect.LowercaseFunction + '(' + columns[0] + ") like ?";
			}

			//TODO: get SQL rendering out of this package!

		}

		public override TypedValue[] GetTypedValues(ISessionFactoryImplementor sessionFactory, System.Type persistentClass) {
			return new TypedValue[] { Expression.GetTypedValue( sessionFactory, persistentClass, _propertyName, _value.ToString().ToLower() ) };
		}


		public override string ToString() {
			return _propertyName + " ilike " + _value;
		}
	}
}