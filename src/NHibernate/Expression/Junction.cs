using System;
using System.Text;
using System.Collections;

using NHibernate.Engine;
using NHibernate.Util;

namespace NHibernate.Expression {

	/// <summary>
	/// Summary description for Junction.
	/// </summary>
	public abstract class Junction : Expression	{
		
		private IList expressions = new ArrayList();

		public Junction Add(Expression expression) {
			expressions.Add(expression);
			return this;
		}

		protected abstract String Op { get; }

		public override TypedValue[] GetTypedValues( ISessionFactoryImplementor sessionFactory, System.Type persistentClass ) {

			ArrayList typedValues = new ArrayList();

			foreach ( IEnumerator iter in typedValues ) {

				TypedValue[] subvalues = ( (Expression) iter ).GetTypedValues(sessionFactory, persistentClass);
				for ( int i=0; i<subvalues.Length; i++ ) {
					typedValues.Add( subvalues[i] );
				}
			}
		
			return (TypedValue[]) typedValues.ToArray( typeof (TypedValue[]) );

		}

		public override string ToSqlString( ISessionFactoryImplementor sessionFactory, System.Type persistentClass, string alias) {
			if ( expressions.Count==0 ) return "1=1";
			
			StringBuilder buffer = new StringBuilder()
				.Append('(');

			for( int i=0; i<expressions.Count-1 ; i++ ) {

				buffer.Append( ( (Expression) expressions[i] ).ToSqlString(sessionFactory, persistentClass, alias) );
				buffer.Append( Op );
			}
			buffer.Append( ( (Expression) expressions[expressions.Count] ).ToSqlString(sessionFactory, persistentClass, alias) );

			return buffer.Append(')').ToString();
		}

		public override string ToString() {
			return '(' + String.Join( Op, (string[]) expressions ) + ')';
		}
	}
}