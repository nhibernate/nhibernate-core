//$Id$
using System;
using System.Collections;

namespace NHibernate.Hql
{	
	public class SelectPathExpressionParser : PathExpressionParser
	{
		public string SelectName
		{
			get	{ return currentName; }
			
		}
		
		public override void End(QueryTranslator q)
		{
			if (currentProperty != null && !q.IsShallowQuery())
			{
				// "finish off" the join
				Token(".", q);
				Token(null, q);
			}
			/*if ( isCollectionValued() ) {
			//if ( !q.supportsScalars() ) throw new QueryException("Can't use collection valued property in SELECT: " + currentProperty);
			column = collectionElementColumn;
			type = collectionElementType;
			}*/
			base.End(q);
		}
		
		protected override void  SetExpectingCollectionIndex()
		{
			throw new QueryException("expecting .elements or .indices after collection path expression in select");
		}
	}
}