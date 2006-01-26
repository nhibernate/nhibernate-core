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
	/// A collection of elements of type PropertyDeclaration
	/// </summary>
	public class PropertyDeclarationCollection: System.Collections.CollectionBase
	{
		/// <summary>
		/// Initializes a new empty instance of the PropertyDeclarationCollection class.
		/// </summary>
		public PropertyDeclarationCollection()
		{
			// empty
		}

		/// <summary>
		/// Initializes a new instance of the PropertyDeclarationCollection class, containing elements
		/// copied from an array.
		/// </summary>
		/// <param name="items">
		/// The array whose elements are to be added to the new PropertyDeclarationCollection.
		/// </param>
		public PropertyDeclarationCollection(PropertyDeclaration[] items)
		{
			this.AddRange(items);
		}

		/// <summary>
		/// Initializes a new instance of the PropertyDeclarationCollection class, containing elements
		/// copied from another instance of PropertyDeclarationCollection
		/// </summary>
		/// <param name="items">
		/// The PropertyDeclarationCollection whose elements are to be added to the new PropertyDeclarationCollection.
		/// </param>
		public PropertyDeclarationCollection(PropertyDeclarationCollection items)
		{
			this.AddRange(items);
		}

		/// <summary>
		/// Adds the elements of an array to the end of this PropertyDeclarationCollection.
		/// </summary>
		/// <param name="items">
		/// The array whose elements are to be added to the end of this PropertyDeclarationCollection.
		/// </param>
		public virtual void AddRange(PropertyDeclaration[] items)
		{
			foreach (PropertyDeclaration item in items)
			{
				this.List.Add(item);
			}
		}

		/// <summary>
		/// Adds the elements of another PropertyDeclarationCollection to the end of this PropertyDeclarationCollection.
		/// </summary>
		/// <param name="items">
		/// The PropertyDeclarationCollection whose elements are to be added to the end of this PropertyDeclarationCollection.
		/// </param>
		public virtual void AddRange(PropertyDeclarationCollection items)
		{
			foreach (PropertyDeclaration item in items)
			{
				this.List.Add(item);
			}
		}

		/// <summary>
		/// Adds an instance of type PropertyDeclaration to the end of this PropertyDeclarationCollection.
		/// </summary>
		/// <param name="value">
		/// The PropertyDeclaration to be added to the end of this PropertyDeclarationCollection.
		/// </param>
		public virtual void Add(PropertyDeclaration value)
		{
			this.List.Add(value);
		}

		/// <summary>
		/// Determines whether a specfic PropertyDeclaration value is in this PropertyDeclarationCollection.
		/// </summary>
		/// <param name="value">
		/// The PropertyDeclaration value to locate in this PropertyDeclarationCollection.
		/// </param>
		/// <returns>
		/// true if value is found in this PropertyDeclarationCollection;
		/// false otherwise.
		/// </returns>
		public virtual bool Contains(PropertyDeclaration value)
		{
			return this.List.Contains(value);
		}

		/// <summary>
		/// Return the zero-based index of the first occurrence of a specific value
		/// in this PropertyDeclarationCollection
		/// </summary>
		/// <param name="value">
		/// The PropertyDeclaration value to locate in the PropertyDeclarationCollection.
		/// </param>
		/// <returns>
		/// The zero-based index of the first occurrence of the _ELEMENT value if found;
		/// -1 otherwise.
		/// </returns>
		public virtual int IndexOf(PropertyDeclaration value)
		{
			return this.List.IndexOf(value);
		}

		/// <summary>
		/// Inserts an element into the PropertyDeclarationCollection at the specified index
		/// </summary>
		/// <param name="index">
		/// The index at which the PropertyDeclaration is to be inserted.
		/// </param>
		/// <param name="value">
		/// The PropertyDeclaration to insert.
		/// </param>
		public virtual void Insert(int index, PropertyDeclaration value)
		{
			this.List.Insert(index, value);
		}

		/// <summary>
		/// Gets or sets the PropertyDeclaration at the given index in this PropertyDeclarationCollection.
		/// </summary>
		public virtual PropertyDeclaration this[int index]
		{
			get
			{
				return (PropertyDeclaration) this.List[index];
			}
			set
			{
				this.List[index] = value;
			}
		}

		/// <summary>
		/// Removes the first occurrence of a specific PropertyDeclaration from this PropertyDeclarationCollection.
		/// </summary>
		/// <param name="value">
		/// The PropertyDeclaration value to remove from this PropertyDeclarationCollection.
		/// </param>
		public virtual void Remove(PropertyDeclaration value)
		{
			this.List.Remove(value);
		}

		/// <summary>
		/// Type-specific enumeration class, used by PropertyDeclarationCollection.GetEnumerator.
		/// </summary>
		public class Enumerator: System.Collections.IEnumerator
		{
			private System.Collections.IEnumerator wrapped;

			public Enumerator(PropertyDeclarationCollection collection)
			{
				this.wrapped = ((System.Collections.CollectionBase)collection).GetEnumerator();
			}

			public PropertyDeclaration Current
			{
				get
				{
					return (PropertyDeclaration) (this.wrapped.Current);
				}
			}

			object System.Collections.IEnumerator.Current
			{
				get
				{
					return (PropertyDeclaration) (this.wrapped.Current);
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
		/// Returns an enumerator that can iterate through the elements of this PropertyDeclarationCollection.
		/// </summary>
		/// <returns>
		/// An object that implements System.Collections.IEnumerator.
		/// </returns>        
		public new virtual PropertyDeclarationCollection.Enumerator GetEnumerator()
		{
			return new PropertyDeclarationCollection.Enumerator(this);
		}
	}
}
