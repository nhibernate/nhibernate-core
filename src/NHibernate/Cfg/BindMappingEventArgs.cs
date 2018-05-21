using System;
using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Cfg
{
	public class BindMappingEventArgs: EventArgs
	{
		[Obsolete("Please use constructor without a dialect parameter.", true)]
		public BindMappingEventArgs(Dialect.Dialect dialect, HbmMapping mapping, string fileName) : this(mapping, fileName)
		{
			Dialect = dialect;
		}

		public BindMappingEventArgs(HbmMapping mapping, string fileName)
		{
			Mapping = mapping;
			FileName = fileName;
		}

		//Since v5.2
		[Obsolete("This property will be removed in a future version.", true)]
		public Dialect.Dialect Dialect { get; }
		public HbmMapping Mapping { get; }
		public string FileName { get; }
	}
}