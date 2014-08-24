using System;
using System.Collections;
using System.Collections.Generic;

namespace NHibernate.DomainModel.NHSpecific
{
	/// <summary>
	/// Summary description for Child.
	/// </summary>
	public class Child
	{
		private int _id;
		private string _fullName;
		private IDictionary<string, Parent> _parents;
		private SexType _sex;

		private IList<Child> _siblings;

		private Child[] _friends;

		// default it to today - ms sql has problems with null datetimes
		private DateTime _favoriteDate = DateTime.Today;

		public Child()
		{
			_friends = new Child[3];
		}

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public string FullName
		{
			get { return _fullName; }
			set { _fullName = value; }
		}

		public IList<Child> Siblings
		{
			get
			{
				if (_siblings == null) _siblings = new List<Child>();
				return _siblings;
			}
			set { _siblings = value; }
		}

		public Child FirstSibling
		{
			get { return (Child) Siblings[0]; }
			set { Siblings.Insert(0, value); }
		}


		public Child SecondSibling
		{
			get { return (Child) Siblings[1]; }
			set { Siblings.Insert(1, value); }
		}

		public IDictionary<string, Parent> Parents
		{
			get
			{
				if (_parents == null) _parents = new Dictionary<string, Parent>();
				return _parents;
			}
			set { _parents = value; }
		}

		public Parent Mom
		{
			get { return (Parent) Parents["mom"]; }
			set
			{
				if (Parents.ContainsKey("mom") == false)
				{
					Parents.Add("mom", value);
				}
				else
				{
					Parents["mom"] = value;
				}
			}
		}

		public Parent Dad
		{
			get { return (Parent) Parents["dad"]; }
			set
			{
				if (Parents.ContainsKey("dad") == false)
				{
					Parents.Add("dad", value);
				}
				else
				{
					Parents["dad"] = value;
				}
			}
		}

		public SexType Sex
		{
			get { return _sex; }
			set { _sex = value; }
		}

		public Child[] Friends
		{
			get { return _friends; }
			set { _friends = value; }
		}


		public DateTime FavoriteDate
		{
			get { return _favoriteDate; }
			set { _favoriteDate = value; }
		}
	}
}