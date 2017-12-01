using System;
using System.Linq.Expressions;
using NHibernate.Criterion;
using NHibernate.Impl;

namespace NHibernate.Test.NHSpecificTest.GH948
{
	public static class Projections
	{
		/// <summary>
		/// Returns the root entity.
		/// </summary>
		/// <returns>The root entity.</returns>
		public static EntityProjection RootEntity()
		{
			return new EntityProjection();
		}

		/// <summary>
		/// Returns an aliased entity.
		/// </summary>
		/// <param name="type">The type of the entity.</param>
		/// <param name="alias">The alias of the entity.</param>
		/// <returns>A projection of the entity.</returns>
		public static EntityProjection Entity(System.Type type, string alias)
		{
			return new EntityProjection(type, alias);
		}

		/// <summary>
		/// Returns an aliased entity.
		/// </summary>
		/// /// <typeparam name="T">The type of the entity.</typeparam>
		/// <param name="alias">The alias of the entity.</param>
		/// <returns>A projection of the entity.</returns>
		public static EntityProjection Entity<T>(string alias)
		{
			return Entity(typeof(T), alias);
		}

		/// <summary>
		/// Returns an aliased entity.
		/// </summary>
		/// /// <typeparam name="T">The type of the entity.</typeparam>
		/// <param name="alias">The alias of the entity.</param>
		/// <returns>A projection of the entity.</returns>
		public static EntityProjection Entity<T>(Expression<Func<T>> alias)
		{
			return Entity(typeof(T), ExpressionProcessor.FindMemberExpression(alias.Body));
		}
	}
}
