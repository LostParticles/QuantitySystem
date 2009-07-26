﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuantitySystem.Quantities.BaseQuantities;

namespace Qs.QsTypes
{
    /// <summary>
    /// Gives the factorial of real numbers.
    /// should be complex number also but I will not include it
    /// ----------
    /// the implementation now is integer numbers.
    /// </summary>
    public static class Gamma
    {
        
        public static AnyQuantity<double> Factorial(AnyQuantity<double> number)
        {
            int v = (int)number.Value;

            //if (v > 170) throw new ArgumentOutOfRangeException("Number", number, "Number is greater than 170");
            if (v < 0) throw new ArgumentOutOfRangeException("Number", number, "Number is less than 0");

            AnyQuantity<double> num = (AnyQuantity<double>)number.Clone();
            num.Value = Math.Floor(num.Value);

            

            double Total = num.Value == 0 ? 1 : num.Value;

            //I am calculating the value part first for fast calculation
            for (int i = v; i >1; i--)
            {
                num.Value--;
                Total = Total * num.Value;
            }

            //   Raise the power of the unit.
            //    I think 3.5<kg>! = 11.631728<kg^3> is wrong and of course not <kg^4>  but it is <kg^3.5>
            QuantitySystem.Units.Unit TotalUnit = num.Unit.RaiseUnitPower((float)number.Value);


            // if we have fraction correct the calculation with Gamma Factorial.
            if (number.Value > v)
            {
                Total = GammaFactorial(number.Value);
            }

            AnyQuantity<double> TotalQuantity = TotalUnit.GetThisUnitQuantity<double>(Total);

            return TotalQuantity;
        
        }

        public static double GammaFactorial(double number)
        {
            //I checked this http://www.rskey.org/gamma.htm 
            // then this ...  http://www.luschny.de/math/factorial/approx/SimpleCases.html



            //the used function  near to the windows calculator
            Func<double, double> StieltjesFactorial = delegate(double x)
            {
                var y = x + 1; var p = 1.0;
                while (y < 7) { p = p * y; y = y + 1; }
                var r = Math.Exp(y * Math.Log(y) - y + 1 / (12 * y + 2 / (5 * y + 53 / (42 * y))));
                if (x < 7) r = r / p;
                return r * Math.Sqrt(2 * Math.PI / y);
            };


            Func<double, double> StieltjesLnFactorial = delegate(double z)
            {
                var a0 = 1 / 12; var a1 = 1 / 30; var a2 = 53 / 210; var a3 = 195 / 371;
                var a4 = 22999 / 22737; var a5 = 29944523 / 19733142;
                var a6 = 109535241009 / 48264275462;
                var Z = z + 1;

                return (1 / 2) * Math.Log(2 * Math.PI) + (Z - 1 / 2) * Math.Log(Z) - Z +
                    a0 / (Z + a1 / (Z + a2 / (Z + a3 / (Z + a4 / (Z + a5 / (Z + a6 / Z))))));
            };

            //gives value of the normal factorial :S
            Func<double, double> StieltjesFactorialHP = delegate(double x)
            {
                var y = x; var p = 1.0;
                while (y < 8)  
                {
                    p = p*y;
                    y = y+1;
                };
                var r = Math.Exp(StieltjesLnFactorial(y));

                if (x < 8) return  (r * x) / (p * y);
                else
                    return r;
            };


            //gives value less the normal factorial :(
            Func<double, double> Nemes = delegate(double x)
            {
                double temp = x;
                temp += (1 / 12) * (1 / x);
                temp += (1 / 1440) * (1 / x * x * x);
                temp += (239 / 362880) * (1 / x * x * x * x * x);

                return Math.Pow(temp, x) * Math.Exp(-x) * Math.Sqrt(2) * Math.Sqrt(Math.PI * x);

            };

            
            return StieltjesFactorial(number);
            


        }

    }
}