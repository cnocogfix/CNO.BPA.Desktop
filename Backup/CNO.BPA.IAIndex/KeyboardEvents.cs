using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;

using Emc.InputAccel.IndexPlus.Scripting;
using Emc.InputAccel.QuickModule.ClientScriptingInterface;
using CNO.BPA.DataDirector;
using CNO.BPA.DataHandler;


namespace CNO.BPA.IAIndex
{
   public class KeyboardEvents : IUserScript
   {
      Director _dataDirector = new Director();
      DataAccess _dataAccess = new DataAccess();
      CommonParameters _cp;
      IEventInformation _ddEventInfo;
      int _docNodeId;
      IIndexField _valField;

      [UserScript()]
      public void UserScript(IEventInformation eventInformation)
      {
         //MessageBox.Show("Launching DataDirector.");
         try
         {
            int currentNodeLevel = eventInformation.CurrentNode.Level;
            if (currentNodeLevel > 1)
            {
               MessageBox.Show("Please select a document or page before launching DataDirector.");
               return;

            }
            IIAValueProvider nodeValueProvider = eventInformation.CurrentNode.Values(eventInformation.Application.CurrentTask.WorkflowStep);
            //get document node from the page node
            IBatchNode docNode = null;
            IBatchNode folderNode = null;
            IBatchNode envelopeNode = null;

            switch (currentNodeLevel)
            {
               case 0://on page
                  //get document node from the page node
                  docNode = eventInformation.CurrentNode.Parent();
                  folderNode = docNode.Parent();
                  envelopeNode = folderNode.Parent();

                  break;
               case 1://on document
                  docNode = eventInformation.CurrentNode;
                  folderNode = docNode.Parent();
                  envelopeNode = folderNode.Parent();
                  break;
            }



            string nodePath = "$node=" + docNode.Id.ToString();
            _docNodeId = docNode.Id;

            _ddEventInfo = eventInformation;
            IIndexFields iFields = eventInformation.GetCurrentNodeFields();
            _valField = iFields.GetFieldById("Validation");
            string ddrunning = nodeValueProvider.GetString(nodePath + "/$instance=Standard_MDF/E_DDRUNNING", "");
            if (ddrunning == "T")
            {
               MessageBox.Show("Data Director is already running.  Please complete current document before attempting to launch Data Director again.", "Data Director already running.");
               return;
            }
            nodeValueProvider.SetString(nodePath + "/$instance=Standard_MDF/E_DDRUNNING", "T");

            //set start of validation if needed for doc
            string valStart = string.Empty;
            valStart = nodeValueProvider.GetString(nodePath + "/$instance=Standard_MDF/D_VALIDATION_AUDIT_START", "");
            if (valStart.Trim().Length == 0)
            {
               //set val start to now
               valStart = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
               nodeValueProvider.SetString(nodePath + "/$instance=Standard_MDF/D_VALIDATION_AUDIT_START", valStart);

            }


            _dataDirector.Complete += new CompleteEventHandler(Director_Complete);



            //MessageBox.Show("DD02");
            _cp = new CommonParameters();


            string instanceName = eventInformation.Application.CurrentTask.WorkflowStep.Name;


            IBatchNode prevDocNode = docNode.Previous(envelopeNode);
            string previousNodeId = "";
            int previousDD_Item_Seq = 0;
            if (prevDocNode != null)
            {
               previousNodeId = prevDocNode.Id.ToString();
               string previousNodePath = "$node=" + previousNodeId;
               //get previous dd_item_seq
               string strPrev_DD_Item_Seq = nodeValueProvider.GetString(previousNodePath + "/$instance=STANDARD_MDF/D_DD_ITEM_SEQ", "");
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


            string TrackUser = nodeValueProvider.GetString(nodePath + "/$instance=Standard_MDF/TRACK_USER", "");
            string TrackPerformance = nodeValueProvider.GetString(nodePath + "/$instance=Standard_MDF/TRACK_PERFORMANCE", "");
            string BatchNo = nodeValueProvider.GetString(nodePath + "/$instance=Standard_MDF/BATCH_NO", "");
            string SiteID = nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/SITE_ID", "");
            string WorkCategory = nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/WORK_CATEGORY", "");
            string ScanDate = nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/SCAN_COMPLETE", "");
            string ScannerID = nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/SCANNER_ID", "");
            string BoxNo = nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/BOX_NO", "");
            string ReceivedDate = nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/RECEIVED_DATE", "");
            string ReceivedDateCRD = nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/CRD_RECEIVED_DATE", "");
            string D_RouteCode = nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/D_ROUTE_CODE", "");
            string D_AWDSourceType = nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/D_AWD_SOURCE_TYPE", "");
            string D_AWDStatus = nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/D_AWD_STATUS", "");
            string D_CompanyCode = nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/D_COMPANY_CODE", "");
            string D_FaxID = nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/D_FAX_ID", "");
            string D_FaxAccount = nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/D_FAX_ACCOUNT", "");
            string D_FaxTo = nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/D_FAX_RECIPIENT", "");
            string D_FaxFrom = nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/D_FAX_SENDER", "");
            string D_FaxServer = nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/D_FAX_SERVER", "");
            string D_TypeOfBill = nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/D_TYPE_OF_BILL", "");
            string D_CountHCFA = nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/D_HCFA_COUNT", "");
            string D_CountUB92 = nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/D_UB92_COUNT", "");
            string D_CountPHEOMB = nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/D_PHEOMB_COUNT", "");
            string D_CountEOMB = nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/D_EOMB_COUNT", "");
            string D_CountAttachment = nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/D_ATTACHMENT_COUNT", "");
            string D_WorkCategory = nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/D_WORK_CATEGORY", "");
            string D_BusinessArea = nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/D_BUSINESS_AREA", "");
            string D_WorkType = nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/D_WORK_TYPE", "");
            string D_SiteID = nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/D_SITE_ID", "");
            string D_SystemID = nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/D_SYSTEM_ID", "");
            string D_ReceivedDate = nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/D_RECEIVED_DATE", "");

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
            _cp.CurrentNodeID = docNode.Id.ToString();
            _cp.PreviousNodeID = previousNodeId;
            _cp.DDItemSeqPrevious = previousDD_Item_Seq;
            _cp.EnvelopeNodeID = envelopeNode.Id.ToString();
            _cp.SiteID = SiteID.Trim();
            _cp.WorkCategory = WorkCategory.Trim();
            _cp.AWDSourceType = D_AWDSourceType.Trim();
            _cp.AWDStatus = D_AWDStatus.Trim();
            _cp.ReceivedDate = ReceivedDate.Trim();
            _cp.ReceivedDateCRD = ReceivedDateCRD.Trim();
            _cp.FaxServer = D_FaxServer.Trim();
            _cp.FaxAccount = D_FaxAccount.Trim();
            _cp.FaxID = D_FaxID.Trim();
            _cp.FaxFrom = D_FaxFrom.Trim();
            _cp.FaxTo = D_FaxTo.Trim();
            _cp.TypeOfBill = D_TypeOfBill.Trim();
            _cp.BoxNo = BoxNo.Trim();
            _cp.BusinessArea = D_BusinessArea.Trim();
            _cp.RouteCode = D_RouteCode.Trim();
            _cp.WorkType = D_WorkType.Trim();
            _cp.SystemID = D_SystemID.Trim();
            _cp.CompanyCode = D_CompanyCode.Trim();
            _cp.ScanDate = ScanDate.Trim();
            _cp.ScannerID = ScannerID.Trim();
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
            _cp.CustomReqIndexProperty6 = CustomReqIndexProperty6.Trim();
            _cp.CountHCFA = D_CountHCFA.Trim();
            _cp.CountUB92 = D_CountUB92.Trim();
            _cp.CountPHEOMB = D_CountPHEOMB.Trim();
            _cp.CountEOMB = D_CountEOMB.Trim();
            _cp.CountAttachment = D_CountAttachment.Trim();
            _cp.ValidationStart = valStart.Trim();
            _cp.ImgCount = docNode.ChildCount(0);
            _dataAccess.getMVIFieldData(ref _cp);
            processCustParmsD(nodeValueProvider, nodePath, ref _cp);

            //_cp.SiteID = "CIC";
            //_cp.WorkCategory = "CLM";
            //_cp.EnvelopeNodeID = "1";
            //_cp.ScannerID = "57";
            //_cp.BatchNo = "TestDDBatch";
            //_cp.CurrentNodeID = "1";

            //launch the data director
            _dataDirector.LaunchDD(ref _cp);
         }
         catch (Exception ex)
         {
            MessageBox.Show("Error: " + ex.Message);
         }

      }
      private void processCustParmsD(IIAValueProvider nodeValueProvider, string nodePath, ref CNO.BPA.DataHandler.CommonParameters CP)
      {
         try
         {

            foreach (DataRow dr in CP._MVIFieldData.DD_MVI_FIELD_DEFINITION.Rows)
            {

               string dName = dr["IA_MDF_NAME"].ToString();

               if (dName.Length != 0)
               //update the localValue in the dataset
               {
                  string mdfValue = nodeValueProvider.GetString(nodePath + "/$instance=Standard_MDF/" + dName, "");
                  if (mdfValue.Trim().Length != 0 && CP[dr["FIELD_NAME"].ToString()].ToString().Length == 0)
                     //only set the values if they haven't been set by previous logic already
                  {
                     dr["LOCAL_VALUE"] = mdfValue.Trim();
                     CP[dr["FIELD_NAME"].ToString()] = mdfValue.Trim();
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
            IIAValueProvider nodeValueProvider = _ddEventInfo.TaskRootNode.Values(_ddEventInfo.Application.CurrentTask.WorkflowStep);
            string nodePath = "$node=" + _docNodeId.ToString();
            string valText = string.Empty;
            if (Result == 0)
            {
               valText = "Validation: 0";
               //update mdf values of correct doc
               if (_cp.BatchItemID.ToString().Length > 0)
               {
                  nodeValueProvider.SetString(nodePath + "/$instance=STANDARD_MDF/D_BATCH_ITEM_ID", _cp.BatchItemID.ToString());
               }
               if (_cp.DDItemSeq.ToString().Length > 0)
               {
                  nodeValueProvider.SetString(nodePath + "/$instance=STANDARD_MDF/D_DD_ITEM_SEQ", _cp.DDItemSeq.ToString());
               }
            }
            else
            {
               //there are no indexed items
               valText = string.Empty;
            }
            nodeValueProvider.SetString(nodePath + "/$instance=STANDARD_MDF/D_VALIDATION", valText);
            //update validation text box saved when DD was called
            _valField.Value = valText;
            nodeValueProvider.SetString(nodePath + "/$instance=Standard_MDF/E_DDRUNNING", "F");
         }
         catch (Exception ex)
         {
            MessageBox.Show("Error: " + ex.Message);
         }
      }

   }
}
