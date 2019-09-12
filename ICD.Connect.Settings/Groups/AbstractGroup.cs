using ICD.Common.Utils;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Settings.Originators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ICD.Connect.Settings.Groups
{
	public abstract class AbstractGroup<TOriginator, TSettings> : AbstractOriginator<TSettings>, IGroup<TOriginator>
		where TSettings : IGroupSettings, new()
		where TOriginator : class, IOriginator
	{
		private readonly List<TOriginator> m_Items;
		private readonly SafeCriticalSection m_ItemsSection;

		#region Properties

		/// <summary>
		/// Gets the number of items in the group.
		/// </summary>
		public int Count { get { return m_ItemsSection.Execute(() => m_Items.Count); } }

		/// <summary>
		/// Gets the items in the group.
		/// </summary>
		public IEnumerable<TOriginator> Items { get { return m_ItemsSection.Execute(() => m_Items.ToList()); } }

		/// <summary>
		/// Gets the items in the group.
		/// </summary>
		IEnumerable<IOriginator> IGroup.Items { get { return Items.Cast<IOriginator>(); } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		protected AbstractGroup()
		{
			m_Items = new List<TOriginator>();
			m_ItemsSection = new SafeCriticalSection();
		}

		#region Settings

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			m_ItemsSection.Execute(() => m_Items.Clear());
		}

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(TSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.Ids = m_ItemsSection.Execute(() => m_Items.Select(i => i.Id).ToList());
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(TSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			m_ItemsSection.Enter();
			try
			{
				m_Items.Clear();
				IEnumerable<TOriginator> items = GetOriginatorsSkipExceptions(settings.Ids, factory);
				m_Items.AddRange(items);
			}
			finally
			{
				m_ItemsSection.Leave();
			}
		}

		private IEnumerable<TOriginator> GetOriginatorsSkipExceptions(IEnumerable<int> ids,
		                                                       IDeviceFactory factory)
		{
			foreach (int id in ids)
			{
				TOriginator output;

				try
				{
					output = factory.GetOriginatorById<TOriginator>(id);
				}
				catch (Exception e)
				{
					Log(eSeverity.Error, "Failed to instantiate {0} with id {1} - {2}", typeof(TOriginator).Name, id, e.Message);
					continue;
				}

				yield return output;
			}
		}

		#endregion
	}
}
