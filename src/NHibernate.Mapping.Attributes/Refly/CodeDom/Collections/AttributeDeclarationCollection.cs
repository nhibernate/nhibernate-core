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
namespace Refly.CodeDom
{
    /// <summary>
    /// A collection of elements of type AttributeDeclaration
    /// </summary>
    public class AttributeDeclarationCollection: System.Collections.CollectionBase
    {
        /// <summary>
        /// Initializes a new empty instance of the AttributeDeclarationCollection class.
        /// </summary>
        public AttributeDeclarationCollection()
        {
            // empty
        }

        /// <summary>
        /// Adds an instance of type AttributeDeclaration to the end of this AttributeDeclarationCollection.
        /// </summary>
        /// <param name="value">
        /// The AttributeDeclaration to be added to the end of this AttributeDeclarationCollection.
        /// </param>
        public virtual AttributeDeclaration Add(ITypeDeclaration type)
        {
        	AttributeDeclaration attr = new AttributeDeclaration(type);
            this.List.Add(attr);
        	return attr;
        }        

		public virtual AttributeDeclaration Add(Type type)
		{
			return this.Add(new TypeTypeDeclaration(type));
		}        

		public virtual AttributeDeclaration Add(String type)
		{
			return this.Add(new StringTypeDeclaration(type));
		}        

        /// <summary>
        /// Determines whether a specfic AttributeDeclaration value is in this AttributeDeclarationCollection.
        /// </summary>
        /// <param name="value">
        /// The AttributeDeclaration value to locate in this AttributeDeclarationCollection.
        /// </param>
        /// <returns>
        /// true if value is found in this AttributeDeclarationCollection;
        /// false otherwise.
        /// </returns>
        public virtual bool Contains(AttributeDeclaration value)
        {
            return this.List.Contains(value);
        }

        /// <summary>
        /// Inserts an element into the AttributeDeclarationCollection at the specified index
        /// </summary>
        /// <param name="index">
        /// The index at which the AttributeDeclaration is to be inserted.
        /// </param>
        /// <param name="value">
        /// The AttributeDeclaration to insert.
        /// </param>
        public virtual void Insert(int index, AttributeDeclaration value)
        {
            this.List.Insert(index, value);
        }

        /// <summary>
        /// Removes the first occurrence of a specific AttributeDeclaration from this AttributeDeclarationCollection.
        /// </summary>
        /// <param name="value">
        /// The AttributeDeclaration value to remove from this AttributeDeclarationCollection.
        /// </param>
        public virtual void Remove(AttributeDeclaration value)
        {
            this.List.Remove(value);
        }

        /// <summary>
        /// Type-specific enumeration class, used by AttributeDeclarationCollection.GetEnumerator.
        /// </summary>
        public class Enumerator: System.Collections.IEnumerator
        {
            private System.Collections.IEnumerator wrapped;

            public Enumerator(AttributeDeclarationCollection collection)
            {
                this.wrapped = ((System.Collections.CollectionBase)collection).GetEnumerator();
            }

            public AttributeDeclaration Current
            {
                get
                {
                    return (AttributeDeclaration) (this.wrapped.Current);
                }
            }

            object System.Collections.IEnumerator.Current
            {
                get
                {
                    return (AttributeDeclaration) (this.wrapped.Current);
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
        /// Returns an enumerator that can iterate through the elements of this AttributeDeclarationCollection.
        /// </summary>
        /// <returns>
        /// An object that implements System.Collections.IEnumerator.
        /// </returns>        
        public new virtual AttributeDeclarationCollection.Enumerator GetEnumerator()
        {
            return new AttributeDeclarationCollection.Enumerator(this);
        }

		public System.CodeDom.CodeAttributeDeclarationCollection ToCodeDom()
		{
			System.CodeDom.CodeAttributeDeclarationCollection attrs = 
				new System.CodeDom.CodeAttributeDeclarationCollection();
			foreach(AttributeDeclaration attr in this)
			{
				attrs.Add(attr.ToCodeDom());
			}
			return attrs;
		}
    }
}
