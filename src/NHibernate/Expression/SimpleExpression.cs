using System;

using NHibernate.Type;
using NHibernate.Engine;
using NHibernate.Util;

namespace NHibernate.Expression {

	/// <summary>
	/// SimpleExpression
	/// </summary>
	public abstract class SimpleExpression : Expression {

		private readonly string propertyName;

		private readonly object value;

		internal SimpleExpression(string propertyName, object value) {
			this.propertyName = propertyName;
			this.value = value;
		}

		public override string ToSqlString(ISessionFactoryImplementor sessionFactory, System.Type persistentClass, string alias) {																	return StringHelper.Join(
				" and ", 
				StringHelper.Suffix( GetColumns(sessionFactory, persistentClass, propertyName, alias), Op + "?" )
			);
			//TODO: get SQL rendering out of this package!
		}

		public override TypedValue[] GetTypedValues(ISessionFactoryImplementor sessionFactory, System.Type persistentClass) {
			return new TypedValue[] { GetTypedValue(sessionFactory, persistentClass, propertyName, value) };
		}

		public override string ToString() {
			return propertyName + Op + value;
		}

		protected abstract string Op { get; } //protected ???
	}
}