using System;

namespace NHibernate.Transaction
{
	public interface ISynchronization
	{
		void AfterCompletion(bool successful);
	}
}
