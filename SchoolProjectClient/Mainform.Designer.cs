namespace SchoolProjectClient
{
    partial class Mainform
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Mainform));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.StyleCombobox = new System.Windows.Forms.ComboBox();
            this.TweetRefreshButton = new System.Windows.Forms.Button();
            this.TweetFlowLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tableLayoutPanel1.BackgroundImage")));
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 8F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.StyleCombobox, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.TweetRefreshButton, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.TweetFlowLayout, 1, 3);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 8F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.904905F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 8F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 95.09509F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 8F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(484, 714);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // StyleCombobox
            // 
            this.StyleCombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.StyleCombobox.FormattingEnabled = true;
            this.StyleCombobox.Location = new System.Drawing.Point(166, 11);
            this.StyleCombobox.Name = "StyleCombobox";
            this.StyleCombobox.Size = new System.Drawing.Size(149, 21);
            this.StyleCombobox.Sorted = true;
            this.StyleCombobox.TabIndex = 0;
            this.StyleCombobox.SelectedIndexChanged += new System.EventHandler(this.StyleCombobox_SelectedIndexChanged);
            // 
            // TweetRefreshButton
            // 
            this.TweetRefreshButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TweetRefreshButton.Enabled = false;
            this.TweetRefreshButton.Location = new System.Drawing.Point(2, 2);
            this.TweetRefreshButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.TweetRefreshButton.Name = "TweetRefreshButton";
            this.TweetRefreshButton.Size = new System.Drawing.Size(4, 4);
            this.TweetRefreshButton.TabIndex = 2;
            this.TweetRefreshButton.Text = "Refresh tweets";
            this.TweetRefreshButton.UseVisualStyleBackColor = true;
            this.TweetRefreshButton.Click += new System.EventHandler(this.TweetRefreshButton_Click);
            // 
            // TweetFlowLayout
            // 
            this.TweetFlowLayout.AutoScroll = true;
            this.TweetFlowLayout.AutoScrollMinSize = new System.Drawing.Size(0, 700);
            this.TweetFlowLayout.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.TweetFlowLayout, 3);
            this.TweetFlowLayout.Location = new System.Drawing.Point(10, 51);
            this.TweetFlowLayout.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.TweetFlowLayout.MinimumSize = new System.Drawing.Size(461, 651);
            this.TweetFlowLayout.Name = "TweetFlowLayout";
            this.TweetFlowLayout.Size = new System.Drawing.Size(461, 651);
            this.TweetFlowLayout.TabIndex = 3;
            // 
            // Mainform
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 712);
            this.Controls.Add(this.tableLayoutPanel1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Mainform";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TTS Client";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Mainform_FormClosing);
            this.Load += new System.EventHandler(this.Mainform_Load);
            this.Shown += new System.EventHandler(this.Mainform_Shown);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ComboBox StyleCombobox;
        private System.Windows.Forms.Button TweetRefreshButton;
        private System.Windows.Forms.FlowLayoutPanel TweetFlowLayout;
    }
}

