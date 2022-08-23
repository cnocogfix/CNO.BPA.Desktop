using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using CNO.BPA.DataHandler;
using CNO.BPA.DataDirector;
using CNO.BPA.DataValidation;
using CNO.BPA.Validation4086;

namespace Custom.InputAccel.UimScript
{
    using Emc.InputAccel.CaptureClient;
    using Emc.InputAccel.UimScript;

    public class ScriptBACKSCANFORM : UimScriptDocument
    {
        [System.Runtime.InteropServices.DllImport("iaclnt32.dll", SetLastError = true)]
        static extern void IADebugOutVB_PHEOMB(int iaclient, string strText);

        private const string _validationRule = "One";
        
        public ScriptBACKSCANFORM()
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

            //AutoValidation(dataContext);
        }


        private void AutoValidation(IUimDataContext dataContext)
        {
            try
            {
                int[] intCompanyCode = new int[2];
                int[] intDocumentType = new int[29];
                string strTemplateCode;
                int intCount;
                int intValue;
                int intCompanyCode_FieldTotal;
                int intDocumentType_FieldTotal;

                dataContext.FindFieldDataContext("AUTO_WORK_TYPE").SetValue("BACKSCAN");
                dataContext.FindFieldDataContext("AUTO_BUSINESS_AREA").SetValue("CPLPIFL");

                intCompanyCode[0] = Convert.ToInt32(dataContext.FindFieldDataContext("COMPANY_CPL").Text);
                intCompanyCode[1] = Convert.ToInt32(dataContext.FindFieldDataContext("COMPANY_ILI").Text);
                intCompanyCode[2] = Convert.ToInt32(dataContext.FindFieldDataContext("COMPANY_LFC").Text);

                //Assign the Document Type
                //for loop to populate the first 20 values
                for (int i = 0; i <= 20; i++)
                {
                    string value ="";  //used if we need a leading 0 included 
                    int y = i + 1;
                    if(i < 10)
                    {
                        value = "0" + y.ToString();
                    }
                    intDocumentType[i] = Convert.ToInt32(dataContext.FindFieldDataContext("DOCTYPE_DOC" + value + y.ToString()));
                }
                //the other 8 slots does not have a predictable numbering scheme so I need to manually write them out
                intDocumentType[21] = Convert.ToInt32(dataContext.FindFieldDataContext("DOCTYPE_DOC43"));
                intDocumentType[22] = Convert.ToInt32(dataContext.FindFieldDataContext("DOCTYPE_DOC44"));
                intDocumentType[23] = Convert.ToInt32(dataContext.FindFieldDataContext("DOCTYPE_DOC45"));
                intDocumentType[24] = Convert.ToInt32(dataContext.FindFieldDataContext("DOCTYPE_DOC60"));
                intDocumentType[25] = Convert.ToInt32(dataContext.FindFieldDataContext("DOCTYPE_DOC66"));
                intDocumentType[26] = Convert.ToInt32(dataContext.FindFieldDataContext("DOCTYPE_DOC88"));
                intDocumentType[27] = Convert.ToInt32(dataContext.FindFieldDataContext("DOCTYPE_DOC98"));
                intDocumentType[28] = Convert.ToInt32(dataContext.FindFieldDataContext("DOCTYPE_DOC99"));

                //Extracting the company code
                intCompanyCode_FieldTotal = intCompanyCode[0] + intCompanyCode[1] + intCompanyCode[2];

                if (intCompanyCode_FieldTotal == 1)
                {
                    if (intCompanyCode[0] == 1)
                    {
                        dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").SetValue("CPL");
                    }
                    else if (intCompanyCode[1] == 1)
                    {
                        dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").SetValue("ILI");
                    }
                    else if (intCompanyCode[2] == 1)
                    {
                        dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").SetValue("LFC");
                    }
                }
                else if (intCompanyCode_FieldTotal > 1)
                {
                    dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").SetValue("");
                }
                else if (intCompanyCode_FieldTotal == 0)
                {
                    dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").SetValue("CPL");
                }

                intDocumentType_FieldTotal = 0;

                for (int i = 0; i <= 28; i++) {
                    intDocumentType_FieldTotal = intDocumentType_FieldTotal + intDocumentType[i];
                }
                
                if (intDocumentType_FieldTotal == 1)
                {
                    //bool first = true;  //used to determine if this is the first instance of the condition being successful
                    //the as-is process code uses Elseif's so the action can only occur if the previous condition fails
                    //therefore we will use first to ensure that this condition only triggers when the last condition had failed
                    //int value;

                    if (intDocumentType[0] == 1) {
                        dataContext.FindFieldDataContext("AUTO_MOLIDOC_TYPE").SetValue("0001");
                    } else if (intDocumentType[1] == 1) {
                        dataContext.FindFieldDataContext("AUTO_MOLIDOC_TYPE").SetValue("0002");
                    } else if (intDocumentType[2] == 1) {
                        dataContext.FindFieldDataContext("AUTO_MOLIDOC_TYPE").SetValue("0003");
                    } else if (intDocumentType[3] == 1) {
                        dataContext.FindFieldDataContext("AUTO_MOLIDOC_TYPE").SetValue("0004");
                    } else if (intDocumentType[4] == 1) {
                        dataContext.FindFieldDataContext("AUTO_MOLIDOC_TYPE").SetValue("0005");
                    } else if (intDocumentType[5] == 1) {
                        dataContext.FindFieldDataContext("AUTO_MOLIDOC_TYPE").SetValue("0006");
                    } else if (intDocumentType[6] == 1) {
                        dataContext.FindFieldDataContext("AUTO_MOLIDOC_TYPE").SetValue("0007");
                    } else if (intDocumentType[7] == 1) {
                        dataContext.FindFieldDataContext("AUTO_MOLIDOC_TYPE").SetValue("0008");
                    } else if (intDocumentType[8] == 1) {
                        dataContext.FindFieldDataContext("AUTO_MOLIDOC_TYPE").SetValue("0009");
                    } else if (intDocumentType[9] == 1) {
                        dataContext.FindFieldDataContext("AUTO_MOLIDOC_TYPE").SetValue("0010");
                    } else if (intDocumentType[10] == 1) {
                        dataContext.FindFieldDataContext("AUTO_MOLIDOC_TYPE").SetValue("0011");
                    } else if (intDocumentType[11] == 1) {
                        dataContext.FindFieldDataContext("AUTO_MOLIDOC_TYPE").SetValue("0012");
                    } else if (intDocumentType[12] == 1) {
                        dataContext.FindFieldDataContext("AUTO_MOLIDOC_TYPE").SetValue("0013");
                    } else if (intDocumentType[13] == 1) {
                        dataContext.FindFieldDataContext("AUTO_MOLIDOC_TYPE").SetValue("0014");
                    } else if (intDocumentType[14] == 1) {
                        dataContext.FindFieldDataContext("AUTO_MOLIDOC_TYPE").SetValue("0015");
                    } else if (intDocumentType[15] == 1) {
                        dataContext.FindFieldDataContext("AUTO_MOLIDOC_TYPE").SetValue("0016");
                    } else if (intDocumentType[16] == 1) {
                        dataContext.FindFieldDataContext("AUTO_MOLIDOC_TYPE").SetValue("0017");
                    } else if (intDocumentType[17] == 1) {
                        dataContext.FindFieldDataContext("AUTO_MOLIDOC_TYPE").SetValue("0018");
                    } else if (intDocumentType[18] == 1) {
                        dataContext.FindFieldDataContext("AUTO_MOLIDOC_TYPE").SetValue("0019");
                    } else if (intDocumentType[19] == 1) {
                        dataContext.FindFieldDataContext("AUTO_MOLIDOC_TYPE").SetValue("0020");
                    } else if (intDocumentType[20] == 1) {
                        dataContext.FindFieldDataContext("AUTO_MOLIDOC_TYPE").SetValue("0021");
                    } else if (intDocumentType[21] == 1) {
                        dataContext.FindFieldDataContext("AUTO_MOLIDOC_TYPE").SetValue("0043");
                    } else if (intDocumentType[22] == 1) {
                        dataContext.FindFieldDataContext("AUTO_MOLIDOC_TYPE").SetValue("0044");
                    } else if (intDocumentType[23] == 1) {
                        dataContext.FindFieldDataContext("AUTO_MOLIDOC_TYPE").SetValue("0045");
                    } else if (intDocumentType[24] == 1) {
                        dataContext.FindFieldDataContext("AUTO_MOLIDOC_TYPE").SetValue("0060");
                    } else if (intDocumentType[25] == 1) {
                        dataContext.FindFieldDataContext("AUTO_MOLIDOC_TYPE").SetValue("0066");
                    } else if (intDocumentType[26] == 1) {
                        dataContext.FindFieldDataContext("AUTO_MOLIDOC_TYPE").SetValue("0088");
                    } else if (intDocumentType[27] == 1) {
                        dataContext.FindFieldDataContext("AUTO_MOLIDOC_TYPE").SetValue("0098");
                    } else if (intDocumentType[28] == 1) {
                        dataContext.FindFieldDataContext("AUTO_MOLIDOC_TYPE").SetValue("0099");
                    }
            
                }
                else
                {
                    dataContext.FindFieldDataContext("AUTO_MOLIDOC_TYPE").SetValue("");
                }

                //Validation Check
                if (dataContext.FindFieldDataContext("AUTO_MOLIDOC_TYPE").Text != "" && dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").Text != "")
                {
                    dataContext.FindFieldDataContext("VALIDATED").SetValue("SUCCESS");
                }
                else
                {
                    dataContext.FindFieldDataContext("VALIDATED").SetValue("");
                }

                if (dataContext.FindFieldDataContext("VALIDATED").Text.Trim() == "")
                {
                    //TODO
                    //    CurrentDocument.Fields("VALIDATED").RequiresValidation = True
                    //    CurrentDocument.Fields("VALIDATED").SetStatusError
                    
                }
                else
                {
                    //TODO
                       /*CurrentDocument.Fields("VALIDATED").RequiresValidation = False
                        CurrentDocument.Fields("VALIDATED").SetStatusOK
                        */
                }
            }
            catch (Exception ex)
            {
                int errNum = 0;
                string errMessage = ex.Message;
                WriteError(errNum, errMessage, "AutoValidation", "");
                //not sure how to set output message or setstatuserror
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
            catch (Exception ex)
            {
                //If this fails Nothing more we can do...
            }

        }

    }
}
