using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numeric.Range;

namespace Numeric.Test
{
   [TestClass]
   public class CRangeTest
   {
      #region "mock class"

      /// <summary>
      /// Mock class exposing protected method of range base class.
      /// </summary>
      private class CMockRange<T> : CRange<T> where T : struct, IComparable
      {
         public CMockRange(T s, T e)
            : base(s, e)
         { }

         public bool _Contains(T value)
         {
            return base.Contains(value);
         }

         public bool _Contains(CRange<T> range)
         {
            return base.Contains(range);
         }

         public bool _IsSharingRange(CRange<T> range)
         {
            return base.IsSharingRange(range);
         }
      }

      #endregion

      #region "test: constructor"

      [TestMethod(), Description("Asserts that the default constructor returns the expected instance with its properties.")]
      public void TestRange_DefaultConstructor()
      {
         CRange<decimal> r = new CRange<decimal>(1m, 2m);
         
         Assert.AreEqual<decimal>(1m, r.Start);
         Assert.AreEqual<decimal>(2m, r.End);
      }

      [TestMethod(), Description("Asserts that a range can be built with the same value for its endpoints.")]
      public void TestRange_DefaultConstructor_SameEndPoints()
      {
         CRange<decimal> r = new CRange<decimal>(1m, 1m);

         Assert.AreEqual<decimal>(1m, r.Start);
         Assert.AreEqual<decimal>(1m, r.End);
      }

      [TestMethod(), Description("Verifies that an argument exception is thrown when start is greater than end.")]
      [ExpectedException(typeof(ArgumentException), "Start value cannot be greater than end value.")]
      public void TestRange_DefaultConstructor_ArgumentException()
      {
         CRange<decimal> r = new CRange<decimal>(2m, 1m);

         Assert.AreEqual<decimal>(1m, r.Start);
         Assert.AreEqual<decimal>(1m, r.End);
      }

      #endregion

      #region "test: methods"
      
      [TestMethod(), Description("Asserts that to string method returns the expected result.")]
      public void TestRange_ToString()
      {
         CRange<int> r = new CRange<int>(1, 2);

         Assert.AreEqual<string>("(1, 2)", r.ToString());
      }

      [TestMethod(), Description("Asserts that to string overload method returns the expected result.")]
      public void TestRange_ToString_Overload()
      {
         CRange<int> r = new CRange<int>(1, 2);

         Assert.AreEqual<string>("[2,1-", r.ToString("[b,a-"));
      }

      [TestMethod(), Description("Asserts that range can be cloned.")]
      public void TestRange_Clone_Instance()
      {
         CRange<DateTime> r = new CRange<DateTime>(DateTime.Now, DateTime.Now.AddMinutes(1));
         object clone = r.Clone();

         Assert.IsInstanceOfType(clone, typeof(CRange<DateTime>));
         Assert.IsFalse(object.ReferenceEquals(r, clone));
      }

      [TestMethod(), Description("Asserts that range can be cloned.")]
      public void TestRange_Clone_Properties()
      {
         CRange<DateTime> r = new CRange<DateTime>(DateTime.Now, DateTime.Now.AddMinutes(1));
         CRange<DateTime> clone = (CRange<DateTime>)r.Clone();

         Assert.AreEqual<DateTime>(r.Start, clone.Start);
         Assert.AreEqual<DateTime>(r.End, clone.End);
      }

      [TestMethod(), Description("Asserts that a clone is equal to the parent object.")]
      public void TestRange_Clone_Equality()
      {
         CRange<DateTime> r = new CRange<DateTime>(DateTime.Now, DateTime.Now.AddMinutes(1));
         CRange<DateTime> clone = (CRange<DateTime>)r.Clone();

         Assert.AreEqual<CRange<DateTime>>(r, clone);
      }

      [TestMethod(), Description("Asserts that equality can be applied to ranges.")]
      public void TestRange_Equality()
      {
         CRange<DateTime> r1 = new CRange<DateTime>(DateTime.Now, DateTime.Now.AddMinutes(1));
         CRange<decimal> r2 = new CRange<decimal>(-2m, -1m);
         CRange<int> r3 = new CRange<int>(-2, -1);
         CRange<decimal> r4 = new CRange<decimal>(-2m, -1m);
         
         Assert.IsTrue(r4.Equals(r2));
         Assert.IsFalse(r4.Equals(r1));
         Assert.IsFalse(r4.Equals(r3));
      }

      [TestMethod(), Description("Asserts that contains method returns the expected result for single values.")]
      public void TestRange_Contains_SingleValues()
      {
         CMockRange<int> r = new CMockRange<int>(-1, 1);

         Assert.IsTrue(r._Contains(-1));
         Assert.IsTrue(r._Contains(0));
         Assert.IsTrue(r._Contains(1));
         Assert.IsFalse(r._Contains(-2));
         Assert.IsFalse(r._Contains(2));
      }

      [TestMethod(), Description("Asserts that contains method returns the expected result for ranges.")]
      public void TestRange_Contains_Ranges()
      {
         CMockRange<int> r = new CMockRange<int>(-10, 10);

         Assert.IsTrue(r._Contains(new CRange<int>(1,5)));
         Assert.IsTrue(r._Contains((CRange<int>)r.Clone()));
         Assert.IsTrue(r._Contains(new CRange<int>(-5, 5)));
         Assert.IsFalse(r._Contains(new CRange<int>(-11, 5)));
         Assert.IsFalse(r._Contains(new CRange<int>(0, 11)));
      }

      [TestMethod(), Description("Asserts that is sharing range method returns the expected result for ranges.")]
      public void TestRange_IsSharingRange_Ranges()
      {
         CMockRange<int> r = new CMockRange<int>(-10, 10);

         //contains
         Assert.IsTrue(r._IsSharingRange(new CRange<int>(1, 5)));
         Assert.IsTrue(r._IsSharingRange((CRange<int>)r.Clone()));
         Assert.IsTrue(r._IsSharingRange(new CRange<int>(-5, 5)));
         //sharing
         Assert.IsTrue(r._IsSharingRange(new CRange<int>(-11, 5)));
         Assert.IsTrue(r._IsSharingRange(new CRange<int>(0, 11)));
         //outside
         Assert.IsFalse(r._IsSharingRange(new CRange<int>(-12, -11)));
         Assert.IsFalse(r._IsSharingRange(new CRange<int>(11, 200)));
      }

      #endregion
   }
}
