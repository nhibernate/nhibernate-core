namespace NHibernate.Cfg.MappingSchema
{
	public partial class HbmProperty : IEntityPropertyMapping
	{
		#region Implementation of IEntityPropertyMapping

		public string Name
		{
			get { return name; }
		}

		#endregion
	}
}