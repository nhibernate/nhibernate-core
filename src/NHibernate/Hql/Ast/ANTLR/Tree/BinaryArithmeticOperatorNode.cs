using System;
using Antlr.Runtime;
using NHibernate.Hql.Ast.ANTLR.Util;
using NHibernate.Type;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	/// <summary>
	/// Nodes which represent binary arithmetic operators.
	/// </summary>
	[CLSCompliant(false)]
	public class BinaryArithmeticOperatorNode : AbstractSelectExpression, IBinaryOperatorNode, IDisplayableNode
	{
		public BinaryArithmeticOperatorNode(IToken token)
			: base(token)
		{
		}

		public void Initialize()
		{
			IASTNode lhs = LeftHandOperand;
			IASTNode rhs = RightHandOperand;
			if (lhs == null)
			{
				throw new SemanticException("left-hand operand of a binary operator was null");
			}
			if (rhs == null)
			{
				throw new SemanticException("right-hand operand of a binary operator was null");
			}

			IType lhType = (lhs is SqlNode) ? ((SqlNode)lhs).DataType : null;
			IType rhType = (rhs is SqlNode) ? ((SqlNode)rhs).DataType : null;

			if (lhs is IExpectedTypeAwareNode && rhType != null)
			{
				IType expectedType;

				// we have something like : "? [op] rhs"
				if (IsDateTimeType(rhType))
				{
					// more specifically : "? [op] datetime"
					//      1) if the operator is MINUS, the param needs to be of
					//          some datetime type
					//      2) if the operator is PLUS, the param needs to be of
					//          some numeric type
					expectedType = Type == HqlSqlWalker.PLUS ? NHibernateUtil.Double : rhType;
				}
				else
				{
					expectedType = rhType;
				}
				((IExpectedTypeAwareNode)lhs).ExpectedType = expectedType;
			}
			else if (rhs is ParameterNode && lhType != null)
			{
				IType expectedType = null;

				// we have something like : "lhs [op] ?"
				if (IsDateTimeType(lhType))
				{
					// more specifically : "datetime [op] ?"
					//      1) if the operator is MINUS, we really cannot determine
					//          the expected type as either another datetime or
					//          numeric would be valid
					//      2) if the operator is PLUS, the param needs to be of
					//          some numeric type
					if (Type == HqlSqlWalker.PLUS)
					{
						expectedType = NHibernateUtil.Double;
					}
				}
				else
				{
					expectedType = lhType;
				}
				((IExpectedTypeAwareNode)rhs).ExpectedType = expectedType;
			}
		}

		public override IType DataType
		{
			get
			{
				/*
				 * Figure out the type of the binary expression by looking at
				 * the types of the operands. Sometimes we don't know both types,
				 * if, for example, one is a parameter.
				 */
				if (base.DataType == null)
				{
					base.DataType = ResolveDataType();
				}
				return base.DataType;
			}
			set
			{
				base.DataType = value;
			}
		}


		private IType ResolveDataType()
		{
			// TODO : we may also want to check that the types here map to exactly one column/JDBC-type
			//      can't think of a situation where arithmetic expression between multi-column mappings
			//      makes any sense.
			IASTNode lhs = LeftHandOperand;
			IASTNode rhs = RightHandOperand;
			IType lhType = (lhs is SqlNode) ? ((SqlNode)lhs).DataType : null;
			IType rhType = (rhs is SqlNode) ? ((SqlNode)rhs).DataType : null;

			if (IsDateTimeType(lhType) || IsDateTimeType(rhType))
			{
				return ResolveDateTimeArithmeticResultType(lhType, rhType);
			}
			else
			{
				if (lhType == null)
				{
					if (rhType == null)
					{
						// we do not know either type
						return NHibernateUtil.Double; //BLIND GUESS!
					}
					else
					{
						// we know only the rhs-hand type, so use that
						return rhType;
					}
				}
				else
				{
					if (rhType == null)
					{
						// we know only the lhs-hand type, so use that
						return lhType;
					}
					else
					{
						if (lhType == NHibernateUtil.Double || rhType == NHibernateUtil.Double) return NHibernateUtil.Double;
						if (lhType == NHibernateUtil.Decimal || rhType == NHibernateUtil.Decimal) return NHibernateUtil.Decimal;
						if (lhType == NHibernateUtil.Int64 || rhType == NHibernateUtil.Int64) return NHibernateUtil.Int64;
						if (lhType == NHibernateUtil.Int32 || rhType == NHibernateUtil.Int32) return NHibernateUtil.Int32;
						return lhType;
					}
				}
			}
		}

		private static bool IsDateTimeType(IType type)
		{
			if (type == null)
			{
				return false;
			}
			return typeof(DateTime).IsAssignableFrom(type.ReturnedClass);
		}

		private IType ResolveDateTimeArithmeticResultType(IType lhType, IType rhType)
		{
			// here, we work under the following assumptions:
			//      ------------ valid cases --------------------------------------
			//      1) datetime + {something other than datetime} : always results
			//              in a datetime ( db will catch invalid conversions )
			//      2) datetime - datetime : always results in a DOUBLE
			//      3) datetime - {something other than datetime} : always results
			//              in a datetime ( db will catch invalid conversions )
			//      ------------ invalid cases ------------------------------------
			//      4) datetime + datetime
			//      5) {something other than datetime} - datetime
			//      6) datetime * {any type}
			//      7) datetime / {any type}
			//      8) {any type} / datetime
			// doing so allows us to properly handle parameters as either the left
			// or right side here in the majority of cases
			bool lhsIsDateTime = IsDateTimeType(lhType);
			bool rhsIsDateTime = IsDateTimeType(rhType);

			// handle the (assumed) valid cases:
			// #1 - the only valid datetime addition synatx is one or the other is a datetime (but not both)
			if (Type == HqlSqlWalker.PLUS)
			{
				// one or the other needs to be a datetime for us to get into this method in the first place...
				return lhsIsDateTime ? lhType : rhType;
			}
			else if (Type == HqlSqlWalker.MINUS)
			{
				// #3 - note that this is also true of "datetime - :param"...
				if (lhsIsDateTime && !rhsIsDateTime)
				{
					return lhType;
				}
				// #2
				if (lhsIsDateTime && rhsIsDateTime)
				{
					return NHibernateUtil.Double;
				}
			}
			return null;
		}

		public override void SetScalarColumnText(int i)
		{
			ColumnHelper.GenerateSingleScalarColumn(ASTFactory, this, i);
		}

		/**
		 * Retrieves the left-hand operand of the operator.
		 *
		 * @return The left-hand operand
		 */
		public IASTNode LeftHandOperand
		{
			get { return GetChild(0); }
		}

		/**
		 * Retrieves the right-hand operand of the operator.
		 *
		 * @return The right-hand operand
		 */
		public IASTNode RightHandOperand
		{
			get { return GetChild(1); }
		}

		public string GetDisplayText()
		{
			return "{dataType=" + DataType + "}";
		}
	}
}
