using System;

using NHibernate.Engine;
using NHibernate.Util;

namespace NHibernate.Expression {

	/// <summary>
	/// InExpression
	/// </summary>
	public class InExpression : Expression {
		private readonly string propertyName;
		private readonly object[] values;
	
		internal InExpression(string propertyName, object[] values) {
			this.propertyName = propertyName;
			this.values = values;
		}
		
		public override string ToSqlString(ISessionFactoryImplementor sessionFactory, System.Type persistentClass, string alias) {
			string pars = StringHelper.Repeat( "?, ", values.Length-1 );
			if ( values.Length>0 ) pars+="?";
			return GetColumns(sessionFactory, persistentClass, propertyName, alias) + " in (" + pars + ')';
		
			//TODO: get SQL rendering out of this package!
		}
	
		public override TypedValue[] GetTypedValues(ISessionFactoryImplementor sessionFactory, System.Type persistentClass) {
			TypedValue[] tvs = new TypedValue[ values.Length ];
			for ( int i=0; i<tvs.Length; i++ ) {
				tvs[i] = GetTypedValue( sessionFactory, persistentClass, propertyName, values[i] );
			}
			return tvs;
		}

		public override string ToString() {
			return propertyName + " in (" + StringHelper.ToString(values) + ')';
		}
		
	}
}