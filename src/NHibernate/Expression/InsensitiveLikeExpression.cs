using System;
using System.Text;

using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Persister;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Expression
{
	/// <summary>
	/// Summary description for InsensitiveLikeExpression.
	/// 
	/// </summary>
	//TODO:H2.0.3 renamed this to ILikeExpression
	public class InsensitiveLikeExpression: Expression 
	{

		private readonly string propertyName;
		private readonly object expressionValue;

		internal InsensitiveLikeExpression(string propertyName, object expressionValue) 
		{		
			this.propertyName = propertyName;
			this.expressionValue = expressionValue;
		}

		public override SqlString ToSqlString(ISessionFactoryImplementor factory, System.Type persistentClass, string alias) 
		{
			//TODO: add default capacity
			SqlStringBuilder sqlBuilder = new SqlStringBuilder();

			IType propertyType = ((IQueryable)factory.GetPersister(persistentClass)).GetPropertyType(propertyName);
			string[] columnNames = GetColumns(factory, persistentClass, propertyName, alias);
			string[] paramColumnNames = GetColumns(factory, persistentClass, propertyName , null);
			Parameter[] parameters = Parameter.GenerateParameters(factory, alias, paramColumnNames, propertyType);

			
			if(factory.Dialect is PostgreSQLDialect) 
			{
				sqlBuilder.Add(columnNames[0]);
				sqlBuilder.Add(" ilike ");
			}
			else 
			{
				sqlBuilder.Add(factory.Dialect.LowercaseFunction)
					.Add("(")
					.Add(columnNames[0])
					.Add(")")
					.Add(" like ");
			}

			sqlBuilder.Add(parameters[0]);

			return sqlBuilder.ToSqlString();
		}


		public override TypedValue[] GetTypedValues(ISessionFactoryImplementor sessionFactory, System.Type persistentClass) 
		{
			return new TypedValue[] { Expression.GetTypedValue( sessionFactory, persistentClass, propertyName, expressionValue.ToString().ToLower() ) };
		}


		public override string ToString() 
		{
			return propertyName + " ilike " + expressionValue;
		}
	}
}