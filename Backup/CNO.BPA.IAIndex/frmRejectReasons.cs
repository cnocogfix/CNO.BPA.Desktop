using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CNO.BPA.IAIndex
{
   public partial class frmRejectReasons : Form
   {
      private string _rejectResult = "";

      public frmRejectReasons()
      {
         InitializeComponent();
      }

      private void frmRejectReasons_Load(object sender, EventArgs e)
      {

      }
      public void addProcess(string processName)
      {
         this.cboProcessList.Items.Add(processName);
      }

      private void rbWrongProcess_CheckedChanged(object sender, EventArgs e)
      {
         if (rbWrongProcess.Checked)
         {
            this.cboProcessList.Enabled = true;
         }
         else
         {
            this.cboProcessList.Enabled = false;
         }
      }
      public string RejectResult
      {
         get { return _rejectResult ; }
         set { _rejectResult = value; }
      }

      private void btnOK_Click(object sender, EventArgs e)
      {
         if (this.rbWrongProcess.Checked == false && this.rbResearch.Checked == false && this.rbPrint.Checked == false)
         {

            MessageBox.Show("Please make a selection or select Cancel to close this window.");
            return;
         }
         else if (this.rbWrongProcess.Checked && this.cboProcessList.SelectedIndex == -1)
         {
            MessageBox.Show("You must specify the correct process in order to continue.");
            return;

         }
         else
         {
            if (rbWrongProcess.Checked)
            {
               _rejectResult = cboProcessList.SelectedItem.ToString();
            }
            else if (rbResearch.Checked)
            {
               _rejectResult = "RESEARCH";
            }
            else if (rbPrint.Checked)
            {
               _rejectResult = "PRINT";
            }
            this.DialogResult = DialogResult.OK;
         }
      }
   }
}
