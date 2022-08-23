using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CNO.BPA.DataHandler;
using Emc.InputAccel.IndexPlus.Scripting;
using Emc.InputAccel.QuickModule.ClientScriptingInterface;

namespace CNO.BPA.IAIndex
{


   public class FieldEvents
      : IFieldEvents
   {
      public void Initialize(IFieldEventInformation info)
      {
      }

      public void Changed(IFieldEventInformation info)
      {
         //System.Windows.Forms.MessageBox.Show(info.CurrentField.ValueName);
         //System.Windows.Forms.MessageBox.Show(info.CurrentField.Title);
         System.Windows.Forms.MessageBox.Show(info.CurrentField.Value);

      }


      public void RejectChanged(IFieldEventInformation info)
      {
         try
         {

            if (info.CurrentField.Value == "1")
            {
               int currentNodeLevel = info.CurrentNode.Level;
               //get document node from the page node
               IBatchNode docNode = null;
               IBatchNode folderNode = null;
               IBatchNode envelopeNode = null;

               switch (currentNodeLevel)
               {
                  case 0://on page
                     //get document node from the page node
                     docNode = info.CurrentNode.Parent();
                     folderNode = docNode.Parent();
                     envelopeNode = folderNode.Parent();

                     break;
                  case 1://on document
                     docNode = info.CurrentNode;
                     folderNode = docNode.Parent();
                     envelopeNode = folderNode.Parent();
                     break;
               }



               frmRejectReasons rejectForm = new frmRejectReasons();
               //IBatchInformationListParameters ibparms = info.Application.CreateBatchInformationListParameters();
               
               //foreach (IBatchInformation ibinfo in info.Application.ProcessList(ibparms))
               //{
               //   rejectForm.addProcess(ibinfo.Name);
               //}
               DataAccess dataAcces = new DataAccess();
               List<string> IAProcessList = dataAcces.getIAProcessNames();
               foreach (string processName in IAProcessList)
               {
                  rejectForm.addProcess(processName);
               }

               DialogResult dRes = rejectForm.ShowDialog();

               if (dRes == DialogResult.OK)
               {
                  string rejectResult = rejectForm.RejectResult;
                  //set the reject path
                  IIAValueProvider nodeValueProvider = info.CurrentNode.Values(info.Application.CurrentTask.WorkflowStep);
                  string nodePath = "$node=" + docNode.Id.ToString();
                  string defaultPath = nodeValueProvider.GetString(nodePath + "/$instance=STANDARD_MDF/DEFAULTREJECTPATH", "");
                  string rejectPath;
                  if (defaultPath.EndsWith(@"\"))
                  {
                     rejectPath = defaultPath + rejectResult;
                  }
                  else
                  {
                     rejectPath = defaultPath + @"\" + rejectResult;
                  }

                  nodeValueProvider.SetString(nodePath + "/$instance=STANDARD_MDF/D_REJECTIMAGEPATH", rejectPath);
               }
               else
               {
                  info.CurrentField.Value = "0";
               }
            }
         }
         catch (Exception ex)
         {
            MessageBox.Show("Error: " + ex.Message);
         }
      }

      public void Populate(IFieldEventInformation info)
      {
      }

      public ValidationResult Validate(IFieldEventInformation info, ValidationReason validationReason, Boolean silent)
      {
         return default(ValidationResult);
      }

   }
}
