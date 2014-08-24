using System.Collections.Generic;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping;
using NHibernate.Util;
using System;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class FiltersBinder: Binder
	{
		private readonly IFilterable filterable;

		public FiltersBinder(IFilterable filterable, Mappings mappings) : base(mappings)
		{
			this.filterable = filterable;
		}

		public void Bind(IEnumerable<HbmFilter> filters)
		{
			Bind(filters, (name, condition) => filterable.AddFilter(name, condition));
		}

		public void Bind(IEnumerable<HbmFilter> filters, Action<string, string> addFilterDelegate)
		{
			if (filters == null)
			{
				return;
			}
			foreach (var filter in filters)
			{
				string name = filter.name;
				if (name.IndexOf('.') > -1)
					throw new MappingException("Filter name can't contain the character '.'(point): " + name);

				string condition = (filter.Text.LinesToString() ?? string.Empty).Trim();
				if (string.IsNullOrEmpty(condition))
				{
					condition = filter.condition;
				}
				if (string.IsNullOrEmpty(condition))
				{
					var fdef = mappings.GetFilterDefinition(name);
					if (fdef != null)
					{
						// where immediately available, apply the condition
						condition = fdef.DefaultFilterCondition;
					}
				}

				mappings.ExpectedFilterDefinition(filterable, name, condition);

				log.Debug(string.Format("Applying filter [{0}] as [{1}]", name, condition));
				addFilterDelegate(name, condition);
			}
		}
	}
}