namespace NHibernate.Cfg.MappingSchema
{
	public partial class HbmOneToOne : AbstractDecoratable, IEntityPropertyMapping
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
			get { return true; }
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