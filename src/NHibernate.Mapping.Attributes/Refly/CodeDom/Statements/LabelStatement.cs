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
	/// <summary>
	/// Summary description for LabelStatement.
	/// </summary>
	public class LabeledStatement : Statement
	{
		private String label;
		private Statement statement;

		public LabeledStatement(string label, Statement statement)
		{
			if (label==null)
				throw new ArgumentNullException("label");
			if (statement ==null)
				throw new ArgumentNullException("statement");

			this.label = label;
			this.statement = statement;
		}

		public override void ToCodeDom(CodeStatementCollection statements)
		{
			CodeStatementCollection col = new CodeStatementCollection();
			statement.ToCodeDom(col);

			statements.Add(
				new CodeLabeledStatement(label,col[0])
				);
		}

	}
}
