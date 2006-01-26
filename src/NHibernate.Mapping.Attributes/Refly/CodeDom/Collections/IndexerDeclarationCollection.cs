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

namespace Refly.CodeDom.Collections
{
	/// <summary>
	/// A collection of elements of type IndexerDeclaration
	/// </summary>
	public class IndexerDeclarationCollection: System.Collections.CollectionBase
	{
		/// <summary>
		/// Initializes a new empty instance of the IndexerDeclarationCollection class.
		/// </summary>
		public IndexerDeclarationCollection()
		{
			// empty
		}

		/// <summary>
		/// Initializes a new instance of the IndexerDeclarationCollection class, containing elements
		/// copied from an array.
		/// </summary>
		/// <param name="items">
		/// The array whose elements are to be added to the new IndexerDeclarationCollection.
		/// </param>
		public IndexerDeclarationCollection(IndexerDeclaration[] items)
		{
			this.AddRange(items);
		}

		/// <summary>
		/// Initializes a new instance of the IndexerDeclarationCollection class, containing elements
		/// copied from another instance of IndexerDeclarationCollection
		/// </summary>
		/// <param name="items">
		/// The IndexerDeclarationCollection whose elements are to be added to the new IndexerDeclarationCollection.
		/// </param>
		public IndexerDeclarationCollection(IndexerDeclarationCollection items)
		{
			this.AddRange(items);
		}

		/// <summary>
		/// Adds the elements of an array to the end of this IndexerDeclarationCollection.
		/// </summary>
		/// <param name="items">
		/// The array whose elements are to be added to the end of this IndexerDeclarationCollection.
		/// </param>
		public virtual void AddRange(IndexerDeclaration[] items)
		{
			foreach (IndexerDeclaration item in items)
			{
				this.List.Add(item);
			}
		}

		/// <summary>
		/// Adds the elements of another IndexerDeclarationCollection to the end of this IndexerDeclarationCollection.
		/// </summary>
		/// <param name="items">
		/// The IndexerDeclarationCollection whose elements are to be added to the end of this IndexerDeclarationCollection.
		/// </param>
		public virtual void AddRange(IndexerDeclarationCollection items)
		{
			foreach (IndexerDeclaration item in items)
			{
				this.List.Add(item);
			}
		}

		/// <summary>
		/// Adds an instance of type IndexerDeclaration to the end of this IndexerDeclarationCollection.
		/// </summary>
		/// <param name="value">
		/// The IndexerDeclaration to be added to the end of this IndexerDeclarationCollection.
		/// </param>
		public virtual void Add(IndexerDeclaration value)
		{
			this.List.Add(value);
		}

		/// <summary>
		/// Determines whether a specfic IndexerDeclaration value is in this IndexerDeclarationCollection.
		/// </summary>
		/// <param name="value">
		/// The IndexerDeclaration value to locate in this IndexerDeclarationCollection.
		/// </param>
		/// <returns>
		/// true if value is found in this IndexerDeclarationCollection;
		/// false otherwise.
		/// </returns>
		public virtual bool Contains(IndexerDeclaration value)
		{
			return this.List.Contains(value);
		}

		/// <summary>
		/// Return the zero-based index of the first occurrence of a specific value
		/// in this IndexerDeclarationCollection
		/// </summary>
		/// <param name="value">
		/// The IndexerDeclaration value to locate in the IndexerDeclarationCollection.
		/// </param>
		/// <returns>
		/// The zero-based index of the first occurrence of the _ELEMENT value if found;
		/// -1 otherwise.
		/// </returns>
		public virtual int IndexOf(IndexerDeclaration value)
		{
			return this.List.IndexOf(value);
		}

		/// <summary>
		/// Inserts an element into the IndexerDeclarationCollection at the specified index
		/// </summary>
		/// <param name="index">
		/// The index at which the IndexerDeclaration is to be inserted.
		/// </param>
		/// <param name="value">
		/// The IndexerDeclaration to insert.
		/// </param>
		public virtual void Insert(int index, IndexerDeclaration value)
		{
			this.List.Insert(index, value);
		}

		/// <summary>
		/// Gets or sets the IndexerDeclaration at the given index in this IndexerDeclarationCollection.
		/// </summary>
		public virtual IndexerDeclaration this[int index]
		{
			get
			{
				return (IndexerDeclaration) this.List[index];
			}
			set
			{
				this.List[index] = value;
			}
		}

		/// <summary>
		/// Removes the first occurrence of a specific IndexerDeclaration from this IndexerDeclarationCollection.
		/// </summary>
		/// <param name="value">
		/// The IndexerDeclaration value to remove from this IndexerDeclarationCollection.
		/// </param>
		public virtual void Remove(IndexerDeclaration value)
		{
			this.List.Remove(value);
		}

		/// <summary>
		/// Type-specific enumeration class, used by IndexerDeclarationCollection.GetEnumerator.
		/// </summary>
		public class Enumerator: System.Collections.IEnumerator
		{
			private System.Collections.IEnumerator wrapped;

			public Enumerator(IndexerDeclarationCollection collection)
			{
				this.wrapped = ((System.Collections.CollectionBase)collection).GetEnumerator();
			}

			public IndexerDeclaration Current
			{
				get
				{
					return (IndexerDeclaration) (this.wrapped.Current);
				}
			}

			object System.Collections.IEnumerator.Current
			{
				get
				{
					return (IndexerDeclaration) (this.wrapped.Current);
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
		/// Returns an enumerator that can iterate through the elements of this IndexerDeclarationCollection.
		/// </summary>
		/// <returns>
		/// An object that implements System.Collections.IEnumerator.
		/// </returns>        
		public new virtual IndexerDeclarationCollection.Enumerator GetEnumerator()
		{
			return new IndexerDeclarationCollection.Enumerator(this);
		}
	}

}
