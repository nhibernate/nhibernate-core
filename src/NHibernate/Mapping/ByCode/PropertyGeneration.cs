using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Mapping.ByCode
{
	public abstract class PropertyGeneration
	{
		public static PropertyGeneration Never = new NeverPropertyGeneration();
		public static PropertyGeneration Insert = new InsertPropertyGeneration();
		public static PropertyGeneration Always = new AlwaysPropertyGeneration();

		internal abstract HbmPropertyGeneration ToHbm();

		public class AlwaysPropertyGeneration : PropertyGeneration
		{
			internal override HbmPropertyGeneration ToHbm()
			{
				return HbmPropertyGeneration.Always;
			}
		}

		public class InsertPropertyGeneration : PropertyGeneration
		{
			internal override HbmPropertyGeneration ToHbm()
			{
				return HbmPropertyGeneration.Insert;
			}
		}

		public class NeverPropertyGeneration : PropertyGeneration
		{
			internal override HbmPropertyGeneration ToHbm()
			{
				return HbmPropertyGeneration.Never;
			}
		}
	}
}