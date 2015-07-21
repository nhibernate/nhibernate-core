namespace NHibernate.Mapping.ByCode
{
	public interface IFilterMapper
	{
		void Condition(string sqlCondition);
	}
}