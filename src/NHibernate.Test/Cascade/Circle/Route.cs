using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.Cascade.Circle
{
	public class Route
	{
		private long routeId;
		private long version;
		private ISet<Node> nodes = new HashSet<Node>();
		private ISet<Vehicle> vehicles = new HashSet<Vehicle>();
		private string name;
		private string transientField = null;
		
		public virtual long RouteId
		{
			get { return routeId; }
			set { routeId = value; }
		}
		
		public virtual long Version
		{
			get { return version; }
			set { version = value; }
		}
		
		public virtual ISet<Node> Nodes
		{
			get { return nodes; }
			set { nodes = value; }
		}
		
		public virtual ISet<Vehicle> Vehicles
		{
			get { return vehicles; }
			set { vehicles = value; }
		}
		
		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}
		
		public virtual string TransientField
		{
			get { return transientField; }
			set { transientField = value; }
		}
		
		public override string ToString()
		{
			var buffer = new StringBuilder();
		
			buffer.AppendFormat("Route name: {0}, id: {1}, transientField: {2}", name, routeId, transientField);
		
			foreach(Node node in nodes)
				buffer.AppendFormat("Node: {0}", node.ToString());

			foreach(Vehicle vehicle in vehicles)
				buffer.AppendFormat("Vehicle: {0}", vehicle.ToString());
			
			return buffer.ToString();
		}
	}
}