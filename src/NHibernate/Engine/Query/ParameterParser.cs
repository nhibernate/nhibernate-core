using System;
using NHibernate.Hql.Classic;
using NHibernate.Util;

namespace NHibernate.Engine.Query
{
	/// <summary>
	/// The single available method <see cref="ParameterParser.Parse" />
	/// is responsible for parsing a query string and recognizing tokens in
	/// relation to parameters (either named, ejb3-style, or ordinal) and
	/// providing callbacks about such recognitions.
	/// </summary>
	public class ParameterParser
	{
		private static readonly int NewLineLength = Environment.NewLine.Length;

		public interface IRecognizer
		{
			void OutParameter(int position);
			void OrdinalParameter(int position);
			void NamedParameter(string name, int position);
			void JpaPositionalParameter(string name, int position);
			void Other(char character);
		}

		// Disallow instantiation
		private ParameterParser()
		{
		}

		/// <summary>
		/// Performs the actual parsing and tokenizing of the query string making appropriate
		/// callbacks to the given recognizer upon recognition of the various tokens.
		/// </summary>
		/// <remarks>
		/// Note that currently, this only knows how to deal with a single output
		/// parameter (for callable statements).  If we later add support for
		/// multiple output params, this, obviously, needs to change.
		/// </remarks>
		/// <param name="sqlString">The string to be parsed/tokenized.</param>
		/// <param name="recognizer">The thing which handles recognition events.</param>
		/// <exception cref="QueryException" />
		public static void Parse(string sqlString, IRecognizer recognizer)
		{
			bool hasMainOutputParameter = sqlString.IndexOf("call") > 0 &&
			                              sqlString.IndexOf("?") < sqlString.IndexOf("call") &&
			                              sqlString.IndexOf("=") < sqlString.IndexOf("call");
			bool foundMainOutputParam = false;

			int stringLength = sqlString.Length;
			bool inQuote = false;
			bool afterNewLine = false;
			for (int indx = 0; indx < stringLength; indx++)
			{
				// check comments
				if (indx + 1 < stringLength && sqlString.Substring(indx,2) == "/*")
				{
					var closeCommentIdx = sqlString.IndexOf("*/");
					indx = closeCommentIdx + 1;
					continue;
				}
				if (afterNewLine && (indx + 1 < stringLength) && sqlString.Substring(indx, 2) == "--")
				{
					var closeCommentIdx = sqlString.IndexOf(Environment.NewLine, indx + 2);
					indx = closeCommentIdx + NewLineLength - 1;
					continue;
				}
				if (indx + NewLineLength -1 < stringLength && sqlString.Substring(indx, NewLineLength) == Environment.NewLine)
				{
					afterNewLine = true;
					indx += NewLineLength - 1;
					continue;
				}
				afterNewLine = false;

				char c = sqlString[indx];
				if (inQuote)
				{
					if ('\'' == c)
					{
						inQuote = false;
					}
					recognizer.Other(c);
				}
				else if ('\'' == c)
				{
					inQuote = true;
					recognizer.Other(c);
				}
				else
				{
					if (c == ':')
					{
						// named parameter
						int right = StringHelper.FirstIndexOfChar(sqlString, ParserHelper.HqlSeparators, indx + 1);
						int chopLocation = right < 0 ? sqlString.Length : right;
						string param = sqlString.Substring(indx + 1, chopLocation - (indx + 1));
						recognizer.NamedParameter(param, indx);
						indx = chopLocation - 1;
					}
					else if (c == '?')
					{
						// could be either an ordinal or ejb3-positional parameter
						if (indx < stringLength - 1 && char.IsDigit(sqlString[indx + 1]))
						{
							// a peek ahead showed this as an ejb3-positional parameter
							int right = StringHelper.FirstIndexOfChar(sqlString, ParserHelper.HqlSeparators, indx + 1);
							int chopLocation = right < 0 ? sqlString.Length : right;
							string param = sqlString.Substring(indx + 1, chopLocation - (indx + 1));
							// make sure this "name" is an integral
							try
							{
								int.Parse(param);
							}
							catch (FormatException e)
							{
								throw new QueryException("ejb3-style positional param was not an integral ordinal", e);
							}
							recognizer.JpaPositionalParameter(param, indx);
							indx = chopLocation - 1;
						}
						else
						{
							if (hasMainOutputParameter && !foundMainOutputParam)
							{
								foundMainOutputParam = true;
								recognizer.OutParameter(indx);
							}
							else
							{
								recognizer.OrdinalParameter(indx);
							}
						}
					}
					else
					{
						recognizer.Other(c);
					}
				}
			}
		}
	}
}