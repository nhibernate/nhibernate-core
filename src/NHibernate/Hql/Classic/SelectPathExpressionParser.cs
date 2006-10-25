namespace NHibernate.Hql.Classic
{
	/// <summary></summary>
	public class SelectPathExpressionParser : PathExpressionParser
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="q"></param>
		public override void End( QueryTranslator q )
		{
			if( CurrentProperty != null && !q.IsShallowQuery )
			{
				// "finish off" the join
				Token( ".", q );
				Token( null, q );
			}
			base.End( q );
		}

		/// <summary></summary>
		protected override void SetExpectingCollectionIndex()
		{
			throw new QueryException( "expecting .elements or .indices after collection path expression in select" );
		}

		/// <summary></summary>
		public string SelectName
		{
			get { return CurrentName; }
		}
	}
}