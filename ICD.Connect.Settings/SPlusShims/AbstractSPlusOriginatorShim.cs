using System;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Settings.Core;
using ICD.Connect.Settings.Simpl;
using ICD.Connect.Settings.SPlusShims.GlobalEvents;

namespace ICD.Connect.Settings.SPlusShims
{
	public abstract class AbstractSPlusOriginatorShim<TOriginator> : AbstractSPlusShim,
		ISPlusOriginatorShim<TOriginator>
		where TOriginator : class, ISimplOriginator
	{
		#region Events

		/// <summary>
		/// Raised when wrapped originator settings are applied.
		/// </summary>
		[PublicAPI("S+")]
		public event EventHandler OnSettingsApplied;

		/// <summary>
		/// Raised when wrapped originator settings are cleared.
		/// </summary>
		[PublicAPI("S+")]
		public event EventHandler OnSettingsCleared;

		#endregion

		private TOriginator m_Originator;
		private int m_OriginatorId;

		#region Properties

		/// <summary>
		/// Gets the wrapped originator.
		/// </summary>
		public TOriginator Originator { get { return m_Originator; } }

		/// <summary>
		/// Gets the wrapped originator.
		/// </summary>
		ISimplOriginator ISPlusOriginatorShim.Originator { get { return Originator; } }

		#endregion

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();

			SetOriginator(default(TOriginator));
		}

		#region Methods

		/// <summary>
		/// Sets the wrapped originator.
		/// </summary>
		/// <param name="id"></param>
		[PublicAPI("SPlus")]
		public virtual void SetOriginator(int id)
		{
			TOriginator originator = GetOriginator(id);
			SetOriginator(originator);
		}

		private void SetOriginator(TOriginator originator)
		{
			// ReSharper disable once CompareNonConstrainedGenericWithNull
			m_OriginatorId = originator == null ? 0 : originator.Id;

			Unsubscribe(m_Originator);
			m_Originator = originator;
			Subscribe(m_Originator);
		}

		/// <summary>
		/// Subscribes to the originator events.
		/// </summary>
		/// <param name="originator"></param>
		protected virtual void Subscribe(TOriginator originator)
		{
			if(Originator == null)
				return;
			Originator.OnSettingsApplied += OriginatorOnSettingsApplied;
			Originator.OnSettingsCleared += OriginatorOnSettingsCleared;
		}

		protected virtual void Unsubscribe(TOriginator originator)
		{
			if (Originator == null)
				return;
			Originator.OnSettingsApplied -= OriginatorOnSettingsApplied;
			Originator.OnSettingsCleared -= OriginatorOnSettingsCleared;
		}

		private void OriginatorOnSettingsApplied(object sender, EventArgs eventArgs)
		{
			OnSettingsApplied.Raise(this);
		}

		private void OriginatorOnSettingsCleared(object sender, EventArgs eventArgs)
		{
			OnSettingsCleared.Raise(this);
		}

		protected override void EnvironmentLoaded(EnvironmentLoadedEventInfo environmentLoadedEventInfo)
		{
			base.EnvironmentLoaded(environmentLoadedEventInfo);

			SetOriginator(m_OriginatorId);
		}

		protected override void EnvironmentUnloaded(EnvironmentUnloadedEventInfo environmentUnloadedEventInfo)
		{
			base.EnvironmentUnloaded(environmentUnloadedEventInfo);

			SetOriginator(default(TOriginator));
		}

		#endregion

		/// <summary>
		/// Gets the originator for the given id.
		/// </summary>
		/// <returns></returns>
		[CanBeNull]
		protected TOriginator GetOriginator(int id)
		{
			IOriginator output;
			bool childExists = ServiceProvider.GetService<ICore>().Originators.TryGetChild(id, out output);

			if (!childExists)
				Log(eSeverity.Error, "No Originator with id {0}", id);

			if (output is TOriginator)
				return (TOriginator)output;

			Log(eSeverity.Error, "Originator at id {0} is not of type {1}.",
			    id, typeof(TOriginator).FullName);

			return default(TOriginator);
		}
	}
}
