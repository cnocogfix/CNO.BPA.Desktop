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

    public class ScriptCHECK1 : UimScriptDocument
    {
        private const string _validationRule = "One";
 
        public ScriptCHECK1()
            : base()
        {

        }
      
        public void DocumentUnload(IUimDataContext dataContext)
        {
        }

        
     
        public void DocumentExtracted(IUimDataContext dataContext) {

            //Set OperatorID
            if (Share.getOperatorID() == "*")
            {
                dataContext.FindFieldDataContext("Operator").SetValue(ModCommon.getOperatorID());
            }
            else
            {
                dataContext.FindFieldDataContext("Operator").SetValue(Share.getOperatorID());
            }

            CHECK_ISSUE_DATE_AfterRecogntion(dataContext.FindFieldDataContext("CHECK_ISSUE_DATE"));
            CHECK_AMOUNT_AfterRecognition(dataContext.FindFieldDataContext("CHECK_AMOUNT"));
            POLICY_NUMBER_AfterRecognition(dataContext.FindFieldDataContext("POLICY_NUMBER"));
        }

        public void ExecuteValidationRuleOne(IUimDataContext dataContext)
        {
            
        }
        
        private void CHECK_AMOUNT_ValidateValue(IUimDataContext dataContext)
        {
            IUimFieldDataContext localDpDocField = dataContext.FindFieldDataContext("CHECK_AMOUNT");

            try
            {
                //everything goes directly to Index, if the field is not valid blank it out
                //test
                if (localDpDocField.FlaggedReason != "")
                {
                    localDpDocField.SetValue("");
                }
            }
            catch (Exception ex)
            {
                localDpDocField.SetValue("");
            }
        }

        private void CHECK_ISSUE_DATE_AfterRecogntion(IUimFieldDataContext localDpDocField)
        {
            string strLocalDpDocField = localDpDocField.ToString();
            strLocalDpDocField = strLocalDpDocField.Replace(" ", ""); //yeah we could've used trim but I'm just coming as-is code
            localDpDocField.SetValue(strLocalDpDocField);
        }

        private void CHECK_AMOUNT_AfterRecognition(IUimFieldDataContext localDpDocField)
        {
            bool foundNumber;

            string strLocalDpDocField = localDpDocField.ToString();
            strLocalDpDocField = strLocalDpDocField.Replace(",", "");
            strLocalDpDocField = strLocalDpDocField.Replace("$", "");

            //remove leading decimals
            foundNumber = false;
            while (foundNumber == false)
            {
                if (strLocalDpDocField.Length > 0)
                {
                    if (strLocalDpDocField.Substring(0, 1) == ".")  
                    {
                        localDpDocField.SetValue(strLocalDpDocField.Substring(1, strLocalDpDocField.Length - 1));
                    }
                    else
                    {
                        foundNumber = true;
                    }
                }
                else
                {
                    foundNumber = true;
                }
            }
        }

        private void POLICY_NUMBER_AfterRecognition(IUimFieldDataContext localDpDocField)
        {
            CNO.BPA.DataHandler.CommonParameters objDH = new CNO.BPA.DataHandler.CommonParameters();
            CNO.BPA.DataValidation.Validation objDV = new CNO.BPA.DataValidation.Validation();
            int intReturnValue;
            bool foundOther;
            int Opos;

            string strLocalDpDocField = localDpDocField.Text; //change it to a string so we can maniupulate the data
            //remove spaces
            strLocalDpDocField = strLocalDpDocField.Replace(" ", "");//could've used trim, but I'm copying the as-is code 
            //change leading O's to 0's
            foundOther = false;
            Opos = 0;
            while (foundOther == false && Opos < strLocalDpDocField.Length-1)
            {
                if (strLocalDpDocField.Length > 0)
                {
                    if (strLocalDpDocField.Substring(Opos, 1) == "O")
                    {
                        strLocalDpDocField = strLocalDpDocField.Substring(0,Opos) + "0" + strLocalDpDocField.Substring(Opos+1);
                        Opos = Opos + 1;
                    }
                    else
                    {
                        foundOther = true;
                    }
                }
                else
                {
                    foundOther = true;
                }
            }

            //writing the new string back to the field, just for data intergrity purposes
            localDpDocField.SetValue(strLocalDpDocField);

            //initialize parameters
            objDH.ID = strLocalDpDocField;
            objDH.WorkCategory = "RCHK";
            objDH.SiteID = "CPL";

            intReturnValue = objDV.Validate(ref objDH, true);

            if (intReturnValue == 0)
            {
                //pull back the form values
                localDpDocField.UimDataContext.FindFieldDataContext("POLICY_NUMBER").SetValue(objDH.PolicyNo);
                localDpDocField.UimDataContext.FindFieldDataContext("SYSTEM_ID").SetValue(objDH.SystemID);
                localDpDocField.UimDataContext.FindFieldDataContext("COMPANY_CODE").SetValue(objDH.CompanyCode);
            }
            else
            {
                //as is code references localDpDocField.Value = "", in this context localDpDocField is equal to "POLICY_NUMBER"
                localDpDocField.UimDataContext.FindFieldDataContext("POLICY_NUMBER").SetValue("");
            }




        }

    }
}
