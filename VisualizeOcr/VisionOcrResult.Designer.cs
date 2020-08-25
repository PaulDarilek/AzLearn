using System;

namespace VisualizeOcr
{
    partial class VisionOcrResult
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblPage = new System.Windows.Forms.Label();
            this.udPage = new System.Windows.Forms.NumericUpDown();
            this.lblLine = new System.Windows.Forms.Label();
            this.udLine = new System.Windows.Forms.NumericUpDown();
            this.lblWord = new System.Windows.Forms.Label();
            this.udWord = new System.Windows.Forms.NumericUpDown();
            this.txtWord = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pic = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.udPage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udLine)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udWord)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic)).BeginInit();
            this.SuspendLayout();
            // 
            // lblPage
            // 
            this.lblPage.AutoSize = true;
            this.lblPage.Location = new System.Drawing.Point(13, 5);
            this.lblPage.Name = "lblPage";
            this.lblPage.Size = new System.Drawing.Size(33, 15);
            this.lblPage.TabIndex = 0;
            this.lblPage.Text = "Page";
            // 
            // udPage
            // 
            this.udPage.Location = new System.Drawing.Point(13, 23);
            this.udPage.Name = "udPage";
            this.udPage.Size = new System.Drawing.Size(58, 23);
            this.udPage.TabIndex = 1;
            this.udPage.ValueChanged += new System.EventHandler(this.udPage_ValueChanged);
            // 
            // lblLine
            // 
            this.lblLine.AutoSize = true;
            this.lblLine.Location = new System.Drawing.Point(89, 5);
            this.lblLine.Name = "lblLine";
            this.lblLine.Size = new System.Drawing.Size(29, 15);
            this.lblLine.TabIndex = 0;
            this.lblLine.Text = "Line";
            // 
            // udLine
            // 
            this.udLine.Location = new System.Drawing.Point(89, 23);
            this.udLine.Name = "udLine";
            this.udLine.Size = new System.Drawing.Size(58, 23);
            this.udLine.TabIndex = 1;
            this.udLine.ValueChanged += new System.EventHandler(this.udLine_ValueChanged);
            // 
            // lblWord
            // 
            this.lblWord.AutoSize = true;
            this.lblWord.Location = new System.Drawing.Point(166, 5);
            this.lblWord.Name = "lblWord";
            this.lblWord.Size = new System.Drawing.Size(36, 15);
            this.lblWord.TabIndex = 0;
            this.lblWord.Text = "Word";
            // 
            // udWord
            // 
            this.udWord.Location = new System.Drawing.Point(166, 23);
            this.udWord.Name = "udWord";
            this.udWord.Size = new System.Drawing.Size(58, 23);
            this.udWord.TabIndex = 1;
            this.udWord.ValueChanged += new System.EventHandler(this.udWord_ValueChanged);
            // 
            // txtWord
            // 
            this.txtWord.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWord.Location = new System.Drawing.Point(231, 23);
            this.txtWord.Name = "txtWord";
            this.txtWord.Size = new System.Drawing.Size(679, 23);
            this.txtWord.TabIndex = 3;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.pic);
            this.panel1.Location = new System.Drawing.Point(13, 52);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(897, 499);
            this.panel1.TabIndex = 4;
            // 
            // pic
            // 
            this.pic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pic.Location = new System.Drawing.Point(0, 0);
            this.pic.Name = "pic";
            this.pic.Size = new System.Drawing.Size(481, 188);
            this.pic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pic.TabIndex = 2;
            this.pic.TabStop = false;
            // 
            // VisionOcrResult
            // 
            this.ClientSize = new System.Drawing.Size(922, 563);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.txtWord);
            this.Controls.Add(this.udWord);
            this.Controls.Add(this.lblWord);
            this.Controls.Add(this.udLine);
            this.Controls.Add(this.lblLine);
            this.Controls.Add(this.udPage);
            this.Controls.Add(this.lblPage);
            this.Name = "VisionOcrResult";
            this.Load += new System.EventHandler(this.VisionOcrResult_Load);
            ((System.ComponentModel.ISupportInitialize)(this.udPage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udLine)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udWord)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblPage;
        private System.Windows.Forms.NumericUpDown udPage;
        private System.Windows.Forms.Label lblLine;
        private System.Windows.Forms.NumericUpDown udLine;
        private System.Windows.Forms.Label lblWord;
        private System.Windows.Forms.NumericUpDown udWord;
        private System.Windows.Forms.TextBox txtWord;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pic;
    }
}

