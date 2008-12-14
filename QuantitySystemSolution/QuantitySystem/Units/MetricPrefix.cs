﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace QuantitySystem.Units
{

    public struct MetricPrefix
    {
        private readonly string _Prefix;

        public string Prefix
        {
            get { return _Prefix; }
        }

        private readonly string _Symbol;

        public string Symbol
        {
            get { return _Symbol; }
        } 


        private double PrefixExponent;


        public MetricPrefix(string prefix, string symbol, double exponent)
        {
            _Prefix = prefix;
            _Symbol = symbol;
            PrefixExponent = exponent;
        }

        /// <summary>
        /// Gets the factor to convert to the required prefix.
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns>Conversion factor</returns>
        public double GetFactorForConvertTo(MetricPrefix prefix)
        {
            return prefix.Factor / this.Factor;
        }

        #region SI Standard prefixes as static properties


        #region Positive
        public static MetricPrefix Yotta { get { return new MetricPrefix("yotta", "Y", 24); } }
        public static MetricPrefix Zetta { get { return new MetricPrefix("zetta", "Z", 21); } }
        public static MetricPrefix Exa { get { return new MetricPrefix("exa", "E", 18); } }
        public static MetricPrefix Peta { get { return new MetricPrefix("peta", "P", 15); } }
        public static MetricPrefix Tera { get { return new MetricPrefix("tera", "T", 12); } }
        public static MetricPrefix Giga { get { return new MetricPrefix("giga", "G", 9); } }
        public static MetricPrefix Mega { get { return new MetricPrefix("mega", "M", 6); } }
        public static MetricPrefix Kilo { get { return new MetricPrefix("kilo", "k", 3); } }
        public static MetricPrefix Hecto { get { return new MetricPrefix("hecto", "h", 2); } }
        public static MetricPrefix Deka { get { return new MetricPrefix("deka", "da", 1); } }

        #endregion

        public static MetricPrefix None { get { return new MetricPrefix("", "", 0); } }

        #region Negative 
        public static MetricPrefix Deci { get { return new MetricPrefix("deci", "d", -1); } }
        public static MetricPrefix Centi { get { return new MetricPrefix("centi", "c", -2); } }
        public static MetricPrefix Milli { get { return new MetricPrefix("milli", "m", -3); } }
        public static MetricPrefix Micro { get { return new MetricPrefix("micro", "µ", -6); } }
        public static MetricPrefix Nano { get { return new MetricPrefix("nano", "n", -9); } }
        public static MetricPrefix Pico { get { return new MetricPrefix("pico", "p", -12); } }
        public static MetricPrefix Femto { get { return new MetricPrefix("femto", "f", -15); } }
        public static MetricPrefix Atto { get { return new MetricPrefix("atto", "a", -18); } }
        public static MetricPrefix Zepto { get { return new MetricPrefix("zepto", "z", -21); } }
        public static MetricPrefix Yocto { get { return new MetricPrefix("yocto", "y", -24); } }
        #endregion
        
        #endregion

        #region static constructor

        public static MetricPrefix FromExponent(double exponent)
        {
            int exp = (int)exponent; 
            switch (exp)
            {
                case -24:
                    return Yocto;
                case -21:
                    return Zepto;
                case -18:
                    return Atto;
                case -15:
                    return Femto;
                case -12:
                    return Pico;
                case -9:
                    return Nano;
                case -6:
                    return Micro;
                case -3:
                    return Milli;
                case -2:
                    return Centi;
                case -1:
                    return Deci;
                case 0:
                    return None;
                case 1:
                    return Deka;
                case 2:
                    return Hecto;
                case 3:
                    return Kilo;
                case 6:
                    return Mega;
                case 9:
                    return Giga;
                case 12:
                    return Tera;
                case 15:
                    return Peta;
                case 18:
                    return Exa;
                case 21:
                    return Zetta;
                case 24:
                    return Yotta;
                default:
                    throw new MetricPrefixException("No SI Prefix found for exponent = " + exponent.ToString(CultureInfo.InvariantCulture));
            }
        }

        public static MetricPrefix FromPrefixName(string prefixName)
        {
            switch (prefixName.ToLower(CultureInfo.InvariantCulture))
            {

                case "yocto":
                    return Yocto;
                case "zepto":
                    return Zepto;
                case "atto":
                    return Atto;
                case "femto":
                    return Femto;
                case "pico":
                    return Pico;
                case "nano":
                    return Nano;
                case "micro":
                    return Micro;
                case "milli":
                    return Milli;
                case "centi":
                    return Centi;
                case "deci":
                    return Deci;
                case "none":
                    return None;
                case "deka":
                    return Deka;
                case "hecto":
                    return Hecto;
                case "kilo":
                    return Kilo;
                case "mega":
                    return Mega;
                case "giga":
                    return Giga;
                case "tera":
                    return Tera;
                case "peta":
                    return Peta;
                case "exa":
                    return Exa;
                case "zetta":
                    return Zetta;
                case "yotta":
                    return Yotta;
                default:
                    throw new MetricPrefixException("No SI Prefix found for prefix = " + prefixName);

            }
        }

        #endregion

        #region Properties
        public double Exponent
        {
            get
            {
                return this.PrefixExponent;
            }
        }

        public double Factor
        {
            get
            {
                return Math.Pow(10, PrefixExponent);
            }
        }
        #endregion



        #region Operations
        public MetricPrefix Invert()
        {
            return MetricPrefix.FromExponent(0 - this.PrefixExponent);
        }

        public static MetricPrefix Add(MetricPrefix firstPrefix, MetricPrefix secondPrefix)
        {
            double exp = firstPrefix.Exponent + secondPrefix.Exponent;
            
            MetricPrefix prefix = MetricPrefix.FromExponent(exp);
            return prefix;
        }

        public static MetricPrefix Subtract(MetricPrefix firstPrefix, MetricPrefix secondPrefix)
        {
            double exp = firstPrefix.Exponent - secondPrefix.Exponent;

            MetricPrefix prefix = MetricPrefix.FromExponent(exp);
            return prefix;
        }

        public static MetricPrefix operator +(MetricPrefix firstPrefix, MetricPrefix secondPrefix)
        {
            return Add(firstPrefix, secondPrefix);
        }

        public static MetricPrefix operator -(MetricPrefix firstPrefix, MetricPrefix secondPrefix)
        {
            return Subtract(firstPrefix, secondPrefix);
        }


        #endregion

    }

}