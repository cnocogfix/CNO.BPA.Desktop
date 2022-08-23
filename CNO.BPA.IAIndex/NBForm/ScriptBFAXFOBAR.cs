using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using CNO.BPA.DataHandler;
using CNO.BPA.DataDirector;
using CNO.BPA.DataValidation;
using CNO.BPA.PrivacyMailingValidation;

namespace Custom.InputAccel.UimScript
{
    using Emc.InputAccel.CaptureClient;
    using Emc.InputAccel.UimScript;

    public class ScriptBFAXBFOBAR: UimScriptDocument
    {
        [System.Runtime.InteropServices.DllImport("iaclnt32.dll", SetLastError = true)]
        static extern void IADebugOutVB_PHEOMB(int iaclient, string strText);

        private const string _validationRule = "One";
        //private string ddrunning;

        Director _dataDirector;// = new Director();
        DataAccess _dataAccess;// = new DataAccess();
        CommonParameters _cp;
        int _docNodeId;
        IUimFieldDataContext _valField;
        IUimDataContext _dataContext;

        //private Dictionary<string, string> values;
        DocumentNode docNode;

        public ScriptBFAXBFOBAR()
            : base()
        {
            try
            {
                _dataDirector = new Director();
                _dataAccess = new DataAccess();
                //values = new Dictionary<string, string>();
                //values.Add("E_DDRUNNING", "F");
            }
            catch (Exception e)
            {


            }

        }
public void DocumentExtracted(IUimDataContext dataContext)
		{
			if (Share.getOperatorID() == "*")
			{
				dataContext.FindFieldDataContext("Operator").SetValue(ModCommon.getOperatorID());
			}
			else
			{
				dataContext.FindFieldDataContext("Operator").SetValue(Share.getOperatorID());
			}
		}

		private void AutoValidation(IUimDataContext dataContext)
		{
			try
			{
				CommonParameters commonParameters = new CommonParameters();
				Validation validation = new Validation();
				string[] array = new string[6];
				if (dataContext.FindFieldDataContext("TemplateCode").ToString() == "C14")
				{
					dataContext.FindFieldDataContext("AUTO_DOC_TYPE_ORIG").SetValue("APP");
					dataContext.FindFieldDataContext("AUTO_DOC_TYPE").SetValue("APP");
					if (dataContext.FindFieldDataContext("C14BarCode").ToString() == "???" || dataContext.FindFieldDataContext("C14BarCode").ToString().Length < 10)
					{
						dataContext.FindFieldDataContext("AUTO_DOC_TYPE_ORIG").SetValue("UKN");
					}
				}
				if (dataContext.FindFieldDataContext("AUTO_CONTROL_NO").ToString() == "")
				{
					commonParameters.ControlNo = "";
				}
				else
				{
					commonParameters.ControlNo = dataContext.FindFieldDataContext("AUTO_CONTROL_NO").ToString();
				}
				if (dataContext.FindFieldDataContext("AUTO_DOC_TYPE_ORIG").ToString() == "")
				{
					dataContext.FindFieldDataContext("AUTO_DOC_TYPE_ORIG").SetValue("UKN");
					commonParameters.DocTypeOrig = "UKN";
				}
				else
				{
					commonParameters.DocTypeOrig = dataContext.FindFieldDataContext("AUTO_DOC_TYPE_ORIG").ToString();
				}
				string a;
				if (dataContext.FindFieldDataContext("TemplateCode").ToString() != "ATTACHMENT")
				{
					a = validation.getALCCC14data(ref commonParameters);
					if (a == "SUCCESS")
					{
						dataContext.FindFieldDataContext("AUTO_REPLACEMENT_INDICATOR").SetValue(commonParameters.ReplacementIndicator);
						dataContext.FindFieldDataContext("AUTO_STATE").SetValue(commonParameters.State);
						dataContext.FindFieldDataContext("AUTO_TRANSACTION_TYPE").SetValue(commonParameters.TransType);
						dataContext.FindFieldDataContext("AUTO_WRITING_AGENT").SetValue(commonParameters.WritingAgent);
						if (dataContext.FindFieldDataContext("AUTO_PRODUCT_CATEGORY").ToString().Contains("$"))
						{
							if (!(dataContext.FindFieldDataContext("AUTO_PRODUCT_CATEGORY").ToString().Substring(0, 3) == commonParameters.ProductCategory.Substring(0, 3)))
							{
								dataContext.FindFieldDataContext("AUTO_PRODUCT_CATEGORY").SetValue(commonParameters.ProductCategory);
							}
						}
						else
						{
							dataContext.FindFieldDataContext("AUTO_PRODUCT_CATEGORY").SetValue(commonParameters.ProductCategory);
						}
						dataContext.FindFieldDataContext("AUTO_ACORD_TYPE").SetValue(commonParameters.AcordType);
					}
				}
				a = validation.getFrontOfficeBarcodeValues(ref commonParameters);
				if (a == "SUCCESS")
				{
					if (!string.IsNullOrEmpty(commonParameters.FDocClassName))
					{
						dataContext.FindFieldDataContext("AUTO_DOC_CLASS").SetValue(commonParameters.FDocClassName);
						dataContext.FindFieldDataContext("AUTO_DOC_TYPE").SetValue(commonParameters.DocType);
						dataContext.FindFieldDataContext("AUTO_BUSINESS_AREA").SetValue(commonParameters.BusinessArea);
						dataContext.FindFieldDataContext("AUTO_WORK_TYPE").SetValue(commonParameters.WorkType);
						dataContext.FindFieldDataContext("AUTO_STATUS").SetValue(commonParameters.AWDStatus);
						dataContext.FindFieldDataContext("AUTO_ACORD_TYPE").SetValue(commonParameters.AcordType);
					}
					else
					{
						dataContext.FindFieldDataContext("AUTO_ACORD_TYPE").SetValue(commonParameters.AcordType);
						dataContext.FindFieldDataContext("AUTO_DOC_TYPE").SetValue(commonParameters.DocType);
						a = validation.getFrontOfficeProdCatXrefValues(ref commonParameters);
						if (a == "SUCCESS")
						{
							dataContext.FindFieldDataContext("AUTO_DOC_CLASS").SetValue(commonParameters.FDocClassName);
						}
					}
				}
				commonParameters.Clear();
			}
			catch (Exception ex)
			{
				int errorNum = 0;
				string batchName = "";
				ModCommon.WriteError(errorNum, ex.Message, "AutoValidation", batchName);
				CommonParameters commonParameters = new CommonParameters();
				commonParameters.Clear();
			}
		}

		private void parseBarCode(IUimDataContext dataContext)
		{
			if (dataContext.FindFieldDataContext("TemplateCode").ToString() == "C14")
			{
				this.parseC14Barcode(dataContext);
			}
			else
			{
				this.parseBFOBarcode(dataContext);
			}
		}

		private void parseBFOBarcode(IUimDataContext dataContext)
		{
			try
			{
				string[] array = null;
				for (int i = 0; i < 5; i++)
				{
					string text = "BFOBarCode" + i.ToString();
					array[i] = dataContext.FindFieldDataContext(text).ToString();
				}
				int j = array.GetLowerBound(0);
				while (j <= array.GetUpperBound(0))
				{
					string text2 = array[j].Substring(0, 3);
					string text3 = text2;
					switch (text3)
					{
					case "CTR":
						dataContext.FindFieldDataContext("AUTO_CONTROL_NO").SetValue(array[j].Substring(4));
						break;
					case "DOC":
						dataContext.FindFieldDataContext("AUTO_DOC_TYPE_ORIG").SetValue(array[j].Substring(4));
						break;
					case "CMP":
						dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").SetValue(array[j].Substring(4));
						break;
					case "REQ":
						dataContext.FindFieldDataContext("AUTO_RETIREMENT_ID").SetValue(array[j].Substring(4));
						break;
					case "POL":
						dataContext.FindFieldDataContext("AUTO_POLICY_NO").SetValue(array[j].Substring(4));
						break;
					case "CDT":
						dataContext.FindFieldDataContext("AUTO_CONTROL_YEAR").SetValue(array[j].Substring(4));
						break;
					}
					IL_1A6:
					j++;
					continue;
					goto IL_1A6;
				}
			}
			catch (Exception ex)
			{
				dataContext.FindFieldDataContext("Validation").FlaggedReason = ex.Message;
				dataContext.FindFieldDataContext("Validation").SetValidationError(ex);
			}
		}

		private void parseC14Barcode(IUimDataContext dataContext)
		{
			try
			{
				string text = null;
				string[] array = new string[6];
				array = text.Split(new char[]
				{
					'%'
				});
				string text2 = array[0];
				string text3 = text2;
				if (text3 != null)
				{
					if (!(text3 == "C14A"))
					{
						if (text3 == "C14B")
						{
							dataContext.FindFieldDataContext("AUTO_FORM_TYPE").SetValue(array[0]);
							dataContext.FindFieldDataContext("AUTO_PRODUCT_CATEGORY").SetValue(array[1]);
							dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").SetValue(array[2]);
							dataContext.FindFieldDataContext("AUTO_CONTROL_YEAR").SetValue(array[3]);
							dataContext.FindFieldDataContext("AUTO_CONTROL_NO").SetValue(array[4]);
							if (array.GetUpperBound(0) >= 6)
							{
								dataContext.FindFieldDataContext("AUTO_LIFEPRO_INIDICATOR").SetValue(array[5]);
								dataContext.FindFieldDataContext("AUTO_CONTROL_NO_MASTER").SetValue(array[6]);
							}
							else
							{
								dataContext.FindFieldDataContext("AUTO_LIFEPRO_INDICATOR").SetValue(array[5]);
							}
						}
					}
					else
					{
						dataContext.FindFieldDataContext("AUTO_FORM_TYPE").SetValue(array[0]);
						dataContext.FindFieldDataContext("AUTO_PRODUCT_CATEGORY").SetValue(array[1]);
						dataContext.FindFieldDataContext("AUTO_CONTROL_NO").SetValue(array[2]);
						dataContext.FindFieldDataContext("AUTO_LIFEPRO_INIDICATOR").SetValue(array[3]);
						if (array.GetUpperBound(0) > 4)
						{
							dataContext.FindFieldDataContext("AUTO_CONTROL_NO_MASTER").SetValue(array[4]);
						}
					}
				}
			}
			catch (Exception ex)
			{
				dataContext.FindFieldDataContext("Validation").FlaggedReason = ex.Message;
				dataContext.FindFieldDataContext("Validation").SetValidationError(ex);
			}
		}

		public string Reverse(string s)
		{
			char[] array = s.ToCharArray();
			Array.Reverse(array);
			return new string(array);
		}
    }
}
