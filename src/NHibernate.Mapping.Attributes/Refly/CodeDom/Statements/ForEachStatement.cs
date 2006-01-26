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
using System.Collections;

namespace Refly.CodeDom.Statements
{
	using Refly.CodeDom.Collections;
	using Refly.CodeDom.Expressions;

	public enum ForEachType
	{
		Snippet,
		Full
	}

	/// <summary>
	/// Summary description for ForEachStatement.
	/// </summary>
	public class ForEachStatement : Statement
	{
		private ITypeDeclaration localType;
		private string localName;
		private Expression collection;
		private StatementCollection body = new StatementCollection();

		private ForEachType outputType = ForEachType.Full;
		private bool enumeratorDisposable;

		public ForEachStatement(
			ITypeDeclaration localType,
			string localName,
			Expression collection,
			bool enumeratorDisposable
			)
		{
			if (localType==null)
				throw new ArgumentNullException("localType");
			if (localName==null)
				throw new ArgumentNullException("localName");
			if (collection ==null)
				throw new ArgumentNullException("collection");

			this.localType = localType;
			this.localName = localName;
			this.collection = collection;
			this.enumeratorDisposable = enumeratorDisposable;
		}

		public ITypeDeclaration LocalType
		{
			get
			{
				return this.localType;
			}
		}

		public String LocalName
		{
			get
			{
				return this.localName;
			}
		}

		public VariableReferenceExpression Local
		{
			get
			{
				return new VariableReferenceExpression(
					new VariableDeclarationStatement(this.LocalType,this.LocalName)
					);
			}
		}

		public Expression Collection
		{
			get
			{
				return this.collection;
			}
		}

		public StatementCollection Body
		{
			get
			{
				return this.body;
			}
		}

		public ForEachType OutputType
		{
			get
			{
				return this.outputType;
			}
		}

		public bool EnumeratorDisposable
		{
			get
			{
				return this.enumeratorDisposable;
			}
			set
			{
				this.enumeratorDisposable = value;
			}
		}

		public override void ToCodeDom(CodeStatementCollection statements)
		{
			StatementCollection sts = new StatementCollection();
			switch(this.outputType)
			{
				case ForEachType.Snippet:
					ToCodeDomSnippet(sts);
					break;
				default:
					ToCodeDomFull(sts);
					break;
			}
			sts.ToCodeDom(statements);
		}

		public void ToCodeDomSnippet(StatementCollection sts)
		{
			throw new Exception("not supported yet");
		}

		public void ToCodeDomFull(StatementCollection sts)
		{
			// create enumrator and initialize it
			sts.Add( Stm.Comment("<foreach>") );
			sts.Add( Stm.Comment("This loop mimics a foreach call. See C# programming language, pg 247") );
			sts.Add( Stm.Comment("Here, the enumerator is seale and does not implement IDisposable") );
			VariableDeclarationStatement enDecl = Stm.Var(
				typeof(IEnumerator),
				"enumerator",
				this.Collection.Method("GetEnumerator").Invoke()
				);
			sts.Add(enDecl);
			
			// get expression
			VariableReferenceExpression en = Expr.Var(enDecl);

			// iterate
			IterationStatement iter = Stm.While( en.Method("MoveNext").Invoke() );
			sts.Add(iter);
			// get local parameter
			VariableDeclarationStatement localDecl = Stm.Var(
				this.LocalType,
				this.LocalName
				);
			localDecl.InitExpression = (en.Prop("Current")).Cast(this.LocalType);
			iter.Statements.Add(localDecl);

			// add statements
			iter.Statements.Add(Stm.Comment("foreach body"));
			iter.Statements.AddRange(this.body);
			sts.Add( Stm.Comment("</foreach>") );
	
		}

	}
}
