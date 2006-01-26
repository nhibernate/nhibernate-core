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

namespace Refly.CodeDom.Expressions
{
	/// <summary>
	/// Summary description for PropertyReferenceExpression.
	/// </summary>
	public class PropertyReferenceExpression : Expression
	{
		private Expression target;
		private PropertyDeclaration property;
		public PropertyReferenceExpression(Expression target,PropertyDeclaration property)
		{
			if (target==null)
				throw new ArgumentNullException("target");
			if (property==null)
				throw new ArgumentNullException("property");

			this.target = target;
			this.property = property;
		}

		public Expression Target
		{
			get
			{
				return this.target;
			}
		}

		public PropertyDeclaration Property
		{
			get
			{
				return this.property;
			}
		}

		public override System.CodeDom.CodeExpression ToCodeDom()
		{
			return new CodePropertyReferenceExpression(
				this.target.ToCodeDom(),
				this.property.Name
				);
		}
	}
}
