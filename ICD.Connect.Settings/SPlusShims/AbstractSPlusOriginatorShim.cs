﻿using System;
using ICD.Common.Properties;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Settings.Core;
using ICD.Connect.Settings.Simpl;

namespace ICD.Connect.Settings.SPlusShims
{
	public abstract class AbstractSPlusOriginatorShim<TOriginator> : IDisposable,
		ISPlusOriginatorShim<TOriginator> 
		where TOriginator : ISimplOriginator
	{
		private TOriginator m_Originator;
		private int m_OriginatorId;

		protected static ILoggerService Logger{get { return ServiceProvider.GetService<ILoggerService>(); }}

		/// <summary>
		/// Gets the wrapped originator.
		/// </summary>
		public TOriginator Originator { get { return m_Originator; } }

		/// <summary>
		/// Gets the wrapped originator.
		/// </summary>
		ISimplOriginator ISPlusOriginatorShim.Originator { get { return Originator; } }

		/// <summary>
		/// The Simpl Windows Location, set by S+
		/// </summary>
		[PublicAPI("S+")]
		public string Location { get; set; }

		protected AbstractSPlusOriginatorShim()
		{
			SPlusShimCore.ShimManager.RegisterShim((ISPlusOriginatorShim<ISimplOriginator>)this);
			ServiceProvider.GetService<ICore>().OnSettingsApplied += CoreLoaded;
		}

		

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public void Dispose()
		{
			SetOriginator(default(TOriginator));
			ServiceProvider.GetService<ICore>().OnSettingsApplied -= CoreLoaded;
		}

		/// <summary>
		/// Sets the wrapped originator.
		/// </summary>
		/// <param name="id"></param>
		[PublicAPI("SPlus")]
		public void SetOriginator(int id)
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
		}


		protected virtual void Unsubscribe(TOriginator originator)
		{
		}

		private void CoreLoaded(object sender, EventArgs eventArgs)
		{
			SetOriginator(m_Originator);
		}

		#endregion

		/// <summary>
		/// Gets the originator for the given id.
		/// </summary>
		/// <returns></returns>
		[CanBeNull]
		private static TOriginator GetOriginator(int id)
		{
			IOriginator output;
			bool childExists = ServiceProvider.GetService<ICore>().Originators.TryGetChild(id, out output);

			if (!childExists)
			{
				Logger.AddEntry(eSeverity.Error,
								"No Originator with id {0}",
								id);
			}

			if (!(output is TOriginator))
			{
				Logger.AddEntry(eSeverity.Error,
				                "Originator at id {0} is not of type {1}.",
				                id,
				                typeof(TOriginator).FullName);

				return default(TOriginator);
			}

			return (TOriginator)output;
		}
	}
}
