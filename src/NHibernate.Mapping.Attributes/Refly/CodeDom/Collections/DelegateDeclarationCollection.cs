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
    /// A collection of elements of type DelegateDeclaration
    /// </summary>
    public class DelegateDeclarationCollection : System.Collections.CollectionBase
    {
        /// <summary>
        /// Initializes a new empty instance of the DelegateDeclarationCollection class.
        /// </summary>
        public DelegateDeclarationCollection()
        {
            // empty
        }

        /// <summary>
        /// Initializes a new instance of the DelegateDeclarationCollection class, containing elements
        /// copied from an array.
        /// </summary>
        /// <param name="items">
        /// The array whose elements are to be added to the new DelegateDeclarationCollection.
        /// </param>
        public DelegateDeclarationCollection(DelegateDeclaration[] items)
        {
            this.AddRange(items);
        }

        /// <summary>
        /// Initializes a new instance of the DelegateDeclarationCollection class, containing elements
        /// copied from another instance of DelegateDeclarationCollection
        /// </summary>
        /// <param name="items">
        /// The DelegateDeclarationCollection whose elements are to be added to the new DelegateDeclarationCollection.
        /// </param>
        public DelegateDeclarationCollection(DelegateDeclarationCollection items)
        {
            this.AddRange(items);
        }

        /// <summary>
        /// Adds the elements of an array to the end of this DelegateDeclarationCollection.
        /// </summary>
        /// <param name="items">
        /// The array whose elements are to be added to the end of this DelegateDeclarationCollection.
        /// </param>
        public virtual void AddRange(DelegateDeclaration[] items)
        {
            foreach (DelegateDeclaration item in items)
            {
                this.List.Add(item);
            }
        }

        /// <summary>
        /// Adds the elements of another DelegateDeclarationCollection to the end of this DelegateDeclarationCollection.
        /// </summary>
        /// <param name="items">
        /// The DelegateDeclarationCollection whose elements are to be added to the end of this DelegateDeclarationCollection.
        /// </param>
        public virtual void AddRange(DelegateDeclarationCollection items)
        {
            foreach (DelegateDeclaration item in items)
            {
                this.List.Add(item);
            }
        }

        /// <summary>
        /// Adds an instance of type DelegateDeclaration to the end of this DelegateDeclarationCollection.
        /// </summary>
        /// <param name="value">
        /// The DelegateDeclaration to be added to the end of this DelegateDeclarationCollection.
        /// </param>
        public virtual void Add(DelegateDeclaration value)
        {
            this.List.Add(value);
        }

        /// <summary>
        /// Determines whether a specfic DelegateDeclaration value is in this DelegateDeclarationCollection.
        /// </summary>
        /// <param name="value">
        /// The DelegateDeclaration value to locate in this DelegateDeclarationCollection.
        /// </param>
        /// <returns>
        /// true if value is found in this DelegateDeclarationCollection;
        /// false otherwise.
        /// </returns>
        public virtual bool Contains(DelegateDeclaration value)
        {
            return this.List.Contains(value);
        }

        /// <summary>
        /// Checks the existence of a method name.
        /// </summary>
        /// <remarks></remarks>
        /// <param name="Name"></param>
        /// <returns></returns>
        public virtual bool ContainsDelegateName(string Name)
        {
            foreach (DelegateDeclaration md in this.List)
            {
                if (md.Name == Name)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Return the zero-based index of the first occurrence of a specific value
        /// in this DelegateDeclarationCollection
        /// </summary>
        /// <param name="value">
        /// The DelegateDeclaration value to locate in the DelegateDeclarationCollection.
        /// </param>
        /// <returns>
        /// The zero-based index of the first occurrence of the _ELEMENT value if found;
        /// -1 otherwise.
        /// </returns>
        public virtual int IndexOf(DelegateDeclaration value)
        {
            return this.List.IndexOf(value);
        }

        /// <summary>
        /// Inserts an element into the DelegateDeclarationCollection at the specified index
        /// </summary>
        /// <param name="index">
        /// The index at which the DelegateDeclaration is to be inserted.
        /// </param>
        /// <param name="value">
        /// The DelegateDeclaration to insert.
        /// </param>
        public virtual void Insert(int index, DelegateDeclaration value)
        {
            this.List.Insert(index, value);
        }

        /// <summary>
        /// Gets or sets the DelegateDeclaration at the given index in this DelegateDeclarationCollection.
        /// </summary>
        public virtual DelegateDeclaration this[int index]
        {
            get
            {
                return (DelegateDeclaration)this.List[index];
            }
            set
            {
                this.List[index] = value;
            }
        }

        /// <summary>
        /// Gets or sets the DelegateDeclaration with the given name in this DelegateDeclarationCollection.
        /// </summary>
        /// <remarks></remarks>
        public virtual DelegateDeclaration this[string Name]
        {
            get
            {
                foreach (DelegateDeclaration md in this.List)
                    if (md.Name == Name)
                        return md;
                throw new ApplicationException(Name + "() method declaration does not exist.");
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific DelegateDeclaration from this DelegateDeclarationCollection.
        /// </summary>
        /// <param name="value">
        /// The DelegateDeclaration value to remove from this DelegateDeclarationCollection.
        /// </param>
        public virtual void Remove(DelegateDeclaration value)
        {
            this.List.Remove(value);
        }

        /// <summary>
        /// Type-specific enumeration class, used by DelegateDeclarationCollection.GetEnumerator.
        /// </summary>
        public class Enumerator : System.Collections.IEnumerator
        {
            private System.Collections.IEnumerator wrapped;

            public Enumerator(DelegateDeclarationCollection collection)
            {
                this.wrapped = ((System.Collections.CollectionBase)collection).GetEnumerator();
            }

            public DelegateDeclaration Current
            {
                get
                {
                    return (DelegateDeclaration)(this.wrapped.Current);
                }
            }

            object System.Collections.IEnumerator.Current
            {
                get
                {
                    return (DelegateDeclaration)(this.wrapped.Current);
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
        /// Returns an enumerator that can iterate through the elements of this DelegateDeclarationCollection.
        /// </summary>
        /// <returns>
        /// An object that implements System.Collections.IEnumerator.
        /// </returns>        
        public new virtual DelegateDeclarationCollection.Enumerator GetEnumerator()
        {
            return new DelegateDeclarationCollection.Enumerator(this);
        }
    }

}
