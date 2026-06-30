using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Linq;
using MathNet;
using MathNet.Numerics;
using MathNet.Numerics.Statistics;
using System.Diagnostics;
using MathNet.Numerics.Interpolation;

namespace ASCOM.GeminiTelescope
{
    public class PEClass
    {
        public DateTime[] X = null;
        public float[] Y = null;
        public Complex32[] fft = null;
        public Complex32[] fftFiltered = null;

        public List<int> selectedBins = new List<int>();
        public float[] XF = null;
        public float[] YF = null;

        public double StepX;  //in seconds
        public double arcsecsPerStep; 
        public float WormPeriodSteps; // number of steps per worm period 
        public int WormPeriods; //number of worm periods
        public string FileName;
        public int PECOneWorm;

        double _wormPeriodSeconds;
        public double WormPeriodSeconds
        {
            get
            {
                return _wormPeriodSeconds * TimeScale;
            }

            set
            {
                _wormPeriodSeconds = value;
            }
        }

        public int PECMax = 0;

        public int fft_size;
        public double MaxX;
        public float PeakAll;
        public float RMSAll;
        public float Drift;
        public int RateDivisor;
        public int EncoderMultiplier;

        public double TimeScale = 1.0;

        double[] stepsPerSecond;
        double[] guideDivisor;

        Random rand = new Random();


        public bool LoadFromGemini(int deg = 0)
        {

            frmProgress.Initialize(0, 100, "Getting PEC Data from Gemini", null);
            frmProgress.ShowProgress(null);

            GetPECStatusFromGemini();

            List<double> yVal = new List<double>();
            List<DateTime> xVal = new List<DateTime>();

            double y = 0;
            DateTime x0 = new DateTime(2000, 1, 1, 1, 0, 0);
            DateTime x = x0;

            xVal.Add(x);
            yVal.Add((float)y);

            string guide = GeminiHardware.Instance.DoCommandResult("<502:", GeminiHardware.Instance.MAX_TIMEOUT, false);
            double guideRate = 0.5;
            GeminiHardware.Instance.m_Util.StringToDouble(guide, out guideRate);
            if (guideRate < 0.2 || guideRate > 0.9)
            {
                GeminiHardware.Instance.Trace.Error("frmPEC Error", $"Guide rate is invalid: {guide}, using 0.5 instead");
                guideRate = 0.5;
            }
            string rateDivisor = GeminiHardware.Instance.DoCommandResult("<411:", GeminiHardware.Instance.MAX_TIMEOUT, false);
            string rateSlowDivisor = GeminiHardware.Instance.DoCommandResult("<413:", GeminiHardware.Instance.MAX_TIMEOUT, false);
            string rateFastDivisor = GeminiHardware.Instance.DoCommandResult("<414:", GeminiHardware.Instance.MAX_TIMEOUT, false);

            string wormGearRatio = GeminiHardware.Instance.DoCommandResult("<21:", GeminiHardware.Instance.MAX_TIMEOUT, false);
            string spurGearRatio = GeminiHardware.Instance.DoCommandResult("<23:", GeminiHardware.Instance.MAX_TIMEOUT, false);
            string encoderResolution = GeminiHardware.Instance.DoCommandResult("<25:", GeminiHardware.Instance.MAX_TIMEOUT, false);

            stepsPerSecond = new double[3];
            guideDivisor = new double[3];
            double frequency = GeminiHardware.Instance.GeminiLevel >= 6 ? 12000000 : 1.5e6;

            RateDivisor = 448771;
            int.TryParse(rateDivisor, out RateDivisor);

            if (spurGearRatio != null && wormGearRatio != null && encoderResolution != null)
            {
                stepsPerSecond[1] = (frequency) / (guideDivisor[1] = double.Parse(rateDivisor));
                stepsPerSecond[0] = (frequency) / (guideDivisor[0] = double.Parse(rateSlowDivisor));
                stepsPerSecond[2] = (frequency) / (guideDivisor[2] = double.Parse(rateFastDivisor));       
                arcsecsPerStep = 1296000.00 / (Math.Abs(double.Parse(wormGearRatio)) * double.Parse(spurGearRatio) * double.Parse(encoderResolution));
            }

            EncoderMultiplier = 1;

            if (GeminiHardware.Instance.ServoFirmwareVersion(true) >= 2)
            {
                string raMultiplier = GeminiHardware.Instance.DoCommandResult("<401:", GeminiHardware.Instance.MAX_TIMEOUT, false);
                int m = 0;
                int.TryParse(raMultiplier, out m);
                if ((m & 1) != 0)
                {
                    stepsPerSecond[0] *= 4;        //account for 4x multiplier
                    stepsPerSecond[1] *= 4;        //account for 4x multiplier
                    stepsPerSecond[2] *= 4;        //account for 4x multiplier
                    arcsecsPerStep /= 4;
                    EncoderMultiplier = 4;
                }
            }

            var ung = GeminiHardware.Instance.DoCommandResult("<513:", 5000, false);
            double unguided = guideDivisor[1];
            double.TryParse(ung, out unguided);

            int cnt = 0;

#if true
            double divisor = guideDivisor[1];
            double scaleDn = guideDivisor[0] / guideDivisor[1];
            double scaleUp = guideDivisor[2] / guideDivisor[1];
            for (int i = 0; i < PECMax;)
            {
                string res = GeminiHardware.Instance.DoCommandResult($"<511:{i};1", GeminiHardware.Instance.MAX_TIMEOUT, false);
                
                if (res != null)
                {
                    string[] p = res.Split(';');
                    int rep = 0, v = 0;

                    int.TryParse(p[0], out v); //divisor
                    int.TryParse(p[1], out rep); //reps

                    if (rep == 0) break;

                    divisor = v;

                    //if (divisor == unguided)
                    //{
                      
                    //    x = x0.AddSeconds(cnt * unguided / frequency / multiplier);
                    //    var diff = 12e6 * arcsecsPerStep * StepX * multiplier * (1 / divisor - 1 / unguided);
                    //    y += diff < 0 ? diff * scaleDn : diff * scaleUp;
                    //    xVal.Add(x);
                    //    yVal.Add((float)y);

                    //    cnt += rep;
                    //    x = x0.AddSeconds(cnt * unguided / frequency / multiplier);
                    //    diff = 12e6 * arcsecsPerStep * StepX * multiplier * (1 / divisor - 1 / unguided);
                    //    y += diff < 0 ? diff * scaleDn : diff * scaleUp;
                    //    xVal.Add(x);
                    //    yVal.Add((float)y);
                    //}
                    //else
                        for (int j = i; j < i + rep; ++j, ++cnt)
                        {
                            x = x0.AddSeconds(cnt * unguided / frequency / EncoderMultiplier);
                            var diff = 12e6 * arcsecsPerStep * StepX * EncoderMultiplier * (1 / divisor - 1 / unguided);
                            y += diff < 0 ? diff * scaleDn : diff * scaleUp;
                            xVal.Add(x);
                            yVal.Add((float)y);
                        }
                    i += rep;
                    frmProgress.Update((i * 100) / PECMax, null);
                }
                else break;
            }
#else
            for (int i = 0; i < PECMax;)
            {
                string res = GeminiHardware.Instance.DoCommandResult("<511:" + i.ToString(), GeminiHardware.Instance.MAX_TIMEOUT, false);

                if (res != null)
                {
                    string[] p = res.Split(';');
                    int rep = 0, v = 0;

                    int.TryParse(p[0], out v);
                    int.TryParse(p[1], out rep);
                    if (rep <= 0) break;
                    //if (rep > 1) Debugger.Break();

                    if (v == 1) v = -1;
                    else
                        if (v == 8) v = 1;
                    else v = 0;
                    for (int j = i; j < i + rep; ++j, ++cnt)
                    {
                        x = x0.AddSeconds(cnt / stepsPerSecond[1]);
                        y += (float)((guideRate * v * (v > 0 ? 1.0 / (1.0 + guideRate) : 1.0 / (1.0 - guideRate))) * arcsecsPerStep);
                        xVal.Add(x);
                        yVal.Add((float)y);
                    }
                    i += rep;
                    frmProgress.Update((i * 100) / PECMax, null);
                }
                else break;
            }
#endif
            if (PECOneWorm == 0) PECOneWorm = 6400;

            FileName = "gemini://CurrPEC.pec";

            WormPeriodSteps = PECMax;
            WormPeriods = PECMax / PECOneWorm;
            StepX = (1.0 / stepsPerSecond[1]);

            xVal.Add(x0.AddSeconds(PECMax * unguided / frequency / EncoderMultiplier * TimeScale));
            yVal.Add(yVal.Last());


            IInterpolation interp = null;

            //if (xVal.Count > 5)
            //    interp = MathNet.Numerics.Interpolate.CubicSplineRobust(xVal.Select(z => z.ToOADate()).ToArray(), yVal.ToArray());

            if (xVal.Count > 5)
                interp = MathNet.Numerics.Interpolate.Linear(xVal.Select(z => z.ToOADate()).ToArray(), yVal.ToArray());


            this.X = new DateTime[PECMax];
            this.Y = new float[PECMax];

            x = x0;
            for (int i=0; i < PECMax; ++i)
            {
                //if (i == 5032) Debugger.Break();

                x = x0.AddSeconds(i * unguided / frequency / EncoderMultiplier);
                X[i] = x; // xVal[i];
                Y[i] = interp != null? (float)(interp.Interpolate(x.ToOADate())) : 0;// + (rand.NextDouble() * 2 - 1) * arcsecsPerStep / 2);
                if (double.IsNaN(Y[i])) Y[i] = (float)yVal[i];
                //Y[i] = (float)yVal[i];
            }

            //X = xVal.ToArray();
            //Y = yVal.Select(z => (float)z).ToArray();

            UpdateDataFFT(deg);

            frmProgress.HideProgress();
            return true;
        }

        public void GetPECStatusFromGemini()
        {
            for (int i = 0; i < 3; ++i)
            {
                string pecmax = GeminiHardware.Instance.DoCommandResult("<503:", GeminiHardware.Instance.MAX_TIMEOUT, false);
                if (int.TryParse(pecmax, out PECMax)) break;
            }

            string wormPeriod = GeminiHardware.Instance.DoCommandResult("<27:", GeminiHardware.Instance.MAX_TIMEOUT, false);
            int.TryParse(wormPeriod, out PECOneWorm);
            if (PECOneWorm == 0) PECOneWorm = 6400;


            int max = 25600;
            if (GeminiHardware.Instance.GeminiLevel > 5)
            {
                // no limit on PEC table in L6
                max = PECOneWorm * 4;   // up to 4x worm cycle
            }

            if (PECMax <= 0)
            {
                PECMax = PECOneWorm;
            }

  
            string rateDivisor = GeminiHardware.Instance.DoCommandResult("<411:", GeminiHardware.Instance.MAX_TIMEOUT, false);
            string rateSlowDivisor = GeminiHardware.Instance.DoCommandResult("<413:", GeminiHardware.Instance.MAX_TIMEOUT, false);
            string rateFastDivisor = GeminiHardware.Instance.DoCommandResult("<414:", GeminiHardware.Instance.MAX_TIMEOUT, false);

            string wormGearRatio = GeminiHardware.Instance.DoCommandResult("<21:", GeminiHardware.Instance.MAX_TIMEOUT, false);
            string spurGearRatio = GeminiHardware.Instance.DoCommandResult("<23:", GeminiHardware.Instance.MAX_TIMEOUT, false);
            string encoderResolution = GeminiHardware.Instance.DoCommandResult("<25:", GeminiHardware.Instance.MAX_TIMEOUT, false);

            stepsPerSecond = new double[3];

            if (spurGearRatio != null && wormGearRatio != null && encoderResolution != null)
            {

                double frequency = 12000000;

                stepsPerSecond[1] = (frequency) / double.Parse(rateDivisor);
                stepsPerSecond[0] = (frequency) / double.Parse(rateSlowDivisor);
                stepsPerSecond[2] = (frequency) / double.Parse(rateFastDivisor);

                arcsecsPerStep = 1296000.00 / (Math.Abs(double.Parse(wormGearRatio)) * double.Parse(spurGearRatio) * double.Parse(encoderResolution));

                WormPeriodSeconds = 360.0 / SiderealRate * 3600.0 / Math.Abs(double.Parse(wormGearRatio));
            }

            if (GeminiHardware.Instance.ServoFirmwareVersion(true) >= 2)
            {
                string raMultiplier = GeminiHardware.Instance.DoCommandResult("<401:", GeminiHardware.Instance.MAX_TIMEOUT, false);
                int m = 0;
                int.TryParse(raMultiplier, out m);
                if ((m & 1) != 0)
                {
                    stepsPerSecond[0] *= 4;        //account for 4x multiplier
                    stepsPerSecond[1] *= 4;        //account for 4x multiplier
                    stepsPerSecond[2] *= 4;        //account for 4x multiplier
                    //arcsecsPerStep /= 4;
                }
            }


            WormPeriodSteps = PECMax;
            WormPeriods = PECMax / PECOneWorm;
            StepX = (1.0 / stepsPerSecond[1]);

        }

        internal bool UpdateDataFFT(int deg = 0)
        {
            fft_size = 8192;
            while (fft_size < WormPeriodSteps) fft_size *= 2;
            fft_size = (int)WormPeriodSteps;
            var startTime = new DateTime(2000, 1, 1, 1, 0, 0);
            var xd = X.Select(x => (x - startTime).TotalSeconds / StepX);
            var yd = Y.Select(z => (double)z);
            var maxXd = xd.Max();
            MaxX = maxXd;

            Func<double, double> poly = null;

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


            MathNet.Numerics.Interpolation.IInterpolation interp = null;
            if (xd.Count() < 5 || yd.Count() < 5) return false;

            try
            {
                interp = MathNet.Numerics.Interpolate.Linear(xd, yd);
            }
            catch (Exception ex)
            {
                GeminiHardware.Instance.Trace.Error("PEC Error", ex.Message);
            }
            //fft_size = (int)maxXd;
            var y = new float[fft_size];

            int start = (int)xd.Min();
            int end = (int)xd.Max();

            for (int i = start, j=0; i < end && j < fft_size ; ++i, ++j)
            {
                double z = interp == null ? 0 : interp.Interpolate(i) - poly(i);
                if (double.IsNaN(z)) z = 0;
                y[j] = (float)z;
            }
            PeakAll = y.Max() - y.Min();
            RMSAll = (float)y.RootMeanSquare();

            //Drift = (float)((poly(1) - poly(0)) * PECOneWorm);   // arcsec/sec

            //Drift = (float)((interp.Interpolate(end) - interp.Interpolate(start+1)) / (end - start) * (double)PECOneWorm * StepX) ;

            var compl = y.Select(v => (Complex32)v).ToArray();

            MathNet.Numerics.IntegralTransforms.Fourier.Forward(compl, MathNet.Numerics.IntegralTransforms.FourierOptions.NoScaling);

            fft = compl;
            return true;
        }

        public bool SendToGemini(IInterpolation interp, int deg = 0)
        {

            frmProgress.Initialize(0, 100, "Sending PEC Data to Gemini", null);
            frmProgress.ShowProgress(null);


            for (int i = 0; i < 3; ++i)
            {
                string pecmax = GeminiHardware.Instance.DoCommandResult("<503:", GeminiHardware.Instance.MAX_TIMEOUT, false);
                if (int.TryParse(pecmax, out PECMax)) break;
            }

            string wormPeriod = GeminiHardware.Instance.DoCommandResult("<27:", GeminiHardware.Instance.MAX_TIMEOUT, false);
            int.TryParse(wormPeriod, out PECOneWorm);
            if (PECOneWorm == 0) PECOneWorm = 6400;


            int max = 25600;
            if (GeminiHardware.Instance.GeminiLevel > 5)
            {
                // no limit on PEC table in L6
                max = PECOneWorm * 4;   // up to 4x worm cycle
            }

            if (PECMax <= 0)
            {
                PECMax = PECOneWorm;
            }



            List<float> yVal = new List<float>();
            List<DateTime> xVal = new List<DateTime>();

            float y = 0;
            DateTime x = new DateTime(2000, 1, 1, 1, 0, 0);

            xVal.Add(x);
            yVal.Add(y);

            string guide = GeminiHardware.Instance.DoCommandResult("<502:", GeminiHardware.Instance.MAX_TIMEOUT, false);
            double guideRate = 0.5;
            GeminiHardware.Instance.m_Util.StringToDouble(guide, out guideRate);
            if (guideRate < 0.2 || guideRate > 0.9)
            {
                GeminiHardware.Instance.Trace.Error("frmPEC Error", $"Guide rate is invalid: {guide}, using 0.5 instead");
                guideRate = 0.5;
            }
            string rateDivisor = GeminiHardware.Instance.DoCommandResult("<411:", GeminiHardware.Instance.MAX_TIMEOUT, false);
            string rateSlowDivisor = GeminiHardware.Instance.DoCommandResult("<413:", GeminiHardware.Instance.MAX_TIMEOUT, false);
            string rateFastDivisor = GeminiHardware.Instance.DoCommandResult("<414:", GeminiHardware.Instance.MAX_TIMEOUT, false);

            string wormGearRatio = GeminiHardware.Instance.DoCommandResult("<21:", GeminiHardware.Instance.MAX_TIMEOUT, false);
            string spurGearRatio = GeminiHardware.Instance.DoCommandResult("<23:", GeminiHardware.Instance.MAX_TIMEOUT, false);
            string encoderResolution = GeminiHardware.Instance.DoCommandResult("<25:", GeminiHardware.Instance.MAX_TIMEOUT, false);

            double arcSecondsPerStep = 0;
            stepsPerSecond = new double[3];

            if (spurGearRatio != null && wormGearRatio != null && encoderResolution != null)
            {

                double frequency = 12000000;

                stepsPerSecond[1] = (frequency) / double.Parse(rateDivisor);
                stepsPerSecond[0] = (frequency) / double.Parse(rateSlowDivisor);
                stepsPerSecond[2] = (frequency) / double.Parse(rateFastDivisor);

                arcSecondsPerStep = 1296000.00 / (Math.Abs(double.Parse(wormGearRatio)) * double.Parse(spurGearRatio) * double.Parse(encoderResolution));
            }

            if (GeminiHardware.Instance.ServoFirmwareVersion(true) >= 2)
            {
                string raMultiplier = GeminiHardware.Instance.DoCommandResult("<401:", GeminiHardware.Instance.MAX_TIMEOUT, false);
                int m = 0;
                int.TryParse(raMultiplier, out m);
                if ((m & 1) != 0)
                {
                    stepsPerSecond[0] *= 4;        //account for 4x multiplier
                    stepsPerSecond[1] *= 4;        //account for 4x multiplier
                    stepsPerSecond[2] *= 4;        //account for 4x multiplier
                    arcSecondsPerStep /= 4;
                }
            }


            for (int i = 0; i < PECMax;)
            {
                string res = GeminiHardware.Instance.DoCommandResult("<511:" + i.ToString(), GeminiHardware.Instance.MAX_TIMEOUT, false);

                if (res != null)
                {
                    string[] p = res.Split(';');
                    int rep = 0, v = 0;

                    int.TryParse(p[0], out v);
                    int.TryParse(p[1], out rep);
                    if (rep <= 0) break;

                    if (v == 1) v = -1;
                    else
                        if (v == 8) v = 1;
                    else v = 0;
                    for (int j = i; j < i + rep; ++j)
                    {
                        x = x.AddSeconds(1.0 / stepsPerSecond[v + 1]);
                        y += (float)((guideRate * v * (v > 0 ? 1.0 / (1.0 + guideRate) : 1.0 / (1.0 - guideRate))) * arcSecondsPerStep);
                        xVal.Add(x);
                        yVal.Add(y);
                    }
                    i += rep;
                    frmProgress.Update((i * 100) / PECMax, null);
                }
                else break;
            }
            if (PECOneWorm == 0) PECOneWorm = 6400;

            FileName = "gemini://CurrPEC.pec";

            WormPeriodSteps = PECMax;
            WormPeriods = PECMax / PECOneWorm;
            StepX = (1.0 / stepsPerSecond[1]);
            this.X = xVal.ToArray();
            this.Y = yVal.ToArray();


            UpdateDataFFT(deg);

            frmProgress.HideProgress();
            return true;
        }
        public bool LoadFromPempro(string file, int deg = 0)
        {
            string[] lines = null;
            try
            {
                lines = File.ReadAllLines(file);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Can't load file '{file}':\r\n\r\n{ex}", SharedResources.TELESCOPE_DRIVER_NAME, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return false;
            }

            GetPECStatusFromGemini();

            double scale = 1.0;
            double rotation = 0;
            double dec = 0;
            double guideRate = 0.5;
            double pecMax = 6400;
            double spur = 25;
            double encoder = 256;
            int cycles = 1;
            double stepSize = 0.5625;
            double startIndex = 0;
            double wormPeriod = 239.3447;
            double wormTeeth = 360;
            long cycleTime = TimeSpan.FromSeconds(wormPeriod).Ticks;
            double oX = 0, oY = 0;
            DateTime x0 = new DateTime(2000, 1, 1, 1, 0, 0);
            double pecIndex = 0;
            long phaseOffset = 0;
            double pecTablePeriod = 0;
            TimeSpan initTimeSpan = TimeSpan.Zero;
            double exposureTime = 0;
            double exposureDelay = 0;
            WormPeriods = 1;

            List<DateTime> lX = new List<DateTime>();
            List<float> lY = new List<float>();

            for (int i = 0; i < lines.Length; ++i)
            {
                var l = lines[i];

                if (l.StartsWith("# Image Scale "))
                    scale = pemproDouble(l);
                else
                if (l.StartsWith("# Rotation "))
                    rotation = pemproDouble(l);
                else
                if (l.StartsWith("# Declination "))
                    dec = pemproDouble(l);
                else
                if (l.StartsWith("# Gemini PEC Guide Rate"))
                    guideRate = pemproDouble(l);
                else
                if (l.StartsWith("# Gemini PEC Max "))
                    pecMax = pemproDouble(l);
                else
                if (l.StartsWith("# Gemini RA Spur Ratio"))
                    spur = pemproDouble(l);
                else
                if (l.StartsWith("# Gemini PEC Table Period"))
                    pecTablePeriod = pemproDouble(l);
                else
                if (l.StartsWith("# Gemini PEC Index"))
                {
                    pecIndex = pemproDouble(l);

                }
                else if (l.StartsWith("# Gemini PEC Phase"))
                {
                    var p = pemproDouble(l);
                    //phaseOffset = TimeSpan.FromSeconds(p ).Ticks;
                }
                else
                if (l.StartsWith("# Worm Phase"))
                {
                    var p = pemproDouble(l);
                    phaseOffset = TimeSpan.FromSeconds(p).Ticks;
                }
                else
                if (l.StartsWith("# Gemini RA Encoder Resolution"))
                    encoder = pemproDouble(l);
                else
                if (l.StartsWith("# Gemini PEC Worm Cycles "))
                {

                    cycles = (int)pemproDouble(l);

                }
                else
                if (l.StartsWith("# Gemini Minimum Step Size "))
                    stepSize = pemproDouble(l);
                else
                if (l.StartsWith("# Gemini PEC Index"))
                    startIndex = pemproDouble(l);
                else
                if (l.StartsWith("# Worm Period"))
                {
                    wormPeriod = pemproDouble(l);
                    cycleTime = TimeSpan.FromSeconds(wormPeriod).Ticks * WormPeriods;
                }
                else
                if (l.StartsWith("# Gemini Worm Teeth"))
                    wormTeeth = pemproDouble(l);
                else
                if (l.StartsWith("# Exposure Time"))
                    exposureTime = pemproDouble(l);
                else
                  if (l.StartsWith("# Exposure Delay Interval"))
                    exposureDelay = pemproDouble(l);
                else
                if (l.StartsWith("# Tracking star at "))
                {
                    var e = l.Split(new char[] { '(', ')', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    oX = double.Parse(e[1].Trim());
                    oY = double.Parse(e[2].Trim());
                }
                else
                if (l.StartsWith("##### START USER COMMENTS #####"))
                {
                    do
                    {
                        if (lines[++i].StartsWith("##### END USER COMMENTS #####"))
                            break;
                    } while (i < lines.Length);
                }
                else if (l.StartsWith("# "))
                {
                }
                else
                {
                    var d = l.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (d.Length >= 8)
                    {
                        var time = TimeSpan.Parse(d[1]);
                        var boxX = int.Parse(d[2]);
                        var boxY = int.Parse(d[3]);
                        var centX = double.Parse(d[4]);
                        var centY = double.Parse(d[5]);
                        var star = double.Parse(d[6]);
                        var pecIdx = double.Parse(d[7]);

                        if (lX.Count == 0)
                        {
                            initTimeSpan = TimeSpan.FromSeconds((pecIdx * pecTablePeriod / (double)PECMax - (exposureTime + exposureDelay)/2) % (wormPeriod * WormPeriods));
                            time = TimeSpan.Zero;
                        }


                        var dx = (boxX + centX) - oX;
                        var dy = (boxY + centY) - oY;

                        dx = Math.Cos(rotation / 180.0 * Math.PI) * dx - Math.Sin(rotation / 180.0 * Math.PI) * dy;

                        var t = TimeSpan.FromTicks((initTimeSpan.Ticks + time.Ticks));
                        Debug.WriteLine($"{time}  {t}");
                        lX.Add(x0 + t);
                        lY.Add((float)(dx * scale));

                        //                        Debug.WriteLine($"{t.TotalSeconds},{dx * scale}");
                    }

                }
            }

            X = lX.ToArray();
            Y = lY.ToArray();

            PECMax = (int)pecMax;
            PECOneWorm = (int)( spur * encoder);

            if (GeminiHardware.Instance.ServoFirmwareVersion(true) >= 2)
            {
                string raMultiplier = GeminiHardware.Instance.DoCommandResult("<401:", GeminiHardware.Instance.MAX_TIMEOUT, false);
                int m = 0;
                int.TryParse(raMultiplier, out m);
                if ((m & 1) != 0)
                {
                    PECOneWorm *= 4;
                    stepSize /= 4;
                    PECOneWorm *= 4;
                    //cycles /= 4;
                }
            }


            StepX = stepSize;
            WormPeriodSteps = PECOneWorm;
            WormPeriods = (int)cycles;

            return true;
        }

        //https://en.wikipedia.org/wiki/Earth%27s_rotation

        const double SiderealRate = 15.041068640261926; // 360 / 23 h 56 m 4.09053083288s 

        public double NumberOfCycles {
            get {
                if (X == null || X.Length < 5) return 0;
                return (X.Last() - X.First()).TotalSeconds / WormPeriodSeconds;
            }
        }

        public bool ProgramPEC(Func<double,double> func, int cycles = 1, double combineSteps = 1.0)
        {
            
            //ProgramPEC3(func); return true;

            double err = 0;
            double pos = 0;

            int cnt = 0;

            int[] pec = new int[PECMax];

            Random r = new Random();
            double scaleDn = guideDivisor[0] / guideDivisor[1];
            double scaleUp = guideDivisor[2] / guideDivisor[1];


            for (int i=0; i < PECMax; ++i)
            {
                double x = (i / (double)(PECMax - 1)) * Math.PI * 2.0;

                double y = func(x);// + (r.NextDouble() - r.NextDouble()) * arcsecsPerStep / 1.5;// - arcsecsPerStep * i / (double)PECMax;// i * StepX * SiderealRate; // in arcsecs, from sidereal rate position

                double corr = y - pos;

                double delta = 0;

                if (-corr >= arcsecsPerStep * combineSteps)
                {
                    pec[i] = -1; // (int)(corr / (0.5 * arcsecsPerStep));
                
                    delta = arcsecsPerStep * pec[i] / scaleUp;
//                    GeminiHardware.Instance.DoCommand($">511:{cnt};{mult}", false);
                }
                else if (corr >= arcsecsPerStep * combineSteps)
                {
                    pec[i] = 1;// (int)(corr / (0.5 * arcsecsPerStep));
                    delta = arcsecsPerStep * pec[i] / scaleDn;
                    //GeminiHardware.Instance.DoCommand($">511:{cnt};8;{mult}", false);
                }
                else
                {
                    pec[i] = 0;
                    delta = 0;// arcsecsPerStep * stepsPerSecond[1];
                    //GeminiHardware.Instance.DoCommand($">511:{cnt};0", false);
                }

                pos += delta ;
                err = y - pos;

                //Debug.WriteLine($"{y}, {pos}, {corr}, {err}");
            }

            int max_cnt = (int)(50 * combineSteps);

            int step = 0;
            for(int i=0; i < PECMax;)
            {
                int c = 0;
                int b = 0;
                int val = pec[i];
                c = 0;
                int start = i;
                //collect all 0 entries
                while( i < PECMax && Math.Sign(val) == Math.Sign(pec[i]) && c < max_cnt)
                {
                    c += pec[i]==0? 1 : Math.Abs(pec[i]);
                    b += pec[i];
                    ++i;
                }
                if (start + c > PECMax) c = PECMax - start;
                if (c > 0)
                    GeminiHardware.Instance.DoCommand($">511:{(val < 0? 1 : (val > 0? 8 : 0))};{start};{c}", false);
                else
                if (c == 1 && val != 0) // convert one step into two half-steps
                {
                    int newdiv = (int)guideDivisor[1];
                    if (val < 0)
                        newdiv = (int)((guideDivisor[0] + guideDivisor[1]) / 2);
                    else if (val > 0)
                        newdiv = (int)((guideDivisor[2] + guideDivisor[1]) / 2);

                    GeminiHardware.Instance.DoCommand($">511:{newdiv};{start};2", false);
                    System.Threading.Thread.Sleep(10);
                    i++;
                }
                //i += c;
                step++;
            }

            return true;
        }

        private void ProgramPEC2(Func<double, double> PE)
        {
         

            var Clock = 12e6;
            var TrackRate = SiderealRate;
            var StepSize = arcsecsPerStep;
            var Period = WormPeriodSteps;
            int Mode = 4;
            int NomDivisor = 0;
            var ArcsecTicks = Clock / TrackRate;
            var StepTime = StepX;
     
            int i, StepCount, Divisor, PrevDivisor, rep, diff, corrections = 0;
            double NextWormPos,     // Position where the worm will be after the next step  
             ActWormPos,        // current position: n * StepSize + PE(n)
             DeltaPE,       // PE difference step(n) - step(n-1)
             DivisorSum,        // the sum of all divisor so far
             Distance;      // next step's length = StepSize + DeltaPE

            ActWormPos = PE(0);     // Worm start position in arcsecs
            Debug.WriteLine($"#Starting at PE(0) position  {ActWormPos}");
            double StepDivisor = (Clock * Mode * StepSize / TrackRate);

            PrevDivisor = (int)(StepDivisor + (PE(0)) / StepSize);   // Start divisor
            rep = 1;                // count the previous divisor for run-length encoding
            DivisorSum = 0.0;
            StepCount = 0;
            double err = 0;

            Debug.WriteLine($"#Position Divisor Steps # Stepwidth for CurrPEC.pec lines\r\n");

            
            for (i = 0; i < Period - 1; i++)
            {
                DeltaPE = PE(2 * Math.PI * (i + 1) / (double)Period) - PE(2 * Math.PI * (i) / (double)Period);// Delta PE of upcoming step in arcsecs
                DeltaPE /= 2;

                if (DeltaPE > 0) DeltaPE *= 2;
                else DeltaPE /= 1.5;
                NextWormPos = ActWormPos + StepSize + DeltaPE + err;          // Where will the worm be after the next motor tick 
                Distance = NextWormPos - ActWormPos ;            // next step's pointing error in arcsecs;

                //if (Distance > 0) Distance /= 1.5;
                //else Distance *= 2;

                var steps = Distance / StepSize;

                double ddiv = Clock * Mode / (steps / StepX);

                // in time StepX need to execute 'steps' steps. divisor = 
                Divisor = (int)(ddiv + 0.5);

                err = (Clock / ddiv - Clock / Divisor) * StepSize; // remainder in arcsecs
                // freq / divsor = dist/step
                // divisor = freq / dist

                //Divisor = (int)(StepDivisor + Distance / StepSize + 0.5);   // distance longer than ideal: move it slower: bigger divisor and vice versa, rounded
                DivisorSum += Divisor;                  // A correct sum(Divisors) for one rev avoids drift
                Debug.WriteLine($"{i}, {Divisor} \t# {ActWormPos} {DeltaPE} {Distance}\t{DivisorSum / Clock} Err={err}arcsecs");

                diff = Math.Abs((Divisor - PrevDivisor)/Mode);

                //	if ((/*Mode >*/ 1) && (diff < 8))
                //	   diff = 0;
                if ((Math.Abs(diff) > 75)  || i == Period-1) // != 0)
                {
                    if (rep != 0)
                    {
                        GeminiHardware.Instance.DoCommand($">511:{Divisor};{i - rep};{rep}", false);

                        //Debug.WriteLine($"{i-rep-1}, {PrevDivisor}, {rep} \t# {Distance}\r\n");
                        StepCount += rep;
                        corrections++;
                        PrevDivisor = Divisor;
                        rep = 1;
                        ActWormPos = NextWormPos;
                    }
                    else
                    {
                        rep++;
                        ActWormPos += StepSize;
                    }

                }
                else
                {                  
                    rep++;
                    ActWormPos += StepSize;
                }

            }

            Debug.WriteLine($"TOTAL CORRECTIONS = {corrections}, total Steps = {StepCount}");
        }


        private void ProgramPEC3(Func<double, double> PE)
        {

            var Clock = 12e6;
            var TrackRate = SiderealRate;
            var StepSize = arcsecsPerStep;
            var Period = WormPeriodSteps;
            int Mode = 4;
            int NomDivisor = 0;
            var ArcsecTicks = Clock / TrackRate;
            var StepTime = StepX;

            int i, StepCount, Divisor, PrevDivisor, rep, diff, corrections = 0;
            double NextWormPos,     // Position where the worm will be after the next step  
             ActWormPos,        // current position: n * StepSize + PE(n)
             DeltaPE,       // PE difference step(n) - step(n-1)
             DivisorSum,        // the sum of all divisor so far
             Distance;      // next step's length = StepSize + DeltaPE

            ActWormPos = PE(0);     // Worm start position in arcsecs
            Debug.WriteLine($"#Starting at PE(0) position  {ActWormPos}");
            double StepDivisor = (Clock * Mode * StepSize / TrackRate);

            NomDivisor = PrevDivisor = (int)(StepDivisor + (PE(0)) / StepSize);   // Start divisor
            rep = 1;                // count the previous divisor for run-length encoding
            DivisorSum = 0.0;
            StepCount = 0;
            double err = 0;

            Debug.WriteLine($"#Position Divisor Steps # Stepwidth for CurrPEC.pec lines\r\n");

            double pecStep = PECOneWorm;
            double pecStepTime = WormPeriodSeconds / pecStep;

            double running_error = 0;
            int curr_divisor = NomDivisor;
            for (i = 1; i < Period - 1; i++)
            {
                err = PE(2 * Math.PI * (i+1)) - PE(2 * Math.PI * (i));// error of upcoming step in arcsecs

                var DesiredWormPos = Clock / NomDivisor * i;

                NextWormPos = DesiredWormPos + err + running_error;          // Where will the worm be after the next motor tick 


                Distance = NextWormPos - DesiredWormPos;            // next step's pointing error in arcsecs;

                //if (Distance > 0) Distance /= 1.5;
                //else Distance *= 2;

                var steps = Distance / StepSize;

                double ddiv = Clock * Mode / (steps / StepX);

                // in time StepX need to execute 'steps' steps. divisor = 
                Divisor = (int)(ddiv + 0.5);

                err = (Clock / ddiv - Clock / Divisor) * StepSize; // remainder in arcsecs
                // freq / divsor = dist/step
                // divisor = freq / dist

                //Divisor = (int)(StepDivisor + Distance / StepSize + 0.5);   // distance longer than ideal: move it slower: bigger divisor and vice versa, rounded
                DivisorSum += Divisor;                  // A correct sum(Divisors) for one rev avoids drift
                Debug.WriteLine($"{i}, {Divisor} \t# {ActWormPos}  {Distance}\t{DivisorSum / Clock} Err={err}arcsecs");

                diff = Math.Abs((Divisor - PrevDivisor) / Mode);

                //	if ((/*Mode >*/ 1) && (diff < 8))
                //	   diff = 0;
                if ((Math.Abs(diff) > 75) || i == Period - 1) // != 0)
                {
                    if (rep != 0)
                    {
                        GeminiHardware.Instance.DoCommand($">511:{Divisor};{i - rep};{rep}", false);

                        //Debug.WriteLine($"{i-rep-1}, {PrevDivisor}, {rep} \t# {Distance}\r\n");
                        StepCount += rep;
                        corrections++;
                        PrevDivisor = Divisor;
                        rep = 1;
                        ActWormPos = NextWormPos;
                    }
                    else
                    {
                        rep++;
                        ActWormPos += StepSize;
                    }

                }
                else
                {
                    rep++;
                    ActWormPos += StepSize;
                }

            }

            Debug.WriteLine($"TOTAL CORRECTIONS = {corrections}, total Steps = {StepCount}");
        }

        private double pemproDouble(string l)
        {
            double r = double.NaN;
            var i = l.LastIndexOf('#') + 1;
            double.TryParse(l.Substring(i), out r);
            return r;
        }

    }

    public class PECFilter
    {
        public bool Enabled = true;
        public bool UseIntegerOnly = true;
        public double MaxAmplitude = 0.3;
        public double MaxDivisor = 30;
        public int[] IncludeList = null;
        public int[] MountList = null;
        public string Name = "filter";

        static public string[] Mount_names = { "Custom", "GM-8", "G-11", "HGM-200", "MI-250", "Titan", "Titan50", "G11T", "GM811" };

        private int _mount = 5;
        public int MountType
        {
            get
            {
                return _mount;
            }
            set
            {
                if (value <= 0) MountList = null;

                _mount = value;
            }
        }

        public float[] Apply(PEClass pec, int deg = 0)
        {
            if (pec.fft == null) pec.UpdateDataFFT(deg);
            var compl = pec.fft;
            var fft_size = pec.fft_size;
            double winGain = 1;

            for (int i = 0; i < pec.fft_size; ++i)
            {
                var mag = compl[i].Magnitude / fft_size * 2 / winGain;
                var time = 1 / (i / (double)fft_size) * pec.StepX;


                if (mag > 0.3)
                {

                    double freq = pec.WormPeriods * pec.PECOneWorm * pec.StepX / time;


                }
            }
            MathNet.Numerics.IntegralTransforms.Fourier.Inverse(compl, MathNet.Numerics.IntegralTransforms.FourierOptions.NoScaling);

            var r = compl.Select(x => x.Magnitude);

            return r.ToArray();
        }

    }

}
