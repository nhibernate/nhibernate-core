using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH3392
{
    public class Dad
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual ISet<Kid> Kids { get; set; }
        public virtual ISet<FriendOfTheFamily> Friends { get; set; }
    }
    public class Mum
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual ISet<Kid> Kids { get; set; }
        public virtual ISet<FriendOfTheFamily> Friends { get; set; }
    }
    public class Kid
    {
        public virtual int MumId { get; set; }
        public virtual int DadId { get; set; }
        public virtual string Name { get; set; }

        public override int GetHashCode()
        {
            return (MumId + "|" + DadId).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var child = obj as Kid;
            return child != null && (MumId == child.MumId && DadId == child.DadId);
        }
    }

    public class FriendOfTheFamily
    {
        public virtual MumAndDadId Id { get; set; }
        public virtual string Name { get; set; }
    }

    public class MumAndDadId
    {
        public virtual int MumId { get; set; }
        public virtual int DadId { get; set; }
        public override int GetHashCode()
        {
            return (MumId + "|" + DadId).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var child = obj as MumAndDadId;
            return child != null && (MumId == child.MumId && DadId == child.DadId);
        }
    }
}
