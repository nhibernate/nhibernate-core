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
using System.Collections.Specialized;

namespace Refly.CodeDom
{
	using Refly.CodeDom.Collections;
	using Refly.CodeDom.Expressions;
	using Refly.CodeDom.Statements;

	/// <summary>
	/// An attribute argument.
	/// </summary>
	public class AttributeArgument
	{
		private string name;
		private Expression value;
		
		internal AttributeArgument(string name, Expression value)
		{
			if (name==null)
				throw new ArgumentNullException("name");
			if (value==null)
				throw new ArgumentNullException("value");
			this.name=name;
			this.value=value;
		}
		
		public string Name
		{
			get
			{
				return this.name;
			}
		}
		
		public Expression Value
		{
			get
			{
				return this.value;
			}
			set
			{
				if (value==null)
					throw new ArgumentNullException("value");
				this.value=value;
			}
		}
		
		public System.CodeDom.CodeAttributeArgument ToCodeDom()
		{
			return new System.CodeDom.CodeAttributeArgument(this.Name, this.Value.ToCodeDom() );
		}
	}
}
