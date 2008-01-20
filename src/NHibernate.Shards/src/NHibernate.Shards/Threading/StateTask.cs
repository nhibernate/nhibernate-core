namespace NHibernate.Shards.Threading
{
	public enum StateTask
	{
		Running = 1,
		Canceled = 2,
		Ran = 4
	}
}