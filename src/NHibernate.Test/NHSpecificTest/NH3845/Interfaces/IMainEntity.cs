using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH3845.Interfaces
{
	public interface IMainEntity
	{
		int MainEntityId { get; set; }
		string Text { get; set; }
		ISet<IPropertyEntityBase> Properties { get; }
		ISet<ISeparateEntity> SeparateEntities { get; }
	}
}
