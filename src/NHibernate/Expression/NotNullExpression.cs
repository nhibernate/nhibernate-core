using System;
using System.Text;

using NHibernate.Engine;
using NHibernate.Persister;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Expression {

	/// <summary>
	/// NotNullExpression
	/// </summary>
	public class NotNullExpression : Expression {

		private readonly string propertyName;
	
		private static readonly TypedValue[] NoValues = new TypedValue[0];
	
		internal NotNullExpression(string propertyName) {
			this.propertyName = propertyName;
		}


		public override SqlString ToSqlString(ISessionFactoryImplementor factory, System.Type persistentClass, string alias) {
			//TODO: add default capacity
			SqlStringBuilder sqlBuilder = new SqlStringBuilder();

			IType propertyType = ((IQueryable)factory.GetPersister(persistentClass)).GetPropertyType(propertyName);
			string[] columnNames = GetColumns(factory, persistentClass, propertyName, alias);
			
			
			bool andNeeded = false;
			
			for(int i = 0; i < columnNames.Length; i++){
				if(andNeeded) sqlBuilder.Add(" AND ");
				andNeeded = true;

				sqlBuilder.Add(columnNames[i])
					.Add(" IS NOT NULL");

			}

			return sqlBuilder.ToSqlString();
		}
	
		public override TypedValue[] GetTypedValues(ISessionFactoryImplementor sessionFactory, System.Type persistentClass) {
			return NoValues;
		}

		public override string ToString() {
			return propertyName + " is not null";
		}
	}
}