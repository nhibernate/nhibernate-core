using System;
using System.Collections.Concurrent;
using System.Reflection;
using NHibernate.Util;

namespace NHibernate.Id.Enhanced
{
	public partial class OptimizerFactory
	{
		private static readonly INHibernateLogger Log = NHibernateLogger.For(typeof(OptimizerFactory));

		public const string None = "none";
		public const string HiLo = "hilo";
		public const string Pool = "pooled";
		public const string PoolLo = "pooled-lo";

		private static readonly System.Type[] CtorSignature = new[] { typeof(System.Type), typeof(int) };

		/// <summary>
		/// Marker interface for an optimizer that wishes to know the user-specified initial value.
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

		private static IOptimizer BuildOptimizer(string type, System.Type returnClass, int incrementSize)
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
				case Pool:
					optimizerClassName = typeof(PooledOptimizer).FullName;
					break;
				case PoolLo:
					optimizerClassName = typeof(PooledLoOptimizer).FullName;
					break;
				default:
					optimizerClassName = type;
					break;
			}

			try
			{
				System.Type optimizerClass = ReflectHelper.ClassForName(optimizerClassName);
				ConstructorInfo ctor = optimizerClass.GetConstructor(CtorSignature);

				if (ctor == null)
					throw new HibernateException("Optimizer does not have expected constructor");

				return (IOptimizer) ctor.Invoke(new object[] { returnClass, incrementSize });
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Unable to instantiate id generator optimizer."); // FIXME: Review log message.
			}

			// the default...
			return new NoopOptimizer(returnClass, incrementSize);
		}

		public static IOptimizer BuildOptimizer(string type, System.Type returnClass, int incrementSize, long explicitInitialValue)
		{
			// FIXME: Disable this warning, or refactor without the deprecated version.
			IOptimizer optimizer = BuildOptimizer(type, returnClass, incrementSize);

			if (optimizer is IInitialValueAwareOptimizer)
				((IInitialValueAwareOptimizer) optimizer).InjectInitialValue(explicitInitialValue);

			return optimizer;
		}

		#region Nested type: HiLoOptimizer

		public partial class HiLoOptimizer : OptimizerSupport
		{
			private readonly AsyncLock _asyncLock = new AsyncLock();
			private readonly TenantStateStore<GenerationState> _stateStore = new TenantStateStore<GenerationState>();

			public HiLoOptimizer(System.Type returnClass, int incrementSize) : base(returnClass, incrementSize)
			{
				if (incrementSize < 1)
				{
					throw new HibernateException("increment size cannot be less than 1");
				}
				if (Log.IsDebugEnabled())
				{
					Log.Debug("Creating hilo optimizer with [incrementSize={0}; returnClass={1}]", incrementSize, returnClass.FullName);
				}
			}

			public class GenerationState
			{
				public long LastSourceValue { get; internal set; } = -1;
				public long Value { get; internal set; }
				public long UpperLimit { get; internal set; }
			}

			/// <summary>
			/// Exposure intended for testing purposes.
			/// </summary>
			public override long LastSourceValue
			{
				get { return _stateStore.NoTenantGenerationState.LastSourceValue; }
			}

			public long LastValue
			{
				get { return _stateStore.NoTenantGenerationState.Value - 1; }
			}

			/// <summary>
			/// Exposure intended for testing purposes.
			/// </summary>
			public long HiValue
			{
				get { return _stateStore.NoTenantGenerationState.UpperLimit; }
			}

			public long GetHiValue(string tenantIdentifier)
			{
				return _stateStore.LocateGenerationState(tenantIdentifier).UpperLimit;
			}

			public long GetLastSourceValue(string tenantIdentifier)
			{
				return _stateStore.LocateGenerationState(tenantIdentifier).LastSourceValue;
			}
			public long GetLastValue(string tenantIdentifier)
			{
				return _stateStore.LocateGenerationState(tenantIdentifier).Value - 1;
			}

			public override bool ApplyIncrementSizeToSourceValues
			{
				get { return false; }
			}

			public override object Generate(IAccessCallback callback)
			{
				using (_asyncLock.Lock())
				{
					var generationState = _stateStore.LocateGenerationState(callback.GetTenantIdentifier());

					if (generationState.LastSourceValue < 0)
					{
						generationState.LastSourceValue = callback.GetNextValue();
						while (generationState.LastSourceValue <= 0)
						{
							generationState.LastSourceValue = callback.GetNextValue();
						}

						// upperLimit defines the upper end of the bucket values
						generationState.UpperLimit = (generationState.LastSourceValue * IncrementSize) + 1;

						// initialize value to the low end of the bucket
						generationState.Value = generationState.UpperLimit - IncrementSize;
					}
					else if (generationState.UpperLimit <= generationState.Value)
					{
						generationState.LastSourceValue = callback.GetNextValue();
						generationState.UpperLimit = (generationState.LastSourceValue * IncrementSize) + 1;
					}

					return Make(generationState.Value++);
				}
			}
		}

		#endregion

		#region Nested type: NoopOptimizer

		public partial class NoopOptimizer : OptimizerSupport
		{
			private long _lastSourceValue = -1;

			public NoopOptimizer(System.Type returnClass, int incrementSize) : base(returnClass, incrementSize) { }

			public override long LastSourceValue
			{
				get { return _lastSourceValue; }
			}

			public override bool ApplyIncrementSizeToSourceValues
			{
				get { return false; }
			}

			public override object Generate(IAccessCallback callback)
			{
				// We must use a local variable here to avoid concurrency issues.
				// With the local value we can avoid synchronizing the whole method.

				long val = -1;
				while (val <= 0)
					val = callback.GetNextValue();

				// This value is only stored for easy access in test. Should be no
				// threading concerns there.
				_lastSourceValue = val;

				return Make(val);
			}
		}

		#endregion

		#region Nested type: OptimizerSupport

		/// <summary>
		/// Common support for optimizer implementations.
		/// </summary>
		public abstract partial class OptimizerSupport : IOptimizer
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
		/// range, then use the PooledLoOptimizer strategy.
		/// </para>
		/// </summary>
		public partial class PooledOptimizer : OptimizerSupport, IInitialValueAwareOptimizer
		{
			private long _initialValue;
			private readonly AsyncLock _asyncLock = new AsyncLock();
			private readonly TenantStateStore<GenerationState> _stateStore = new TenantStateStore<GenerationState>();
			public PooledOptimizer(System.Type returnClass, int incrementSize) : base(returnClass, incrementSize)
			{
				if (incrementSize < 1)
				{
					throw new HibernateException("increment size cannot be less than 1");
				}
				if (Log.IsDebugEnabled())
				{
					Log.Debug("Creating pooled optimizer with [incrementSize={0}; returnClass={1}]", incrementSize, returnClass.FullName);
				}
			}

			public class GenerationState
			{
				public long Value { get; internal set; }
				public long HiValue { get; internal set; } = -1;
			}

			public override long LastSourceValue
			{
				get { return _stateStore.NoTenantGenerationState.HiValue; }
			}

			/// <summary>
			/// Exposure intended for testing purposes.
			/// </summary>
			/// 
			public long GetLastSourceValue(string tenantIdentifier)
			{
				return _stateStore.LocateGenerationState(tenantIdentifier).HiValue;
			}
			public long GetLastValue(string tenantIdentifier)
			{
				 return _stateStore.LocateGenerationState(tenantIdentifier).Value - 1;
			}

			public override bool ApplyIncrementSizeToSourceValues
			{
				get { return true; }
			}

			public void InjectInitialValue(long initialValue)
			{
				_initialValue = initialValue;
			}

			public override object Generate(IAccessCallback callback)
			{
				using (_asyncLock.Lock())
				{
					var generationState = _stateStore.LocateGenerationState(callback.GetTenantIdentifier());

					if (generationState.HiValue < 0)
					{
						generationState.Value = callback.GetNextValue();
						if (generationState.Value < 1)
						{
							// unfortunately not really safe to normalize this
							// to 1 as an initial value like we do the others
							// because we would not be able to control this if
							// we are using a sequence...
							Log.Info("pooled optimizer source reported [{0}] as the initial value; use of 1 or greater highly recommended", generationState.Value);
						}

						if ((_initialValue == -1 && generationState.Value < IncrementSize) || generationState.Value == _initialValue)
							generationState.HiValue = callback.GetNextValue();
						else
						{
							generationState.HiValue = generationState.Value;
							generationState.Value = generationState.HiValue - IncrementSize;
						}
					}
					else if (generationState.Value >= generationState.HiValue)
					{
						generationState.HiValue = callback.GetNextValue();
						generationState.Value = generationState.HiValue - IncrementSize;
					}

					return Make(generationState.Value++);
				}
			}
		}
		#endregion

		#region Nested type: PooledLoOptimizer

		public partial class PooledLoOptimizer : OptimizerSupport
		{
			private readonly AsyncLock _asyncLock = new AsyncLock();
			private readonly TenantStateStore<GenerationState> _stateStore = new TenantStateStore<GenerationState>();

			public PooledLoOptimizer(System.Type returnClass, int incrementSize) : base(returnClass, incrementSize)
			{
				if (incrementSize < 1)
				{
					throw new HibernateException("increment size cannot be less than 1");
				}
				if (Log.IsDebugEnabled())
				{
					Log.Debug("Creating pooled optimizer (lo) with [incrementSize={0}; returnClass={1}]", incrementSize, returnClass.FullName);
				}
			}

			public class GenerationState
			{
				public long LastSourceValue { get; internal set; } = -1;
				public long Value { get; internal set; }
			}

			public override object Generate(IAccessCallback callback)
			{
				using (_asyncLock.Lock())
				{
					var generationState = _stateStore.LocateGenerationState(callback.GetTenantIdentifier());

					if (generationState.LastSourceValue < 0 || generationState.Value >= (generationState.LastSourceValue + IncrementSize))
					{
						generationState.LastSourceValue = callback.GetNextValue();
						generationState.Value = generationState.LastSourceValue;
						// handle cases where initial-value is less than one (hsqldb for instance).
						if (generationState.Value < 1)
						{
							generationState.Value = 1;
						}
					}

					return Make(generationState.Value++);
				}
			}


			public override long LastSourceValue
			{
				get { return _stateStore.NoTenantGenerationState.LastSourceValue; }
			}

			public override bool ApplyIncrementSizeToSourceValues
			{
				get { return true; }
			}

			/// <summary>
			/// Exposure intended for testing purposes.
			/// </summary>
			public long LastValue
			{
				get { return _stateStore.NoTenantGenerationState.Value - 1; }
			}

			public long GetLastSourceValue(string tenantIdentifier)
			{
				return _stateStore.LocateGenerationState(tenantIdentifier).LastSourceValue;
			}

			public long GetLastValue(string tenantIdentifier)
			{
				return _stateStore.LocateGenerationState(tenantIdentifier).Value - 1;
			}
		}

		#endregion
	}
}
