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
	/// A collection of elements of type ConstructorDeclaration
	/// </summary>
	public class ConstructorDeclarationCollection: System.Collections.CollectionBase
	{
		/// <summary>
		/// Initializes a new empty instance of the ConstructorDeclarationCollection class.
		/// </summary>
		public ConstructorDeclarationCollection()
		{
			// empty
		}

		/// <summary>
		/// Initializes a new instance of the ConstructorDeclarationCollection class, containing elements
		/// copied from an array.
		/// </summary>
		/// <param name="items">
		/// The array whose elements are to be added to the new ConstructorDeclarationCollection.
		/// </param>
		public ConstructorDeclarationCollection(ConstructorDeclaration[] items)
		{
			this.AddRange(items);
		}

		/// <summary>
		/// Initializes a new instance of the ConstructorDeclarationCollection class, containing elements
		/// copied from another instance of ConstructorDeclarationCollection
		/// </summary>
		/// <param name="items">
		/// The ConstructorDeclarationCollection whose elements are to be added to the new ConstructorDeclarationCollection.
		/// </param>
		public ConstructorDeclarationCollection(ConstructorDeclarationCollection items)
		{
			this.AddRange(items);
		}

		/// <summary>
		/// Adds the elements of an array to the end of this ConstructorDeclarationCollection.
		/// </summary>
		/// <param name="items">
		/// The array whose elements are to be added to the end of this ConstructorDeclarationCollection.
		/// </param>
		public virtual void AddRange(ConstructorDeclaration[] items)
		{
			foreach (ConstructorDeclaration item in items)
			{
				this.List.Add(item);
			}
		}

		/// <summary>
		/// Adds the elements of another ConstructorDeclarationCollection to the end of this ConstructorDeclarationCollection.
		/// </summary>
		/// <param name="items">
		/// The ConstructorDeclarationCollection whose elements are to be added to the end of this ConstructorDeclarationCollection.
		/// </param>
		public virtual void AddRange(ConstructorDeclarationCollection items)
		{
			foreach (ConstructorDeclaration item in items)
			{
				this.List.Add(item);
			}
		}

		/// <summary>
		/// Adds an instance of type ConstructorDeclaration to the end of this ConstructorDeclarationCollection.
		/// </summary>
		/// <param name="value">
		/// The ConstructorDeclaration to be added to the end of this ConstructorDeclarationCollection.
		/// </param>
		public virtual void Add(ConstructorDeclaration value)
		{
			this.List.Add(value);
		}

		/// <summary>
		/// Determines whether a specfic ConstructorDeclaration value is in this ConstructorDeclarationCollection.
		/// </summary>
		/// <param name="value">
		/// The ConstructorDeclaration value to locate in this ConstructorDeclarationCollection.
		/// </param>
		/// <returns>
		/// true if value is found in this ConstructorDeclarationCollection;
		/// false otherwise.
		/// </returns>
		public virtual bool Contains(ConstructorDeclaration value)
		{
			return this.List.Contains(value);
		}

		/// <summary>
		/// Return the zero-based index of the first occurrence of a specific value
		/// in this ConstructorDeclarationCollection
		/// </summary>
		/// <param name="value">
		/// The ConstructorDeclaration value to locate in the ConstructorDeclarationCollection.
		/// </param>
		/// <returns>
		/// The zero-based index of the first occurrence of the _ELEMENT value if found;
		/// -1 otherwise.
		/// </returns>
		public virtual int IndexOf(ConstructorDeclaration value)
		{
			return this.List.IndexOf(value);
		}

		/// <summary>
		/// Inserts an element into the ConstructorDeclarationCollection at the specified index
		/// </summary>
		/// <param name="index">
		/// The index at which the ConstructorDeclaration is to be inserted.
		/// </param>
		/// <param name="value">
		/// The ConstructorDeclaration to insert.
		/// </param>
		public virtual void Insert(int index, ConstructorDeclaration value)
		{
			this.List.Insert(index, value);
		}

		/// <summary>
		/// Gets or sets the ConstructorDeclaration at the given index in this ConstructorDeclarationCollection.
		/// </summary>
		public virtual ConstructorDeclaration this[int index]
		{
			get
			{
				return (ConstructorDeclaration) this.List[index];
			}
			set
			{
				this.List[index] = value;
			}
		}

		/// <summary>
		/// Removes the first occurrence of a specific ConstructorDeclaration from this ConstructorDeclarationCollection.
		/// </summary>
		/// <param name="value">
		/// The ConstructorDeclaration value to remove from this ConstructorDeclarationCollection.
		/// </param>
		public virtual void Remove(ConstructorDeclaration value)
		{
			this.List.Remove(value);
		}

		/// <summary>
		/// Type-specific enumeration class, used by ConstructorDeclarationCollection.GetEnumerator.
		/// </summary>
		public class Enumerator: System.Collections.IEnumerator
		{
			private System.Collections.IEnumerator wrapped;

			public Enumerator(ConstructorDeclarationCollection collection)
			{
				this.wrapped = ((System.Collections.CollectionBase)collection).GetEnumerator();
			}

			public ConstructorDeclaration Current
			{
				get
				{
					return (ConstructorDeclaration) (this.wrapped.Current);
				}
			}

			object System.Collections.IEnumerator.Current
			{
				get
				{
					return (ConstructorDeclaration) (this.wrapped.Current);
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
		/// Returns an enumerator that can iterate through the elements of this ConstructorDeclarationCollection.
		/// </summary>
		/// <returns>
		/// An object that implements System.Collections.IEnumerator.
		/// </returns>        
		public new virtual ConstructorDeclarationCollection.Enumerator GetEnumerator()
		{
			return new ConstructorDeclarationCollection.Enumerator(this);
		}
	}

}
