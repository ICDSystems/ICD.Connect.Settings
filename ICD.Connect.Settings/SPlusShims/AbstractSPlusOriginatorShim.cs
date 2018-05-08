using System;
using ICD.Common.Properties;
using ICD.Common.Utils.Services;
using ICD.Connect.Settings.Core;
using ICD.Connect.Settings.Simpl;

namespace ICD.Connect.Settings.SPlusShims
{
	public abstract class AbstractSPlusOriginatorShim<TOriginator> : IDisposable
		where TOriginator : ISimplOriginator
	{
		private TOriginator m_Originator;
		private ushort m_OriginatorId;

		/// <summary>
		/// Gets the wrapped originator.
		/// </summary>
		protected TOriginator Originator { get { return m_Originator; } }

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public void Dispose()
		{
			SetOriginator(0);
		}

		/// <summary>
		/// Sets the wrapped originator.
		/// </summary>
		/// <param name="id"></param>
		[PublicAPI("SPlus")]
		public void SetOriginator(ushort id)
		{
			m_OriginatorId = id;

			Unsubscribe(m_Originator);
			m_Originator = GetOriginator(id);
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

		#endregion

		/// <summary>
		/// Gets the originator for the given id.
		/// </summary>
		/// <returns></returns>
		[CanBeNull]
		private static TOriginator GetOriginator(ushort id)
		{
			IOriginator output;
			ServiceProvider.GetService<ICore>().Originators.TryGetChild(id, out output);
			return (TOriginator)output;
		}
	}
}
