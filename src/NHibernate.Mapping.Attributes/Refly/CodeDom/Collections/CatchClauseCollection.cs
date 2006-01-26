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
	using Refly.CodeDom.Statements;

	/// <summary>
	/// A collection of elements of type CatchClause
	/// </summary>
	public class CatchClauseCollection : System.Collections.CollectionBase
	{
		/// <summary>
		/// Initializes a new empty instance of the CatchClauseCollection class.
		/// </summary>
		public CatchClauseCollection()
		{
			// empty
		}

		/// <summary>
		/// Initializes a new instance of the CatchClauseCollection class, containing elements
		/// copied from an array.
		/// </summary>
		/// <param name="items">
		/// The array whose elements are to be added to the new CatchClauseCollection.
		/// </param>
		public CatchClauseCollection(CatchClause[] items)
		{
			this.AddRange(items);
		}

		/// <summary>
		/// Initializes a new instance of the CatchClauseCollection class, containing elements
		/// copied from another instance of CatchClauseCollection
		/// </summary>
		/// <param name="items">
		/// The CatchClauseCollection whose elements are to be added to the new CatchClauseCollection.
		/// </param>
		public CatchClauseCollection(CatchClauseCollection items)
		{
			this.AddRange(items);
		}

		/// <summary>
		/// Adds the elements of an array to the end of this CatchClauseCollection.
		/// </summary>
		/// <param name="items">
		/// The array whose elements are to be added to the end of this CatchClauseCollection.
		/// </param>
		public virtual void AddRange(CatchClause[] items)
		{
			foreach (CatchClause item in items)
			{
				this.List.Add(item);
			}
		}

		/// <summary>
		/// Adds the elements of another CatchClauseCollection to the end of this CatchClauseCollection.
		/// </summary>
		/// <param name="items">
		/// The CatchClauseCollection whose elements are to be added to the end of this CatchClauseCollection.
		/// </param>
		public virtual void AddRange(CatchClauseCollection items)
		{
			foreach (CatchClause item in items)
			{
				this.List.Add(item);
			}
		}

		/// <summary>
		/// Adds an instance of type CatchClause to the end of this CatchClauseCollection.
		/// </summary>
		/// <param name="value">
		/// The CatchClause to be added to the end of this CatchClauseCollection.
		/// </param>
		public virtual void Add(CatchClause value)
		{
			this.List.Add(value);
		}

		/// <summary>
		/// Determines whether a specfic CatchClause value is in this CatchClauseCollection.
		/// </summary>
		/// <param name="value">
		/// The CatchClause value to locate in this CatchClauseCollection.
		/// </param>
		/// <returns>
		/// true if value is found in this CatchClauseCollection;
		/// false otherwise.
		/// </returns>
		public virtual bool Contains(CatchClause value)
		{
			return this.List.Contains(value);
		}

		/// <summary>
		/// Return the zero-based index of the first occurrence of a specific value
		/// in this CatchClauseCollection
		/// </summary>
		/// <param name="value">
		/// The CatchClause value to locate in the CatchClauseCollection.
		/// </param>
		/// <returns>
		/// The zero-based index of the first occurrence of the _ELEMENT value if found;
		/// -1 otherwise.
		/// </returns>
		public virtual int IndexOf(CatchClause value)
		{
			return this.List.IndexOf(value);
		}

		/// <summary>
		/// Inserts an element into the CatchClauseCollection at the specified index
		/// </summary>
		/// <param name="index">
		/// The index at which the CatchClause is to be inserted.
		/// </param>
		/// <param name="value">
		/// The CatchClause to insert.
		/// </param>
		public virtual void Insert(int index, CatchClause value)
		{
			this.List.Insert(index, value);
		}

		/// <summary>
		/// Gets or sets the CatchClause at the given index in this CatchClauseCollection.
		/// </summary>
		public virtual CatchClause this[int index]
		{
			get
			{
				return (CatchClause) this.List[index];
			}
			set
			{
				this.List[index] = value;
			}
		}

		/// <summary>
		/// Removes the first occurrence of a specific CatchClause from this CatchClauseCollection.
		/// </summary>
		/// <param name="value">
		/// The CatchClause value to remove from this CatchClauseCollection.
		/// </param>
		public virtual void Remove(CatchClause value)
		{
			this.List.Remove(value);
		}

		public CodeCatchClause[] ToCodeDomArray()
		{
			CodeCatchClause[] cc = new CodeCatchClause[this.Count];
			int i = 0;
			foreach(CatchClause st in this)
			{
				cc[i++]=st.ToCodeDom();
			}
			return cc;
		}

		/// <summary>
		/// Type-specific enumeration class, used by CatchClauseCollection.GetEnumerator.
		/// </summary>
		public class Enumerator: System.Collections.IEnumerator
		{
			private System.Collections.IEnumerator wrapped;

			public Enumerator(CatchClauseCollection collection)
			{
				this.wrapped = ((System.Collections.CollectionBase)collection).GetEnumerator();
			}

			public CatchClause Current
			{
				get
				{
					return (CatchClause) (this.wrapped.Current);
				}
			}

			object System.Collections.IEnumerator.Current
			{
				get
				{
					return (CatchClause) (this.wrapped.Current);
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
		/// Returns an enumerator that can iterate through the elements of this CatchClauseCollection.
		/// </summary>
		/// <returns>
		/// An object that implements System.Collections.IEnumerator.
		/// </returns>        
		public new virtual CatchClauseCollection.Enumerator GetEnumerator()
		{
			return new CatchClauseCollection.Enumerator(this);
		}
	}

}
