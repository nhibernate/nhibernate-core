using System;
using System.Collections.Generic;

namespace NHibernate.Test.Cascade.Circle
{
	public class Tour
	{
		private long tourId;
		private long version;
		private string name;
		private ISet<Node> nodes = new HashSet<Node>();
		
		public virtual long TourId
		{
			get { return tourId; }
			set { tourId = value; }
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
		
		public virtual ISet<Node> Nodes
		{
			get { return nodes; }
			set { nodes = value; }
		}
	}
}