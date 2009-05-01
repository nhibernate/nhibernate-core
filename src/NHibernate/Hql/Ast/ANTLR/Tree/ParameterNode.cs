using System;
using System.Text;
using Antlr.Runtime;
using NHibernate.Engine;
using NHibernate.Param;
using NHibernate.Type;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	/// <summary>
	/// Implementation of ParameterNode.
	/// Author: Steve Ebersole
	/// Ported by: Steve Strong
	/// </summary>
	[CLSCompliant(false)]
	public class ParameterNode : HqlSqlWalkerNode, IDisplayableNode, IExpectedTypeAwareNode
	{
		private IParameterSpecification _parameterSpecification;

		public ParameterNode(IToken token) : base(token)
		{
		}

		public IParameterSpecification HqlParameterSpecification
		{
			get { return _parameterSpecification; }
			set { _parameterSpecification = value; }
		}

		public string GetDisplayText()
		{
			return "{" + (_parameterSpecification == null ? "???" : _parameterSpecification.RenderDisplayInfo()) + "}";
		}

		public IType ExpectedType
		{
			get
			{
				return HqlParameterSpecification == null ? null : HqlParameterSpecification.ExpectedType;
			}

			set
			{
				HqlParameterSpecification.ExpectedType = value;
				DataType = value;
			}
		}

		public override string RenderText(ISessionFactoryImplementor sessionFactory)
		{
			int count = 0;
			if (ExpectedType != null && (count = ExpectedType.GetColumnSpan(sessionFactory)) > 1)
			{
				StringBuilder buffer = new StringBuilder();
				buffer.Append("(?");
				for (int i = 1; i < count; i++)
				{
					buffer.Append(", ?");
				}
				buffer.Append(")");
				return buffer.ToString();
			}
			else
			{
				return "?";
			}
		}
	}
}
