using System;
using Antlr.Runtime;
using NHibernate.Engine;
using NHibernate.Hql.Ast.ANTLR.Util;
using NHibernate.Param;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	/// <summary>
	/// Implementation of ParameterNode.
	/// Author: Steve Ebersole
	/// Ported by: Steve Strong
	/// </summary>
	[CLSCompliant(false)]
	public class ParameterNode : HqlSqlWalkerNode, IDisplayableNode, IExpectedTypeAwareNode, ISelectExpression
	{
		private string _alias;
		private IParameterSpecification _parameterSpecification;
		private int _scalarColumnIndex = -1;

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

		public override SqlString RenderText(ISessionFactoryImplementor sessionFactory)
		{
			int count;
			if (ExpectedType != null && (count = ExpectedType.GetColumnSpan(sessionFactory)) > 1)
			{
				SqlStringBuilder buffer = new SqlStringBuilder();
				buffer.Add("(");
				buffer.AddParameter();
				for (int i = 1; i < count; i++)
				{
					buffer.Add(",");
					buffer.AddParameter();
				}
				buffer.Add(")");
				return buffer.ToSqlString();
			}
			else
			{
				return new SqlString(Parameter.Placeholder);
			}
		}

		#region ISelectExpression

		public void SetScalarColumnText(int i)
		{
			ColumnHelper.GenerateSingleScalarColumn(ASTFactory, this, i);
		}

		public FromElement FromElement
		{
			get { return null; }
		}

		public bool IsConstructor
		{
			get { return false; }
		}

		public bool IsReturnableEntity
		{
			get { return false; }
		}

		public bool IsScalar
		{
			get { return DataType != null && !DataType.IsAssociationType; }
		}

		public string Alias
		{
			get { return _alias; }
			set { _alias = value; }
		}

		public void SetScalarColumn(int i)
		{
			_scalarColumnIndex = i;
			SetScalarColumnText(i);
		}

		public int ScalarColumnIndex
		{
			get { return _scalarColumnIndex; }
		}

		#endregion
	}
}
