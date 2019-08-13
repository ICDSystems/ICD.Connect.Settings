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
		private readonly List<TOriginator> m_GroupItems;
		private readonly SafeCriticalSection m_GroupSection;

		public IEnumerable<TOriginator> GroupItems { get { return m_GroupSection.Execute(() => m_GroupItems.ToList()); } }

		protected AbstractGroup()
		{
			m_GroupItems = new List<TOriginator>();
			m_GroupSection = new SafeCriticalSection();
		}

		#region Settings

		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			m_GroupSection.Execute(() => m_GroupItems.Clear());
		}

		protected override void CopySettingsFinal(TSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.Ids = m_GroupSection.Execute(() => m_GroupItems.Select(i => i.Id).ToList());
		}

		protected override void ApplySettingsFinal(TSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			m_GroupSection.Enter();
			try
			{
				m_GroupItems.Clear();
				IEnumerable<TOriginator> items = GetOriginatorsSkipExceptions(settings.Ids, factory);
				m_GroupItems.AddRange(items);
			}
			finally
			{
				m_GroupSection.Leave();
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
