using System;
using System.Collections;

namespace NHibernate.DomainModel
{
	/// <summary>
	/// Summary description for Master.
	/// </summary>
	[Serializable]
	public class Master
	{
		private static object _emptyObject = new object();

		private Master _otherMaster;
		private IDictionary _details = new Hashtable();
		private IDictionary _moreDetails = new Hashtable();
		private IDictionary _incoming = new Hashtable();
		private IDictionary _outgoing = new Hashtable();
		private string _name = "master";
		private DateTime _stamp;
		// private BigDecimal bigDecimal = new BigDecimal("1234.123"); TODO: how to do in .net
		private int _x;
		
		public Master OtherMaster
		{
			get { return _otherMaster; }
			set { _otherMaster = value; }
		}

		public void AddDetail(Detail d) 
		{
			_details.Add(d, _emptyObject);
		}

		public void RemoveDetail(Detail d) 
		{
			_details.Remove(d);
		}

		public IDictionary Details
		{
			get { return _details; }
			set { _details = value; }
		}
		
		public IDictionary MoreDetails
		{
			get { return _moreDetails; }
			set { _moreDetails = value; }
		}

		public void AddIncoming(Master m) 
		{
			_incoming.Add(m, _emptyObject);
		}
	
		public void RemoveIncoming(Master m) 
		{
			_incoming.Remove(m);
		}

		public IDictionary Incoming
		{
			get { return _incoming; }
			set { _incoming = value; }
		}

		public void AddOutgoing(Master m) 
		{
			_outgoing.Add(m, _emptyObject);
		}

		public void RemoveOutgoing(Master m) 
		{
			_outgoing.Remove(m);
		}

		public IDictionary Outgoing
		{
			get { return _outgoing; }
			set { _outgoing = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public DateTime Stamp
		{
			get { return _stamp; }
			set { _stamp = value; }
		}

		public int X
		{
			get { return _x; }
			set { _x = value; }
		}

	}
}
