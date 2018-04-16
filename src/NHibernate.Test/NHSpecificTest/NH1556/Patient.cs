namespace NHibernate.Test.NHSpecificTest.NH1556
{
   public class Patient
	{
      // Assigned by reflection
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
      private long id;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value
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
