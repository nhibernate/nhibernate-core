using NHibernate.Dialect;

namespace NHibernate.Test.NHSpecificTest.NH3202
{
	public class OffsetStartsAtOneTestDialect : MsSql2008Dialect
	{
		public bool ForceOffsetStartsAtOne { get; set; }
		public override bool OffsetStartsAtOne { get { return ForceOffsetStartsAtOne; } }
	}
}