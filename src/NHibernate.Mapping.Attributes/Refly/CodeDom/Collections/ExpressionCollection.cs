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
	using Refly.CodeDom.Expressions;
	/// <summary>
	/// A collection of elements of type Expression
	/// </summary>
	public class ExpressionCollection: System.Collections.CollectionBase
	{
		/// <summary>
		/// Initializes a new empty instance of the ExpressionCollection class.
		/// </summary>
		public ExpressionCollection()
		{
			// empty
		}

		/// <summary>
		/// Initializes a new instance of the ExpressionCollection class, containing elements
		/// copied from an array.
		/// </summary>
		/// <param name="items">
		/// The array whose elements are to be added to the new ExpressionCollection.
		/// </param>
		public ExpressionCollection(Expression[] items)
		{
			this.AddRange(items);
		}

		/// <summary>
		/// Initializes a new instance of the ExpressionCollection class, containing elements
		/// copied from another instance of ExpressionCollection
		/// </summary>
		/// <param name="items">
		/// The ExpressionCollection whose elements are to be added to the new ExpressionCollection.
		/// </param>
		public ExpressionCollection(ExpressionCollection items)
		{
			this.AddRange(items);
		}

		/// <summary>
		/// Adds the elements of an array to the end of this ExpressionCollection.
		/// </summary>
		/// <param name="items">
		/// The array whose elements are to be added to the end of this ExpressionCollection.
		/// </param>
		public virtual void AddRange(Expression[] items)
		{
			foreach (Expression item in items)
			{
				this.List.Add(item);
			}
		}

		/// <summary>
		/// Adds the elements of another ExpressionCollection to the end of this ExpressionCollection.
		/// </summary>
		/// <param name="items">
		/// The ExpressionCollection whose elements are to be added to the end of this ExpressionCollection.
		/// </param>
		public virtual void AddRange(ExpressionCollection items)
		{
			foreach (Expression item in items)
			{
				this.List.Add(item);
			}
		}

		/// <summary>
		/// Adds an instance of type Expression to the end of this ExpressionCollection.
		/// </summary>
		/// <param name="value">
		/// The Expression to be added to the end of this ExpressionCollection.
		/// </param>
		public virtual void Add(Expression value)
		{
			this.List.Add(value);
		}

		public CodeExpression[] ToCodeDomArray()
		{
			CodeExpression[] cs = new CodeExpression[this.Count];
			int i = 0;
			foreach(Expression e in this)
			{
				cs[i++] = e.ToCodeDom();
			}
			return cs;
		}

		/// <summary>
		/// Type-specific enumeration class, used by ExpressionCollection.GetEnumerator.
		/// </summary>
		public class Enumerator: System.Collections.IEnumerator
		{
			private System.Collections.IEnumerator wrapped;

			public Enumerator(ExpressionCollection collection)
			{
				this.wrapped = ((System.Collections.CollectionBase)collection).GetEnumerator();
			}

			public Expression Current
			{
				get
				{
					return (Expression) (this.wrapped.Current);
				}
			}

			object System.Collections.IEnumerator.Current
			{
				get
				{
					return (Expression) (this.wrapped.Current);
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
		/// Returns an enumerator that can iterate through the elements of this ExpressionCollection.
		/// </summary>
		/// <returns>
		/// An object that implements System.Collections.IEnumerator.
		/// </returns>        
		public new virtual ExpressionCollection.Enumerator GetEnumerator()
		{
			return new ExpressionCollection.Enumerator(this);
		}
	}

}
