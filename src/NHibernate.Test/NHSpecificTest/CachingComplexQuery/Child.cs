using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.CachingComplexQuery
{
	public class Child
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }

		public virtual Person Parent { get; set; }

		public virtual ISet<Pet> Pets
		{
			get => _pets ?? (_pets = new HashSet<Pet>());
			set => _pets = value;
		}
		private ISet<Pet> _pets;
	}
}
