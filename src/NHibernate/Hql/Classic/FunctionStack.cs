using System;
using System.Collections;
using NHibernate.Dialect.Function;
using NHibernate.Type;
using NHibernate.Engine;

namespace NHibernate.Hql.Classic
{
	public class FunctionStack
	{
		private class FunctionHolder
		{
			private readonly PathExpressionParser pathExpressionParser = new PathExpressionParser();
			public PathExpressionParser PathExpressionParser
			{
				get
				{
					FirstValidColumnType = pathExpressionParser.WhereColumnType;
					return pathExpressionParser;
				}
			}

			private readonly ISQLFunction sqlFunction;
			public ISQLFunction SqlFunction
			{
				get { return sqlFunction; }
			}

			private readonly IFunctionGrammar functionGrammar;
			public IFunctionGrammar FunctionGrammar
			{
				get { return functionGrammar; }
			}

			public FunctionHolder(ISQLFunction sqlFunction)
			{
				pathExpressionParser.UseThetaStyleJoin = true;
				this.sqlFunction = sqlFunction;
				functionGrammar = sqlFunction as IFunctionGrammar;
				if (functionGrammar == null)
					functionGrammar = new CommonGrammar();
			}

			private IType firstValidColumnReturnType = null;
			/// <summary>
			/// Used to hold column type in nested functions.
			/// </summary>
			public IType FirstValidColumnType
			{
				get { return firstValidColumnReturnType; }
				set
				{
					if (firstValidColumnReturnType == null)
						firstValidColumnReturnType = value;
				}
			}
		}

		private Stack stack = new Stack(5);
		private readonly IMapping mapping;
		public FunctionStack(IMapping mapping)
		{
			if (mapping == null)
				throw new ArgumentNullException("mapping");
			this.mapping = mapping;
		}

		public void Push(ISQLFunction sqlFunction)
		{
			if (sqlFunction == null)
				throw new ArgumentNullException("sqlFunction");
			stack.Push(new FunctionHolder(sqlFunction));
		}

		public void Pop()
		{
			IType firstReturnType = null;
			try
			{
				// Example in nested functions
				// abs(max(a.BodyWeight))
				// To know the ReturnType of "abs" we must know the ReturnType of "max" and the ReturnType of "max"
				// depend on the type of property "a.BodyWeight"
				FunctionHolder fh = (stack.Peek() as FunctionHolder);
				firstReturnType = fh.SqlFunction.ReturnType(fh.FirstValidColumnType, mapping);
				stack.Pop();
			}
			catch (InvalidOperationException ex)
			{
				throw new QueryException("Parsing HQL: Pop on empty functions stack.", ex);
			}
			if (stack.Count > 0)
				(stack.Peek() as FunctionHolder).FirstValidColumnType = firstReturnType;
		}

		public bool HasFuctions
		{
			get { return stack.Count > 0; }
		}

		public int NestedFunctionCount
		{
			get { return stack.Count; }
		}

		public PathExpressionParser PathExpressionParser
		{
			get { return (stack.Peek() as FunctionHolder).PathExpressionParser; }
		}

		public ISQLFunction SqlFunction
		{
			get { return (stack.Peek() as FunctionHolder).SqlFunction; }
		}

		public IFunctionGrammar FunctionGrammar
		{
			get { return (stack.Peek() as FunctionHolder).FunctionGrammar; }
		}

		public IType GetReturnType()
		{
			FunctionHolder fh = (stack.Peek() as FunctionHolder);
			return fh.SqlFunction.ReturnType(fh.FirstValidColumnType, mapping);
		}
	}
}
