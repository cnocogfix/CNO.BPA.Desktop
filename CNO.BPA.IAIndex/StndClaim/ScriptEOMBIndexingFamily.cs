using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using CNO.BPA.DataHandler;

namespace Custom.InputAccel.UimScript
{
    using Emc.InputAccel.CaptureClient;
    using Emc.InputAccel.UimScript;

    public class ScriptEOMBIndexingFamily : UimScriptDocument
    {
        private const string _validationRule = "One";
        string rejectReasonCode;
        bool extraction;

        public ScriptEOMBIndexingFamily()
            : base()
        {
            extraction = false;
            rejectReasonCode = "";

        }

        public void DocumentLoad(IUimDataContext dataContext)
        {

            dataContext.TaskFinishOnErrorNotAllowed = true;

            //MessageBox.Show("DocumentLoad event!!!");

            //Set OperatorID
            if (Share.getOperatorID() == "*")
            {
                dataContext.FindFieldDataContext("Operator").SetValue(ModCommon.getOperatorID());
            }
            else
            {
                dataContext.FindFieldDataContext("Operator").SetValue(Share.getOperatorID());
            }

            //Set machine name
            dataContext.FindFieldDataContext("MACHINE_NAME").SetValue(ModCommon.getMachineName());
            

            try
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        public void DocumentUnload(IUimDataContext dataContext)
        {
            
            //MessageBox.Show("DocumentUnload event");
            try
            {

                if (!extraction)
                {
                    string rejected = dataContext.FindFieldDataContext("Rejected").Text;
                    if (rejected == "1")
                    {

                        CNO.BPA.DataHandler.CommonParameters cp = new CNO.BPA.DataHandler.CommonParameters();
                        cp.BatchNo = dataContext.FindFieldDataContext("BatchName").Text.Trim();
                        cp.CurrentNodeID = dataContext.FindFieldDataContext("NodeID").Text.Trim();
                        cp.ProcessStep = "ManualValidation";
                        cp.ReasonCode = rejectReasonCode;
                        DataAccess dataAccess = new DataAccess();
                        dataAccess.createBatchItemReject(ref cp);

                    }
                    //else
                    //{
                    //    DataAccess dataAccess = new DataAccess();
                    //    dataAccess.updateReasonCode(dataContext.FindFieldDataContext("BatchName").Text.Trim(), dataContext.FindFieldDataContext("NodeID").Text.Trim(), "Unrejected");
                    //}
                }

            }
            catch (Exception ex)
            {

            }

        }


        public void DocumentExtracted(IUimDataContext dataContext)
        {
            extraction = true;

            //Set OperatorID
            if (Share.getOperatorID() == "*")
            {
                dataContext.FindFieldDataContext("Operator").SetValue(ModCommon.getOperatorID());
            }
            else
            {
                dataContext.FindFieldDataContext("Operator").SetValue(Share.getOperatorID());
            }

            //PolicyNumber1(dataContext);
            //PreIndex1(dataContext);
            //RoutingCode(dataContext);
            //LineOfBusiness(dataContext);

            //extraction = false;

        }

        public void SelectionChangeRejected(IUimFormControlContext controlContext)
        {
            try
            {
                if (controlContext.ChoiceValue == "1")
                {
                    ValidationRejectForm rejectForm = new ValidationRejectForm();
                    DataAccess dataAccess = new DataAccess();

                    Dictionary<string, string> rejectOptions = new Dictionary<string, string>();

                    DataTable automateToIndexOptionsTable = dataAccess.getRejectOptions("AUTOMATE_INDEX").Tables[0];
                    foreach (DataRow row in automateToIndexOptionsTable.Rows)
                    {
                        rejectOptions.Add(row["REJECT_OPTION"].ToString().Trim(), row["REASON_CODE"].ToString().Trim());
                        rejectForm.addWrongDeptOption(row["REJECT_OPTION"].ToString().Trim());
                    }


                    DataTable wrongCompanyOptionsTable = dataAccess.getRejectOptions("WRONG_COMPANY").Tables[0];
                    foreach (DataRow row in wrongCompanyOptionsTable.Rows)
                    {
                        rejectOptions.Add(row["REJECT_OPTION"].ToString().Trim(), row["REASON_CODE"].ToString().Trim());
                        rejectForm.addWrongCompanyOption(row["REJECT_OPTION"].ToString().Trim());
                    }

                    //DataTable issuesTable = dataAccess.getRejectOptions("ISSUES").Tables[0];
                    DataTable issuesTable = dataAccess.getRejectOptions("INDEX_ISSUES").Tables[0];
                    foreach (DataRow row in issuesTable.Rows)
                    {
                        rejectOptions.Add(row["REJECT_OPTION"].ToString().Trim(), row["REASON_CODE"].ToString().Trim());
                        rejectForm.addIssueOption(row["REJECT_OPTION"].ToString().Trim());
                    }

                    DataTable TrashTable = dataAccess.getRejectOptions("TRASH").Tables[0];
                    foreach (DataRow row in TrashTable.Rows)
                    {
                        rejectOptions.Add(row["REJECT_OPTION"].ToString().Trim(), row["REASON_CODE"].ToString().Trim());
                    }

                    DataTable InterofficeTable = dataAccess.getRejectOptions("INTER_OFFICE").Tables[0];
                    foreach (DataRow row in InterofficeTable.Rows)
                    {
                        rejectOptions.Add(row["REJECT_OPTION"].ToString().Trim(), row["REASON_CODE"].ToString().Trim());
                    }

                    DialogResult dRes = rejectForm.ShowDialog();

                    if (dRes == DialogResult.OK)
                    {
                        string rejectResult = rejectForm.RejectResult;
                        rejectReasonCode = rejectOptions[rejectResult];
                        controlContext.FindFieldData("Validation").SetValue(rejectResult);
                        controlContext.FindFieldData("Validated").SetValue(rejectResult);

                    }
                    else
                    {
                        controlContext.SetText("0");
                    }
                }
                else //Unmarking reject button
                {
                    rejectReasonCode = "";
                    controlContext.FindFieldData("Validation").SetValue("");
                    controlContext.FindFieldData("Validated").SetValue("");
                }
            }
            catch (Exception ex)
            {
                controlContext.SetText("0");
                rejectReasonCode = "";
                controlContext.FindFieldData("Validation").SetValue("");
                controlContext.FindFieldData("Validated").SetValue("");
            }

        }
      
        public void ExecuteValidationRuleOne(IUimDataContext dataContext)
        {
            
        }


        public void ExitControlLOB(IUimFormControlContext controlContext)
        {
            LineOfBusiness(controlContext.ParentForm.UimDataContext);
        }

        private void LineOfBusiness(IUimDataContext dataContext)
        {
            IUimFieldDataContext field = dataContext.FindFieldDataContext("LOB");

            //translate all @ to 0s
            field.SetValue(field.Text.Replace('@', '0'));

            //remove all punctuation marks
            field.SetValue(ModCommon.RemovePunctuation(field.Text));

            //Trim all leading and trailing spaces
            field.SetValue(field.Text.Trim());

            //Remove double spaces if any
            field.SetValue(field.Text.Replace("  ", " "));

        }

        public void ExitControlROUTE_CODE(IUimFormControlContext controlContext)
        {
            RoutingCode(controlContext.ParentForm.UimDataContext);
        }

        private void RoutingCode(IUimDataContext dataContext)
        {
            IUimFieldDataContext field = dataContext.FindFieldDataContext("ROUTE_CODE");

            //translate all @ to 0s
            field.SetValue(field.Text.Replace('@', '0'));

            //remove all punctuation marks
            field.SetValue(ModCommon.RemovePunctuation(field.Text));

            //Trim all leading and trailing spaces
            field.SetValue(field.Text.Trim());

            //Remove double spaces if any
            field.SetValue(field.Text.Replace("  ", " "));

        }

        public void ExitControlOCR_POLICY_NO1(IUimFormControlContext controlContext)
        {
            PolicyNumber1(controlContext.ParentForm.UimDataContext);
        }

        private void PolicyNumber1(IUimDataContext dataContext)
        {
            string sField;
            string sTemp1;
            string sTemp2;
            string sTemp3;
            string sTemp4;

            IUimFieldDataContext field = dataContext.FindFieldDataContext("OCR_POLICY_NO1");

            //Data Cleanup
	        //Translate all Os to 0s
            field.SetValue(field.Text.Replace('O', '0'));
            field.SetValue(field.Text.Replace('o', '0'));


            //Backup the field data after first value
            sField = field.Text;
            
            //translate all @ to 0s
            field.SetValue(field.Text.Replace('@', '0'));

            //compress the HIC
            field.SetValue(field.Text.Replace(" ", ""));

            //if the Policy Number begins with P then translate the next character which may be an "i" or "l" or "1" to "L"
            if (field.Text.Length < 9)
            {
                return;
            }
            if (field.Text.Substring(0, 2).ToUpper() == "PI" || field.Text.Substring(0, 2).ToUpper() == "PL")
            {
                field.SetValue("PL" + field.Text.Substring(2, field.Text.Length));
            }

            //Remove all punctuation marks
            field.SetValue(ModCommon.RemovePunctuation(field.Text));
            try
            {
                sTemp1 = field.Text.Substring(0, 9);
                sTemp2 = field.Text.Substring(9, 1);
                sTemp3 = field.Text.Substring(10, 1);
                sTemp4 = field.Text.Substring(11, 10);

            }
            catch (ArgumentOutOfRangeException ex)
            {
                return;
            }
            //If the policy number has 9 digits followed by 4, 8 or 0 then translate the 4 to A, 8 to B and 0 to D
            int integer;
            if (int.TryParse(sTemp1, out integer) && field.Text.Length == 10)
            {
                if (sTemp2 == "4")
                {
                    sTemp2 = "A";
                }
                if (sTemp2 == "8")
                {
                    sTemp2 = "B";
                }
                if (sTemp2 == "0")
                {
                    sTemp2 = "D";
                }
                field.SetValue(sTemp1 + sTemp2);
                
            }

            //If the policy number has 9 digits followed by 4, 8 or 0 followed by a digit then translate the 4 to A, 8 to B and 0 to D.
            if (int.TryParse(sTemp1, out integer) && field.Text.Length == 11 && int.TryParse(sTemp3, out integer))
            {
                if (sTemp2 == "4")
                {
                    sTemp2 = "A";
                }
                if (sTemp2 == "8")
                {
                    sTemp2 = "B";
                }
                if (sTemp2 == "0")
                {
                    sTemp2 = "D";
                }
                field.SetValue(sTemp1 + sTemp2 + sTemp3);
            }


            //If the policy number validation still fails, then revert abck to original number got from extraction and aftre first rule
            if (field.Text.Length == 10 || field.Text.Length == 11)
            {
                if (int.TryParse(sTemp1, out integer) && (sTemp3 == "" || int.TryParse(sTemp3, out integer)))
                {
                    if (sTemp2 == "A" || sTemp2 == "B" || sTemp2 == "D")
                    {
                        return;
                    }
                }
            }

            field.SetValue(sField);

            //If the policy number begins with a 4 or 8 followed by 9 digits then translate the 4 to A, 8 to B
            sTemp1 = field.Text.Substring(0, 1);
            sTemp2 = field.Text.Substring(1, 9);
            if (field.Text.Length == 10)
            {
                if (int.TryParse(sTemp2, out integer))
                {
                    if (sTemp1 == "4")
                    {
                        sTemp1 = "A";
                    }
                    if (sTemp1 == "8")
                    {
                        sTemp1 = "B";
                    }
                    field.SetValue(sTemp1 + sTemp2);
                }
            }

        }

        public void ExitControlPreIndex1(IUimFormControlContext controlContext)
        {
            PreIndex1(controlContext.ParentForm.UimDataContext);
        }
        
        private void PreIndex1(IUimDataContext dataContext)
        {
            string sField;
            string sTemp1;
            string sTemp2;
            string sTemp3;
            string sTemp4;

            IUimFieldDataContext field = dataContext.FindFieldDataContext("PreIndex1");

            //Data Cleanup
            //Translate all Os to 0s
            field.SetValue(field.Text.Replace('O', '0'));
            field.SetValue(field.Text.Replace('o', '0'));


            //Backup the field data after first value
            sField = field.Text;

            //translate all @ to 0s
            field.SetValue(field.Text.Replace('@', '0'));

            //compress the HIC
            field.SetValue(field.Text.Replace(" ", ""));

            //if the Policy Number begins with P then translate the next character which may be an "i" or "l" or "1" to "L"
            if (field.Text.Substring(0, 2).ToUpper() == "PI" || field.Text.Substring(0, 2).ToUpper() == "PL")
            {
                field.SetValue("PL" + field.Text.Substring(2, field.Text.Length));
            }

            //Remove all punctuation marks
            field.SetValue(ModCommon.RemovePunctuation(field.Text));
            try
            {
                sTemp1 = field.Text.Substring(0, 9);
                sTemp2 = field.Text.Substring(9, 1);
                sTemp3 = field.Text.Substring(10, 1);
                sTemp4 = field.Text.Substring(11, 10);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return;
            }

            //If the policy number has 9 digits followed by 4, 8 or 0 then translate the 4 to A, 8 to B and 0 to D
            int integer;
            if (int.TryParse(sTemp1, out integer) && field.Text.Length == 10)
            {
                if (sTemp2 == "4")
                {
                    sTemp2 = "A";
                }
                if (sTemp2 == "8")
                {
                    sTemp2 = "B";
                }
                if (sTemp2 == "0")
                {
                    sTemp2 = "D";
                }
                field.SetValue(sTemp1 + sTemp2);

            }

            //If the policy number has 9 digits followed by 4, 8 or 0 followed by a digit then translate the 4 to A, 8 to B and 0 to D.
            if (int.TryParse(sTemp1, out integer) && field.Text.Length == 11 && int.TryParse(sTemp3, out integer))
            {
                if (sTemp2 == "4")
                {
                    sTemp2 = "A";
                }
                if (sTemp2 == "8")
                {
                    sTemp2 = "B";
                }
                if (sTemp2 == "0")
                {
                    sTemp2 = "D";
                }
                field.SetValue(sTemp1 + sTemp2 + sTemp3);
            }


            //If the policy number validation still fails, then revert abck to original number got from extraction and aftre first rule
            if (field.Text.Length == 10 || field.Text.Length == 11)
            {
                if (int.TryParse(sTemp1, out integer) && (sTemp3 == "" || int.TryParse(sTemp3, out integer)))
                {
                    if (sTemp2 == "A" || sTemp2 == "B" || sTemp2 == "D")
                    {
                        return;
                    }
                }
            }

            field.SetValue(sField);

            //If the policy number begins with a 4 or 8 followed by 9 digits then translate the 4 to A, 8 to B
            sTemp1 = field.Text.Substring(0, 1);
            sTemp2 = field.Text.Substring(1, 9);
            if (field.Text.Length == 10)
            {
                if (int.TryParse(sTemp2, out integer))
                {
                    if (sTemp1 == "4")
                    {
                        sTemp1 = "A";
                    }
                    if (sTemp1 == "8")
                    {
                        sTemp1 = "B";
                    }
                    field.SetValue(sTemp1 + sTemp2);
                }
            }

        }
    }
}
