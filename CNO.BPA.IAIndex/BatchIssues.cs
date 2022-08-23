using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Custom.InputAccel.UimScript
{
   public partial class BatchIssues : Form
   {

      CNO.BPA.DataHandler.DataAccess _dbAccess = null;
      string _batchNo;

      public BatchIssues(string BatchNo)
      {
         InitializeComponent();
         _dbAccess = new CNO.BPA.DataHandler.DataAccess();
         _batchNo = BatchNo;
      }

      private void btnOK_Click(object sender, EventArgs e)
      {
         //if they click ok, establish a new dictionary
         Dictionary<string, int> BatchIssues = new Dictionary<string, int>();
         foreach (DataGridViewRow row in dgvBatchIssues.Rows)
         {
            if (row.Cells[1].Value.ToString() != "0" && row.Cells[1].Value.ToString().Length > 0)
            {
               int freq = 0;
               if (Int32.TryParse(row.Cells[1].Value.ToString(), out freq))
               {
                  BatchIssues.Add(row.Cells[0].Value.ToString(), freq);
               }
            }
         }
         if (BatchIssues.Count == 0)
         {
            MessageBox.Show("You must indicate the number of issues encountered for the current batch");
            return;
         }
         else
         {
            foreach (KeyValuePair<string, int> batchIssue in BatchIssues)
            {
               _dbAccess.insertBatchIssue(_batchNo, batchIssue.Key, batchIssue.Value);
            }

         }
         //then leave
         this.DialogResult = DialogResult.OK;
         this.Close();
      }

      private void btnCancel_Click(object sender, EventArgs e)
      {
         this.DialogResult = DialogResult.Cancel;
         this.Close();
      }
      private void populateGrid()
      {
         Dictionary<string, int> BatchIssues = _dbAccess.getBatchIssueTypes();

         foreach (KeyValuePair <string, int> kvp in BatchIssues)
         {
            object[] newrow = { kvp.Key, kvp.Value };
            dgvBatchIssues.Rows.Add(newrow);
         }


      }

      private void BatchIssues_Shown(object sender, EventArgs e)
      {
         populateGrid();
      }

   }
}
