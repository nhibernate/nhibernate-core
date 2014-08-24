namespace NHibernate.Test.NHSpecificTest.NH1556
{
   public class Patient
   {
      private long id;
      private string firstName;
      private string lastName;

      protected Patient()
      {
      }

      public Patient(string firstName, string lastName)
      {
         this.firstName = firstName;
         this.lastName = lastName;
      }

      public virtual long Id
      {
         get { return id; }
      }

      public virtual string FirstName
      {
         get { return firstName; }
      }

      public virtual string LastName
      {
         get { return lastName; }
      }
   }
}
