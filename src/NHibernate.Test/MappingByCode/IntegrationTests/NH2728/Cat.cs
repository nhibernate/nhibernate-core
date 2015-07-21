using System;

namespace NHibernate.Test.MappingByCode.IntegrationTests.NH2728
{
	public class Cat : IAnimal
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual DateTime Born { get; set; }
	}
}