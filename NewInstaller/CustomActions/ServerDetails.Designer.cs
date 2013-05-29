namespace OSAInstallCustomActions
{
    partial class ServerDetails
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServerDetails));
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.q1TextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.q2TextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.q3TextBox = new System.Windows.Forms.TextBox();
            this.q4TextBox = new System.Windows.Forms.TextBox();
            this.ipRadioButton = new System.Windows.Forms.RadioButton();
            this.localhostRadioButton = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Enabled = false;
            this.okButton.Location = new System.Drawing.Point(103, 63);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "Ok";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(184, 63);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // q1TextBox
            // 
            this.q1TextBox.Location = new System.Drawing.Point(91, 11);
            this.q1TextBox.MaxLength = 3;
            this.q1TextBox.Name = "q1TextBox";
            this.q1TextBox.Size = new System.Drawing.Size(33, 20);
            this.q1TextBox.TabIndex = 3;
            this.q1TextBox.Text = "192";
            this.q1TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.q1TextBox.TextChanged += new System.EventHandler(this.IPTextChanged);
            this.q1TextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.q1TextBox_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(124, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(10, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = ".";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(170, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(10, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = ".";
            // 
            // q2TextBox
            // 
            this.q2TextBox.Location = new System.Drawing.Point(137, 11);
            this.q2TextBox.MaxLength = 3;
            this.q2TextBox.Name = "q2TextBox";
            this.q2TextBox.Size = new System.Drawing.Size(33, 20);
            this.q2TextBox.TabIndex = 5;
            this.q2TextBox.Text = "168";
            this.q2TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.q2TextBox.TextChanged += new System.EventHandler(this.IPTextChanged);
            this.q2TextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.q2TextBox_KeyPress);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(214, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(10, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = ".";
            // 
            // q3TextBox
            // 
            this.q3TextBox.Location = new System.Drawing.Point(181, 11);
            this.q3TextBox.MaxLength = 3;
            this.q3TextBox.Name = "q3TextBox";
            this.q3TextBox.Size = new System.Drawing.Size(33, 20);
            this.q3TextBox.TabIndex = 7;
            this.q3TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.q3TextBox.TextChanged += new System.EventHandler(this.IPTextChanged);
            this.q3TextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.q3TextBox_KeyPress);
            // 
            // q4TextBox
            // 
            this.q4TextBox.Location = new System.Drawing.Point(226, 11);
            this.q4TextBox.MaxLength = 3;
            this.q4TextBox.Name = "q4TextBox";
            this.q4TextBox.Size = new System.Drawing.Size(33, 20);
            this.q4TextBox.TabIndex = 9;
            this.q4TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.q4TextBox.TextChanged += new System.EventHandler(this.IPTextChanged);
            this.q4TextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.q4TextBox_KeyPress);
            // 
            // ipRadioButton
            // 
            this.ipRadioButton.AutoSize = true;
            this.ipRadioButton.Location = new System.Drawing.Point(12, 12);
            this.ipRadioButton.Name = "ipRadioButton";
            this.ipRadioButton.Size = new System.Drawing.Size(35, 17);
            this.ipRadioButton.TabIndex = 10;
            this.ipRadioButton.TabStop = true;
            this.ipRadioButton.Text = "IP";
            this.ipRadioButton.UseVisualStyleBackColor = true;
            // 
            // localhostRadioButton
            // 
            this.localhostRadioButton.AutoSize = true;
            this.localhostRadioButton.Location = new System.Drawing.Point(12, 38);
            this.localhostRadioButton.Name = "localhostRadioButton";
            this.localhostRadioButton.Size = new System.Drawing.Size(71, 17);
            this.localhostRadioButton.TabIndex = 11;
            this.localhostRadioButton.TabStop = true;
            this.localhostRadioButton.Text = "Localhost";
            this.localhostRadioButton.UseVisualStyleBackColor = true;
            this.localhostRadioButton.CheckedChanged += new System.EventHandler(this.localhostRadioButton_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Location = new System.Drawing.Point(12, 92);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(254, 124);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Info";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(6, 19);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(241, 99);
            this.textBox1.TabIndex = 1;
            this.textBox1.Text = "The address of the computer that is running or will run MySQL, that will host the" +
    " OSA database. \r\n\r\nUsing localhost will set the destination to the current compu" +
    "ter.\r\n";
            // 
            // ServerDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(278, 228);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.localhostRadioButton);
            this.Controls.Add(this.ipRadioButton);
            this.Controls.Add(this.q4TextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.q3TextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.q2TextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.q1TextBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ServerDetails";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Server - Details";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.ServerDetails_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TextBox q1TextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox q2TextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox q3TextBox;
        private System.Windows.Forms.TextBox q4TextBox;
        private System.Windows.Forms.RadioButton ipRadioButton;
        private System.Windows.Forms.RadioButton localhostRadioButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBox1;
    }
}