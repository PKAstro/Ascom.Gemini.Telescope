namespace ASCOM.GeminiTelescope
{
    partial class frmPECChart
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPECChart));
            this.chrt = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cbTrend = new System.Windows.Forms.ComboBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadFromGeminiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadFromFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveToGeminiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unguidedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alignVerticallyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.programToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sin2xToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sinTCossqrt3tToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tSqrttSiin2tToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sqrttsin5tToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sqrttSin10tToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setDriftCorrectionFromDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.preciseTimeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.use1PPSClockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.usePCClockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.useGeminiClockMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.split = new System.Windows.Forms.SplitContainer();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lbPECDivisor = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.lbCycles = new System.Windows.Forms.Label();
            this.lbCycle = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lbDrift = new System.Windows.Forms.Label();
            this.lbRMSAll = new System.Windows.Forms.Label();
            this.lbPeakAll = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lbPeakFilt = new System.Windows.Forms.Label();
            this.lbRMSFilt = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Number = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Period = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Amplitude = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.grid = new System.Windows.Forms.DataGridView();
            this.setSiderealRateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.chrt)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.split)).BeginInit();
            this.split.Panel1.SuspendLayout();
            this.split.Panel2.SuspendLayout();
            this.split.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            this.SuspendLayout();
            // 
            // chrt
            // 
            chartArea1.AxisX.LabelStyle.Format = "m:ss";
            chartArea1.AxisX.LineColor = System.Drawing.Color.DimGray;
            chartArea1.AxisX.MajorGrid.LineColor = System.Drawing.Color.DarkGray;
            chartArea1.AxisX.MajorTickMark.LineColor = System.Drawing.Color.Silver;
            chartArea1.AxisX.MinorGrid.Enabled = true;
            chartArea1.AxisX.MinorGrid.LineColor = System.Drawing.Color.Gainsboro;
            chartArea1.AxisX.ScrollBar.IsPositionedInside = false;
            chartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.DarkGray;
            chartArea1.AxisY.MajorTickMark.LineColor = System.Drawing.Color.DarkGray;
            chartArea1.AxisY.MinorGrid.Enabled = true;
            chartArea1.AxisY.MinorGrid.LineColor = System.Drawing.Color.Gainsboro;
            chartArea1.AxisY.ScrollBar.IsPositionedInside = false;
            chartArea1.BorderColor = System.Drawing.Color.Silver;
            chartArea1.Name = "ChartArea1";
            this.chrt.ChartAreas.Add(chartArea1);
            this.chrt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chrt.Location = new System.Drawing.Point(0, 0);
            this.chrt.Name = "chrt";
            this.chrt.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.EarthTones;
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series1.Name = "Series1";
            this.chrt.Series.Add(series1);
            this.chrt.Size = new System.Drawing.Size(623, 408);
            this.chrt.TabIndex = 0;
            this.chrt.Text = "PEC Chart";
            this.chrt.Click += new System.EventHandler(this.chrt_Click);
            this.chrt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.chrt_KeyDown);
            this.chrt.KeyUp += new System.Windows.Forms.KeyEventHandler(this.chrt_KeyUp);
            this.chrt.MouseDown += new System.Windows.Forms.MouseEventHandler(this.chrt_MouseDown);
            this.chrt.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.chrt_PreviewKeyDown);
            // 
            // comboBox1
            // 
            this.comboBox1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Time",
            "Frequency"});
            this.comboBox1.Location = new System.Drawing.Point(43, 32);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(96, 21);
            this.comboBox1.TabIndex = 1;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "View:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(158, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "De-trend:";
            // 
            // cbTrend
            // 
            this.cbTrend.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.cbTrend.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTrend.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbTrend.FormattingEnabled = true;
            this.cbTrend.Items.AddRange(new object[] {
            "None",
            "Linear",
            "Quadratic",
            "Cubic",
            "Quartic",
            "Quintic"});
            this.cbTrend.Location = new System.Drawing.Point(210, 32);
            this.cbTrend.Name = "cbTrend";
            this.cbTrend.Size = new System.Drawing.Size(96, 21);
            this.cbTrend.TabIndex = 3;
            this.cbTrend.SelectedIndexChanged += new System.EventHandler(this.cbTrend_SelectedIndexChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.testToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(853, 24);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadFromGeminiToolStripMenuItem,
            this.loadFromFileToolStripMenuItem,
            this.toolStripMenuItem1,
            this.saveToGeminiToolStripMenuItem,
            this.saveToFileToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            this.fileToolStripMenuItem.DropDownOpening += new System.EventHandler(this.fileToolStripMenuItem_DropDownOpening);
            // 
            // loadFromGeminiToolStripMenuItem
            // 
            this.loadFromGeminiToolStripMenuItem.Name = "loadFromGeminiToolStripMenuItem";
            this.loadFromGeminiToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.loadFromGeminiToolStripMenuItem.Text = "Load from Gemini";
            this.loadFromGeminiToolStripMenuItem.Click += new System.EventHandler(this.loadFromGeminiToolStripMenuItem_Click);
            // 
            // loadFromFileToolStripMenuItem
            // 
            this.loadFromFileToolStripMenuItem.Name = "loadFromFileToolStripMenuItem";
            this.loadFromFileToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.loadFromFileToolStripMenuItem.Text = "Load from File...";
            this.loadFromFileToolStripMenuItem.Click += new System.EventHandler(this.loadFromFileToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(167, 6);
            // 
            // saveToGeminiToolStripMenuItem
            // 
            this.saveToGeminiToolStripMenuItem.Name = "saveToGeminiToolStripMenuItem";
            this.saveToGeminiToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.saveToGeminiToolStripMenuItem.Text = "Save to Gemini...";
            // 
            // saveToFileToolStripMenuItem
            // 
            this.saveToFileToolStripMenuItem.Name = "saveToFileToolStripMenuItem";
            this.saveToFileToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.saveToFileToolStripMenuItem.Text = "Save to File...";
            // 
            // testToolStripMenuItem
            // 
            this.testToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.unguidedToolStripMenuItem,
            this.toolStripMenuItem2,
            this.programToolStripMenuItem,
            this.setDriftCorrectionFromDataToolStripMenuItem,
            this.preciseTimeToolStripMenuItem,
            this.setSiderealRateToolStripMenuItem});
            this.testToolStripMenuItem.Name = "testToolStripMenuItem";
            this.testToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.testToolStripMenuItem.Text = "Test";
            // 
            // unguidedToolStripMenuItem
            // 
            this.unguidedToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startToolStripMenuItem,
            this.stopToolStripMenuItem,
            this.alignVerticallyToolStripMenuItem});
            this.unguidedToolStripMenuItem.Name = "unguidedToolStripMenuItem";
            this.unguidedToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.unguidedToolStripMenuItem.Text = "Unguided";
            // 
            // startToolStripMenuItem
            // 
            this.startToolStripMenuItem.Name = "startToolStripMenuItem";
            this.startToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.startToolStripMenuItem.Text = "Start";
            this.startToolStripMenuItem.Click += new System.EventHandler(this.startToolStripMenuItem_Click);
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.stopToolStripMenuItem.Text = "Stop";
            this.stopToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
            // 
            // alignVerticallyToolStripMenuItem
            // 
            this.alignVerticallyToolStripMenuItem.Name = "alignVerticallyToolStripMenuItem";
            this.alignVerticallyToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.alignVerticallyToolStripMenuItem.Text = "Align Vertically";
            this.alignVerticallyToolStripMenuItem.Click += new System.EventHandler(this.alignVerticallyToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem3,
            this.toolStripMenuItem4,
            this.toolStripMenuItem5});
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(231, 22);
            this.toolStripMenuItem2.Text = "Guided";
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(153, 22);
            this.toolStripMenuItem3.Text = "Start";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.toolStripMenuItem3_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(153, 22);
            this.toolStripMenuItem4.Text = "Stop";
            this.toolStripMenuItem4.Click += new System.EventHandler(this.toolStripMenuItem4_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(153, 22);
            this.toolStripMenuItem5.Text = "Align Vertically";
            this.toolStripMenuItem5.Click += new System.EventHandler(this.toolStripMenuItem5_Click);
            // 
            // programToolStripMenuItem
            // 
            this.programToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sin2xToolStripMenuItem,
            this.sinTCossqrt3tToolStripMenuItem,
            this.tSqrttSiin2tToolStripMenuItem,
            this.sqrttsin5tToolStripMenuItem,
            this.sqrttSin10tToolStripMenuItem});
            this.programToolStripMenuItem.Name = "programToolStripMenuItem";
            this.programToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.programToolStripMenuItem.Text = "Program";
            this.programToolStripMenuItem.Click += new System.EventHandler(this.programToolStripMenuItem_Click);
            // 
            // sin2xToolStripMenuItem
            // 
            this.sin2xToolStripMenuItem.Name = "sin2xToolStripMenuItem";
            this.sin2xToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.sin2xToolStripMenuItem.Text = "sqrt(t) * sin(2t)";
            this.sin2xToolStripMenuItem.Click += new System.EventHandler(this.sin2xToolStripMenuItem_Click);
            // 
            // sinTCossqrt3tToolStripMenuItem
            // 
            this.sinTCossqrt3tToolStripMenuItem.Name = "sinTCossqrt3tToolStripMenuItem";
            this.sinTCossqrt3tToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.sinTCossqrt3tToolStripMenuItem.Text = "sin t + cos (t√3)";
            this.sinTCossqrt3tToolStripMenuItem.Visible = false;
            this.sinTCossqrt3tToolStripMenuItem.Click += new System.EventHandler(this.sinTCossqrt3tToolStripMenuItem_Click);
            // 
            // tSqrttSiin2tToolStripMenuItem
            // 
            this.tSqrttSiin2tToolStripMenuItem.Name = "tSqrttSiin2tToolStripMenuItem";
            this.tSqrttSiin2tToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.tSqrttSiin2tToolStripMenuItem.Text = "t + sqrt(t) * sin(2t)";
            this.tSqrttSiin2tToolStripMenuItem.Click += new System.EventHandler(this.tSqrttSiin2tToolStripMenuItem_Click);
            // 
            // sqrttsin5tToolStripMenuItem
            // 
            this.sqrttsin5tToolStripMenuItem.Name = "sqrttsin5tToolStripMenuItem";
            this.sqrttsin5tToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.sqrttsin5tToolStripMenuItem.Text = "sqrt(t) * sin(5t)";
            this.sqrttsin5tToolStripMenuItem.Click += new System.EventHandler(this.sqrttsin5tToolStripMenuItem_Click);
            // 
            // sqrttSin10tToolStripMenuItem
            // 
            this.sqrttSin10tToolStripMenuItem.Name = "sqrttSin10tToolStripMenuItem";
            this.sqrttSin10tToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.sqrttSin10tToolStripMenuItem.Text = "sqrt(t) * sin(10t)";
            this.sqrttSin10tToolStripMenuItem.Click += new System.EventHandler(this.sqrttSin10tToolStripMenuItem_Click);
            // 
            // setDriftCorrectionFromDataToolStripMenuItem
            // 
            this.setDriftCorrectionFromDataToolStripMenuItem.Name = "setDriftCorrectionFromDataToolStripMenuItem";
            this.setDriftCorrectionFromDataToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.setDriftCorrectionFromDataToolStripMenuItem.Text = "Set Drift Correction from Data";
            this.setDriftCorrectionFromDataToolStripMenuItem.Click += new System.EventHandler(this.setDriftCorrectionFromDataToolStripMenuItem_Click);
            // 
            // preciseTimeToolStripMenuItem
            // 
            this.preciseTimeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.use1PPSClockToolStripMenuItem,
            this.usePCClockToolStripMenuItem,
            this.useGeminiClockMenuItem,
            this.testToolStripMenuItem1});
            this.preciseTimeToolStripMenuItem.Name = "preciseTimeToolStripMenuItem";
            this.preciseTimeToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.preciseTimeToolStripMenuItem.Text = "Time Reference";
            this.preciseTimeToolStripMenuItem.DropDownOpening += new System.EventHandler(this.preciseTimeToolStripMenuItem_DropDownOpening);
            // 
            // use1PPSClockToolStripMenuItem
            // 
            this.use1PPSClockToolStripMenuItem.Name = "use1PPSClockToolStripMenuItem";
            this.use1PPSClockToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.use1PPSClockToolStripMenuItem.Text = "Use 1PPS GPS Clock";
            this.use1PPSClockToolStripMenuItem.Click += new System.EventHandler(this.use1PPSGPSClockToolStripMenuItem_Click_1);
            // 
            // usePCClockToolStripMenuItem
            // 
            this.usePCClockToolStripMenuItem.CheckOnClick = true;
            this.usePCClockToolStripMenuItem.Name = "usePCClockToolStripMenuItem";
            this.usePCClockToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.usePCClockToolStripMenuItem.Text = "Use PC Clock";
            this.usePCClockToolStripMenuItem.Click += new System.EventHandler(this.usePCClockToolStripMenuItem_Click_1);
            // 
            // useGeminiClockMenuItem
            // 
            this.useGeminiClockMenuItem.Name = "useGeminiClockMenuItem";
            this.useGeminiClockMenuItem.Size = new System.Drawing.Size(211, 22);
            this.useGeminiClockMenuItem.Text = "Use Gemini Sidereal Clock";
            this.useGeminiClockMenuItem.Click += new System.EventHandler(this.useGeminiClock_Click);
            // 
            // testToolStripMenuItem1
            // 
            this.testToolStripMenuItem1.Name = "testToolStripMenuItem1";
            this.testToolStripMenuItem1.Size = new System.Drawing.Size(211, 22);
            this.testToolStripMenuItem1.Text = "Test PC vs 1PPS";
            this.testToolStripMenuItem1.Click += new System.EventHandler(this.testToolStripMenuItem1_Click_1);
            // 
            // split
            // 
            this.split.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.split.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.split.Location = new System.Drawing.Point(0, 59);
            this.split.Name = "split";
            // 
            // split.Panel1
            // 
            this.split.Panel1.Controls.Add(this.chrt);
            // 
            // split.Panel2
            // 
            this.split.Panel2.Controls.Add(this.splitContainer1);
            this.split.Size = new System.Drawing.Size(853, 408);
            this.split.SplitterDistance = 623;
            this.split.TabIndex = 6;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dataGridView1);
            this.splitContainer1.Panel2.Controls.Add(this.grid);
            this.splitContainer1.Size = new System.Drawing.Size(226, 408);
            this.splitContainer1.SplitterDistance = 235;
            this.splitContainer1.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 37.14286F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 62.85714F));
            this.tableLayoutPanel1.Controls.Add(this.lbPECDivisor, 1, 7);
            this.tableLayoutPanel1.Controls.Add(this.label11, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.label10, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.lbCycles, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.lbCycle, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.label8, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.lbDrift, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.lbRMSAll, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.lbPeakAll, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label6, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label7, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.lbPeakFilt, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lbRMSFilt, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 8;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(226, 235);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // lbPECDivisor
            // 
            this.lbPECDivisor.AutoSize = true;
            this.lbPECDivisor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbPECDivisor.Location = new System.Drawing.Point(87, 204);
            this.lbPECDivisor.Name = "lbPECDivisor";
            this.lbPECDivisor.Size = new System.Drawing.Size(135, 30);
            this.lbPECDivisor.TabIndex = 15;
            this.lbPECDivisor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label11.Location = new System.Drawing.Point(4, 204);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(76, 30);
            this.label11.TabIndex = 14;
            this.label11.Text = "PEC Divisor:";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label10.Location = new System.Drawing.Point(4, 175);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(76, 28);
            this.label10.TabIndex = 13;
            this.label10.Text = "Cycles:";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbCycles
            // 
            this.lbCycles.AutoSize = true;
            this.lbCycles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbCycles.Location = new System.Drawing.Point(87, 175);
            this.lbCycles.Name = "lbCycles";
            this.lbCycles.Size = new System.Drawing.Size(135, 28);
            this.lbCycles.TabIndex = 12;
            this.lbCycles.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbCycle
            // 
            this.lbCycle.AutoSize = true;
            this.lbCycle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbCycle.Location = new System.Drawing.Point(87, 146);
            this.lbCycle.Name = "lbCycle";
            this.lbCycle.Size = new System.Drawing.Size(135, 28);
            this.lbCycle.TabIndex = 11;
            this.lbCycle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label8.Location = new System.Drawing.Point(4, 146);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(76, 28);
            this.label8.TabIndex = 10;
            this.label8.Text = "Worm Cycle:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbDrift
            // 
            this.lbDrift.AutoSize = true;
            this.lbDrift.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbDrift.Location = new System.Drawing.Point(87, 117);
            this.lbDrift.Name = "lbDrift";
            this.lbDrift.Size = new System.Drawing.Size(135, 28);
            this.lbDrift.TabIndex = 9;
            this.lbDrift.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbRMSAll
            // 
            this.lbRMSAll.AutoSize = true;
            this.lbRMSAll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbRMSAll.Location = new System.Drawing.Point(87, 88);
            this.lbRMSAll.Name = "lbRMSAll";
            this.lbRMSAll.Size = new System.Drawing.Size(135, 28);
            this.lbRMSAll.TabIndex = 8;
            this.lbRMSAll.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbPeakAll
            // 
            this.lbPeakAll.AutoSize = true;
            this.lbPeakAll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbPeakAll.Location = new System.Drawing.Point(87, 59);
            this.lbPeakAll.Name = "lbPeakAll";
            this.lbPeakAll.Size = new System.Drawing.Size(135, 28);
            this.lbPeakAll.TabIndex = 7;
            this.lbPeakAll.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(4, 1);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 28);
            this.label3.TabIndex = 0;
            this.label3.Text = "Peak (filtered):";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Location = new System.Drawing.Point(4, 30);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 28);
            this.label4.TabIndex = 1;
            this.label4.Text = "RMS (filtered):";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Location = new System.Drawing.Point(4, 59);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(76, 28);
            this.label5.TabIndex = 2;
            this.label5.Text = "Peak (all):";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label6.Location = new System.Drawing.Point(4, 88);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(76, 28);
            this.label6.TabIndex = 3;
            this.label6.Text = "RMS (all):";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label7.Location = new System.Drawing.Point(4, 117);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(76, 28);
            this.label7.TabIndex = 4;
            this.label7.Text = "Drift:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbPeakFilt
            // 
            this.lbPeakFilt.AutoSize = true;
            this.lbPeakFilt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbPeakFilt.Location = new System.Drawing.Point(87, 1);
            this.lbPeakFilt.Name = "lbPeakFilt";
            this.lbPeakFilt.Size = new System.Drawing.Size(135, 28);
            this.lbPeakFilt.TabIndex = 5;
            this.lbPeakFilt.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbRMSFilt
            // 
            this.lbRMSFilt.AutoSize = true;
            this.lbRMSFilt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbRMSFilt.Location = new System.Drawing.Point(87, 30);
            this.lbRMSFilt.Name = "lbRMSFilt";
            this.lbRMSFilt.Size = new System.Drawing.Size(135, 28);
            this.lbRMSFilt.TabIndex = 6;
            this.lbRMSFilt.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Number,
            this.Period,
            this.Amplitude});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.EnableHeadersVisualStyles = false;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.ShowCellErrors = false;
            this.dataGridView1.ShowEditingIcon = false;
            this.dataGridView1.Size = new System.Drawing.Size(226, 169);
            this.dataGridView1.TabIndex = 1;
            // 
            // Number
            // 
            this.Number.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Number.HeaderText = "#";
            this.Number.Name = "Number";
            this.Number.ReadOnly = true;
            this.Number.Width = 39;
            // 
            // Period
            // 
            this.Period.HeaderText = "Per";
            this.Period.Name = "Period";
            this.Period.ReadOnly = true;
            // 
            // Amplitude
            // 
            this.Amplitude.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Amplitude.HeaderText = "Amplitude";
            this.Amplitude.Name = "Amplitude";
            this.Amplitude.ReadOnly = true;
            // 
            // grid
            // 
            this.grid.AllowUserToAddRows = false;
            this.grid.AllowUserToDeleteRows = false;
            this.grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid.Location = new System.Drawing.Point(0, 0);
            this.grid.Name = "grid";
            this.grid.ReadOnly = true;
            this.grid.Size = new System.Drawing.Size(226, 169);
            this.grid.TabIndex = 0;
            // 
            // setSiderealRateToolStripMenuItem
            // 
            this.setSiderealRateToolStripMenuItem.Name = "setSiderealRateToolStripMenuItem";
            this.setSiderealRateToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.setSiderealRateToolStripMenuItem.Text = "Set Sidereal Rate...";
            this.setSiderealRateToolStripMenuItem.Click += new System.EventHandler(this.setSiderealRateToolStripMenuItem_Click);
            // 
            // frmPECChart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(853, 465);
            this.Controls.Add(this.split);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbTrend);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmPECChart";
            this.Text = "PEC Chart";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmPECChart_FormClosing);
            this.Load += new System.EventHandler(this.frmPECChart_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chrt)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.split.Panel1.ResumeLayout(false);
            this.split.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.split)).EndInit();
            this.split.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chrt;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbTrend;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadFromGeminiToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadFromFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToGeminiToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.SplitContainer split;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lbDrift;
        private System.Windows.Forms.Label lbRMSAll;
        private System.Windows.Forms.Label lbPeakAll;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lbPeakFilt;
        private System.Windows.Forms.Label lbRMSFilt;
        private System.Windows.Forms.Label lbCycle;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.DataGridView grid;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Number;
        private System.Windows.Forms.DataGridViewTextBoxColumn Period;
        private System.Windows.Forms.DataGridViewTextBoxColumn Amplitude;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unguidedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem programToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sin2xToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sinTCossqrt3tToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem alignVerticallyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setDriftCorrectionFromDataToolStripMenuItem;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lbCycles;
        private System.Windows.Forms.Label lbPECDivisor;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ToolStripMenuItem tSqrttSiin2tToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem sqrttsin5tToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sqrttSin10tToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem preciseTimeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem use1PPSClockToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem usePCClockToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem useGeminiClockMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem setSiderealRateToolStripMenuItem;
    }
}