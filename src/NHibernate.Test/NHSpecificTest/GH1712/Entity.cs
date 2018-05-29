using System;

namespace NHibernate.Test.NHSpecificTest.GH1712
{
	class GenericEntity<TId> : IEquatable<GenericEntity<TId>> where TId : IEquatable<TId>
	{
		public virtual TId Id { get; set; }
		public virtual bool Equals(GenericEntity<TId> other)
		{
			throw new NotImplementedException();
		}
	}
}
