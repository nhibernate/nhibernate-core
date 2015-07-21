using System;
using System.Collections.Generic;

namespace NHibernate.DomainModel.NHSpecific
{
	/// <summary>
	/// Summary description for Parent.
	/// </summary>
	public class Parent
	{
		private int _id;
		private string _adultName;
		private ISet<object> _children;
		private ISet<Parent> _adultFriends;


		public Parent()
		{
			_adultFriends = new SortedSet<Parent>(new ParentComparer());
		}

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public string AdultName
		{
			get { return _adultName; }
			set { _adultName = value; }
		}

		public ISet<object> Children
		{
			get { return _children; }
			set { _children = value; }
		}

		public ISet<Parent> AdultFriends
		{
			get { return _adultFriends; }
			set { _adultFriends = value; }
		}

		public void AddFriend(Parent friend)
		{
			_adultFriends.Add(friend);
		}
	}
}