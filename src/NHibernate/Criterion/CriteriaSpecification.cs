using NHibernate.SqlCommand;
using NHibernate.Transform;

namespace NHibernate.Criterion
{
	public static class CriteriaSpecification
	{
		/// <summary> The alias that refers to the "root" entity of the criteria query.</summary>
		public readonly static string RootAlias = "this";

		/// <summary> Each row of results is a <see cref="System.Collections.IDictionary"/> from alias to entity instance</summary>
		public readonly static IResultTransformer AliasToEntityMap;

		/// <summary> Each row of results is an instance of the root entity</summary>
		public readonly static IResultTransformer RootEntity;

		/// <summary> Each row of results is a distinct instance of the root entity</summary>
		public readonly static IResultTransformer DistinctRootEntity;

		/// <summary> This result transformer is selected implicitly by calling <see cref="ICriteria.SetProjection"/> </summary>
		public readonly static IResultTransformer Projection;

		/// <summary> Specifies joining to an entity based on an inner join.</summary>
		public readonly static JoinType InnerJoin;

		/// <summary> Specifies joining to an entity based on a full join.</summary>
		public readonly static JoinType FullJoin;

		/// <summary> Specifies joining to an entity based on a left outer join.</summary>
		public readonly static JoinType LeftJoin;

		static CriteriaSpecification()
		{
			AliasToEntityMap = new AliasToEntityMapResultTransformer();
			RootEntity = new RootEntityResultTransformer();
			DistinctRootEntity = new DistinctRootEntityResultTransformer();
			Projection = new PassThroughResultTransformer();
			InnerJoin = JoinType.InnerJoin;
			FullJoin = JoinType.FullJoin;
			LeftJoin = JoinType.LeftOuterJoin;
		}
	}
}
