using System;
using NHibernate.Mapping.ByCode;

namespace NHibernate.Test.NHSpecificTest.GH2330
{
	public abstract class Node
	{
		private int _id;
		public virtual int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public virtual bool Deleted { get; set; }
		public virtual string FamilyName { get; set; }

		public static void AddMapping(ModelMapper mapper)
		{
			mapper.Class<Node>(ca =>
			{
				ca.Id(x => x.Id, map => map.Generator(Generators.Identity));
				ca.Property(x => x.Deleted);
				ca.Property(x => x.FamilyName);
				ca.Table("Node");
				ca.Abstract(true);
			});

			mapper.JoinedSubclass<PersonBase>(ca =>
			{
				ca.Key(x => x.Column("FK_Node_ID"));
				ca.Extends(typeof(Node));
				ca.Property(x => x.Deleted);
				ca.Property(x => x.Login);
			});
		}
	}

	[Serializable]
	public class PersonBase : Node
	{
		public virtual string Login { get; set; }
		public override bool Deleted { get; set; }
	}

	[Serializable]
	public class UserEntityVisit
	{
		private int _id;
		public virtual int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public virtual bool Deleted { get; set; }

		private PersonBase _PersonBase;
		public virtual PersonBase PersonBase
		{
			get { return _PersonBase; }
			set { _PersonBase = value; }
		}

		public static void AddMapping(ModelMapper mapper)
		{
			mapper.Class<UserEntityVisit>(ca =>
			{
				ca.Id(x => x.Id, map => map.Generator(Generators.Identity));
				ca.Property(x => x.Deleted);
				ca.ManyToOne(x => x.PersonBase);
			});
		}
	}
}
