using System;
using System.Collections.Generic;


namespace NHibernate.Test.NHSpecificTest.NH1217
{
    public interface IRoot
    {
        String Name { get; set; }

        IList<INode> Nodes { get; set; }

        INode AddNode(String label);

        IEdge AddLink(INode from, INode to, String label);
    }
}