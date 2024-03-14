using System.Data.Common;

namespace NHibernate.Driver
{
#if NET6_0_OR_GREATER
	//TODO: Include in IDriveConnectionCommandProvider for NET6_0_OR_GREATER
	internal interface IDriveConnectionCommandProviderWithBatchSupport : IDriveConnectionCommandProvider
	{
		DbBatch CreateBatch();
		bool CanCreateBatch { get; }

	}
#endif

}
