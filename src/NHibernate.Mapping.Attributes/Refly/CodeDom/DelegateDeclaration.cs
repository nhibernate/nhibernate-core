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
	/// <summary>
	/// A delegate declaration
	/// </summary>
	public class DelegateDeclaration : Declaration
	{
		private MethodSignature signature = new MethodSignature();

		internal DelegateDeclaration(string name, Declaration declaringType)
			:base(name,declaringType.Conformer)
		{
			this.Attributes = MemberAttributes.Public;
		}

		public MethodSignature Signature 
		{
			get {
				return this.signature;
			}
		}

		public CodeTypeMember ToCodeDom() 
		{
			CodeTypeDelegate d = new CodeTypeDelegate();
			d.Name  = this.Name;
			foreach(ParameterDeclaration p in signature.Parameters)
				d.Parameters.Add(p.ToCodeDom());
			d.ReturnType = signature.ReturnType.TypeReference;
			base.ToCodeDom(d);
			
			return d;
		}

    }
}
