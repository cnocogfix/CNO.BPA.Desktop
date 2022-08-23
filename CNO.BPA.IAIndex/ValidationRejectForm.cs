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
    public partial class ValidationRejectForm : Form
    {
        private string _rejectResult = "";

        public ValidationRejectForm()
        {
            InitializeComponent();
        }

        private void frmRejectReasons_Load(object sender, EventArgs e)
        {

        }

        public void addWrongDeptOption(string wrongDeptOption)
        {
            this.cboWrongDeptOptions.Items.Add(wrongDeptOption);
        }

        public void addWrongCompanyOption(string wrongCompanyOption)
        {
            this.cboWrongCompany.Items.Add(wrongCompanyOption);
        }

        public void addIssueOption(string issueOption)
        {
            this.cboIssues.Items.Add(issueOption);
        }

        private void rbWrongDept_CheckedChanged(object sender, EventArgs e)
        {
            if (rbWrongDept.Checked)
            {
                this.cboWrongDeptOptions.Enabled = true;
            }
            else
            {
                this.cboWrongDeptOptions.Enabled = false;
            }
        }

        private void rbWrongCompany_CheckedChanged(object sender, EventArgs e)
        {
            if (rbWrongCompany.Checked)
            {
                this.cboWrongCompany.Enabled = true;
            }
            else
            {
                this.cboWrongCompany.Enabled = false;
            }
        }

        private void rbIssues_CheckedChanged(object sender, EventArgs e)
        {
            if (rbIssues.Checked)
            {
                this.cboIssues.Enabled = true;
            }
            else
            {
                this.cboIssues.Enabled = false;
            }
        }

        public string RejectResult
        {
            get { return _rejectResult ; }
            set { _rejectResult = value; }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (this.rbWrongDept.Checked == false && this.rbWrongCompany.Checked == false &&
                this.rbTrash.Checked == false && this.rbInteroffice.Checked == false && this.rbIssues.Checked == false)
            {

                MessageBox.Show("Please make a selection or select Cancel to close this window.");
                return;
            }
            else if (this.rbWrongDept.Checked && this.cboWrongDeptOptions.SelectedIndex == -1)
            {
                MessageBox.Show("You must specify the correct wrong department option in order to continue.");
                return;

            }
            else if (this.rbWrongCompany.Checked && this.cboWrongCompany.SelectedIndex == -1)
            {
                MessageBox.Show("You must specify the correct wrong company option in order to continue.");
                return;

            }
            else if (this.rbIssues.Checked && this.cboIssues.SelectedIndex == -1)
            {
                MessageBox.Show("You must specify the correct issue option in order to continue.");
                return;

            }
            else
            {
                if (rbWrongDept.Checked)
                {
                    _rejectResult = cboWrongDeptOptions.SelectedItem.ToString();
                }
                else if (rbWrongCompany.Checked)
                {
                    _rejectResult = cboWrongCompany.SelectedItem.ToString();
                }
                else if (rbTrash.Checked)
                {
                    _rejectResult = "Trash";
                }
                else if (rbInteroffice.Checked)
                {
                    _rejectResult = "Interoffice";
                }
                else if (rbIssues.Checked)
                {
                    _rejectResult = cboIssues.SelectedItem.ToString();
                }
                this.DialogResult = DialogResult.OK;
            }
        }
   }
}
