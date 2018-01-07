using System;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Collection;

namespace NHibernate.Test.NHSpecificTest.GH1515
{
	class LayzInitializationTestCollection : ILazyInitializedCollection
	{

		private bool isInitialized;

		public LayzInitializationTestCollection(bool isInitialized)
		{
			this.isInitialized = isInitialized;
		}

		public Task ForceInitializationAsync(CancellationToken cancellationToken)
		{
			return Task.Run(() => ForceInitialization(), cancellationToken);
		}

		public bool WasInitialized { get { return isInitialized;  } }
		public void ForceInitialization()
		{
			isInitialized = true;
		}
	}
}
