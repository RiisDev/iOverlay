using System.Windows.Forms;

namespace iOverlay.Widgets
{
    partial class ValorantWidget
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
            this.artistNameLabel = new System.Windows.Forms.Label();
            this.rankNameLabel = new System.Windows.Forms.Label();
            this.bunifuLabel1 = new System.Windows.Forms.Label();
            this.winPctLabel = new System.Windows.Forms.Label();
            this.KDLabel = new System.Windows.Forms.Label();
            this.bunifuLabel4 = new System.Windows.Forms.Label();
            this.rankRRProgress = new Bunifu.UI.WinForms.BunifuCircleProgress();
            this.rankIcon = new Bunifu.UI.WinForms.BunifuPictureBox();
            this.webView = new Microsoft.Web.WebView2.WinForms.WebView2();
            ((System.ComponentModel.ISupportInitialize)(this.rankIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.webView)).BeginInit();
            this.SuspendLayout();
            // 
            // artistNameLabel
            // 
            this.artistNameLabel.Cursor = System.Windows.Forms.Cursors.Default;
            this.artistNameLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.artistNameLabel.ForeColor = System.Drawing.Color.White;
            this.artistNameLabel.Location = new System.Drawing.Point(78, 12);
            this.artistNameLabel.Name = "artistNameLabel";
            this.artistNameLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.artistNameLabel.Size = new System.Drawing.Size(64, 15);
            this.artistNameLabel.TabIndex = 5;
            this.artistNameLabel.Text = "RANK";
            // 
            // rankNameLabel
            // 
            this.rankNameLabel.Cursor = System.Windows.Forms.Cursors.Default;
            this.rankNameLabel.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rankNameLabel.ForeColor = System.Drawing.Color.White;
            this.rankNameLabel.Location = new System.Drawing.Point(78, 26);
            this.rankNameLabel.Name = "rankNameLabel";
            this.rankNameLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.rankNameLabel.Size = new System.Drawing.Size(161, 30);
            this.rankNameLabel.TabIndex = 4;
            this.rankNameLabel.Text = "UNRAKED";
            // 
            // bunifuLabel1
            // 
            this.bunifuLabel1.Cursor = System.Windows.Forms.Cursors.Default;
            this.bunifuLabel1.Font = new System.Drawing.Font("Segoe UI Semibold", 8F, System.Drawing.FontStyle.Bold);
            this.bunifuLabel1.ForeColor = System.Drawing.Color.White;
            this.bunifuLabel1.Location = new System.Drawing.Point(235, 12);
            this.bunifuLabel1.Name = "bunifuLabel1";
            this.bunifuLabel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.bunifuLabel1.Size = new System.Drawing.Size(48, 13);
            this.bunifuLabel1.TabIndex = 6;
            this.bunifuLabel1.Text = "WIN %";
            // 
            // winPctLabel
            // 
            this.winPctLabel.Cursor = System.Windows.Forms.Cursors.Default;
            this.winPctLabel.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.winPctLabel.ForeColor = System.Drawing.Color.White;
            this.winPctLabel.Location = new System.Drawing.Point(235, 26);
            this.winPctLabel.Name = "winPctLabel";
            this.winPctLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.winPctLabel.Size = new System.Drawing.Size(48, 23);
            this.winPctLabel.TabIndex = 7;
            this.winPctLabel.Text = "0%";
            // 
            // KDLabel
            // 
            this.KDLabel.Cursor = System.Windows.Forms.Cursors.Default;
            this.KDLabel.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.KDLabel.ForeColor = System.Drawing.Color.White;
            this.KDLabel.Location = new System.Drawing.Point(286, 26);
            this.KDLabel.Name = "KDLabel";
            this.KDLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.KDLabel.Size = new System.Drawing.Size(60, 23);
            this.KDLabel.TabIndex = 9;
            this.KDLabel.Text = "0";
            // 
            // bunifuLabel4
            // 
            this.bunifuLabel4.Cursor = System.Windows.Forms.Cursors.Default;
            this.bunifuLabel4.Font = new System.Drawing.Font("Segoe UI Semibold", 8F, System.Drawing.FontStyle.Bold);
            this.bunifuLabel4.ForeColor = System.Drawing.Color.White;
            this.bunifuLabel4.Location = new System.Drawing.Point(289, 12);
            this.bunifuLabel4.Name = "bunifuLabel4";
            this.bunifuLabel4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.bunifuLabel4.Size = new System.Drawing.Size(41, 13);
            this.bunifuLabel4.TabIndex = 8;
            this.bunifuLabel4.Text = "K/D";
            // 
            // rankRRProgress
            // 
            this.rankRRProgress.Animated = false;
            this.rankRRProgress.AnimationInterval = 1;
            this.rankRRProgress.AnimationSpeed = 1;
            this.rankRRProgress.BackColor = System.Drawing.Color.Transparent;
            this.rankRRProgress.CircleMargin = 5;
            this.rankRRProgress.Dock = System.Windows.Forms.DockStyle.Left;
            this.rankRRProgress.Font = new System.Drawing.Font("Microsoft Sans Serif", 1.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rankRRProgress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.rankRRProgress.IsPercentage = false;
            this.rankRRProgress.LineProgressThickness = 5;
            this.rankRRProgress.LineThickness = 5;
            this.rankRRProgress.Location = new System.Drawing.Point(0, 0);
            this.rankRRProgress.Name = "rankRRProgress";
            this.rankRRProgress.ProgressAnimationSpeed = 200;
            this.rankRRProgress.ProgressBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(107)))), ((int)(((byte)(90)))));
            this.rankRRProgress.ProgressColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(214)))), ((int)(((byte)(180)))));
            this.rankRRProgress.ProgressColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(214)))), ((int)(((byte)(180)))));
            this.rankRRProgress.ProgressEndCap = Bunifu.UI.WinForms.BunifuCircleProgress.CapStyles.Round;
            this.rankRRProgress.ProgressFillStyle = Bunifu.UI.WinForms.BunifuCircleProgress.FillStyles.Solid;
            this.rankRRProgress.ProgressStartCap = Bunifu.UI.WinForms.BunifuCircleProgress.CapStyles.Round;
            this.rankRRProgress.SecondaryFont = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.rankRRProgress.Size = new System.Drawing.Size(72, 72);
            this.rankRRProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.rankRRProgress.SubScriptColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(166)))), ((int)(((byte)(166)))));
            this.rankRRProgress.SubScriptMargin = new System.Windows.Forms.Padding(5, -20, 0, 0);
            this.rankRRProgress.SubScriptText = "";
            this.rankRRProgress.SuperScriptColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(166)))), ((int)(((byte)(166)))));
            this.rankRRProgress.SuperScriptMargin = new System.Windows.Forms.Padding(5, 20, 0, 0);
            this.rankRRProgress.SuperScriptText = "";
            this.rankRRProgress.TabIndex = 16;
            this.rankRRProgress.Text = "30";
            this.rankRRProgress.TextMargin = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.rankRRProgress.Value = 30;
            this.rankRRProgress.ValueByTransition = 30;
            this.rankRRProgress.ValueMargin = new System.Windows.Forms.Padding(0, 5, 0, 0);
            // 
            // rankIcon
            // 
            this.rankIcon.AllowFocused = false;
            this.rankIcon.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.rankIcon.AutoSizeHeight = true;
            this.rankIcon.BorderRadius = 26;
            this.rankIcon.Image = global::iOverlay.Properties.Resources.Unranked;
            this.rankIcon.IsCircle = true;
            this.rankIcon.Location = new System.Drawing.Point(10, 10);
            this.rankIcon.Name = "rankIcon";
            this.rankIcon.Size = new System.Drawing.Size(53, 53);
            this.rankIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.rankIcon.TabIndex = 17;
            this.rankIcon.TabStop = false;
            this.rankIcon.Type = Bunifu.UI.WinForms.BunifuPictureBox.Types.Circle;
            // 
            // webView
            // 
            this.webView.AllowExternalDrop = true;
            this.webView.CreationProperties = null;
            this.webView.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webView.Location = new System.Drawing.Point(0, 0);
            this.webView.Name = "webView";
            this.webView.Size = new System.Drawing.Size(0, 0);
            this.webView.TabIndex = 18;
            this.webView.ZoomFactor = 1D;
            // 
            // ValorantWidget
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.ClientSize = new System.Drawing.Size(334, 72);
            this.Controls.Add(this.webView);
            this.Controls.Add(this.rankIcon);
            this.Controls.Add(this.KDLabel);
            this.Controls.Add(this.bunifuLabel4);
            this.Controls.Add(this.winPctLabel);
            this.Controls.Add(this.bunifuLabel1);
            this.Controls.Add(this.artistNameLabel);
            this.Controls.Add(this.rankNameLabel);
            this.Controls.Add(this.rankRRProgress);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ValorantWidget";
            this.Opacity = 0.8D;
            this.Text = "ValorantWidget";
            this.Load += new System.EventHandler(this.ValorantWidget_Load);
            ((System.ComponentModel.ISupportInitialize)(this.rankIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.webView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private Label artistNameLabel;
        private Label rankNameLabel;
        private Label bunifuLabel1;
        private Label winPctLabel;
        private Label KDLabel;
        private Label bunifuLabel4;
        private Bunifu.UI.WinForms.BunifuCircleProgress rankRRProgress;
        private Bunifu.UI.WinForms.BunifuPictureBox rankIcon;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;
    }
}