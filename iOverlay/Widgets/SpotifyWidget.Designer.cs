namespace iOverlay.Widgets
{
    partial class SpotifyWidget
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SpotifyWidget));
            this.songNameLabel = new Bunifu.UI.WinForms.BunifuLabel();
            this.artistNameLabel = new Bunifu.UI.WinForms.BunifuLabel();
            this.bunifuProgressBar1 = new Bunifu.UI.WinForms.BunifuProgressBar();
            this.albumArt = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // songNameLabel
            // 
            this.songNameLabel.AllowParentOverrides = false;
            this.songNameLabel.AutoEllipsis = false;
            this.songNameLabel.Cursor = System.Windows.Forms.Cursors.Default;
            this.songNameLabel.CursorType = System.Windows.Forms.Cursors.Default;
            this.songNameLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.songNameLabel.Location = new System.Drawing.Point(77, 8);
            this.songNameLabel.Name = "songNameLabel";
            this.songNameLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.songNameLabel.Size = new System.Drawing.Size(75, 21);
            this.songNameLabel.TabIndex = 2;
            this.songNameLabel.Text = "Paralyzed";
            this.songNameLabel.TextAlignment = System.Drawing.ContentAlignment.TopLeft;
            this.songNameLabel.TextFormat = Bunifu.UI.WinForms.BunifuLabel.TextFormattingOptions.Default;
            // 
            // artistNameLabel
            // 
            this.artistNameLabel.AllowParentOverrides = false;
            this.artistNameLabel.AutoEllipsis = false;
            this.artistNameLabel.Cursor = System.Windows.Forms.Cursors.Default;
            this.artistNameLabel.CursorType = System.Windows.Forms.Cursors.Default;
            this.artistNameLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.artistNameLabel.Location = new System.Drawing.Point(79, 32);
            this.artistNameLabel.Name = "artistNameLabel";
            this.artistNameLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.artistNameLabel.Size = new System.Drawing.Size(32, 15);
            this.artistNameLabel.TabIndex = 3;
            this.artistNameLabel.Text = "Sueco";
            this.artistNameLabel.TextAlignment = System.Drawing.ContentAlignment.TopLeft;
            this.artistNameLabel.TextFormat = Bunifu.UI.WinForms.BunifuLabel.TextFormattingOptions.Default;
            // 
            // bunifuProgressBar1
            // 
            this.bunifuProgressBar1.AllowAnimations = false;
            this.bunifuProgressBar1.Animation = 0;
            this.bunifuProgressBar1.AnimationSpeed = 220;
            this.bunifuProgressBar1.AnimationStep = 10;
            this.bunifuProgressBar1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(223)))), ((int)(((byte)(223)))));
            this.bunifuProgressBar1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("bunifuProgressBar1.BackgroundImage")));
            this.bunifuProgressBar1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(223)))), ((int)(((byte)(223)))));
            this.bunifuProgressBar1.BorderRadius = 9;
            this.bunifuProgressBar1.BorderThickness = 1;
            this.bunifuProgressBar1.Location = new System.Drawing.Point(75, 53);
            this.bunifuProgressBar1.Maximum = 100;
            this.bunifuProgressBar1.MaximumValue = 100;
            this.bunifuProgressBar1.Minimum = 0;
            this.bunifuProgressBar1.MinimumValue = 0;
            this.bunifuProgressBar1.Name = "bunifuProgressBar1";
            this.bunifuProgressBar1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.bunifuProgressBar1.ProgressBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(223)))), ((int)(((byte)(223)))));
            this.bunifuProgressBar1.ProgressColorLeft = System.Drawing.Color.DodgerBlue;
            this.bunifuProgressBar1.ProgressColorRight = System.Drawing.Color.DodgerBlue;
            this.bunifuProgressBar1.Size = new System.Drawing.Size(266, 13);
            this.bunifuProgressBar1.TabIndex = 0;
            this.bunifuProgressBar1.Value = 50;
            this.bunifuProgressBar1.ValueByTransition = 50;
            // 
            // albumArt
            // 
            this.albumArt.BackColor = System.Drawing.Color.Black;
            this.albumArt.BackgroundImage = global::iOverlay.Properties.Resources.Spotify_wziyAHw3le;
            this.albumArt.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.albumArt.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.albumArt.FlatAppearance.BorderSize = 0;
            this.albumArt.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this.albumArt.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.albumArt.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.albumArt.Font = new System.Drawing.Font("Segoe MDL2 Assets", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.albumArt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.albumArt.Location = new System.Drawing.Point(0, 0);
            this.albumArt.Name = "albumArt";
            this.albumArt.Size = new System.Drawing.Size(70, 70);
            this.albumArt.TabIndex = 6;
            this.albumArt.Text = "\xE768";
            this.albumArt.UseVisualStyleBackColor = false;
            // 
            // SpotifyWidget
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(365, 70);
            this.Controls.Add(this.albumArt);
            this.Controls.Add(this.artistNameLabel);
            this.Controls.Add(this.songNameLabel);
            this.Controls.Add(this.bunifuProgressBar1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SpotifyWidget";
            this.Text = "SpotifyWidget";
            this.Load += new System.EventHandler(this.SpotifyWidget_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Bunifu.UI.WinForms.BunifuProgressBar bunifuProgressBar1;
        private Bunifu.UI.WinForms.BunifuLabel songNameLabel;
        private Bunifu.UI.WinForms.BunifuLabel artistNameLabel;
        private System.Windows.Forms.Button albumArt;
    }
}