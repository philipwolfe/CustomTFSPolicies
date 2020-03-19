/* Original Code: Philip Wolfe http://www.philipwolfe.com
 */
using System;
using System.Collections.Generic;
using EnvDTE;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace CustomTFSPolicies
{
	[Serializable]
	public class CheckForBuildPolicy : PolicyBaseBoilerplate
	{
		//base.Initialize will check the pendingcheckin for null and 
		//check if this object has been disposed.
		//It will also assign pendingCheckin to a protected PendingCheckin property

		string errMessage = "Custom Policy Violation (Press F1) - A clean build is required for project: {0}";
		
		#region Constructor
		public CheckForBuildPolicy()
		{
			type = "Custom Policy - Check for Clean Build";
			typeDescription = "This policy will prompt the user to build before they check in.";
			description = "Require build before checking in";
			installationInstructions = "To install this policy, register this assembly in the Checkin Policies folder under TeamFoundation in the registry.";
			dialogTitle = "Clean Build Policy";
			helpText = "You must perform a clean build before you check in your code.  This ensures that your changes will not cause build errors in the project.  To build, click on the Build menu and select Rebuild Solution.";
		}
		#endregion

		#region Overrides
		
		public override PolicyFailure[] Evaluate()
		{
			try
			{
				List<PolicyFailure> failureList = new List<PolicyFailure>();
				_DTE m_DTE = PendingCheckin.GetService(typeof(_DTE)) as _DTE;

				if (m_DTE != null)
				{
					if (m_DTE.Solution != null)
					{
						if (m_DTE.Solution.Projects != null)
						{
							foreach (Project project in m_DTE.Solution.Projects)
							{
								ProjectUptoDate(project, failureList);
							}
						}
					}
				}

				return failureList.ToArray();
				
			}
			catch
			{
				return new PolicyFailure[0];
			}
		}
	
		#endregion

		#region Privates
		
		//Code from StaticAnalysisPolicy class in the StanPolicy.dll file
		private bool ProjectUptoDate(Project project, List<PolicyFailure> failuresList)
		{
			IVsBuildableProjectCfg cfg1;
			IVsHierarchy hierarchy1 = GetVsProjectFromDTE(project);
			IVsProjectCfg2[] cfgArray1 = new IVsProjectCfg2[1];
			IVsSolutionBuildManager m_currBuildManager = PendingCheckin.GetService(typeof(IVsSolutionBuildManager)) as IVsSolutionBuildManager;

			m_currBuildManager.FindActiveProjectCfg(IntPtr.Zero, IntPtr.Zero, hierarchy1, cfgArray1);
			if (cfgArray1[0] == null)
			{
				return false;
			}
			cfgArray1[0].get_BuildableProjectCfg(out cfg1);
			if (cfg1 == null)
			{
				return false;
			}
			int[] numArray1 = new int[1];
			int[] numArray2 = new int[1];
			int num1 = cfg1.QueryStartUpToDateCheck(1, numArray1, numArray2);
			if ((numArray1[0] != 0) && !ErrorHandler.Failed(num1))
			{
				//http://msdn2.microsoft.com/en-us/library/microsoft.visualstudio.shell.interop.ivsbuildableprojectcfg.startuptodatecheck(VS.80).aspx
				num1 = cfg1.StartUpToDateCheck(null, 1);
				if (ErrorHandler.Failed(num1))
				{
					string text1 = String.Format(errMessage, project.Name);
					failuresList.Add(new PolicyFailure(text1, this));
					return false;
				}
			}
			return true;
		}

		private IVsHierarchy GetVsProjectFromDTE(Project project)
		{
			IVsHierarchy hierarchy1;
			string projectName = project.UniqueName;
			
			IVsSolution solution = PendingCheckin.GetService(typeof(IVsSolution)) as IVsSolution;
			solution.GetProjectOfUniqueName(projectName, out hierarchy1);
			
			return hierarchy1;
		}
		#endregion
	}
}