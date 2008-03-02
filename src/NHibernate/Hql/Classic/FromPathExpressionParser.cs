using NHibernate.Persister.Collection;
using NHibernate.Type;

namespace NHibernate.Hql.Classic
{
	/// <summary>
	/// FromPathExpressionParser
	/// </summary>
	public class FromPathExpressionParser : PathExpressionParser
	{
		public override void End(QueryTranslator q)
		{
			if (!IsCollectionValued)
			{
				IType type = PropertyType;
				if (type.IsEntityType)
				{
					// "finish off" the join
					Token(".", q);
					Token(null, q);
				}
				else if (type.IsCollectionType)
				{
					// default to element set if no elements() specified
					Token(".", q);
					Token(CollectionPropertyNames.Elements, q);
				}
			}
			base.End(q);
		}

		protected override void SetExpectingCollectionIndex()
		{
			throw new QueryException("illegal syntax near collection-valued path expression in from: " + CollectionName);
		}
	}
}