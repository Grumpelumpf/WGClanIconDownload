using System.Windows.Forms;

namespace WGClanIconDownload
{
    public partial class MainWindow : Form
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.progressLabel = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.checkedListBoxRegion = new System.Windows.Forms.CheckedListBox();
            this.clanIconPreview_PictureBox1 = new System.Windows.Forms.PictureBox();
            this.buttonStart = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.progressBar2 = new System.Windows.Forms.ProgressBar();
            this.progressBar3 = new System.Windows.Forms.ProgressBar();
            this.progressBar4 = new System.Windows.Forms.ProgressBar();
            this.progressName_Label1 = new System.Windows.Forms.Label();
            this.progressName_Label2 = new System.Windows.Forms.Label();
            this.progressName_Label3 = new System.Windows.Forms.Label();
            this.progressName_Label4 = new System.Windows.Forms.Label();
            this.clanIconPreview_PictureBox2 = new System.Windows.Forms.PictureBox();
            this.clanIconPreview_PictureBox3 = new System.Windows.Forms.PictureBox();
            this.clanIconPreview_PictureBox4 = new System.Windows.Forms.PictureBox();
            this.threads_numericUpDown = new System.Windows.Forms.NumericUpDown();
            this.progressPage_Label1 = new System.Windows.Forms.Label();
            this.progressPage_Label4 = new System.Windows.Forms.Label();
            this.progressPage_Label3 = new System.Windows.Forms.Label();
            this.progressPage_Label2 = new System.Windows.Forms.Label();
            this.showThreadCount_Label1 = new System.Windows.Forms.Label();
            this.showThreadCount_Label2 = new System.Windows.Forms.Label();
            this.showThreadCount_Label3 = new System.Windows.Forms.Label();
            this.showThreadCount_Label4 = new System.Windows.Forms.Label();
            this.downloadCounter_Label1 = new System.Windows.Forms.Label();
            this.downloadCounter_Label2 = new System.Windows.Forms.Label();
            this.downloadCounter_Label3 = new System.Windows.Forms.Label();
            this.downloadCounter_Label4 = new System.Windows.Forms.Label();
            this.dump_button = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.clanIconPreview_PictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.clanIconPreview_PictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.clanIconPreview_PictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.clanIconPreview_PictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.threads_numericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // progressLabel
            // 
            this.progressLabel.AutoSize = true;
            this.progressLabel.Location = new System.Drawing.Point(21, 170);
            this.progressLabel.Name = "progressLabel";
            this.progressLabel.Size = new System.Drawing.Size(73, 13);
            this.progressLabel.TabIndex = 0;
            this.progressLabel.Text = "progressLabel";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(24, 31);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(241, 24);
            this.progressBar1.TabIndex = 1;
            // 
            // checkedListBoxRegion
            // 
            this.checkedListBoxRegion.FormattingEnabled = true;
            this.checkedListBoxRegion.Location = new System.Drawing.Point(24, 192);
            this.checkedListBoxRegion.Name = "checkedListBoxRegion";
            this.checkedListBoxRegion.Size = new System.Drawing.Size(135, 94);
            this.checkedListBoxRegion.Sorted = true;
            this.checkedListBoxRegion.TabIndex = 2;
            // 
            // clanIconPreview_PictureBox1
            // 
            this.clanIconPreview_PictureBox1.Location = new System.Drawing.Point(375, 23);
            this.clanIconPreview_PictureBox1.Margin = new System.Windows.Forms.Padding(0);
            this.clanIconPreview_PictureBox1.Name = "clanIconPreview_PictureBox1";
            this.clanIconPreview_PictureBox1.Size = new System.Drawing.Size(32, 32);
            this.clanIconPreview_PictureBox1.TabIndex = 3;
            this.clanIconPreview_PictureBox1.TabStop = false;
            // 
            // buttonStart
            // 
            this.buttonStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonStart.Location = new System.Drawing.Point(275, 234);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(75, 23);
            this.buttonStart.TabIndex = 4;
            this.buttonStart.Text = "buttonStart";
            this.buttonStart.UseMnemonic = false;
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Location = new System.Drawing.Point(275, 263);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(75, 23);
            this.buttonStop.TabIndex = 5;
            this.buttonStop.Text = "buttonStop";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // progressBar2
            // 
            this.progressBar2.Location = new System.Drawing.Point(24, 63);
            this.progressBar2.Name = "progressBar2";
            this.progressBar2.Size = new System.Drawing.Size(241, 24);
            this.progressBar2.TabIndex = 6;
            // 
            // progressBar3
            // 
            this.progressBar3.Location = new System.Drawing.Point(24, 95);
            this.progressBar3.Name = "progressBar3";
            this.progressBar3.Size = new System.Drawing.Size(241, 24);
            this.progressBar3.TabIndex = 7;
            // 
            // progressBar4
            // 
            this.progressBar4.Location = new System.Drawing.Point(24, 127);
            this.progressBar4.Name = "progressBar4";
            this.progressBar4.Size = new System.Drawing.Size(241, 24);
            this.progressBar4.TabIndex = 8;
            // 
            // progressName_Label1
            // 
            this.progressName_Label1.AutoSize = true;
            this.progressName_Label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.progressName_Label1.Location = new System.Drawing.Point(271, 31);
            this.progressName_Label1.Name = "progressName_Label1";
            this.progressName_Label1.Size = new System.Drawing.Size(51, 20);
            this.progressName_Label1.TabIndex = 9;
            this.progressName_Label1.Text = "label1";
            // 
            // progressName_Label2
            // 
            this.progressName_Label2.AutoSize = true;
            this.progressName_Label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.progressName_Label2.Location = new System.Drawing.Point(271, 63);
            this.progressName_Label2.Name = "progressName_Label2";
            this.progressName_Label2.Size = new System.Drawing.Size(51, 20);
            this.progressName_Label2.TabIndex = 10;
            this.progressName_Label2.Text = "label2";
            // 
            // progressName_Label3
            // 
            this.progressName_Label3.AutoSize = true;
            this.progressName_Label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.progressName_Label3.Location = new System.Drawing.Point(271, 95);
            this.progressName_Label3.Name = "progressName_Label3";
            this.progressName_Label3.Size = new System.Drawing.Size(51, 20);
            this.progressName_Label3.TabIndex = 11;
            this.progressName_Label3.Text = "label3";
            // 
            // progressName_Label4
            // 
            this.progressName_Label4.AutoSize = true;
            this.progressName_Label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.progressName_Label4.Location = new System.Drawing.Point(271, 127);
            this.progressName_Label4.Name = "progressName_Label4";
            this.progressName_Label4.Size = new System.Drawing.Size(51, 20);
            this.progressName_Label4.TabIndex = 12;
            this.progressName_Label4.Text = "label4";
            // 
            // clanIconPreview_PictureBox2
            // 
            this.clanIconPreview_PictureBox2.Location = new System.Drawing.Point(375, 55);
            this.clanIconPreview_PictureBox2.Margin = new System.Windows.Forms.Padding(0);
            this.clanIconPreview_PictureBox2.Name = "clanIconPreview_PictureBox2";
            this.clanIconPreview_PictureBox2.Size = new System.Drawing.Size(32, 32);
            this.clanIconPreview_PictureBox2.TabIndex = 13;
            this.clanIconPreview_PictureBox2.TabStop = false;
            // 
            // clanIconPreview_PictureBox3
            // 
            this.clanIconPreview_PictureBox3.Location = new System.Drawing.Point(375, 87);
            this.clanIconPreview_PictureBox3.Margin = new System.Windows.Forms.Padding(0);
            this.clanIconPreview_PictureBox3.Name = "clanIconPreview_PictureBox3";
            this.clanIconPreview_PictureBox3.Size = new System.Drawing.Size(32, 32);
            this.clanIconPreview_PictureBox3.TabIndex = 14;
            this.clanIconPreview_PictureBox3.TabStop = false;
            // 
            // clanIconPreview_PictureBox4
            // 
            this.clanIconPreview_PictureBox4.Location = new System.Drawing.Point(375, 119);
            this.clanIconPreview_PictureBox4.Margin = new System.Windows.Forms.Padding(0);
            this.clanIconPreview_PictureBox4.Name = "clanIconPreview_PictureBox4";
            this.clanIconPreview_PictureBox4.Size = new System.Drawing.Size(32, 32);
            this.clanIconPreview_PictureBox4.TabIndex = 15;
            this.clanIconPreview_PictureBox4.TabStop = false;
            // 
            // threads_numericUpDown
            // 
            this.threads_numericUpDown.Location = new System.Drawing.Point(275, 192);
            this.threads_numericUpDown.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.threads_numericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.threads_numericUpDown.Name = "threads_numericUpDown";
            this.threads_numericUpDown.Size = new System.Drawing.Size(38, 20);
            this.threads_numericUpDown.TabIndex = 16;
            this.threads_numericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.threads_numericUpDown.ValueChanged += new System.EventHandler(this.threads_numericUpDown_ValueChanged);
            // 
            // progressPage_Label1
            // 
            this.progressPage_Label1.AutoSize = true;
            this.progressPage_Label1.Location = new System.Drawing.Point(420, 36);
            this.progressPage_Label1.Name = "progressPage_Label1";
            this.progressPage_Label1.Size = new System.Drawing.Size(24, 13);
            this.progressPage_Label1.TabIndex = 17;
            this.progressPage_Label1.Text = "0/0";
            // 
            // progressPage_Label4
            // 
            this.progressPage_Label4.AutoSize = true;
            this.progressPage_Label4.Location = new System.Drawing.Point(420, 132);
            this.progressPage_Label4.Name = "progressPage_Label4";
            this.progressPage_Label4.Size = new System.Drawing.Size(24, 13);
            this.progressPage_Label4.TabIndex = 18;
            this.progressPage_Label4.Text = "0/0";
            // 
            // progressPage_Label3
            // 
            this.progressPage_Label3.AutoSize = true;
            this.progressPage_Label3.Location = new System.Drawing.Point(420, 100);
            this.progressPage_Label3.Name = "progressPage_Label3";
            this.progressPage_Label3.Size = new System.Drawing.Size(24, 13);
            this.progressPage_Label3.TabIndex = 19;
            this.progressPage_Label3.Text = "0/0";
            // 
            // progressPage_Label2
            // 
            this.progressPage_Label2.AutoSize = true;
            this.progressPage_Label2.Location = new System.Drawing.Point(420, 70);
            this.progressPage_Label2.Name = "progressPage_Label2";
            this.progressPage_Label2.Size = new System.Drawing.Size(24, 13);
            this.progressPage_Label2.TabIndex = 20;
            this.progressPage_Label2.Text = "0/0";
            // 
            // showThreadCount_Label1
            // 
            this.showThreadCount_Label1.AutoSize = true;
            this.showThreadCount_Label1.Location = new System.Drawing.Point(328, 36);
            this.showThreadCount_Label1.Name = "showThreadCount_Label1";
            this.showThreadCount_Label1.Size = new System.Drawing.Size(13, 13);
            this.showThreadCount_Label1.TabIndex = 21;
            this.showThreadCount_Label1.Text = "0";
            // 
            // showThreadCount_Label2
            // 
            this.showThreadCount_Label2.AutoSize = true;
            this.showThreadCount_Label2.Location = new System.Drawing.Point(328, 68);
            this.showThreadCount_Label2.Name = "showThreadCount_Label2";
            this.showThreadCount_Label2.Size = new System.Drawing.Size(13, 13);
            this.showThreadCount_Label2.TabIndex = 22;
            this.showThreadCount_Label2.Text = "0";
            // 
            // showThreadCount_Label3
            // 
            this.showThreadCount_Label3.AutoSize = true;
            this.showThreadCount_Label3.Location = new System.Drawing.Point(328, 100);
            this.showThreadCount_Label3.Name = "showThreadCount_Label3";
            this.showThreadCount_Label3.Size = new System.Drawing.Size(13, 13);
            this.showThreadCount_Label3.TabIndex = 23;
            this.showThreadCount_Label3.Text = "0";
            // 
            // showThreadCount_Label4
            // 
            this.showThreadCount_Label4.AutoSize = true;
            this.showThreadCount_Label4.Location = new System.Drawing.Point(328, 132);
            this.showThreadCount_Label4.Name = "showThreadCount_Label4";
            this.showThreadCount_Label4.Size = new System.Drawing.Size(13, 13);
            this.showThreadCount_Label4.TabIndex = 24;
            this.showThreadCount_Label4.Text = "0";
            // 
            // downloadCounter_Label1
            // 
            this.downloadCounter_Label1.AutoSize = true;
            this.downloadCounter_Label1.Location = new System.Drawing.Point(485, 36);
            this.downloadCounter_Label1.Name = "downloadCounter_Label1";
            this.downloadCounter_Label1.Size = new System.Drawing.Size(0, 13);
            this.downloadCounter_Label1.TabIndex = 25;
            // 
            // downloadCounter_Label2
            // 
            this.downloadCounter_Label2.AutoSize = true;
            this.downloadCounter_Label2.Location = new System.Drawing.Point(485, 70);
            this.downloadCounter_Label2.Name = "downloadCounter_Label2";
            this.downloadCounter_Label2.Size = new System.Drawing.Size(0, 13);
            this.downloadCounter_Label2.TabIndex = 26;
            // 
            // downloadCounter_Label3
            // 
            this.downloadCounter_Label3.AutoSize = true;
            this.downloadCounter_Label3.Location = new System.Drawing.Point(485, 100);
            this.downloadCounter_Label3.Name = "downloadCounter_Label3";
            this.downloadCounter_Label3.Size = new System.Drawing.Size(0, 13);
            this.downloadCounter_Label3.TabIndex = 27;
            // 
            // downloadCounter_Label4
            // 
            this.downloadCounter_Label4.AutoSize = true;
            this.downloadCounter_Label4.Location = new System.Drawing.Point(485, 132);
            this.downloadCounter_Label4.Name = "downloadCounter_Label4";
            this.downloadCounter_Label4.Size = new System.Drawing.Size(0, 13);
            this.downloadCounter_Label4.TabIndex = 28;
            // 
            // dump_button
            // 
            this.dump_button.Location = new System.Drawing.Point(416, 235);
            this.dump_button.Name = "dump_button";
            this.dump_button.Size = new System.Drawing.Size(68, 21);
            this.dump_button.TabIndex = 29;
            this.dump_button.Text = "Dump";
            this.dump_button.UseVisualStyleBackColor = true;
            this.dump_button.Click += new System.EventHandler(this.dump_button_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(599, 308);
            this.Controls.Add(this.dump_button);
            this.Controls.Add(this.downloadCounter_Label4);
            this.Controls.Add(this.downloadCounter_Label3);
            this.Controls.Add(this.downloadCounter_Label2);
            this.Controls.Add(this.downloadCounter_Label1);
            this.Controls.Add(this.showThreadCount_Label4);
            this.Controls.Add(this.showThreadCount_Label3);
            this.Controls.Add(this.showThreadCount_Label2);
            this.Controls.Add(this.showThreadCount_Label1);
            this.Controls.Add(this.progressPage_Label2);
            this.Controls.Add(this.progressPage_Label3);
            this.Controls.Add(this.progressPage_Label4);
            this.Controls.Add(this.progressPage_Label1);
            this.Controls.Add(this.threads_numericUpDown);
            this.Controls.Add(this.clanIconPreview_PictureBox4);
            this.Controls.Add(this.clanIconPreview_PictureBox3);
            this.Controls.Add(this.clanIconPreview_PictureBox2);
            this.Controls.Add(this.progressName_Label4);
            this.Controls.Add(this.progressName_Label3);
            this.Controls.Add(this.progressName_Label2);
            this.Controls.Add(this.progressName_Label1);
            this.Controls.Add(this.progressBar4);
            this.Controls.Add(this.progressBar3);
            this.Controls.Add(this.progressBar2);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.clanIconPreview_PictureBox1);
            this.Controls.Add(this.checkedListBoxRegion);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.progressLabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainWindow";
            this.Text = "WG ClanIcon Downloader";
            ((System.ComponentModel.ISupportInitialize)(this.clanIconPreview_PictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.clanIconPreview_PictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.clanIconPreview_PictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.clanIconPreview_PictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.threads_numericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label progressLabel;
        private System.Windows.Forms.ProgressBar progressBar1;
        private CheckedListBox checkedListBoxRegion;
        private PictureBox clanIconPreview_PictureBox1;
        private Button buttonStart;
        private Button buttonStop;
        private ProgressBar progressBar2;
        private ProgressBar progressBar3;
        private ProgressBar progressBar4;
        private Label progressName_Label1;
        private Label progressName_Label2;
        private Label progressName_Label3;
        private Label progressName_Label4;
        private PictureBox clanIconPreview_PictureBox2;
        private PictureBox clanIconPreview_PictureBox3;
        private PictureBox clanIconPreview_PictureBox4;
        private NumericUpDown threads_numericUpDown;
        private Label progressPage_Label1;
        private Label progressPage_Label4;
        private Label progressPage_Label3;
        private Label progressPage_Label2;
        private Label showThreadCount_Label1;
        private Label showThreadCount_Label2;
        private Label showThreadCount_Label3;
        private Label showThreadCount_Label4;
        private Label downloadCounter_Label1;
        private Label downloadCounter_Label2;
        private Label downloadCounter_Label3;
        private Label downloadCounter_Label4;
        private Button dump_button;
    }
}

