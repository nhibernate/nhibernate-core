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
using System.Collections;

namespace Refly.CodeDom.Collections
{
	using Refly.CodeDom.Expressions;

     /// <summary>
    /// A dictionary with keys of type string and values of type AttributeArgument
    /// </summary>
    public class StringAttributeArgumentDictionary: System.Collections.DictionaryBase
    {
        /// <summary>
        /// Initializes a new empty instance of the StringAttributeArgumentDictionary class
        /// </summary>
        public StringAttributeArgumentDictionary()
        {}

        /// <summary>
        /// Gets or sets the AttributeArgument associated with the given string
        /// </summary>
        /// <param name="key">
        /// The string whose value to get or set.
        /// </param>
        public virtual AttributeArgument this[string key]
        {
            get
            {
                return (AttributeArgument) this.Dictionary[key];
            }
            set
            {
            	if (value==null)
            		throw new ArgumentNullException("value");
                this.Dictionary[key] = value;
            }
        }

        /// <summary>
        /// Adds an element with the specified key and value to this StringAttributeArgumentDictionary.
        /// </summary>
        /// <param name="key">
        /// The string key of the element to add.
        /// </param>
        /// <param name="value">
        /// The AttributeArgument value of the element to add.
        /// </param>
		public virtual AttributeArgument Add(string name, Expression expression)
        {
        	if(name==null)
        		throw new ArgumentNullException("name");
        	if (expression==null)
        		throw new ArgumentNullException("expression");
        	AttributeArgument arg=new AttributeArgument(name,expression);
        	this.Dictionary.Add(arg.Name,arg);
        	return arg;
        }

        /// <summary>
        /// Determines whether this StringAttributeArgumentDictionary contains a specific key.
        /// </summary>
        /// <param name="key">
        /// The string key to locate in this StringAttributeArgumentDictionary.
        /// </param>
        /// <returns>
        /// true if this StringAttributeArgumentDictionary contains an element with the specified key;
        /// otherwise, false.
        /// </returns>
        public virtual bool Contains(string key)
        {
            return this.Dictionary.Contains(key);
        }

        /// <summary>
        /// Determines whether this StringAttributeArgumentDictionary contains a specific key.
        /// </summary>
        /// <param name="key">
        /// The string key to locate in this StringAttributeArgumentDictionary.
        /// </param>
        /// <returns>
        /// true if this StringAttributeArgumentDictionary contains an element with the specified key;
        /// otherwise, false.
        /// </returns>
        public virtual bool ContainsKey(string key)
        {
            return this.Dictionary.Contains(key);
        }

        /// <summary>
        /// Removes the element with the specified key from this StringAttributeArgumentDictionary.
        /// </summary>
        /// <param name="key">
        /// The string key of the element to remove.
        /// </param>
        public virtual void Remove(string key)
        {
            this.Dictionary.Remove(key);
        }

        /// <summary>
        /// Gets a collection containing the keys in this StringAttributeArgumentDictionary.
        /// </summary>
        public virtual System.Collections.ICollection Keys
        {
            get
            {
                return this.Dictionary.Keys;
            }
        }

        /// <summary>
        /// Gets a collection containing the values in this StringAttributeArgumentDictionary.
        /// </summary>
        public virtual System.Collections.ICollection Values
        {
            get
            {
                return this.Dictionary.Values;
            }
        }
        
        public System.CodeDom.CodeAttributeArgumentCollection ToCodeDom()
        {
        	System.CodeDom.CodeAttributeArgumentCollection args =
        	    new System.CodeDom.CodeAttributeArgumentCollection();
        	
        	foreach(AttributeArgument arg in this.Values)
        	{
        		args.Add( arg.ToCodeDom() );
        	}
        	
        	return args;
        }
    }
}
