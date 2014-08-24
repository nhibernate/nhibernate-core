using System.Data;

namespace NHibernate.Id.Insert
{
	public interface IBinder
	{
		object Entity { get;}
		void BindValues(IDbCommand cm);
	}
}