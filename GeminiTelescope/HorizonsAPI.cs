using MathNet.Numerics.Interpolation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ASCOM.GeminiTelescope
{
    class HorizonsAPI
    {
        public class Result
        {
            public string name { get; set; }
            public string pdes { get; set; }
            public string spkid { get; set; }
            public List<string> alias { get; set; }
        }

        public class Lookup
        {
            public Signature signature { get; set; }
            public int count { get; set; }
            public List<Result> result { get; set; }
        }

        public class Signature
        {
            public string source { get; set; }
            public string version { get; set; }
        }


        public class OneInterval
        {
            public DateTime time;
            public double ra, dec;
            public double ra_apparent, dec_apparent;
            public double dra, ddec;
            public double alt, az;
            public double elevation;
            public string constellation;
        }

        /// <summary>
        /// SCOUT Output format: https://ssd-api.jpl.nasa.gov/doc/scout.html
        /// </summary>
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

        public class Dec
        {
            public string max { get; set; }
            public string min { get; set; }
        }

        public class Eph
        {
            public Median median { get; set; }
            public string time { get; set; }                
         }

        public class Median
        {
            public string ra { get; set; }
         
            public string dec { get; set; }
            public string ddec { get; set; }
           
            public string dra { get; set; }
        }

        public class Ra
        {
            public string max { get; set; }
            public string min { get; set; }
        }

        public class ScoutResult
        {
            public List<Eph> eph { get; set; }
        }


        ///////////////////////////////////////

        /// <summary>
        /// SCOUT List of objects
        /// </summary>
        /// 
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class ScoutObject
        {          
            public string objectName { get; set; }
        }

        public class ScoutObjectList
        {
            public List<ScoutObject> data { get; set; }         
        
        }
        //////////////SCOUT List of objects//////////////

        public class Ephemeris
        {
            public string api;

            public string name, spkid;

            public DateTime start;
            public DateTime stop;

            public double maxRateChange;

            
            public DateTime prev_time;
            public OneInterval[] data;

            public string raw;

            public double prev_RA, prev_DEC, prev_dRA, prev_dDEC;

            internal bool Visible(DateTime utcNow)
            {
                double ra, dec, dra, ddec;
                return GetCurrent(utcNow, out ra, out dec, out dra, out ddec);  // ignore coordinates and rates, return if object is visible
            }

            internal DateTime StartVisible(DateTime now, double horizon = 0)
            {
                for(int i=0; i<data.Length; ++i)
                {
                    if (data[i].time < now) continue;

                    if (data[i].elevation > horizon) return i > 0? data[i - 1].time : data[i].time;
                }
                return DateTime.MaxValue;
            }

            internal DateTime EndVisible(DateTime now, double horizon = 0)
            {
                int i = 0;
                for (i = 0; i < data.Length; ++i)
                {
                    if (data[i].time > now)
                    {
                        for (; i < data.Length && data[i].elevation > horizon; ++i) ;
                        if (i < data.Length)
                            return data[i].time;
                        else
                            return data[i - 1].time;    // last element
                    }
                }
                return DateTime.MaxValue;
            }
            internal double MaxElevation(DateTime now, out DateTime max, out double currentEl)
            {
                DateTime endv = EndVisible(now);
                max = DateTime.Now;
                currentEl = 0;
                var mxe = data.Where(z => z.time >= now && z.time < endv);
                if (mxe.Count() == 0) return 0;

                currentEl = mxe.First().elevation;

                double mx = mxe.Max(x => x.elevation);
                var m = data.Where(x => x.elevation == mx && x.time >= now).OrderBy(y=>y.time);
                if (m.Count() > 0)
                {
                    max = m.FirstOrDefault().time;
                    return m.FirstOrDefault().elevation;
                }
                max = DateTime.Now;
                return 0;
            }
            internal bool GetCurrent(DateTime utcNow, out double ra, out double dec, out double dRA, out double dDec)
            {
                ra = dec = dRA = dDec = 0;
                //if (!Visible(utcNow)) return false; // outside the eph range

                OneInterval search = new OneInterval();
                search.time = utcNow;
                int step = Array.BinarySearch(data, search, new CompareIntervals());
                if (step < 0)
                    step = ~step; // next step later than now                    

                if (step >= data.Length || step <= 0) return false;

                double u = (utcNow.Ticks - data[step - 1].time.Ticks) / (double)(data[step].time.Ticks - data[step - 1].time.Ticks);

                prev_RA = ra   = data[step - 1].ra_apparent * (1 - u) + data[step].ra_apparent * u;
                prev_DEC= dec  = data[step - 1].dec_apparent * (1 - u) + data[step].dec_apparent * u;
                prev_dRA =dRA  = data[step - 1].dra * (1 - u) + data[step].dra * (u);
                prev_dDEC = dDec = data[step - 1].ddec;// * (1 - u) + data[step].ddec * u;
                //Debug.WriteLine($"XYZ, {step}, {u}, {dRA}, {dDec}, {data[step-1].ddec}");
                prev_time = utcNow;

                if (data[step].elevation <= 1) return false;   // too low in elevation!
                return true;

            }
        }

        public class CompareIntervals : IComparer<OneInterval>
        {

            public int Compare(OneInterval x, OneInterval y)
            {
                if (x.time > y.time) return 1;
                if (x.time < y.time) return -1;
                return 0;
            }

        }
        public class Parse
        {
            public static Ephemeris Extract(string e)
            {
                var eph = new Ephemeris();

                if (!e.StartsWith("API VERSION"))
                    throw new Exception("Invalid ephemeris document");

                Regex r = new Regex(@"API VERSION:\b*(.*)$", RegexOptions.Multiline);
                var res = r.Match(e);
                if (res.Groups.Count > 1)
                    eph.api = res.Groups[1].Captures[0].ToString().Trim();



                r = new Regex(@"Target body name\: (.*?)\{");
                res = r.Match(e);
                if (res.Groups.Count > 1)
                    eph.name = res.Groups[1].Captures[0].ToString().Trim();


                var steps = e.Split(new string[] { "$$SOE", "$$EOE" }, StringSplitOptions.RemoveEmptyEntries);
                if (steps.Length != 3)
                {
                    throw new Exception("No ephemeris data found!");
                }
                if (e.Contains("No ephemeris meets criteria"))
                {
                    throw new Exception("Object does not rise above the horizon during specified period");
                }

                var lines = steps[1].Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

                List<OneInterval> inter = new List<OneInterval>();
                foreach (var l in lines)
                {
                    try
                    {
                        var d = l.Split(',');
                        var ii = new OneInterval();
                        ii.time = DateTime.Parse(d[0], GeminiHardware.Instance.m_GeminiCulture);
                        ii.ra = degToDouble(d[3]);
                        ii.dec = degToDouble(d[4]);
                        ii.ra_apparent = degToDouble(d[5]);
                        ii.dec_apparent = degToDouble(d[6]);
                        ii.dra = double.Parse(d[7], GeminiHardware.Instance.m_GeminiCulture) / 3600; //arcsecs per sec
                        ii.ddec = double.Parse(d[8], GeminiHardware.Instance.m_GeminiCulture) / 3600;
                        ii.az = double.Parse(d[9], GeminiHardware.Instance.m_GeminiCulture);
                        ii.elevation = ii.alt = double.Parse(d[10], GeminiHardware.Instance.m_GeminiCulture);
                       
                        ii.constellation = d[11].Trim();
                        inter.Add(ii);
                    }
                    catch (Exception ex)
                    {

                    }
                }

                eph.data = inter.ToArray();

                if (inter.Count > 0)
                {
                    eph.start = inter.First().time;
                    eph.stop = inter.Last().time;
                }

                double mx = 0;
                for (int i = 1; i < eph.data.Length; ++i)
                {
                    var m = Math.Abs(eph.data[i].dra - eph.data[i - 1].dra) / 60; //per second (intervals are 60 seconds)
                    mx = Math.Max(mx, m);
                    m = Math.Abs(eph.data[i].ddec - eph.data[i - 1].ddec) / 60;
                    m = Math.Max(mx, m);
                }

                eph.maxRateChange = mx; // max rate of rate change per second (ra and dec)

                eph.raw = e;

                return eph;
            }


            public static List<string> ExtractScoutAllObjects(string e)
            {
                ScoutObjectList list = JsonConvert.DeserializeObject<ScoutObjectList>(e);
                List<string> l = new List<string>();
                foreach(var obj in list.data)
                {
                    l.Add($"[SC] {obj.objectName}");
                }
                return l;
            }
            public static Ephemeris ExtractScout(string e)
            {
                var eph = new Ephemeris();


                ScoutResult scout = JsonConvert.DeserializeObject<ScoutResult>(e);

                List<OneInterval> inter = new List<OneInterval>();
                foreach (var l in scout.eph)
                {
                    try
                    {

                        var ii = new OneInterval();
                        ii.time = DateTime.Parse(l.time, GeminiHardware.Instance.m_GeminiCulture);
                        ii.ra = double.Parse(l.median.ra, GeminiHardware.Instance.m_GeminiCulture);
                        ii.dec = double.Parse(l.median.dec, GeminiHardware.Instance.m_GeminiCulture);
                        ii.ra_apparent = ii.ra;
                        ii.dec_apparent = ii.dec;
                        // scout dRA/dDec rates are in arcsec/minute
                        ii.dra = double.Parse(l.median.dra, GeminiHardware.Instance.m_GeminiCulture) / 60; //arcsecs per sec
                        ii.ddec = double.Parse(l.median.ddec, GeminiHardware.Instance.m_GeminiCulture) / 60;
                        ii.az = 0;
                        ii.elevation = ii.alt = 90;// double.Parse(d[10], GeminiHardware.Instance.m_GeminiCulture);

                        ii.constellation = "";// d[11].Trim();
                        inter.Add(ii);
                    }
                    catch (Exception ex)
                    {

                    }
                }

                eph.data = inter.ToArray();

                if (inter.Count > 0)
                {
                    eph.start = inter.First().time;
                    eph.stop = inter.Last().time;
                }

                double mx = 0;
                for (int i = 1; i < eph.data.Length; ++i)
                {
                    var m = Math.Abs(eph.data[i].dra - eph.data[i - 1].dra) / 3600; //per second (intervals are 1 hour)
                    mx = Math.Max(mx, m);
                    m = Math.Abs(eph.data[i].ddec - eph.data[i - 1].ddec) / 3600;
                    m = Math.Max(mx, m);
                }

                eph.maxRateChange = mx; // max rate of rate change per second (ra and dec)

                eph.raw = e;

                return eph;
            }



            private static double degToDouble(string v)
            {
                //+13 06 00.98181
                var d = v.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if (d.Length != 3) return 0;
                return Math.Sign(double.Parse(d[0], GeminiHardware.Instance.m_GeminiCulture))*(Math.Abs(double.Parse(d[0], GeminiHardware.Instance.m_GeminiCulture)) + double.Parse(d[1], GeminiHardware.Instance.m_GeminiCulture) / 60 + double.Parse(d[2], GeminiHardware.Instance.m_GeminiCulture) / 3600);
            }



            public static List<List<string>> ExtractMultiple(string jpl)
            {
                var result = new List<List<string>>();
                try
                {
                    Regex r = new Regex(@".*Matching small-bodies:.*----(.*)\*\*\*\*\*", RegexOptions.Singleline);
                    var res = r.Match(jpl);
                    if (res.Groups.Count > 1)
                    {
                        var t = res.Groups[1].Captures[0].ToString().Trim(new char[] { ' ', '\t', '\r', '\n' });
                        var tlines = t.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        tlines.Remove(tlines.Last());
                        tlines.Remove(tlines.Last());
                        foreach (var line in tlines)
                        {
                            var l = line.Trim();
                            var c = line.Split(new string[] { "  " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            result.Add(c);
                        }
                    }
                }
                catch { }

                return result.OrderByDescending(x => x.ElementAt(1)).ToList();  // decreasing date order
            }


        }





    }
}
