using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace NHibernate.Cfg.MappingSchema
{
	public partial class HbmOneToOne : AbstractDecoratable, IEntityPropertyMapping, IFormulasMapping, IRelationship
	{
		#region Implementation of IEntityPropertyMapping

		public string Name
		{
			get { return name; }
		}

		public string Access
		{
			get { return access; }
		}

		// 6.0 Todo : remove XmlIgnore after removing the setter. See #3607 fix.
		[XmlIgnore]
		public bool OptimisticLock
		{
			get => optimisticlock;
			// Since v5.4.10
			[Obsolete("Providing a setter for OptimisticLock was unintended and will be removed.")]
			set => optimisticlock = value;
		}

		#endregion	
		
		#region Overrides of AbstractDecoratable

		protected override HbmMeta[] Metadatas
		{
			get { return meta ?? Array.Empty<HbmMeta>(); }
		}

		#endregion

		#region Implementation of IFormulasMapping

		[XmlIgnore]
		public IEnumerable<HbmFormula> Formulas
		{
			get { return formula ?? AsFormulas(); }
		}

		private IEnumerable<HbmFormula> AsFormulas()
		{
			if (string.IsNullOrEmpty(formula1))
			{
				yield break;
			}
			else
			{
				yield return new HbmFormula { Text = new[] { formula1 } };
			}
		}

		#endregion

		#region Implementation of IRelationship

		public string EntityName
		{
			get { return entityname; }
		}

		public string Class
		{
			get { return @class; }
		}

		public HbmNotFoundMode NotFoundMode
		{
			get { return HbmNotFoundMode.Exception; }
		}

		#endregion

		public HbmLaziness? Lazy
		{
			get { return lazySpecified ? lazy : (HbmLaziness?)null; }
		}

		public bool IsLazyProperty
		{
			get { return Lazy == HbmLaziness.Proxy; }
		}
	}
}
