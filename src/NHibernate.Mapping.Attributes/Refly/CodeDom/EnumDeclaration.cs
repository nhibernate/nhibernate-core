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
using System.CodeDom;

namespace Refly.CodeDom
{
	using Refly.CodeDom.Expressions;
	using Refly.CodeDom.Collections;
	/// <summary>
	/// A enum declaration
	/// </summary>
	public class EnumDeclaration : MemberDeclaration
	{
		private bool flags;
		private FieldDeclarationCollection fields = new FieldDeclarationCollection();

		internal EnumDeclaration(
			string name,
			Declaration declaringType,  
			bool flags)
			:base(name, declaringType)
		{
			this.flags = flags;
			this.Attributes = MemberAttributes.Public;
		}

		public FieldDeclaration AddField(string name)
		{
			FieldDeclaration fd = new FieldDeclaration(
				Conformer.ToCapitalized(name),
				this,
				new TypeTypeDeclaration(typeof(int))
				);
			fd.Attributes = MemberAttributes.Public;
			this.fields.Add(fd);			
			return fd;
		}

		public FieldDeclaration AddField(string name, int value)
		{
			FieldDeclaration fd = AddField(name);
			fd.InitExpression = Expr.Prim(value);
			return fd;
		}

		public FieldDeclarationCollection Fields
		{
			get
			{
				return this.fields;
			}
		}

		public override CodeTypeMember ToCodeDom()
		{
			CodeTypeDeclaration c = new CodeTypeDeclaration();

			c.IsEnum = true;
			base.ToCodeDom(c);

			// if flag adding attribute
			if (flags)
			{
				CodeAttributeDeclaration attr = new CodeAttributeDeclaration("Flags");
				c.CustomAttributes.Add(attr);
			}

			// adding values
			foreach(FieldDeclaration fd in this.fields)
			{
				if (fd.Name=="Value__")
					continue;

				c.Members.Add( fd.ToCodeDom()) ;
			}

			return c;
		}
	}
}
