using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace NHibernate.Util
{
	public class ParserException : ApplicationException
	{
		public ParserException(string message) : base(message) {}
	}

	public class TypeNameParser
	{
		private TextReader input;

		private void SkipSpaces()
		{
			while (input.Peek() == ' ')
			{
				input.Read();
			}
		}

		private char[] Characters(int count)
		{
			var chars = new char[count];
			if (input.ReadBlock(chars, 0, count) < count)
			{
				throw new ParserException(count + " characters expected");
			}

			return chars;
		}

		private char[] PossiblyEscapedCharacter()
		{
			if (input.Peek() == '\\')
			{
				return Characters(2);
			}
			else
			{
				return Characters(1);
			}
		}

		private string AssemblyName()
		{
			var result = new StringBuilder();
			SkipSpaces();

			int code;
			while ((code = input.Peek()) != -1)
			{
				var ch = (char) code;

				if (ch == ']')
				{
					break;
				}

				result.Append(PossiblyEscapedCharacter());
			}

			return result.ToString();
		}

		private string BracketedPart(string defaultNamespace, string defaultAssembly)
		{
			Debug.Assert(input.Peek() == '[');

			var result = new StringBuilder();
			var genericTypeName = new StringBuilder(200);

			int depth = 0;
			do
			{
				int c = input.Peek();
				if (c == '[')
				{
					depth++;
					result.Append(PossiblyEscapedCharacter());
				}
				else if (c == ']')
				{
					depth--;
					if (genericTypeName.Length > 0)
					{
						var r = Parse(genericTypeName.ToString(), defaultNamespace, defaultAssembly);
						result.Append(r.ToString());
						genericTypeName.Remove(0, genericTypeName.Length);
					}
					result.Append(PossiblyEscapedCharacter());
				}
				else if (c == ',' || c == ' ')
				{
					if (genericTypeName.Length > 0)
						genericTypeName.Append(PossiblyEscapedCharacter());
					else
						result.Append(PossiblyEscapedCharacter());
				}
				else
				{
					genericTypeName.Append(PossiblyEscapedCharacter());
				}
			}
			while (depth > 0 && input.Peek() != -1);

			if (depth > 0 && input.Peek() == -1)
			{
				throw new ParserException("Unmatched left bracket ('[')");
			}

			return result.ToString();
		}

		public AssemblyQualifiedTypeName ParseTypeName(string text, string defaultNamespace, string defaultAssembly)
		{
			text = text.Trim();
			if (IsSystemType(text))
			{
				defaultNamespace = null;
				defaultAssembly = null;
			}
			var type = new StringBuilder(text.Length);
			string assembly = StringHelper.IsEmpty(defaultAssembly) ? null : defaultAssembly;

			try
			{
				bool seenNamespace = false;

				using (input = new StringReader(text))
				{
					int code;
					while ((code = input.Peek()) != -1)
					{
						var ch = (char) code;

						if (ch == '.')
						{
							seenNamespace = true;
						}

						if (ch == ',')
						{
							input.Read();
							assembly = AssemblyName();
							if (input.Peek() != -1)
							{
								throw new ParserException("Extra characters found at the end of the type name");
							}
						}
						else if (ch == '[')
						{
							type.Append(BracketedPart(defaultNamespace, defaultAssembly));
						}
						else
						{
							type.Append(PossiblyEscapedCharacter());
						}
					}

					input.Close();
				}
				if (!seenNamespace && StringHelper.IsNotEmpty(defaultNamespace))
				{
					type.Insert(0, '.').Insert(0, defaultNamespace);
				}
				return new AssemblyQualifiedTypeName(type.ToString(), assembly);
			}
			catch (Exception e)
			{
				throw new ArgumentException("Invalid fully-qualified type name: " + text, "text", e);
			}
		}

		private bool IsSystemType(string tyname)
		{
			return tyname.StartsWith("System."); // ugly
		}

		public static AssemblyQualifiedTypeName Parse(string text)
		{
			return Parse(text, null, null);
		}

		public static AssemblyQualifiedTypeName Parse(string text, string defaultNamespace, string defaultAssembly)
		{
			return new TypeNameParser().ParseTypeName(text, defaultNamespace, defaultAssembly);
		}
	}
}