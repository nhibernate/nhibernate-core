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
	/// A collection of elements of type EventDeclaration
	/// </summary>
	public class EventDeclarationCollection: System.Collections.CollectionBase
	{
		/// <summary>
		/// Initializes a new empty instance of the EventDeclarationCollection class.
		/// </summary>
		public EventDeclarationCollection()
		{
			// empty
		}

		/// <summary>
		/// Initializes a new instance of the EventDeclarationCollection class, containing elements
		/// copied from an array.
		/// </summary>
		/// <param name="items">
		/// The array whose elements are to be added to the new EventDeclarationCollection.
		/// </param>
		public EventDeclarationCollection(EventDeclaration[] items)
		{
			this.AddRange(items);
		}

		/// <summary>
		/// Initializes a new instance of the EventDeclarationCollection class, containing elements
		/// copied from another instance of EventDeclarationCollection
		/// </summary>
		/// <param name="items">
		/// The EventDeclarationCollection whose elements are to be added to the new EventDeclarationCollection.
		/// </param>
		public EventDeclarationCollection(EventDeclarationCollection items)
		{
			this.AddRange(items);
		}

		/// <summary>
		/// Adds the elements of an array to the end of this EventDeclarationCollection.
		/// </summary>
		/// <param name="items">
		/// The array whose elements are to be added to the end of this EventDeclarationCollection.
		/// </param>
		public virtual void AddRange(EventDeclaration[] items)
		{
			foreach (EventDeclaration item in items)
			{
				this.List.Add(item);
			}
		}

		/// <summary>
		/// Adds the elements of another EventDeclarationCollection to the end of this EventDeclarationCollection.
		/// </summary>
		/// <param name="items">
		/// The EventDeclarationCollection whose elements are to be added to the end of this EventDeclarationCollection.
		/// </param>
		public virtual void AddRange(EventDeclarationCollection items)
		{
			foreach (EventDeclaration item in items)
			{
				this.List.Add(item);
			}
		}

		/// <summary>
		/// Adds an instance of type EventDeclaration to the end of this EventDeclarationCollection.
		/// </summary>
		/// <param name="value">
		/// The EventDeclaration to be added to the end of this EventDeclarationCollection.
		/// </param>
		public virtual void Add(EventDeclaration value)
		{
			this.List.Add(value);
		}

		/// <summary>
		/// Determines whether a specfic EventDeclaration value is in this EventDeclarationCollection.
		/// </summary>
		/// <param name="value">
		/// The EventDeclaration value to locate in this EventDeclarationCollection.
		/// </param>
		/// <returns>
		/// true if value is found in this EventDeclarationCollection;
		/// false otherwise.
		/// </returns>
		public virtual bool Contains(EventDeclaration value)
		{
			return this.List.Contains(value);
		}

		/// <summary>
		/// Return the zero-based index of the first occurrence of a specific value
		/// in this EventDeclarationCollection
		/// </summary>
		/// <param name="value">
		/// The EventDeclaration value to locate in the EventDeclarationCollection.
		/// </param>
		/// <returns>
		/// The zero-based index of the first occurrence of the _ELEMENT value if found;
		/// -1 otherwise.
		/// </returns>
		public virtual int IndexOf(EventDeclaration value)
		{
			return this.List.IndexOf(value);
		}

		/// <summary>
		/// Inserts an element into the EventDeclarationCollection at the specified index
		/// </summary>
		/// <param name="index">
		/// The index at which the EventDeclaration is to be inserted.
		/// </param>
		/// <param name="value">
		/// The EventDeclaration to insert.
		/// </param>
		public virtual void Insert(int index, EventDeclaration value)
		{
			this.List.Insert(index, value);
		}

		/// <summary>
		/// Gets or sets the EventDeclaration at the given index in this EventDeclarationCollection.
		/// </summary>
		public virtual EventDeclaration this[int index]
		{
			get
			{
				return (EventDeclaration) this.List[index];
			}
			set
			{
				this.List[index] = value;
			}
		}

		/// <summary>
		/// Removes the first occurrence of a specific EventDeclaration from this EventDeclarationCollection.
		/// </summary>
		/// <param name="value">
		/// The EventDeclaration value to remove from this EventDeclarationCollection.
		/// </param>
		public virtual void Remove(EventDeclaration value)
		{
			this.List.Remove(value);
		}

		/// <summary>
		/// Type-specific enumeration class, used by EventDeclarationCollection.GetEnumerator.
		/// </summary>
		public class Enumerator: System.Collections.IEnumerator
		{
			private System.Collections.IEnumerator wrapped;

			public Enumerator(EventDeclarationCollection collection)
			{
				this.wrapped = ((System.Collections.CollectionBase)collection).GetEnumerator();
			}

			public EventDeclaration Current
			{
				get
				{
					return (EventDeclaration) (this.wrapped.Current);
				}
			}

			object System.Collections.IEnumerator.Current
			{
				get
				{
					return (EventDeclaration) (this.wrapped.Current);
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
		/// Returns an enumerator that can iterate through the elements of this EventDeclarationCollection.
		/// </summary>
		/// <returns>
		/// An object that implements System.Collections.IEnumerator.
		/// </returns>        
		public new virtual EventDeclarationCollection.Enumerator GetEnumerator()
		{
			return new EventDeclarationCollection.Enumerator(this);
		}
	}

}
