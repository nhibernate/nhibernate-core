using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH2459
{
    public class TrainingComponent
    {
        protected TrainingComponent() {}
        public virtual Guid Id { get; set; }
        public virtual string Code { get; set; }
        public virtual string Title { get; set; }
    }

    public class SkillSet : TrainingComponent {
        
    }

    public class Qualification : TrainingComponent {
        
    }
}
