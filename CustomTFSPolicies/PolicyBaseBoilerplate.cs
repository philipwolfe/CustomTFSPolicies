using System;
using System.Windows.Forms;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace CustomTFSPolicies
{
	[Serializable]
	public abstract class PolicyBaseBoilerplate : PolicyBase
	{
		protected string type;
		protected string typeDescription;
		protected string description;
		protected string installationInstructions;
		protected string dialogTitle;
		protected string helpText;

		public override string Type
		{
			get { return type; }
		}

		public override string TypeDescription
		{
			get { return typeDescription; }
		}

		public override string Description
		{
			get { return description; }
		}

		public override string InstallationInstructions
		{
			get
			{
				return installationInstructions;
			}
			set
			{
				installationInstructions = value;
			}
		}

		public override void DisplayHelp(PolicyFailure failure)
		{
			MessageBox.Show(helpText, dialogTitle);
		}

		public override bool Edit(IPolicyEditArgs args)
		{
			// no configuration to save
			return true;
		}
	}
}