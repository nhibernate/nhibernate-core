namespace NHibernate.Test.NHSpecificTest.NH1092
{
	public class SubscriberAbstract
	{
		public virtual string Username { get; set; }
	}

	public class Subscriber1 : SubscriberAbstract { }

	public class Subscriber2 : SubscriberAbstract { }
}