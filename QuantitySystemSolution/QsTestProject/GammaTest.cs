﻿//using Qs.Modules;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantitySystem.Quantities.BaseQuantities;
using QuantitySystem.Units;
using Qs.Types;
using Qs.Runtime.Operators;

namespace QsTestProject
{
    
    
    /// <summary>
    ///This is a test class for GammaTest and is intended
    ///to contain all GammaTest Unit Tests
    ///</summary>
    [TestClass()]
    public class GammaTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Factorial
        ///</summary>
        [TestMethod()]
        public void FactorialTest()
        {
            QsValue number = new QsScalar() { NumericalQuantity = Unit.ParseQuantity("5<kg>") };

            QsScalar actual;
            actual = (QsScalar)QsGamma.Factorial(number);

            Assert.AreEqual<AnyQuantity<double>>(Unit.ParseQuantity("120<kg^5>"), actual.NumericalQuantity);

        }
    }
}
