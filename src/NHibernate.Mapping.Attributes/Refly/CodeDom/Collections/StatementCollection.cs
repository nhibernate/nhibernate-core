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
	using Refly.CodeDom.Expressions;
	/// <summary>
	/// A collection of elements of type Statement
	/// </summary>
	public class StatementCollection: System.Collections.CollectionBase
	{
		/// <summary>
		/// Initializes a new empty instance of the StatementCollection class.
		/// </summary>
		public StatementCollection()
		{
			// empty
		}

		/// <summary>
		/// Initializes a new instance of the StatementCollection class, containing elements
		/// copied from an array.
		/// </summary>
		/// <param name="items">
		/// The array whose elements are to be added to the new StatementCollection.
		/// </param>
		public StatementCollection(Statement[] items)
		{
			this.AddRange(items);
		}

		/// <summary>
		/// Initializes a new instance of the StatementCollection class, containing elements
		/// copied from another instance of StatementCollection
		/// </summary>
		/// <param name="items">
		/// The StatementCollection whose elements are to be added to the new StatementCollection.
		/// </param>
		public StatementCollection(StatementCollection items)
		{
			this.AddRange(items);
		}

		/// <summary>
		/// Adds the elements of an array to the end of this StatementCollection.
		/// </summary>
		/// <param name="items">
		/// The array whose elements are to be added to the end of this StatementCollection.
		/// </param>
		public virtual void AddRange(Statement[] items)
		{
			foreach (Statement item in items)
			{
				this.List.Add(item);
			}
		}

		/// <summary>
		/// Adds the elements of another StatementCollection to the end of this StatementCollection.
		/// </summary>
		/// <param name="items">
		/// The StatementCollection whose elements are to be added to the end of this StatementCollection.
		/// </param>
		public virtual void AddRange(StatementCollection items)
		{
			foreach (Statement item in items)
			{
				this.List.Add(item);
			}
		}

		/// <summary>
		/// Adds an instance of type Statement to the end of this StatementCollection.
		/// </summary>
		/// <param name="value">
		/// The Statement to be added to the end of this StatementCollection.
		/// </param>
		public virtual void Add(Statement value)
		{
			this.List.Add(value);
		}

		public virtual void Add(Expression expression)
		{
			this.Add( new ExpressionStatement(expression) );
		}

		public virtual void AddAssign(Expression left, Expression right)
		{
			this.Add( Stm.Assign(left,right));
		}

		public void Return(Expression expression)
		{
			this.Add( Stm.Return(expression ) );
		}

		public virtual void Insert(int index, Statement value) 
		{
			this.List.Insert(index, value);
		}

		public virtual void Insert(int index, Expression expression) 
		{
			this.Insert(index, new ExpressionStatement(expression));
		}

		/// <summary>
		/// Type-specific enumeration class, used by StatementCollection.GetEnumerator.
		/// </summary>
		public class Enumerator: System.Collections.IEnumerator
		{
			private System.Collections.IEnumerator wrapped;

			public Enumerator(StatementCollection collection)
			{
				this.wrapped = ((System.Collections.CollectionBase)collection).GetEnumerator();
			}

			public Statement Current
			{
				get
				{
					return (Statement) (this.wrapped.Current);
				}
			}

			object System.Collections.IEnumerator.Current
			{
				get
				{
					return (Statement) (this.wrapped.Current);
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
		/// Returns an enumerator that can iterate through the elements of this StatementCollection.
		/// </summary>
		/// <returns>
		/// An object that implements System.Collections.IEnumerator.
		/// </returns>        
		public new virtual StatementCollection.Enumerator GetEnumerator()
		{
			return new StatementCollection.Enumerator(this);
		}

		public void ToCodeDom(CodeStatementCollection col)
		{
			foreach(Statement st in this)
			{
				st.ToCodeDom(col);
			}
		}

		public CodeStatement[] ToCodeDomArray()
		{
			CodeStatementCollection col = new CodeStatementCollection();
			ToCodeDom(col);

			CodeStatement[] sts = new CodeStatement[col.Count];
			col.CopyTo(sts,0);

			return sts;
		}
	}

}
