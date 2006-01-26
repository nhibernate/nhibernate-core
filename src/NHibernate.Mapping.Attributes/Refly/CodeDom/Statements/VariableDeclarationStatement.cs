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

namespace Refly.CodeDom.Statements
{
	using Refly.CodeDom.Expressions;

	public class VariableDeclarationStatement : Statement
	{
		private ITypeDeclaration type;
		private String name;
		private Expression initExpression;

		public VariableDeclarationStatement(
			ITypeDeclaration type,
			string name
			)
		{
			if (type==null)
				throw new ArgumentNullException("type");
			if (name==null)
				throw new ArgumentNullException("name");
			this.type = type;
			this.name = name;
		}

		public ITypeDeclaration Type
		{
			get
			{
				return this.type;
			}
		}

		public String Name
		{
			get
			{
				return this.name;
			}
		}

		public Expression InitExpression
		{
			get
			{
				return this.initExpression;
			}
			set
			{
				this.initExpression = value;
			}
		}

		public static explicit operator VariableReferenceExpression(VariableDeclarationStatement stm)
		{
			return new VariableReferenceExpression(stm);
		}

		public override void ToCodeDom(CodeStatementCollection statements)
		{
			CodeVariableDeclarationStatement var = new CodeVariableDeclarationStatement(
				this.Type.TypeReference,
				this.Name
				);
			if (this.InitExpression!=null)
				var.InitExpression = this.InitExpression.ToCodeDom();

			statements.Add(var);
		}
	}
}
