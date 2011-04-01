using System;
using System.Linq;
using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Mapping.ByCode.Impl
{
	public class NaturalIdMapper : AbstractBasePropertyContainerMapper, INaturalIdMapper
	{
		private readonly HbmNaturalId naturalIdmapping;

		public NaturalIdMapper(System.Type rootClass, HbmNaturalId naturalIdmapping, HbmMapping mapDoc) : base(rootClass, mapDoc)
		{
			this.naturalIdmapping = naturalIdmapping;
		}

		#region Overrides of AbstractBasePropertyContainerMapper

		protected override void AddProperty(object property)
		{
			if (property == null)
			{
				throw new ArgumentNullException("property");
			}
			var toAdd = new[] {property};
			naturalIdmapping.Items = naturalIdmapping.Items == null ? toAdd : naturalIdmapping.Items.Concat(toAdd).ToArray();
		}

		#endregion

		#region Implementation of INaturalIdAttributesMapper

		public void Mutable(bool isMutable)
		{
			naturalIdmapping.mutable = isMutable;
		}

		#endregion
	}
}