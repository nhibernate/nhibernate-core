using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NHibernate.Metadata;

namespace NHibernate.Linq.Expressions
{
	/// <summary>
	/// An entity is where we want to execute our query on, in RDBMS case it is a Table.
	/// May also be used in from clause of a select expression
	/// </summary>
	public class EntityExpression:SqlExpression
	{
		public EntityExpression(System.Type type, string alias)
			: base(SqlExpressionType.Entity, type)
		{
			this.Alias = alias;
		}

		public string Alias { get; protected set; }

		public override string ToString()
		{
			return string.Format("(({0}) as {1}", Type.ToString(), this.Alias);
		}
	}
}
