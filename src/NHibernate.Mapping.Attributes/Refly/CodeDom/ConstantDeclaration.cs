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
	using Refly.CodeDom.Collections;
	using Refly.CodeDom.Expressions;

	/// <summary>
	/// A constant value declaration.
	/// </summary>
	public class ConstantDeclaration : MemberDeclaration
	{
		private Type type;
		private SnippetExpression expression;

		internal ConstantDeclaration(
			string name,
			Declaration declaringType, 
			Type t, 
			SnippetExpression expression
			)
			:base(name,declaringType)
		{
			if (t==null)
				throw new ArgumentNullException("t");
			if (expression == null)
				throw new ArgumentNullException("t");
			this.type = t;
			this.expression = expression;
		}

		public Type Type
		{
			get
			{
				return this.type;
			}
		}

		public SnippetExpression Expression
		{
			get
			{
				return this.expression;
			}
		}

		public override System.CodeDom.CodeTypeMember ToCodeDom()
		{
			CodeMemberField f = new CodeMemberField(
				this.Type,
				this.Conformer.NormalizeMember(this.Name,this.Attributes)
				);
			f.InitExpression = this.Expression.ToCodeDom();

			// setting attributes
			this.Attributes = MemberAttributes.Const | this.Attributes;
			base.ToCodeDom(f);

			return f;
		}

	}
}
