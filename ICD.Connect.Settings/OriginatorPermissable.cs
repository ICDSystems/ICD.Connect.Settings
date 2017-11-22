using ICD.Common.Permissions;

namespace ICD.Connect.Settings
{
	public class OriginatorPermissable : Permissable
	{
		protected OriginatorPermissable(string name) : base(name)
		{
		}

		public IPermissable CopySettings { get { return new OriginatorPermissable("IOriginator.CopySettings"); } }
		public IPermissable ApplySettings { get { return new OriginatorPermissable("IOriginator.ApplySettings"); } }
		public IPermissable ClearSettings { get { return new OriginatorPermissable("IOriginator.ClearSettings"); } }
	}
}
