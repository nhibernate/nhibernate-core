using System;
using System.Collections;

namespace NHibernate.DomainModel.NHSpecific
{
	/// <summary>
	/// Summary description for Team.
	/// </summary>
	public class Team	
	{
		private int _id;
		private string _name;
		private IList _players;

		public Team() 
		{
		}

		public int Id 
		{
			get {return _id;}
			set {_id = value;}
		}

		public string Name 
		{
			get {return _name;}
			set {_name = value;}
		}

		public IList Players 
		{
			get {return _players;}
			set {_players = value;}
		}

	}
}
