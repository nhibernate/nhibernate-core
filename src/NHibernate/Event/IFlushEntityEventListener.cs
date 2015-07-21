namespace NHibernate.Event
{
	public interface IFlushEntityEventListener
	{
		void OnFlushEntity(FlushEntityEvent @event);
	}
}