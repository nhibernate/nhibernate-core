//$Id$
using System;
using System.Collections;

namespace NHibernate.Hql {	
	public class SelectPathExpressionParser : PathExpressionParser {
		
		
		public override void End(QueryTranslator q) {
			if (currentProperty != null && !q.IsShallowQuery) {
				// "finish off" the join
				Token(".", q);
				Token(null, q);
			}
			base.End(q);
		}
		
		protected override void SetExpectingCollectionIndex() {
			throw new QueryException("expecting .elements or .indices after collection path expression in select");
		}

		public string SelectName {
			get	{ return currentName; }
		}
	}
}