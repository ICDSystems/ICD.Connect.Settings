﻿using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Telemetry.Attributes;
using ICD.Connect.Telemetry.Providers.External;

namespace ICD.Connect.Settings.Cores
{
	public sealed class CoreExternalTelemetryProvider : AbstractExternalTelemetryProvider<ICore>
	{
		[PublicAPI("DAV-PRO")]
		[EventTelemetry("OnOriginatorIdsChanged")]
		public event EventHandler OnOriginatorIdsChanged;

		private readonly IcdHashSet<Guid> m_OriginatorIds;
		private readonly SafeCriticalSection m_OriginatorIdsSection;

		#region Properties

		[PublicAPI("DAV-PRO")]
		[PropertyTelemetry("SoftwareVersion", null, null)]
		public string SoftwareVersion { get { return Parent.GetType().GetAssembly().GetName().Version.ToString(); } }

		[PublicAPI("DAV-PRO")]
		[PropertyTelemetry("SoftwareInformationalVersion", null, null)]
		public string SoftwareInformationalVersion
		{
			get
			{
				string version;
				return Parent.GetType().GetAssembly().TryGetInformationalVersion(out version) ? version : null;
			}
		}

		[PublicAPI("DAV-PRO")]
		[PropertyTelemetry("OriginatorIds", null, "OnOriginatorIdsChanged")]
		public IEnumerable<Guid> OriginatorIds { get { return m_OriginatorIdsSection.Execute(() => m_OriginatorIds.ToArray()); } }
		
		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public CoreExternalTelemetryProvider()
		{
			m_OriginatorIds = new IcdHashSet<Guid>();
			m_OriginatorIdsSection = new SafeCriticalSection();
		}

		#region Methods

		/// <summary>
		/// Sets the parent telemetry provider that this instance extends.
		/// </summary>
		/// <param name="parent"></param>
		protected override void SetParent(ICore parent)
		{
			base.SetParent(parent);

			UpdateOriginatorIds();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Updates the OriginatorIds collection and raises the OnOriginatorIdsChanged event
		/// if the collection has changed.
		/// </summary>
		private void UpdateOriginatorIds()
		{
			IcdHashSet<Guid> originatorIds =
				Parent.Originators
					  .Select(d => d.Uuid)
					  .ToIcdHashSet();

			m_OriginatorIdsSection.Enter();

			try
			{
				if (originatorIds.SetEquals(m_OriginatorIds))
					return;

				m_OriginatorIds.Clear();
				m_OriginatorIds.AddRange(originatorIds);
			}
			finally
			{
				m_OriginatorIdsSection.Leave();
			}

			OnOriginatorIdsChanged.Raise(this);
		}

		#endregion

		#region Provider Callbacks

		/// <summary>
		/// Subscribe to the parent events.
		/// </summary>
		/// <param name="parent"></param>
		protected override void Subscribe(ICore parent)
		{
			base.Subscribe(parent);

			if (parent == null)
				return;

			parent.Originators.OnCollectionChanged += OriginatorsOnCollectionChanged;
		}

		/// <summary>
		/// Unsubscribe from the parent events.
		/// </summary>
		/// <param name="parent"></param>
		protected override void Unsubscribe(ICore parent)
		{
			base.Unsubscribe(parent);

			if (parent == null)
				return;

			parent.Originators.OnCollectionChanged -= OriginatorsOnCollectionChanged;
		}

		/// <summary>
		/// Called when originators are added/removed to/from the core.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void OriginatorsOnCollectionChanged(object sender, EventArgs eventArgs)
		{
			UpdateOriginatorIds();
		}

		#endregion
	}
}
