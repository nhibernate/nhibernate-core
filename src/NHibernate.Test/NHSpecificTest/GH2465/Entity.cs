using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.GH2465
{
	public class Entity
	{
		private readonly IList<string> _identityNames = new List<string>();

		public virtual Guid Id { get; set; }

		public virtual IList<string> IdentityNames => _identityNames;
	}
}
