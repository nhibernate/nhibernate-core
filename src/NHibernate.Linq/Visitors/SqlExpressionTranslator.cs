using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NHibernate.Engine;
using NHibernate.Linq.Expressions;
using NHibernate.SqlCommand;
using NHibernate.Persister.Entity;
using NHibernate.Type;
using NHibernate.Metadata;

namespace NHibernate.Linq.Visitors
{
	//TODO: Process the query source by converting into select expression
	//TODO: Select expression should be changed so that it can handle columns or projections instead of expression.
	public class SqlExpressionToSqlStringVisitor:NHibernateExpressionVisitor
	{
		public static SqlString Translate(Expression expr,ISessionFactoryImplementor sessionFactory,IList<object> parameterList)
		{

			var visitor = new SqlExpressionToSqlStringVisitor(sessionFactory,parameterList);
			visitor.Visit(expr);
			return visitor.sqlStringBuilder.ToSqlString();
		}

		public SqlExpressionToSqlStringVisitor(ISessionFactoryImplementor sessionFactory,IList<object> parameterList)
		{
			this.sqlStringBuilder = new SqlStringBuilder();
			this.sessionFactory = sessionFactory;
			this.parameterList = parameterList;

		}

		private readonly IList<object> parameterList;
		private readonly ISessionFactoryImplementor sessionFactory;
		private SqlStringBuilder sqlStringBuilder;

		protected override Expression VisitUnary(UnaryExpression u)
		{
			switch(u.NodeType)
			{
				case ExpressionType.Not:
					{
						sqlStringBuilder.Add(" NOT (");
						Expression ret=base.VisitUnary(u);
						sqlStringBuilder.Add(")");
						return ret;
					}
				case ExpressionType.Negate:
					{
						sqlStringBuilder.Add("(-1 * (");
						Expression ret = base.VisitUnary(u);
						sqlStringBuilder.Add("))");
						return ret;
					}
			}
			return base.VisitUnary(u);
		}
		protected override Expression VisitBinary(BinaryExpression b)
		{
			string op = "";

			#region operators

			switch (b.NodeType)
			{
				case ExpressionType.Add:
					op = "+";
					break;
				case ExpressionType.And:
				case ExpressionType.AndAlso:
					op = "AND";
					break;
				case ExpressionType.Divide:
					op = "/";
					break;
				case ExpressionType.GreaterThan:
					op = ">";
					break;
				case ExpressionType.GreaterThanOrEqual:
					op = ">=";
					break;
				case ExpressionType.LessThan:
					op = "<";
					break;
				case ExpressionType.LessThanOrEqual:
					op = "<=";
					break;
				case ExpressionType.Modulo:
					op = "MOD";
					break;
				case ExpressionType.Multiply:
					op = "*";
					break;
				case ExpressionType.Or:
				case ExpressionType.OrElse:
					op = "OR";
					break;
				case ExpressionType.Subtract:
					op = "-";
					break;
				case ExpressionType.Equal:
					op = "=";
					break;
				case ExpressionType.NotEqual:
					op = "!=";
					break;
				case ExpressionType.Power:
				default:
					throw new NotImplementedException();
			}

			#endregion

			sqlStringBuilder.Add("(");
			this.Visit(b.Left);
			sqlStringBuilder.Add(" ");
			sqlStringBuilder.Add(op);
			sqlStringBuilder.Add(" ");
			this.Visit(b.Right);
			sqlStringBuilder.Add(")");
			return b;
		}

		protected override Expression VisitConstant(ConstantExpression c)
		{
			sqlStringBuilder.AddParameter();
			parameterList.Add(c.Value);
			return c;
		}

		protected override Expression VisitProperty(PropertyExpression property)
		{
			ParameterExpression expr = property.Expression as ParameterExpression;

			sqlStringBuilder.Add(string.Format("{0}.{1}", expr.Name, property.Name));
			return property;
		}

		protected override Expression VisitSelect(SelectExpression select)
		{
			SqlSelectBuilder selectBuilder=new SqlSelectBuilder(this.sessionFactory);
			SqlString selectString = new SqlString("*");
			SqlString fromString = SqlExpressionToSqlStringVisitor.Translate(select.From, sessionFactory, parameterList);
			fromString=new SqlString("(",fromString,")");
			SqlString whereString = SqlExpressionToSqlStringVisitor.Translate(select.Where, sessionFactory, parameterList);
			SqlString outerJoinsAfterFrom=new SqlString();
			SqlString outerJoinsAfterWhere=new SqlString();
			selectBuilder.SetFromClause(fromString);
			selectBuilder.SetWhereClause(whereString);
			selectBuilder.SetSelectClause(selectString);
			selectBuilder.SetOuterJoins(outerJoinsAfterFrom, outerJoinsAfterWhere);
			this.sqlStringBuilder.Add(selectBuilder.ToSqlString());
			return select;
		}

		protected override Expression VisitQuerySource(QuerySourceExpression expr)
		{

			sqlStringBuilder.Add("SELECT ");
			IClassMetadata metadata = sessionFactory.GetClassMetadata(expr.Query.ElementType);
			IPropertyMapping mapping = (IPropertyMapping)sessionFactory.GetEntityPersister(metadata.EntityName);
			string[] names = metadata.PropertyNames;






			bool started = false;
			for (int i = 0; i < names.Length;i++ )
			{
				string name = names[i];
				IType propertyType = metadata.GetPropertyType(name);
				if (!(propertyType.IsComponentType |
					propertyType.IsCollectionType |
					propertyType.IsAssociationType |
					propertyType.IsAnyType))
				{
					if(started)
						sqlStringBuilder.Add(", ");
					started = true;
					sqlStringBuilder.Add(mapping.ToColumns(name)[0]);
					sqlStringBuilder.Add(" AS ");
					sqlStringBuilder.Add(name);
				}

			}
			sqlStringBuilder.Add(" FROM ");
			sqlStringBuilder.Add(expr.Query.ElementType.Name);
			return base.VisitQuerySource(expr);
		}
		public override string ToString()
		{
			return sqlStringBuilder.ToString();
		}
	}
}
