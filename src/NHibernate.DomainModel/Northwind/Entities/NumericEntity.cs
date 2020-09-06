namespace NHibernate.DomainModel.Northwind.Entities
{
	public class NumericEntity
	{
		public virtual short Short { get; set; }

		public virtual short? NullableShort { get; set; }

		public virtual int Integer { get; set; }

		public virtual int? NullableInteger { get; set; }

		public virtual long Long { get; set; }

		public virtual long? NullableLong { get; set; }

		public virtual decimal Decimal { get; set; }

		public virtual decimal? NullableDecimal { get; set; }

		public virtual float Single { get; set; }

		public virtual float? NullableSingle { get; set; }

		public virtual double Double { get; set; }

		public virtual double? NullableDouble { get; set; }
	}
}
