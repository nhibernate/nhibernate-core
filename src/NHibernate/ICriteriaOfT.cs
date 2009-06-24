
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NHibernate
{

	/// <summary>
	/// Criteria&lt;T&gt; is an API for retrieving entities by composing
	/// <see cref="Criterion.Expression" /> objects expressed using Lambda expression syntax.
	/// </summary>
	/// <remarks>
	/// <code>
	/// IList&lt;Cat&gt cats = session.QueryOver&lt;Cat&gt;() 
	///     .Add( c =&gt; c.Name == "Tigger" ) 
	///     .Add( c =&gt; c.Weight > minWeight ) ) 
	///     .List(); 
	/// </code>
	/// </remarks>
	public interface ICriteria<T>
	{

		/// <summary>
		/// Add criterion expressed as a lambda expression
		/// </summary>
		/// <param name="expression">Lambda expression</param>
		/// <returns>criteria instance</returns>
		ICriteria<T> Add(Expression<Func<T, bool>> expression);

		/// <summary>
		/// Get the results of the root type and fill the <see cref="IList&lt;T&gt;"/>
		/// </summary>
		/// <param name="results">The list filled with the results.</param>
		IList<T> List();

	}

}
