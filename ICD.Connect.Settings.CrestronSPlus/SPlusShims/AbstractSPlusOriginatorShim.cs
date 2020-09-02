using System;
using System.Collections.Generic;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Connect.Settings.Cores;
using ICD.Connect.Settings.CrestronSPlus.SPlusShims.GlobalEvents;
using ICD.Connect.Settings.Originators;
#if SIMPLSHARP
using ICDPlatformString = Crestron.SimplSharp.SimplSharpString;
#else
using ICDPlatformString = System.String;
#endif

namespace ICD.Connect.Settings.CrestronSPlus.SPlusShims
{
	public delegate void SPlusStringDelegate(ICDPlatformString data);

	public delegate void SPlusBoolDelegate(ushort data);

	public delegate void SPlusIntDelegate(int data);

	public abstract class AbstractSPlusOriginatorShim<TOriginator> : AbstractSPlusShim, ISPlusOriginatorShim<TOriginator>
		where TOriginator : class, IOriginator
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

		/// <summary>
		/// Raised when the wrapped originator changes.
		/// </summary>
		[PublicAPI("S+")]
		public event EventHandler OnOriginatorChanged;

		/// <summary>
		/// Raised when the shim starts/stops wrapping an originator.
		/// </summary>
		[PublicAPI("S+")]
		public event EventHandler OnHasOriginatorChanged;

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
		IOriginator ISPlusOriginatorShim.Originator { get { return Originator; } }

		/// <summary>
		/// Returns true if the shim is currently wrapping an originator.
		/// </summary>
		[PublicAPI("S+")]
		public ushort HasOriginator { get { return (m_Originator != null).ToUShort(); } }

		/// <summary>
		/// Gets the ID of the current wrapped originator, or 0 if no originator is wrapped
		/// </summary>
		[PublicAPI("S+")]
		public int OriginatorId { get { return m_Originator != null ? m_Originator.Id : 0; } }

		#endregion

		#region SPlus

		[PublicAPI("S+")]
		public SPlusStringDelegate OriginatorName { get; set; }

		[PublicAPI("S+")]
		public SPlusStringDelegate OriginatorCombineName { get; set; }

		[PublicAPI("S+")]
		public SPlusStringDelegate OriginatorUuid { get; set; }

		[PublicAPI("S+")]
		public SPlusStringDelegate OriginatorCategory { get; set; }

		[PublicAPI("S+")]
		public SPlusBoolDelegate OriginatorHide { get; set; }

		[PublicAPI("S+")]
		public SPlusBoolDelegate OriginatorDisable { get; set; }

		[PublicAPI("S+")]
		public SPlusIntDelegate OriginatorOrder { get; set; }

		[PublicAPI("S+")]
		public void SetDisabled(ushort state)
		{
			if (Originator != null)
				Originator.Disable = state.ToBool();
		}

		#endregion

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnSettingsApplied = null;
			OnSettingsCleared = null;
			OnOriginatorChanged = null;
			OnHasOriginatorChanged = null;

			base.Dispose();

			SetOriginator(default(TOriginator));
		}

		#region Methods

		/// <summary>
		/// Sets the wrapped originator.
		/// </summary>
		/// <param name="id"></param>
		[PublicAPI("S+")]
		public void SetOriginator(int id)
		{
			m_OriginatorId = id;

			TOriginator originator = GetOriginator(id);
			SetOriginator(originator);
		}

		
		/// <summary>
		/// Sets the wrapped originator.
		/// Uses ushort lowword/highword for S+
		/// </summary>
		/// <param name="idLowByte"></param>
		/// <param name="idHighByte"></param>
		[PublicAPI("S+")]
		public void SetOriginatorSPlus(ushort idLowByte, ushort idHighByte)
		{
			SetOriginator(SPlusUtils.ConvertToInt(idLowByte, idHighByte));
		}

		protected override void EnvironmentLoaded(EnvironmentLoadedEventInfo environmentLoadedEventInfo)
		{
			base.EnvironmentLoaded(environmentLoadedEventInfo);

			SetOriginator(m_OriginatorId);
		}

		protected override void EnvironmentUnloaded(EnvironmentUnloadedEventInfo environmentUnloadedEventInfo)
		{
			base.EnvironmentUnloaded(environmentUnloadedEventInfo);

			SetOriginator(null);
		}

		#endregion

		#region Private/Protected Methods

		/// <summary>
		/// Called when the originator is attached.
		/// Do any actions needed to syncronize
		/// </summary>
		protected virtual void InitializeOriginator()
		{
			if (Originator == null)
				return;

			var nameDelegate = OriginatorName;
			if (nameDelegate != null)
				nameDelegate(Originator.Name);

			var combineNameDelegate = OriginatorCombineName;
			if (combineNameDelegate != null)
				combineNameDelegate(Originator.CombineName);

			var uuidDelegate = OriginatorUuid;
			if (uuidDelegate != null)
				uuidDelegate(Originator.Uuid.ToString());

			var categoryDelegate = OriginatorCategory;
			if (categoryDelegate != null)
				categoryDelegate(Originator.Category);

			var hideDelegate = OriginatorHide;
			if (hideDelegate != null)
				hideDelegate(Originator.Hide.ToUShort());

			var disableDelegate = OriginatorDisable;
			if (disableDelegate != null)
				disableDelegate(Originator.Disable.ToUShort());

			var orderDelegate = OriginatorOrder;
			if (orderDelegate != null)
				orderDelegate(Originator.Order);
		}

		/// <summary>
		/// Called when the originator is detached
		/// Do any actions needed to desyncronize
		/// </summary>
		protected virtual void DeinitializeOriginator()
		{
			
		}

		/// <summary>
		/// Sets the wrapped originator.
		/// </summary>
		/// <param name="originator"></param>
		private void SetOriginator(TOriginator originator)
		{
			TOriginator old = m_Originator;
			if (originator == old)
				return;

			Unsubscribe(m_Originator);
			if (m_Originator != null)
				DeinitializeOriginator();

			m_Originator = originator;
			Subscribe(m_Originator);

			OnOriginatorChanged.Raise(this);

			if (m_Originator != null)
				InitializeOriginator();

			if (old == null || m_Originator == null)
				OnHasOriginatorChanged.Raise(this);
		}

		/// <summary>
		/// Gets the originator for the given id.
		/// </summary>
		/// <returns></returns>
		[CanBeNull]
		private TOriginator GetOriginator(int id)
		{
			ICore core = ServiceProvider.TryGetService<ICore>();

			if (core == null)
			{
				Logger.Log(eSeverity.Error, "No {0} service found", typeof(ICore));
				return null;
			}

			IOriginator output;
			bool childExists = core.Originators.TryGetChild(id, out output);

			if (!childExists)
			{
				Logger.Log(eSeverity.Error, "No Originator with id {0}", id);
				return default(TOriginator);
			}

			if (output is TOriginator)
				return (TOriginator)output;

			Logger.Log(eSeverity.Error, "Originator at id {0} is not of type {1}.",
			           id, typeof(TOriginator).Name);

			return default(TOriginator);
		}

		#endregion

		#region Originator Callbacks

		/// <summary>
		/// Subscribes to the originator events.
		/// </summary>
		/// <param name="originator"></param>
		protected virtual void Subscribe(TOriginator originator)
		{
			if (Originator == null)
				return;

			Originator.OnSettingsApplied += OriginatorOnSettingsApplied;
			Originator.OnSettingsCleared += OriginatorOnSettingsCleared;
			originator.OnNameChanged += OriginatorOnNameChanged;
			originator.OnDisableStateChanged += OriginatorOnDisableStateChanged;
		}

		/// <summary>
		/// Unsubscribes from the originator events.
		/// </summary>
		/// <param name="originator"></param>
		protected virtual void Unsubscribe(TOriginator originator)
		{
			if (Originator == null)
				return;

			Originator.OnSettingsApplied -= OriginatorOnSettingsApplied;
			Originator.OnSettingsCleared -= OriginatorOnSettingsCleared;
			originator.OnNameChanged -= OriginatorOnNameChanged;
			originator.OnDisableStateChanged -= OriginatorOnDisableStateChanged;
		}

		private void OriginatorOnSettingsApplied(object sender, EventArgs eventArgs)
		{
			OnSettingsApplied.Raise(this);
		}

		private void OriginatorOnSettingsCleared(object sender, EventArgs eventArgs)
		{
			OnSettingsCleared.Raise(this);
		}

		private void OriginatorOnDisableStateChanged(object sender, BoolEventArgs args)
		{
			var disableDelegate = OriginatorDisable;
			if (disableDelegate != null)
				disableDelegate(args.Data.ToUShort());
		}

		private void OriginatorOnNameChanged(object sender, EventArgs args)
		{
			var nameDelegate = OriginatorName;
			if (nameDelegate != null)
				nameDelegate(Originator.Name);

			var combineNameDelegate = OriginatorCombineName;
			if (combineNameDelegate != null)
				combineNameDelegate(Originator.CombineName);
		}

		#endregion

		#region Console

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (IConsoleCommand c in GetBaseConsoleCommands())
				yield return c;

			yield return new GenericConsoleCommand<int>("SetOriginatorId", "Sets the Originator ID for the Shim", i => SetOriginator(i));
		}

		private IEnumerable<IConsoleCommand> GetBaseConsoleCommands()
		{
			return base.GetConsoleCommands();
		}

		/// <summary>
		/// Calls the delegate for each console status item.
		/// </summary>
		/// <param name="addRow"></param>
		public override void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			base.BuildConsoleStatus(addRow);
			addRow("Originator Id", m_OriginatorId);
			addRow("Has Originator", Originator != null);
		}

		#endregion
	}
}
