using System;
using System.Collections;

namespace NHibernate.Eg 
{
	public class Vertex 
	{
		private IList incoming = new ArrayList();
		private IList outgoing = new ArrayList();
		private string name;
		private long key;
		private int version;
		private DateTime creationDate = DateTime.Now;

		protected IList Incoming 
		{
			get { return incoming; }
			set { incoming = value; }
		}

		protected IList Outgoing 
		{
			get { return outgoing; }
			set { outgoing = value; }
		}

		public void AddIncoming(Edge e) 
		{
			incoming.Add(e);
			e.Sink = this;
		}

		public void AddOutgoing(Edge e) 
		{
			outgoing.Add(e);
			e.Source = this;
		}

		public void RemoveIncoming(Edge e) 
		{
			incoming.Remove(e);
			e.Sink = null;
		}

		public void RemoveOutgoing(Edge e) 
		{
			outgoing.Remove(e);
			e.Source = null;
		}

		public string Name 
		{
			get { return name; }
			set { name = value; }
		}

		public virtual float ExcessCapacity 
		{
			get 
			{
				float excess = 0.0f;
				foreach( Edge edge in incoming) 
				{
					excess -= edge.Capacity;
				}
				foreach( Edge edge in outgoing) 
				{
					excess += edge.Capacity;
				}
				return excess;
			}
		}

		public long Key 
		{
			get { return key; }
			set { key = value; }
		}

		public int Version 
		{
			get { return version; }
			set { version = value; }
		}

		public DateTime CreationDate 
		{
			get { return creationDate; }
			set { creationDate = value; }
		}
	}
}
