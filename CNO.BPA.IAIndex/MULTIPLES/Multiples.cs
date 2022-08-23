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

    public class ScriptMultiples : UimScriptDocument
    {

        [System.Runtime.InteropServices.DllImport("iaclnt32.dll", SetLastError=true)] 
        static extern void IADebugOutVB_PHEOMB(int iaclient, string strText);

        private const string _validationRule = "One";
        //private string ddrunning;

        Director _dataDirector;// = new Director();
        DataAccess _dataAccess;// = new DataAccess();
        CommonParameters _cp;
        int _docNodeId;
        IUimFieldDataContext _valField;
        //IUimDataContext _dataContext;

        //private Dictionary<string, string> values;
        DocumentNode docNode;

        private bool AutoValidationRan = false;
        private bool extraction = false;

        public ScriptMultiples() : base()
        {
            try
            {
                _dataDirector = new Director();
                _dataAccess = new DataAccess();
                //values = new Dictionary<string, string>();
                //values.Add("E_DDRUNNING", "F");

            }
            catch (Exception ex)
            {
                

            }

        }

        public void DocumentLoad(IUimDataContext dataContext)
        {

            dataContext.TaskFinishOnErrorNotAllowed = true;
            //_dataContext = dataContext;

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
                if (ModCommon.gstrMachineName.Length == 0)
                {
                    dataContext.FindFieldDataContext("MACHINE_NAME").SetValue(ModCommon.getMachineName());
                }

                //IIAValueProvider nodeValueProvider = taskInfo.TaskRootNode.Values(taskInfo.Application.CurrentTask.WorkflowStep);
                //string nodePath = "$node=" + taskInfo.TaskRootNode.Id.ToString();
                //ddrunning = nodeValueProvider.GetString(nodePath + "/$instance=Standard_MDF/E_DDRUNNING", "");

                if (Share.getDD_Running() == true)
                {
                   //set ddrunning to f in case task was previously closed improperly
                    Share.setDD_Running(false); // nodeValueProvider.SetString(nodePath + "/$instance=Standard_MDF/E_DDRUNNING", "F");

                }
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
                extraction = false;

                if (ModCommon.gstrMachineName.Length == 0)
                {
                    dataContext.FindFieldDataContext("MACHINE_NAME").SetValue(ModCommon.getMachineName());
                }

                string rejected = dataContext.FindFieldDataContext("Rejected").Text;
                if (rejected == "1")
                {

                    CommonParameters CP = new CommonParameters();
                    string BatchNo = docNode.getValues()["BATCH_NO"];
                    CP.BatchNo = BatchNo.Trim();
                    CP.CurrentNodeID = docNode.getValues()["NODE_ID"];
                    DataAccess dataAcces = new DataAccess();
                    dataAcces.createBatchItemReject(ref CP);

                }

                //TODO: Default(set to 0) Replicate value on the last document no matter what..\


                //dataContext.FindFieldDataContext("Rejected")
                dataContext.FindFieldDataContext("D_VALIDATION_AUDIT_START").SetValue(docNode.getValues()["D_VALIDATION_AUDIT_START"]);
                dataContext.FindFieldDataContext("D_BATCH_ITEM_ID").SetValue(docNode.getValues()["D_BATCH_ITEM_ID"]);
                dataContext.FindFieldDataContext("D_DD_ITEM_SEQ").SetValue(docNode.getValues()["D_DD_ITEM_SEQ"]);

                		        	//stnd_mdf_d.WriteString("D_VALIDATION_AUDIT_START", data.FindFieldDataContext("D_VALIDATION_AUDIT_START").Text);
		        	//stnd_mdf_d.WriteString("D_BATCH_ITEM_ID", data.FindFieldDataContext("D_BATCH_ITEM_ID").Text);
		        	//stnd_mdf_d.WriteString("D_DD_ITEM_SEQ", data.FindFieldDataContext("D_DD_ITEM_SEQ").Text);
		        	//D_VALIDATION
		        	//Reject info from DD

                //Clear tree structure
                if (Share.getTree().Count > 0)
                {
                    Share.getTree().Clear();
                }


                docNode = null;
            }
            catch (Exception ex)
            {

            }

        }

        public void FormLoad(IUimDataEntryFormContext form)
        {



            //Save values to Share in case document splits
            if (docNode.getValues() != null && !string.IsNullOrEmpty(docNode.getNodeId().ToString()))
            {
                //Share.SetProperties(new Dictionary<string, string>(docNode.getValues()));
                Share.setLastSelectedNode(docNode);
            }

            //Retrieve values from Share to doc level values.
            /*
            if (Share.getProperties() != null && (docNode.getValues() == null || string.IsNullOrEmpty(docNode.getValues()["NODE_ID"])))
            {
                docNode.setValues(Share.getProperties());
                //Share.SetProperties(null);
            }*/

            /*
            if (Share.getSplittingLastDoc() == true)
            {
                Share.setLastDocNodeId(Convert.ToInt32(values["NODE_ID"]));
                Share.setSplittingLastDoc(false);
            }

            if (Share.getLastDocNodeId() == 0)
            {
                Share.setLastDocNodeId(Convert.ToInt32(values["NODE_ID"]));
            }*/

            //Hide replicate field for last document
            //form.

        }

        public void DocumentExtracted(IUimDataContext dataContext) {
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

            AUTO_POLICY_NUMBER_AfterRecognition(dataContext);
            AutoValidation(dataContext);


            //TODO: bring in SITE_ID from Desktop_prepare if it's blank set it to "CIC"
            /*If gstrStepHCFA = "AUTOVAL" Then //if Extraction
		        Set objExternalValue = CurrentBatch.ExternalValues("SiteIDValue")

		        If Not (objExternalValue Is Nothing) Then
			        CurrentDocument.Fields.Item("SITE_ID").Value = objExternalValue.Value
		        Else
			        CurrentDocument.Fields.Item("SITE_ID").Value = "CIC"
		        End If

	        End If*/

            //extraction = false;
        }


        public void SelectionChangeRejected(IUimFormControlContext controlContext)
        {
            string originalStatus = controlContext.Text;
            ValidationRejectForm rejectForm = new ValidationRejectForm();
            //IBatchInformationListParameters ibparms = info.Application.CreateBatchInformationListParameters();

            //foreach (IBatchInformation ibinfo in info.Application.ProcessList(ibparms))
            //{
            //   rejectForm.addProcess(ibinfo.Name);
            //}
            DataAccess dataAcces = new DataAccess();
            List<string> IAProcessList = dataAcces.getIAProcessNames();
            foreach (string processName in IAProcessList)
            {
                //////rejectForm.addProcess(processName);
            }

            DialogResult dRes = rejectForm.ShowDialog();

            if (dRes == DialogResult.OK)
            {
                string rejectResult = rejectForm.RejectResult;
                controlContext.FindFieldData("Validation").SetValue(rejectResult);
                controlContext.FindFieldData("Validated").SetValue(rejectResult);   

            }
            else
            {
                if (originalStatus == "1")
                {
                    controlContext.SetText("0");
                }
                else
                {
                    controlContext.SetText("1");
                }
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

                Share.setDD_Running(true);


                
                //_dataDirector.LaunchDD(ref _cp);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

        }

        public void ExecuteValidationRuleOne(IUimDataContext dataContext)
        {
  


            //PVValidate SECTION START
            /*if(AutoValidationRan == true) {

                ValidatePVReturn(dataContext);
            }*/
            //PVValidate SECTION END

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

        private void AutoValidation(IUimDataContext dataContext)
        {
            string strTemplateName;
            string strTemplateCode;
            string strAutoCompanyCodeAC; 
            bool lngRetValBankers;
            bool lngRetValCPL;
            string strPolicyNumber; 
            string VALIDATED_STATUS; //As Variant
            int intlenPolicy_Number;

            try {

                //Get the template name
                strTemplateName = dataContext.FindFieldDataContext("TEMPLATE_NAME").Text;
                strTemplateCode = dataContext.FindFieldDataContext("TEMPLATE_CODE").Text;
            
            
                //Assign the worktype based on template code
                if (strTemplateCode == "PLCYASSIGN")
                {
                    dataContext.FindFieldDataContext("AUTO_WORK_TYPE").SetValue("CLNTMAINT");


                }
                else if (strTemplateCode == "REQBENECHG")
                {
                    dataContext.FindFieldDataContext("AUTO_WORK_TYPE").SetValue("BENE");


                }
                else if (strTemplateCode == "EFT")
                {
                    dataContext.FindFieldDataContext("AUTO_WORK_TYPE").SetValue( "PACSETUP");


                }
                else if (strTemplateCode == "LOAN")
                {
                    dataContext.FindFieldDataContext("AUTO_WORK_TYPE").SetValue( "LOAN");


                }
                else if (strTemplateCode == "SURRENDER")
                {
                    dataContext.FindFieldDataContext("AUTO_WORK_TYPE").SetValue( "SURRENDER");


                }
                else if (strTemplateCode == "CANCEL")
                {
                    dataContext.FindFieldDataContext("AUTO_WORK_TYPE").SetValue( "CANCEL");


                }
                else if (strTemplateCode == "BILL")
                {
                    dataContext.FindFieldDataContext("AUTO_WORK_TYPE").SetValue("CBMAINT");


                }
                else
                {
                    dataContext.FindFieldDataContext("VALIDATED").SetValue("");

                }

                //START: Code for ASSIGNMENT_OF_POLICY form
                //Check if the current template is Bankers or CPL
                lngRetValBankers = strTemplateName.Contains("ASSIGNMENT_BANKERS");
                lngRetValCPL = strTemplateName.Contains("ASSIGNMENT_CPL");

                if (lngRetValBankers && !lngRetValCPL)
                {
                    dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").SetValue("ILI");
                }
                else if (!lngRetValBankers && lngRetValCPL)
                {
                    dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").SetValue("CPL");
                }
            
                //END: Code for ASSIGNMENT_OF_POLICY form

                //START: Code for CHANGE_OF_BENEFICIARY form
                //Check if the current template is Bankers or CPL
                lngRetValBankers = strTemplateName.Contains("BENE_BANKERS");
                lngRetValCPL = strTemplateName.Contains("BENE_CPL");

                if (lngRetValBankers && !lngRetValCPL)
                {
                    dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").SetValue("ILI");
                }
                else if (!lngRetValBankers && lngRetValCPL)
                {
                    dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").SetValue("CPL");
                }
	            //END: Code for CHANGE_OF_BENEFICIARY form

	            //START: Code for CANCEL_FORM_LETTER form
	            //Check if the current template is Bankers or CPL
                lngRetValBankers = strTemplateName.Contains("CANCEL_BANKERS");
                lngRetValCPL = strTemplateName.Contains("CANCEL_CPL");

                if (lngRetValBankers && !lngRetValCPL)
                {
                    dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").SetValue("ILI");
                }
                else if (!lngRetValBankers && lngRetValCPL)
                {
                    dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").SetValue("CPL");
                }
	            //END: Code for CANCEL_FORM_LETTER form

	            //START: Code for COMBINED_BILL_FORM form
	            //Check if the current template is Bankers or CPL
                lngRetValBankers = strTemplateName.Contains("BILL_BANKERS");
                lngRetValCPL = strTemplateName.Contains("BILL_CPL");

                if (lngRetValBankers && !lngRetValCPL)
                {
                    dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").SetValue("ILI");
                }
                else if (!lngRetValBankers && lngRetValCPL)
                {
                    dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").SetValue("CPL");
                }
                //END: Code for COMBINED_BILL_FORM form

                //START: Code for EFT form
                //Check if the current template is Bankers or CPL
                if (strTemplateCode == "EFT")
                {
                    lngRetValBankers = strTemplateName.Contains("EFT_BANKERS");
                    lngRetValCPL = strTemplateName.Contains("EFT_CPL");

                    if (lngRetValBankers && !lngRetValCPL)
                    {
                        dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").SetValue("ILI");
                    }
                    else if (!lngRetValBankers && lngRetValCPL)
                    {
                        dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").SetValue("CPL");
                    }

                    if (strTemplateName == "EFT_KW")
                    {
                        strAutoCompanyCodeAC = dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").Text;

                        lngRetValBankers = strAutoCompanyCodeAC.Contains("Bankers Conseco");
                        lngRetValCPL = strAutoCompanyCodeAC.Contains("Colonial Penn");

                        if (lngRetValBankers && !lngRetValCPL)
                        {
                            dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").SetValue("ILI");
                        }
                        else if (!lngRetValBankers && lngRetValCPL)
                        {
                            dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").SetValue("CPL");
                        }
                        else
                        {
                            dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").SetValue("");
                        }
                    }

                }
                //END: Code for EFT form

                //START: Code for POLICYHOLDER_CASH_SURRENDER form
                //Check if the current template is Bankers or CPL
                lngRetValBankers = strTemplateName.Contains("SURRENDER_BANKERS");
                lngRetValCPL = strTemplateName.Contains("SURRENDER_CPL");

                if (lngRetValBankers && !lngRetValCPL)
                {
                    dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").SetValue("ILI");
                }
                else if (!lngRetValBankers && lngRetValCPL)
                {
                    dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").SetValue("CPL");
                }
                //END: Code for POLICYHOLDER_CASH_SURRENDER form

                //START: Code for POLICYHOLDER_LOAN_REQUEST form
                //Check if the current template is Bankers or CPL
                lngRetValBankers = strTemplateName.Contains("LOAN_BANKERS");
                lngRetValCPL = strTemplateName.Contains("LOAN_CPL");

                if (lngRetValBankers && !lngRetValCPL)
                {
                    dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").SetValue("ILI");
                }
                else if (!lngRetValBankers && lngRetValCPL)
                {
                    dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").SetValue("CPL");
                }
                //END: Code for POLICYHOLDER_LOAN_REQUEST form

                //Perform check digit validation on Policy Number
                //Get the Policy Number
	            strPolicyNumber = dataContext.FindFieldDataContext("AUTO_POLICY_NUMBER").Text;

	            intlenPolicy_Number = strPolicyNumber.Length;

	            if (!string.IsNullOrEmpty(strPolicyNumber) && intlenPolicy_Number == 10) {
		            //Call method CheckDigitPolicyNumber
		            VALIDATED_STATUS = CheckDigitSum(strPolicyNumber, dataContext);

		            dataContext.FindFieldDataContext("VALIDATED").SetValue(VALIDATED_STATUS);


		            if (string.IsNullOrEmpty(VALIDATED_STATUS)) {
			            //Make policy number blank if it is invalid
			            dataContext.FindFieldDataContext("AUTO_POLICY_NUMBER").SetValue("");

		            }


	            } else {
		            dataContext.FindFieldDataContext("VALIDATED").SetValue("");

		            //Make policy number blank if it is invalid
		            dataContext.FindFieldDataContext("AUTO_POLICY_NUMBER").SetValue("");

	            }


	            if (!string.IsNullOrEmpty(dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").Text) & !string.IsNullOrEmpty(dataContext.FindFieldDataContext("VALIDATED").Text)) {
		            dataContext.FindFieldDataContext("VALIDATED").SetValue("SUCCESS");

	            } else {
		            dataContext.FindFieldDataContext("VALIDATED").SetValue("");

	            }

                if (string.IsNullOrEmpty(dataContext.FindFieldDataContext("VALIDATED").Text.Trim())) {
	                dataContext.FindFieldDataContext("VALIDATED").SetDataConfirmed(false); //RequiresValidation = true;
                    dataContext.FindFieldDataContext("VALIDATED").SetValidationError(new Exception());
                } else {
                    dataContext.FindFieldDataContext("VALIDATED").SetDataConfirmed(true); //CurrentDocument.Fields("VALIDATED").RequiresValidation = false;
	                //CurrentDocument.Fields("VALIDATED").SetStatusOK();
                }


            } catch (Exception ex) {

            } finally {

            }
        }

        private void AUTO_POLICY_NUMBER_AfterRecognition(IUimDataContext dataContext)
        {
            dataContext.FindFieldDataContext("AUTO_POLICY_NUMBER").SetValue(ModCommon.RemovePunctuation(dataContext.FindFieldDataContext("AUTO_POLICY_NUMBER").Text));

        }


        private string CheckDigitSum(string Policy_Number, IUimDataContext dataContext)
        {
            int lngLenPolicy_Number = 0;
            char[] strArrayPolicy_Number = null;
            char[] tempPolicy_Number = null;// As Variant
            int ILoop = 0;
            int JLoop = 0;
            bool IsCharOrDigit1 = false;
            bool IsCharOrDigit2 = false;
            bool IsFirstCharANumber;
            bool IsSecondCharANumber;
            bool IsNumber = false;
            int firstDigit = 0;
            int secondDigit = 0;
            int Sum = 0;
            int Remainder = 0;
            char chkDigit_PolicyNumber; // As Variant
            string result = "";

            try
            {

                //'Finding the length of policy number
                lngLenPolicy_Number = Policy_Number.Length;

                //'Reallocate the "Policy_Number" storage space to "strArrayPolicy_Number" string array variable
                strArrayPolicy_Number = Policy_Number.ToCharArray();

                //'Reallocate the "Policy_Number"+1 storage space to "tempPolicy_Number" string array variable
                tempPolicy_Number = new char[lngLenPolicy_Number + 2];


                //Check the first two characters of Policy_Number are Digit or Alphabet and convert.
                if ((int)strArrayPolicy_Number[0] >= 65 && (int)strArrayPolicy_Number[0] <= 90)
                {
                    firstDigit = ModCommon.ReplacewithDigit((int)strArrayPolicy_Number[0]);

                    tempPolicy_Number[0] = (char)(firstDigit + 48);

                    IsCharOrDigit1 = true;
                    IsFirstCharANumber = false;
                }
                else if ((int)strArrayPolicy_Number[0] >= 48 && (int)strArrayPolicy_Number[0] <= 57)
                {
                    tempPolicy_Number[0] = strArrayPolicy_Number[0]; // error within old code.. Old Code: tempPolicy_Number(1) = strArrayPolicy_Number(1)

                    IsCharOrDigit1 = true;
                    IsFirstCharANumber = true;
                }
                else
                {
                    IsCharOrDigit1 = false;
                    IsFirstCharANumber = false;
                }

                if ((int)strArrayPolicy_Number[1] >= 65 && (int)strArrayPolicy_Number[1] <= 90)
                {
                    secondDigit = ModCommon.ReplacewithDigit((int)strArrayPolicy_Number[1]);

                    tempPolicy_Number[1] = (char)(secondDigit + 48);

                    IsCharOrDigit2 = true;
                    IsSecondCharANumber = false;
                }
                else if ((int)strArrayPolicy_Number[1] >= 48 && (int)strArrayPolicy_Number[1] <= 57)
                {
                    tempPolicy_Number[1] = strArrayPolicy_Number[1];

                    IsCharOrDigit2 = true;
                    IsSecondCharANumber = true;
                }
                else
                {
                    IsCharOrDigit2 = false;
                    IsSecondCharANumber = false;
                }


                //'If first two letters of Policy_Number are Alphabet or Digit
                if (IsFirstCharANumber != true || IsSecondCharANumber != true)
                {
                    if (IsCharOrDigit1 == true && IsCharOrDigit2 == true)
                    {
                        ILoop = 2;
                        //'Check the 3 to 9 characters of Policy_Number are Digit or Alphabet
                        while (ILoop <= lngLenPolicy_Number - 2)
                        {
                            if ((int)strArrayPolicy_Number[ILoop] >= 48 && (int)strArrayPolicy_Number[ILoop] <= 57)
                            {
                                IsNumber = true;
                            }
                            else
                            {
                                IsNumber = false;
                                break;
                            }
                            ILoop++;

                        }

                        //'If 3 to 9 cahracters of Policy_Number are Digit from 0 to 9
                        if (IsNumber == true)
                        {
                            //'Insert "0" value at third index of String Array variable
                            tempPolicy_Number[2] = '0';

                            //'Move the values from one string array variable to another string array variable
                            for (ILoop = 2; ILoop <= lngLenPolicy_Number - 2; ILoop++)
                            {
                                strArrayPolicy_Number[ILoop] = Policy_Number[ILoop];

                                tempPolicy_Number[ILoop + 1] = strArrayPolicy_Number[ILoop];

                            }

                            Sum = 0;

                            ILoop = lngLenPolicy_Number - 1;

                            JLoop = 2;

                            //'As per checkdigit algorithm calculated the Sum
                            while (ILoop >= 0 && JLoop <= 7)
                            {
                                Sum = Sum + (tempPolicy_Number[ILoop] * JLoop); // DANGEROUS.. multiplication as a char or a number? Ambiguity here due to variant type.. TODO

                                if (JLoop == 7)
                                {
                                    JLoop = 1;
                                }

                                JLoop = JLoop + 1;
                                ILoop = ILoop - 1;
                            }

                            //Peform Mod operation on Sum
                            Remainder = Sum % 11;

                            //Substract the remainder from 11
                            chkDigit_PolicyNumber = (char)((11 - Remainder) + 48);


                            if (chkDigit_PolicyNumber == 10)
                            {
                                //if checkdigit is 10 then replace with "0"
                                chkDigit_PolicyNumber = '0';


                            }
                            else if (chkDigit_PolicyNumber == 11)
                            {
                                chkDigit_PolicyNumber = 'B';

                            }



                            if (chkDigit_PolicyNumber == strArrayPolicy_Number[lngLenPolicy_Number - 1])
                            {
                                result = "SUCCESS";
                            }
                            else
                            {
                                result = "";
                            }

                        }
                        else
                        {
                            result = "";

                        }
                        //is a number if else ends

                    }
                    else
                    {
                        result = "";

                    }

                }

                return result;
            }
            catch (Exception ex)
            {
                int errNum = 0;
                string batchName = "";
                ModCommon.WriteError(errNum, ex.Message, "CheckDigitSum", batchName);

                //dataContext.FindFieldDataContext("Validation").OutputMessage = strErrDesc;
                dataContext.FindFieldDataContext("Validation").SetValidationError(ex);
            }
            return null;
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
            catch (Exception ex)
            {

            }

        }


   
    }
}
