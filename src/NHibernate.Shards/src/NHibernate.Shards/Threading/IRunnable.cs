namespace NHibernate.Shards.Threading
{
	/// <summary>
	/// The <c>IRunnable</c> interface should be implemented by any class 
	/// whose instances are intended to be executed by a thread
	/// </summary>
	public interface IRunnable
	{
		/// <summary>
		/// When an object implementing interface Runnable is used to create a thread, 
		/// starting the thread causes the object's run method to be called in that 
		/// separately executing thread.
		/// </summary>
		void Run();
	}
}