using System;

namespace NHibernate.Mapping
{
	public interface ITableOwner
	{
		Table Table { set; }
	}
}