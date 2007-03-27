using System;
using System.Collections;
using System.Globalization;
using Iesi.Collections;
using NHibernate.Dialect.Function;
using NHibernate.Hql.Util;
using NHibernate.Type;
using NHibernate.Util;
using System.Text.RegularExpressions;

namespace NHibernate.Hql.Classic
{
	/// <summary>
	/// Parsers the select clause of a hibernate query, looking
	/// for a table (well, really class) alias.
	/// </summary>
	public class SelectParser : IParser
	{
		/// <summary></summary>
		static SelectParser()
		{
		}

		/// <summary></summary>
		public SelectParser()
		{
			//TODO: would be nice to use false, but issues with MS SQL
			//TODO: H2.0.3 why not getting info from Dialect?
			pathExpressionParser.UseThetaStyleJoin = true;
		}

		private bool ready;
		private bool first;
		private bool afterNew;
		private bool insideNew;
		private System.Type holderClass;

		private SelectPathExpressionParser pathExpressionParser = new SelectPathExpressionParser();

		private FunctionStack funcStack;
		private int parenCount = 0;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="token"></param>
		/// <param name="q"></param>
		public void Token(string token, QueryTranslator q)
		{
			string lctoken = token.ToLower(CultureInfo.InvariantCulture);

			if (first)
			{
				first = false;
				if (lctoken.Equals("distinct"))
				{
					q.Distinct = true;
					return;
				}
				else if (lctoken.Equals("all"))
				{
					q.Distinct = false;
					return;
				}
			}

			if (afterNew)
			{
				afterNew = false;
				holderClass = SessionFactoryHelper.GetImportedClass(q.Factory, token);
				if (holderClass == null)
				{
					throw new QueryException("class not found: " + token);
				}
				q.HolderClass = holderClass;
				insideNew = true;
			}
			else if (token.Equals(StringHelper.Comma))
			{
				if (ready)
				{
					throw new QueryException("alias or expression expected in SELECT");
				}
				q.AppendScalarSelectToken(StringHelper.CommaSpace);
				ready = true;
			}
			else if ("new".Equals(lctoken))
			{
				afterNew = true;
				ready = false;
			}
			else if (StringHelper.OpenParen.Equals(token))
			{
				parenCount++;
				if (!funcStack.HasFuctions && holderClass != null && !ready)
				{
					//opening paren in new Foo ( ... )
					ready = true;
				}
				else if (funcStack.HasFuctions)
				{
					q.AppendScalarSelectToken(token);
				}
				else
				{
					throw new QueryException("HQL function expected before '(' in SELECT clause.");
				}
				ready = true;
			}
			else if (StringHelper.ClosedParen.Equals(token))
			{
				parenCount--;
				if (parenCount < 0)
				{
					throw new QueryException("'(' expected before ')' in SELECT clause.");
				}

				if (insideNew && !funcStack.HasFuctions && !ready)
				{
					//if we are inside a new Result(), but not inside a nested function
					insideNew = false;
				}
				else if (funcStack.HasFuctions)
				{
					q.AppendScalarSelectToken(token);
					IType scalarType = funcStack.GetReturnType();
					funcStack.Pop();
					ready = funcStack.HasFuctions;
					// if all function was parsed add de type of the first function in stack
					if(!funcStack.HasFuctions)
						q.AddSelectScalar(scalarType);
				}
			}
			else if (IsHQLFunction(lctoken, q) && token == q.Unalias(token))
			{
				if (!ready && !funcStack.HasFuctions)
				{
					// The syntax control inside a functions is delegated to the render
					throw new QueryException("',' expected before function in SELECT: " + token);
				}
				if (funcStack.HasFuctions && funcStack.FunctionGrammar.IsKnownArgument(lctoken))
				{
					// Some function, like extract, may have KnownArgument with the same name of another function
					q.AppendScalarSelectToken(token);
				}
				else
				{
					// Is a nested function
					funcStack.Push(GetFunction(lctoken, q));
					q.AppendScalarSelectToken(token);
					if (!funcStack.SqlFunction.HasArguments)
					{
						q.AddSelectScalar(funcStack.GetReturnType());
						if (!funcStack.SqlFunction.HasParenthesesIfNoArguments)
						{
							funcStack.Pop();
							ready = funcStack.HasFuctions;
						}
					}
				}
			}
			else if (funcStack.HasFuctions)
			{
				bool constantToken = false;
				if (!ready && parenCount != funcStack.NestedFunctionCount)
				{
					throw new QueryException("'(' expected after HQL function in SELECT");
				}
				try
				{
					ParserHelper.Parse(funcStack.PathExpressionParser, q.Unalias(token), ParserHelper.PathSeparators, q);
				}
				catch (QueryException)
				{
					if (IsPathExpression(token))
						throw;
					// If isn't a path the token is added like part of function arguments
					constantToken = true;
				}

				if (constantToken)
				{
					q.AppendScalarSelectToken(token);
				}
				else
				{
					if (funcStack.PathExpressionParser.IsCollectionValued)
					{
						q.AddCollection(
							funcStack.PathExpressionParser.CollectionName,
							funcStack.PathExpressionParser.CollectionRole);
					}
					q.AppendScalarSelectToken(funcStack.PathExpressionParser.WhereColumn);
					funcStack.PathExpressionParser.AddAssociation(q);
				}
				// after a function argument
				ready = false;
			}
			else
			{
				if (!ready)
				{
					throw new QueryException("',' expected in SELECT before:" + token);
				}

				try
				{
					//High probablly to find a valid pathExpression
					ParserHelper.Parse(pathExpressionParser, q.Unalias(token), ParserHelper.PathSeparators, q);
					if (pathExpressionParser.IsCollectionValued)
					{
						q.AddCollection(
							pathExpressionParser.CollectionName,
							pathExpressionParser.CollectionRole);
					}
					else if (pathExpressionParser.WhereColumnType.IsEntityType)
					{
						q.AddSelectClass(pathExpressionParser.SelectName);
					}
					q.AppendScalarSelectTokens(pathExpressionParser.WhereColumns);
					q.AddSelectScalar(pathExpressionParser.WhereColumnType);
					pathExpressionParser.AddAssociation(q);				
				}
				catch (QueryException)
				{
					// Accept costants in SELECT: NH-280
					// TODO: Parse a costant expression like 5+3+8 (now is not supported in SELECT)
					if (IsStringCostant(token))
					{
						q.AppendScalarSelectToken(token);
						q.AddSelectScalar(NHibernateUtil.String);
					}
					else if (IsIntegerConstant(token))
					{
						q.AppendScalarSelectToken(token);
						q.AddSelectScalar(GetIntegerConstantType(token));
					}
					else if (IsFloatingPointConstant(token))
					{
						q.AppendScalarSelectToken(token);
						q.AddSelectScalar(GetFloatingPointConstantType(token));
					}
					else if (IsParameter(token))
					{
						//q.AddNamedParameter(token.Substring(1));
						//q.AppendScalarSelectToken(token);
						throw new QueryException("parameters are not supported in SELECT.", new NotSupportedException());
					}
					else
						throw;
				}
				ready = false;
			}
		}

		private bool IsPathExpression(string token)
		{
			return Regex.IsMatch(token, @"\A[A-Za-z_][A-Za-z_0-9]*[.][A-Za-z_][A-Za-z_0-9]*\z", RegexOptions.Singleline);
		}

		private bool IsStringCostant(string token)
		{
			return Regex.IsMatch(token,@"\A'('{2})*([^'\r\n]*)('{2})*([^'\r\n]*)('{2})*'\z",RegexOptions.Singleline);
		}

		private bool IsIntegerConstant(string token)
		{
			// The tokenizer make difficult parse a signed numerical constant
			return Regex.IsMatch(token,@"\A[+-]?\d\d*\z",RegexOptions.Singleline);
		}

		private bool IsFloatingPointConstant(string token)
		{
			// The tokenizer make difficult parse a signed numerical constant
			return Regex.IsMatch(token, @"\A(?:[-+]?(\d+\.\d+)|(\.\d+))\z", RegexOptions.Singleline);
		}

		private static string paramMatcher = string.Format("\\A([{0}][A-Za-z_][A-Za-z_0-9]*)|[{1}]\\z", StringHelper.NamePrefix, StringHelper.SqlParameter);
		private bool IsParameter(string token)
		{
			return Regex.IsMatch(token, paramMatcher, RegexOptions.Singleline);
		}

		private IType GetIntegerConstantType(string token)
		{
			return NHibernateUtil.Int32;
		}

		private IType GetFloatingPointConstantType(string token)
		{
			return NHibernateUtil.Double;
		}

		private bool IsHQLFunction(string funcName, QueryTranslator q)
		{
			return q.Factory.SQLFunctionRegistry.HasFunction(funcName);
		}

		private ISQLFunction GetFunction(string name, QueryTranslator q)
		{
			return q.Factory.SQLFunctionRegistry.FindSQLFunction(name);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="q"></param>
		public void Start(QueryTranslator q)
		{
			ready = true;
			first = true;
			afterNew = false;
			holderClass = null;
			parenCount = 0;
			funcStack = new FunctionStack(q.Factory);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="q"></param>
		public void End(QueryTranslator q)
		{
			if (parenCount > 0 || funcStack.HasFuctions)
			{
				throw new QueryException("close paren missed in SELECT");
			}
		}
	}
}