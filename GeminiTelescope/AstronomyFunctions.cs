//tabs=4
// --------------------------------------------------------------------------------
//
// Astronomy Functions
//
// Description:	Astronomy functions class that wraps up the NOVAS fucntions in a
//              quick way to call them. 
//
// Author:		(rbt) Robert Turner <robert@robertturnerastro.com>
//
// Edit Log:
//
// Date			Who	Vers	Description
// -----------	---	-----	-------------------------------------------------------
// 08-JUL-2009	rbt	1.0.0	Initial edit
// --------------------------------------------------------------------------------
//

using System;
using System.Collections.Generic;
using System.Text;

namespace ASCOM.GeminiTelescope
{
    
    public class AstronomyFunctions
    {
        static AstronomyFunctions()
        { }

        //----------------------------------------------------------------------------------------
        // UTC DateTime to UTC Julian date
        //----------------------------------------------------------------------------------------
        public static double DateUtcToJulian(DateTime dt)
        {
            double tNow = (double)dt.Ticks - 6.30822816E+17;	// .NET ticks at 01-Jan-2000T00:00:00
            double j = 2451544.5 + (tNow / 8.64E+11);		// Tick difference to days difference
            return j;
        }

        //----------------------------------------------------------------------------------------
        // UTC Julian date to UTC DateTime
        //----------------------------------------------------------------------------------------
        public static DateTime JulianToDateUtc(double j)
        {
            long tix = (long)(6.30822816E+17 + (8.64E+11 * (j - 2451544.5)));
            DateTime dt = new DateTime(tix);
            return dt;
        }

        public static void ToJ2000(ref double ra, ref double dec)
        {
            //GeminiHardware.Instance.m_Transform.SiteElevation = GeminiHardware.Instance.Elevation;
            //GeminiHardware.Instance.m_Transform.SiteLatitude = GeminiHardware.Instance.Latitude;
            //GeminiHardware.Instance.m_Transform.SiteLongitude = GeminiHardware.Instance.Longitude;

            //GeminiHardware.Instance.m_Transform.JulianDateUTC = DateUtcToJulian(DateTime.Now);
            //GeminiHardware.Instance.m_Transform.SiteTemperature = 20;
            //GeminiHardware.Instance.m_Transform.SetTopocentric(ra, dec);
            //GeminiHardware.Instance.m_Transform.Refresh();
            //ra = GeminiHardware.Instance.m_Transform.RAJ2000;
            //dec = GeminiHardware.Instance.m_Transform.DecJ2000;


            // Rigorous equatorial precession computation from JNow to J2000
            // with much better precision around DEC=90 than ASCOM.Transform class
            double tNow = (DateUtcToJulian(DateTime.Now) - 2451545.0) / 36525;
            EqPrecession(tNow, 0, ref ra, ref dec); 
        }


        //----------------------------------------------------------------------------------------
        // Calculate Precession
        //----------------------------------------------------------------------------------------
        public static double Precess(DateTime datetime)
        {
            int y = datetime.Year + 1900;
            if (y >= 3900) { y = y - 1900; }
            int p = y - 1;
            int r = p / 1000;
            int s = 2 - r + r / 4;
            int t = (int)Math.Truncate(365.25 * p);
            double r1 = (s + t - 693597.5) / 36525;
            double s1 = 6.646 + 2400.051 * r1;

            return 24 - s1 + (24 * (y - 1900));

        }
        //----------------------------------------------------------------------------------------
        // Current Local Apparent Sidereal Time for Longitude
        //----------------------------------------------------------------------------------------

        public static double LocalSiderealTime(double longitude)
        {
            double days_since_j_2000 = DateUtcToJulian(DateTime.Now.ToUniversalTime()) - 2451545.0;
            double t = days_since_j_2000 / 36525;
            double l1mst = 280.46061837 + 360.98564736629 * days_since_j_2000 + longitude;
            if (l1mst < 0)
            {
                while (l1mst < 0)
                {
                    l1mst = l1mst + 360;
                }
            }
            else
            {
                while (l1mst > 360)
                {
                    l1mst = l1mst - 360;
                }
            }
            //calculate OM
            double om1 = 125.04452 - 1934.136261 * t;
            if (om1 < 0)
            {
                while (om1 < 0)
                {
                    om1 = om1 + 360;
                }
            }
            else
            {
                while (om1 > 360)
                {
                    om1 = om1 - 360;
                }
            }
            //calculat L
            double La = 280.4665 + 36000.7698 * t;
            if (La < 0)
            {
                while (La < 0)
                {
                    La = La + 360;
                }
            }
            else
            {
                while (La > 360)
                {
                    La = La - 360;
                }
            }
            //calculate L1
            double L11 = 218.3165 + 481267.8813 * t;
            if (L11 < 0)
            {
                while (L11 < 0)
                {
                    L11 = L11 + 360;
                }
            }
            else
            {
                while (L11 > 360)
                {
                    L11 = L11 - 360;
                }
            }
            //calculate e
            double ea1 = 23.439 - 0.0000004 * t;
            if (ea1 < 0)
            {
                while (ea1 < 0)
                {
                    ea1 = ea1 + 360;
                }
            }
            else
            {
                while (ea1 > 360)
                {
                    ea1 = ea1 - 360;
                }
            }


            double dp1 = (-172.2 * (Math.Sin(om1))) - (1.32 * (Math.Sin(2 * La))) + (0.21 * Math.Sin(2 * om1));
            double de1 = (9.2 * (Math.Cos(om1))) + (0.57 * (Math.Cos(2 * La))) + (0.1 * (Math.Cos(2 * La))) - (0.09 * (Math.Cos(2 * om1)));
            double eps1 = ea1 + de1;
            double correction1 = (dp1 * Math.Cos(ea1)) / 3600;
            //l1mst = l1mst + correction1;

            return l1mst;

        }


        //----------------------------------------------------------------------------------------
        // Current Local Apparent Sidereal Time for Longitude
        //----------------------------------------------------------------------------------------

        public static double LocalSiderealTime(double longitude, DateTime dt)
        {
            double days_since_j_2000 = DateUtcToJulian(dt.ToUniversalTime()) - 2451545.0;
            double t = days_since_j_2000 / 36525;
            double l1mst = 280.46061837 + 360.98564736629 * days_since_j_2000 + longitude;
            if (l1mst < 0)
            {
                while (l1mst < 0)
                {
                    l1mst = l1mst + 360;
                }
            }
            else
            {
                while (l1mst > 360)
                {
                    l1mst = l1mst - 360;
                }
            }
            //calculate OM
            double om1 = 125.04452 - 1934.136261 * t;
            if (om1 < 0)
            {
                while (om1 < 0)
                {
                    om1 = om1 + 360;
                }
            }
            else
            {
                while (om1 > 360)
                {
                    om1 = om1 - 360;
                }
            }
            //calculat L
            double La = 280.4665 + 36000.7698 * t;
            if (La < 0)
            {
                while (La < 0)
                {
                    La = La + 360;
                }
            }
            else
            {
                while (La > 360)
                {
                    La = La - 360;
                }
            }
            //calculate L1
            double L11 = 218.3165 + 481267.8813 * t;
            if (L11 < 0)
            {
                while (L11 < 0)
                {
                    L11 = L11 + 360;
                }
            }
            else
            {
                while (L11 > 360)
                {
                    L11 = L11 - 360;
                }
            }
            //calculate e
            double ea1 = 23.439 - 0.0000004 * t;
            if (ea1 < 0)
            {
                while (ea1 < 0)
                {
                    ea1 = ea1 + 360;
                }
            }
            else
            {
                while (ea1 > 360)
                {
                    ea1 = ea1 - 360;
                }
            }


            double dp1 = (-172.2 * (Math.Sin(om1))) - (1.32 * (Math.Sin(2 * La))) + (0.21 * Math.Sin(2 * om1));
            double de1 = (9.2 * (Math.Cos(om1))) + (0.57 * (Math.Cos(2 * La))) + (0.1 * (Math.Cos(2 * La))) - (0.09 * (Math.Cos(2 * om1)));
            double eps1 = ea1 + de1;
            double correction1 = (dp1 * Math.Cos(ea1)) / 3600;
            //l1mst = l1mst + correction1;

            return l1mst;

        }



        //----------------------------------------------------------------------------------------
        // Convert Double Angle to Hour Minute Second Display 
        //----------------------------------------------------------------------------------------
        public static string ConvertDoubleToHMS(double d)
        {
            double totalseconds = d / 15 * 3600;
            int hours = (int)Math.Truncate(totalseconds / 3600);
            int minutes = (int)Math.Truncate((totalseconds - hours * 3600) / 60);
            int seconds = (int)Math.Truncate(totalseconds - (hours * 3600) - (minutes * 60));
            return hours.ToString().PadLeft(2, '0') + ":" + minutes.ToString().PadLeft(2, '0') + ":" + seconds.ToString().PadLeft(2, '0');
        }
        //----------------------------------------------------------------------------------------
        // Convert Double Angle to Degrees Minute Second Display 
        //----------------------------------------------------------------------------------------
        public static string ConvertDoubleToDMS(double d)
        {

            int degrees = (int)Math.Truncate(d);

            int minutes = (int)Math.Truncate((d - (double)degrees) * 60);
            int seconds = (int)Math.Round((d - (double)degrees - (double)minutes / 60) * 3600, 0);

            if (seconds == 60)
            {
                minutes += 1;
                seconds = 0;
            }



            string output = degrees.ToString() + ":" + minutes.ToString().PadLeft(2, '0') + ":" + seconds.ToString().PadLeft(2, '0');
            if (d >= 0)
            {
                output = "+" + output;
            }
            return output;
        }
        //----------------------------------------------------------------------------------------
        // Calculate RA and Dec From Altitude and Azimuth and Site
        //----------------------------------------------------------------------------------------
        public static double CalculateRa(double Altitude, double Azimuth, double Latitude, double Longitude)
        {

            //double hourAngle = Math.Acos((Math.Cos(Altitude) - Math.Sin(Declination) * Math.Sin(Latitude)) / Math.Cos(Declination) * Math.Cos(Latitude))*SharedResources.RAD_DEG;
            //double hourAngle = Math.Acos((Math.Cos(Altitude) * Math.Cos(Latitude) - Math.Sin(Latitude) * Math.Sin(Altitude) * Math.Cos(Azimuth)) / Math.Cos(Declination)) * SharedResources.RAD_DEG;
            //double hourAngle = Math.Atan(-Math.Sin(Azimuth) * Math.Cos(Altitude) - Math.Cos(Azimuth) * Math.Sin(Latitude) * Math.Cos(Altitude) + Math.Sin(Altitude) * Math.Cos(Latitude)) * SharedResources.RAD_DEG;
            double hourAngle = Math.Atan2(-Math.Sin(Azimuth) * Math.Cos(Altitude), -Math.Cos(Azimuth) * Math.Sin(Latitude) * Math.Cos(Altitude) + Math.Sin(Altitude) * Math.Cos(Latitude)) * SharedResources.RAD_DEG;
            if (hourAngle < 0)
            { hourAngle += 360; }
            else if (hourAngle >= 360)
            { hourAngle -= 360; }
            double lst = LocalSiderealTime(Longitude * SharedResources.RAD_DEG);
            double ra = lst - hourAngle;

            return ra;
        }
        public static double CalculateDec(double Altitude, double Azimuth, double Latitude)
        {

            //return Math.Asin(Math.Cos(Azimuth)*Math.Cos(Latitude)*Math.Cos(Altitude) + Math.Sin(Latitude)*Math.Sin(Altitude));
            return Math.Asin(Math.Cos(Azimuth) * Math.Cos(Latitude) * Math.Cos(Altitude) + Math.Sin(Latitude) * Math.Sin(Altitude)) * SharedResources.RAD_DEG;
        }
        //----------------------------------------------------------------------------------------
        // Calculate Altitude and Azimuth From Ra/Dec and Site
        //----------------------------------------------------------------------------------------
        public static double CalculateAltitude(double RightAscension, double Declination, double Latitude, double Longitude)
        {
            double lst = LocalSiderealTime(Longitude * SharedResources.RAD_DEG);
            double ha = lst * SharedResources.DEG_RAD - RightAscension;
            return Math.Asin(Math.Sin(Declination) * Math.Sin(Latitude) + Math.Cos(Declination) * Math.Cos(ha) * Math.Cos(Latitude)) * SharedResources.RAD_DEG;

        }


        //----------------------------------------------------------------------------------------
        // Calculate Altitude and Azimuth From Ra/Dec and Site
        //----------------------------------------------------------------------------------------
        public static double CalculateAltitude(double RightAscension, double Declination, double Latitude, double Longitude, double lst)
        {
            double ha = lst  - RightAscension;
            return Math.Asin(Math.Sin(Declination) * Math.Sin(Latitude) + Math.Cos(Declination) * Math.Cos(ha) * Math.Cos(Latitude)) * SharedResources.RAD_DEG;

        }

        public static double CalculateAzimuth(double RightAscension, double Declination, double Latitude, double Longitude)
        {
            double lst = LocalSiderealTime(Longitude * SharedResources.RAD_DEG);
            double ha = lst * SharedResources.DEG_RAD - RightAscension;

            double A1 = -Math.Cos(Declination) * Math.Sin(ha) / Math.Cos(Math.Asin(Math.Sin(Declination) * Math.Sin(Latitude) + Math.Cos(Declination) * Math.Cos(ha) * Math.Cos(Latitude)));
            double A2 = (Math.Sin(Declination) * Math.Cos(Latitude) - Math.Cos(Declination) * Math.Cos(ha) * Math.Sin(Latitude)) / Math.Cos(Math.Asin(Math.Sin(Declination) * Math.Sin(Latitude) + Math.Cos(Declination) * Math.Cos(ha) * Math.Cos(Latitude)));

            double azimuth = Math.Atan2(A1, A2);
            if (azimuth < 0)
            {
                azimuth = 2 * Math.PI + azimuth;
            }

            return azimuth * SharedResources.RAD_DEG;
        }

        //----------------------------------------------------------------------------------------
        // Range RA and DEC
        //----------------------------------------------------------------------------------------
        public static double RangeHa(double RightAscension)
        {
            if (RightAscension < 0)
            {
                return 24 + RightAscension;
            }
            else if (RightAscension >= 24)
            {
                return RightAscension - 24;
            }
            else
            {
                return RightAscension;
            }
        }
        public static double RangeDec(double Declination)
        {
            if (Declination > 90)
            {
                return 90;
            }
            else if (Declination < -90)
            {
                return -90;
            }
            else
            {
                return Declination;
            }
        }




        /////////////////////////////////////////
        //
        // Adapted from Astronomy on the Personal Computer by Oliver Montenbruck and Thomas Pfleger, 1994
        //

        const double degs = 57.2957795130823;
        const double rads = 0.017453292519943295;

        //   Conversion to and from rectangular and polar coordinates.
        //   X,Y,Z form a left handed set of axes, and r is the radius vector
        //   of the point from the origin. Theta is the elevation angle of
        //   r with the XY plane, and phi is the angle anti-clockwise from the
        //   X axis and the projection of r in the X,Y plane.
        //
        //   in astronomical coordinate systems,
        //
        //   item    equatorial          ecliptic (helio or geo centric)
        //   z       celestial pole      ecliptic pole
        //   x,y     equatorial plane    ecliptic
        //   theta   declination         latitude
        //   phi     right ascension     longitude
        //

        static double rectangular(double R, double THETA, double phi, int idx)
        {
            //  takes spherical coordinates in degrees and returns the rectangular
            //  coordinate shown by index, 1 = x, 2 = y, 3 = z
            //
            //  x = r.cos(theta).cos(phi)
            //  y = r.cos(theta).sin(phi)
            //  z = r.sin(theta)
            //
            double r_cos_theta;
            r_cos_theta = R * Math.Cos(THETA * rads);

            if (idx == 1)
                return r_cos_theta * Math.Cos(phi * rads); //returns x coord
            if (idx == 2)
                return r_cos_theta * Math.Sin(phi * rads); //returns y coord

            return R * Math.Sin(THETA * rads);         //returns z coord 
       }

        static double rlength(double x, double y, double Z)
        {
            //   returns radius vector given the rectangular coords
            return Math.Sqrt(x * x + y * y + Z * Z);
        }


        static double spherical(double x, double y, double Z, int idx)
        {
            //
            //   Takes the rectangular coordinates and returns the shperical
            //   coordinate selected by index - 1 = r, 2 = theta, 3 = phi
            //
            //   r = sqrt(x*x + y*y + z*z)
            //   tan(phi) = y/x - use atan2 to get in correct quadrant
            //   tan(theta) = z/sqrt(x*x + y*y) - likewise
            //


            double rho;
            rho = x * x + y * y ;


            if (idx == 1)
                return Math.Sqrt(rho + Z * Z);    //returns r
            if (idx == 2) {
                rho = Math.Sqrt(rho);
                return degs * Math.Atan(Z / rho);    //returns theta
            }

            rho = Math.Sqrt(rho);

            var r = degs * Math.Atan2(y, x);      //returns phi
            if (r < 0) r += 360;
            return r;
        }


        //
        //  EqPrecession
        //  Equinox conversion for the precession (Rigorous method) as given in Astronomy on
        //  the personal computer by Montenbruck, Pfleger.
        //
        //  yearold is the epoch to precess from, yearnew is the epoch to precess
        //  to
        //
        //  ra and dec must BOTH be in decimal degrees.
        //
        //  entered by Han Kleijn, converted to C# by PK

        static void EqPrecession(double yearold, double yearnew, ref double raold, ref double decold)
        {
            double x, y, Z;
            double A11, A12, A13;
            double A21, A22, A23;
            double A31, A32, A33;
            double U, V, W, Z2;

            const double SEC = 3600;
            double DT, t1, t2, ZETA, THETA;
            double C1, S1, C2, S2, C3, S3;

            raold = raold * 360.0 / 24.0;   //convert to degrees

            x = rectangular(1, decold, raold, 1);
            y = rectangular(1, decold, raold, 2);
            Z = rectangular(1, decold, raold, 3);

            t1 = yearold;
            t2 = yearnew;
            DT = t2 - t1;
            ZETA = ((2306.2181 + (1.39656 - 0.000139 * t1) * t1) + ((0.30188 - 0.000345 * t1) + 0.017998 * DT) * DT) * DT / SEC;
            Z2 = ZETA + ((0.7928 + 0.000411 * t1) + 0.000205 * DT) * DT * DT / SEC;

            THETA = ((2004.3109 - (0.8533 + 0.000217 * t1) * t1) - ((0.42665 + 0.000217 * t1) + 0.041833 * DT) * DT) * DT / SEC;
            C1 = Math.Cos(rads * Z2);
            C2 = Math.Cos(rads * THETA);
            C3 = Math.Cos(rads * ZETA);


            S1 = Math.Sin(rads * Z2);
            S2 = Math.Sin(rads * THETA);
            S3 = Math.Sin(rads * ZETA);


            A11 = -S1 * S3 + C1 * C2 * C3;
            A12 = -S1 * C3 - C1 * C2 * S3;
            A13 = -C1 * S2;
            A21 = C1 * S3 + S1 * C2 * C3;
            A22 = C1 * C3 - S1 * C2 * S3;
            A23 = -S1 * S2;
            A31 = S2 * C3;
            A32 = -S2 * S3;
            A33 = C2;


            U = A11 * x + A12 * y + A13 * Z;
            V = A21 * x + A22 * y + A23 * Z;
            W = A31 * x + A32 * y + A33 * Z;


            x = U;
            y = V;
            Z = W;

            // Polar
            raold = spherical(x, y, Z, 3) * 24 / 360.0;
            decold = spherical(x, y, Z, 2);
        }
        //////////////////////////////////////////




    }
}

