namespace NHibernate.Event
{
	public partial interface IFlushEntityEventListener
	{
		void OnFlushEntity(FlushEntityEvent @event);
	}
}