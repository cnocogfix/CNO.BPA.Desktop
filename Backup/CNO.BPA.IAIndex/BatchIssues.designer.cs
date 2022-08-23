namespace CNO.BPA.IAIndex
{
    partial class BatchIssues
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
           this.dgvBatchIssues = new System.Windows.Forms.DataGridView();
           this.Issue_Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
           this.FREQUENCY = new System.Windows.Forms.DataGridViewTextBoxColumn();
           this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
           this.btnCancel = new System.Windows.Forms.Button();
           this.btnOK = new System.Windows.Forms.Button();
           ((System.ComponentModel.ISupportInitialize)(this.dgvBatchIssues)).BeginInit();
           this.tableLayoutPanel1.SuspendLayout();
           this.SuspendLayout();
           // 
           // dgvBatchIssues
           // 
           this.dgvBatchIssues.AllowUserToAddRows = false;
           this.dgvBatchIssues.AllowUserToDeleteRows = false;
           this.dgvBatchIssues.AllowUserToResizeRows = false;
           this.dgvBatchIssues.BorderStyle = System.Windows.Forms.BorderStyle.None;
           this.dgvBatchIssues.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
           this.dgvBatchIssues.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Issue_Type,
            this.FREQUENCY});
           this.dgvBatchIssues.Location = new System.Drawing.Point(0, 2);
           this.dgvBatchIssues.MultiSelect = false;
           this.dgvBatchIssues.Name = "dgvBatchIssues";
           this.dgvBatchIssues.Size = new System.Drawing.Size(291, 249);
           this.dgvBatchIssues.TabIndex = 0;
           // 
           // Issue_Type
           // 
           this.Issue_Type.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
           this.Issue_Type.HeaderText = "Issue Type";
           this.Issue_Type.Name = "Issue_Type";
           this.Issue_Type.ReadOnly = true;
           this.Issue_Type.Width = 84;
           // 
           // FREQUENCY
           // 
           this.FREQUENCY.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
           this.FREQUENCY.HeaderText = "Frequency";
           this.FREQUENCY.MaxInputLength = 2;
           this.FREQUENCY.Name = "FREQUENCY";
           // 
           // tableLayoutPanel1
           // 
           this.tableLayoutPanel1.ColumnCount = 2;
           this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.30303F));
           this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 49.69697F));
           this.tableLayoutPanel1.Controls.Add(this.btnCancel, 1, 0);
           this.tableLayoutPanel1.Controls.Add(this.btnOK, 0, 0);
           this.tableLayoutPanel1.Location = new System.Drawing.Point(119, 261);
           this.tableLayoutPanel1.Name = "tableLayoutPanel1";
           this.tableLayoutPanel1.RowCount = 1;
           this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
           this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
           this.tableLayoutPanel1.Size = new System.Drawing.Size(165, 29);
           this.tableLayoutPanel1.TabIndex = 16;
           // 
           // btnCancel
           // 
           this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
           this.btnCancel.Location = new System.Drawing.Point(85, 3);
           this.btnCancel.Name = "btnCancel";
           this.btnCancel.Size = new System.Drawing.Size(75, 23);
           this.btnCancel.TabIndex = 7;
           this.btnCancel.Text = "Cancel";
           this.btnCancel.UseVisualStyleBackColor = true;
           this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
           // 
           // btnOK
           // 
           this.btnOK.Location = new System.Drawing.Point(3, 3);
           this.btnOK.Name = "btnOK";
           this.btnOK.Size = new System.Drawing.Size(75, 23);
           this.btnOK.TabIndex = 6;
           this.btnOK.Text = "OK";
           this.btnOK.UseVisualStyleBackColor = true;
           this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
           // 
           // BatchIssues
           // 
           this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
           this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
           this.ClientSize = new System.Drawing.Size(292, 298);
           this.Controls.Add(this.tableLayoutPanel1);
           this.Controls.Add(this.dgvBatchIssues);
           this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
           this.Name = "BatchIssues";
           this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
           this.Text = "Enter Batch Issues";
           this.Shown += new System.EventHandler(this.BatchIssues_Shown);
           ((System.ComponentModel.ISupportInitialize)(this.dgvBatchIssues)).EndInit();
           this.tableLayoutPanel1.ResumeLayout(false);
           this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvBatchIssues;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.DataGridViewTextBoxColumn Issue_Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn FREQUENCY;
    }
}