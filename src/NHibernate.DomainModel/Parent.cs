using System;
using System.Collections;

namespace NHibernate.DomainModel
{
	//TODO: replace this with H2.0.3 version
	/// <summary>
	/// Summary description for Parent.
	/// </summary>
	public class Parent
	{
		private int id;
		private string adultName;
		private IDictionary children;
		private IDictionary adultFriends;


		public Parent()
		{
			adultFriends = new SortedList(new ParentComparer());
		}

		public int Id {
			get {return id;}
			set {id = value;}
		}

		public string AdultName {
			get {return adultName;}
			set {adultName = value;}
		}

		public IDictionary Children {
			get {return children;}
			set {children = value;}
		}

		public IDictionary AdultFriends {
			get {return adultFriends;}
			set {adultFriends = value;}
		}

		public void AddFriend(Parent friend) {
			adultFriends.Add(friend, friend);
		}

	}
}
