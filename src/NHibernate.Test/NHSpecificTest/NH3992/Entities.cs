using System;

namespace NHibernate.Test.NHSpecificTest.NH3992
{
	public class BaseEntity
	{
		public virtual Guid Id { get; set; }
		public virtual string BaseField { get; set; }
	}

	public class MappedEntity : UnmappedEntity
	{
		public virtual string TopLevelField { get; set; }
	}

	public class UnmappedEntity : BaseEntity
	{
		public virtual string ExtendedField { get; set; }
	}

	public interface IBaseInterface
	{
		Guid Id { get; set; }
		string BaseField { get; set; }
	}

	public interface IUnmappedInterface : IBaseInterface
	{
		string ExtendedField { get; set; }
	}

	public class MappedEntityFromInterface : IUnmappedInterface
	{
		public virtual Guid Id { get; set; }
		public virtual string BaseField { get; set; }
		public virtual string ExtendedField { get; set; }
		public virtual string TopLevelField { get; set; }
	}

	// Animal entities copied from NH2691
	public abstract class Animal
	{
		public virtual int Id { get; set; }
		public virtual string Description { get; set; }
		public virtual int Sequence { get; set; }
	}

	public abstract class Mammal : Animal
	{
		public virtual bool Pregnant { get; set; }
		public virtual DateTime? BirthDate { get; set; }
	}

	public class Dog : Mammal { }

	// Mapped -> Unmapped -> Mapped -> Root
	namespace Longchain1
	{
		public class MappedRoot
		{
			public virtual Guid Id { get; set; }
			public virtual string BaseField { get; set; }
		}

		public class MappedExtension : MappedRoot
		{
			public virtual string MappedExtensionField { get; set; }
		}

		public class UnmappedExtension : MappedExtension
		{
			public virtual string UnmappedExtensionField { get; set; }
		}

		public class TopLevel : UnmappedExtension
		{
			public virtual string TopLevelExtensionField { get; set; }
		}
	}

	// Mapped -> Mapped -> Unmapped -> Root
	namespace Longchain2
	{
		public class MappedRoot
		{
			public virtual Guid Id { get; set; }
			public virtual string BaseField { get; set; }
		}

		public class UnmappedExtension : MappedRoot
		{
			public virtual string UnmappedExtensionField { get; set; }
		}

		public class MappedExtension : UnmappedExtension
		{
			public virtual string MappedExtensionField { get; set; }
		}


		public class TopLevel : MappedExtension
		{
			public virtual string TopLevelExtensionField { get; set; }
		}
	}

	// Mapped -> Unmapped -> Mapped -> Unmapped -> Root
	namespace Longchain3
	{
		public class MappedRoot
		{
			public virtual Guid Id { get; set; }
			public virtual string BaseField { get; set; }
		}

		public class FirstUnmappedExtension : MappedRoot
		{
			public virtual string FirstUnmappedExtensionField { get; set; }
		}

		public class MappedExtension : FirstUnmappedExtension
		{
			public virtual string MappedExtensionField { get; set; }
		}

		public class SecondUnmappedExtension : MappedExtension
		{
			public virtual string SecondUnmappedExtensionField { get; set; }
		}

		public class TopLevel : SecondUnmappedExtension
		{
			public virtual string TopLevelExtensionField { get; set; }
		}
	}

}
