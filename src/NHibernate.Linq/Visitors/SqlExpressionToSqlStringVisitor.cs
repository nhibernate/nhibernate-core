using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NHibernate.Engine;
using NHibernate.Linq.Expressions;
using NHibernate.SqlCommand;
using SelectFragment=NHibernate.SqlCommand.SelectFragment;
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
			return visitor.selectBuilder.ToSqlString();
		}

		public SqlExpressionToSqlStringVisitor(ISessionFactoryImplementor sessionFactory,IList<object> parameterList)
		{
			this.selectBuilder = new SqlStringBuilder();
			this.sessionFactory = sessionFactory;
			this.parameterList = parameterList;
		}

		private readonly IList<object> parameterList;
		private ISessionFactoryImplementor sessionFactory;
		private SqlStringBuilder selectBuilder;
		protected override Expression VisitUnary(UnaryExpression u)
		{
			switch(u.NodeType)
			{
				case ExpressionType.Not:
					{
						selectBuilder.Add(" NOT (");
						Expression ret=base.VisitUnary(u);
						selectBuilder.Add(")");
						return ret;
					}
				case ExpressionType.Negate:
					{
						selectBuilder.Add("(-1 * (");
						Expression ret = base.VisitUnary(u);
						selectBuilder.Add("))");
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

			selectBuilder.Add("(");
			this.Visit(b.Left);
			selectBuilder.Add(" ");
			selectBuilder.Add(op);
			selectBuilder.Add(" ");
			this.Visit(b.Right);
			selectBuilder.Add(")");
			return b;
		}
		protected override Expression VisitConditional(ConditionalExpression c)
		{
			return base.VisitConditional(c);
		}

		protected override Expression VisitConstant(ConstantExpression c)
		{
			selectBuilder.AddParameter();
			parameterList.Add(c.Value);
			return c;
		}

		protected override Expression VisitProperty(PropertyExpression property)
		{
			ParameterExpression expr = property.Expression as ParameterExpression;

			selectBuilder.Add(string.Format("{0}.{1}", expr.Name, property.Name));
			return property;
		}

		protected override Expression VisitSelect(SelectExpression select)
		{
			selectBuilder.Add("(SELECT ");
			if(select.Projection!=null)
				this.Visit(select.Projection);
			else
				selectBuilder.Add(select.FromAlias+".* ");

			if (select.From != null)
			{
				selectBuilder.Add(" FROM (");
				this.Visit(select.From);
				selectBuilder.Add(") AS ");
				selectBuilder.Add(select.FromAlias);
			}
			else 
				throw new NotImplementedException();
			if(select.Where!=null)
			{
				selectBuilder.Add(" WHERE ");
				this.Visit(select.Where);
			}
			selectBuilder.Add(")");
			return select;
		}

		protected override Expression VisitQuerySource(QuerySourceExpression expr)
		{
			selectBuilder.Add("SELECT ");
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
						selectBuilder.Add(", ");
					started = true;
					selectBuilder.Add(mapping.ToColumns(name)[0]);
				}

			}
			selectBuilder.Add(" FROM ");
			selectBuilder.Add(expr.Query.ElementType.Name);
			return base.VisitQuerySource(expr);
		}
		protected override Expression VisitParameter(ParameterExpression p)
		{
			return base.VisitParameter(p);
		}
		public override string ToString()
		{
			return selectBuilder.ToString();
		}
	}
}
