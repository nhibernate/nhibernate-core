namespace NHibernate.Test.NHSpecificTest.GH2445
{
	public class UnsignedEntity
	{
		public virtual uint Id { get; set; }

		public virtual short Short { get; set; }

		public virtual int Integer { get; set; }

		public virtual long Long { get; set; }

		public virtual uint? NullableNumber { get; set; }

		public virtual ushort ShortNumber { get; set; }

		public virtual ushort? NullableShortNumber { get; set; }

		public virtual ulong LargeNumber { get; set; }

		public virtual ulong? NullableLargeNumber { get; set; }
	}
}
