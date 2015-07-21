using System;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;

namespace NHibernate.Test
{
	public static class HbmMappingExtensions
	{
		public static void ShowInConsole(this HbmMapping mapping)
		{
			Console.WriteLine(mapping.AsString());
		}
	}
}