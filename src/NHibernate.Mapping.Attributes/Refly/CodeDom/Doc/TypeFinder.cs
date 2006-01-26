/// Refly License
/// 
/// Copyright (c) 2004 Jonathan de Halleux, http://www.dotnetwiki.org
///
/// This software is provided 'as-is', without any express or implied warranty. 
/// In no event will the authors be held liable for any damages arising from 
/// the use of this software.
/// 
/// Permission is granted to anyone to use this software for any purpose, 
/// including commercial applications, and to alter it and redistribute it 
/// freely, subject to the following restrictions:
///
/// 1. The origin of this software must not be misrepresented; 
/// you must not claim that you wrote the original software. 
/// If you use this software in a product, an acknowledgment in the product 
/// documentation would be appreciated but is not required.
/// 
/// 2. Altered source versions must be plainly marked as such, 
/// and must not be misrepresented as being the original software.
///
///3. This notice may not be removed or altered from any source distribution.

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Refly.CodeDom.Doc
{
	using Refly.CodeDom.Collections;

	/// <summary>
	/// Summary description for TypeFinder.
	/// </summary>
	public sealed class TypeFinder
	{
		static private AssemblyCollection assemblies = new AssemblyCollection();
		static private StringDictionary names = new StringDictionary();
		static private Regex word = new Regex(@"\w",
			RegexOptions.Multiline | RegexOptions.Compiled
			);
		static private StringCollection imports = new StringCollection();

		private TypeFinder(){}

		static TypeFinder()
		{
			AddAssembly(typeof(int).Assembly);
		}

		static public void AddImports(string ns)
		{
			lock(typeof(TypeFinder))
			{
				imports.Add(ns);
			}
		}

		static public void AddAssembly(Assembly a)
		{
			lock(typeof(TypeFinder))
			{
				assemblies.Add(a);
				foreach(Type t in a.GetExportedTypes())
				{
					names.Add(t.FullName,null);
				}
			}
		}

		static public string CrossRef(string text)
		{
			return word.Replace(text,new MatchEvaluator(MatchEval));
		}

		static private string GetType(string name)
		{
			if (names.ContainsKey(name))
				return name;
			foreach(string ns in imports)
			{
				string key = String.Format("{0}.{1}",ns,name);
				if (names.ContainsKey(key))
					return key;
			}
			return null;
		}

		static private string MatchEval(Match m)
		{
			string s = GetType(m.Value);
			if (s!=null)
				return String.Format("<see cref=\"{0}\"/>",m.Value);
			else
				return m.Value;
		}
	}
}
