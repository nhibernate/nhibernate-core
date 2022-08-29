using System;
using System.Collections.Generic;
using Antlr.Runtime;
using NHibernate.Dialect.Function;
using NHibernate.Hql.Ast.ANTLR.Util;
using NHibernate.Type;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	/// <summary>
	/// Represents an aggregate function i.e. min, max, sum, avg.
	/// 
	/// Author: Joshua Davis
	/// Ported by: Steve Strong
	/// </summary>
	[CLSCompliant(false)]
	public class AggregateNode : AbstractSelectExpression, ISelectExpression
	{
		public AggregateNode(IToken token)
			: base(token)
		{
		}

		public string FunctionName
		{
			get
			{
				if (SessionFactoryHelper.FindSQLFunction(Text) is ISQLFunctionExtended sqlFunction)
				{
					return sqlFunction.Name;
				}

				return Text;
			}
		}

		public override IType DataType
		{
			get
			{
				// Get the function return value type, based on the type of the first argument.
				return SessionFactoryHelper.FindFunctionReturnType(Text, (IEnumerable<IASTNode>) this);
			}
			set
			{
				base.DataType = value;
			}
		}

		public override void SetScalarColumnText(int i)
		{
			ColumnHelper.GenerateSingleScalarColumn(ASTFactory, this, i);
		}
	}
}
