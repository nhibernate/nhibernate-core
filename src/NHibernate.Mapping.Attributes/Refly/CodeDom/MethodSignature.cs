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
	/// A method signature
	/// </summary>
	public class MethodSignature
	{
		private ParameterDeclarationCollection parameters = new ParameterDeclarationCollection();
		private ITypeDeclaration returnType = new TypeTypeDeclaration(typeof(void));

		public ParameterDeclarationCollection Parameters
		{
			get
			{
				return this.parameters;
			}
		}

		public ITypeDeclaration ReturnType
		{
			get
			{
				return this.returnType;
			}
			set
			{
				this.returnType = value;
			}
		}

		public ArgumentReferenceExpression this[string name]
		{
			get
			{
				if (name==null)
					throw new ArgumentNullException("name");
				foreach(ParameterDeclaration p in this.parameters)
				{
					if (p.Name==name)
						return new ArgumentReferenceExpression(p);
				}
				throw new ArgumentException("could not find " + name);
			}
		}

		public void ToCodeDom(CodeConstructor ctr)
		{
			foreach(ParameterDeclaration p in this.Parameters)
			{
				ctr.Parameters.Add(p.ToCodeDom());
			}
		}

		public void ToCodeDom(CodeMemberMethod m)
		{
			m.ReturnType = this.ReturnType.TypeReference;
			foreach(ParameterDeclaration p in this.Parameters)
			{
				m.Parameters.Add(p.ToCodeDom());
			}
		}

		public void ToCodeDom(CodeMemberProperty m)
		{
			m.Type = this.ReturnType.TypeReference;
			foreach(ParameterDeclaration p in this.Parameters)
			{
				m.Parameters.Add(p.ToCodeDom());
			}
		}

	}
}
