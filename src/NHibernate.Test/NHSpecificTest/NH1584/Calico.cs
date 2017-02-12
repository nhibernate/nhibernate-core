namespace NHibernate.Test.NHSpecificTest.NH1584
{
	/// <summary>
	/// This class describes a few of the attributes possible for a &quot;Calico&quot; coat.
	/// </summary>
	public class Calico : CoatPattern
	{
		public Calico()
		{
			Description = "Orange, black and white coloration.";
		}

		public virtual Female Cat { get; set; }

		public virtual bool HasPatches { get; set; }

		public virtual bool IsMottled { get; set; }
	}
}