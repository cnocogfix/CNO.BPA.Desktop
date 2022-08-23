using CNO.BPA.DataHandler;
using Emc.InputAccel.UimScript;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace Custom.InputAccel.UimScript
{
	public class ScriptSTNDNONFORM : UimScriptDocument
	{
		private const string _validationRule = "One";

		private string rejectReasonCode;

		private bool extraction;

		public ScriptSTNDNONFORM()
		{
			this.extraction = false;
			this.rejectReasonCode = "";
		}

		public void DocumentLoad(IUimDataContext dataContext)
		{
			dataContext.TaskFinishOnErrorNotAllowed = true;
			if (Share.getOperatorID() == "*")
			{
				dataContext.FindFieldDataContext("Operator").SetValue(ModCommon.getOperatorID());
			}
			else
			{
				dataContext.FindFieldDataContext("Operator").SetValue(Share.getOperatorID());
			}
			dataContext.FindFieldDataContext("MACHINE_NAME").SetValue(ModCommon.getMachineName());
		}

		public void DocumentUnload(IUimDataContext dataContext)
		{
		}

		public void DocumentExtracted(IUimDataContext dataContext)
		{
			this.extraction = true;
			if (Share.getOperatorID() == "*")
			{
				dataContext.FindFieldDataContext("Operator").SetValue(ModCommon.getOperatorID());
			}
			else
			{
				dataContext.FindFieldDataContext("Operator").SetValue(Share.getOperatorID());
			}
		}

		public void SelectionChangeRejected(IUimFormControlContext controlContext)
		{
			try
			{
				if (controlContext.ChoiceValue == "1")
				{
					ValidationRejectForm validationRejectForm = new ValidationRejectForm();
					DataAccess dataAccess = new DataAccess();
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					DataTable dataTable = dataAccess.getRejectOptions("AUTOMATE_INDEX").Tables[0];
					foreach (DataRow dataRow in dataTable.Rows)
					{
						dictionary.Add(dataRow["REJECT_OPTION"].ToString().Trim(), dataRow["REASON_CODE"].ToString().Trim());
						validationRejectForm.addWrongDeptOption(dataRow["REJECT_OPTION"].ToString().Trim());
					}
					DataTable dataTable2 = dataAccess.getRejectOptions("WRONG_COMPANY").Tables[0];
					foreach (DataRow dataRow in dataTable2.Rows)
					{
						dictionary.Add(dataRow["REJECT_OPTION"].ToString().Trim(), dataRow["REASON_CODE"].ToString().Trim());
						validationRejectForm.addWrongCompanyOption(dataRow["REJECT_OPTION"].ToString().Trim());
					}
					DataTable dataTable3 = dataAccess.getRejectOptions("VALIDATION_ISSUES").Tables[0];
					foreach (DataRow dataRow in dataTable3.Rows)
					{
						dictionary.Add(dataRow["REJECT_OPTION"].ToString().Trim(), dataRow["REASON_CODE"].ToString().Trim());
						validationRejectForm.addIssueOption(dataRow["REJECT_OPTION"].ToString().Trim());
					}
					DataTable dataTable4 = dataAccess.getRejectOptions("TRASH").Tables[0];
					foreach (DataRow dataRow in dataTable4.Rows)
					{
						dictionary.Add(dataRow["REJECT_OPTION"].ToString().Trim(), dataRow["REASON_CODE"].ToString().Trim());
					}
					DataTable dataTable5 = dataAccess.getRejectOptions("INTER_OFFICE").Tables[0];
					foreach (DataRow dataRow in dataTable5.Rows)
					{
						dictionary.Add(dataRow["REJECT_OPTION"].ToString().Trim(), dataRow["REASON_CODE"].ToString().Trim());
					}
					DialogResult dialogResult = validationRejectForm.ShowDialog();
					if (dialogResult == DialogResult.OK)
					{
						string rejectResult = validationRejectForm.RejectResult;
						this.rejectReasonCode = dictionary[rejectResult];
						controlContext.FindFieldData("RejectOption").SetValue(rejectResult);
						controlContext.FindFieldData("Validation").SetValue("");
						controlContext.FindFieldData("Validated").SetValue("");
					}
					else
					{
						controlContext.SetText("0");
					}
				}
				else
				{
					this.rejectReasonCode = "";
					controlContext.FindFieldData("Validation").SetValue("");
					controlContext.FindFieldData("Validated").SetValue("");
				}
			}
			catch (Exception var_11_43F)
			{
				controlContext.SetText("0");
				this.rejectReasonCode = "";
				controlContext.FindFieldData("Validation").SetValue("");
				controlContext.FindFieldData("Validated").SetValue("");
			}
		}
	}
}
