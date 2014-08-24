using System.Collections.Generic;
using System.Linq;

namespace NHibernate.Test.NHSpecificTest.NH2772
{
	public class Trip
	{
		private readonly IList<Trackpoint> _Trackpoints;

		public Trip()
		{
			_Trackpoints = new List<Trackpoint>();
		}

		public virtual string Header { get; set; }
		public virtual int Id { get; set; }
		public virtual byte[] Image { get; set; }

		public virtual IEnumerable<Trackpoint> Trackpoints
		{
			get { return _Trackpoints.AsEnumerable(); }
		}

		public virtual Trackpoint CreateTrackpoint()
		{
			var tp = new Trackpoint(this);
			_Trackpoints.Add(tp);
			return tp;
		}
	}

	public class Trackpoint
	{
		protected Trackpoint()
		{
		}

		public Trackpoint(Trip trip)
			: this()
		{
			Trip = trip;
		}

		public virtual int Id { get; set; }

		public virtual double Lat { get; set; }
		public virtual double Lon { get; set; }

		public virtual Trip Trip { get; set; }
	}
}
