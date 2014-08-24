using System;
using System.Collections.Generic;

namespace NHibernate.Test.Cascade.Circle
{
	public class Vehicle
	{
		private long vehicleId;
		private long version;
		private string name;
		private ISet<Transport> transports = new HashSet<Transport>();
		private Route route;
		private string transientField = "vehicle original value";
		
		public virtual long VehicleId
		{
			get { return vehicleId; }
			set { vehicleId = value; }
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
		
		public virtual ISet<Transport> Transports
		{
			get { return transports; }
			set { transports = value; }
		}
		
		public virtual Route Route
		{
			get { return route; }
			set { route = value; }
		}
		
		public virtual string TransientField
		{
			get { return transientField; }
			set { transientField = value; }
		}
		
		public override string ToString()
		{
			return String.Format("{0} id: {1}", name, vehicleId);
		}
	}
}