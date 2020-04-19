using System.Collections.Generic;

namespace NHibernate.DomainModel.Northwind.Entities
{
	public class AnotherEntityRequired
	{
		public virtual int Id { get; set; }

		public virtual string Output { get; set; }

		public virtual string Input { get; set; }

		public virtual Address Address { get; set; }

		public virtual AnotherEntityNullability InputNullability { get; set; }

		public virtual string NullableOutput { get; set; }

		public virtual AnotherEntityRequired NullableAnotherEntityRequired { get; set; }

		public virtual int? NullableAnotherEntityRequiredId { get; set; }

		public virtual ISet<AnotherEntity> RelatedItems { get; set; } = new HashSet<AnotherEntity>();

		public virtual bool? NullableBool { get; set; }
	}

	public enum AnotherEntityNullability
	{
		False = 0,
		True = 1
	}
}
