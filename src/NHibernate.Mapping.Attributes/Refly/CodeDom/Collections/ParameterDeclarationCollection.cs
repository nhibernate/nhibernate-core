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
	/// A collection of elements of type ParameterDeclaration
	/// </summary>
	public class ParameterDeclarationCollection: System.Collections.CollectionBase
	{
		/// <summary>
		/// Initializes a new empty instance of the ParameterDeclarationCollection class.
		/// </summary>
		public ParameterDeclarationCollection()
		{
			// empty
		}

		/// <summary>
		/// Initializes a new instance of the ParameterDeclarationCollection class, containing elements
		/// copied from an array.
		/// </summary>
		/// <param name="items">
		/// The array whose elements are to be added to the new ParameterDeclarationCollection.
		/// </param>
		public ParameterDeclarationCollection(ParameterDeclaration[] items)
		{
			this.AddRange(items);
		}

		/// <summary>
		/// Initializes a new instance of the ParameterDeclarationCollection class, containing elements
		/// copied from another instance of ParameterDeclarationCollection
		/// </summary>
		/// <param name="items">
		/// The ParameterDeclarationCollection whose elements are to be added to the new ParameterDeclarationCollection.
		/// </param>
		public ParameterDeclarationCollection(ParameterDeclarationCollection items)
		{
			this.AddRange(items);
		}

		/// <summary>
		/// Adds the elements of an array to the end of this ParameterDeclarationCollection.
		/// </summary>
		/// <param name="items">
		/// The array whose elements are to be added to the end of this ParameterDeclarationCollection.
		/// </param>
		public virtual void AddRange(ParameterDeclaration[] items)
		{
			foreach (ParameterDeclaration item in items)
			{
				this.List.Add(item);
			}
		}

		/// <summary>
		/// Adds the elements of another ParameterDeclarationCollection to the end of this ParameterDeclarationCollection.
		/// </summary>
		/// <param name="items">
		/// The ParameterDeclarationCollection whose elements are to be added to the end of this ParameterDeclarationCollection.
		/// </param>
		public virtual void AddRange(ParameterDeclarationCollection items)
		{
			foreach (ParameterDeclaration item in items)
			{
				this.List.Add(item);
			}
		}

		public virtual ParameterDeclaration Add(ITypeDeclaration type, string name, bool nonNull)
		{
			ParameterDeclaration pm =new ParameterDeclaration(type,name,nonNull);
			this.List.Add(pm);
			return pm;
		}

		public virtual ParameterDeclaration Add(ITypeDeclaration type, string name)
		{
			return this.Add(type,name,true);
		}

		public virtual ParameterDeclaration Add(Type type, string name)
		{
			return this.Add(type,name,true);
		}

		public virtual ParameterDeclaration Add(Type type, string name, bool nonNull)
		{
			return this.Add(new TypeTypeDeclaration(type),name);
		}

		public virtual ParameterDeclaration Add(String type, string name)
		{
			return this.Add(type,name,true);
		}

		public virtual ParameterDeclaration Add(String type, string name, bool nonNull)
		{
			return this.Add(new StringTypeDeclaration(type),name,nonNull);
		}

		/// <summary>
		/// Determines whether a specfic ParameterDeclaration value is in this ParameterDeclarationCollection.
		/// </summary>
		/// <param name="value">
		/// The ParameterDeclaration value to locate in this ParameterDeclarationCollection.
		/// </param>
		/// <returns>
		/// true if value is found in this ParameterDeclarationCollection;
		/// false otherwise.
		/// </returns>
		public virtual bool Contains(ParameterDeclaration value)
		{
			return this.List.Contains(value);
		}

		/// <summary>
		/// Return the zero-based index of the first occurrence of a specific value
		/// in this ParameterDeclarationCollection
		/// </summary>
		/// <param name="value">
		/// The ParameterDeclaration value to locate in the ParameterDeclarationCollection.
		/// </param>
		/// <returns>
		/// The zero-based index of the first occurrence of the _ELEMENT value if found;
		/// -1 otherwise.
		/// </returns>
		public virtual int IndexOf(ParameterDeclaration value)
		{
			return this.List.IndexOf(value);
		}

		/// <summary>
		/// Inserts an element into the ParameterDeclarationCollection at the specified index
		/// </summary>
		/// <param name="index">
		/// The index at which the ParameterDeclaration is to be inserted.
		/// </param>
		/// <param name="value">
		/// The ParameterDeclaration to insert.
		/// </param>
		public virtual void Insert(int index, ParameterDeclaration value)
		{
			this.List.Insert(index, value);
		}

		/// <summary>
		/// Gets or sets the ParameterDeclaration at the given index in this ParameterDeclarationCollection.
		/// </summary>
		public virtual ParameterDeclaration this[int index]
		{
			get
			{
				return (ParameterDeclaration) this.List[index];
			}
			set
			{
				this.List[index] = value;
			}
		}

		/// <summary>
		/// Removes the first occurrence of a specific ParameterDeclaration from this ParameterDeclarationCollection.
		/// </summary>
		/// <param name="value">
		/// The ParameterDeclaration value to remove from this ParameterDeclarationCollection.
		/// </param>
		public virtual void Remove(ParameterDeclaration value)
		{
			this.List.Remove(value);
		}

		/// <summary>
		/// Type-specific enumeration class, used by ParameterDeclarationCollection.GetEnumerator.
		/// </summary>
		public class Enumerator: System.Collections.IEnumerator
		{
			private System.Collections.IEnumerator wrapped;

			public Enumerator(ParameterDeclarationCollection collection)
			{
				this.wrapped = ((System.Collections.CollectionBase)collection).GetEnumerator();
			}

			public ParameterDeclaration Current
			{
				get
				{
					return (ParameterDeclaration) (this.wrapped.Current);
				}
			}

			object System.Collections.IEnumerator.Current
			{
				get
				{
					return (ParameterDeclaration) (this.wrapped.Current);
				}
			}

			public bool MoveNext()
			{
				return this.wrapped.MoveNext();
			}

			public void Reset()
			{
				this.wrapped.Reset();
			}
		}

		/// <summary>
		/// Returns an enumerator that can iterate through the elements of this ParameterDeclarationCollection.
		/// </summary>
		/// <returns>
		/// An object that implements System.Collections.IEnumerator.
		/// </returns>        
		public new virtual ParameterDeclarationCollection.Enumerator GetEnumerator()
		{
			return new ParameterDeclarationCollection.Enumerator(this);
		}
	}

}
