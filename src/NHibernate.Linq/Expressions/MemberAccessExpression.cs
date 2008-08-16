using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace NHibernate.Linq.Expressions
{
	/// <summary>
	/// A member access expression is produced where a member of an object is accessed(can be field or Property)
	/// </summary>
	public class MemberAccessExpression:SqlExpression
	{
		public MemberAccessExpression(System.Type type, SqlExpression expression, string name)
			: base(SqlExpressionType.MemberAccess, type)
		{
			this.Name = name;
			this.Expression = expression;
		}

		public string Name { get; protected set; }
		public SqlExpression Expression { get; protected set; }
		public override string ToString()
		{
			return string.Format("({0}.{1})", this.Expression, ".", this.Name);
		}
	}
}
