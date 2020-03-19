/*
 * Original Code: Microsoft Visual Studio SDK
 * */

using System;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace CustomTFSPolicies
{
	[Serializable]
	public class CheckForCommentsPolicy : PolicyBaseBoilerplate
	{
		string errMessage = "Custom Policy Violation (Press F1) - Check In comments are required.";
		
		public CheckForCommentsPolicy()
		{
			type = "Custom Policy - Check for Comments";
			typeDescription = "This policy will prompt the user to enter comments before they can check in.";
			description = "Require comments before checking in";
			installationInstructions = "To install this policy, register this assembly in the Checkin Policies folder under TeamFoundation in the registry.";
			dialogTitle = "Check for Comments Policy";
			helpText = "Comments are required to check in changes to track why the changes were made.  To add comments, click on the Source Files button and type in your comments.";
		}

		public override PolicyFailure[] Evaluate()
		{
			string proposedComment = PendingCheckin.PendingChanges.Comment;

			if (String.IsNullOrEmpty(proposedComment))
			{
				return new PolicyFailure[]
					{
						new PolicyFailure(errMessage, this),
					};
			}
			else
			{
				return new PolicyFailure[0];
			}
		}
	}
}