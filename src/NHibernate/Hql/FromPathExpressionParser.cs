using System;

using NHibernate.Type;
using NHibernate.Sql;

namespace NHibernate.Hql {
	/// <summary>
	/// FromPathExpressionParser
	/// </summary>
	public class FromPathExpressionParser : PathExpressionParser {

		public override void End(QueryTranslator q) {
			if ( !IsCollectionValued ) {
				IType type = GetPropertyType(q);
				if ( type.IsEntityType ) {
					// "finish off" the join
					Token(".", q);
					Token(null, q);
				}else if ( type.IsPersistentCollectionType ) {
					// default to element set if no elements() specified
					Token(".", q);
					Token(CollectionElements, q);
				}
			}
			base.End(q);
		}
		
		protected override void SetExpectingCollectionIndex() {
			throw new QueryException("expecting .elements or .indices after collection path expression in from");
		}
	}
}