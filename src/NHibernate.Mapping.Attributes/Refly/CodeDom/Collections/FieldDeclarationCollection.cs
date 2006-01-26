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
	/// <summary>
	/// A collection of elements of type FieldDeclaration
	/// </summary>
	public class FieldDeclarationCollection: System.Collections.CollectionBase
	{
		/// <summary>
		/// Initializes a new empty instance of the FieldDeclarationCollection class.
		/// </summary>
		public FieldDeclarationCollection()
		{
			// empty
		}

		/// <summary>
		/// Initializes a new instance of the FieldDeclarationCollection class, containing elements
		/// copied from an array.
		/// </summary>
		/// <param name="items">
		/// The array whose elements are to be added to the new FieldDeclarationCollection.
		/// </param>
		public FieldDeclarationCollection(FieldDeclaration[] items)
		{
			this.AddRange(items);
		}

		/// <summary>
		/// Initializes a new instance of the FieldDeclarationCollection class, containing elements
		/// copied from another instance of FieldDeclarationCollection
		/// </summary>
		/// <param name="items">
		/// The FieldDeclarationCollection whose elements are to be added to the new FieldDeclarationCollection.
		/// </param>
		public FieldDeclarationCollection(FieldDeclarationCollection items)
		{
			this.AddRange(items);
		}

		/// <summary>
		/// Adds the elements of an array to the end of this FieldDeclarationCollection.
		/// </summary>
		/// <param name="items">
		/// The array whose elements are to be added to the end of this FieldDeclarationCollection.
		/// </param>
		public virtual void AddRange(FieldDeclaration[] items)
		{
			foreach (FieldDeclaration item in items)
			{
				this.List.Add(item);
			}
		}

		/// <summary>
		/// Adds the elements of another FieldDeclarationCollection to the end of this FieldDeclarationCollection.
		/// </summary>
		/// <param name="items">
		/// The FieldDeclarationCollection whose elements are to be added to the end of this FieldDeclarationCollection.
		/// </param>
		public virtual void AddRange(FieldDeclarationCollection items)
		{
			foreach (FieldDeclaration item in items)
			{
				this.List.Add(item);
			}
		}

		/// <summary>
		/// Adds an instance of type FieldDeclaration to the end of this FieldDeclarationCollection.
		/// </summary>
		/// <param name="value">
		/// The FieldDeclaration to be added to the end of this FieldDeclarationCollection.
		/// </param>
		public virtual void Add(FieldDeclaration value)
		{
			this.List.Add(value);
		}

		/// <summary>
		/// Determines whether a specfic FieldDeclaration value is in this FieldDeclarationCollection.
		/// </summary>
		/// <param name="value">
		/// The FieldDeclaration value to locate in this FieldDeclarationCollection.
		/// </param>
		/// <returns>
		/// true if value is found in this FieldDeclarationCollection;
		/// false otherwise.
		/// </returns>
		public virtual bool Contains(FieldDeclaration value)
		{
			return this.List.Contains(value);
		}

		/// <summary>
		/// Return the zero-based index of the first occurrence of a specific value
		/// in this FieldDeclarationCollection
		/// </summary>
		/// <param name="value">
		/// The FieldDeclaration value to locate in the FieldDeclarationCollection.
		/// </param>
		/// <returns>
		/// The zero-based index of the first occurrence of the _ELEMENT value if found;
		/// -1 otherwise.
		/// </returns>
		public virtual int IndexOf(FieldDeclaration value)
		{
			return this.List.IndexOf(value);
		}

		/// <summary>
		/// Inserts an element into the FieldDeclarationCollection at the specified index
		/// </summary>
		/// <param name="index">
		/// The index at which the FieldDeclaration is to be inserted.
		/// </param>
		/// <param name="value">
		/// The FieldDeclaration to insert.
		/// </param>
		public virtual void Insert(int index, FieldDeclaration value)
		{
			this.List.Insert(index, value);
		}

		/// <summary>
		/// Gets or sets the FieldDeclaration at the given index in this FieldDeclarationCollection.
		/// </summary>
		public virtual FieldDeclaration this[int index]
		{
			get
			{
				return (FieldDeclaration) this.List[index];
			}
			set
			{
				this.List[index] = value;
			}
		}

		/// <summary>
		/// Removes the first occurrence of a specific FieldDeclaration from this FieldDeclarationCollection.
		/// </summary>
		/// <param name="value">
		/// The FieldDeclaration value to remove from this FieldDeclarationCollection.
		/// </param>
		public virtual void Remove(FieldDeclaration value)
		{
			this.List.Remove(value);
		}

		/// <summary>
		/// Type-specific enumeration class, used by FieldDeclarationCollection.GetEnumerator.
		/// </summary>
		public class Enumerator: System.Collections.IEnumerator
		{
			private System.Collections.IEnumerator wrapped;

			public Enumerator(FieldDeclarationCollection collection)
			{
				this.wrapped = ((System.Collections.CollectionBase)collection).GetEnumerator();
			}

			public FieldDeclaration Current
			{
				get
				{
					return (FieldDeclaration) (this.wrapped.Current);
				}
			}

			object System.Collections.IEnumerator.Current
			{
				get
				{
					return (FieldDeclaration) (this.wrapped.Current);
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
		/// Returns an enumerator that can iterate through the elements of this FieldDeclarationCollection.
		/// </summary>
		/// <returns>
		/// An object that implements System.Collections.IEnumerator.
		/// </returns>        
		public new virtual FieldDeclarationCollection.Enumerator GetEnumerator()
		{
			return new FieldDeclarationCollection.Enumerator(this);
		}
	}

}
