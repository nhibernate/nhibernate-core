using System;

namespace NHibernate.Mapping.ByCode
{
	public interface IColumnsMapper
	{
		void Column(Action<IColumnMapper> columnMapper);
		void Columns(params Action<IColumnMapper>[] columnMapper);
		void Column(string name);
	}
}