using System;
using System.Text;

using NHibernate.Engine;
using NHibernate.Persister;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Expression {

	/// <summary>
	/// InExpression - should only be used with a Single Value column - no multicolumn properties...
	/// </summary>
	public class InExpression : Expression {
		private readonly string propertyName;
		private readonly object[] values;
	
		internal InExpression(string propertyName, object[] values) {
			this.propertyName = propertyName;
			this.values = values;
		}
		

		public override SqlString ToSqlString(ISessionFactoryImplementor factory, System.Type persistentClass, string alias) {
			//TODO: add default capacity
			SqlStringBuilder sqlBuilder = new SqlStringBuilder();

			IType propertyType = ((IQueryable)factory.GetPersister(persistentClass)).GetPropertyType(propertyName);
			string[] columnNames = GetColumns(factory, persistentClass, propertyName, alias);
			string[] paramColumnNames = GetColumns(factory, persistentClass, propertyName , null);
			
			if (columnNames.Length!=1) throw new HibernateException("InExpression may only be used with single-column properties");

			// each value should have its own parameter
			Parameter[] parameters = new Parameter[values.Length];
			
			for(int i = 0; i < values.Length; i++) {
				// we can hardcode 0 because this will only be for a single column
				string paramInColumnNames = paramColumnNames[0] + StringHelper.Underscore + i;
				parameters[i] = Parameter.GenerateParameters(factory, alias, new string[]{paramInColumnNames}, propertyType)[0];
			}
			
			sqlBuilder.Add(columnNames[0])
				.Add(" in (");
	
			bool commaNeeded = false;
			foreach(Parameter parameter in parameters){
				if(commaNeeded) sqlBuilder.Add(StringHelper.CommaSpace);
				commaNeeded = true;

				sqlBuilder.Add(parameter);

			}

			sqlBuilder.Add(")");

			return sqlBuilder.ToSqlString();	
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