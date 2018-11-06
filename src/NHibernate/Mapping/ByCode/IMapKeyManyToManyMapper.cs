namespace NHibernate.Mapping.ByCode
{
	// 6.0 TODO: inherit from IColumnsAndFormulasMapper
	public interface IMapKeyManyToManyMapper : IColumnsMapper
	{
		void ForeignKey(string foreignKeyName);
		// 6.0 TODO: remove once inherited from IColumnsAndFormulasMapper
		void Formula(string formula);
	}
}
