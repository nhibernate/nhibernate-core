using System;


namespace NHibernate.Test.NHSpecificTest.NH1217
{
    public interface IEdge
    {
        String Label { get; set; }
		    
        INode FromNode { get; set; }
		    
        INode ToNode { get; set; }
    }
}