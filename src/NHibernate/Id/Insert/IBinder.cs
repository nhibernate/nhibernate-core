using System.Data.Common;

namespace NHibernate.Id.Insert
{
	public partial interface IBinder
	{
		object Entity { get;}
		void BindValues(DbCommand cm);
	}
}