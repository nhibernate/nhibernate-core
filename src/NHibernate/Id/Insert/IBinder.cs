using System.Data.Common;

namespace NHibernate.Id.Insert
{
	public interface IBinder
	{
		object Entity { get;}
		void BindValues(DbCommand cm);
	}
}