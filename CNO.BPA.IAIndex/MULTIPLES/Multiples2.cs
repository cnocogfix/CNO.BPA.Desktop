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

    public class ScriptMultiples2 : UimScriptDocument
    {

        public ScriptMultiples2() : base()
        {
            
        }

        public void DocumentLoad(IUimDataContext dataContext)
        {

            dataContext.TaskFinishOnErrorNotAllowed = true;


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

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        public void DocumentUnload(IUimDataContext dataContext)
        {
 
        }

        public void FormLoad(IUimDataEntryFormContext form)
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

           
        }   
    }
}
