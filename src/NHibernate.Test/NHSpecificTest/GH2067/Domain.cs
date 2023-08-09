using System;

namespace NHibernate.Test.NHSpecificTest.GH2067
{
	public interface ICat
	{
		Guid Id { get; set; }
		string Name { get; set; }
	}

	public class Cat : ICat
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}

	public interface IPet
	{
		string OwnerName { get; set; }
	}

	public interface IDomesticCat : IPet, ICat
	{
	}

	public sealed class DomesticCat : Cat, IDomesticCat
	{
		public string OwnerName { get; set; }
	}
}
