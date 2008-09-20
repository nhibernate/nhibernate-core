using System.Linq.Expressions;
using NHibernate.Type;

namespace NHibernate.Linq.Expressions
{
	public abstract class PropertyExpression : NHExpression
	{

		protected PropertyExpression(string name,NHExpressionType nodeType, System.Type type, Expression source, IType nhibernateType)
			: base(nodeType, type)
		{
			NHibernateType = nhibernateType;
			Source = source;
			Name = name;
		}

		public Expression Source { get; protected set; }
		public IType NHibernateType { get; protected set; }
		public string Name { get; protected set; }

		public override string ToString()
		{
			return string.Format("({0}).Property({1})", Source, this.Name);
		}
	}
}