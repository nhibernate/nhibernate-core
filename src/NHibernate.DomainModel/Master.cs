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
		private Iesi.Collections.ISet _details = new Iesi.Collections.HashedSet();
		private Iesi.Collections.ISet _moreDetails = new Iesi.Collections.HashedSet();
		private Iesi.Collections.ISet _incoming = new Iesi.Collections.HashedSet();
		private Iesi.Collections.ISet _outgoing = new Iesi.Collections.HashedSet();
		private string _name = "master";
		// since this is a version column a value should be set.  NHibernate is unable
		// to set it right now because it checks to see if value==null -> since this 
		// is a struct it is not null...
		private DateTime stamp = DateTime.Now;
		// private BigDecimal bigDecimal = new BigDecimal("1234.123"); TODO: how to do in .net
		private int _x;
		
		public Master OtherMaster
		{
			get { return _otherMaster; }
			set { _otherMaster = value; }
		}

		public void AddDetail(Detail d) 
		{
			_details.Add( d );
		}

		public void RemoveDetail(Detail d) 
		{
			_details.Remove(d);
		}

		public Iesi.Collections.ISet Details
		{
			get { return _details; }
			set { _details = value; }
		}
		
		public Iesi.Collections.ISet MoreDetails
		{
			get { return _moreDetails; }
			set { _moreDetails = value; }
		}

		public void AddIncoming(Master m) 
		{
			_incoming.Add( m );
		}
	
		public void RemoveIncoming(Master m) 
		{
			_incoming.Remove(m);
		}

		public Iesi.Collections.ISet Incoming
		{
			get { return _incoming; }
			set { _incoming = value; }
		}

		public void AddOutgoing(Master m) 
		{
			_outgoing.Add( m );
		}

		public void RemoveOutgoing(Master m) 
		{
			_outgoing.Remove(m);
		}

		public Iesi.Collections.ISet Outgoing
		{
			get { return _outgoing; }
			set { _outgoing = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public int X
		{
			get { return _x; }
			set { _x = value; }
		}

	}
}
