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

    public class ScriptCheck : UimScriptDocument
    {
        private const string _validationRule = "One";

        public ScriptCheck()
            : base()
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

            CHECK_AMOUNT_AfterRecognition(dataContext.FindFieldDataContext("CHECK_AMOUNT"));
            CHECK_ISSUE_DATE_AfterRecogntion(dataContext.FindFieldDataContext("CHECK_ISSUE_DATE"));
        }

       


        public void ExecuteValidationRuleOne(IUimDataContext dataContext)
        {
           
        }

        

  
        private void CHECK_AMOUNT_ValidateValue(IUimFieldDataContext localDpDocField)
        {
            if (localDpDocField.FlaggedReason != "")
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
                    if (strLocalDpDocField.Substring(1, 1) == ".")
                    {
                        localDpDocField.SetValue(strLocalDpDocField.Substring(1));
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

    }
}
