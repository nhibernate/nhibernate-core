namespace NHibernate.Test.NHSpecificTest.NH1584
{
	/// <summary>
	/// This class describes a few of the properties of a &quot;Tabby&quot; coat pattern.
	/// </summary>
	public class Tabby : CoatPattern
	{
		public Tabby()
		{
			Description = "A distinctive coat that features stripes, dots, or swirling patterns.";
		}

		public virtual Male Cat { get; set; }

		public virtual bool HasSpots { get; set; }

		public virtual bool HasStripes { get; set; }

		public virtual bool HasSwirls { get; set; }
	}
}