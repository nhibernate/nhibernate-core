using System;
using NHibernate.Transform;

namespace NHibernate
{
	/// <summary>
	/// Contains static declarations from Criteria interface in Hibernate.
	/// </summary>
	public sealed class CriteriaUtil
	{
		/// <summary>
		/// Each row of results is an <c>IDictionary</c> from alias to entity instance
		/// </summary>
		public static readonly IResultTransformer AliasToEntityMap = Transformers.AliasToEntityMap;

		/// <summary>
		/// Each row of results is an instance of the root entity
		/// </summary>
		public static readonly IResultTransformer RootEntity = new RootEntityResultTransformer();

		/// <summary>
		/// Each row of results is a distinct instance of the root entity
		/// </summary>
		public static readonly IResultTransformer DistinctRootEntity = new DistinctRootEntityResultTransformer();

		/// <summary>
		/// The alias that refers to the "root" entity of the criteria query.
		/// </summary>
		public const string RootAlias = "this";
	}
}