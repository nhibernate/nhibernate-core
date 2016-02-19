using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH3845.Interfaces
{
	public interface IAnotherEntity
	{
		int AnotherEntityId { get; set; }
		string Text { get; set; }
		ISet<IPropertyEntityC> ChildEntities { get; }
	}
}
