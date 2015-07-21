using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace NHibernate.Util
{
	public class ParserException : ApplicationException
	{
		public ParserException(string message) : base(message) { }
	}

	public class TypeNameParser
	{
		private readonly string defaultNamespace;
		private readonly string defaultAssembly;
		private static readonly Regex WhiteSpaces = new Regex(@"[\t\r\n]", RegexOptions.Compiled);
		private static readonly Regex MultipleSpaces = new Regex(@"[ ]+", RegexOptions.Compiled);

		public TypeNameParser(string defaultNamespace, string defaultAssembly)
		{
			this.defaultNamespace = defaultNamespace;
			this.defaultAssembly = defaultAssembly;
		}

		public static AssemblyQualifiedTypeName Parse(string type)
		{
			return Parse(type, null, null);
		}

		public static AssemblyQualifiedTypeName Parse(string type, string defaultNamespace, string defaultAssembly)
		{
			return new TypeNameParser(defaultNamespace, defaultAssembly).ParseTypeName(type);
		}

		public AssemblyQualifiedTypeName ParseTypeName(string typeName)
		{
			if (typeName == null)
			{
				throw new ArgumentNullException("typeName");
			}
			var type = WhiteSpaces.Replace(typeName, " ");
			type = MultipleSpaces.Replace(type, " ").Replace(", [", ",[").Replace("[ [", "[[").Replace("] ]", "]]");
			if (type.Trim(' ','[', ']', '\\', ',') == string.Empty)
			{
				throw new ArgumentException(string.Format("The type to parse is not a type name:{0}", typeName), "typeName");
			}
			int genericTypeArgsStartIdx = type.IndexOf('[');
			int genericTypeArgsEndIdx = type.LastIndexOf(']');
			int genericTypeCardinalityIdx = -1;
			if (genericTypeArgsStartIdx >= 0)
			{
				genericTypeCardinalityIdx = type.IndexOf('`', 0, genericTypeArgsStartIdx);
			}

			if (genericTypeArgsStartIdx == -1 || genericTypeCardinalityIdx == -1)
			{
				return ParseNonGenericType(type);
			}
			else
			{
				var isArrayType = type.EndsWith("[]");
				if(genericTypeCardinalityIdx < 0)
				{
					throw new ParserException("Invalid generic fully-qualified type name:" + type);
				}
				// the follow will fail with a generic class with more the 9 type-args (I would see that entity class)
				string cardinalityString = type.Substring(genericTypeCardinalityIdx + 1, 1);
				int genericTypeCardinality = int.Parse(cardinalityString);

				// get the FullName of the non-generic type
				string fullName = type.Substring(0, genericTypeArgsStartIdx);
				if (type.Length - genericTypeArgsEndIdx - 1 > 0)
					fullName += type.Substring(genericTypeArgsEndIdx + 1, type.Length - genericTypeArgsEndIdx - 1);

				// parse the type arguments
				var genericTypeArgs = new List<AssemblyQualifiedTypeName>();
				string typeArguments = type.Substring(genericTypeArgsStartIdx + 1, genericTypeArgsEndIdx - genericTypeArgsStartIdx - 1);
				foreach (string item in GenericTypesArguments(typeArguments, genericTypeCardinality))
				{
					var typeArgument = ParseTypeName(item);
					genericTypeArgs.Add(typeArgument);
				}

				// construct the generic type definition
				return MakeGenericType(ParseNonGenericType(fullName), isArrayType, genericTypeArgs.ToArray());
			}
		}

		public AssemblyQualifiedTypeName MakeGenericType(AssemblyQualifiedTypeName qualifiedName, bool isArrayType,
		                                                 AssemblyQualifiedTypeName[] typeArguments)
		{
			Debug.Assert(typeArguments.Length > 0);

			var baseType = qualifiedName.Type;
			var sb = new StringBuilder(typeArguments.Length * 200);
			sb.Append(baseType);
			sb.Append('[');
			for (int i = 0; i < typeArguments.Length; i++)
			{
				if(i>0)
				{
					sb.Append(",");
				}
				sb.Append('[').Append(typeArguments[i].ToString()).Append(']');
			}
			sb.Append(']');
			if(isArrayType)
			{
				sb.Append("[]");
			}
			return new AssemblyQualifiedTypeName(sb.ToString(), qualifiedName.Assembly);
		}

		private static IEnumerable<string> GenericTypesArguments(string s, int cardinality)
		{
			Debug.Assert(cardinality != 0);
			Debug.Assert(string.IsNullOrEmpty(s) == false);
			Debug.Assert(s.Length > 0);

			int startIndex = 0;
			while (cardinality > 0)
			{
				var sb = new StringBuilder(s.Length);
				int bracketCount = 0;

				for (int i = startIndex; i < s.Length; i++)
				{
					switch (s[i])
					{
						case '[':
							bracketCount++;
							continue;

						case ']':
							if (--bracketCount == 0)
							{
								string item = s.Substring(startIndex + 1, i - startIndex - 1);
								yield return item;
								sb = new StringBuilder(s.Length);
								startIndex = i + 2; // 2 = '[' + ']'
							}
							break;

						default:
							sb.Append(s[i]);
							continue;
					}
				}

				if (bracketCount != 0)
				{
					throw new ParserException(string.Format("The brackets are unbalanced in the type name: {0}", s));
				}
				if (sb.Length > 0)
				{
					var result = sb.ToString();
					startIndex += result.Length;
					yield return result.TrimStart(' ', ',');
				}
				cardinality--;
			}
		}

		private AssemblyQualifiedTypeName ParseNonGenericType(string typeName)
		{
			string typeFullName;
			string assembliQualifiedName;

			if (NeedDefaultAssembly(typeName))
			{
				assembliQualifiedName = defaultAssembly;
				typeFullName = typeName;
			}
			else
			{
				int assemblyFullNameIdx = FindAssemblyQualifiedNameStartIndex(typeName);
				if (assemblyFullNameIdx > 0)
				{
					assembliQualifiedName =
						typeName.Substring(assemblyFullNameIdx + 1, typeName.Length - assemblyFullNameIdx - 1).Trim();
					typeFullName = typeName.Substring(0, assemblyFullNameIdx).Trim();
				}
				else
				{
					assembliQualifiedName = null;
					typeFullName = typeName.Trim();					
				}
			}

			if (NeedDefaultNamespace(typeFullName) && !string.IsNullOrEmpty(defaultNamespace))
			{
				typeFullName = string.Concat(defaultNamespace, ".", typeFullName);
			}

			return new AssemblyQualifiedTypeName(typeFullName, assembliQualifiedName);
		}

		private static int FindAssemblyQualifiedNameStartIndex(string typeName)
		{
			for (int i = 0; i < typeName.Length; i++)
			{
				if(typeName[i] == ',' && typeName[i-1] != '\\')
				{
					return i;
				}
			}

			return -1;
		}

		private static bool NeedDefaultNamespaceOrDefaultAssembly(string typeFullName)
		{
			return !typeFullName.StartsWith("System."); // ugly
		}

		private static bool NeedDefaultNamespace(string typeFullName)
		{
			if (!NeedDefaultNamespaceOrDefaultAssembly(typeFullName))
			{
				return false;
			}
			int assemblyFullNameIndex = typeFullName.IndexOf(',');
			int firstDotIndex = typeFullName.IndexOf('.');
			// does not have a dot or the dot is part of AssemblyFullName
			return firstDotIndex < 0 || (firstDotIndex > assemblyFullNameIndex && assemblyFullNameIndex > 0);
		}

		private static bool NeedDefaultAssembly(string typeFullName)
		{
			return NeedDefaultNamespaceOrDefaultAssembly(typeFullName) && typeFullName.IndexOf(',') < 0;
		}
	}
}