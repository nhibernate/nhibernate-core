
namespace NHibernate.Test.NHSpecificTest.NH2693 {
   using System;
   using System.Collections.Generic;

   public class FirstLevel {
      public FirstLevel() {
         SecondLevels = new HashSet<SecondLevelComponent>();
      }

      public virtual Guid Id { get; set; }
      public virtual ICollection<SecondLevelComponent> SecondLevels { get; set; }
   }

   public class SecondLevelComponent {
      public virtual FirstLevel FirstLevel { get; set; }
      public virtual ThirdLevel ThirdLevel { get; set; }
      public virtual SpecificThirdLevel SpecificThirdLevel { get; set; }
      public virtual bool SomeBool { get; set; }
   }

   public abstract class ThirdLevel {
      public virtual Guid Id { get; set; }
   }

   public class SpecificThirdLevel : ThirdLevel {
      public SpecificThirdLevel() {
				FourthLevels = new HashSet<FourthLevel>();
      }

      public virtual ICollection<FourthLevel> FourthLevels { get; set; }
   }

   public class FourthLevel {
      public virtual Guid Id { get; set; }
      public virtual SpecificThirdLevel SpecificThirdLevel { get; set; }
      public virtual string SomeString { get; set; }
   }
}
