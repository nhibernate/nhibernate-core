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
	/// A collection of elements of type MethodDeclaration
	/// </summary>
	public class MethodDeclarationCollection: System.Collections.CollectionBase
	{
		/// <summary>
		/// Initializes a new empty instance of the MethodDeclarationCollection class.
		/// </summary>
		public MethodDeclarationCollection()
		{
			// empty
		}

		/// <summary>
		/// Initializes a new instance of the MethodDeclarationCollection class, containing elements
		/// copied from an array.
		/// </summary>
		/// <param name="items">
		/// The array whose elements are to be added to the new MethodDeclarationCollection.
		/// </param>
		public MethodDeclarationCollection(MethodDeclaration[] items)
		{
			this.AddRange(items);
		}

		/// <summary>
		/// Initializes a new instance of the MethodDeclarationCollection class, containing elements
		/// copied from another instance of MethodDeclarationCollection
		/// </summary>
		/// <param name="items">
		/// The MethodDeclarationCollection whose elements are to be added to the new MethodDeclarationCollection.
		/// </param>
		public MethodDeclarationCollection(MethodDeclarationCollection items)
		{
			this.AddRange(items);
		}

		/// <summary>
		/// Adds the elements of an array to the end of this MethodDeclarationCollection.
		/// </summary>
		/// <param name="items">
		/// The array whose elements are to be added to the end of this MethodDeclarationCollection.
		/// </param>
		public virtual void AddRange(MethodDeclaration[] items)
		{
			foreach (MethodDeclaration item in items)
			{
				this.List.Add(item);
			}
		}

		/// <summary>
		/// Adds the elements of another MethodDeclarationCollection to the end of this MethodDeclarationCollection.
		/// </summary>
		/// <param name="items">
		/// The MethodDeclarationCollection whose elements are to be added to the end of this MethodDeclarationCollection.
		/// </param>
		public virtual void AddRange(MethodDeclarationCollection items)
		{
			foreach (MethodDeclaration item in items)
			{
				this.List.Add(item);
			}
		}

		/// <summary>
		/// Adds an instance of type MethodDeclaration to the end of this MethodDeclarationCollection.
		/// </summary>
		/// <param name="value">
		/// The MethodDeclaration to be added to the end of this MethodDeclarationCollection.
		/// </param>
		public virtual void Add(MethodDeclaration value)
		{
			this.List.Add(value);
		}

		/// <summary>
		/// Determines whether a specfic MethodDeclaration value is in this MethodDeclarationCollection.
		/// </summary>
		/// <param name="value">
		/// The MethodDeclaration value to locate in this MethodDeclarationCollection.
		/// </param>
		/// <returns>
		/// true if value is found in this MethodDeclarationCollection;
		/// false otherwise.
		/// </returns>
		public virtual bool Contains(MethodDeclaration value)
		{
			return this.List.Contains(value);
		}

		/// <summary>
		/// Checks the existence of a method name.
		/// </summary>
		/// <remarks></remarks>
		/// <param name="Name"></param>
		/// <returns></returns>
		public virtual bool ContainsMethodName(string Name) 
		{
			foreach(MethodDeclaration md in this.List) 
				if(md.Name == Name)
					return true;
			return false;
		}

		/// <summary>
		/// Return the zero-based index of the first occurrence of a specific value
		/// in this MethodDeclarationCollection
		/// </summary>
		/// <param name="value">
		/// The MethodDeclaration value to locate in the MethodDeclarationCollection.
		/// </param>
		/// <returns>
		/// The zero-based index of the first occurrence of the _ELEMENT value if found;
		/// -1 otherwise.
		/// </returns>
		public virtual int IndexOf(MethodDeclaration value)
		{
			return this.List.IndexOf(value);
		}

		/// <summary>
		/// Inserts an element into the MethodDeclarationCollection at the specified index
		/// </summary>
		/// <param name="index">
		/// The index at which the MethodDeclaration is to be inserted.
		/// </param>
		/// <param name="value">
		/// The MethodDeclaration to insert.
		/// </param>
		public virtual void Insert(int index, MethodDeclaration value)
		{
			this.List.Insert(index, value);
		}

		/// <summary>
		/// Gets or sets the MethodDeclaration at the given index in this MethodDeclarationCollection.
		/// </summary>
		public virtual MethodDeclaration this[int index]
		{
			get
			{
				return (MethodDeclaration) this.List[index];
			}
			set
			{
				this.List[index] = value;
			}
		}

		/// <summary>
		/// Gets or sets the MethodDeclaration with the given name in this MethodDeclarationCollection.
		/// </summary>
		/// <remarks></remarks>
		public virtual MethodDeclaration this[string Name] 
		{
			get {
				foreach(MethodDeclaration md in this.List) 
					if(md.Name == Name)
						return md;
            throw new ApplicationException(Name + "() method declaration does not exist.");
			}
		}

		/// <summary>
		/// Removes the first occurrence of a specific MethodDeclaration from this MethodDeclarationCollection.
		/// </summary>
		/// <param name="value">
		/// The MethodDeclaration value to remove from this MethodDeclarationCollection.
		/// </param>
		public virtual void Remove(MethodDeclaration value)
		{
			this.List.Remove(value);
		}

		/// <summary>
		/// Type-specific enumeration class, used by MethodDeclarationCollection.GetEnumerator.
		/// </summary>
		public class Enumerator: System.Collections.IEnumerator
		{
			private System.Collections.IEnumerator wrapped;

			public Enumerator(MethodDeclarationCollection collection)
			{
				this.wrapped = ((System.Collections.CollectionBase)collection).GetEnumerator();
			}

			public MethodDeclaration Current
			{
				get
				{
					return (MethodDeclaration) (this.wrapped.Current);
				}
			}

			object System.Collections.IEnumerator.Current
			{
				get
				{
					return (MethodDeclaration) (this.wrapped.Current);
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
		/// Returns an enumerator that can iterate through the elements of this MethodDeclarationCollection.
		/// </summary>
		/// <returns>
		/// An object that implements System.Collections.IEnumerator.
		/// </returns>        
		public new virtual MethodDeclarationCollection.Enumerator GetEnumerator()
		{
			return new MethodDeclarationCollection.Enumerator(this);
		}
	}

}
