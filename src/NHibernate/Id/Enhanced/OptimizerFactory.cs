using System;
using System.Reflection;

using NHibernate.Util;

namespace NHibernate.Id.Enhanced
{
	public class OptimizerFactory
	{
		public const string None = "none";
		public const string HiLo = "hilo";
		public const string LegacyHilo = "legacy-hilo";
		public const string Pool = "pooled";
		public const string PoolLo = "pooled-lo";

		private static readonly System.Type[] CtorSignature = new[] { typeof(System.Type), typeof(int) };
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(OptimizerFactory));


		/// <summary>
		/// Marker interface for optimizer which wish to know the user-specified initial value.
		/// <p/>
		/// Used instead of constructor injection since that is already a public understanding and
		/// because not all optimizers care.
		/// </summary>
		public interface IInitialValueAwareOptimizer
		{
			/// <summary>
			/// Reports the user-specified initial value to the optimizer.
			/// <p/>
			/// <tt>-1</tt> is used to indicate that the user did not specify.
			/// <param name="initialValue">The initial value specified by the user, or <tt>-1</tt> to indicate that the
			/// user did not specify.</param>
			/// </summary>
			void InjectInitialValue(long initialValue);
		}


		[Obsolete("Use BuildOptimizer(string type, System.Type returnClass, int incrementSize, long explicitInitialValue) instead.")]
		public static IOptimizer BuildOptimizer(string type, System.Type returnClass, int incrementSize)
		{
			if (string.IsNullOrEmpty(type))
			{
				throw new ArgumentNullException("type");
			}
			if (returnClass == null)
			{
				throw new ArgumentNullException("returnClass");
			}
			string optimizerClassName;
			switch (type)
			{
				case None:
					optimizerClassName = typeof(NoopOptimizer).FullName;
					break;
				case HiLo:
					optimizerClassName = typeof(HiLoOptimizer).FullName;
					break;
				//case LegacyHilo:  FIXME
				//    FIXME optimizerClassName = typeof(HiLoOptimizer).FullName;
				//    break;
				case Pool:
					optimizerClassName = typeof(PooledOptimizer).FullName;
					break;
				//case PoolLo:    FIXME
				//    optimizerClassName = typeof(PooledOptimizer).FullName;
				//    break;
				default:
					optimizerClassName = type;
					break;
			}

			try
			{
				System.Type optimizerClass = ReflectHelper.ClassForName(optimizerClassName);
				ConstructorInfo ctor = optimizerClass.GetConstructor(CtorSignature);
				return (IOptimizer)ctor.Invoke(new object[] { returnClass, incrementSize });
			}
			catch (Exception)
			{
				log.Error("Unable to instantiate id generator optimizer.");  // FIXME: Review log message.
			}

			// the default...
			return new NoopOptimizer(returnClass, incrementSize);
		}


		public static IOptimizer BuildOptimizer(string type, System.Type returnClass, int incrementSize, long explicitInitialValue)
		{
			// FIXME: Disable this warning, or refactor without the deprecated version.
			IOptimizer optimizer = BuildOptimizer(type, returnClass, incrementSize);

			if (optimizer is IInitialValueAwareOptimizer)
				((IInitialValueAwareOptimizer)optimizer).InjectInitialValue(explicitInitialValue);

			return optimizer;
		}


		#region Nested type: HiLoOptimizer

		public class HiLoOptimizer : OptimizerSupport
		{
			private long upperLimit;
			private long lastSourceValue = -1;
			private long value_Renamed;

			public HiLoOptimizer(System.Type returnClass, int incrementSize)
				: base(returnClass, incrementSize)
			{
				if (incrementSize < 1)
				{
					throw new HibernateException("increment size cannot be less than 1");
				}
				if (log.IsDebugEnabled)
				{
					log.Debug("Creating hilo optimizer with [incrementSize=" + incrementSize + "; returnClass=" + returnClass.FullName
							  + "]");
				}
			}

			/// <summary>
			/// Exposure intended for testing purposes.
			/// </summary>
			public override long LastSourceValue
			{
				get { return lastSourceValue; }
			}

			public long LastValue
			{
				get { return value_Renamed - 1; }
			}

			/// <summary>
			/// Exposure intended for testing purposes.
			/// </summary>
			public long HiValue
			{
				get { return upperLimit; }
			}

			public override bool ApplyIncrementSizeToSourceValues
			{
				get { return false; }
			}

			public override object Generate(IAccessCallback callback)
			{
				if (lastSourceValue < 0)
				{
					lastSourceValue = callback.NextValue;
					while (lastSourceValue <= 0)
					{
						lastSourceValue = callback.NextValue;
					}
					// upperLimit defines the upper end of the bucket values
					upperLimit = (lastSourceValue * IncrementSize) + 1;
					// initialize value to the low end of the bucket
					value_Renamed = upperLimit - IncrementSize;
				}
				else if (upperLimit <= value_Renamed)
				{
					lastSourceValue = callback.NextValue;
					upperLimit = (lastSourceValue * IncrementSize) + 1;
				}
				return Make(value_Renamed++);
			}
		}

		#endregion

		#region Nested type: NoopOptimizer

		public class NoopOptimizer : OptimizerSupport
		{
			private long lastSourceValue = -1;

			public NoopOptimizer(System.Type returnClass, int incrementSize) : base(returnClass, incrementSize) { }

			public override long LastSourceValue
			{
				get { return lastSourceValue; }
			}

			public override bool ApplyIncrementSizeToSourceValues
			{
				get { return false; }
			}

			public override object Generate(IAccessCallback callback)
			{
				// The following note is from Hibernate. I haven't researched it fully. See HHH-3001.
				// Note: it is incredibly important that the method-local variable be
				// used here to avoid concurrency issues.

				long val = lastSourceValue;
				while (val <= 0)
					val = callback.NextValue;
				lastSourceValue = val;
				return Make(val);
			}
		}

		#endregion

		#region Nested type: OptimizerSupport

		/// <summary>
		/// Common support for optimizer implementations.
		/// </summary>
		public abstract class OptimizerSupport : IOptimizer
		{
			/// <summary>
			/// Construct an optimizer
			/// </summary>
			/// <param name="returnClass">The expected id class.</param>
			/// <param name="incrementSize">The increment size.</param>
			protected OptimizerSupport(System.Type returnClass, int incrementSize)
			{
				if (returnClass == null)
				{
					throw new HibernateException("return class is required");
				}
				ReturnClass = returnClass;
				IncrementSize = incrementSize;
			}

			public System.Type ReturnClass { get; protected set; }

			#region IOptimizer Members

			public int IncrementSize { get; protected set; }

			public abstract long LastSourceValue { get; }

			public abstract bool ApplyIncrementSizeToSourceValues { get; }
			public abstract object Generate(IAccessCallback param);

			#endregion

			protected virtual object Make(long value)
			{
				return IdentifierGeneratorFactory.CreateNumber(value, ReturnClass);
			}
		}

		#endregion

		#region Nested type: PooledOptimizer

		/// <summary>
		/// Optimizer which uses a pool of values, storing the next low value of the range in the database.
		/// <para>
		/// Note that this optimizer works essentially the same as the HiLoOptimizer, except that here the
		/// bucket ranges are actually encoded into the database structures.
		/// </para>
		/// <para>
		/// Note that if you prefer that the database value be interpreted as the bottom end of our current
		/// range, the use the PooledLoOptimizer strategy.
		/// </para>
		/// </summary>
		public class PooledOptimizer : OptimizerSupport, IInitialValueAwareOptimizer
		{
			private long hiValue = -1;
			private long value_Renamed;
			private long initialValue;

			public PooledOptimizer(System.Type returnClass, int incrementSize)
				: base(returnClass, incrementSize)
			{
				if (incrementSize < 1)
				{
					throw new HibernateException("increment size cannot be less than 1");
				}
				if (log.IsDebugEnabled)
				{
					log.Debug("Creating pooled optimizer with [incrementSize=" + incrementSize + "; returnClass="
							  + returnClass.FullName + "]");
				}
			}

			public override long LastSourceValue
			{
				get { return hiValue; }
			}


			/// <summary>
			/// Exposure intended for testing purposes.
			/// </summary>
			public long LastValue
			{
				get { return value_Renamed - 1; }
			}

			public override bool ApplyIncrementSizeToSourceValues
			{
				get { return true; }
			}


			public void InjectInitialValue(long initialValue)
			{
				this.initialValue = initialValue;
			}


			public override object Generate(IAccessCallback callback)
			{
				if (hiValue < 0)
				{
					value_Renamed = callback.NextValue;
					if (value_Renamed < 1)
					{
						// unfortunately not really safe to normalize this
						// to 1 as an initial value like we do the others
						// because we would not be able to control this if
						// we are using a sequence...
						log.Info("pooled optimizer source reported [" + value_Renamed
								 + "] as the initial value; use of 1 or greater highly recommended");
					}

					if ((initialValue == -1 && value_Renamed < IncrementSize) || value_Renamed == initialValue)
						hiValue = callback.NextValue;
					else
					{
						hiValue = value_Renamed;
						value_Renamed = hiValue - IncrementSize;
					}
				}
				else if (value_Renamed >= hiValue)
				{
					hiValue = callback.NextValue;
					value_Renamed = hiValue - IncrementSize;
				}
				return Make(value_Renamed++);
			}
		}

		#endregion
	}
}