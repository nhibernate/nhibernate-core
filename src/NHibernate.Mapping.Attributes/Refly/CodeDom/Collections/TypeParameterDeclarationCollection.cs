#if WHIDBEY
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
    /// A collection of elements of type ITypeParameterDeclaration
    /// </summary>
    public class TypeParameterDeclarationCollection : System.Collections.CollectionBase
    {
        /// <summary>
        /// Adds an instance of type ITypeParameterDeclaration to the end of this TypeParameterDeclarationCollection.
        /// </summary>
        /// <param name="value">
        /// The ITypeParameterDeclaration to be added to the end of this TypeParameterDeclarationCollection.
        /// </param>
        public virtual void Add(TypeParameterDeclaration value)
        {
            this.List.Add(value);
        }

        /// <summary>
        /// Determines whether a specfic ITypeParameterDeclaration value is in this TypeParameterDeclarationCollection.
        /// </summary>
        /// <param name="value">
        /// The ITypeParameterDeclaration value to locate in this TypeParameterDeclarationCollection.
        /// </param>
        /// <returns>
        /// true if value is found in this TypeParameterDeclarationCollection;
        /// false otherwise.
        /// </returns>
        public virtual bool Contains(TypeParameterDeclaration value)
        {
            return this.List.Contains(value);
        }

        /// <summary>
        /// Return the zero-based index of the first occurrence of a specific value
        /// in this TypeParameterDeclarationCollection
        /// </summary>
        /// <param name="value">
        /// The ITypeParameterDeclaration value to locate in the TypeParameterDeclarationCollection.
        /// </param>
        /// <returns>
        /// The zero-based index of the first occurrence of the _ELEMENT value if found;
        /// -1 otherwise.
        /// </returns>
        public virtual int IndexOf(TypeParameterDeclaration value)
        {
            return this.List.IndexOf(value);
        }

        /// <summary>
        /// Inserts an element into the TypeParameterDeclarationCollection at the specified index
        /// </summary>
        /// <param name="index">
        /// The index at which the ITypeParameterDeclaration is to be inserted.
        /// </param>
        /// <param name="value">
        /// The ITypeParameterDeclaration to insert.
        /// </param>
        public virtual void Insert(int index, TypeParameterDeclaration value)
        {
            this.List.Insert(index, value);
        }

        /// <summary>
        /// Gets or sets the ITypeParameterDeclaration at the given index in this TypeParameterDeclarationCollection.
        /// </summary>
        public virtual TypeParameterDeclaration this[int index]
        {
            get
            {
                return (TypeParameterDeclaration)this.List[index];
            }
            set
            {
                this.List[index] = value;
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific ITypeParameterDeclaration from this TypeParameterDeclarationCollection.
        /// </summary>
        /// <param name="value">
        /// The ITypeParameterDeclaration value to remove from this TypeParameterDeclarationCollection.
        /// </param>
        public virtual void Remove(TypeParameterDeclaration value)
        {
            this.List.Remove(value);
        }

        /// <summary>
        /// Type-specific enumeration class, used by TypeParameterDeclarationCollection.GetEnumerator.
        /// </summary>
        public sealed class Enumerator : System.Collections.IEnumerator
        {
            private System.Collections.IEnumerator wrapped;

            public Enumerator(TypeParameterDeclarationCollection collection)
            {
                this.wrapped = ((System.Collections.CollectionBase)collection).GetEnumerator();
            }

            public TypeParameterDeclaration Current
            {
                get
                {
                    return (TypeParameterDeclaration)(this.wrapped.Current);
                }
            }

            object System.Collections.IEnumerator.Current
            {
                get
                {
                    return (TypeParameterDeclaration)(this.wrapped.Current);
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
        /// Returns an enumerator that can iterate through the elements of this TypeParameterDeclarationCollection.
        /// </summary>
        /// <returns>
        /// An object that implements System.Collections.IEnumerator.
        /// </returns>        
        public new virtual TypeParameterDeclarationCollection.Enumerator GetEnumerator()
        {
            return new TypeParameterDeclarationCollection.Enumerator(this);
        }

        public System.CodeDom.CodeTypeParameter[] ToCodeDom()
        {
            System.CodeDom.CodeTypeParameter[] tps = new System.CodeDom.CodeTypeParameter[this.Count];

            int index = 0;
            foreach (TypeParameterDeclaration tp in this)
                tps[index++] = tp.ToCodeDom();
            return tps;
        }
    }
}
#endif