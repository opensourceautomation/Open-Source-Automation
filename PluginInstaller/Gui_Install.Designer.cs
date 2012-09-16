namespace PluginInstaller
{
    partial class Gui_Install
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Gui_Install));
            this.ImagePictureBox_frm_gui_install = new System.Windows.Forms.PictureBox();
            this.progressBar_frm_gui_install = new System.Windows.Forms.ProgressBar();
            this.label_gui_install_author = new System.Windows.Forms.Label();
            this.richTextBox_gui_install = new System.Windows.Forms.RichTextBox();
            this.button_frm_gui_cancel = new System.Windows.Forms.Button();
            this.label_gui_install_statut = new System.Windows.Forms.Label();
            this.button_frm_gui_install = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.ImagePictureBox_frm_gui_install)).BeginInit();
            this.SuspendLayout();
            // 
            // ImagePictureBox_frm_gui_install
            // 
            this.ImagePictureBox_frm_gui_install.ErrorImage = null;
            this.ImagePictureBox_frm_gui_install.InitialImage = null;
            this.ImagePictureBox_frm_gui_install.Location = new System.Drawing.Point(15, 27);
            this.ImagePictureBox_frm_gui_install.Name = "ImagePictureBox_frm_gui_install";
            this.ImagePictureBox_frm_gui_install.Size = new System.Drawing.Size(70, 61);
            this.ImagePictureBox_frm_gui_install.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ImagePictureBox_frm_gui_install.TabIndex = 15;
            this.ImagePictureBox_frm_gui_install.TabStop = false;
            // 
            // progressBar_frm_gui_install
            // 
            this.progressBar_frm_gui_install.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar_frm_gui_install.Location = new System.Drawing.Point(94, 62);
            this.progressBar_frm_gui_install.Name = "progressBar_frm_gui_install";
            this.progressBar_frm_gui_install.Size = new System.Drawing.Size(289, 14);
            this.progressBar_frm_gui_install.TabIndex = 13;
            // 
            // label_gui_install_author
            // 
            this.label_gui_install_author.AutoSize = true;
            this.label_gui_install_author.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_gui_install_author.Location = new System.Drawing.Point(91, 46);
            this.label_gui_install_author.Name = "label_gui_install_author";
            this.label_gui_install_author.Size = new System.Drawing.Size(80, 13);
            this.label_gui_install_author.TabIndex = 12;
            this.label_gui_install_author.Text = "by AuthorName";
            // 
            // richTextBox_gui_install
            // 
            this.richTextBox_gui_install.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox_gui_install.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_gui_install.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox_gui_install.Location = new System.Drawing.Point(15, 115);
            this.richTextBox_gui_install.Name = "richTextBox_gui_install";
            this.richTextBox_gui_install.Size = new System.Drawing.Size(368, 128);
            this.richTextBox_gui_install.TabIndex = 11;
            this.richTextBox_gui_install.Text = "Show Plugin Description";
            // 
            // button_frm_gui_cancel
            // 
            this.button_frm_gui_cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_frm_gui_cancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_frm_gui_cancel.Location = new System.Drawing.Point(311, 86);
            this.button_frm_gui_cancel.Name = "button_frm_gui_cancel";
            this.button_frm_gui_cancel.Size = new System.Drawing.Size(72, 23);
            this.button_frm_gui_cancel.TabIndex = 10;
            this.button_frm_gui_cancel.Text = "Cancel";
            this.button_frm_gui_cancel.UseVisualStyleBackColor = true;
            this.button_frm_gui_cancel.Click += new System.EventHandler(this.button_frm_gui_cancel_Click_1);
            // 
            // label_gui_install_statut
            // 
            this.label_gui_install_statut.AutoSize = true;
            this.label_gui_install_statut.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_gui_install_statut.Location = new System.Drawing.Point(91, 27);
            this.label_gui_install_statut.Name = "label_gui_install_statut";
            this.label_gui_install_statut.Size = new System.Drawing.Size(249, 16);
            this.label_gui_install_statut.TabIndex = 9;
            this.label_gui_install_statut.Text = "Install PluginName VersionPlugin ?";
            // 
            // button_frm_gui_install
            // 
            this.button_frm_gui_install.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_frm_gui_install.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_frm_gui_install.Location = new System.Drawing.Point(233, 86);
            this.button_frm_gui_install.Name = "button_frm_gui_install";
            this.button_frm_gui_install.Size = new System.Drawing.Size(72, 23);
            this.button_frm_gui_install.TabIndex = 8;
            this.button_frm_gui_install.Text = "Install";
            this.button_frm_gui_install.UseVisualStyleBackColor = true;
            this.button_frm_gui_install.Click += new System.EventHandler(this.button_frm_gui_install_Click_1);
            // 
            // Gui_Install
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(398, 270);
            this.Controls.Add(this.ImagePictureBox_frm_gui_install);
            this.Controls.Add(this.progressBar_frm_gui_install);
            this.Controls.Add(this.label_gui_install_author);
            this.Controls.Add(this.richTextBox_gui_install);
            this.Controls.Add(this.button_frm_gui_cancel);
            this.Controls.Add(this.label_gui_install_statut);
            this.Controls.Add(this.button_frm_gui_install);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Gui_Install";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Plugin Installer";
            ((System.ComponentModel.ISupportInitialize)(this.ImagePictureBox_frm_gui_install)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox ImagePictureBox_frm_gui_install;
        private System.Windows.Forms.ProgressBar progressBar_frm_gui_install;
        private System.Windows.Forms.Label label_gui_install_author;
        private System.Windows.Forms.RichTextBox richTextBox_gui_install;
        private System.Windows.Forms.Button button_frm_gui_cancel;
        private System.Windows.Forms.Label label_gui_install_statut;
        private System.Windows.Forms.Button button_frm_gui_install;

    }
}