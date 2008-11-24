﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

using QuantitySystem.Quantities.BaseQuantities;
using QuantitySystem.Quantities;
using System.Reflection;

namespace QuantitySystem.Units
{
    public partial class Unit
    {
        #region Dynamically created unit

        private List<Unit> SubUnits { get; set; } //the list shouldn't been modified by sub classes

        /// <summary>
        /// Construct a unit based on the quantity type in SI units.
        /// </summary>
        /// <param name="quantityType"></param>
        public Unit(Type quantityType)
        {

            QuantityDimension dimension = QuantityDimension.DimensionFrom(quantityType);

            SubUnits = new List<Unit>();

            if (dimension.Mass.Exponent != 0)
            {
                Unit u = new SI.Gram();
                u.UnitExponent = dimension.Mass.Exponent;
                SubUnits.Add(u);
            }

            if (dimension.Length.Exponent != 0)
            {
                Unit u = new SI.Metre();
                u.UnitExponent = dimension.Length.Exponent;
                SubUnits.Add(u);
            }

            if (dimension.Time.Exponent != 0)
            {
                Unit u = new SI.Second();
                u.UnitExponent = dimension.Time.Exponent;
                SubUnits.Add(u);
            }

            if (dimension.Temperature.Exponent != 0)
            {
                Unit u = new SI.Kelvin();
                u.UnitExponent = dimension.Temperature.Exponent;
                SubUnits.Add(u);
            }

            if (dimension.LuminousIntensity.Exponent != 0)
            {
                Unit u = new SI.Candela();
                u.UnitExponent = dimension.LuminousIntensity.Exponent;
                SubUnits.Add(u);
            }

            if (dimension.AmountOfSubstance.Exponent != 0)
            {
                Unit u = new SI.Mole();
                u.UnitExponent = dimension.AmountOfSubstance.Exponent;
                SubUnits.Add(u);
            }

            if (dimension.ElectricCurrent.Exponent != 0)
            {
                Unit u = new SI.Ampere();
                u.UnitExponent = dimension.ElectricCurrent.Exponent;
                SubUnits.Add(u);
            }


            this.symbol = GenerateUnitSymbolFromSubBaseUnits();


            this.isDefaultUnit = false;
            this.quantityType = quantityType;



            this.isBaseUnit = false;



        }


        /// <summary>
        /// Construct a unit based on the default units of the internal quantities of passed quantity instance.
        /// </summary>
        /// <param name="quantity"></param>
        public Unit(BaseQuantity quantity)
        {
            SubUnits = new List<Unit>();

            Type m_QuantityType = quantity.GetType();


            //try direct mapping first to get the unit

            Type InnerUnitType = Unit.GetSIUnitTypeOf(m_QuantityType);



            if (InnerUnitType == null) //no direct mapping so get it from the inner quantities
            {
                //I can't cast BaseQuantity to AnyQuantity<object>  very annoying
                //so I used reflection.

                MethodInfo GIQ = m_QuantityType.GetMethod("GetInternalQuantities");

                //casted the array to BaseQuantity array also
                var InternalQuantities = GIQ.Invoke(quantity, null) as BaseQuantity[];

                foreach (var InnerQuantity in InternalQuantities)
                {
                    //try to get the quantity direct unit
                    Type l2_InnerUnitType = Unit.GetSIUnitTypeOf(InnerQuantity.GetType());

                    if (l2_InnerUnitType == null)
                    {
                        //this means for this quantity there is no direct mapping to SI Unit
                        // so we should create unit for this quantity

                        Unit un = new Unit(InnerQuantity);

                        SubUnits.Add(un);
                    }
                    else
                    {
                        //found :) create it with the exponent

                        Unit un = (Unit)Activator.CreateInstance(l2_InnerUnitType);
                        un.UnitExponent = InnerQuantity.Exponent;

                        SubUnits.Add(un);
                    }


                }

            }
            else
            {
                //subclass of AnyQuantity
                //use direct mapping with the exponent of the quantity

                Unit un = (Unit)Activator.CreateInstance(InnerUnitType);
                un.UnitExponent = quantity.Exponent;

                SubUnits.Add(un);

            }

            this.symbol = GenerateUnitSymbolFromSubBaseUnits();

            this.isDefaultUnit = false;

            if (!m_QuantityType.IsGenericTypeDefinition)
            {
                //the passed type is AnyQuantity<object> for example
                //I want to get the type without type parameters AnyQuantity<>

                this.quantityType = m_QuantityType.GetGenericTypeDefinition();

            }
            else
            {

                this.quantityType = m_QuantityType;
            }

            this.isBaseUnit = false;


        }



        #endregion

        #region Unit Symbol processing

        /// <summary>
        /// adjust the symbol string.
        /// </summary>
        /// <returns></returns>
        private string GenerateUnitSymbolFromSubBaseUnits()
        {
            string UnitNumerator="";
            string UnitDenominator="";

            Func<string, int, Object> ConcatenateUnit = delegate(string symbol, int exponent)
            {

                if (exponent > 0)
                {
                    if (UnitNumerator.Length > 0) UnitNumerator += ".";
                    UnitNumerator += symbol;
                    if (exponent > 1) UnitNumerator += "^" + exponent.ToString(CultureInfo.InvariantCulture);
                }

                if (exponent < 0)
                {
                    if (UnitDenominator.Length > 0) UnitDenominator += ".";
                    UnitDenominator += symbol;
                    if (exponent < -1) UnitDenominator += "^" + Math.Abs(exponent).ToString(CultureInfo.InvariantCulture);
                }

                return null;
            };

            foreach (Unit unit in SubUnits)
            {
                ConcatenateUnit(unit.Symbol, unit.UnitExponent);
            }

            //return <UnitNumerator / UnitDenominator>
            Func<string> FormatUnitSymbol = delegate()
            {
               string UnitSymbol = "<";

                if (UnitNumerator.Length > 0) UnitSymbol += UnitNumerator;
                else UnitSymbol += "1";

                if (UnitDenominator.Length > 0) UnitSymbol += "/" + UnitDenominator;

                UnitSymbol += ">";

                return UnitSymbol;
            };

            return FormatUnitSymbol();

        }


        #endregion



    }
}