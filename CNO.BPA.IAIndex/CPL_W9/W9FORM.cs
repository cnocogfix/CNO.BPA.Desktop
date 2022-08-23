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

    public class ScriptW9FORM : UimScriptDocument
    {
        private const string _validationRule = "One";

        public ScriptW9FORM()
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


            dataContext.FindFieldDataContext("AUTO_POLICY_NUMBER").SetValue(ModCommon.RemovePunctuation(dataContext.FindFieldDataContext("AUTO_POLICY_NUMBER").Text));
            AutoValidation(dataContext);
        }

        public void ExecuteValidationRuleOne(IUimDataContext dataContext)
        {
            if (dataContext.FindFieldDataContext("VALIDATED").ToString() == "")
            {
                
            }
        }

       
    
        private void AutoValidation(IUimDataContext dataContext)
        {
            try
            {
                dataContext.FindFieldDataContext("AUTO_BUSINESS_AREA").SetValue("CPLDNBL");
                dataContext.FindFieldDataContext("AUTO_WORK_TYPE").SetValue("BACKSCAN");
                dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").SetValue("CPL");

                if (dataContext.FindFieldDataContext("AUTO_POLICY_NUMBER").ToString() != "")
                {
                    dataContext.FindFieldDataContext("VALIDATED").SetValue("SUCCESS");
                }
                else
                {
                    dataContext.FindFieldDataContext("VALIDATED").SetValue("");
                }


            }
            catch (Exception ex)
            {
                //WriteError(ex.Message.ToString(), "AutoValidation");
                //not sure how to set output message or setstatuserror
            }

        }

    }
}
