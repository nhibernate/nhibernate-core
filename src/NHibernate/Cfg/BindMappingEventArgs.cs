using System;
using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Cfg
{
	public class BindMappingEventArgs: EventArgs
	{
		public BindMappingEventArgs(Dialect.Dialect dialect, HbmMapping mapping, string fileName)
		{
			Dialect = dialect;
			Mapping = mapping;
			FileName = fileName;
		}

		public Dialect.Dialect Dialect { get; private set; }
		public HbmMapping Mapping { get; private set; }
		public string FileName { get; private set; }
	}
}