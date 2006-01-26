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
using System.Reflection;

namespace Refly.CodeDom.Collections
{
	/// <summary>
	/// A collection of elements of type Assembly
	/// </summary>
	public class AssemblyCollection: System.Collections.CollectionBase
	{
		/// <summary>
		/// Initializes a new empty instance of the AssemblyCollection class.
		/// </summary>
		public AssemblyCollection()
		{
			// empty
		}

		/// <summary>
		/// Initializes a new instance of the AssemblyCollection class, containing elements
		/// copied from an array.
		/// </summary>
		/// <param name="items">
		/// The array whose elements are to be added to the new AssemblyCollection.
		/// </param>
		public AssemblyCollection(Assembly[] items)
		{
			this.AddRange(items);
		}

		/// <summary>
		/// Initializes a new instance of the AssemblyCollection class, containing elements
		/// copied from another instance of AssemblyCollection
		/// </summary>
		/// <param name="items">
		/// The AssemblyCollection whose elements are to be added to the new AssemblyCollection.
		/// </param>
		public AssemblyCollection(AssemblyCollection items)
		{
			this.AddRange(items);
		}

		/// <summary>
		/// Adds the elements of an array to the end of this AssemblyCollection.
		/// </summary>
		/// <param name="items">
		/// The array whose elements are to be added to the end of this AssemblyCollection.
		/// </param>
		public virtual void AddRange(Assembly[] items)
		{
			foreach (Assembly item in items)
			{
				this.List.Add(item);
			}
		}

		/// <summary>
		/// Adds the elements of another AssemblyCollection to the end of this AssemblyCollection.
		/// </summary>
		/// <param name="items">
		/// The AssemblyCollection whose elements are to be added to the end of this AssemblyCollection.
		/// </param>
		public virtual void AddRange(AssemblyCollection items)
		{
			foreach (Assembly item in items)
			{
				this.List.Add(item);
			}
		}

		/// <summary>
		/// Adds an instance of type Assembly to the end of this AssemblyCollection.
		/// </summary>
		/// <param name="value">
		/// The Assembly to be added to the end of this AssemblyCollection.
		/// </param>
		public virtual void Add(Assembly value)
		{
			this.List.Add(value);
		}

		/// <summary>
		/// Determines whether a specfic Assembly value is in this AssemblyCollection.
		/// </summary>
		/// <param name="value">
		/// The Assembly value to locate in this AssemblyCollection.
		/// </param>
		/// <returns>
		/// true if value is found in this AssemblyCollection;
		/// false otherwise.
		/// </returns>
		public virtual bool Contains(Assembly value)
		{
			return this.List.Contains(value);
		}

		/// <summary>
		/// Return the zero-based index of the first occurrence of a specific value
		/// in this AssemblyCollection
		/// </summary>
		/// <param name="value">
		/// The Assembly value to locate in the AssemblyCollection.
		/// </param>
		/// <returns>
		/// The zero-based index of the first occurrence of the _ELEMENT value if found;
		/// -1 otherwise.
		/// </returns>
		public virtual int IndexOf(Assembly value)
		{
			return this.List.IndexOf(value);
		}

		/// <summary>
		/// Inserts an element into the AssemblyCollection at the specified index
		/// </summary>
		/// <param name="index">
		/// The index at which the Assembly is to be inserted.
		/// </param>
		/// <param name="value">
		/// The Assembly to insert.
		/// </param>
		public virtual void Insert(int index, Assembly value)
		{
			this.List.Insert(index, value);
		}

		/// <summary>
		/// Gets or sets the Assembly at the given index in this AssemblyCollection.
		/// </summary>
		public virtual Assembly this[int index]
		{
			get
			{
				return (Assembly) this.List[index];
			}
			set
			{
				this.List[index] = value;
			}
		}

		/// <summary>
		/// Removes the first occurrence of a specific Assembly from this AssemblyCollection.
		/// </summary>
		/// <param name="value">
		/// The Assembly value to remove from this AssemblyCollection.
		/// </param>
		public virtual void Remove(Assembly value)
		{
			this.List.Remove(value);
		}

		/// <summary>
		/// Type-specific enumeration class, used by AssemblyCollection.GetEnumerator.
		/// </summary>
		public class Enumerator: System.Collections.IEnumerator
		{
			private System.Collections.IEnumerator wrapped;

			public Enumerator(AssemblyCollection collection)
			{
				this.wrapped = ((System.Collections.CollectionBase)collection).GetEnumerator();
			}

			public Assembly Current
			{
				get
				{
					return (Assembly) (this.wrapped.Current);
				}
			}

			object System.Collections.IEnumerator.Current
			{
				get
				{
					return (Assembly) (this.wrapped.Current);
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
		/// Returns an enumerator that can iterate through the elements of this AssemblyCollection.
		/// </summary>
		/// <returns>
		/// An object that implements System.Collections.IEnumerator.
		/// </returns>        
		public new virtual AssemblyCollection.Enumerator GetEnumerator()
		{
			return new AssemblyCollection.Enumerator(this);
		}
	}
}
