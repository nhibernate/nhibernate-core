namespace NHibernate.Mapping.ByCode
{
	public interface IMapKeyManyToManyMapper : IColumnsMapper
	{
		void ForeignKey(string foreignKeyName);
		void Formula(string formula);
	}
}