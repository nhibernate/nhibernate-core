namespace NHibernate.Mapping.ByCode
{
	// 6.0 TODO: inherit from IColumnsAndFormulasMapper
	public interface IMapKeyManyToManyMapper : IColumnsMapper
	{
		void ForeignKey(string foreignKeyName);
		// 6.0 TODO: remove once inhertied from IColumnsAndFormulasMapper
		void Formula(string formula);
	}
}
