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
	/// A collection of elements of type ThrowedExceptionDeclaration
	/// </summary>
	public class ThrowedExceptionDeclarationCollection: System.Collections.CollectionBase
	{
		/// <summary>
		/// Initializes a new empty instance of the ThrowedExceptionDeclarationCollection class.
		/// </summary>
		public ThrowedExceptionDeclarationCollection()
		{
			// empty
		}

		/// <summary>
		/// Initializes a new instance of the ThrowedExceptionDeclarationCollection class, containing elements
		/// copied from an array.
		/// </summary>
		/// <param name="items">
		/// The array whose elements are to be added to the new ThrowedExceptionDeclarationCollection.
		/// </param>
		public ThrowedExceptionDeclarationCollection(ThrowedExceptionDeclaration[] items)
		{
			this.AddRange(items);
		}

		/// <summary>
		/// Initializes a new instance of the ThrowedExceptionDeclarationCollection class, containing elements
		/// copied from another instance of ThrowedExceptionDeclarationCollection
		/// </summary>
		/// <param name="items">
		/// The ThrowedExceptionDeclarationCollection whose elements are to be added to the new ThrowedExceptionDeclarationCollection.
		/// </param>
		public ThrowedExceptionDeclarationCollection(ThrowedExceptionDeclarationCollection items)
		{
			this.AddRange(items);
		}

		/// <summary>
		/// Adds the elements of an array to the end of this ThrowedExceptionDeclarationCollection.
		/// </summary>
		/// <param name="items">
		/// The array whose elements are to be added to the end of this ThrowedExceptionDeclarationCollection.
		/// </param>
		public virtual void AddRange(ThrowedExceptionDeclaration[] items)
		{
			foreach (ThrowedExceptionDeclaration item in items)
			{
				this.List.Add(item);
			}
		}

		/// <summary>
		/// Adds the elements of another ThrowedExceptionDeclarationCollection to the end of this ThrowedExceptionDeclarationCollection.
		/// </summary>
		/// <param name="items">
		/// The ThrowedExceptionDeclarationCollection whose elements are to be added to the end of this ThrowedExceptionDeclarationCollection.
		/// </param>
		public virtual void AddRange(ThrowedExceptionDeclarationCollection items)
		{
			foreach (ThrowedExceptionDeclaration item in items)
			{
				this.List.Add(item);
			}
		}

		/// <summary>
		/// Adds an instance of type ThrowedExceptionDeclaration to the end of this ThrowedExceptionDeclarationCollection.
		/// </summary>
		/// <param name="value">
		/// The ThrowedExceptionDeclaration to be added to the end of this ThrowedExceptionDeclarationCollection.
		/// </param>
		public virtual void Add(ThrowedExceptionDeclaration value)
		{
			this.List.Add(value);
		}

		/// <summary>
		/// Determines whether a specfic ThrowedExceptionDeclaration value is in this ThrowedExceptionDeclarationCollection.
		/// </summary>
		/// <param name="value">
		/// The ThrowedExceptionDeclaration value to locate in this ThrowedExceptionDeclarationCollection.
		/// </param>
		/// <returns>
		/// true if value is found in this ThrowedExceptionDeclarationCollection;
		/// false otherwise.
		/// </returns>
		public virtual bool Contains(ThrowedExceptionDeclaration value)
		{
			return this.List.Contains(value);
		}

		/// <summary>
		/// Return the zero-based index of the first occurrence of a specific value
		/// in this ThrowedExceptionDeclarationCollection
		/// </summary>
		/// <param name="value">
		/// The ThrowedExceptionDeclaration value to locate in the ThrowedExceptionDeclarationCollection.
		/// </param>
		/// <returns>
		/// The zero-based index of the first occurrence of the _ELEMENT value if found;
		/// -1 otherwise.
		/// </returns>
		public virtual int IndexOf(ThrowedExceptionDeclaration value)
		{
			return this.List.IndexOf(value);
		}

		/// <summary>
		/// Inserts an element into the ThrowedExceptionDeclarationCollection at the specified index
		/// </summary>
		/// <param name="index">
		/// The index at which the ThrowedExceptionDeclaration is to be inserted.
		/// </param>
		/// <param name="value">
		/// The ThrowedExceptionDeclaration to insert.
		/// </param>
		public virtual void Insert(int index, ThrowedExceptionDeclaration value)
		{
			this.List.Insert(index, value);
		}

		/// <summary>
		/// Gets or sets the ThrowedExceptionDeclaration at the given index in this ThrowedExceptionDeclarationCollection.
		/// </summary>
		public virtual ThrowedExceptionDeclaration this[int index]
		{
			get
			{
				return (ThrowedExceptionDeclaration) this.List[index];
			}
			set
			{
				this.List[index] = value;
			}
		}

		/// <summary>
		/// Removes the first occurrence of a specific ThrowedExceptionDeclaration from this ThrowedExceptionDeclarationCollection.
		/// </summary>
		/// <param name="value">
		/// The ThrowedExceptionDeclaration value to remove from this ThrowedExceptionDeclarationCollection.
		/// </param>
		public virtual void Remove(ThrowedExceptionDeclaration value)
		{
			this.List.Remove(value);
		}

		/// <summary>
		/// Type-specific enumeration class, used by ThrowedExceptionDeclarationCollection.GetEnumerator.
		/// </summary>
		public class Enumerator: System.Collections.IEnumerator
		{
			private System.Collections.IEnumerator wrapped;

			public Enumerator(ThrowedExceptionDeclarationCollection collection)
			{
				this.wrapped = ((System.Collections.CollectionBase)collection).GetEnumerator();
			}

			public ThrowedExceptionDeclaration Current
			{
				get
				{
					return (ThrowedExceptionDeclaration) (this.wrapped.Current);
				}
			}

			object System.Collections.IEnumerator.Current
			{
				get
				{
					return (ThrowedExceptionDeclaration) (this.wrapped.Current);
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
		/// Returns an enumerator that can iterate through the elements of this ThrowedExceptionDeclarationCollection.
		/// </summary>
		/// <returns>
		/// An object that implements System.Collections.IEnumerator.
		/// </returns>        
		public new virtual ThrowedExceptionDeclarationCollection.Enumerator GetEnumerator()
		{
			return new ThrowedExceptionDeclarationCollection.Enumerator(this);
		}
	}

}
