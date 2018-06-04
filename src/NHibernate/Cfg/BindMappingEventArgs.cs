using System;
using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Cfg
{
	public class BindMappingEventArgs : EventArgs
	{
		//6.0 TODO: Remove
		internal Lazy<Dialect.Dialect> LazyDialect;

		[Obsolete("Please use constructor without a dialect parameter.")]
		public BindMappingEventArgs(Dialect.Dialect dialect, HbmMapping mapping, string fileName)
			: this(mapping, fileName)
		{
			LazyDialect = new Lazy<Dialect.Dialect>(() => dialect);
		}

		public BindMappingEventArgs(HbmMapping mapping, string fileName)
		{
			Mapping = mapping;
			FileName = fileName;
		}

		//Since v5.2
		[Obsolete("This property will be removed in a future version.")]
		public Dialect.Dialect Dialect => LazyDialect.Value;
		public HbmMapping Mapping { get; }
		public string FileName { get; }
	}
}