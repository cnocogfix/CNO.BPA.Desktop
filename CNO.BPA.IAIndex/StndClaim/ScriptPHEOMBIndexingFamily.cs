using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using CNO.BPA.DataHandler;
using CNO.BPA.DataDirector;
using CNO.BPA.DataValidation;

namespace Custom.InputAccel.UimScript
{
    using Emc.InputAccel.CaptureClient;
    using Emc.InputAccel.UimScript;

    public class ScriptPHEOMBIndexingFamily : UimScriptDocument
    {

        [System.Runtime.InteropServices.DllImport("iaclnt32.dll", SetLastError=true)] 
        static extern void IADebugOutVB_PHEOMB(int iaclient, string strText);

        private const string _validationRule = "One";
        string rejectReasonCode;
        bool extraction;

        Director _dataDirector;// = new Director();
        DataAccess _dataAccess;// = new DataAccess();
        CommonParameters _cp;
 
        //private Dictionary<string, string> values;
        DocumentNode docNode;

        /*
         *  ExternalValues: { SiteIDValue, Template_Code }
         */
        string[] externalValues;

        public ScriptPHEOMBIndexingFamily()
            : base()
        {
            try
            {
                extraction = false;
                rejectReasonCode = "";
            }
            catch (Exception e)
            {
                //e.

            }

        }

        public void DocumentLoad(IUimDataContext dataContext)
        {

            dataContext.TaskFinishOnErrorNotAllowed = true;
           
            //MessageBox.Show("DocumentLoad event!!!");

            try
            {

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

                //IIAValueProvider nodeValueProvider = taskInfo.TaskRootNode.Values(taskInfo.Application.CurrentTask.WorkflowStep);
                //string nodePath = "$node=" + taskInfo.TaskRootNode.Id.ToString();
                //ddrunning = nodeValueProvider.GetString(nodePath + "/$instance=Standard_MDF/E_DDRUNNING", "");

                if (Share.getDD_Running() == true)
                {
                   //set ddrunning to f in case task was previously closed improperly
                    Share.setDD_Running(false); // nodeValueProvider.SetString(nodePath + "/$instance=Standard_MDF/E_DDRUNNING", "F");
                }
                //             //Extraction methods does not work in Identification Step [21.4 upgrade 20th July 2022]
                /*

                string ruleFailMessage = "Validation field is empty";
                if (!dataContext.FindFieldDataContext("Validation").Text.ToUpper().Contains("VAL"))
                {
                    dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                    Exception ex = new Exception(ruleFailMessage);
                    dataContext.FindFieldDataContext("Validation").SetValidationError(ex);
                }
                */

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

        
        public void DocumentExtracted(IUimDataContext dataContext) {
            extraction = true;

            //External Values
            char[] delimiterChar = { ';' };
            externalValues = dataContext.StepCustomValue.Split(delimiterChar);

            //Set OperatorID
            if (Share.getOperatorID() == "*")
            {
                dataContext.FindFieldDataContext("Operator").SetValue(ModCommon.getOperatorID());
            }
            else
            {
                dataContext.FindFieldDataContext("Operator").SetValue(Share.getOperatorID());
            }


            //AutoHICLookup(dataContext);
        }

        public void ExitControl(IUimFormControlContext controlContext)
        {
            //MessageBox.Show("ExitControl event");


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
        
        public void ButtonClickDataDirector(IUimFormControlContext controlContext)
        {
            try
            {
                
                if (Share.getDD_Running() == true)
                {
                    MessageBox.Show("Data Director is already running. Please complete current document before attempting to launch Data Director again.", "Data Director already running.");
                    return;
                }

                LaunchHICSearch(controlContext.ParentForm.UimDataContext);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }



            //MessageBox.Show("ButtonClickDataDirector event");
            try
            {
                //controlContext.ui

                IUimFieldDataContext field1 = controlContext.FindFieldData("D_COMPANY_CODE");
                MessageBox.Show(field1.Text);

                //Form1 newForm = new Form1();
                //newForm.Show();


                //IUimDataContext dataContext = controlContext.ParentForm.UimDataContext;

                //controlContext.

                // do lookup using name entered by user.  Blank should force list of all vendors
                //LookupFromVendorName(dataContext, "");
            }
            catch (Exception e)
            {
                Console.Write(e);
            }
        }

        public void ExecuteValidationRuleOne(IUimDataContext dataContext)
        {
            basicFieldCleanUp(dataContext.FindFieldDataContext("ROUTE_CODE"));
            basicFieldCleanUp(dataContext.FindFieldDataContext("LOB"));
            basicFieldCleanUp(dataContext.FindFieldDataContext("OCR_POLICY_NO1"));
            //HIC look up
            
            //Validate Validation
            if (dataContext.FindFieldDataContext("Validation").Text.Trim() == "")
            {
                string ruleFailMessage = "Validation field invalid";
                dataContext.SetValidationRuleFailMessage( _validationRule, ruleFailMessage);
                throw new Exception(ruleFailMessage);
            }

            //Validate OCR_POLICY_NO1 START
            int lngLen;
            //Field clean up
            policyNumberCleanUp(dataContext.FindFieldDataContext("OCR_POLICY_NO1"));
            
            //CIG Policy Number format check
            if (dataContext.FindFieldDataContext("SITE_ID").Text.Trim().ToUpper() == "CIC")
            {
                lngLen = dataContext.FindFieldDataContext("OCR_POLICY_NO1").Text.Trim().Length;
                if (lngLen != 0 && (lngLen < 5 || lngLen > 10))
                {
                    string ruleFailMessage = "CIG Policy #'s must be blank or 6-10 chars";
                    dataContext.SetValidationRuleFailMessage( _validationRule, ruleFailMessage);
                    throw new Exception(ruleFailMessage);
                }

            }

            //BLC Policy Number format check
            if (dataContext.FindFieldDataContext("SITE_ID").Text.Trim().ToUpper() == "BLC")
            {
                //Policy Number shuold be no greater than 9 digits.
                lngLen = dataContext.FindFieldDataContext("OCR_POLICY_NO1").Text.Trim().Length;
                if (lngLen > 9)
                {
                    string ruleFailMessage = "BLC Policy #'s may not exceed 9 digits";
                    dataContext.SetValidationRuleFailMessage( _validationRule, ruleFailMessage);
                    throw new Exception(ruleFailMessage);
                }
                //Policy Number must be numeric
                int result;
                if (!int.TryParse(dataContext.FindFieldDataContext("OCR_POLICY_NO1").Text, out result) && lngLen > 0)
                {
                    string ruleFailMessage = "BLC Policy #'s must be numeric";
                    dataContext.SetValidationRuleFailMessage( _validationRule, ruleFailMessage);
                    throw new Exception(ruleFailMessage);
                }
            
            }
            //Validate OCR_POLICY_NO1 END



            /*
             	If UCase(Trim(CurrentDocument.Fields("SITE_ID").Value )) = "BLC" Then
		            'Policy Number should be no greater than 9 digits.
		            lngLen = Len(Trim(CurrentField.Value))
		            If lngLen > 9 Then
			            CurrentField.SetStatusError	
			            CurrentField.OutputMessage = "BLC Policy #'s may not exceed 9 digits"
		            End If
		            'Policy Number must be numeric
		            If Not IsNumeric(CurrentField.Value) And lngLen > 0 Then
			            CurrentField.SetStatusError
			            CurrentField.OutputMessage = "BLC Policy #'s must be numeric"
		            End If
	            End If 
             
             * /




            bool emptyDocument = false;
            foreach(DocumentNode doc in Share.getTree().AsEnumerable()) {
                if (doc.isEmpty() == true) {
                    emptyDocument = true;
                }
            }
            if (emptyDocument == true)
            {
                string ruleFailMessage = "There is an empty document.";
                dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                throw new Exception(ruleFailMessage);
            }

            /*if (inode.Next() == null && replicate == "1")
            {
                string ruleFailMessage = "Replicate is not a valid selection for the last document in an envelope.  Document " + values["NODE_ID"] + ".";
                dataContext.SetValidationRuleFailMessage( _validationRule, ruleFailMessage);
                throw new Exception(ruleFailMessage);
            }*/
            }

        public void ExitControlROUTE_CODE(IUimFormControlContext controlContext)
        {
            basicFieldCleanUp(controlContext.FindFieldData("ROUTE_CODE"));
        }

        public void ExitControlLOB(IUimFormControlContext controlContext)
        {
            basicFieldCleanUp(controlContext.FindFieldData("LOB"));
        }

        public void ExitControlValidation(IUimFormControlContext controlContext)
        {
            if (controlContext.Text.Trim() != "")
            {
                   //   
            }
        }

        private void basicFieldCleanUp(IUimFieldDataContext field)
        {
            //IUimFieldDataContext field = controlContext.FindFieldData(fieldName);

            //translate all @ to 0s
            field.SetValue(field.Text.Replace('@', '0'));

            //remove all punctuation marks
            field.SetValue(ModCommon.RemovePunctuation(field.Text));

            //Trim all leading and trailing spaces
            field.SetValue(field.Text.Trim());

            //Remove double spaces if any
            field.SetValue(field.Text.Replace("  ", " "));
        }

        private void policyNumberCleanUp(IUimFieldDataContext field)
        {
            if (field.Name != "OCR_POLICY_NO1") {
                return;
            }

            field.SetValue(field.Text.Replace('@', '0'));
            field.SetValue(field.Text.Replace('O', '0'));
            field.SetValue(field.Text.Replace('o', '0'));

            if (field.UimDataContext.FindFieldDataContext("SITE_ID").Text.Trim() == "BLC")
            {
                field.SetValue(ModCommon.removeAlphaNumeric(field.Text));
            }

            if (field.UimDataContext.FindFieldDataContext("SITE_ID").Text.Trim() == "CIC")
            {
                if (field.Text.Substring(0, 2).ToUpper() == "PI" || field.Text.Substring(0, 2).ToUpper() == "PL")
                {
                    field.SetValue("PL" + field.Text.Substring(2, field.Text.Length));
                }
            }

            field.SetValue(ModCommon.RemovePunctuation(field.Text));

            field.SetValue(field.Text.Replace(" ", ""));

        }

        private void LaunchHICSearch(IUimDataContext dataContext)
        {
            int lngReturnValue;
            string strSourceData = "";

            CommonParameters CP = new CommonParameters();
            CNO.BPA.DataValidation.Validation Validation = new CNO.BPA.DataValidation.Validation();

            CP.SiteID = dataContext.FindFieldDataContext("SITE_ID").Text.Trim();
            CP.WorkCategory = "CLM";

            Share.setDD_Running(true);
            lngReturnValue = Validation.Validate(ref CP);
            Share.setDD_Running(false);

            if (!string.IsNullOrEmpty(CP.RejectCode))
            {
                dataContext.FindFieldDataContext("Rejected").SetValue("1");
                rejectReasonCode = CP.RejectCode;
                CP.Clear();
                return;
            }

            if (lngReturnValue == 0)
            {
                dataContext.FindFieldDataContext("AUTO_POLICY_NO1").SetValue(CP.PolicyNo);
                dataContext.FindFieldDataContext("AUTO_INSURED_NAME").SetValue(CP.InsuredName);
                dataContext.FindFieldDataContext("AUTO_SORT_CODE").SetValue(CP.SortCode);
                dataContext.FindFieldDataContext("ADMIN_COMPANY_CODE").SetValue(CP.CompanyNo);
                dataContext.FindFieldDataContext("ADMIN_SYSTEM_ID").SetValue(CP.SystemNo);
                dataContext.FindFieldDataContext("SYSTEM_ID").SetValue(CP.SystemID);
                dataContext.FindFieldDataContext("COMPANY_CODE").SetValue(CP.CompanyCode);
                dataContext.FindFieldDataContext("VALIDATED").SetValue("(" + CP.Validation.ToString() + ")");
                dataContext.FindFieldDataContext("Validation").SetValue("(" + CP.Validation.ToString() + ")");

                if (CP.SortCode.Trim().Length == 0 || CP.SortCode.Trim() == "000")
                {
                    dataContext.FindFieldDataContext("SORT_CODE").SetValue("000");
                    dataContext.FindFieldDataContext("LOB").SetValue("MS");
                }
                else
                {
                    dataContext.FindFieldDataContext("SORT_CODE").SetValue(CP.SortCode);
                    dataContext.FindFieldDataContext("LOB").SetValue(CP.LineOfBusiness);
                }

                validatePVReturn(dataContext);
                CP.Clear();

                if (dataContext.FindFieldDataContext("SITE_ID").Text.Trim() == "CIC")
                {
                    if (dataContext.FindFieldDataContext("LOB").Text == "MS")
                    {
                        strSourceData = "2034";
                    }
                    else if (dataContext.FindFieldDataContext("LOB").Text == "OH")
                    {
                        strSourceData = "2024";
                    }
                    else if (dataContext.FindFieldDataContext("LOB").Text == "LTC")
                    {
                        strSourceData = "2002";
                    }
                }
                else if (dataContext.FindFieldDataContext("SITE_ID").Text.Trim() == "BLC")
                {
                    if (dataContext.FindFieldDataContext("LOB").Text == "MS")
                    {
                        strSourceData = "1935";
                    }
                    else if (dataContext.FindFieldDataContext("LOB").Text == "OH")
                    {
                        strSourceData = "1936";
                    }
                }

                dataContext.FindFieldDataContext("LOBSourceData").SetValue(strSourceData);


            }

        }
        
        private void AutoHICLookup(IUimDataContext dataContext)
        {
            try
            {
                int lngReturnValue;
                string strSourceData = "";

                CommonParameters CP = new CommonParameters();
                Validation Validation = new Validation();

                CP.SiteID = dataContext.FindFieldDataContext("SITE_ID").Text.Trim();
                CP.WorkCategory = "CLM";
                CP.ID = dataContext.FindFieldDataContext("OCR_POLICY_NO1").Text;
                CP.SiteRestriction = "T";

                lngReturnValue = Validation.ValidateHIC(ref CP);

                if (lngReturnValue == 0)
                {
                    dataContext.FindFieldDataContext("AUTO_POLICY_NO1").SetValue(CP.PolicyNo);
                    dataContext.FindFieldDataContext("AUTO_INSURED_NAME").SetValue(CP.InsuredName);
                    dataContext.FindFieldDataContext("AUTO_SORT_CODE").SetValue(CP.SortCode);
                    dataContext.FindFieldDataContext("ADMIN_COMPANY_CODE").SetValue(CP.CompanyNo);
                    dataContext.FindFieldDataContext("ADMIN_SYSTEM_ID").SetValue(CP.SystemNo);
                    dataContext.FindFieldDataContext("SYSTEM_ID").SetValue(CP.SystemID);
                    dataContext.FindFieldDataContext("COMPANY_CODE").SetValue(CP.CompanyCode);
                    dataContext.FindFieldDataContext("AUTO_BUSINESS_AREA").SetValue(CP.BusinessArea);
                    dataContext.FindFieldDataContext("AUTO_WORK_TYPE").SetValue(CP.WorkType);
                    dataContext.FindFieldDataContext("VALIDATED").SetValue("(" + CP.Validation.ToString() + ")");
                    dataContext.FindFieldDataContext("Validation").SetValue("(" + CP.Validation.ToString() + ")");

                    if (CP.SortCode.Trim().Length == 0 || CP.SortCode.Trim() == "000")
                    {
                        dataContext.FindFieldDataContext("SORT_CODE").SetValue("000");
                        dataContext.FindFieldDataContext("LOB").SetValue("MS");
                    }
                    else
                    {
                        dataContext.FindFieldDataContext("SORT_CODE").SetValue(CP.SortCode);
                        dataContext.FindFieldDataContext("LOB").SetValue(CP.LineOfBusiness);
                    }

                    validatePVReturn(dataContext);

                    if (dataContext.FindFieldDataContext("SITE_ID").Text.Trim() == "CIC")
                    {
                        if (dataContext.FindFieldDataContext("LOB").Text == "MS")
                        {
                            strSourceData = "2034";
                        }
                        else if (dataContext.FindFieldDataContext("LOB").Text == "OH")
                        {
                            strSourceData = "2024";
                        }
                        else if (dataContext.FindFieldDataContext("LOB").Text == "LTC")
                        {
                            strSourceData = "2002";
                        }
                    }
                    else if (dataContext.FindFieldDataContext("SITE_ID").Text.Trim() == "BLC")
                    {
                        if (dataContext.FindFieldDataContext("LOB").Text == "MS")
                        {
                            strSourceData = "1935";
                        }
                        else if (dataContext.FindFieldDataContext("LOB").Text == "OH")
                        {
                            strSourceData = "1936";
                        }
                    }

                    dataContext.FindFieldDataContext("LOBSourceData").SetValue(strSourceData);


                }

                CP.Clear();

                /*
                     
                    If Trim(CurrentDocument.Fields("Validation").Value) = "" Then
                        CurrentDocument.Fields("Validation").RequiresValidation = True
                        CurrentDocument.Fields("Validation").SetStatusError
                    Else
                        Dim beginsWith As String
                        beginsWith = "(IA:"

                        If Left (UCase(Trim(CurrentDocument.Fields("Validation").Value)),Len(beginsWith)) <> beginsWith Then
                            CurrentDocument.Fields("Validation").RequiresValidation = True
                            CurrentDocument.Fields("Validation").SetStatusError
                        Else
                            CurrentDocument.Fields("Validation").RequiresValidation = False
                            CurrentDocument.Fields("Validation").SetStatusOK
                        End If
                    End If
                     
                 */

            }
            catch (Exception ex)
            {
                //WriteError();
            }

        }

        private void validatePVReturn(IUimDataContext dataContext)
        {
            int lngLen;

            foreach (IUimFieldDataContext field in dataContext.GetFieldDataContextArray())
            {
                switch (field.Name)
                {
                    case "AUTO_POLICY_NO1":
                        //field clean up
                        policyNumberCleanUp(field);

                        //CIG policy number format check
                        if (dataContext.FindFieldDataContext("SITE_ID").Text.Trim().ToUpper() == "CIC")
                        {
                            //Policy Number must be blank or 5 or 10 characters in length
                            lngLen = field.Text.Trim().Length;
                            if (lngLen != 0 && (lngLen < 5 || lngLen > 10))
                            {
                                //field
                            }
                        }

                        break;


                }
            }
        }

        private void WriteError(int errorNum, string errorDesc, string errorLocation, string batchName)
        {
            string strData;

            try
            {

                strData = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + " | " + errorNum + " | " + 
                    errorDesc + " | " + errorLocation + " | " + batchName;
                IADebugOutVB_PHEOMB(0, strData);

            }
            catch(Exception ex)
            {

            }

        }
    }
}
