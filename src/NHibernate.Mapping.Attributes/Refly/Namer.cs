using System;
using System.CodeDom;

namespace Refly.CodeDom
{
	/// <summary>
	/// Summary description for Namer.
	/// </summary>
	public sealed class Namer
	{
		private Namer(){}

		static public string ToCamel(string name)
		{
			string clone = name.TrimStart('_');
			return String.Format("_{0}{1}", 
				Char.ToLower(clone[0]), 
				clone.Substring(1,clone.Length-1)
				);
		}

		static public string ToCapit(String name)
		{
			string clone = name.TrimStart('_');
			return String.Format("{0}{1}", 
				Char.ToUpper(clone[0]), 
				clone.Substring(1,clone.Length-1)
				);
		}

		static public string NormalizeMember(string name, MemberDeclaration member)
		{
			if (member.Attributes==MemberAttributes.Public)
				return Namer.ToCapit(name);
			else
				return Namer.ToCamel(name);
		}
	}
}
