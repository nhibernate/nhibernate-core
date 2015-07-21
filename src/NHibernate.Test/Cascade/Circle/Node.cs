using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.Cascade.Circle
{
	public class Node
	{
		private long nodeId;
		private long version;
		private string name;
		private ISet<Transport> deliveryTransports = new HashSet<Transport>();
		private ISet<Transport> pickupTransports = new HashSet<Transport>();
		private Route route = null;
		private Tour tour;
		private string transientField = "node original value";
		
		public virtual long NodeId
		{
			get { return nodeId; }
			set { nodeId = value; }
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
		
		public virtual ISet<Transport> DeliveryTransports
		{
			get { return deliveryTransports; }
			set { deliveryTransports = value; }
		}
		
		public virtual ISet<Transport> PickupTransports
		{
			get { return pickupTransports; }
			set { pickupTransports = value; }
		}
		
		public virtual Route Route
		{
			get { return route; }
			set { route = value; }
		}
		
		public virtual Tour Tour
		{
			get { return tour; }
			set { tour = value; }
		}
		
		public virtual string TransientField
		{
			get { return transientField; }
			set { transientField = value; }
		}
		
		public override string ToString()
		{
			var buffer = new StringBuilder();
		
			buffer.AppendFormat("{0}, id: {1}", name, nodeId);

			if (route != null)
				buffer.AppendFormat(" route name: {0}, tour name: {1}", route.Name, tour == null ? "null" : tour.Name);
			
			if (deliveryTransports != null)
			{
				foreach(Transport transport in deliveryTransports)
					buffer.AppendFormat("Delivery Transports: {0}", transport.ToString());
			}

			if (route != null)
			{
				foreach(Transport transport in pickupTransports)
					buffer.AppendFormat("Node: {0}", transport.ToString());
			}
			
			return buffer.ToString();
		}
	}
}