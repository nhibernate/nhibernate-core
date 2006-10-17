using System;
using System.Text;
using NHibernate.Engine;
using NHibernate.Type;
using System.Collections;

namespace NHibernate.Dialect.Function
{
	/// <summary>
  /// A SQLFunction implementation that emulates the ANSI SQL trim function
  /// on dialects which do not support the full definition.  However, this function
  /// definition does assume the availability of ltrim, rtrim, and replace functions
  /// which it uses in various combinations to emulate the desired ANSI trim()
	/// functionality.
	/// </summary>
	public class AnsiTrimEmulationFunction: ISQLFunction
	{
		private static readonly ISQLFunction LeadingSpaceTrim = new SQLFunctionTemplate(NHibernateUtil.String, "ltrim( ?1 )");
		private static readonly ISQLFunction TrailingSpaceTrim = new SQLFunctionTemplate(NHibernateUtil.String, "rtrim( ?1 )");
		private static readonly ISQLFunction BothSpaceTrim = new SQLFunctionTemplate(NHibernateUtil.String, "ltrim( rtrim( ?1 ) )");
		private static readonly ISQLFunction BothSpaceTrimFrom = new SQLFunctionTemplate(NHibernateUtil.String, "ltrim( rtrim( ?2 ) )");

		private static readonly ISQLFunction LeadingTrim = new SQLFunctionTemplate(NHibernateUtil.String, "replace( replace( rtrim( replace( replace( ?1, ' ', '${space}$' ), ?2, ' ' ) ), ' ', ?2 ), '${space}$', ' ' )");
		private static readonly ISQLFunction TrailingTrim = new SQLFunctionTemplate(NHibernateUtil.String, "replace( replace( ltrim( replace( replace( ?1, ' ', '${space}$' ), ?2, ' ' ) ), ' ', ?2 ), '${space}$', ' ' )");
		private static readonly ISQLFunction BothTrim = new SQLFunctionTemplate(NHibernateUtil.String, "replace( replace( ltrim( rtrim( replace( replace( ?1, ' ', '${space}$' ), ?2, ' ' ) ) ), ' ', ?2 ), '${space}$', ' ' )");

		#region ISQLFunction Members

		public IType ReturnType(IType columnType, IMapping mapping)
		{
			return NHibernateUtil.String;
		}

		public bool HasArguments
		{
			get { return true; }
		}

		public bool HasParenthesesIfNoArguments
		{
			get { return true; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		/// <param name="factory"></param>
		/// <returns></returns>
		/// <remarks>
		/// according to both the ANSI-SQL and EJB3 specs, trim can either take
		/// exactly one parameter or a variable number of parameters between 1 and 4.
		/// from the SQL spec:
		///<![CDATA[
		/// <trim function> ::=
		///      TRIM <left paren> <trim operands> <right paren>
		///
		/// <trim operands> ::=
		///      [ [ <trim specification> ] [ <trim character> ] FROM ] <trim source>
		///
		/// <trim specification> ::=
		///      LEADING
		///      | TRAILING
		///      | BOTH
		///]]>
		/// If only trim specification is omitted, BOTH is assumed;
		/// if trim character is omitted, space is assumed
		/// </remarks>
		public string Render(System.Collections.IList args, ISessionFactoryImplementor factory)
		{
			if (args.Count < 1 || args.Count > 4)
			{
				throw new QueryException("function takes between 1 and 4 arguments");
			}
			if (args.Count == 1)
			{
				// we have the form: trim(trimSource)
				//      so we trim leading and trailing spaces
				return BothSpaceTrim.Render(args, factory);
			}
			else if ("from".Equals((string)args[0],StringComparison.InvariantCultureIgnoreCase))
			{
				// we have the form: trim(from trimSource).
				//      This is functionally equivalent to trim(trimSource)
				return BothSpaceTrimFrom.Render(args, factory);
			}
			else
			{
				// otherwise, a trim-specification and/or a trim-character
				// have been specified;  we need to decide which options
				// are present and "do the right thing"
				bool leading = true;         // should leading trim-characters be trimmed?
				bool trailing = true;        // should trailing trim-characters be trimmed?
				string trimCharacter = null;    // the trim-character
				string trimSource = null;       // the trim-source

				// potentialTrimCharacterArgIndex = 1 assumes that a
				// trim-specification has been specified.  we handle the
				// exception to that explicitly
				int potentialTrimCharacterArgIndex = 1;
				string firstArg = (string)args[0];
				if ("leading".Equals(firstArg,StringComparison.InvariantCultureIgnoreCase))
				{
					trailing = false;
				}
				else if ("trailing".Equals(firstArg,StringComparison.InvariantCultureIgnoreCase))
				{
					leading = false;
				}
				else if ("both".Equals(firstArg, StringComparison.InvariantCultureIgnoreCase))
				{
				}
				else
				{
					potentialTrimCharacterArgIndex = 0;
				}

				string potentialTrimCharacter = (string)args[potentialTrimCharacterArgIndex];
				if ("from".Equals(potentialTrimCharacter, StringComparison.InvariantCultureIgnoreCase))
				{
					trimCharacter = "' '";
					trimSource = (string)args[potentialTrimCharacterArgIndex + 1];
				}
				else if (potentialTrimCharacterArgIndex + 1 >= args.Count)
				{
					trimCharacter = "' '";
					trimSource = potentialTrimCharacter;
				}
				else
				{
					trimCharacter = potentialTrimCharacter;
					if ("from".Equals((string)args[potentialTrimCharacterArgIndex + 1], StringComparison.InvariantCultureIgnoreCase))
					{
						trimSource = (string)args[potentialTrimCharacterArgIndex + 2];
					}
					else
					{
						trimSource = (string)args[potentialTrimCharacterArgIndex + 1];
					}
				}

				IList argsToUse = new ArrayList();
				argsToUse.Add(trimSource);
				argsToUse.Add(trimCharacter);

				if (trimCharacter.Equals("' '"))
				{
					if (leading && trailing)
					{
						return BothSpaceTrim.Render(argsToUse, factory);
					}
					else if (leading)
					{
						return LeadingSpaceTrim.Render(argsToUse, factory);
					}
					else
					{
						return TrailingSpaceTrim.Render(argsToUse, factory);
					}
				}
				else
				{
					if (leading && trailing)
					{
						return BothTrim.Render(argsToUse, factory);
					}
					else if (leading)
					{
						return LeadingTrim.Render(argsToUse, factory);
					}
					else
					{
						return TrailingTrim.Render(argsToUse, factory);
					}
				}
			}
		}

		#endregion
	}
}
