using System;

using NHibernate.Engine;
using NHibernate.Util;

namespace NHibernate.Expression {

	/// <summary>
	/// BetweenExpression
	/// </summary>
	public class BetweenExpression : Expression {

		private readonly string propertyName;
		private readonly object lo;
		private readonly object hi;
	
		internal BetweenExpression(string propertyName, object lo, object hi) {
			this.propertyName = propertyName;
			this.lo = lo;
			this.hi = hi;
		}

		
		public override string ToSqlString(ISessionFactoryImplementor sessionFactory, System.Type persistentClass, string alias) {
			return string.Join(
				" and ", 
				StringHelper.Suffix( GetColumns(sessionFactory, persistentClass, propertyName, alias), " between ? and ?" )
				);
		
			//TODO: get SQL rendering out of this package!
		}
	
		public override TypedValue[] GetTypedValues(ISessionFactoryImplementor sessionFactory, System.Type persistentClass) {
			return new TypedValue[] { 
				GetTypedValue(sessionFactory, persistentClass, propertyName, lo),
				GetTypedValue(sessionFactory, persistentClass, propertyName, hi) 
				};
		}

		public override string ToString() {
			return propertyName + " between " + lo + " and " + hi;
		}	
	}
}
