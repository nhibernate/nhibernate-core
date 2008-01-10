using System;
using System.Collections.Generic;
using NHibernate.Dialect.Function;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Hql.Classic
{
	public class FunctionStack
	{
		private class FunctionHolder
		{
			private readonly PathExpressionParser pathExpressionParser = new PathExpressionParser();
			private readonly ISQLFunction sqlFunction;
			private readonly IFunctionGrammar functionGrammar;
			private IType firstValidColumnReturnType = null;

			public PathExpressionParser PathExpressionParser
			{
				get
				{
					FirstValidColumnType = pathExpressionParser.WhereColumnType;
					return pathExpressionParser;
				}
			}

			public ISQLFunction SqlFunction
			{
				get { return sqlFunction; }
			}

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

		private readonly Stack<FunctionHolder> stack = new Stack<FunctionHolder>(5);
		private readonly IMapping mapping;
		
		public FunctionStack(IMapping mapping)
		{
			if (mapping == null)
			{
				throw new ArgumentNullException("mapping");
			}
			this.mapping = mapping;
		}

		public void Push(ISQLFunction sqlFunction)
		{
			if (sqlFunction == null)
			{
				throw new ArgumentNullException("sqlFunction");
			}
			stack.Push(new FunctionHolder(sqlFunction));
		}

		private FunctionHolder Peek()
		{
			return stack.Peek();
		}

		public void Pop()
		{
			IType firstReturnType;
			try
			{
				// Example in nested functions
				// abs(max(a.BodyWeight))
				// To know the ReturnType of "abs" we must know the ReturnType of "max" and the ReturnType of "max"
				// depend on the type of property "a.BodyWeight"
				FunctionHolder fh = Peek();
				firstReturnType = fh.SqlFunction.ReturnType(fh.FirstValidColumnType, mapping);
				stack.Pop();
			}
			catch (InvalidOperationException ex)
			{
				throw new QueryException("Parsing HQL: Pop on empty functions stack.", ex);
			}
			if (stack.Count > 0)
			{
				Peek().FirstValidColumnType = firstReturnType;
			}
		}

		public bool HasFunctions
		{
			get { return stack.Count > 0; }
		}

		public int NestedFunctionCount
		{
			get { return stack.Count; }
		}

		public PathExpressionParser PathExpressionParser
		{
			get { return Peek().PathExpressionParser; }
		}

		public ISQLFunction SqlFunction
		{
			get { return Peek().SqlFunction; }
		}

		public IFunctionGrammar FunctionGrammar
		{
			get { return Peek().FunctionGrammar; }
		}

		public IType GetReturnType()
		{
			FunctionHolder fh = Peek();
			return fh.SqlFunction.ReturnType(fh.FirstValidColumnType, mapping);
		}
	}
}
