using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode.Impl;
using NHibernate.Util;

namespace NHibernate.Mapping.ByCode
{
	public class Import
	{
		private System.Type importType;
		private string rename;

		public Import(System.Type importType, string rename)
		{
			this.importType = importType;
			this.rename = rename;
		}

		public void AddToMapping(HbmMapping hbmMapping)
		{
			var hbmImport =
				new HbmImport
				{
					@class = importType.GetShortClassName(hbmMapping),
					rename = this.rename
				};

			hbmMapping.import = ArrayHelper.Append(hbmMapping.import, hbmImport);
		}
	}
}
