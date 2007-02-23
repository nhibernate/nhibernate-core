using System;
using System.Diagnostics;

using Iesi.Collections;

namespace NHibernate.DomainModel
{
	/// <summary>
	/// Summary description for Master.
	/// </summary>
	[Serializable]
	public class Master : INamed
	{
		private static object _emptyObject = new object();

		private Master _otherMaster;
		private ISet _details = new HashedSet();
		private ISet _moreDetails = new HashedSet();
		private ISet _incoming = new HashedSet();
		private ISet _outgoing = new HashedSet();
		private string _name = "master";
		private DateTime version;
		//private BigDecimal bigDecimal = new BigDecimal("1234.123"); //TODO: how to do in .net
		private int _x;

		public Master OtherMaster
		{
			get { return _otherMaster; }
			set { _otherMaster = value; }
		}

		public void AddDetail(Detail d)
		{
			_details.Add(d);
		}

		public void RemoveDetail(Detail d)
		{
			_details.Remove(d);
		}

		public ISet Details
		{
			get { return _details; }
			set
			{
				Trace.WriteLine("Details assigned");
				_details = value;
			}
		}

		public ISet MoreDetails
		{
			get { return _moreDetails; }
			set { _moreDetails = value; }
		}

		public void AddIncoming(Master m)
		{
			_incoming.Add(m);
		}

		public void RemoveIncoming(Master m)
		{
			_incoming.Remove(m);
		}

		public ISet Incoming
		{
			get { return _incoming; }
			set { _incoming = value; }
		}

		public void AddOutgoing(Master m)
		{
			_outgoing.Add(m);
		}

		public void RemoveOutgoing(Master m)
		{
			_outgoing.Remove(m);
		}

		public ISet Outgoing
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