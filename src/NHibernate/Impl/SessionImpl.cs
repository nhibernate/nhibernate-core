using System;

namespace NHibernate.Impl {
	
	public class SessionImpl {
		

		public interface Executable {
			void Execute();
			void AfterTransactionCompletion();
			object[] PropertySpaces { get; }
		}
	}
}
