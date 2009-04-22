using System;
using System.Globalization;
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
		public SelectParser()
		{
			//TODO: would be nice to use false, but issues with MS SQL
			//TODO: H2.0.3 why not getting info from Dialect?
			pathExpressionParser.UseThetaStyleJoin = true;
		}

		private bool readyForAliasOrExpression;
		private bool first;
		private bool afterNew;
		private bool insideNew;
		private System.Type holderClass;

		private readonly SelectPathExpressionParser pathExpressionParser = new SelectPathExpressionParser();

		private FunctionStack funcStack;
		private int parenCount = 0;

		public void Token(string token, QueryTranslator q)
		{
			string lctoken = token.ToLowerInvariant();

			if (first)
			{
				first = false;
				if ("distinct".Equals(lctoken))
				{
					q.Distinct = true;
					return;
				}
				else if ("all".Equals(lctoken))
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
				if (readyForAliasOrExpression)
				{
					throw new QueryException("alias or expression expected in SELECT");
				}
				q.AppendScalarSelectToken(StringHelper.CommaSpace);
				readyForAliasOrExpression = true;
			}
			else if ("new".Equals(lctoken))
			{
				afterNew = true;
				readyForAliasOrExpression = false;
			}
			else if (StringHelper.OpenParen.Equals(token))
			{
				parenCount++;
				if (!funcStack.HasFunctions && holderClass != null && !readyForAliasOrExpression)
				{
					//opening paren in new Foo ( ... )
					readyForAliasOrExpression = true;
				}
				else if (funcStack.HasFunctions)
				{
					q.AppendScalarSelectToken(token);
				}
				else
				{
					throw new QueryException("HQL function expected before '(' in SELECT clause.");
				}
				readyForAliasOrExpression = true;
			}
			else if (StringHelper.ClosedParen.Equals(token))
			{
				parenCount--;
				if (parenCount < 0)
				{
					throw new QueryException("'(' expected before ')' in SELECT clause.");
				}

				if (insideNew && !funcStack.HasFunctions && !readyForAliasOrExpression)
				{
					//if we are inside a new Result(), but not inside a nested function
					insideNew = false;
				}
				else if (funcStack.HasFunctions)
				{
					q.AppendScalarSelectToken(token);
					IType scalarType = funcStack.GetReturnType();
					funcStack.Pop();

					// Can't have an alias or expression right after the closing parenthesis of a function call.
					readyForAliasOrExpression = false;

					// if all functions were parsed add the type of the first function in stack
					if (!funcStack.HasFunctions)
						q.AddSelectScalar(scalarType);
				}
			}
			else if (IsHQLFunction(lctoken, q) && token == q.Unalias(token))
			{
				if (!readyForAliasOrExpression && !funcStack.HasFunctions)
				{
					// The syntax control inside a functions is delegated to the render
					throw new QueryException("',' expected before function in SELECT: " + token);
				}
				if (funcStack.HasFunctions && funcStack.FunctionGrammar.IsKnownArgument(lctoken))
				{
					// Some function, like extract, may have KnownArgument with the same name of another function
					q.AppendScalarSelectToken(token);
				}
				else
				{
					// Is a nested function
					funcStack.Push(GetFunction(lctoken, q));
					q.AppendScalarSelectToken(token);
					if (!funcStack.SqlFunction.HasArguments && !funcStack.SqlFunction.HasParenthesesIfNoArguments)
					{
						q.AddSelectScalar(funcStack.GetReturnType());
						funcStack.Pop();
						readyForAliasOrExpression = funcStack.HasFunctions;
					}
				}
			}
			else if (funcStack.HasFunctions)
			{
				bool constantToken = false;
				var expectedParen = parenCount + ((insideNew) ? -1 : 0);
				if (!readyForAliasOrExpression && expectedParen != funcStack.NestedFunctionCount)
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

				if (token.StartsWith(ParserHelper.HqlVariablePrefix))
				{
					q.AddNamedParameter(token.Substring(1));
					q.AppendScalarSelectParameter();
				}
				else if (constantToken)
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
				readyForAliasOrExpression = false;
			}
			else
			{
				if (!readyForAliasOrExpression)
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
						q.AddSelectScalar(GetFloatingPointConstantType());
					}
					else if (token.StartsWith(ParserHelper.HqlVariablePrefix))
					{
						q.AddNamedParameter(token.Substring(1));
						q.AppendScalarSelectParameter();
					}
					else
						throw;
				}
				readyForAliasOrExpression = false;
			}
		}

		#region RegExs
		private static readonly Regex pathExpressionRegEx = new Regex(@"\A[A-Za-z_][A-Za-z_0-9]*[.][A-Za-z_][A-Za-z_0-9]*\z", RegexOptions.Singleline | RegexOptions.Compiled);
		private static readonly Regex stringCostantRegEx = new Regex(@"\A'('{2})*([^'\r\n]*)('{2})*([^'\r\n]*)('{2})*'\z", RegexOptions.Singleline | RegexOptions.Compiled);
		#endregion

		private static bool IsPathExpression(string token)
		{
			return pathExpressionRegEx.IsMatch(token);
		}

		private static bool IsStringCostant(string token)
		{
			return stringCostantRegEx.IsMatch(token);
		}

		private static bool IsIntegerConstant(string token)
		{
			long l;
			return long.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out l);
		}

		private static bool IsFloatingPointConstant(string token)
		{
			double d;
			return double.TryParse(token, NumberStyles.Number, CultureInfo.InvariantCulture, out d);
		}

		private static IType GetIntegerConstantType(string token)
		{
			int i;
			if (int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out i))
			{
				return NHibernateUtil.Int32;
			}
			else
			{
				return NHibernateUtil.Int64;
			}
		}

		private static IType GetFloatingPointConstantType()
		{
			return NHibernateUtil.Double;
		}

		private static bool IsHQLFunction(string funcName, QueryTranslator q)
		{
			return q.Factory.SQLFunctionRegistry.HasFunction(funcName);
		}

		private static ISQLFunction GetFunction(string name, QueryTranslator q)
		{
			return q.Factory.SQLFunctionRegistry.FindSQLFunction(name);
		}

		public void Start(QueryTranslator q)
		{
			readyForAliasOrExpression = true;
			first = true;
			afterNew = false;
			holderClass = null;
			parenCount = 0;
			funcStack = new FunctionStack(q.Factory);
		}

		public void End(QueryTranslator q)
		{
			if (parenCount > 0 || funcStack.HasFunctions)
			{
				throw new QueryException("close paren missed in SELECT");
			}
		}
	}
}
