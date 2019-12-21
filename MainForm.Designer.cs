namespace DailyDepositReader
{
    partial class MainForm
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
            this.btnRead = new System.Windows.Forms.Button();
            this.ctlStartDate = new System.Windows.Forms.DateTimePicker();
            this.ctlEndDate = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtFolder = new System.Windows.Forms.TextBox();
            this.lblCurrentFile = new System.Windows.Forms.Label();
            this.grdResults = new System.Windows.Forms.DataGridView();
            this.ctlFolderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.btnBrowseFolder = new System.Windows.Forms.Button();
            this.txtWeekDayPercents = new System.Windows.Forms.TextBox();
            this.chkShowAllWarnings = new System.Windows.Forms.CheckBox();
            this.ActivityDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DayOfWeek = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Weather = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Circumstances = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Net = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TrxCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AvgSale = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CardReg = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CardMach = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BankDep = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CoffeeTrx = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CoffeeCups = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CoffeePrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Gift = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BirdSeed = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Plant = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.General = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Seasonal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Water = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Soil = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Houseplant = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Tools = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Wreaths = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Seeds = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Fert = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Pots = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Pruners = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Chocolate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Bulbs = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Chemicals = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BirdFeeders = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Unused5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GiftCard = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NotVoided = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.grdResults)).BeginInit();
            this.SuspendLayout();
            // 
            // btnRead
            // 
            this.btnRead.Location = new System.Drawing.Point(122, 136);
            this.btnRead.Name = "btnRead";
            this.btnRead.Size = new System.Drawing.Size(175, 23);
            this.btnRead.TabIndex = 8;
            this.btnRead.Text = "Read Deposit Files";
            this.btnRead.UseVisualStyleBackColor = true;
            this.btnRead.Click += new System.EventHandler(this.btnRead_Click);
            // 
            // ctlStartDate
            // 
            this.ctlStartDate.Location = new System.Drawing.Point(122, 48);
            this.ctlStartDate.Name = "ctlStartDate";
            this.ctlStartDate.Size = new System.Drawing.Size(200, 20);
            this.ctlStartDate.TabIndex = 4;
            // 
            // ctlEndDate
            // 
            this.ctlEndDate.Location = new System.Drawing.Point(122, 74);
            this.ctlEndDate.Name = "ctlEndDate";
            this.ctlEndDate.Size = new System.Drawing.Size(200, 20);
            this.ctlEndDate.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Start Date:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "End Date:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Daily Deposit Folder:";
            // 
            // txtFolder
            // 
            this.txtFolder.Location = new System.Drawing.Point(122, 22);
            this.txtFolder.Name = "txtFolder";
            this.txtFolder.Size = new System.Drawing.Size(358, 20);
            this.txtFolder.TabIndex = 1;
            // 
            // lblCurrentFile
            // 
            this.lblCurrentFile.AutoSize = true;
            this.lblCurrentFile.Location = new System.Drawing.Point(119, 109);
            this.lblCurrentFile.Name = "lblCurrentFile";
            this.lblCurrentFile.Size = new System.Drawing.Size(62, 13);
            this.lblCurrentFile.TabIndex = 7;
            this.lblCurrentFile.Text = "(current file)";
            // 
            // grdResults
            // 
            this.grdResults.AllowUserToAddRows = false;
            this.grdResults.AllowUserToDeleteRows = false;
            this.grdResults.AllowUserToResizeRows = false;
            this.grdResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdResults.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ActivityDate,
            this.DayOfWeek,
            this.Weather,
            this.Circumstances,
            this.Net,
            this.TrxCount,
            this.AvgSale,
            this.CardReg,
            this.CardMach,
            this.BankDep,
            this.CoffeeTrx,
            this.CoffeeCups,
            this.CoffeePrice,
            this.Gift,
            this.BirdSeed,
            this.Plant,
            this.General,
            this.Seasonal,
            this.Water,
            this.Soil,
            this.Houseplant,
            this.Tools,
            this.Wreaths,
            this.Seeds,
            this.Fert,
            this.Pots,
            this.Pruners,
            this.Chocolate,
            this.Bulbs,
            this.Chemicals,
            this.BirdFeeders,
            this.Unused5,
            this.GiftCard,
            this.NotVoided});
            this.grdResults.Location = new System.Drawing.Point(15, 183);
            this.grdResults.Name = "grdResults";
            this.grdResults.ReadOnly = true;
            this.grdResults.Size = new System.Drawing.Size(978, 333);
            this.grdResults.TabIndex = 10;
            // 
            // btnBrowseFolder
            // 
            this.btnBrowseFolder.Location = new System.Drawing.Point(486, 20);
            this.btnBrowseFolder.Name = "btnBrowseFolder";
            this.btnBrowseFolder.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseFolder.TabIndex = 2;
            this.btnBrowseFolder.Text = "Browse...";
            this.btnBrowseFolder.UseVisualStyleBackColor = true;
            this.btnBrowseFolder.Click += new System.EventHandler(this.btnBrowseFolder_Click);
            // 
            // txtWeekDayPercents
            // 
            this.txtWeekDayPercents.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWeekDayPercents.Enabled = false;
            this.txtWeekDayPercents.Location = new System.Drawing.Point(695, 12);
            this.txtWeekDayPercents.Multiline = true;
            this.txtWeekDayPercents.Name = "txtWeekDayPercents";
            this.txtWeekDayPercents.Size = new System.Drawing.Size(298, 161);
            this.txtWeekDayPercents.TabIndex = 11;
            // 
            // chkShowAllWarnings
            // 
            this.chkShowAllWarnings.AutoSize = true;
            this.chkShowAllWarnings.Location = new System.Drawing.Point(330, 140);
            this.chkShowAllWarnings.Name = "chkShowAllWarnings";
            this.chkShowAllWarnings.Size = new System.Drawing.Size(116, 17);
            this.chkShowAllWarnings.TabIndex = 9;
            this.chkShowAllWarnings.Text = "Report all warnings";
            this.chkShowAllWarnings.UseVisualStyleBackColor = true;
            // 
            // ActivityDate
            // 
            this.ActivityDate.Frozen = true;
            this.ActivityDate.HeaderText = "Activity Date";
            this.ActivityDate.Name = "ActivityDate";
            this.ActivityDate.ReadOnly = true;
            this.ActivityDate.Width = 80;
            // 
            // DayOfWeek
            // 
            this.DayOfWeek.Frozen = true;
            this.DayOfWeek.HeaderText = "Day Of Week";
            this.DayOfWeek.Name = "DayOfWeek";
            this.DayOfWeek.ReadOnly = true;
            this.DayOfWeek.Width = 80;
            // 
            // Weather
            // 
            this.Weather.HeaderText = "Weather";
            this.Weather.Name = "Weather";
            this.Weather.ReadOnly = true;
            this.Weather.Width = 80;
            // 
            // Circumstances
            // 
            this.Circumstances.HeaderText = "Circumstances";
            this.Circumstances.Name = "Circumstances";
            this.Circumstances.ReadOnly = true;
            this.Circumstances.Width = 80;
            // 
            // Net
            // 
            this.Net.HeaderText = "Net Sales";
            this.Net.Name = "Net";
            this.Net.ReadOnly = true;
            // 
            // TrxCount
            // 
            this.TrxCount.HeaderText = "Trx Count";
            this.TrxCount.Name = "TrxCount";
            this.TrxCount.ReadOnly = true;
            // 
            // AvgSale
            // 
            this.AvgSale.HeaderText = "Avg Sale";
            this.AvgSale.Name = "AvgSale";
            this.AvgSale.ReadOnly = true;
            // 
            // CardReg
            // 
            this.CardReg.HeaderText = "Card Register";
            this.CardReg.Name = "CardReg";
            this.CardReg.ReadOnly = true;
            // 
            // CardMach
            // 
            this.CardMach.HeaderText = "Card Machine";
            this.CardMach.Name = "CardMach";
            this.CardMach.ReadOnly = true;
            // 
            // BankDep
            // 
            this.BankDep.HeaderText = "Cash & Checks";
            this.BankDep.Name = "BankDep";
            this.BankDep.ReadOnly = true;
            // 
            // CoffeeTrx
            // 
            this.CoffeeTrx.HeaderText = "Coffee Trx";
            this.CoffeeTrx.Name = "CoffeeTrx";
            this.CoffeeTrx.ReadOnly = true;
            // 
            // CoffeeCups
            // 
            this.CoffeeCups.HeaderText = "Coffee Cups";
            this.CoffeeCups.Name = "CoffeeCups";
            this.CoffeeCups.ReadOnly = true;
            // 
            // CoffeePrice
            // 
            this.CoffeePrice.HeaderText = "Coffee Price";
            this.CoffeePrice.Name = "CoffeePrice";
            this.CoffeePrice.ReadOnly = true;
            // 
            // Gift
            // 
            this.Gift.HeaderText = "Gift";
            this.Gift.Name = "Gift";
            this.Gift.ReadOnly = true;
            // 
            // BirdSeed
            // 
            this.BirdSeed.HeaderText = "Bird Seed";
            this.BirdSeed.Name = "BirdSeed";
            this.BirdSeed.ReadOnly = true;
            // 
            // Plant
            // 
            this.Plant.HeaderText = "Plant";
            this.Plant.Name = "Plant";
            this.Plant.ReadOnly = true;
            // 
            // General
            // 
            this.General.HeaderText = "General";
            this.General.Name = "General";
            this.General.ReadOnly = true;
            // 
            // Seasonal
            // 
            this.Seasonal.HeaderText = "Seasonal";
            this.Seasonal.Name = "Seasonal";
            this.Seasonal.ReadOnly = true;
            // 
            // Water
            // 
            this.Water.HeaderText = "Water";
            this.Water.Name = "Water";
            this.Water.ReadOnly = true;
            // 
            // Soil
            // 
            this.Soil.HeaderText = "Soil";
            this.Soil.Name = "Soil";
            this.Soil.ReadOnly = true;
            // 
            // Houseplant
            // 
            this.Houseplant.HeaderText = "Houseplant";
            this.Houseplant.Name = "Houseplant";
            this.Houseplant.ReadOnly = true;
            // 
            // Tools
            // 
            this.Tools.HeaderText = "Tools";
            this.Tools.Name = "Tools";
            this.Tools.ReadOnly = true;
            // 
            // Wreaths
            // 
            this.Wreaths.HeaderText = "Baked";
            this.Wreaths.Name = "Wreaths";
            this.Wreaths.ReadOnly = true;
            // 
            // Seeds
            // 
            this.Seeds.HeaderText = "Seeds";
            this.Seeds.Name = "Seeds";
            this.Seeds.ReadOnly = true;
            // 
            // Fert
            // 
            this.Fert.HeaderText = "Fert";
            this.Fert.Name = "Fert";
            this.Fert.ReadOnly = true;
            // 
            // Pots
            // 
            this.Pots.HeaderText = "Pots";
            this.Pots.Name = "Pots";
            this.Pots.ReadOnly = true;
            // 
            // Pruners
            // 
            this.Pruners.HeaderText = "Pruners";
            this.Pruners.Name = "Pruners";
            this.Pruners.ReadOnly = true;
            // 
            // Chocolate
            // 
            this.Chocolate.HeaderText = "Chocolate";
            this.Chocolate.Name = "Chocolate";
            this.Chocolate.ReadOnly = true;
            // 
            // Bulbs
            // 
            this.Bulbs.HeaderText = "Bulbs";
            this.Bulbs.Name = "Bulbs";
            this.Bulbs.ReadOnly = true;
            // 
            // Chemicals
            // 
            this.Chemicals.HeaderText = "Chemicals";
            this.Chemicals.Name = "Chemicals";
            this.Chemicals.ReadOnly = true;
            // 
            // BirdFeeders
            // 
            this.BirdFeeders.HeaderText = "Bird Feeders";
            this.BirdFeeders.Name = "BirdFeeders";
            this.BirdFeeders.ReadOnly = true;
            // 
            // Unused5
            // 
            this.Unused5.HeaderText = "Coffee";
            this.Unused5.Name = "Unused5";
            this.Unused5.ReadOnly = true;
            // 
            // GiftCard
            // 
            this.GiftCard.HeaderText = "Gift Card";
            this.GiftCard.Name = "GiftCard";
            this.GiftCard.ReadOnly = true;
            // 
            // NotVoided
            // 
            this.NotVoided.HeaderText = "Not Voided";
            this.NotVoided.Name = "NotVoided";
            this.NotVoided.ReadOnly = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1005, 528);
            this.Controls.Add(this.chkShowAllWarnings);
            this.Controls.Add(this.txtWeekDayPercents);
            this.Controls.Add(this.btnBrowseFolder);
            this.Controls.Add(this.grdResults);
            this.Controls.Add(this.lblCurrentFile);
            this.Controls.Add(this.txtFolder);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ctlEndDate);
            this.Controls.Add(this.ctlStartDate);
            this.Controls.Add(this.btnRead);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "Read Daily Deposit Files (ver 3/26/2017)";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.grdResults)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnRead;
        private System.Windows.Forms.DateTimePicker ctlStartDate;
        private System.Windows.Forms.DateTimePicker ctlEndDate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtFolder;
        private System.Windows.Forms.Label lblCurrentFile;
        private System.Windows.Forms.DataGridView grdResults;
        private System.Windows.Forms.FolderBrowserDialog ctlFolderBrowser;
        private System.Windows.Forms.Button btnBrowseFolder;
        private System.Windows.Forms.TextBox txtWeekDayPercents;
        private System.Windows.Forms.CheckBox chkShowAllWarnings;
        private System.Windows.Forms.DataGridViewTextBoxColumn ActivityDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn DayOfWeek;
        private System.Windows.Forms.DataGridViewTextBoxColumn Weather;
        private System.Windows.Forms.DataGridViewTextBoxColumn Circumstances;
        private System.Windows.Forms.DataGridViewTextBoxColumn Net;
        private System.Windows.Forms.DataGridViewTextBoxColumn TrxCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn AvgSale;
        private System.Windows.Forms.DataGridViewTextBoxColumn CardReg;
        private System.Windows.Forms.DataGridViewTextBoxColumn CardMach;
        private System.Windows.Forms.DataGridViewTextBoxColumn BankDep;
        private System.Windows.Forms.DataGridViewTextBoxColumn CoffeeTrx;
        private System.Windows.Forms.DataGridViewTextBoxColumn CoffeeCups;
        private System.Windows.Forms.DataGridViewTextBoxColumn CoffeePrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn Gift;
        private System.Windows.Forms.DataGridViewTextBoxColumn BirdSeed;
        private System.Windows.Forms.DataGridViewTextBoxColumn Plant;
        private System.Windows.Forms.DataGridViewTextBoxColumn General;
        private System.Windows.Forms.DataGridViewTextBoxColumn Seasonal;
        private System.Windows.Forms.DataGridViewTextBoxColumn Water;
        private System.Windows.Forms.DataGridViewTextBoxColumn Soil;
        private System.Windows.Forms.DataGridViewTextBoxColumn Houseplant;
        private System.Windows.Forms.DataGridViewTextBoxColumn Tools;
        private System.Windows.Forms.DataGridViewTextBoxColumn Wreaths;
        private System.Windows.Forms.DataGridViewTextBoxColumn Seeds;
        private System.Windows.Forms.DataGridViewTextBoxColumn Fert;
        private System.Windows.Forms.DataGridViewTextBoxColumn Pots;
        private System.Windows.Forms.DataGridViewTextBoxColumn Pruners;
        private System.Windows.Forms.DataGridViewTextBoxColumn Chocolate;
        private System.Windows.Forms.DataGridViewTextBoxColumn Bulbs;
        private System.Windows.Forms.DataGridViewTextBoxColumn Chemicals;
        private System.Windows.Forms.DataGridViewTextBoxColumn BirdFeeders;
        private System.Windows.Forms.DataGridViewTextBoxColumn Unused5;
        private System.Windows.Forms.DataGridViewTextBoxColumn GiftCard;
        private System.Windows.Forms.DataGridViewTextBoxColumn NotVoided;
    }
}

