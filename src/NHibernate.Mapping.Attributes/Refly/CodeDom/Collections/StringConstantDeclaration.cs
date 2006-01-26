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

namespace Refly.CodeDom.Collections
{
	using Refly.CodeDom;
	/// <summary>
	/// A dictionary with keys of type String and values of type ConstantDeclaration
	/// </summary>
	public class StringConstantDeclaration: System.Collections.DictionaryBase
	{
		/// <summary>
		/// Initializes a new empty instance of the StringConstantDeclaration class
		/// </summary>
		public StringConstantDeclaration()
		{
			// empty
		}

		/// <summary>
		/// Gets or sets the ConstantDeclaration associated with the given String
		/// </summary>
		/// <param name="key">
		/// The String whose value to get or set.
		/// </param>
		public virtual ConstantDeclaration this[String key]
		{
			get
			{
				return (ConstantDeclaration) this.Dictionary[key];
			}
		}

		/// <summary>
		/// Adds an element with the specified key and value to this StringConstantDeclaration.
		/// </summary>
		/// <param name="key">
		/// The String key of the element to add.
		/// </param>
		/// <param name="value">
		/// The ConstantDeclaration value of the element to add.
		/// </param>
		public virtual void Add(ConstantDeclaration value)
		{
			this.Dictionary.Add(value.Name, value);
		}

		/// <summary>
		/// Determines whether this StringConstantDeclaration contains a specific key.
		/// </summary>
		/// <param name="key">
		/// The String key to locate in this StringConstantDeclaration.
		/// </param>
		/// <returns>
		/// true if this StringConstantDeclaration contains an element with the specified key;
		/// otherwise, false.
		/// </returns>
		public virtual bool Contains(String key)
		{
			return this.Dictionary.Contains(key);
		}

		/// <summary>
		/// Removes the element with the specified key from this StringConstantDeclaration.
		/// </summary>
		/// <param name="key">
		/// The String key of the element to remove.
		/// </param>
		public virtual void Remove(String key)
		{
			this.Dictionary.Remove(key);
		}

		/// <summary>
		/// Gets a collection containing the keys in this StringConstantDeclaration.
		/// </summary>
		public virtual System.Collections.ICollection Keys
		{
			get
			{
				return this.Dictionary.Keys;
			}
		}

		/// <summary>
		/// Gets a collection containing the values in this StringConstantDeclaration.
		/// </summary>
		public virtual System.Collections.ICollection Values
		{
			get
			{
				return this.Dictionary.Values;
			}
		}
	}
}
