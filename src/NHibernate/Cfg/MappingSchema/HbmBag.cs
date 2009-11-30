namespace NHibernate.Cfg.MappingSchema
{
	public partial class HbmBag : AbstractDecoratable, ICollectionPropertyMapping
	{
		#region Implementation of IEntityPropertyMapping

		public string Name
		{
			get { return name; }
		}

		public string Access
		{
			get { return access; }
		}

		public bool OptimisticKock
		{
			get { return optimisticlock; }
		}

		#endregion

		#region Implementation of IReferencePropertyMapping

		public string Cascade
		{
			get { return cascade; }
		}

		#endregion

		#region Overrides of AbstractDecoratable

		protected override HbmMeta[] Metadatas
		{
			get { return meta ?? new HbmMeta[0]; }
		}

		#endregion
	}
}