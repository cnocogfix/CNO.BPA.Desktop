using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using CNO.BPA.DataHandler;
using CNO.BPA.DataDirector;

namespace Custom.InputAccel.UimScript
{
    using Emc.InputAccel.CaptureClient;
    using Emc.InputAccel.UimScript;

    public class ScriptBPAStandardDocType : UimScriptDocument
    {
        private const string _validationRule = "One";

 
        Director _dataDirector = new Director();
        DataAccess _dataAccess = new DataAccess();
        CommonParameters _cp = new CommonParameters();
        int _docNodeId;

        string rejectReasonCode;
        string ocrField;
        bool alreadyRejected;
        string emctest;

        string _valText = null;
        IUimFieldDataContext _valField;
        //IUimFormControlContext _valField1;
        //IUimFormControlContext _controlContext;
        //IUimFieldDataContext _fieldDataContext;
        string validationValue;

        //private Dictionary<string, string> values;
        DocumentNode docNode;

        public ScriptBPAStandardDocType()
            : base()
        {


        }

        public void DocumentLoad(IUimDataContext dataContext)
        {

            dataContext.TaskFinishOnErrorNotAllowed = true;
            //validationValue = string.Empty;

            //MessageBox.Show("DocumentLoad event!!!");

            try
            {

                string nodeId = dataContext.FindFieldDataContext("NODE_ID").Text;

                if (!string.IsNullOrEmpty(nodeId))
                {
                    docNode = new DocumentNode(Convert.ToInt32(nodeId));
                    docNode.setPageCount(Convert.ToInt32(dataContext.FindFieldDataContext("PAGE_COUNT").Text));
                    docNode.setValues(new Dictionary<string, string>());
                    Share.getTree().AddLast(docNode);

                    //docNode.getValues().Add("NODE_ID", dataContext.FindFieldDataContext("NODE_ID").Text);


                    /*
                    values.Add("LASTDOC", dataContext.FindFieldDataContext("LASTDOC").Text);
                    if (values["LASTDOC"].ToUpper() == "Y")
                    {
                        Share.setLastDocNodeId(Convert.ToInt32(values["NODE_ID"]));
                    }*/

                    docNode.getValues().Add("NODE_ID", nodeId);
                    docNode.getValues().Add("ENVELOPE_NODE_ID", dataContext.FindFieldDataContext("ENVELOPE_NODE_ID").Text);
                    docNode.getValues().Add("DEFAULTREJECTPATH", dataContext.FindFieldDataContext("DEFAULTREJECTPATH").Text);
                    docNode.getValues().Add("TRACK_USER", dataContext.FindFieldDataContext("TRACK_USER").Text);
                    docNode.getValues().Add("TRACK_PERFORMANCE", dataContext.FindFieldDataContext("TRACK_PERFORMANCE").Text);
                    docNode.getValues().Add("BATCH_NO", dataContext.FindFieldDataContext("BATCH_NO").Text);
                    docNode.getValues().Add("SITE_ID", dataContext.FindFieldDataContext("SITE_ID").Text);
                    docNode.getValues().Add("WORK_CATEGORY", dataContext.FindFieldDataContext("WORK_CATEGORY").Text);
                    docNode.getValues().Add("SCAN_COMPLETE", dataContext.FindFieldDataContext("SCAN_COMPLETE").Text);
                    docNode.getValues().Add("SCANNER_ID", dataContext.FindFieldDataContext("SCANNER_ID").Text);
                    docNode.getValues().Add("BOX_NO", dataContext.FindFieldDataContext("BOX_NO").Text);
                    docNode.getValues().Add("RECEIVED_DATE", dataContext.FindFieldDataContext("RECEIVED_DATE").Text);
                    docNode.getValues().Add("CRD_RECEIVED_DATE", dataContext.FindFieldDataContext("CRD_RECEIVED_DATE").Text);
                    docNode.getValues().Add("BATCH_DEPARTMENT", dataContext.FindFieldDataContext("BATCH_DEPARTMENT").Text);
                    //Common document level data
                    docNode.getValues().Add("D_ROUTE_CODE", dataContext.FindFieldDataContext("D_ROUTE_CODE").Text);
                    docNode.getValues().Add("D_AWD_SOURCE_TYPE", dataContext.FindFieldDataContext("D_AWD_SOURCE_TYPE").Text);
                    docNode.getValues().Add("D_AWD_STATUS", dataContext.FindFieldDataContext("D_AWD_STATUS").Text);
                    docNode.getValues().Add("D_COMPANY_CODE", dataContext.FindFieldDataContext("D_COMPANY_CODE").Text);
                    docNode.getValues().Add("D_FAX_ID", dataContext.FindFieldDataContext("D_FAX_ID").Text);
                    docNode.getValues().Add("D_FAX_ACCOUNT", dataContext.FindFieldDataContext("D_FAX_ACCOUNT").Text);
                    docNode.getValues().Add("D_FAX_RECIPIENT", dataContext.FindFieldDataContext("D_FAX_RECIPIENT").Text);
                    docNode.getValues().Add("D_FAX_SENDER", dataContext.FindFieldDataContext("D_FAX_SENDER").Text);
                    docNode.getValues().Add("D_FAX_SERVER", dataContext.FindFieldDataContext("D_FAX_SERVER").Text);
                    docNode.getValues().Add("D_TYPE_OF_BILL", dataContext.FindFieldDataContext("D_TYPE_OF_BILL").Text);
                    docNode.getValues().Add("D_HCFA_COUNT", dataContext.FindFieldDataContext("D_HCFA_COUNT").Text);
                    docNode.getValues().Add("D_UB92_COUNT", dataContext.FindFieldDataContext("D_UB92_COUNT").Text);
                    docNode.getValues().Add("D_PHEOMB_COUNT", dataContext.FindFieldDataContext("D_PHEOMB_COUNT").Text);
                    docNode.getValues().Add("D_EOMB_COUNT", dataContext.FindFieldDataContext("D_EOMB_COUNT").Text);
                    docNode.getValues().Add("D_ATTACHMENT_COUNT", dataContext.FindFieldDataContext("D_ATTACHMENT_COUNT").Text);
                    docNode.getValues().Add("D_WORK_CATEGORY", dataContext.FindFieldDataContext("D_WORK_CATEGORY").Text);
                    docNode.getValues().Add("D_BUSINESS_AREA", dataContext.FindFieldDataContext("D_BUSINESS_AREA").Text);
                    docNode.getValues().Add("D_WORK_TYPE", dataContext.FindFieldDataContext("D_WORK_TYPE").Text);
                    docNode.getValues().Add("D_SITE_ID", dataContext.FindFieldDataContext("D_SITE_ID").Text);
                    docNode.getValues().Add("D_SYSTEM_ID", dataContext.FindFieldDataContext("D_SYSTEM_ID").Text);
                    docNode.getValues().Add("D_RECEIVED_DATE", dataContext.FindFieldDataContext("D_RECEIVED_DATE").Text);
                    
                    docNode.getValues().Add("D_REJECTIMAGEPATH", dataContext.FindFieldDataContext("D_REJECTIMAGEPATH").Text);
                    docNode.getValues().Add("D_VALIDATION_AUDIT_START", dataContext.FindFieldDataContext("D_VALIDATION_AUDIT_START").Text);
                    docNode.getValues().Add("D_BATCH_ITEM_ID", dataContext.FindFieldDataContext("D_BATCH_ITEM_ID").Text);
                    docNode.getValues().Add("D_DD_ITEM_SEQ", dataContext.FindFieldDataContext("D_DD_ITEM_SEQ").Text);
                    //Additional document level data for clm_form
                    docNode.getValues().Add("D_AGENT_NO", dataContext.FindFieldDataContext("D_AGENT_NO").Text);
                    docNode.getValues().Add("D_APPLICATION_NO", dataContext.FindFieldDataContext("D_APPLICATION_NO").Text);
                    docNode.getValues().Add("D_BLANK_IMAGE_NO", dataContext.FindFieldDataContext("D_BLANK_IMAGE_NO").Text);
                    docNode.getValues().Add("D_CERTIFICATE_NO", dataContext.FindFieldDataContext("D_CERTIFICATE_NO").Text);
                    docNode.getValues().Add("D_CHECK_AMOUNT", dataContext.FindFieldDataContext("D_CHECK_AMOUNT").Text);
                    docNode.getValues().Add("D_CHECK_ISSUE_DATE", dataContext.FindFieldDataContext("D_CHECK_ISSUE_DATE").Text);
                    docNode.getValues().Add("D_CHECK_NUMBER", dataContext.FindFieldDataContext("D_CHECK_NUMBER").Text);
                    docNode.getValues().Add("D_CONFIRMATION_NO", dataContext.FindFieldDataContext("D_CONFIRMATION_NO").Text);
                    docNode.getValues().Add("D_CONTROL_NO", dataContext.FindFieldDataContext("D_CONTROL_NO").Text);
                    docNode.getValues().Add("D_DOC_TYPE", dataContext.FindFieldDataContext("D_DOC_TYPE").Text);
                    docNode.getValues().Add("D_DOC_TYPE_ORIG", dataContext.FindFieldDataContext("D_DOC_TYPE_ORIG").Text);
                    docNode.getValues().Add("D_DOC_VALUES_SET", dataContext.FindFieldDataContext("D_DOC_VALUES_SET").Text);
                    docNode.getValues().Add("D_EMAIL_DATE", dataContext.FindFieldDataContext("D_EMAIL_DATE").Text);
                    docNode.getValues().Add("D_EMAIL_SENDER", dataContext.FindFieldDataContext("D_EMAIL_SENDER").Text);
                    docNode.getValues().Add("D_EMAIL_TO", dataContext.FindFieldDataContext("D_EMAIL_TO").Text);
                    docNode.getValues().Add("D_F_DOCCLASSNAME", dataContext.FindFieldDataContext("D_F_DOCCLASSNAME").Text);
                    docNode.getValues().Add("D_FNP8_DOCCLASSNAME", dataContext.FindFieldDataContext("D_FNP8_DOCCLASSNAME").Text);
                    docNode.getValues().Add("D_GROUP_NO", dataContext.FindFieldDataContext("D_GROUP_NO").Text);
                    docNode.getValues().Add("D_INSURED_NAME", dataContext.FindFieldDataContext("D_INSURED_NAME").Text);
                    docNode.getValues().Add("D_ISSUE_STATE", dataContext.FindFieldDataContext("D_ISSUE_STATE").Text);
                    docNode.getValues().Add("D_LOB", dataContext.FindFieldDataContext("D_LOB").Text);
                    docNode.getValues().Add("D_OFFICE_NO", dataContext.FindFieldDataContext("D_OFFICE_NO").Text);
                    docNode.getValues().Add("D_PATIENT_BIRTHDATE", dataContext.FindFieldDataContext("D_PATIENT_BIRTHDATE").Text);
                    docNode.getValues().Add("D_PHONE", dataContext.FindFieldDataContext("D_PHONE").Text);
                    docNode.getValues().Add("D_PLAN_CODE", dataContext.FindFieldDataContext("D_PLAN_CODE").Text);
                    docNode.getValues().Add("D_POLICY_NO", dataContext.FindFieldDataContext("D_POLICY_NO").Text);
                    docNode.getValues().Add("D_PROVIDER_TAX_ID", dataContext.FindFieldDataContext("D_PROVIDER_TAX_ID").Text);
                    docNode.getValues().Add("D_SORT_CODE", dataContext.FindFieldDataContext("D_SORT_CODE").Text);
                    docNode.getValues().Add("D_SOURCE_ID", dataContext.FindFieldDataContext("D_SOURCE_ID").Text);
                    docNode.getValues().Add("D_SSN", dataContext.FindFieldDataContext("D_SSN").Text);
                    docNode.getValues().Add("D_STATE", dataContext.FindFieldDataContext("D_STATE").Text);
                    docNode.getValues().Add("D_ZIP", dataContext.FindFieldDataContext("D_ZIP").Text);
                    //Additional document level data for CPL NB form
                    if (!docNode.getValues().ContainsKey("D_CORRESPONDENCE_INDICATOR")) docNode.getValues().Add("D_CORRESPONDENCE_INDICATOR", dataContext.FindFieldDataContext("D_CORRESPONDENCE_INDICATOR").Text);
                    if (!docNode.getValues().ContainsKey("D_COURTEOUS")) docNode.getValues().Add("D_COURTEOUS", dataContext.FindFieldDataContext("D_COURTEOUS").Text);
                    if (!docNode.getValues().ContainsKey("D_DIRECTORY_NO")) docNode.getValues().Add("D_DIRECTORY_NO", dataContext.FindFieldDataContext("D_DIRECTORY_NO").Text);
                    if (!docNode.getValues().ContainsKey("D_KNOWLEDGEABLE")) docNode.getValues().Add("D_KNOWLEDGEABLE", dataContext.FindFieldDataContext("D_KNOWLEDGEABLE").Text);
                    if (!docNode.getValues().ContainsKey("D_LIKELY")) docNode.getValues().Add("D_LIKELY", dataContext.FindFieldDataContext("D_LIKELY").Text);
                    if (!docNode.getValues().ContainsKey("D_MEMBER_NO")) docNode.getValues().Add("D_MEMBER_NO", dataContext.FindFieldDataContext("D_MEMBER_NO").Text);
                    if (!docNode.getValues().ContainsKey("D_PRODUCT_TYPE")) docNode.getValues().Add("D_PRODUCT_TYPE", dataContext.FindFieldDataContext("D_PRODUCT_TYPE").Text);
                    if (!docNode.getValues().ContainsKey("D_PROFESSIONAL")) docNode.getValues().Add("D_PROFESSIONAL", dataContext.FindFieldDataContext("D_PROFESSIONAL").Text);
                    if (!docNode.getValues().ContainsKey("D_QUICK")) docNode.getValues().Add("D_QUICK", dataContext.FindFieldDataContext("D_QUICK").Text);
                    if (!docNode.getValues().ContainsKey("D_REPLACEMENT_INDICATOR")) docNode.getValues().Add("D_REPLACEMENT_INDICATOR", dataContext.FindFieldDataContext("D_REPLACEMENT_INDICATOR").Text);
                    if (!docNode.getValues().ContainsKey("D_THOROUGH")) docNode.getValues().Add("D_THOROUGH", dataContext.FindFieldDataContext("D_THOROUGH").Text);
                    //Additional document level data for 4086 form
                    if (!docNode.getValues().ContainsKey("D_ACCOUNT_NUMBER")) docNode.getValues().Add("D_ACCOUNT_NUMBER", dataContext.FindFieldDataContext("D_ACCOUNT_NUMBER").Text);
                    if (!docNode.getValues().ContainsKey("D_BORROWER_NAME")) docNode.getValues().Add("D_BORROWER_NAME", dataContext.FindFieldDataContext("D_BORROWER_NAME").Text);
                    if (!docNode.getValues().ContainsKey("D_DESCRIPTION")) docNode.getValues().Add("D_DESCRIPTION", dataContext.FindFieldDataContext("D_DESCRIPTION").Text);
                    if (!docNode.getValues().ContainsKey("D_DESCRIPTION2")) docNode.getValues().Add("D_DESCRIPTION2", dataContext.FindFieldDataContext("D_DESCRIPTION2").Text);
                    if (!docNode.getValues().ContainsKey("D_DOCUMENT_DATE")) docNode.getValues().Add("D_DOCUMENT_DATE", dataContext.FindFieldDataContext("D_DOCUMENT_DATE").Text);
                    if (!docNode.getValues().ContainsKey("D_FULL_NAME")) docNode.getValues().Add("D_FULL_NAME", dataContext.FindFieldDataContext("D_FULL_NAME").Text);
                    if (!docNode.getValues().ContainsKey("D_PROPERTY_ADDRESS")) docNode.getValues().Add("D_PROPERTY_ADDRESS", dataContext.FindFieldDataContext("D_PROPERTY_ADDRESS").Text);
                    if (!docNode.getValues().ContainsKey("D_PROPERTY_CITY")) docNode.getValues().Add("D_PROPERTY_CITY", dataContext.FindFieldDataContext("D_PROPERTY_CITY").Text);
                    if (!docNode.getValues().ContainsKey("D_PROPERTY_NAME")) docNode.getValues().Add("D_PROPERTY_NAME", dataContext.FindFieldDataContext("D_PROPERTY_NAME").Text);
                    if (!docNode.getValues().ContainsKey("D_PROPERTY_STATE")) docNode.getValues().Add("D_PROPERTY_STATE", dataContext.FindFieldDataContext("D_PROPERTY_STATE").Text);
                    if (!docNode.getValues().ContainsKey("D_VENDOR_NAME")) docNode.getValues().Add("D_VENDOR_NAME", dataContext.FindFieldDataContext("D_VENDOR_NAME").Text);
					//Additional doc level data for cpl phs form
                    if (!docNode.getValues().ContainsKey("D_APPLICATION_NO")) docNode.getValues().Add("D_APPLICATION_NO", dataContext.FindFieldDataContext("D_APPLICATION_NO").Text);
                    if (!docNode.getValues().ContainsKey("D_COMPANY_CODE")) docNode.getValues().Add("D_COMPANY_CODE", dataContext.FindFieldDataContext("D_COMPANY_CODE").Text);
                    if (!docNode.getValues().ContainsKey("D_DOC_TYPE")) docNode.getValues().Add("D_DOC_TYPE", dataContext.FindFieldDataContext("D_DOC_TYPE").Text);
                    if (!docNode.getValues().ContainsKey("D_FNP8_DOCCLASSNAME")) docNode.getValues().Add("D_FNP8_DOCCLASSNAME", dataContext.FindFieldDataContext("D_FNP8_DOCCLASSNAME").Text);
                    if (!docNode.getValues().ContainsKey("D_LOB")) docNode.getValues().Add("D_LOB", dataContext.FindFieldDataContext("D_LOB").Text);
                    if (!docNode.getValues().ContainsKey("D_MOLIDOC_TYPE")) docNode.getValues().Add("D_MOLIDOC_TYPE", dataContext.FindFieldDataContext("D_MOLIDOC_TYPE").Text);
                    if (!docNode.getValues().ContainsKey("D_OPTOUT_TYPE")) docNode.getValues().Add("D_OPTOUT_TYPE", dataContext.FindFieldDataContext("D_OPTOUT_TYPE").Text);
                    if (!docNode.getValues().ContainsKey("D_POLICY_NO")) docNode.getValues().Add("D_POLICY_NO", dataContext.FindFieldDataContext("D_POLICY_NO").Text);
                    if (!docNode.getValues().ContainsKey("D_SOURCE_ID")) docNode.getValues().Add("D_SOURCE_ID", dataContext.FindFieldDataContext("D_SOURCE_ID").Text);
                    //Additional doc level data for privacy
                    if (!docNode.getValues().ContainsKey("D_CITY")) docNode.getValues().Add("D_CITY", dataContext.FindFieldDataContext("D_CITY").Text);
                    if (!docNode.getValues().ContainsKey("D_COMPANY_CODE")) docNode.getValues().Add("D_COMPANY_CODE", dataContext.FindFieldDataContext("D_COMPANY_CODE").Text);
                    if (!docNode.getValues().ContainsKey("D_DNS")) docNode.getValues().Add("D_DNS", dataContext.FindFieldDataContext("D_DNS").Text);
                    if (!docNode.getValues().ContainsKey("D_DNS_3RD")) docNode.getValues().Add("D_DNS_3RD", dataContext.FindFieldDataContext("D_DNS_3RD").Text);
                    if (!docNode.getValues().ContainsKey("D_DOC_TYPE")) docNode.getValues().Add("D_DOC_TYPE", dataContext.FindFieldDataContext("D_DOC_TYPE").Text);
                    if (!docNode.getValues().ContainsKey("D_F_DOCCLASSNAME")) docNode.getValues().Add("D_F_DOCCLASSNAME", dataContext.FindFieldDataContext("D_F_DOCCLASSNAME").Text);
                    if (!docNode.getValues().ContainsKey("D_FAX_ID")) docNode.getValues().Add("D_FAX_ID", dataContext.FindFieldDataContext("D_FAX_ID").Text);
                    if (!docNode.getValues().ContainsKey("D_FAX_RECIPIENT")) docNode.getValues().Add("D_FAX_RECIPIENT", dataContext.FindFieldDataContext("D_FAX_RECIPIENT").Text);
                    if (!docNode.getValues().ContainsKey("D_FAX_SENDER")) docNode.getValues().Add("D_FAX_SENDER", dataContext.FindFieldDataContext("D_FAX_SENDER").Text);
                    if (!docNode.getValues().ContainsKey("D_FIRST_NAME")) docNode.getValues().Add("D_FIRST_NAME", dataContext.FindFieldDataContext("D_FIRST_NAME").Text);
                    if (!docNode.getValues().ContainsKey("D_FNP8_DOCCLASSNAME")) docNode.getValues().Add("D_FNP8_DOCCLASSNAME", dataContext.FindFieldDataContext("D_FNP8_DOCCLASSNAME").Text);
                    if (!docNode.getValues().ContainsKey("D_INSURED_NAME")) docNode.getValues().Add("D_INSURED_NAME", dataContext.FindFieldDataContext("D_INSURED_NAME").Text);
                    if (!docNode.getValues().ContainsKey("D_ISSUE_STATE")) docNode.getValues().Add("D_ISSUE_STATE", dataContext.FindFieldDataContext("D_ISSUE_STATE").Text);
                    if (!docNode.getValues().ContainsKey("D_LAST_NAME")) docNode.getValues().Add("D_LAST_NAME", dataContext.FindFieldDataContext("D_LAST_NAME").Text);
                    if (!docNode.getValues().ContainsKey("D_LOB")) docNode.getValues().Add("D_LOB", dataContext.FindFieldDataContext("D_LOB").Text);
                    if (!docNode.getValues().ContainsKey("D_MIDDLE_NAME")) docNode.getValues().Add("D_MIDDLE_NAME", dataContext.FindFieldDataContext("D_MIDDLE_NAME").Text);
                    if (!docNode.getValues().ContainsKey("D_PLAN_CODE")) docNode.getValues().Add("D_PLAN_CODE", dataContext.FindFieldDataContext("D_PLAN_CODE").Text);
                    if (!docNode.getValues().ContainsKey("D_SORT_CODE")) docNode.getValues().Add("D_SORT_CODE", dataContext.FindFieldDataContext("D_SORT_CODE").Text);
                    if (!docNode.getValues().ContainsKey("D_SSN")) docNode.getValues().Add("D_SSN", dataContext.FindFieldDataContext("D_SSN").Text);
                    if (!docNode.getValues().ContainsKey("D_STATE")) docNode.getValues().Add("D_STATE", dataContext.FindFieldDataContext("D_STATE").Text);
                    if (!docNode.getValues().ContainsKey("D_STREET_ADDRESS")) docNode.getValues().Add("D_STREET_ADDRESS", dataContext.FindFieldDataContext("D_STREET_ADDRESS").Text);
                    if (!docNode.getValues().ContainsKey("D_STREET_ADDRESS_2")) docNode.getValues().Add("D_STREET_ADDRESS_2", dataContext.FindFieldDataContext("D_STREET_ADDRESS_2").Text);
                    if (!docNode.getValues().ContainsKey("D_UNIQUE_ID")) docNode.getValues().Add("D_UNIQUE_ID", dataContext.FindFieldDataContext("D_UNIQUE_ID").Text);
                    if (!docNode.getValues().ContainsKey("D_ZIP")) docNode.getValues().Add("D_ZIP", dataContext.FindFieldDataContext("D_ZIP").Text);

                    //Additional doc data for check form
                    if (!docNode.getValues().ContainsKey("D_CHECK_AMOUNT")) docNode.getValues().Add("D_CHECK_AMOUNT", dataContext.FindFieldDataContext("D_CHECK_AMOUNT").Text);
                    if (!docNode.getValues().ContainsKey("D_CHECK_ISSUE_DATE")) docNode.getValues().Add("D_CHECK_ISSUE_DATE", dataContext.FindFieldDataContext("D_CHECK_ISSUE_DATE").Text);
                    if (!docNode.getValues().ContainsKey("D_CHECK_NUMBER")) docNode.getValues().Add("D_CHECK_NUMBER", dataContext.FindFieldDataContext("D_CHECK_NUMBER").Text);
                    if (!docNode.getValues().ContainsKey("D_DOC_TYPE")) docNode.getValues().Add("D_DOC_TYPE", dataContext.FindFieldDataContext("D_DOC_TYPE").Text);
                    if (!docNode.getValues().ContainsKey("D_FNP8_DOCCLASSNAME")) docNode.getValues().Add("D_FNP8_DOCCLASSNAME", dataContext.FindFieldDataContext("D_FNP8_DOCCLASSNAME").Text);
                    if (!docNode.getValues().ContainsKey("D_JV_COMPANY_CODE")) docNode.getValues().Add("D_JV_COMPANY_CODE", dataContext.FindFieldDataContext("D_JV_COMPANY_CODE").Text);
                    if (!docNode.getValues().ContainsKey("D_POLICY_NO")) docNode.getValues().Add("D_POLICY_NO", dataContext.FindFieldDataContext("D_POLICY_NO").Text);
                    if (!docNode.getValues().ContainsKey("D_SOURCE_CODE")) docNode.getValues().Add("D_SOURCE_CODE", dataContext.FindFieldDataContext("D_SOURCE_CODE").Text);
                    if (!docNode.getValues().ContainsKey("D_VENDOR_NUMBER")) docNode.getValues().Add("D_VENDOR_NUMBER", dataContext.FindFieldDataContext("D_VENDOR_NUMBER").Text);


                }
                else //NodeId doesn't exist when new document added
                {
                    //docNode = new DocumentNode(); //21.4 Captiva Upgrade
                    docNode = Share.getNewNode(); //21.4 Captiva Upgrade
                    docNode.setValues(new Dictionary<string, string>(Share.getLastSelectedNode().getValues()));
                   // Share.setNewNode(docNode);
                    //This node is inserted into the tree in ScriptMain
                    //This node's nodeid is also set in ScriptMain
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
                


                string rejected = dataContext.FindFieldDataContext("Rejected").Text;
                if (rejected == "1")
                {

                    CommonParameters CP = new CommonParameters();
                    string BatchNo = docNode.getValues()["BATCH_NO"];
                    CP.BatchNo = BatchNo.Trim();
                    CP.CurrentNodeID = docNode.getValues()["NODE_ID"];
                    CP.ProcessStep = "ManualIndex";
                    CP.ReasonCode = rejectReasonCode;
                    DataAccess dataAccess = new DataAccess();
                    dataAccess.createBatchItemReject(ref CP);

                }
                else
                {
                    if (alreadyRejected)
                    {
                        DataAccess dataAccess = new DataAccess();
                        try
                        {
                            dataAccess.updateReasonCode(docNode.getValues()["BATCH_NO"], docNode.getValues()["NODE_ID"], "Unrejected");
                        }
                        catch (Exception ex)
                        {
                            //updateReasonCode failed. There wasn't a rejectItem with such batch_no
                        }
                    }
                }




                //TODO: Default(set to 0) Replicate value on the last document no matter what..\


                dataContext.FindFieldDataContext("D_VALIDATION_AUDIT_START").SetValue(docNode.getValues()["D_VALIDATION_AUDIT_START"]);
                dataContext.FindFieldDataContext("D_BATCH_ITEM_ID").SetValue(docNode.getValues()["D_BATCH_ITEM_ID"]);
                dataContext.FindFieldDataContext("D_DD_ITEM_SEQ").SetValue(docNode.getValues()["D_DD_ITEM_SEQ"]);
                dataContext.FindFieldDataContext("D_REJECTIMAGEPATH").SetValue(docNode.getValues()["D_REJECTIMAGEPATH"]);
                if (Share.getTree() != null && Share.getTree().Last.Value == docNode) {
                    dataContext.FindFieldDataContext("Replicate").SetValue("0");
                }

                		        	//stnd_mdf_d.WriteString("D_VALIDATION_AUDIT_START", data.FindFieldDataContext("D_VALIDATION_AUDIT_START").Text);
		        	//stnd_mdf_d.WriteString("D_BATCH_ITEM_ID", data.FindFieldDataContext("D_BATCH_ITEM_ID").Text);
		        	//stnd_mdf_d.WriteString("D_DD_ITEM_SEQ", data.FindFieldDataContext("D_DD_ITEM_SEQ").Text);
		        	//D_VALIDATION
		        	//Reject info from DD


                if (Share.getTree() != null && Share.getTree().Count > 0 && Share.getTree().Last.Value == docNode)
                {
                    Share.getTree().Clear();
                }

                _dataDirector = null;
                _dataAccess.Disconnect();
                _dataAccess.Dispose();
                _cp.Clear();
                docNode = null;
            }
            catch (Exception ex)
            {
                _dataDirector = null;
                _dataAccess.Disconnect();
                _dataAccess.Dispose();
                _cp.Clear();
                docNode = null;
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


            if (form.FindControl("Rejected").Text == "true")
            {
                alreadyRejected = true;
            }
            else
            {
                alreadyRejected = false;
            }


            //Hide replicate field for last document
            //form.
            if (docNode.getNodeId().ToString() == Convert.ToString(Share.getTree().Last.Value.getNodeId()))
            {
                form.FindControl("Replicate").SetText("");
                form.FindControl("Replicate").EnableControl(false);
            }
            else //21.4 Captiva Upgrade- Setting replicate ON after splitting last node
            {
                form.FindControl("Replicate").SetText("");
                form.FindControl("Replicate").EnableControl(true);
            }

            

            string department = docNode.getValues()["BATCH_DEPARTMENT"];

            if (department.Contains("BLC_FORM") || department.Contains("CIC_FORM") ||
                department.Contains("BLC_ARCHIVE_POL") || department.Contains("CIC_ARCHIVE_POL") ||
                department.Contains("BLC_BE") || department.Contains("CIC_BE") ||
                department.Contains("ENT_BE_HEALTH_CLAIM") || department.Contains("ENT_CRD_CPL_CORR") ||
                department.Contains("BLC_NON") || department.Contains("CIC_NON") ||
                department.Contains("CIC_CASH") || department.Contains("CPL_CHECK") ||
                department.Contains("BLC_CHECKS") || department.Contains("BLC_CLM_CERTIFIED") ||
                department.Contains("BLC_CLM_CERTIFIED") || department.Contains("BLC_LA_NON_FORM") ||
                department.Contains("BLC_LTC") || department.Contains("BLC_MS") ||
                department.Contains("BLC_UNREADABLE") || department.Contains("CIC_CHECKS") ||
                department.Contains("CIC_CLM") || department.Contains("CIC_EXPRESS") ||
                department.Contains("CIC_LA") || department.Contains("CIC_MS") ||
                department.Contains("CIC_SD") || department.Contains("CIC_UNREADABLE") ||
                department.Contains("CPL_NB_CLAIMS_BACKSCAN") || department.Contains("CPL_NB_REINSTATE") ||
                department.Contains("CPL_NB_U30_BACKSCAN") || department.Contains("CPL_PHS_BACKSCAN") ||
                department.Contains("CPL_PHS_FORM") || department.Contains("CPL_PHS_PRIVACY") ||
                department.Contains("POSTALRETURNS") || department.Contains("CPL_PHS_WHITEMAIL") ||
                department.Contains("CPL_PHS_LET") || department.Contains("ENT_CSC_WILTRE") ||
                department.Contains("BLC_COMP") || department.Contains("BLC_FIN") ||
                department.Contains("BLC_PHS") ||
                department.Contains("CIC_COMP") || department.Contains("CIC_PHS") ||
                department.Contains("BLC_CLM_HEALTH_EMAIL") || department.Contains("CIC_LTCG_IMPORT") ||
                department.Contains("CPL_PHS_FAX"))
            {
                form.FindControl("OCRFieldLabel").SetText("Policy No"); //CP.PolicyNo
                ocrField = "PolicyNo";

            } else if (department.Contains("BLC_WORKSITE_IF")  || department.Contains("CIC_NB_ANNUITY")  ||
                department.Contains("CIC_NB_LIFE") || department.Contains("CIC_WORKSITE_IF") ||
                department.Contains("BLC_NBO")) 
            {
                form.FindControl("OCRFieldLabel").SetText("Policy/Reject No"); //CP.PolicyNo
                ocrField = "PolicyNo";

            } else if (department.Contains("ENT_ARCHIVE") || department.Contains("ENT_BE_AGENT") ||
                department.Contains("CIC_NB_HEALTH") || department.Contains("CIC_UW") ||
                department.Contains("CIC_WORKSITE_EMAIL") || department.Contains("CIC_WORKSITE_NB"))
                
            {
                form.FindControl("OCRFieldLabel").SetText("Agent No"); //CP.AgentNo
                ocrField = "AgentNo";

            } else if (department.Contains("CPL_NB_CASH") || department.Contains("CPL_NB_NON_CASH") ||
                department.Contains("CPL_SURVEY") || department.Contains("CPL_PHS_SWEEPS") ||
                department.Contains("CPL_PHS_CULLS")) 
            {
                form.FindControl("OCRFieldLabel").SetText("Member Number"); //CP.ApplicationNo
                ocrField = "ApplicationNo";

            } else if (department.Contains("CPL_Lead_Gen")) 
            {
                form.FindControl("OCRFieldLabel").SetText("Phone Number"); //CP.Phone
                ocrField = "Phone";

            } else if (department.Contains("ENT_PRIVACY_CA"))
            {
                form.FindControl("OCRFieldLabel").SetText("Unique ID"); //CP.PrivMasterID
                ocrField = "PrivMasterID";
            }
            else if (department.Contains("ENT_BE_HEALTH_PROV"))
            {
                form.FindControl("OCRFieldLabel").SetText("Provider TIN/EIN"); //CP.ProviderTIN
                ocrField = "ProviderTIN";
            }
            else if (department.Contains("ENT_RETAINED_ASSETS"))
            {
                form.FindControl("OCRFieldLabel").SetText("Account Number"); //CP.AccountNumber
                ocrField = "AccountNumber";
            }
            else
            {
                form.FindControl("OCRFieldLabel").SetText("");
                form.FindControl("OCRField").ShowControl(false);
                ocrField = "";
            }

        }

		public void ExitControlOCRField(IUimFormControlContext controlContext)
		{
			if (ocrField == "PolicyNo")
			{
                _cp.PolicyNo = controlContext.FindFieldData("OCRField").Text;
			}
			else if (ocrField == "AgentNo")
			{
				_cp.AgentNo = controlContext.FindFieldData("OCRField").Text;
			}
			else if (ocrField == "PrivMasterID")
			{
				_cp.PrivMasterID = controlContext.FindFieldData("OCRField").Text;
			}
			else if (ocrField == "ApplicationNo")
			{
				_cp.ApplicationNo = controlContext.FindFieldData("OCRField").Text;
			}
			else if (ocrField == "Phone")
			{
				_cp.Phone = controlContext.FindFieldData("OCRField").Text;
			}
			else if (ocrField == "PrivMasterID")
			{
				_cp.PrivMasterID = controlContext.FindFieldData("OCRField").Text;
			}
			else if (ocrField == "ProviderTIN")
			{
				_cp.ProviderTIN = controlContext.FindFieldData("OCRField").Text;
			}
			else if (ocrField == "AccountNumber")
			{
				_cp.AccountNumber = controlContext.FindFieldData("OCRField").Text;
			}
		}

		public void TextChangedOCRField(IUimFormControlContext controlContext)
		{
			if (ocrField == "PolicyNo")
			{
				_cp.PolicyNo = controlContext.FindFieldData("OCRField").Text;
			}
			else if (ocrField == "AgentNo")
			{
				_cp.AgentNo = controlContext.FindFieldData("OCRField").Text;
			}
			else if (ocrField == "PrivMasterID")
			{
				_cp.PrivMasterID = controlContext.FindFieldData("OCRField").Text;
			}
			else if (ocrField == "ApplicationNo")
			{
				_cp.ApplicationNo = controlContext.FindFieldData("OCRField").Text;
			}
			else if (ocrField == "Phone")
			{
				_cp.Phone = controlContext.FindFieldData("OCRField").Text;
			}
			else if (ocrField == "PrivMasterID")
			{
				_cp.PrivMasterID = controlContext.FindFieldData("OCRField").Text;
			}
			else if (ocrField == "ProviderTIN")
			{
				_cp.ProviderTIN = controlContext.FindFieldData("OCRField").Text;
			}
			else if (ocrField == "AccountNumber")
			{
				_cp.AccountNumber = controlContext.FindFieldData("OCRField").Text;
			}
		}

        public void SelectionChangeRejected(IUimFormControlContext controlContext)
        {

            try
            {
                //check if already validated if yes, prevent the operator to reject.
                if (!string.IsNullOrEmpty(controlContext.FindFieldData("Validation").Text.Trim()))
                {
                    if (controlContext.Text == "true")
                    {
                        controlContext.SetText("0");
                    }
                    else
                    {
                        MessageBox.Show("Document is already validated. Document can not be rejected!");
                    }
                    return;
                }

                if (controlContext.ChoiceValue == "1")
                {

                    //Disable DD and validation field
                    controlContext.ParentForm.FindControl("Validation").EnableControl(false);
                    controlContext.ParentForm.FindControl("DataDirector").EnableControl(false);

                    IndexRejectForm rejectForm = new IndexRejectForm();
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

                    DataTable issuesTable = dataAccess.getRejectOptions("INDEX_ISSUES").Tables[0]; //21.4 Captiva Upgrade
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

                        controlContext.FindFieldData("RejectOption").SetValue(rejectResult);


                        /*
                        string defaultPath = docNode.getValues()["DEFAULTREJECTPATH"]; //nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/DEFAULTREJECTPATH", "");
                        string rejectPath;
                        if (defaultPath.EndsWith(@"\"))
                        {
                            rejectPath = defaultPath + rejectResult;
                        }
                        else
                        {
                            rejectPath = defaultPath + @"\" + rejectResult;
                        }

                        docNode.getValues()["D_REJECTIMAGEPATH"] = rejectPath;
                         */


                    }
                    else
                    {
                        controlContext.SetText("0");
						controlContext.FindFieldData("RejectOption").SetValue((object)"");
						controlContext.ParentForm.FindControl("Validation").EnableControl(true);
						controlContext.ParentForm.FindControl("DataDirector").EnableControl(true);
						rejectReasonCode = "";
                    }
                }
                else //Unmarking reject button
                {
                    controlContext.ParentForm.FindControl("Validation").EnableControl(true);
                    controlContext.ParentForm.FindControl("DataDirector").EnableControl(true);
                    rejectReasonCode = "";
                }
            }
            catch (Exception ex)
            {
                controlContext.FindFieldData("RejectOption").SetValue("");
                controlContext.SetText("0");
                rejectReasonCode = "";
            }

        }

        public void SelectionChangeBatchIssues(IUimFormControlContext controlContext)
        {
            if (controlContext.ChoiceValue == "1")
            {

                BatchIssues frmBatchIssues = new BatchIssues(docNode.getValues()["BATCH_NO"]);
                DialogResult dRes = frmBatchIssues.ShowDialog();

                if (dRes != DialogResult.OK)
                {
                    controlContext.SetText("0");
                }
            }

        }

        public void ButtonClickDataDirector(IUimFormControlContext controlContext)
        {
            try
            {
                
                //If Rejected don't run DD
                if (controlContext.FindFieldData("Rejected").Text == "1")
                {
                    return;
                }

                _docNodeId = docNode.getNodeId();

                //_valText = "";
                
                _valField = controlContext.FindFieldData("Validation");
                //_valField1 = controlContext.ParentForm.FindControl("Validation");
                //_controlContext = controlContext;
                //_fieldDataContext = controlContext.FieldDataContext;

                if (Share.getDD_Running() == true)
                {
                    MessageBox.Show("Data Director is already running. Please complete current document before attempting to launch Data Director again.", "Data Director already running.");
                    return;
                }

                Share.setDD_Running(true); 

                //set start of validation if needed for doc
                string valStart = string.Empty;
                valStart = docNode.getValues()["D_VALIDATION_AUDIT_START"];
                if (valStart.Trim().Length == 0)
                {
                    //set val start to now
                    valStart = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                    docNode.getValues()["D_VALIDATION_AUDIT_START"] = valStart;
                    //nodeValueProvider.SetString(nodePath + "/$instance=Standard_MDF/D_VALIDATION_AUDIT_START", valStart);

                }


                _dataDirector.Complete += new CNO.BPA.DataDirector.CompleteEventHandler(Director_Complete);
                //_dummyDD.Complete += new DummyDD.CompleteEventHandler(dummyEvent_Complete);
                //dummyDD2.Complete += new DummyDD.CompleteEventHandler(dummyEvent_Complete);
                
                
                //Director_Complete(0, "test");

                //_valField.SetValue(emctest);

                //MessageBox.Show("DD02");
                _cp = new CommonParameters();

                //Get current document using nodeID
                LinkedListNode<DocumentNode> current = Share.getTree().First;
                while (current != null)
                {
                    if (current.Value.getNodeId() == docNode.getNodeId())
                    {
                        break;
                    }
                    current = current.Next;
                }

                
                string previousNodeId = "";
                int previousDD_Item_Seq = 0;
                if (current.Previous != null)
                {
                    previousNodeId = current.Previous.Value.getNodeId().ToString();
                    //string previousNodePath = "$node=" + previousNodeId;
                    //get previous dd_item_seq
                    string strPrev_DD_Item_Seq = current.Previous.Value.getValues()["D_DD_ITEM_SEQ"]; // nodeValueProvider.GetString(previousNodePath + "/$instance=STANDARD_MDF/D_DD_ITEM_SEQ", "");
                    if (int.TryParse(strPrev_DD_Item_Seq, out previousDD_Item_Seq) == false)
                    {
                        //couldn't get dd_item_seq, set previous node blank
                        previousNodeId = "";
                    }

                }



                //MessageBox.Show("cur node = " + docNode.Id.ToString() + System.Environment.NewLine
                //   + "env node = " + envelopeNode.Id.ToString() + System.Environment.NewLine
                //      + "prev node = " + previousNodeId + System.Environment.NewLine
                //    + "prev dd_item_seq = " + previousDD_Item_Seq.ToString());


                #region IA values
                //Batch level data
                string TrackUser = docNode.getValues()["TRACK_USER"]; // nodeValueProvider.GetString(nodePath + "/$instance=Standard_MDF/TRACK_USER", "");
                string TrackPerformance = docNode.getValues()["TRACK_PERFORMANCE"]; // nodeValueProvider.GetString(nodePath + "/$instance=Standard_MDF/TRACK_PERFORMANCE", "");
                string BatchNo = docNode.getValues()["BATCH_NO"]; // nodeValueProvider.GetString(nodePath + "/$instance=Standard_MDF/BATCH_NO", "");
                string SiteID = docNode.getValues()["SITE_ID"]; // nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/SITE_ID", "");
                string WorkCategory = docNode.getValues()["WORK_CATEGORY"]; // nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/WORK_CATEGORY", "");
                string ScanDate = docNode.getValues()["SCAN_COMPLETE"]; // nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/SCAN_COMPLETE", "");
                string ScannerID = docNode.getValues()["SCANNER_ID"]; // nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/SCANNER_ID", "");
                string BoxNo = docNode.getValues()["BOX_NO"]; // nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/BOX_NO", "");
                string ReceivedDate = docNode.getValues()["RECEIVED_DATE"]; // nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/RECEIVED_DATE", "");
                string ReceivedDateCRD = docNode.getValues()["CRD_RECEIVED_DATE"]; // nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/CRD_RECEIVED_DATE", "");
                //Document level data
                string D_RouteCode = docNode.getValues()["D_ROUTE_CODE"]; // nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/D_ROUTE_CODE", "");
                string D_AWDSourceType = docNode.getValues()["D_AWD_SOURCE_TYPE"]; //nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/D_AWD_SOURCE_TYPE", "");
                string D_AWDStatus = docNode.getValues()["D_AWD_STATUS"]; //nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/D_AWD_STATUS", "");
                string D_CompanyCode = docNode.getValues()["D_COMPANY_CODE"];// nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/D_COMPANY_CODE", "");
                string D_FaxID = docNode.getValues()["D_FAX_ID"]; // nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/D_FAX_ID", "");
                string D_FaxAccount = docNode.getValues()["D_FAX_ACCOUNT"]; // nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/D_FAX_ACCOUNT", "");
                string D_FaxTo = docNode.getValues()["D_FAX_RECIPIENT"]; // nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/D_FAX_RECIPIENT", "");
                string D_FaxFrom = docNode.getValues()["D_FAX_SENDER"]; // nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/D_FAX_SENDER", "");
                string D_FaxServer = docNode.getValues()["D_FAX_SERVER"]; // nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/D_FAX_SERVER", "");
                string D_TypeOfBill = docNode.getValues()["D_TYPE_OF_BILL"]; // nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/D_TYPE_OF_BILL", "");
                string D_CountHCFA = docNode.getValues()["D_HCFA_COUNT"]; // nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/D_HCFA_COUNT", "");
                string D_CountUB92 = docNode.getValues()["D_UB92_COUNT"]; // nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/D_UB92_COUNT", "");
                string D_CountPHEOMB = docNode.getValues()["D_PHEOMB_COUNT"]; // nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/D_PHEOMB_COUNT", "");
                string D_CountEOMB = docNode.getValues()["D_EOMB_COUNT"]; // nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/D_EOMB_COUNT", "");
                string D_CountAttachment = docNode.getValues()["D_ATTACHMENT_COUNT"]; // nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/D_ATTACHMENT_COUNT", "");
                string D_WorkCategory = docNode.getValues()["D_WORK_CATEGORY"]; // nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/D_WORK_CATEGORY", "");
                string D_BusinessArea = docNode.getValues()["D_BUSINESS_AREA"]; // nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/D_BUSINESS_AREA", "");
                string D_WorkType = docNode.getValues()["D_WORK_TYPE"]; // nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/D_WORK_TYPE", "");
                string D_SiteID = docNode.getValues()["D_SITE_ID"]; // nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/D_SITE_ID", "");
                string D_SystemID = docNode.getValues()["D_SYSTEM_ID"]; // nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/D_SYSTEM_ID", "");
                string D_ReceivedDate = docNode.getValues()["D_RECEIVED_DATE"]; // nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/D_RECEIVED_DATE", "");
                #endregion

                #region customvalues
                /*
                string CustomReqIndexProperty1 = nodeValueProvider.GetString(nodePath + "/$instance=" + instanceName + "/Level1_CustomReqIndex1", "");
                string CustomReqIndexProperty2 = nodeValueProvider.GetString(nodePath + "/$instance=" + instanceName + "/Level1_CustomReqIndex2", "");
                string CustomReqIndexProperty3 = nodeValueProvider.GetString(nodePath + "/$instance=" + instanceName + "/Level1_CustomReqIndex3", "");
                string CustomReqIndexProperty4 = nodeValueProvider.GetString(nodePath + "/$instance=" + instanceName + "/Level1_CustomReqIndex4", "");
                string CustomReqIndexProperty5 = nodeValueProvider.GetString(nodePath + "/$instance=" + instanceName + "/Level1_CustomReqIndex5", "");
                string CustomReqIndexProperty6 = nodeValueProvider.GetString(nodePath + "/$instance=" + instanceName + "/Level1_CustomReqIndex6", "");
                string CustomIndexProperty1 = nodeValueProvider.GetString(nodePath + "/$instance=" + instanceName + "/Level1_CustomIndex1", "");
                string CustomIndexProperty2 = nodeValueProvider.GetString(nodePath + "/$instance=" + instanceName + "/Level1_CustomIndex2", "");
                string CustomIndexProperty3 = nodeValueProvider.GetString(nodePath + "/$instance=" + instanceName + "/Level1_CustomIndex3", "");
                string CustomIndexProperty4 = nodeValueProvider.GetString(nodePath + "/$instance=" + instanceName + "/Level1_CustomIndex4", "");
                string CustomIndexProperty5 = nodeValueProvider.GetString(nodePath + "/$instance=" + instanceName + "/Level1_CustomIndex5", "");
                string CustomIndexProperty6 = nodeValueProvider.GetString(nodePath + "/$instance=" + instanceName + "/Level1_CustomIndex6", "");
                string CustomIndexProperty7 = nodeValueProvider.GetString(nodePath + "/$instance=" + instanceName + "/Level1_CustomIndex7", "");
                string CustomIndexProperty8 = nodeValueProvider.GetString(nodePath + "/$instance=" + instanceName + "/Level1_CustomIndex8", "");
                string CustomIndexProperty9 = nodeValueProvider.GetString(nodePath + "/$instance=" + instanceName + "/Level1_CustomIndex9", "");
                string CustomIndexProperty10 = nodeValueProvider.GetString(nodePath + "/$instance=" + instanceName + "/Level1_CustomIndex10", "");
                string CustomIndexProperty11 = nodeValueProvider.GetString(nodePath + "/$instance=" + instanceName + "/Level1_CustomIndex11", "");
                string CustomIndexProperty12 = nodeValueProvider.GetString(nodePath + "/$instance=" + instanceName + "/Level1_CustomIndex12", "");
                */
                #endregion customvalues

                #region custom values and extra
                /*
                string L1_BusinessArea = nodeValueProvider.GetString(nodePath + "/$instance=" + instanceName + "/Level1_BusinessArea", "");
                string L1_WorkType = nodeValueProvider.GetString(nodePath + "/$instance=" + instanceName + "/Level1_WorkType", "");
                string L1_SystemID = nodeValueProvider.GetString(nodePath + "/$instance=" + instanceName + "/Level1_SystemID", "");
                string L1_CompanyCode = nodeValueProvider.GetString(nodePath + "/$instance=" + instanceName + "/Level1_CompanyCode", "");
                string L1_BatchNo = nodeValueProvider.GetString(nodePath + "/$instance=" + instanceName + "/Level1_BatchNo", "");
                string L1_ReceivedDate = nodeValueProvider.GetString(nodePath + "/$instance=" + instanceName + "/Level1_ReceivedDate", "");
                string L1_ScanDate = nodeValueProvider.GetString(nodePath + "/$instance=" + instanceName + "/Level1_ScanDate", "");
                string L1_ScannerID = nodeValueProvider.GetString(nodePath + "/$instance=" + instanceName + "/Level1_ScannerID", "");
                string L1_SiteID = nodeValueProvider.GetString(nodePath + "/$instance=" + instanceName + "/Level1_SiteID", "");
                string L1_BoxNo = nodeValueProvider.GetString(nodePath + "/$instance=" + instanceName + "/Level1_BoxNo", "");
                string L1_WorkCategory = nodeValueProvider.GetString(nodePath + "/$instance=" + instanceName + "/Level1_WorkCategory", "");

                
                //Overwrite batch/doc level values if needed
                if (D_WorkCategory.Trim().Length != 0)
                {
                    WorkCategory = D_WorkCategory;
                }
                if (L1_WorkCategory.Trim().Length != 0)
                {
                    WorkCategory = L1_WorkCategory;
                }
                if (L1_BusinessArea.Trim().Length != 0)
                {
                    D_BusinessArea = L1_BusinessArea;
                }
                if (L1_BoxNo.Trim().Length != 0)
                {
                    BoxNo = L1_BoxNo;
                }
                if (D_SiteID.Trim().Length != 0)
                {
                    SiteID = D_SiteID;
                }
                if (L1_SiteID.Trim().Length != 0)
                {
                    SiteID = L1_SiteID;
                }
                if (L1_ScannerID.Trim().Length != 0)
                {
                    ScannerID = L1_ScannerID;
                }
                if (L1_ScanDate.Trim().Length != 0)
                {
                    ScanDate = L1_ScanDate;
                }
                if (D_ReceivedDate.Trim().Length != 0)
                {
                    ReceivedDate = D_ReceivedDate;
                }
                if (L1_ReceivedDate.Trim().Length != 0)
                {
                    ReceivedDate = L1_ReceivedDate;
                }
                if (L1_BatchNo.Trim().Length != 0)
                {
                    BatchNo = L1_BatchNo;
                }
                if (L1_SystemID.Trim().Length != 0)
                {
                    D_SystemID = L1_SystemID;
                }
                if (L1_CompanyCode.Trim().Length != 0)
                {
                    D_CompanyCode = L1_CompanyCode;
                }
                if (L1_WorkType.Trim().Length != 0)
                {
                    D_WorkType = L1_WorkType;
                }
                */
                #endregion

                if (TrackUser.Trim() == "T")
                {
                    _cp.TrackUser = true;
                }
                if (TrackPerformance.Trim() == "T")
                {
                    _cp.TrackPerformance = true;
                }
                //set common parameters
                
                _cp.BatchNo = BatchNo.Trim();
                _cp.CurrentNodeID = docNode.getNodeId().ToString();
                _cp.PreviousNodeID = previousNodeId;
                _cp.DDItemSeqPrevious = previousDD_Item_Seq;
                _cp.EnvelopeNodeID = docNode.getValues()["ENVELOPE_NODE_ID"];
                _cp.SiteID = SiteID.Trim();
                _cp.ScanDate = ScanDate.Trim();
                _cp.ScannerID = ScannerID.Trim();
                _cp.ReceivedDate = ReceivedDate.Trim();
                _cp.ReceivedDateCRD = ReceivedDateCRD.Trim();
                _cp.WorkCategory = WorkCategory.Trim();
                _cp.BoxNo = BoxNo.Trim();
                _cp.AWDSourceType = D_AWDSourceType.Trim();
                _cp.AWDStatus = D_AWDStatus.Trim();
                _cp.FaxServer = D_FaxServer.Trim();
                _cp.FaxAccount = D_FaxAccount.Trim();
                _cp.FaxID = D_FaxID.Trim();
                _cp.FaxFrom = D_FaxFrom.Trim();
                _cp.FaxTo = D_FaxTo.Trim();
                _cp.TypeOfBill = D_TypeOfBill.Trim();
                _cp.BusinessArea = D_BusinessArea.Trim();
                _cp.RouteCode = D_RouteCode.Trim();
                _cp.WorkType = D_WorkType.Trim();
                _cp.SystemID = D_SystemID.Trim();
                _cp.CompanyCode = D_CompanyCode.Trim();

                /*
                _cp.CustomIndexProperty1 = CustomIndexProperty1.Trim();
                _cp.CustomIndexProperty2 = CustomIndexProperty2.Trim();
                _cp.CustomIndexProperty3 = CustomIndexProperty3.Trim();
                _cp.CustomIndexProperty4 = CustomIndexProperty4.Trim();
                _cp.CustomIndexProperty5 = CustomIndexProperty5.Trim();
                _cp.CustomIndexProperty6 = CustomIndexProperty6.Trim();
                _cp.CustomIndexProperty7 = CustomIndexProperty7.Trim();
                _cp.CustomIndexProperty8 = CustomIndexProperty8.Trim();
                _cp.CustomIndexProperty9 = CustomIndexProperty9.Trim();
                _cp.CustomIndexProperty10 = CustomIndexProperty10.Trim();
                _cp.CustomIndexProperty11 = CustomIndexProperty11.Trim();
                _cp.CustomIndexProperty12 = CustomIndexProperty12.Trim();
                _cp.CustomReqIndexProperty1 = CustomReqIndexProperty1.Trim();
                _cp.CustomReqIndexProperty2 = CustomReqIndexProperty2.Trim();
                _cp.CustomReqIndexProperty3 = CustomReqIndexProperty3.Trim();
                _cp.CustomReqIndexProperty4 = CustomReqIndexProperty4.Trim();
                _cp.CustomReqIndexProperty5 = CustomReqIndexProperty5.Trim();
                _cp.CustomReqIndexProperty6 = CustomReqIndexProperty6.Trim();*/
                _cp.CountHCFA = D_CountHCFA.Trim();
                _cp.CountUB92 = D_CountUB92.Trim();
                _cp.CountPHEOMB = D_CountPHEOMB.Trim();
                _cp.CountEOMB = D_CountEOMB.Trim();
                _cp.CountAttachment = D_CountAttachment.Trim();
                _cp.ValidationStart = valStart.Trim();
                _cp.ImgCount = docNode.getPageCount();
                //Set ID:

                //Set Rubberband data to _cp
                if (ocrField == "PolicyNo") // || ocrField == "AgentNo")
                {
                    _cp.PolicyNo = controlContext.FindFieldData("OCRField").Text;
                }
                else if (ocrField == "AgentNo")
                {
                    _cp.AgentNo = controlContext.FindFieldData("OCRField").Text;
                }
                else if (ocrField == "PrivMasterID")
                {
                    _cp.PrivMasterID = controlContext.FindFieldData("OCRField").Text;
                }
                else if (ocrField == "ApplicationNo")
                {
                    _cp.ApplicationNo = controlContext.FindFieldData("OCRField").Text;
                }
                else if (ocrField == "Phone")
                {
                    _cp.Phone = controlContext.FindFieldData("OCRField").Text;
                }
                else if (ocrField == "PrivMasterID")
                {
                    _cp.PrivMasterID = controlContext.FindFieldData("OCRField").Text;
                }
                else if (ocrField == "ProviderTIN")
                {
                    _cp.ProviderTIN = controlContext.FindFieldData("OCRField").Text;
                }
                else if (ocrField == "AccountNumber")
                {
                    _cp.AccountNumber = controlContext.FindFieldData("OCRField").Text;
                }

                _dataAccess.getMVIFieldData(ref _cp);
                processCustParmsD( ref _cp);

                //_cp.SiteID = "CIC";
                //_cp.WorkCategory = "CLM";
                //_cp.EnvelopeNodeID = "1";
                //_cp.ScannerID = "57";
                //_cp.BatchNo = "TestDDBatch";
                //_cp.CurrentNodeID = "1";

                //launch the data director
                _dataDirector.LaunchDD(ref _cp);
                //_dummyDD.launchDummyDD();


            }
            catch (Exception ex)
            {
                Share.setDD_Running(false);
                MessageBox.Show("Error: " + ex.Message);
            }

        }

        private void processCustParmsD(ref CNO.BPA.DataHandler.CommonParameters CP)
        {
            try
            {

                foreach (DataRow dr in CP._MVIFieldData.DD_MVI_FIELD_DEFINITION.Rows)
                {

                    string dName = dr["IA_MDF_NAME"].ToString();

                    if (dName.Length != 0)
                    //update the localValue in the dataset
                    {
                        if (docNode.getValues().ContainsKey(dName))
                        {
                            string mdfValue = docNode.getValues()[dName]; // nodeValueProvider.GetString(nodePath + "/$instance=Standard_MDF/" + dName, "");
                            if (mdfValue.Trim().Length != 0 && CP[dr["FIELD_NAME"].ToString()].ToString().Length == 0)
                            //only set the values if they haven't been set by previous logic already
                            {
                                dr["LOCAL_VALUE"] = mdfValue.Trim();
                                CP[dr["FIELD_NAME"].ToString()] = mdfValue.Trim();
                            }
                        }
                    }

                }


            }
            catch (Exception ex)
            {
                throw new Exception("DataAccess.processCustParmsD: " + ex.Message);
            }
        }

		
        private void Director_Complete(int Result, string Description)
        {
            try
            {

                // MessageBox.Show(Description);
                //MessageBox.Show(Description + _cp.InsuredName + "\r\n" + _cp.PlanCode + "\r\n" + _cp.SortCode);
                //IIAValueProvider nodeValueProvider = _ddEventInfo.TaskRootNode.Values(_ddEventInfo.Application.CurrentTask.WorkflowStep);
                //string nodePath = "$node=" + _docNodeId.ToString();
                
                
                string valText = string.Empty;
                if (Result == 0)
                {
                    valText = "Validation: 0";
                    //valText = "000"; // for testing TODO: remove
                    //update mdf values of correct doc
                    if (_cp.BatchItemID.ToString().Length > 0)
                    {
                        //nodeValueProvider.SetString(nodePath + "/$instance=STANDARD_MDF/D_BATCH_ITEM_ID", _cp.BatchItemID.ToString());
                        docNode.getValues()["D_BATCH_ITEM_ID"] = _cp.BatchItemID.ToString();
                    }
                    if (_cp.DDItemSeq.ToString().Length > 0)
                    {
                        //nodeValueProvider.SetString(nodePath + "/$instance=STANDARD_MDF/D_DD_ITEM_SEQ", _cp.DDItemSeq.ToString());
                        docNode.getValues()["D_DD_ITEM_SEQ"] = _cp.DDItemSeq.ToString();
                    }

                }
                else
                {
                    //there are no indexed items
                    valText = string.Empty;
                }
                //nodeValueProvider.SetString(nodePath + "/$instance=STANDARD_MDF/D_VALIDATION", valText);
                docNode.getValues()["D_VALIDATION"] = valText;
                _valField.SetValue(valText);
               
               
                //_valField.SetValue(valText);
                validationValue = valText;
                //_valField.SetValue(valText);
                //_controlContext.FieldDataContext.UimDataContext.FindFieldDataContext("Validation").setSetFocus();
                
                Share.setDD_Running(false);
                emctest = valText;
                _valText = valText;
                
                //_valField1.SetText(valText);
                //_valField1.SetFocus();
                //_valField.MarkValueChanged();
                //_valField.SetDataConfirmed(true);
                //TextChangedValidation(IUimFormControlContext controlContext)
                //_valField1.SetText(valText);
                //TextChangedValidation(_valField1);

            }
            catch (Exception ex)
            {
                Share.setDD_Running(false);
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        
        public void EnterControlValidation(IUimFormControlContext controlContext)
        {
            controlContext.SetText(validationValue);
        }

        public void ExitControl(IUimFormControlContext controlContext)
       {
            if (!string.IsNullOrEmpty(validationValue) && validationValue.Trim() != "")
            {
                controlContext.FindFieldData("Validation").SetValue(validationValue);
            }
        }

        public void TextChangedValidation(IUimFormControlContext controlContext)
        {
            
         string value = controlContext.FieldDataContext.Text;
        }

        public void ConfirmControlValidation(IUimFormControlContext controlContext)
        {
            string value = controlContext.FieldDataContext.Text;
        }

        public void TextChanged(IUimFormControlContext controlContext)
        {
            if (controlContext.FieldDataContext != null)
            {
                string value = controlContext.FieldDataContext.Text;
            }
        }

        public void ConfirmControl(IUimFormControlContext controlContext)
        {
            string value = controlContext.FieldDataContext.Text;
        }

        public void SelectionChange(IUimFormControlContext controlContext)
        {
            if (!string.IsNullOrEmpty(validationValue) && validationValue.Trim() != "")
            {
                controlContext.FindFieldData("Validation").SetValue(validationValue);
            }
        }

        public void ExecuteValidationRuleOne(IUimDataContext dataContext)
        {


            if (dataContext.FindFieldDataContext("Replicate").Text == "")
            {
                dataContext.FindFieldDataContext("Replicate").SetValue("0");
            }
            if (dataContext.FindFieldDataContext("Rejected").Text == "")
            {
                dataContext.FindFieldDataContext("Rejected").SetValue("0");
            }
            string rejected = dataContext.FindFieldDataContext("Rejected").Text;
            string replicate = dataContext.FindFieldDataContext("Replicate").Text;
            string validation = dataContext.FindFieldDataContext("Validation").Text;

            //dataContext.FindFieldDataContext("Validation").SetValue(_valText);

            validation = validation.PadRight(10);

            if (rejected == "1" && replicate == "1")
            {
                string ruleFailMessage = "Replicate and Reject cannot both be checked.";
                dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                throw new Exception(ruleFailMessage);
            }
            if (Share.getDD_Running() == true)
            {
                string ruleFailMessage = "Data Director is running.  Please complete current document before attempting to end task.";
                dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                throw new Exception(ruleFailMessage);
            }

            if (rejected != "1" && replicate != "1" && validation.Substring(0, 10).ToUpper() != "VALIDATION" && validation.Substring(0, 6).ToUpper() != "MANUAL")
            {
                string ruleFailMessage = "If Validation is not filled in, either Replicate or Reject must be checked.";
                dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                throw new Exception(ruleFailMessage);
            }

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
    }
}
