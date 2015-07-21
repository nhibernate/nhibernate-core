using System;
using System.Collections.Generic;
using System.Diagnostics;

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
		private ISet<Detail> _details = new HashSet<Detail>();
		private ISet<Detail> _moreDetails = new HashSet<Detail>();
		private ISet<Master> _incoming = new HashSet<Master>();
		private ISet<Master> _outgoing = new HashSet<Master>();
		private string _name = "master";
#pragma warning disable 169
		private DateTime version;
#pragma warning restore 169
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

		public ISet<Detail> Details
		{
			get { return _details; }
			set
			{
				Trace.WriteLine("Details assigned");
				_details = value;
			}
		}

		public ISet<Detail> MoreDetails
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

		public ISet<Master> Incoming
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

		public ISet<Master> Outgoing
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