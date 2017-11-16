using ICD.Common.Permissions;

namespace ICD.Connect.Settings
{
	public class OriginatorAction : Action
	{
		protected OriginatorAction(string value) : base(value)
		{
		}

		public IAction CopySettings { get { return new OriginatorAction("IOriginator.CopySettings"); } }
		public IAction ApplySettings { get { return new OriginatorAction("IOriginator.ApplySettings"); } }
		public IAction ClearSettings { get { return new OriginatorAction("IOriginator.ClearSettings"); } }
	}
}
