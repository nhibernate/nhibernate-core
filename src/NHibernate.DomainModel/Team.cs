using System;
using System.Collections;

namespace NHibernate.DomainModel {
	/// <summary>
	/// Summary description for Team.
	/// </summary>
	public class Team	{

		private int id;
		private string name;
		private IList players;

		public Team() {
		}

		public int Id {
			get {return id;}
			set {id = value;}
		}

		public string Name {
			get {return name;}
			set {name = value;}
		}

		public IList Players {
			get {return players;}
			set {players = value;}
		}

	}
}
