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
	using Refly.CodeDom.Collections;
	/// <summary>
	/// Summary description for TryCatchFinallyStatement.
	/// </summary>
	public class TryCatchFinallyStatement : Statement
	{
		private StatementCollection tryStatements = new StatementCollection();
		private CatchClauseCollection catchClauses = new CatchClauseCollection();
		private StatementCollection finallyStatements = new StatementCollection();

		public TryCatchFinallyStatement()
		{}

		public StatementCollection Try
		{
			get
			{
				return this.tryStatements;
			}
		}

		public CatchClauseCollection CatchClauses
		{
			get			
			{
				return this.catchClauses;
			}
		}

		public StatementCollection Finally
		{
			get
			{
				return this.finallyStatements;
			}
		}

		public override void ToCodeDom(CodeStatementCollection statements)
		{
			statements.Add(
				new CodeTryCatchFinallyStatement(
					this.tryStatements.ToCodeDomArray(),
					this.catchClauses.ToCodeDomArray(),
					this.finallyStatements.ToCodeDomArray()
					)
				);
		}

	}
}
