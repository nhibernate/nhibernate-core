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
	/// <summary>
	/// A index declaration.
	/// </summary>
	public class IndexerDeclaration : ImplementationMemberDeclaration
	{
		private MethodSignature signature = new MethodSignature();
		private StatementCollection getStatement = new StatementCollection();
		private StatementCollection setStatement = new StatementCollection();
		private ThrowedExceptionDeclarationCollection getExceptions = new ThrowedExceptionDeclarationCollection();
		private ThrowedExceptionDeclarationCollection setExceptions = new ThrowedExceptionDeclarationCollection();

		internal IndexerDeclaration(
			Declaration declaringType, 
			ITypeDeclaration returnType)
			:base("Item",declaringType)
		{
			this.signature.ReturnType = returnType;
			this.Attributes = MemberAttributes.Public;
		}

		public MethodSignature Signature
		{
			get
			{
				return this.signature;
			}
		}


		public StatementCollection Get
		{
			get
			{
				return this.getStatement;
			}
		}
	
		public ThrowedExceptionDeclarationCollection GetExceptions
		{
			get
			{
				return this.getExceptions;
			}
		}

		public StatementCollection Set
		{
			get
			{
				return this.setStatement;
			}
		}

	
		public ThrowedExceptionDeclarationCollection SetExceptions
		{
			get
			{
				return this.setExceptions;
			}
		}

		public override CodeTypeMember ToCodeDom()
		{
			CodeMemberProperty p = new CodeMemberProperty();

			// name
			p.Name = this.Conformer.NormalizeMember(this.Name,this.Attributes);
			this.Signature.ToCodeDom(p);

			base.ToCodeDom(p);
			this.SetImplementationTypes(p);


			// set or get
			if (this.getStatement!=null)
			{
				foreach(ThrowedExceptionDeclaration ex in this.getExceptions)
				{
					this.Doc.AddException(ex.ExceptionType,
						"get property, " + ex.Description);
				}

				this.getStatement.ToCodeDom(p.GetStatements);
				p.HasGet = true;
			}
			if (this.setStatement!=null)
			{
				foreach(ThrowedExceptionDeclaration ex in this.setExceptions)
				{
					this.Doc.AddException(ex.ExceptionType,
						"set property, " + ex.Description);
				}

				this.setStatement.ToCodeDom(p.SetStatements);
				p.HasSet = true;
			}

			return p;
		}
	}
}
