using NHibernate.Type;

namespace NHibernate.Hql
{
	/// <summary>
	/// FromPathExpressionParser
	/// </summary>
	public class FromPathExpressionParser : PathExpressionParser
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="q"></param>
		public override void End( QueryTranslator q )
		{
			if( !IsCollectionValued )
			{
				IType type = PropertyType;
				if( type.IsEntityType )
				{
					// "finish off" the join
					Token( ".", q );
					Token( null, q );
				}
				else if( type.IsPersistentCollectionType )
				{
					// default to element set if no elements() specified
					Token( ".", q );
					Token( CollectionElements, q );
				}
			}
			base.End( q );
		}

		/// <summary></summary>
		protected override void SetExpectingCollectionIndex()
		{
			throw new QueryException( "expecting .elements or .indices after collection path expression in from" );
		}
	}
}