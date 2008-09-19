using System.Linq.Expressions;
using NHibernate.Type;

namespace NHibernate.Linq.Expressions
{
	public class PropertyExpression : NHExpression
	{
		public PropertyExpression(string name,
		                          System.Type type, Expression expression, IType nhibernateType)
			: this(name, type, expression, nhibernateType, NHExpressionType.Property)
		{
		}


		protected PropertyExpression(string name, System.Type type, Expression expression, IType nhibernateType,
		                             NHExpressionType nodeType)
			: base(nodeType, type)
		{
			NHibernateType = nhibernateType;
			Expression = expression;
			Name = name;
		}

		public Expression Expression { get; protected set; }
		public IType NHibernateType { get; protected set; }
		public string Name { get; protected set; }

		public override string ToString()
		{
			return Expression + "." + Name;
		}
	}
}