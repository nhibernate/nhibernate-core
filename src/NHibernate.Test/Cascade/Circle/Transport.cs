using System;

namespace NHibernate.Test.Cascade.Circle
{
	public class Transport
	{
		private long transportId;
		private long version;
		private string name;
		private Node pickupNode = null;
		private Node deliveryNode = null;
		private Vehicle vehicle;
		private string transientField = "transport original value";
		
		public virtual long TransportId
		{
			get { return transportId; }
			set { transportId = value; }
		}
		
		public virtual long Version
		{
			get { return version; }
			set { version = value; }
		}
		
		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}
		
		public virtual Node PickupNode
		{
			get { return pickupNode; }
			set { pickupNode = value; }
		}
		
		public virtual Node DeliveryNode
		{
			get { return deliveryNode; }
			set { deliveryNode = value; }
		}
		
		public virtual Vehicle Vehicle
		{
			get { return vehicle; }
			set { vehicle = value; }
		}
		
		public virtual string TransientField
		{
			get { return transientField; }
			set { transientField = value; }
		}

		public override string ToString()
		{
			return String.Format("{0} id: {1}", name, transportId);
		}
	}
}