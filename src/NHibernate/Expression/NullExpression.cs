using System;

using NHibernate.Engine;
using NHibernate.Util;

namespace NHibernate.Expression {

	/// <summary>
	/// NullExpression
	/// </summary>
	public class NullExpression : Expression {

		private readonly string propertyName;
	
		private static readonly TypedValue[] NoValues = new TypedValue[0];
	
		internal NullExpression(string propertyName) {
			this.propertyName = propertyName;
		}

		public override string ToSqlString(ISessionFactoryImplementor sessionFactory, System.Type persistentClass, string alias) {
			return string.Join(
				" and ", 
				StringHelper.Suffix( GetColumns(sessionFactory, persistentClass, propertyName, alias), " is null" )
				);
		
			// TODO: get SQL rendering out of this package!
		}
	
		public override TypedValue[] GetTypedValues(ISessionFactoryImplementor sessionFactory, System.Type persistentClass) {
			return NoValues;
		}

		public override string ToString() {
			return propertyName + " is null";
		}
	}
}