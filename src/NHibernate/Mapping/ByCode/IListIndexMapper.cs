using System;

namespace NHibernate.Mapping.ByCode
{
	public interface IListIndexMapper
	{
		void Column(string columnName);
		void Base(int baseIndex);
		void Column(Action<IColumnMapper> columnMapper);
	}
}