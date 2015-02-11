using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH3755
{
    public class ShapeContainer : IShapeContainer
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public IShape Shape { get; set; }
    }
}
