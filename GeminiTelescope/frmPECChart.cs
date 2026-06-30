using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using MathNet;
using MathNet.Numerics;
using System.Diagnostics;
using MathNet.Numerics.Interpolation;
using System.Globalization;
using System.IO;
using System.Threading;
using MathNet;
using MathNet.Numerics;
using MathNet.Numerics.Statistics;
using System.Diagnostics;
using MathNet.Numerics.Interpolation;

namespace ASCOM.GeminiTelescope
{
    public partial class frmPECChart : Form
    {
        public PEClass currentPE = new PEClass();
         
        private static int count = 1;


        bool Loading = false;

        bool usePCClock = true;
        bool use1PPS = false;

        public frmPECChart()
        {
            InitializeComponent();
#if !DEBUG 
            menuStrip1.Visible = false;

#endif

            this.Text = "PEC Chart " + count.ToString();
            count++;
        }


        internal void UpdateData()
        {
            if (comboBox1.SelectedIndex <= 0)
                UpdateDataTime();
            else
                UpdateDataFFT();
            this.Text = $"Configure PEC - {currentPE.FileName}";
            if (currentPE.NumberOfCycles < 0.5)
                lbDrift.Text = $"not ready";
            else          
                lbDrift.Text = $"{currentPE.Drift:0.000}\"/cycle";
            lbPeakAll.Text = $"{currentPE.PeakAll:0.000}\" pk-pk";
            lbRMSAll.Text = $"{currentPE.RMSAll:0.000}\" rms";
            lbCycle.Text = $"{currentPE.PECOneWorm * currentPE.StepX}sec";
            var div = GeminiHardware.Instance.DoCommandResult("<513:",5000, false);
            int dd = 0;
            int.TryParse(div, out dd);
            lbPECDivisor.Text = $"{dd}";
        }

        internal void UpdateDataTime()
        {
            if (Loading) return;

            chrt.ChartAreas.Clear();
            chrt.ChartAreas.Add("area1");
            chrt.Series.Clear();
            //chrt.Series["Series1"].Points.Clear();

            chrt.Series.Add("Series1");
            chrt.Series[0].XValueType = ChartValueType.Auto;
       
            
            chrt.Series["Series1"].ChartType = SeriesChartType.Line;
            var chArea = chrt.ChartAreas[0];
            chArea.AxisY.Title = "arcsecs";
            chArea.AxisX.Title = "time";
            chArea.AxisX.LabelStyle.Format = "m:ss";
            chArea.AxisX.LabelStyle.Enabled = true;
            chArea.AxisY.LabelStyle.Format = "G4";
            chArea.AxisY.LabelStyle.Enabled = true;
            chArea.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            chArea.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            chArea.AxisY.MajorGrid.LineColor = Color.Gray;
            chArea.AxisY.MajorGrid.LineColor = Color.Gray;
            chArea.CursorX.IsUserEnabled = true;
            chArea.CursorX.IsUserSelectionEnabled = true;

            chArea.AxisX.IntervalType = DateTimeIntervalType.Auto;
            chArea.AxisX.LabelStyle.IsEndLabelVisible = true;

            chArea.AxisX.ScaleView.Zoomable = true;
            chArea.AxisY.ScaleView.Zoomable = true;
            chrt.MouseWheel += Chrt_MouseWheel;

            double timeScale = 15.041068640261926 / userSiderealRate;

            currentPE.TimeScale = timeScale;

            if (currentPE.StepX == 0) currentPE.GetPECStatusFromGemini();

            var startTime = new DateTime(2000, 1, 1, 1, 0, 0);


            //chArea.AxisX.ScaleView.Zoom(startTime.ToOADate(), startTime.AddSeconds(currentPE.WormPeriodSeconds).ToOADate());
            //chArea.AxisY.ScaleView.Zoom(-0.1, +0.1);

            if (currentPE.StepX == 0 ||
                currentPE.X == null || currentPE.Y == null || currentPE.X?.Length < 5 || currentPE.Y?.Length < 5) return;
            var xd = currentPE.X.Select(a => (a - startTime).TotalSeconds / currentPE.StepX);
            var yd = currentPE.Y.Select(a => (double)a);

            Func<double, double> poly = null;

            int deg = cbTrend.SelectedIndex;
            if (deg < 0) deg = 0;
            try
            {
                poly = MathNet.Numerics.Fit.PolynomialFunc(xd.ToArray(), yd.ToArray(), deg);
            }
            catch (Exception ex)
            {
                GeminiHardware.Instance.Trace.Error("PEC Error", ex.Message);
                poly = new Func<double, double>((a) => a);
            }

            var p = chrt.Series["Series1"].Points;
            cycleNumber = 0;
            secondsPreviousPoint = 0;
            palette = LoadPalette("color cube", 16);
            secsPerCycle = currentPE.WormPeriodSeconds;// currentPE.PECOneWorm * currentPE.StepX * currentPE.WormPeriods;

            double offsetY = currentPE.Y[0] - poly((currentPE.X[0] - startTime).TotalSeconds / currentPE.StepX);

            if (currentPE.X.Length < 5) return;

            for (int i = 0; i < currentPE.X.Length; ++i)
            {

                string ser = $"Series{cycleNumber + 1}";

                double time;

                if (currentPE.WormPeriods != 0)
                  time = (currentPE.X[i] - startTime).TotalSeconds % (currentPE.WormPeriodSeconds * currentPE.WormPeriods);
                else
                  time = (currentPE.X[i] - startTime).TotalSeconds;

                if (time < secondsPreviousPoint) // wrap to start, new cycle
                {
                    cycleNumber++;
                    ser = $"Series{cycleNumber + 2}";

                    if (chrt.Series.FindByName(ser) == null)
                    {
                        chrt.Series.Add(ser);

                    }
                    chrt.Series[ser].XValueType = ChartValueType.Auto;

                    chrt.Series[ser].ChartType = SeriesChartType.Line;
                    chrt.Series[ser].Color = palette[cycleNumber % palette.Length];

                    chrt.Series[ser].MarkerSize = 1;
                    chrt.Series[ser].Points.Clear();
                    p = chrt.Series[ser].Points;
                }
                secondsPreviousPoint = time;
                if (double.IsNaN(currentPE.StepX) || double.IsInfinity(currentPE.StepX) || currentPE.StepX == 0) return;
                p.AddXY(startTime.AddSeconds(time), currentPE.Y[i] - poly((currentPE.X[i] - startTime).TotalSeconds / currentPE.StepX) - offsetY);
            }

            currentPE.UpdateDataFFT(deg);
        }

        int numberOfZoom = 0;

        double xMin;
        double xMax;
        double yMin ;
        double yMax;


        private void Chrt_MouseWheel(object sender, MouseEventArgs e)
        {
            var chart = (Chart)sender;
            var xAxis = chart.ChartAreas[0].AxisX;
            var yAxis = chart.ChartAreas[0].AxisY;

            if (numberOfZoom <= 0)
            {
                xMin = xAxis.ScaleView.ViewMinimum;
                xMax = xAxis.ScaleView.ViewMaximum;
                yMin = yAxis.ScaleView.ViewMinimum;
                yMax = yAxis.ScaleView.ViewMaximum;
            }
            int IntervalX = 1;
            int IntervalY = 1;
            try
            {
                if (e.Delta < 0 && numberOfZoom > 0) // Scrolled down.
                {
                    var posXStart = xAxis.PixelPositionToValue(e.Location.X) - (xMax - xMin) / (1 + numberOfZoom / 8.0); //Math.Pow(1, numberOfZoom);
                    var posXFinish = xAxis.PixelPositionToValue(e.Location.X) + (xMax - xMin) / (1 + numberOfZoom / 8/0); // Math.Pow(1, numberOfZoom);
                    var posYStart = yAxis.PixelPositionToValue(e.Location.Y) - (yMax - yMin) / (1 + numberOfZoom / 4.0); //Math.Pow(1.1, numberOfZoom);
                    var posYFinish = yAxis.PixelPositionToValue(e.Location.Y) + (yMax - yMin) / (1 + numberOfZoom / 4.0); //Math.Pow(1.1, numberOfZoom);

                    //if (posXStart < 0) posXStart = 0;
                    if (posYStart < 0) posYStart = 0;
                    if (posYFinish > yAxis.Maximum) posYFinish = yAxis.Maximum;
                    if (posXFinish > xAxis.Maximum) posYFinish = xAxis.Maximum;
                    xAxis.ScaleView.Zoom(posXStart, posXFinish);
                    yAxis.ScaleView.Zoom(posYStart, posYFinish);
                    numberOfZoom--;
                }
                else if (e.Delta < 0 && numberOfZoom == 0) //Last scrolled dowm
                {
                    yAxis.ScaleView.ZoomReset();
                    xAxis.ScaleView.ZoomReset();
                }
                else if (e.Delta > 0) // Scrolled up.
                {

                    var posXStart = xAxis.PixelPositionToValue(e.Location.X) - (xMax - xMin) / (1 + numberOfZoom / 8.0); // Math.Pow(1, numberOfZoom);
                    var posXFinish = xAxis.PixelPositionToValue(e.Location.X) + (xMax - xMin) / (1 + numberOfZoom / 8.0); //Math.Pow(1, numberOfZoom);
                    var posYStart = yAxis.PixelPositionToValue(e.Location.Y) - (yMax - yMin) / (1 + numberOfZoom / 8.0); //Math.Pow(1.1, numberOfZoom);
                    var posYFinish = yAxis.PixelPositionToValue(e.Location.Y) + (yMax - yMin) / (1 + numberOfZoom / 8.0); //Math.Pow(1.1, numberOfZoom);

                    xAxis.ScaleView.Zoom(posXStart, posXFinish);
                    yAxis.ScaleView.Zoom(posYStart, posYFinish);
                    numberOfZoom++;
                }

                if (numberOfZoom < 0) numberOfZoom = 0;
            }
            catch { }
        }

        int fft_size = 4096;

        internal void UpdateDataFFT()
        {
            if (Loading) return;

            //chrt.ChartAreas.Clear();
            //var chArea = chrt.ChartAreas.Add(null);
            chrt.Series.Clear();
            //            chrt.Series["Series1"].Points.Clear();
            System.Windows.Forms.DataVisualization.Charting.MSChartExtension.EnableZoomAndPanControls(chrt);

            chrt.Series.Add("Series1");
            chrt.Series[0].XValueType = ChartValueType.Double;
             
            chrt.Series["Series1"].ChartType = SeriesChartType.Column;
            chrt.Series["Series1"].XAxisType = AxisType.Primary;
            chrt.Series["Series1"].SetCustomProperty("PixelPointWidth", "4");
            chrt.Series["Series1"].BorderColor = Color.Blue;
          var chArea = chrt.ChartAreas[0];
            chArea.CursorX.IsUserEnabled = true;
            chArea.CursorX.IsUserSelectionEnabled = true;

            chArea.AxisX.LabelStyle.Format = "F1";
            chArea.AxisX.IntervalType = DateTimeIntervalType.Number;

            chArea.AxisX.LabelStyle.Enabled = true;
            //chArea.AxisX.IsLogarithmic = true;
            chArea.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            chArea.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            chArea.AxisY.MajorGrid.LineColor = Color.Gray;
            chArea.AxisY.MajorGrid.LineColor = Color.Gray;
            
            chArea.AxisX.LineWidth = 2;
            chArea.AxisY.LineWidth = 2;
            chArea.AxisX.ScaleView.Zoomable = true;
            chArea.AxisX.ScrollBar.IsPositionedInside = true;
            chArea.AxisX.ScaleView.SmallScrollMinSizeType = DateTimeIntervalType.Seconds;
          
            chArea.AxisX.ScaleView.SmallScrollSize = 1e-6;
            chArea.AxisX.ScaleView.SmallScrollMinSize = 1e-6;
            chArea.AxisX.ScaleView.SmallScrollSize = 1e-6;
          //  chArea.AxisX.ScaleView.Scroll(ScrollType.SmallDecrement); = 1e-6;
            chArea.AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
            chArea.AxisY.IntervalAutoMode = IntervalAutoMode.FixedCount;
            chArea.AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.All;
            chArea.AxisX.IntervalAutoMode = IntervalAutoMode.FixedCount;
            chArea.AxisX.ScaleView.SizeType = DateTimeIntervalType.Number;
            chArea.CursorX.AutoScroll = true;
            //chArea.AxisY.ScaleView.Zoomable = true;
            //chArea.CursorY.IsUserEnabled = true;
            //chArea.CursorY.IsUserSelectionEnabled = true;
            //chArea.AxisY.ScrollBar.IsPositionedInside = true;
            //chArea.AxisY.ScaleView.SmallScrollSize = 0.05;
            //chArea.AxisX.LabelStyle.Format = "F0";
            chArea.AxisX.LabelStyle.IsEndLabelVisible = true;
            chArea.AxisX.LabelAutoFitStyle = LabelAutoFitStyles.DecreaseFont;
            
            chrt.ChartAreas[0].AxisY.Title = "arcsecs";
            chrt.ChartAreas[0].AxisX.Title = "cycle divisor";
            //chrt.ChartAreas[0].AxisX.IsLogarithmic = true;
            //chrt.ChartAreas[0].AxisX.LogarithmBase = 10;
            chrt.Palette = ChartColorPalette.None;
            chrt.PaletteCustomColors = new Color[] {Color.Red, Color.DarkRed,
                                            Color.Green, Color.Orange,
                                            Color.SteelBlue, Color.Magenta,
                                            Color.Purple, Color.OliveDrab,
                                            Color.DodgerBlue, Color.Sienna,
                                            Color.Teal, Color.YellowGreen,
                                            Color.RoyalBlue, Color.Black,
                                            Color.Teal, Color.OrangeRed};


 
            currentPE.UpdateDataFFT(cbTrend.SelectedIndex);
            var compl = currentPE.fft;
            if (compl == null) return;
            int last = 0;
            int first = 0;
            double winGain = 1;
            fft_size = currentPE.fft_size;
            for (int i = 0; i < fft_size / 2; i++)
            {
                var mag = compl[i].Magnitude / fft_size * 2 / winGain;
                if (mag >= 0.2) last = i;
                if (first == 0 && mag > 0.2) first = i;
            }

            double mx = 0;
            double mn = double.MaxValue;
            double sampleRate = 1.0 / currentPE.StepX; //samples per second

            var points = chrt.Series["Series1"].Points;
            double fmin = double.MaxValue;
            double fmax = double.MinValue;

            for (int i = last + 2; i > 0; i--)
            {
                var mag = compl[i].Magnitude / fft_size * 2 / winGain;
                var time = 1 / (i / (double)fft_size) * currentPE.StepX ;

                
                if (mag > 0.3 )
                {
                    
                    double freq = currentPE.WormPeriods * currentPE.PECOneWorm * currentPE.StepX / time;

                    if (fmax < freq) fmax = freq;
                    if (fmin > freq) fmin = freq;
                    int p = points.AddXY((float)freq, mag);
                    if (time > 5)
                        chrt.Series[0].Points[p].Label = $"[{freq:0.##}] {mag:0.000}\" ({time:0.000}s)";
                    //System.Diagnostics.Debug.WriteLine($"{i} {time} {mag}");
                    if (time > mx) mx = time;
                    if (time < mn) mn = time;
                }

            }
            if (mx == 0) mx = currentPE.MaxX;

            chArea.AxisX.Maximum = fmax * 1.1; // mx * 1.1;
            chArea.AxisX.Minimum = Math.Max(0, (int)(fmin/1.1) );// mn - 10 ;
                                                                 //chArea.AxisX.LogarithmBase = 10;
                                                                 //chArea.AxisX.IsLogarithmic = true;


            //chArea.AxisX.LabelStyle.Enabled = true;

        }

    private void chrt_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateData();
        }

        private void frmPECChart_Load(object sender, EventArgs e)
        {
            //Loading = true;
            cbTrend.SelectedIndex = 0;
            LoadFileGemini(null);
            Loading = false;
            comboBox1.SelectedIndex = 0;
        }


        private void cbTrend_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateData();
        }

        private void loadFromGeminiToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void fileToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            frmPEC frm = Owner as frmPEC;
            var files = frm.FTPFileList("PEC/*");
            loadFromGeminiToolStripMenuItem.DropDownItems.Clear();
            if (files!=null)
                loadFromGeminiToolStripMenuItem.DropDownItems.AddRange(
                       files.Select((x) => new ToolStripMenuItem { Name = x, Text = x, Tag=$"gemini://{x}" }).ToArray()
                   );

            for (int i = 0; i < loadFromGeminiToolStripMenuItem.DropDownItems.Count; ++i)
                loadFromGeminiToolStripMenuItem.DropDownItems[i].Click += LoadFile_Click;
        }

        private void LoadFile_Click(object sender, EventArgs e)
        {
            var f = (sender as ToolStripMenuItem).Tag as string;
            if (f.StartsWith("gemini://"))
            {
                LoadFileGemini(f.Substring(9));
            }
            else
            {
                LoadFileDisk(f);
            }
        }

        private void LoadFileDisk(string f)
        {
        }

        private void LoadFileGemini(string file)
        {
            if (file != null)
                GeminiHardware.Instance.DoCommand("<551:\\PEC\\" + file, false);

            var pec = new PEClass();
            pec.TimeScale = 15.041068640261926 / userSiderealRate;

            if (pec.LoadFromGemini(cbTrend.SelectedIndex))
            {
                if (!string.IsNullOrEmpty(file))
                   pec.FileName = "gemini://" + file;
                currentPE = pec;
                UpdateData();
            }

        }

        private void loadFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Gemini PEC file (*.pec)|*.pec|PemPro raw file (*.txt)|*.txt";
            dlg.FilterIndex = 0;
            dlg.CheckFileExists = true;
            dlg.Title = "Select PEC file to load";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (dlg.FilterIndex == 0)
                {

                }
                else
                {
                    var pec = new PEClass();
                    pec.LoadFromPempro(dlg.FileName, cbTrend.SelectedIndex);
                    pec.FileName = "pempro://" + Path.GetFileNameWithoutExtension(dlg.FileName);
                    currentPE = pec;
                    UpdateData();
                }
            }
        }

        
        Int64[] pos = null;
        List<double> lastN = new List<double>();

        Int64 first_updatePRA = 0;
        int first_updatePECIdx = 0;
        int last_update = 0;
        Int64 half_circle_ra = 0;
        double first_Y = 0;

        System.Windows.Forms.Timer tmUpdatePEC;

        System.Windows.Forms.Timer tmGuide;
        System.Windows.Forms.Timer tmGuideMain;
        int guideDirection = 0;
        int guideSteps = 500;
        int guideCounter = 0;

        DateTime startTime;
        DateTime lastTime;

        Stopwatch stopWatch;

        double secsPerCycle = 0;

        int cycleNumber = 0;

        double driftCorrection = 0;//-0.0010891578523192856;

        double secondsPreviousPoint = 0;

        Color[] palette;

        VerticalLineAnnotation A = new VerticalLineAnnotation();

        NmeaInterpreter gps;

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            startMeasure(false);
        }

        NistClock tmNST = new NistClock();
        DateTime tmStart, tmEnd;

        private void startMeasure(bool guide)
        {
            if (tmUpdatePEC != null)
            {
                tmUpdatePEC.Stop();
                tmUpdatePEC = null;
            }

            if (use1PPS)
            {
                gps = new NmeaInterpreter();
                gps.BaudRate = GeminiHardware.Instance.GpsBaudRate;
                gps.ComPort = GeminiHardware.Instance.GpsComPort;
                gps.Connected = true;
                gps.MilliSeconds = 0;
            }

            pos = new Int64[currentPE.PECMax];
            //currentPE.X = new DateTime[1];
            //currentPE.Y = new float[1];

            GeminiHardware.Instance.UpdatePolledVariables(true);
            var g5 = (Gemini5Hardware)GeminiHardware.Instance;
            var mac = g5.macro;
            /*
             *1733048;4608000;3.676841;+30.000000;+11.956425;359.397971;-19.907929;T;T;N;W;15.633267;0;33;7961.280366;1;>:B:DB60;-1;0;-6;4;0;0;
             * 
             *  0 PRA 2591823;
                1 PDEC 2592000;
                2 RA 19.467428;
                3 DEC +0.179764;
                4 HA
             
                5 Az 270.204063;
                6 El -28.246181;
                7 Gv N;
                8 GW N;
                9 Gw N;
                10 Gm E;
                11 GS 3.350518;
                12 h? 0;
                13 PEC 0;
                14 Western limit 50963.891895;
                15 <99: 1;
                16 B076D900;
            */

            lastN = new List<double>();

            if (usePCClock)
                startTime = lastTime = DateTime.Now ;
            else
            if (use1PPS)
                startTime = lastTime = twoThousand + TimeSpan.FromMilliseconds(gps.MilliSeconds);


            var s = mac.Split(';');
            first_updatePRA = Int64.Parse(s[0]);
            //lastN.Add(first_updatePRA);

            if (!usePCClock && !use1PPS)
            {
                var sidereal = double.Parse(s[11], GeminiHardware.Instance.m_GeminiCulture);

                DateTime currtime = twoThousand + TimeSpan.FromHours(sidereal / 24.0 * SiderealDay );

                startTime = lastTime = currtime ;   // start time
            }


            var specidx = GeminiHardware.Instance.DoCommandResult("<501:", 500, false);
            if (specidx != null)
            {
                var pecidx = int.Parse(specidx);
                first_updatePECIdx = pecidx;
                if (pecidx < currentPE.Y.Length)
                    first_Y = currentPE.Y[pecidx];
                else
                    first_Y = 0;
                secsPerCycle = currentPE.WormPeriodSeconds * currentPE.WormPeriods; // currentPE.PECOneWorm * currentPE.StepX * currentPE.WormPeriods;
            }


            var chArea = chrt.ChartAreas[0];
            chArea.AxisY.Title = "arcsecs";
            chArea.AxisX.Title = "time";
            chArea.AxisX.LabelStyle.Format = "m:ss";
            chArea.AxisX.LabelStyle.Enabled = true;

            chArea.AxisY.LabelStyle.Format = "G4";
            chArea.AxisY.LabelStyle.Enabled = true;
            chArea.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            chArea.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            chArea.AxisY.MajorGrid.LineColor = Color.Gray;
            chArea.AxisY.MajorGrid.LineColor = Color.Gray;
            chArea.CursorX.IsUserEnabled = true;
            chArea.CursorX.IsUserSelectionEnabled = true;

            chArea.AxisX.IntervalType = DateTimeIntervalType.Auto;
            chArea.AxisX.LabelStyle.IsEndLabelVisible = true;


            if (chrt.Series.FindByName("Series2") == null)
            {
                chrt.Series.Add("Series2");

            }

            cycleNumber = 0;
            secondsPreviousPoint = 0;

            palette = LoadPalette("color cube", 16);

            chrt.Series["Series2"].XValueType = ChartValueType.Auto;

            chrt.Series["Series2"].ChartType = SeriesChartType.Line;
            chrt.Series["Series2"].Color = palette[cycleNumber];
            chrt.Series["Series2"].MarkerSize = 1;
            chrt.Series["Series2"].Points.Clear();

            var ticks = GeminiHardware.Instance.DoCommandResult("<238:", 500, false);

            if (ticks!=null)
            {
                var tick_s = ticks.Split(';');
                half_circle_ra = Int64.Parse(tick_s[0]);
            }

            A = new VerticalLineAnnotation();
            A.AxisX = chArea.AxisX;
            A.AxisY = chArea.AxisY;
            A.X = (twoThousand  + TimeSpan.FromSeconds(100)).ToOADate(); // Positioning the line at the 5th second
            A.LineColor = System.Drawing.Color.Red;
            A.LineWidth = 2;
            A.Visible = true;
            chrt.Annotations.Add(A);

            stopWatch = Stopwatch.StartNew();
            var hi = Stopwatch.IsHighResolution;
            tmUpdatePEC = new System.Windows.Forms.Timer();
            tmUpdatePEC.Tick += TmUpdatePEC_Tick;
            tmUpdatePEC.Interval = 500;
            tmUpdatePEC.Start();

            guideCounter = 10;
            guideSteps = 5;
            guideDirection = 1;
            if (tmGuideMain != null) tmGuideMain.Stop();
            if (tmGuide != null) tmGuide.Stop();
            tmGuide = new System.Windows.Forms.Timer();


#if true
            tmGuide.Interval = 1000;
            tmGuide.Tick += TmGuide_Tick;

            tmGuideMain = new System.Windows.Forms.Timer();
            tmGuideMain.Interval = (int)(secsPerCycle * 1000.0 / 4.0);  // 4 times per cylce
            tmGuideMain.Tick += TmGuideMain_Tick;
            if (guide)
                tmGuideMain.Start();
#else
            tmGuide.Interval = 2000;
            if (guide)
            {
                tmGuide.Tick += TmGuideRandom_Tick;
                tmGuide.Start();
            }
#endif
            if (use1PPS)
                startTime = lastTime = twoThousand + TimeSpan.FromMilliseconds(gps.MilliSeconds);


        }



        private void TmGuideMain_Tick(object sender, EventArgs e)
        {
            guideCounter = 10;
            guideSteps = 5;
            guideDirection = 1;
            if (tmGuide != null)
                tmGuide.Start();
            else Debugger.Break();
        }

        object lockGuide = new object();

        private void TmGuide_Tick(object sender, EventArgs e)
        {
            if (!GeminiHardware.Instance.Connected || guideDirection > 4)
            {
                tmGuide.Stop();
                return;
            }
            ThreadPool.QueueUserWorkItem(new WaitCallback((a) =>
            {
                //897542;299180   448770
                double east = 12e6 / 897542.0;// * currentPE.arcsecsPerStep * 4 ;

                double west = 12e6 / 299180.0;// * currentPE.arcsecsPerStep * 4;
                double nom = 12e6 / 448770.0;

                east = east / nom;

                west = west / nom;

                //east = 1;

                lock (lockGuide)
                {
                    if (guideCounter-- > 0) return; //nothing to do
                    switch (guideDirection)
                    {
                        case 1: //west
                            //DoPulse(1, (int)(200 * 0.332363135));
                            DoPulse(1, (int)(200 / west));
                            Debug.WriteLine($"{guideDirection} PULSE WEST");
                            guideDirection = 2;
                            break;

                        case 2: //east
                                //DoPulse(-1, (int)(200 * (1- 0.332363135)));
                            DoPulse(-1, (int)(200 / east / west / 2 ) );
                            Debug.WriteLine($"{guideDirection} PULSE EAST");
                            guideDirection = 3;
                            break;
                        case 3:
                            //DoPulse(-1, (int)(200 * (1 - 0.332363135)));
                            DoPulse(-1, (int)(200 / east / west /2  ));
                            Debug.WriteLine($"{guideDirection} PULSE EAST");
                            guideDirection = 4;
                            break;
                        case 4:
                            //DoPulse(1, (int)(200 * 0.332363135));
                            DoPulse(1, (int)(200 / west));
                            Debug.WriteLine($"{guideDirection} PULSE WEST");
                            //guideDirection = 1;
                            guideDirection = 5;
                            break;
                    }

                    guideCounter = guideSteps;
                }
            }));

        }

        Random rand = new Random();

        private void TmGuideRandom_Tick(object sender, EventArgs e)
        {
            if (!GeminiHardware.Instance.Connected || guideDirection > 4)
            {
                tmGuide.Stop();
                return;
            }
            ThreadPool.QueueUserWorkItem(new WaitCallback((a) =>
            {

            lock (lockGuide)
            {
                    // RA
                    int duration = (int)((rand.NextDouble() * 2 - 1) * 300);
                    string cmd = duration > 0 ? ":Mgw" : ":Mge";
                    duration = Math.Abs(duration);
                    if (duration > 100)
                    {
                        string c = cmd + duration.ToString();

                        GeminiHardware.Instance.EndOfPulseGuide = Math.Max(duration, 100);

                        GeminiHardware.Instance.Velocity = "G";
                        GeminiHardware.Instance.DoCommandResult(c, duration + GeminiHardware.Instance.MAX_TIMEOUT, false);

                        if (!GeminiHardware.Instance.AsyncPulseGuide)
                            GeminiHardware.Instance.WaitForVelocity("TN", duration + GeminiHardware.Instance.MAX_TIMEOUT); // shouldn't take much longer than 'Duration', right?
                    }

                    duration = (int)((rand.NextDouble() * 2 - 1) * 300);
                    cmd = duration > 0 ? ":Mgn" : ":Mgs";
                    duration = Math.Abs(duration);
                    if (duration > 100)
                    {
                        string c = cmd + duration.ToString();

                        GeminiHardware.Instance.EndOfPulseGuide = Math.Max(duration, 100);

                        GeminiHardware.Instance.Velocity = "G";
                        GeminiHardware.Instance.DoCommandResult(c, duration + GeminiHardware.Instance.MAX_TIMEOUT, false);

                        if (!GeminiHardware.Instance.AsyncPulseGuide)
                            GeminiHardware.Instance.WaitForVelocity("TN", duration + GeminiHardware.Instance.MAX_TIMEOUT); // shouldn't take much longer than 'Duration', right?
                    }
                }

                guideCounter = guideSteps;
            }
            ));

        }



        void DoPulse(int dir, int duration)
        {
            string cmd = String.Empty;

            switch (dir)
            {
                case -1:
                    cmd = ":Mge";
                    break;
                case 1:
                    cmd = ":Mgw";
                    break;
            }

           
            string c = cmd + duration.ToString();

       
            GeminiHardware.Instance.EndOfPulseGuide = Math.Max(duration, 100);

            GeminiHardware.Instance.Velocity = "G";
            GeminiHardware.Instance.DoCommandResult(c, duration + GeminiHardware.Instance.MAX_TIMEOUT, false);

            if (!GeminiHardware.Instance.AsyncPulseGuide)
                GeminiHardware.Instance.WaitForVelocity("TN", duration + GeminiHardware.Instance.MAX_TIMEOUT); // shouldn't take much longer than 'Duration', right?
        }

        //https://en.wikipedia.org/wiki/Earth%27s_rotation
        const double SiderealRate = 15.041068640261926; // 360 / 23 h 56 m 4.09053083288s 
        const double SiderealDay = (23.0 + 56.0 / 60 + 4.09053083288 / 3600.0);
        double userSiderealRate = 15.041068640261926; // 360 / 23 h 56 m 4.09053083288s 

        DateTime twoThousand =  new DateTime(2000, 1, 1, 1, 0, 0);
        private void TmUpdatePEC_Tick(object sender, EventArgs e)
        {

            if (!GeminiHardware.Instance.Connected)
            {
                tmUpdatePEC.Stop();
                return;
            }
            GeminiHardware.Instance.UpdatePolledVariables(true);
            //            var timeDiff = (DateTime.Now - start).TotalSeconds;

            var specidx = GeminiHardware.Instance.DoCommandResult("<501:", 500, false);



            var g5 = (Gemini5Hardware)GeminiHardware.Instance;
            var mac = g5.macro;
            /*
             *1733048;4608000;3.676841;+30.000000;+11.956425;359.397971;-19.907929;T;T;N;W;15.633267;0;33;7961.280366;1;>:B:DB60;-1;0;-6;4;0;0;
             * 
             *  0 PRA 2591823;
                1 PDEC 2592000;
                2 RA 19.467428;
                3 DEC +0.179764;
                4 HA
             
                5 Az 270.204063;
                6 El -28.246181;
                7 Gv N;
                8 GW N;
                9 Gw N;
                10 Gm E;
                11 GS 3.350518;
                12 h? 0;
                13 PEC 0;
                14 Western limit 50963.891895;
                15 <99: 1;
                16 B076D900;
            */

            var timeDiff = stopWatch.ElapsedMilliseconds / 1000.0;

            if (mac == null) return;    //timeout

            var s = mac.Split(';');
            var pra = Int64.Parse(s[0]);
            DateTime currtime = DateTime.MinValue;

            if (specidx!=null)
            {
                var pecidx = int.Parse(specidx);
                if (use1PPS)
                {
                    currtime = twoThousand + TimeSpan.FromMilliseconds(gps.MilliSeconds);
                    timeDiff = (currtime - startTime).TotalSeconds;
                    Debug.WriteLine($"TimeDiff: {timeDiff} Stopwatch={stopWatch.ElapsedMilliseconds/1000.0} PECIdxDiff={Math.Abs(pecidx - first_updatePECIdx) * currentPE.StepX } praDiff={Math.Abs(pra - first_updatePRA) * currentPE.arcsecsPerStep}");
                }
                else
                if (!usePCClock)
                {
                    var sidereal = double.Parse(s[11], GeminiHardware.Instance.m_GeminiCulture);

                    currtime = twoThousand + TimeSpan.FromHours(sidereal / 24.0 * SiderealDay);
                    lastTime = currtime;

                    timeDiff = (currtime - startTime).TotalSeconds;
                    //                Debug.WriteLine($"TimeDiff: {timeDiff}");
                }
                var time = timeDiff + first_updatePECIdx * currentPE.StepX;

                //time *= currentPE.TimeScale;

                while (time >= secsPerCycle)
                    time -= secsPerCycle;

                pos[pecidx] = pra;

                string ser = $"Series{cycleNumber + 2}";


                if (time < secondsPreviousPoint) // wrap to start, new cycle
                {

                    UpdateStats();

                    cycleNumber++;
                    ser = $"Series{cycleNumber + 2}";

                    if (chrt.Series.FindByName(ser) == null)
                    {
                        chrt.Series.Add(ser);

                    }
                    chrt.Series[ser].XValueType = ChartValueType.Auto;

                    chrt.Series[ser].ChartType = SeriesChartType.Line;
                    chrt.Series[ser].Color = palette[cycleNumber % palette.Length];

                    chrt.Series[ser].MarkerSize = 1;
                    chrt.Series[ser].Points.Clear();
                }

                var p = chrt.Series[ser].Points;
                var x = twoThousand.AddSeconds(time);


                var yy = Math.Abs(-pra + first_updatePRA) * currentPE.arcsecsPerStep - timeDiff * userSiderealRate + first_Y + timeDiff * driftCorrection;

                if (lastN.Count >= 30)
                {
                    ////var min = lastN.Min();
                    ////var mx = lastN.Max();
                    ////lastN.Remove(min);
                    ////lastN.Remove(mx);

                    //var m = lastN.Median();
                    //var std = lastN.StandardDeviation();
                    //if (Math.Abs(yy - m) > std * 2)
                    //    return;
                    lastN.RemoveAt(0);
                }

                lastN.Add(yy);

                if (use1PPS && lastN.Count < 5)
                {
                    first_updatePRA = pra;
                    startTime = currtime;
                    //first_Y = 0;// Math.Abs(-pra + first_updatePRA) * currentPE.arcsecsPerStep - timeDiff * SiderealRate + first_Y + timeDiff * driftCorrection;
                    first_Y = currentPE.Y[pecidx];
                    first_updatePECIdx = pecidx;
                    return;
                }



                if (Math.Abs(yy) > currentPE.arcsecsPerStep * 2) Debug.WriteLine($"YDIFF={yy}");
                p.AddXY(x, yy);
                //                Debug.WriteLine($"at {x} : {(-pra + first_updatePRA) / (2 * (double)half_circle_ra) * 360 * 60.0 * 60.0 - timeDiff * SiderealRate + first_Y}");

                //A.X = x.ToOADate();

                secondsPreviousPoint = time;
            }
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (stopWatch != null)
            {
                var totalTimer = TimeSpan.FromMilliseconds(stopWatch.ElapsedMilliseconds);

                //            tmEnd = tmNST.GetCorrectedSystemLocalTime();


                var totalAtomic = tmEnd - tmStart;

                var totalGemini = lastTime - startTime;


                Debug.WriteLine($"PC Timer: {totalTimer.TotalSeconds}");
                //            Debug.WriteLine($"NIST    : {totalAtomic.TotalSeconds}");
                Debug.WriteLine($"Gemini  : {totalGemini.TotalSeconds}");
            }
            if (tmUpdatePEC!=null)
            {
                tmUpdatePEC.Stop();
                tmUpdatePEC = null;
                if (stopWatch != null)
                    stopWatch.Stop();
                stopWatch = null;
            }

            if (tmGuide != null)
            {
                tmGuide.Stop();
                tmGuide = null;
            }

            if (tmGuideMain != null)
            {
                tmGuideMain.Stop();
                tmGuideMain = null;
            }



            UpdateStats();
        }

        private void UpdateStats()
        {

            
            List<double> xs = new List<double>();
            List<double> ys = new List<double>();
            var startTime = new DateTime(2000, 1, 1, 1, 0, 0);

            int seriesNbr = 0;
            foreach (var s in chrt.Series)
            {
                if (s.Name == "Series1") continue;
                seriesNbr++;
                var xd = s.Points.Select(x => (DateTime.FromOADate(x.XValue) - new DateTime(2000, 1, 1, 1, 0, 0)).TotalSeconds + (currentPE.PECOneWorm * currentPE.StepX) * seriesNbr);

                var yd = s.Points.Select(x => x.YValues[0]);

                xs = xs.Concat(xd).ToList();
                ys = ys.Concat(yd).ToList();
            }

            var saveY = ys.Select(x => x).ToArray();

            if (ys.Count < 5)
            {
                lbDrift.Text = $"need 1+ cycle";
                lbPeakAll.Text = $"{0:0.000}\" pk-pk";
                lbRMSAll.Text = $"{0:0.000}\" rms";
                lbCycle.Text = $"{currentPE.PECOneWorm * currentPE.StepX}sec";
                lbCycles.Text = $"{0}";
                return;

            }

            if (xs.Count <= 3 || ys.Count <= 3) return;

            var interp = MathNet.Numerics.LinearRegression.SimpleRegression.Fit(xs.ToArray(), ys.ToArray());

            for (int i=0;i <ys.Count; ++i)
            {
                ys[i] = ys[i] - interp.Item2 * xs[i];   // remove drift
            }

            var pk =ys.Max() - ys.Min();
            var rms = ys.RootMeanSquare();

            currentPE.X = xs.Select(x => startTime.AddSeconds(x)).ToArray();
            currentPE.Y = saveY.Select(x => (float)x).ToArray();//ys.Select(y => (float)y).ToArray();
            currentPE.UpdateDataFFT();

            var mean = ys.Mean();
            var std3 = ys.StandardDeviation() ;

            var sY = saveY.ToArray();

            List<Tuple<double, double, double>> tup = xs.Select((x, i) => new Tuple<double, double, double>(x, ys[i], sY[i])).ToList();
            tup = tup.Where(x => Math.Abs(x.Item2 - mean) < std3).ToList();

            if (tup.Count <= 3) return;
            var interp2 = MathNet.Numerics.LinearRegression.SimpleRegression.Fit(tup.Select(z => z.Item1).ToArray(), tup.Select(z => z.Item3).ToArray());
            currentPE.Drift = (float)(interp2.Item2 * currentPE.PECOneWorm * currentPE.StepX);

            //for(int i=0; i < sY.Count(); ++i)
            //{
            //    double d = sY[i] - (interp2.Item1 + (tup[i].Item2 * interp.Item2));

            //    if (Math.Abs(d) >= std3)
            //        sY[i] = interp2.Item1 + (tup[i].Item2 * interp.Item2);
            //}
            if (currentPE.NumberOfCycles < 0.5)
                lbDrift.Text = $"not ready";
            else
                lbDrift.Text = $"{interp.Item2 * currentPE.PECOneWorm * currentPE.StepX:0.000}\"/cycle";

            lbPeakAll.Text = $"{pk:0.000}\" pk-pk";
            lbRMSAll.Text = $"{rms:0.000}\" rms";
            lbCycle.Text = $"{currentPE.PECOneWorm * currentPE.StepX}sec";
            lbCycles.Text = $"{currentPE.NumberOfCycles :0.0}";

            chrt.ChartAreas[0].AxisX.Minimum = Double.NaN;
            chrt.ChartAreas[0].AxisX.Maximum = Double.NaN;
            chrt.ChartAreas[0].RecalculateAxesScale();


        }

        private void frmPECChart_FormClosing(object sender, FormClosingEventArgs e)
        {
            stopToolStripMenuItem_Click(sender, e);
            if (gps != null)           
                gps.Connected = false;
            gps = null;
        }

        private void sin2xToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //clear data
            string res = GeminiHardware.Instance.DoCommandResult("<509:", GeminiHardware.Instance.MAX_TIMEOUT, false);
            int v = 0;
            if (int.TryParse(res, out v))
            {
                GeminiHardware.Instance.DoCommand(">509:" + (v & (~32)).ToString(), false);
            }
            this.UseWaitCursor = true;
            Application.DoEvents();
            currentPE.ProgramPEC(new Func<double, double>(x => 10 * Math.Sqrt(Math.Abs(x)) * Math.Sin(2 * x)));
            this.UseWaitCursor = false;
            //            currentPE.ProgramPEC(new Func<double, double>(x => 2 * (4.349 * Math.Sin(x ) + 0.428 * Math.Sin(3 * x ) + 0.338 * Math.Sin( 4 * x ))));

            GeminiHardware.Instance.DoCommand(">509:32", false);

        }

        private void sinTCossqrt3tToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string res = GeminiHardware.Instance.DoCommandResult("<509:", GeminiHardware.Instance.MAX_TIMEOUT, false);
            int v = 0;
            if (int.TryParse(res, out v))
            {
                GeminiHardware.Instance.DoCommand(">509:" + (v & (~32)).ToString(), false);
            }
            currentPE.ProgramPEC(new Func<double, double>(x =>
            {
                double value;

                value = 5 * Math.Sin(0 + 1 * x);
                value += 5 * Math.Sin(Math.PI / 4 + 2 * x);
                return (value) * Math.PI;
            }));
//            currentPE.ProgramPEC(new Func<double, double>(x => 10 * (Math.Sin( 1.5 * (x ) - 0.5) + Math.Cos((1.5 * (x ) - 0.5) * Math.Sqrt(3)))));
            GeminiHardware.Instance.DoCommand(">509:32", false);

        }


        public static Color[] LoadPalette(string spectrogramPalette, int size)
        {
            //https://github.com/NanoComp/h5utils/blob/master/doc/h5topng-colors.md
            string p = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

            p = Path.Combine(p, "ColorMaps", spectrogramPalette);
            //MessageBox.Show(p);
            var pal = new Color[size];

            try
            {

                if (File.Exists(p))
                {
                    string[] lines = File.ReadAllLines(p);
                    double[] r, g, b, a, x;

                    var r0 = lines.ToList();

                    while (r0[0].StartsWith("#"))
                        r0.RemoveAt(0); //first line is headers

                    int cnt = r0.Count;
                    r = new double[cnt];
                    g = new double[cnt];
                    b = new double[cnt];
                    a = new double[cnt];
                    x = new double[cnt];

                    int i = 0;
                    foreach (var l in r0)
                    {
                        string[] v = l.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        CultureInfo culture = new CultureInfo("en-US");
                        r[i] = double.Parse(v[0], culture);
                        g[i] = double.Parse(v[1], culture);
                        b[i] = double.Parse(v[2], culture);
                        a[i] = double.Parse(v[3], culture);
                        x[i] = (double)i / (double)(r0.Count - 1);
                        ++i;
                    }


                    var ri = LinearSpline.Interpolate(x, r);
                    var gi = LinearSpline.Interpolate(x, g);
                    var bi = LinearSpline.Interpolate(x, b);
                    var ai = LinearSpline.Interpolate(x, a);
                   

                    for (int j = 0; j < size; ++j)
                    {
                        double rc = ri.Interpolate((double)j / (double)size);
                        double gc = gi.Interpolate((double)j / (double)size);
                        double bc = bi.Interpolate((double)j / (double)size);
                        double ac = ai.Interpolate((double)j / (double)size);
                        //ac = 1.0;

                        pal[j] = (Color.FromArgb((byte)(ac * 255), (byte)(rc * 255), (byte)(gc * 255), (byte)(bc * 255)));
                    }
                    return pal;
                }
            }
            catch (Exception ex)
            {
            }

            return null;

        }

        private void alignVerticallyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> remove = new List<string>();
            var av = chrt.Series["Series1"].Points.Average(x=>x.YValues[0]);

            int mx = 0;
            foreach (var s in chrt.Series)
            {
                if (s.Name == "Series1") continue;
                mx = Math.Max(s.Points.Count, mx);
            }


            foreach (var s in chrt.Series)
            {
                if (s.Name == "Series1") continue;
                if (s.Points.Count < mx * 0.99)
                {
                    remove.Add(s.Name);
                    continue;
                }
                var av2 = s.Points.Average(x => x.YValues[0]);
                av2 = av2 - av;
                for(int i=0; i<s.Points.Count; ++i)
                {
                    s.Points[i].YValues[0] -= av2;
                }
                
            }

            foreach( var nm in remove)
            {
                var s = chrt.Series.FindByName(nm);
                chrt.Series.Remove(s);
            }

            chrt.Refresh();
        }

        //drift = 0.0010891578523192856
        private void setDriftCorrectionFromDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tmUpdatePEC != null)
                stopToolStripMenuItem_Click(sender, e); // stop collecting data and compute statistics

            //var s = chrt.Series.Last().Points;

            //var line = MathNet.Numerics.Fit.Line(s.Select(x => (DateTime.FromOADate(x.XValue) - twoThousand).TotalSeconds).ToArray(), s.Select(y => y.YValues[0]).ToArray());
            double drift = currentPE.Drift ;
            if (currentPE.NumberOfCycles < 1)
            {
                MessageBox.Show("Need at least 1 worm cycle for drift calculation!");
                return;
            }
            var clk0 = GeminiHardware.Instance.DoCommandResult("<70:", 2000, false);
            if (clk0 == null) return;

            int c0 = 12000000;
            int.TryParse(clk0, out c0);

            //if (c0 != 12000000)
            //{
            //    MessageBox.Show("Current clock is not set to default 12000000!");
            //}
            var normalCycle = currentPE.WormPeriodSeconds * SiderealRate;   //arcsecs per cycle
            var adjCycle = normalCycle + drift; // new arcsecs per cycle
            var adjSteps = adjCycle / currentPE.WormPeriodSeconds; //new arcsecs/sec
            var adjStepsPerSec = adjSteps / currentPE.arcsecsPerStep / currentPE.EncoderMultiplier;
            var adjClock = (int)Math.Round(currentPE.RateDivisor * adjStepsPerSec);

            if (adjClock > 12000000 + 11717 || adjClock < 12000000 - 11717)
            {
                var r1 = MessageBox.Show($"Drift = {drift:0.00000}arcsec/sec.\r\n\r\nDrift is too large to correct using Gemini Clock correction function!\r\nNew clock would have been {adjClock:0}");

                return;
            }

            var r = MessageBox.Show($"Drift = {drift:0.00000}arcsec/sec.\r\n\r\nWould you like to correct Gemini Clock speed to remove it?\r\nNew clock will be {adjClock:0}",
                     "Drift Removal", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            
            
            //driftCorrection = -drift;
            if (r ==  DialogResult.Yes)
            {
                GeminiHardware.Instance.DoCommand($">70:{adjClock}", false);
                System.Threading.Thread.Sleep(2000);
                var clk = GeminiHardware.Instance.DoCommandResult("<70:", 2000, false);
                if (clk != adjClock.ToString())
                {
                    MessageBox.Show("Clock change failed, drift not corrected!");
                    return;
                }
                //MessageBox.Show("New clock set!\r\n\r\nGemini will be rebooted now for the new setting to take effect.");
                //GeminiHardware.Instance.DoCommandResult(">65535:", GeminiHardware.Instance.MAX_TIMEOUT, false);
                GeminiHardware.Instance.DoCommandResult(">137:", GeminiHardware.Instance.MAX_TIMEOUT, false);
                GeminiHardware.Instance.DoCommandResult(">131:", GeminiHardware.Instance.MAX_TIMEOUT, false);
            }
        }


        int xPos, yPos;

        private void chrt_KeyDown(object sender, KeyEventArgs e)
        {
           
        }

        private void ZoomIn(int xZ, int yZ, int xPos, int yPos)
        {
            var xAxis = chrt.ChartAreas[0].AxisX;
            var yAxis = chrt.ChartAreas[0].AxisY;

            xMin = xAxis.ScaleView.ViewMinimum;
            xMax = xAxis.ScaleView.ViewMaximum;
            yMin = yAxis.ScaleView.ViewMinimum;
            yMax = yAxis.ScaleView.ViewMaximum;

            double delta = 20;

            int IntervalX = 1;
            int IntervalY = 1;
            try
            {
                if (xZ != 0)
                {
                    if (xZ < 0) // Zoom out in X
                    {
                        var posXStart = xMin - (xMax - xMin) / (delta); //Math.Pow(1, numberOfZoom);
                        var posXFinish = xMax + (xMax - xMin) / (delta); // Math.Pow(1, numberOfZoom);
               
                        xAxis.ScaleView.Zoom(posXStart, posXFinish);
                        numberOfZoom--;
                    }                  
                    else if (xZ > 0) // Zoom in in X
                    {

                        var posXStart = xMin + (xMax - xMin) / (delta); // Math.Pow(1, numberOfZoom);
                        var posXFinish = xMax - (xMax - xMin) / (delta);  //Math.Pow(1, numberOfZoom);                     
                        xAxis.ScaleView.Zoom(posXStart, posXFinish);
                    }
                }

                if (yZ != 0)
                {
                    if (yZ < 0) // Zoom out in Y
                    {
                        var posYStart = yMin - (yMax - yMin) / (delta); //Math.Pow(1, numberOfZoom);
                        var posYFinish = yMax + (yMax - yMin) / (delta); // Math.Pow(1, numberOfZoom);

                        yAxis.ScaleView.Zoom(posYStart, posYFinish);
                        numberOfZoom--;
                    }
                    else if (yZ > 0) // Zoom in in X
                    {

                        var posYStart = yMin + (yMax - yMin) / (delta); // Math.Pow(1, numberOfZoom);
                        var posYFinish = yMax - (yMax - yMin) / (delta);  //Math.Pow(1, numberOfZoom);                     
                        yAxis.ScaleView.Zoom(posYStart, posYFinish);
                    }
                }

                if (numberOfZoom < 0) numberOfZoom = 0;
            }
            catch { }

        }

        private void chrt_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void chrt_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
                ZoomIn(0, 1, xPos, yPos);
            else if (e.KeyCode == Keys.Down)
                ZoomIn(0, -1, xPos, yPos);
            else if (e.KeyCode == Keys.Left)
                ZoomIn(-1, 0, xPos, yPos);
            else if (e.KeyCode == Keys.Right)
                ZoomIn(1, 0, xPos, yPos);

        }

        private void loadFromPEMProFileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void programToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void tSqrttSiin2tToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //clear data
            string res = GeminiHardware.Instance.DoCommandResult("<509:", GeminiHardware.Instance.MAX_TIMEOUT, false);
            int v = 0;
            if (int.TryParse(res, out v))
            {
                GeminiHardware.Instance.DoCommand(">509:" + (v & (~32)).ToString(), false);
            }
            this.UseWaitCursor = true;
            Application.DoEvents();
            currentPE.ProgramPEC(new Func<double, double>(x => x + 10 * Math.Sqrt(Math.Abs(x)) * Math.Sin(2 * x)));
            this.UseWaitCursor = false;
            //            currentPE.ProgramPEC(new Func<double, double>(x => 2 * (4.349 * Math.Sin(x ) + 0.428 * Math.Sin(3 * x ) + 0.338 * Math.Sin( 4 * x ))));

            GeminiHardware.Instance.DoCommand(">509:32", false);

        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            startMeasure(true);
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            stopToolStripMenuItem_Click(sender, e);
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            alignVerticallyToolStripMenuItem_Click(sender, e);
        }

        private void sqrttsin5tToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string res = GeminiHardware.Instance.DoCommandResult("<509:", GeminiHardware.Instance.MAX_TIMEOUT, false);
            int v = 0;
            if (int.TryParse(res, out v))
            {
                GeminiHardware.Instance.DoCommand(">509:" + (v & (~32)).ToString(), false);
            }
            this.UseWaitCursor = true;
            Application.DoEvents();
            currentPE.ProgramPEC(new Func<double, double>(x => 5 * Math.Sqrt(Math.Abs(x)) * Math.Sin(5 * x)), 1, 20);
            this.UseWaitCursor = false;
            //            currentPE.ProgramPEC(new Func<double, double>(x => 2 * (4.349 * Math.Sin(x ) + 0.428 * Math.Sin(3 * x ) + 0.338 * Math.Sin( 4 * x ))));

            GeminiHardware.Instance.DoCommand(">509:32", false);
        }


        private void use1PPSGPSClockToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            use1PPSClockToolStripMenuItem.Checked = true;
            usePCClockToolStripMenuItem.Checked = false;
            useGeminiClockMenuItem.Checked = false;

            usePCClock = false;
            use1PPS = true;
        }

        private void usePCClockToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            use1PPSClockToolStripMenuItem.Checked = false;
            usePCClockToolStripMenuItem.Checked = true;
            useGeminiClockMenuItem.Checked = false;
            usePCClock = true;
            use1PPS = false;
        }

        private void useGeminiClock_Click(object sender, EventArgs e)
        {
            use1PPSClockToolStripMenuItem.Checked = false;
            usePCClockToolStripMenuItem.Checked = false;
            useGeminiClockMenuItem.Checked = true;
            usePCClock = false;
            use1PPS = false;

        }

        System.Windows.Forms.Timer tTest = new System.Windows.Forms.Timer();

        double msec = 0;

        private void testToolStripMenuItem1_Click(object sender, EventArgs e)
        {
           
         
        }

        private void TTest_Tick(object sender, EventArgs e)
        {
            if (msec == 0)
            {
                tTest.Stop();
                tTest.Interval = 240000;
                stopWatch = new Stopwatch();
                //gps.MilliSeconds = 0;
                msec = gps.MilliSeconds;
                stopWatch.Start();
                tTest.Start();
                return;
            }
            

            var m = gps.MilliSeconds;
            var s = stopWatch.Elapsed.TotalMilliseconds;

            tTest.Stop();
            gps.Connected = false;
            gps = null;

            MessageBox.Show($"After {s / 1000:0.000}sec StopWatch={s} Gps={m - msec}\r\nDrift: {(m - msec - s)/(m - msec)*239.433 * SiderealRate}\"/cycle");
        }

        private void testToolStripMenuItem1_Click_1(object sender, EventArgs e)
        {
            gps = new NmeaInterpreter();
            gps.BaudRate = GeminiHardware.Instance.GpsBaudRate;
            gps.ComPort = GeminiHardware.Instance.GpsComPort;
            gps.Connected = true;
            msec = 0;
            tTest.Interval = 3000;
            tTest.Tick += TTest_Tick;
            tTest.Start();
        }

        private void preciseTimeToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            if (usePCClock == true)
            {
                use1PPSClockToolStripMenuItem.Checked = false;
                usePCClockToolStripMenuItem.Checked = true;
                useGeminiClockMenuItem.Checked = false;
            }
            else if (use1PPS == true)
            {
                use1PPSClockToolStripMenuItem.Checked = true;
                usePCClockToolStripMenuItem.Checked = false;
                useGeminiClockMenuItem.Checked = false;
            }
            else
            {
                use1PPSClockToolStripMenuItem.Checked = false;
                usePCClockToolStripMenuItem.Checked = false;
                useGeminiClockMenuItem.Checked = true;
            }
        }

        private void setSiderealRateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSiderealRate frm = new frmSiderealRate();

            frm.siderealRate = userSiderealRate;
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                userSiderealRate = frm.siderealRate;
                LoadFileGemini(null);
//                UpdateDataTime();
            }
        }

        private void sqrttSin10tToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string res = GeminiHardware.Instance.DoCommandResult("<509:", GeminiHardware.Instance.MAX_TIMEOUT, false);
            int v = 0;
            if (int.TryParse(res, out v))
            {
                GeminiHardware.Instance.DoCommand(">509:" + (v & (~32)).ToString(), false);
            }
            this.UseWaitCursor = true;
            Application.DoEvents();
            //currentPE.ProgramPEC(new Func<double, double>(x => 3 * Math.Sqrt(Math.Abs(x)) * Math.Sin(10 * x)),1,20);

            currentPE.ProgramPEC(new Func<double, double>(x => 
            
            10 * (Math.Abs(Math.Sin(3 * x)) > 0.5? 1 : 0) * Math.Sin(3 * x)), 1, 1);


            this.UseWaitCursor = false;

            GeminiHardware.Instance.DoCommand(">541:", false);
            GeminiHardware.Instance.DoCommand(">509:32", false);
        }

        private void chrt_MouseDown(object sender, MouseEventArgs e)
        {
            xPos = e.X;
            yPos = e.Y;   
        }
    }


}
