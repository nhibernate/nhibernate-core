using System;
using System.Globalization;

using NHibernate.Hql.Ast.ANTLR.Tree;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Hql.Ast.ANTLR.Util
{
	[CLSCompliant(false)]
	public class LiteralProcessor
	{
		public const string ErrorCannotFetchWithIterate = "fetch may not be used with scroll() or iterate()";
		public const string ErrorNamedParameterDoesNotAppear = "Named parameter does not appear in Query: ";
		public const string ErrorCannotDetermineType = "Could not determine type of: ";
		public const string ErrorCannotFormatLiteral = "Could not format constant value to SQL literal: ";

		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(LiteralProcessor));

		private readonly HqlSqlWalker _walker;
		private static readonly IDecimalFormatter[] _formatters = new IDecimalFormatter[] {
			new ExactDecimalFormatter(),
			new ApproximateDecimalFormatter()
		};

		/// <summary>
		///  Indicates that Float and Double literal values should
		/// be treated using the SQL "exact" format (i.e., '.001')
		/// </summary>
		public static readonly int EXACT = 0;

		/// <summary>
		/// Indicates that Float and Double literal values should
		/// be treated using the SQL "approximate" format (i.e., '1E-3')
		/// </summary>
		public static readonly int APPROXIMATE = 1;

		/// <summary>
		/// In what format should Float and Double literal values be sent
		/// to the database?
		/// See #EXACT, #APPROXIMATE
		/// </summary>
		public static readonly int DECIMAL_LITERAL_FORMAT = EXACT;


		public LiteralProcessor(HqlSqlWalker walker)
		{
			_walker = walker;
		}

		public void LookupConstant(DotNode node)
		{
			string text = ASTUtil.GetPathText(node);
			IQueryable persister = _walker.SessionFactoryHelper.FindQueryableUsingImports(text);
			if (persister != null)
			{
				// the name of an entity class
				string discrim = persister.DiscriminatorSQLValue;
				node.DataType = persister.DiscriminatorType;

				if (InFragment.Null == discrim || InFragment.NotNull == discrim)
				{
					throw new InvalidPathException("subclass test not allowed for null or not null discriminator: '" + text + "'");
				}
				else
				{
					SetSQLValue(node, text, discrim); //the class discriminator value
				}
			}
			else
			{
				Object value = ReflectHelper.GetConstantValue(text);
				if (value == null)
				{
					throw new InvalidPathException("Invalid path: '" + text + "'");
				}
				else
				{
					SetConstantValue(node, text, value);
				}
			}
		}

		public void ProcessNumericLiteral(SqlNode literal)
		{
			if (literal.Type == HqlSqlWalker.NUM_INT || literal.Type == HqlSqlWalker.NUM_LONG)
			{
				literal.Text = DetermineIntegerRepresentation(literal.Text, literal.Type);
			}
			else if (literal.Type == HqlSqlWalker.NUM_FLOAT || literal.Type == HqlSqlWalker.NUM_DOUBLE || literal.Type == HqlSqlWalker.NUM_DECIMAL)
			{
				literal.Text = DetermineDecimalRepresentation(literal.Text, literal.Type);
			}
			else
			{
				log.Warn("Unexpected literal token type [" + literal.Type + "] passed for numeric processing");
			}
		}

		private bool IsAlias(string alias)
		{
			FromClause from = _walker.CurrentFromClause;
			while (from.IsSubQuery)
			{
				if (from.ContainsClassAlias(alias))
				{
					return true;
				}
				from = from.ParentFromClause;
			}
			return from.ContainsClassAlias(alias);
		}

		public void ProcessBoolean(IASTNode constant) 
		{
			// TODO: something much better - look at the type of the other expression!
			// TODO: Have comparisonExpression and/or arithmeticExpression rules complete the resolution of boolean nodes.
			string replacement;
			_walker.TokenReplacements.TryGetValue(constant.Text, out replacement);
			if ( replacement != null ) 
			{
				constant.Text = replacement;
			}
			else 
			{
				bool value = "true" == constant.Text.ToLowerInvariant();
				Dialect.Dialect dialect = _walker.SessionFactoryHelper.Factory.Dialect;
				constant.Text = dialect.ToBooleanValueString(value);
			}
		}


		public void ProcessConstant(SqlNode constant, bool resolveIdent)
		{
			// If the constant is an IDENT, figure out what it means...
			bool isIdent = (constant.Type == HqlSqlWalker.IDENT || constant.Type == HqlSqlWalker.WEIRD_IDENT);

			if (resolveIdent && isIdent && IsAlias(constant.Text))
			{
				// IDENT is a class alias in the FROM.
				IdentNode ident = (IdentNode)constant;
				// Resolve to an identity column.
				ident.Resolve(false, true);
			}
			else
			{
				// IDENT might be the name of a class.
				IQueryable queryable = _walker.SessionFactoryHelper.FindQueryableUsingImports(constant.Text);
				if (isIdent && queryable != null)
				{
					constant.Text = queryable.DiscriminatorSQLValue;
				}
				// Otherwise, it's a literal.
				else
				{
					ProcessLiteral(constant);
				}
			}
		}

		private static string DetermineDecimalRepresentation(string text, int type)
		{
			var literalValue = GetLiteralValue(text, type);

			Decimal number;
			try
			{
				number = Decimal.Parse(literalValue, NumberFormatInfo.InvariantInfo);
			}
			catch (Exception t)
			{
				throw new HibernateException("Could not parse literal [" + text + "] as System.Decimal.", t);
			}

			return _formatters[DECIMAL_LITERAL_FORMAT].Format(number);
		}

		private static string GetLiteralValue(string text, int type)
		{
			string literalValue = text;
			if (type == HqlSqlWalker.NUM_FLOAT)
			{
				if (literalValue.EndsWith("f", StringComparison.OrdinalIgnoreCase))
				{
					return literalValue.Substring(0, literalValue.Length - 1);
				}
			}
			else if (type == HqlSqlWalker.NUM_DOUBLE)
			{
				if (literalValue.EndsWith("d", StringComparison.OrdinalIgnoreCase))
				{
					return literalValue.Substring(0, literalValue.Length - 1);
				}
			}
			else if (type == HqlSqlWalker.NUM_DECIMAL)
			{
				if (literalValue.EndsWith("m", StringComparison.OrdinalIgnoreCase))
				{
					return literalValue.Substring(0, literalValue.Length - 1);
				}
			}
			return literalValue;
		}

		private static string DetermineIntegerRepresentation(string text, int type)
		{
			// prevent the fisrt-exception as possible
			var literalValue = text;
			bool hasLongSpec = literalValue.EndsWith("l", StringComparison.OrdinalIgnoreCase);
			if (hasLongSpec)
			{
				literalValue = literalValue.Substring(0, literalValue.Length - 1);
			}
			try
			{
				if (type == HqlSqlWalker.NUM_INT && literalValue.Length <= 10 || hasLongSpec)
				{
					try
					{
						return int.Parse(literalValue).ToString();
					}
					catch (FormatException)
					{
						log.Info("could not format incoming text [" + text
								 + "] as a NUM_INT; assuming numeric overflow and attempting as NUM_LONG");
					}
					catch (OverflowException)
					{
						log.Info("could not format incoming text [" + text
								 + "] as a NUM_INT; assuming numeric overflow and attempting as NUM_LONG");
					}
				}

				return long.Parse(literalValue).ToString();
			}
			catch (Exception t)
			{
				throw new HibernateException("Could not parse literal [" + text + "] as integer", t);
			}
		}

		private void ProcessLiteral(SqlNode constant)
		{
			string replacement;

			if (_walker.TokenReplacements.TryGetValue(constant.Text, out replacement))
			{
				if (replacement != null)
				{
					if (log.IsDebugEnabled)
					{
						log.Debug("processConstant() : Replacing '" + constant.Text + "' with '" + replacement + "'");
					}
					constant.Text = replacement;
				}
			}
		}

		private static void SetSQLValue(DotNode node, string text, string value)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug("setSQLValue() " + text + " -> " + value);
			}
			node.ClearChildren(); // Chop off the rest of the tree.
			node.Type = HqlSqlWalker.SQL_TOKEN;
			node.Text = value;
			node.SetResolvedConstant(text);
		}

		private void SetConstantValue(DotNode node, string text, object value)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug("setConstantValue() " + text + " -> " + value + " " + value.GetType().Name);
			}

			node.ClearChildren();	// Chop off the rest of the tree.

			if (value is string)
			{
				node.Type = HqlSqlWalker.QUOTED_String;
			}
			else if (value is char)
			{
				node.Type = HqlSqlWalker.QUOTED_String;
			}
			else if (value is byte)
			{
				node.Type = HqlSqlWalker.NUM_INT;
			}
			else if (value is short)
			{
				node.Type = HqlSqlWalker.NUM_INT;
			}
			else if (value is int)
			{
				node.Type = HqlSqlWalker.NUM_INT;
			}
			else if (value is long)
			{
				node.Type = HqlSqlWalker.NUM_LONG;
			}
			else if (value is double)
			{
				node.Type = HqlSqlWalker.NUM_DOUBLE;
			}
			else if (value is decimal)
			{
				node.Type = HqlSqlWalker.NUM_DECIMAL;
			}
			else if (value is float)
			{
				node.Type = HqlSqlWalker.NUM_FLOAT;
			}
			else
			{
				node.Type = HqlSqlWalker.CONSTANT;
			}

			IType type;
			try
			{
				type = TypeFactory.HeuristicType(value.GetType().Name);
			}
			catch (MappingException me)
			{
				throw new QueryException(me);
			}

			if (type == null)
			{
				throw new QueryException(LiteralProcessor.ErrorCannotDetermineType + node.Text);
			}
			try
			{
				ILiteralType literalType = (ILiteralType)type;
				NHibernate.Dialect.Dialect dialect = _walker.SessionFactoryHelper.Factory.Dialect;
				node.Text = literalType.ObjectToSQLString(value, dialect);
			}
			catch (Exception e)
			{
				throw new QueryException(LiteralProcessor.ErrorCannotFormatLiteral + node.Text, e);
			}

			node.DataType = type;
			node.SetResolvedConstant(text);
		}

		interface IDecimalFormatter
		{
			string Format(Decimal number);
		}

		class ExactDecimalFormatter : IDecimalFormatter
		{
			public string Format(Decimal number)
			{
				return number.ToString(NumberFormatInfo.InvariantInfo);
			}
		}

		class ApproximateDecimalFormatter : IDecimalFormatter
		{
			private static readonly string FORMAT_STRING = "#0.0E0";

			public string Format(Decimal number)
			{
				try
				{
					return number.ToString(FORMAT_STRING);

				}
				catch (Exception t)
				{
					throw new HibernateException("Unable to format decimal literal in approximate format [" + number.ToString(NumberFormatInfo.InvariantInfo) + "]", t);
				}
			}
		}
	}
}
