using System;
using System.Linq;

namespace NHibernate.Linq
{
	/// <summary>
	/// Provides a static method that enables LINQ syntax for NHibernate Queries.
	/// </summary>
	public static class NHibernateExtensions
	{
		/// <summary>
		/// Creates a new <see cref="T:NHibernate.Linq.NHibernateQueryProvider"/> object used to evaluate an expression tree.
		/// </summary>
		/// <typeparam name="T">An NHibernate entity type.</typeparam>
		/// <param name="session">An initialized <see cref="T:NHibernate.ISession"/> object.</param>
		/// <returns>An <see cref="T:NHibernate.Linq.NHibernateQueryProvider"/> used to evaluate an expression tree.</returns>
		public static IQueryable<T> Linq<T>(this ISession session)
		{
			return new Query<T>(new NHibernateQueryProvider(session));
		}
	}
}