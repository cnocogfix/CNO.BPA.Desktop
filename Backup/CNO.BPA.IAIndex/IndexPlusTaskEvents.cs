using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Emc.InputAccel.IndexPlus.Scripting;
using Emc.InputAccel.QuickModule.ClientScriptingInterface;
using CNO.BPA.DataHandler;

namespace CNO.BPA.IAIndex
{

   public class IndexPlusTaskEvents
      : IIndexPlusTaskEvents
   {
      public void PrepareTask(IEventInformation taskInfo)
      {
         try
         {
            IIAValueProvider nodeValueProvider = taskInfo.TaskRootNode.Values(taskInfo.Application.CurrentTask.WorkflowStep);
            string nodePath = "$node=" + taskInfo.TaskRootNode.Id.ToString();
            string ddrunning = nodeValueProvider.GetString(nodePath + "/$instance=Standard_MDF/E_DDRUNNING", "");

            if (ddrunning == "T")
            {
               //set ddrunning to f in case task was previously closed improperly
               nodeValueProvider.SetString(nodePath + "/$instance=Standard_MDF/E_DDRUNNING", "F");

            }
         }
         catch (Exception ex)
         {
            MessageBox.Show("Error: " + ex.Message);
         }
      }

      public Boolean BeforeTaskFinished(IEventInformation taskInfo, Boolean cancelling)
      {
         try
         {

            //make sure datadirector isn't running
            IIAValueProvider nodeValueProvider = taskInfo.TaskRootNode.Values(taskInfo.Application.CurrentTask.WorkflowStep);
            string nodePath = "$node=" + taskInfo.TaskRootNode.Id.ToString();
            string ddrunning = nodeValueProvider.GetString(nodePath + "/$instance=Standard_MDF/E_DDRUNNING", "");
            string batchIssues = nodeValueProvider.GetString(nodePath + "/$instance=Standard_MDF/E_BATCHISSUES", "");
            string batchNo = nodeValueProvider.GetString(nodePath + "/$instance=Standard_MDF/BATCH_NO", "");
            if (ddrunning == "T")
            {
               MessageBox.Show("Data Director is running.  Please complete current document before attempting to end task.", "Data Director running.");
               return false;
            }
            else
            {
               if (cancelling == false)
               {
                  //validate everything is filled in correctly
                  //get document node from the page node
                  IBatchNode envelopeNode = taskInfo.TaskRootNode;

                  int i = 0;
                  //loop through docs in envelope
                  foreach (IBatchNode inode in envelopeNode.Children(1))
                  {
                     i++;
                     nodePath = "$node=" + inode.Id.ToString();
                     //get the values to check
                     string rejected = nodeValueProvider.GetString(nodePath + "/$instance=Standard_MDF/D_REJECTED", "");
                     string replicate = nodeValueProvider.GetString(nodePath + "/$instance=Standard_MDF/D_REPLICATE_FLAG", "");
                     string validation = nodeValueProvider.GetString(nodePath + "/$instance=Standard_MDF/D_VALIDATION", "");
                     validation = validation.PadRight(10);
                     if (rejected == "1" && replicate == "1")
                     {
                        MessageBox.Show("Replicate and Reject cannot both be checked for document " + i.ToString() + ".");
                        return false;
                     }

                     if (rejected != "1" && replicate != "1" && validation.Substring(0, 10).ToUpper() != "VALIDATION" && validation.Substring(0, 6).ToUpper() != "MANUAL")
                     {
                        MessageBox.Show("If Validation is not filled in, either Replicate or Reject must be checked for document " + i.ToString() + ".");
                        return false;
                     }
                     if (inode.Next() == null && replicate == "1")
                     {
                        MessageBox.Show("Replicate is not a valid selection for the last document in an envelope.  Document " + i.ToString() + ".");
                        return false;

                     }

                  }
                  if (batchIssues == "1") //prompt user to enter issues
                  {
                     BatchIssues frmBatchIssues = new BatchIssues(batchNo);
                     DialogResult dRes = frmBatchIssues.ShowDialog();

                     if (dRes != DialogResult.OK)
                     {
                        return false;
                     }

                  }
                  //before leaving add any rejected docs to the reject table
                  foreach (IBatchNode inode in envelopeNode.Children(1))
                  {
                     nodePath = "$node=" + inode.Id.ToString();
                     string rejected = nodeValueProvider.GetString(nodePath + "/$instance=Standard_MDF/D_REJECTED", "");
                     if (rejected == "1")
                     {
                        CommonParameters CP = new CommonParameters();
                        string BatchNo = nodeValueProvider.GetString(nodePath + "/$instance=Standard_MDF/BATCH_NO", "");
                        CP.BatchNo = BatchNo.Trim();
                        CP.CurrentNodeID = inode.Id.ToString();
                        DataAccess dataAcces = new DataAccess();
                        dataAcces.createBatchItemReject(ref CP);

                     }
                  }
                  return true;

               }
               else
               {
                  return true;
               }
            }
         }
         catch (Exception ex)
         {
            MessageBox.Show("Error: " + ex.Message);
            return false;
         }
      }
   }


}
