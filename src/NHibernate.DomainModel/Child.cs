using System;
using System.Collections;

namespace NHibernate.DomainModel
{
	//TODO: this conflicts with a H2.0.3 class for testing...
	/// <summary>
	/// Summary description for Child.
	/// </summary>
	public class Child
	{
		private int id;
		private string fullName;
		private IDictionary parents;
		private SexType sex;

		private IList siblings;

		private Child[] friends;

		private System.DateTime favoriteDate;

		public Child()
		{
			friends = new Child[3];
		}
		
		public int Id {
			get {return id;}
			set {id = value;}
		}

		public string FullName {
			get {return fullName;}
			set {fullName = value;}
		}

		public IList Siblings {
			get {
				if(siblings==null) siblings = new ArrayList();
				return siblings;
			}
			set {siblings = value;}
		}

		public Child FirstSibling {
			get{
				return (Child)Siblings[0];
			}
			set {Siblings.Insert(0, value);}
		}


		public Child SecondSibling {
			get{
				return (Child)Siblings[1];
			}
			set {Siblings.Insert(1, value);}
		}

		public IDictionary Parents {
			get {
				if(parents==null)  parents = new Hashtable();
				return parents;
			}
			set {parents = value;}
		}

		public Parent Mom {
			get { return (Parent)Parents["mom"];}
			set {
				if(Parents.Contains("mom")==false) {
					 Parents.Add("mom", value);
				 }
				 else {
					 Parents["mom"] = value;
				 }
			}
		}

		public Parent Dad {
			get { return (Parent)Parents["dad"];}
			set {
				if(Parents.Contains("dad")==false) {
					Parents.Add("dad", value);
				}
				else {
					Parents["dad"] = value;
				}
			
			}
		}

		public SexType Sex {
			get { return sex;}
			set { sex = value;}
		}

		public Child[] Friends {
			get { return friends;}
			set { friends = value;}
		}


		public System.DateTime FavoriteDate{
			get {return favoriteDate;}
			set {favoriteDate = value;}
		}

	}
}
