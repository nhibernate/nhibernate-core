using System;

namespace NHibernate.Eg 
{
	
	public class Edge 
	{
		private float length;
		private string name;
		private float capacity;
		private Vertex source;
		private Vertex sink;
		private long key;
		private DateTime creationDate = DateTime.Now;

		public float Capacity 
		{
			get { return capacity; }
			set { capacity = value; }
		}

		public float Length 
		{
			get { return length; }
			set { length = value; }
		}

		public string Name 
		{
			get { return name; }
			set { name = value; }
		}

		public Vertex Source 
		{
			get { return source; }
			set { source = value; }
		}

		public Vertex Sink 
		{
			get { return sink; }
			set { sink = value; }
		}

		public long Key 
		{
			get { return key; }
			set { key = value; }
		}

		public DateTime CreationDate 
		{
			get { return creationDate; }
			set { creationDate = value; }
		}
	}
}
