using System.Text;
using NHibernate.Engine;
using NHibernate.Type;
using System.Collections;

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
	public class SQLFunctionTemplate: ISQLFunction
	{
		private const char paramId = '?';
		private const int invaligArgIdx = -1;
		private struct TemplateChunk
		{
			public string Chunk; // including paramId if is a param
			public int ArgIndex; // Argument index if is param
			public TemplateChunk(string chunk, int argIndex)
			{
				Chunk = chunk;
				ArgIndex = argIndex;
			}
		}

		private readonly IType returnType = null;
		private readonly bool hasArguments;
		private readonly bool hasParenthesesIfNoArgs;

		private readonly string template;
		private IList paramsChunk = new ArrayList(); 

		public SQLFunctionTemplate(IType type, string template) : this(type, template, true) { }

		public SQLFunctionTemplate(IType type, string template, bool hasParenthesesIfNoArgs)
		{
			returnType = type;
			this.template = template;
			this.hasParenthesesIfNoArgs = hasParenthesesIfNoArgs;
			InitTemplateChunks(template);
			hasArguments = (paramsChunk.Count > 1) || (paramsChunk.Count == 0 && ((TemplateChunk)paramsChunk[0]).ArgIndex != invaligArgIdx);
		}

		private void InitTemplateChunks(string stringPart)
		{
			int chunkStart = 0;
			string paramChunk;
			for (int i = 0; i < stringPart.Length; i++)// find params
			{
				if (paramId.Equals(stringPart[i]))
				{// posible param found
					int argIdx = GetArgIndex(stringPart, i, out paramChunk);
					if (argIdx != invaligArgIdx)
					{// is a valid argument index
						if ((i - chunkStart) > 0)
							AddChunk(stringPart.Substring(chunkStart, (i - chunkStart)), invaligArgIdx); // add the chunk before param
						AddChunk(paramChunk, argIdx); // add the chunk of the param
						i += paramChunk.Length;
						chunkStart = i;
					}
				}
			}
			if (chunkStart < stringPart.Length - 1)
			{// add the last chunk
				AddChunk(stringPart.Substring(chunkStart), invaligArgIdx);
			}
		}

		private void AddChunk(string chunk, int argIndex)
		{
			paramsChunk.Add(new TemplateChunk(chunk, argIndex));
		}

		/// <summary>
		/// Check if is a valid param
		/// </summary>
		/// <param name="stringPart">complete string for template</param>
		/// <param name="startingFrom">Analize string starting from</param>
		/// <param name="paramChunk">The chunk of the param including paramId (char '?')</param>
		/// <returns>the index of the argument (ex: for ?5 return 4) if is a valid param; otherwise invaligArgIdx</returns>
		private int GetArgIndex(string stringPart, int startingFrom, out string paramChunk)
		{
			int result = invaligArgIdx;
			if ((startingFrom >= 0) && (startingFrom < stringPart.Length - 1) &&
				(paramId.Equals(stringPart[startingFrom])))
			{
				int pos = startingFrom + 1;
				StringBuilder argIndex = new StringBuilder();
				while (pos < stringPart.Length && char.IsDigit(stringPart[pos]))
				{
					argIndex.Append(stringPart[pos]);
					pos++;
				}
				if (argIndex.Length > 0)
				{
					paramChunk = paramId + argIndex.ToString();
					result = int.Parse(argIndex.ToString()) - 1;
				}
				else
				{
					paramChunk = string.Empty;
				}
			}
			else
			{
				paramChunk = string.Empty;
			}
			return (result >= 0) ? result : invaligArgIdx;
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
		public string Render(IList args, ISessionFactoryImplementor factory)
		{
			StringBuilder buf = new StringBuilder();
			foreach (TemplateChunk tc in paramsChunk)
			{
				if (tc.ArgIndex >= 0)
				{
					object arg = tc.ArgIndex < args.Count ? args[tc.ArgIndex] : null;
					if (arg != null)
					{
						buf.Append(arg);
					}
				}
				else
				{
					buf.Append(tc.Chunk);
				}
			}
			return buf.ToString();
		}

		#endregion

		public override string ToString()
		{
			return template;
		}
	}
}
