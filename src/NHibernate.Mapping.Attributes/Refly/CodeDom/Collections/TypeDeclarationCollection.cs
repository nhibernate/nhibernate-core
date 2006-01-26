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
	/// A collection of elements of type ITypeDeclaration
	/// </summary>
	public class TypeDeclarationCollection: System.Collections.CollectionBase
	{
		/// <summary>
		/// Initializes a new empty instance of the TypeDeclarationCollection class.
		/// </summary>
		public TypeDeclarationCollection()
		{
			// empty
		}

		/// <summary>
		/// Initializes a new instance of the TypeDeclarationCollection class, containing elements
		/// copied from an array.
		/// </summary>
		/// <param name="items">
		/// The array whose elements are to be added to the new TypeDeclarationCollection.
		/// </param>
		public TypeDeclarationCollection(ITypeDeclaration[] items)
		{
			this.AddRange(items);
		}

		/// <summary>
		/// Initializes a new instance of the TypeDeclarationCollection class, containing elements
		/// copied from another instance of TypeDeclarationCollection
		/// </summary>
		/// <param name="items">
		/// The TypeDeclarationCollection whose elements are to be added to the new TypeDeclarationCollection.
		/// </param>
		public TypeDeclarationCollection(TypeDeclarationCollection items)
		{
			this.AddRange(items);
		}

		/// <summary>
		/// Adds the elements of an array to the end of this TypeDeclarationCollection.
		/// </summary>
		/// <param name="items">
		/// The array whose elements are to be added to the end of this TypeDeclarationCollection.
		/// </param>
		public virtual void AddRange(ITypeDeclaration[] items)
		{
			foreach (ITypeDeclaration item in items)
			{
				this.List.Add(item);
			}
		}

		/// <summary>
		/// Adds the elements of another TypeDeclarationCollection to the end of this TypeDeclarationCollection.
		/// </summary>
		/// <param name="items">
		/// The TypeDeclarationCollection whose elements are to be added to the end of this TypeDeclarationCollection.
		/// </param>
		public virtual void AddRange(TypeDeclarationCollection items)
		{
			foreach (ITypeDeclaration item in items)
			{
				this.List.Add(item);
			}
		}

		/// <summary>
		/// Adds an instance of type ITypeDeclaration to the end of this TypeDeclarationCollection.
		/// </summary>
		/// <param name="value">
		/// The ITypeDeclaration to be added to the end of this TypeDeclarationCollection.
		/// </param>
		public virtual void Add(ITypeDeclaration value)
		{
			this.List.Add(value);
		}

		public virtual void Add(Type value)
		{
			this.List.Add(new TypeTypeDeclaration(value));
		}

		/// <summary>
		/// Determines whether a specfic ITypeDeclaration value is in this TypeDeclarationCollection.
		/// </summary>
		/// <param name="value">
		/// The ITypeDeclaration value to locate in this TypeDeclarationCollection.
		/// </param>
		/// <returns>
		/// true if value is found in this TypeDeclarationCollection;
		/// false otherwise.
		/// </returns>
		public virtual bool Contains(ITypeDeclaration value)
		{
			return this.List.Contains(value);
		}

		/// <summary>
		/// Return the zero-based index of the first occurrence of a specific value
		/// in this TypeDeclarationCollection
		/// </summary>
		/// <param name="value">
		/// The ITypeDeclaration value to locate in the TypeDeclarationCollection.
		/// </param>
		/// <returns>
		/// The zero-based index of the first occurrence of the _ELEMENT value if found;
		/// -1 otherwise.
		/// </returns>
		public virtual int IndexOf(ITypeDeclaration value)
		{
			return this.List.IndexOf(value);
		}

		/// <summary>
		/// Inserts an element into the TypeDeclarationCollection at the specified index
		/// </summary>
		/// <param name="index">
		/// The index at which the ITypeDeclaration is to be inserted.
		/// </param>
		/// <param name="value">
		/// The ITypeDeclaration to insert.
		/// </param>
		public virtual void Insert(int index, ITypeDeclaration value)
		{
			this.List.Insert(index, value);
		}

		/// <summary>
		/// Gets or sets the ITypeDeclaration at the given index in this TypeDeclarationCollection.
		/// </summary>
		public virtual ITypeDeclaration this[int index]
		{
			get
			{
				return (ITypeDeclaration) this.List[index];
			}
			set
			{
				this.List[index] = value;
			}
		}

		/// <summary>
		/// Removes the first occurrence of a specific ITypeDeclaration from this TypeDeclarationCollection.
		/// </summary>
		/// <param name="value">
		/// The ITypeDeclaration value to remove from this TypeDeclarationCollection.
		/// </param>
		public virtual void Remove(ITypeDeclaration value)
		{
			this.List.Remove(value);
		}

		/// <summary>
		/// Type-specific enumeration class, used by TypeDeclarationCollection.GetEnumerator.
		/// </summary>
		public class Enumerator: System.Collections.IEnumerator
		{
			private System.Collections.IEnumerator wrapped;

			public Enumerator(TypeDeclarationCollection collection)
			{
				this.wrapped = ((System.Collections.CollectionBase)collection).GetEnumerator();
			}

			public ITypeDeclaration Current
			{
				get
				{
					return (ITypeDeclaration) (this.wrapped.Current);
				}
			}

			object System.Collections.IEnumerator.Current
			{
				get
				{
					return (ITypeDeclaration) (this.wrapped.Current);
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
		/// Returns an enumerator that can iterate through the elements of this TypeDeclarationCollection.
		/// </summary>
		/// <returns>
		/// An object that implements System.Collections.IEnumerator.
		/// </returns>        
		public new virtual TypeDeclarationCollection.Enumerator GetEnumerator()
		{
			return new TypeDeclarationCollection.Enumerator(this);
		}
	}

}
