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
	/// Abstract class for implementation members declarations.
	/// </summary>
	public abstract class ImplementationMemberDeclaration : MemberDeclaration
	{
		private ITypeDeclaration privateImplementationType = null;
		private TypeDeclarationCollection implementationTypes = new TypeDeclarationCollection();

		internal ImplementationMemberDeclaration(string name, Declaration declaringType)
			:base(name,declaringType)
		{}


		public ITypeDeclaration PrivateImplementationType
		{
			get
			{
				return this.privateImplementationType;
			}
			set
			{
				this.privateImplementationType = value;
			}
		}

		public TypeDeclarationCollection ImplementationTypes
		{
			get
			{
				return this.implementationTypes;
			}
		}

		protected void SetImplementationTypes(CodeMemberMethod member)
		{			
			if (this.privateImplementationType!=null)
				member.PrivateImplementationType = this.privateImplementationType.TypeReference;

			foreach(ITypeDeclaration td in this.implementationTypes)
			{
				member.ImplementationTypes.Add(td.TypeReference);
			}
		}

		protected void SetImplementationTypes(CodeMemberProperty member)
		{			
			if (this.privateImplementationType!=null)
				member.PrivateImplementationType = this.privateImplementationType.TypeReference;

			foreach(ITypeDeclaration td in this.implementationTypes)
			{
				member.ImplementationTypes.Add(td.TypeReference);
			}
		}

	}
}
