using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.GH3421
{
	class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }

		private IDictionary<string, object> _attributes;
		public virtual IDictionary<string, object> Attributes {
			get {
				if (_attributes == null)
					_attributes = new Dictionary<string, object>();
				return _attributes;
			}
			set => _attributes = value;
		}
	}
}
