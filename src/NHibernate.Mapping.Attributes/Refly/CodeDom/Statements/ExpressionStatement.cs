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
	/// <summary>
	/// Summary description for ExpressionStatement.
	/// </summary>
	public class ExpressionStatement : Statement
	{
		private Expression expression;

		public ExpressionStatement(Expression expression)
		{
			if (expression==null)
				throw new ArgumentNullException("expression");
			this.expression = expression;
		}

		public Expression Expression
		{
			get
			{
				return this.expression;
			}
		}

		public override void ToCodeDom(CodeStatementCollection statements)
		{
			if (this.expression!=null)
				statements.Add(new CodeExpressionStatement(this.expression.ToCodeDom()));
			else
				statements.Add(new CodeStatement());
		}

	}
}
