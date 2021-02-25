using System;

namespace NHibernate.Test.NHSpecificTest.GH1712
{
	class GenericEntity<TId> where TId : IEquatable<TId>
	{
		public virtual TId Id { get; set; }
	}
}
