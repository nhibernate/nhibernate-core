
namespace NHibernate.Test.FetchLazyProperties
{
	public abstract class Animal
	{
		public virtual int Id { get; set; }

		public virtual int Formula { get; set; }

		public virtual string Name { get; set; }

		public virtual Address Address { get; set; }

		public virtual byte[] Image { get; set; }

		public virtual Person Owner { get; set; }
	}
}
