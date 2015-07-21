using System.Collections;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;
using System;

namespace NHibernate.Dialect.Function
{
	/// <summary>
	/// Represents HQL functions that can have different representations in different SQL dialects.
	/// E.g. in HQL we can define function <code>concat(?1, ?2)</code> to concatenate two strings 
	/// p1 and p2. Target SQL function will be dialect-specific, e.g. <code>(?1 || ?2)</code> for 
	/// Oracle, <code>concat(?1, ?2)</code> for MySql, <code>(?1 + ?2)</code> for MS SQL.
	/// Each dialect will define a template as a string (exactly like above) marking function 
	/// parameters with '?' followed by parameter's index (first index is 1).
	/// </summary>
	[Serializable]
	public class SQLFunctionTemplate : ISQLFunction
	{
		private const int InvalidArgumentIndex = -1;
		private static readonly Regex SplitRegex = new Regex("(\\?[0-9]+)");

		private struct TemplateChunk
		{
			public string Text; // including prefix if parameter
			public int ArgumentIndex;

			public TemplateChunk(string chunk, int argIndex)
			{
				Text = chunk;
				ArgumentIndex = argIndex;
			}
		}

		private readonly IType returnType = null;
		private readonly bool hasArguments;
		private readonly bool hasParenthesesIfNoArgs;

		private readonly string template;
		private TemplateChunk[] chunks;

		public SQLFunctionTemplate(IType type, string template) : this(type, template, true)
		{
		}

		public SQLFunctionTemplate(IType type, string template, bool hasParenthesesIfNoArgs)
		{
			returnType = type;
			this.template = template;
			this.hasParenthesesIfNoArgs = hasParenthesesIfNoArgs;

			InitFromTemplate();
			hasArguments = chunks.Length > 1;
		}

		private void InitFromTemplate()
		{
			string[] stringChunks = SplitRegex.Split(template);
			chunks = new TemplateChunk[stringChunks.Length];

			for (int i = 0; i < stringChunks.Length; i++)
			{
				string chunk = stringChunks[i];
				if (i % 2 == 0)
				{
					// Text part.
					chunks[i] = new TemplateChunk(chunk, InvalidArgumentIndex);
				}
				else
				{
					// Separator, i.e. argument
					int argIndex = int.Parse(chunk.Substring(1), CultureInfo.InvariantCulture);
					chunks[i] = new TemplateChunk(stringChunks[i], argIndex);
				}
			}
		}

		#region ISQLFunction Members

		public IType ReturnType(IType columnType, IMapping mapping)
		{
			return (returnType == null) ? columnType : returnType;
		}

		public bool HasArguments
		{
			get { return hasArguments; }
		}

		public bool HasParenthesesIfNoArguments
		{
			get { return hasParenthesesIfNoArgs; }
		}

		/// <summary>
		/// Applies the template to passed in arguments.
		/// </summary>
		/// <param name="args">args function arguments</param>
		/// <param name="factory">generated SQL function call</param>
		/// <returns></returns>
		public SqlString Render(IList args, ISessionFactoryImplementor factory)
		{
			SqlStringBuilder buf = new SqlStringBuilder();
			foreach (TemplateChunk tc in chunks)
			{
				if (tc.ArgumentIndex != InvalidArgumentIndex)
				{
					int adjustedIndex = tc.ArgumentIndex - 1; // Arg indices are one-based
					object arg = adjustedIndex < args.Count ? args[adjustedIndex] : null;
					// TODO: if (arg == null) QueryException is better ?
					if (arg != null)
					{
						if (arg is Parameter || arg is SqlString)
						{
							buf.AddObject(arg);
						}
						else
						{
							buf.Add(arg.ToString());
						}
					}
				}
				else
				{
					buf.Add(tc.Text);
				}
			}
			return buf.ToSqlString();
		}

		#endregion

		public override string ToString()
		{
			return template;
		}
	}
}
