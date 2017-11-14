using System;

namespace NHibernate.Test.NHSpecificTest.NH3850
{
	public abstract class DomainClassBase
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual int? Integer { get; set; }
		public virtual long? Long { get; set; }
		public virtual decimal? Decimal { get; set; }
		public virtual double? Double { get; set; }
		public virtual DateTime? DateTime { get; set; }
		public virtual DateTimeOffset? DateTimeOffset { get; set; }
		public virtual decimal NonNullableDecimal { get; set; }
	}

	public class DomainClassAExtendingB : DomainClassBExtendedByA
	{
	}

	public class DomainClassBExtendedByA : DomainClassBase
	{
	}

	public class DomainClassCExtendedByD : DomainClassBase
	{
	}

	public class DomainClassDExtendingC : DomainClassCExtendedByD
	{
	}

	public class DomainClassE : DomainClassBase
	{
	}

	public class DomainClassF : DomainClassBase
	{
	}

	public class DomainClassGExtendedByH : DomainClassBase
	{
	}

	public class DomainClassHExtendingG : DomainClassGExtendedByH
	{
	}
}