using System;
using System.Collections;

namespace NHibernate.DomainModel.NHSpecific
{
	/// <summary>
	/// Summary description for Parent.
	/// </summary>
	public class Parent
	{
		private int _id;
		private string _adultName;
		private IDictionary _children;
		private IDictionary _adultFriends;


		public Parent()
		{
			_adultFriends = new SortedList(new ParentComparer());
		}

		public int Id 
		{
			get {return _id;}
			set {_id = value;}
		}

		public string AdultName 
		{
			get {return _adultName;}
			set {_adultName = value;}
		}

		public IDictionary Children 
		{
			get {return _children;}
			set {_children = value;}
		}

		public IDictionary AdultFriends 
		{
			get {return _adultFriends;}
			set {_adultFriends = value;}
		}

		public void AddFriend(Parent friend) 
		{
			_adultFriends.Add(friend, friend);
		}

	}
}
