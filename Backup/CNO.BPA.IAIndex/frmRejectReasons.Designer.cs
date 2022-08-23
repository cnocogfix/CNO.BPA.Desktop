namespace CNO.BPA.IAIndex
{
   partial class frmRejectReasons
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
         this.btnCancel = new System.Windows.Forms.Button();
         this.btnOK = new System.Windows.Forms.Button();
         this.rbWrongProcess = new System.Windows.Forms.RadioButton();
         this.rbResearch = new System.Windows.Forms.RadioButton();
         this.rbPrint = new System.Windows.Forms.RadioButton();
         this.cboProcessList = new System.Windows.Forms.ComboBox();
         this.groupBox1 = new System.Windows.Forms.GroupBox();
         this.groupBox1.SuspendLayout();
         this.SuspendLayout();
         // 
         // btnCancel
         // 
         this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.btnCancel.Location = new System.Drawing.Point(152, 177);
         this.btnCancel.Name = "btnCancel";
         this.btnCancel.Size = new System.Drawing.Size(75, 23);
         this.btnCancel.TabIndex = 0;
         this.btnCancel.Text = "Cancel";
         this.btnCancel.UseVisualStyleBackColor = true;
         // 
         // btnOK
         // 
         this.btnOK.Location = new System.Drawing.Point(61, 177);
         this.btnOK.Name = "btnOK";
         this.btnOK.Size = new System.Drawing.Size(75, 23);
         this.btnOK.TabIndex = 1;
         this.btnOK.Text = "OK";
         this.btnOK.UseVisualStyleBackColor = true;
         this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
         // 
         // rbWrongProcess
         // 
         this.rbWrongProcess.AutoSize = true;
         this.rbWrongProcess.Location = new System.Drawing.Point(28, 24);
         this.rbWrongProcess.Name = "rbWrongProcess";
         this.rbWrongProcess.Size = new System.Drawing.Size(98, 17);
         this.rbWrongProcess.TabIndex = 2;
         this.rbWrongProcess.TabStop = true;
         this.rbWrongProcess.Text = "Wrong Process";
         this.rbWrongProcess.UseVisualStyleBackColor = true;
         this.rbWrongProcess.CheckedChanged += new System.EventHandler(this.rbWrongProcess_CheckedChanged);
         // 
         // rbResearch
         // 
         this.rbResearch.AutoSize = true;
         this.rbResearch.Location = new System.Drawing.Point(28, 88);
         this.rbResearch.Name = "rbResearch";
         this.rbResearch.Size = new System.Drawing.Size(71, 17);
         this.rbResearch.TabIndex = 3;
         this.rbResearch.TabStop = true;
         this.rbResearch.Text = "Research";
         this.rbResearch.UseVisualStyleBackColor = true;
         // 
         // rbPrint
         // 
         this.rbPrint.AutoSize = true;
         this.rbPrint.Location = new System.Drawing.Point(28, 114);
         this.rbPrint.Name = "rbPrint";
         this.rbPrint.Size = new System.Drawing.Size(46, 17);
         this.rbPrint.TabIndex = 4;
         this.rbPrint.TabStop = true;
         this.rbPrint.Text = "Print";
         this.rbPrint.UseVisualStyleBackColor = true;
         // 
         // cboProcessList
         // 
         this.cboProcessList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.cboProcessList.Enabled = false;
         this.cboProcessList.FormattingEnabled = true;
         this.cboProcessList.Location = new System.Drawing.Point(44, 52);
         this.cboProcessList.Name = "cboProcessList";
         this.cboProcessList.Size = new System.Drawing.Size(205, 21);
         this.cboProcessList.TabIndex = 5;
         // 
         // groupBox1
         // 
         this.groupBox1.Controls.Add(this.cboProcessList);
         this.groupBox1.Controls.Add(this.rbPrint);
         this.groupBox1.Controls.Add(this.rbResearch);
         this.groupBox1.Controls.Add(this.rbWrongProcess);
         this.groupBox1.Location = new System.Drawing.Point(7, 9);
         this.groupBox1.Name = "groupBox1";
         this.groupBox1.Size = new System.Drawing.Size(266, 151);
         this.groupBox1.TabIndex = 6;
         this.groupBox1.TabStop = false;
         // 
         // frmRejectReasons
         // 
         this.AcceptButton = this.btnOK;
         this.AllowDrop = true;
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.CancelButton = this.btnCancel;
         this.ClientSize = new System.Drawing.Size(279, 212);
         this.ControlBox = false;
         this.Controls.Add(this.groupBox1);
         this.Controls.Add(this.btnOK);
         this.Controls.Add(this.btnCancel);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "frmRejectReasons";
         this.Text = "Reason for Rejecting";
         this.Load += new System.EventHandler(this.frmRejectReasons_Load);
         this.groupBox1.ResumeLayout(false);
         this.groupBox1.PerformLayout();
         this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.Button btnCancel;
      private System.Windows.Forms.Button btnOK;
      private System.Windows.Forms.RadioButton rbWrongProcess;
      private System.Windows.Forms.RadioButton rbResearch;
      private System.Windows.Forms.RadioButton rbPrint;
      private System.Windows.Forms.ComboBox cboProcessList;
      private System.Windows.Forms.GroupBox groupBox1;
   }
}