using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numeric.Range;
using System.Linq;

namespace Numeric.Test
{
   [TestClass]
   public class CRangeLineTest
   {

      #region "test: constructor"

      [TestMethod(), Description("Asserts that the default constructor returns the expected instance with its properties.")]
      public void TestRangeLine_DefaultConstructor()
      {
         CRangeLine<decimal> rl = new CRangeLine<decimal>();

         Assert.AreEqual<decimal>(default(decimal), rl.Start);
         Assert.AreEqual<decimal>(default(decimal), rl.End);         
         Assert.AreEqual<int>(0, rl.Count, "Number of segments.");
         Assert.IsFalse(rl.Segments.Any());
      }

      [TestMethod(), Description("Asserts that audit property can be enabled within a range line.")]
      public void TestRangeLine_DefaultConstructor_Audit()
      {
         CRangeLine<decimal> rl = new CRangeLine<decimal>();
         Assert.IsFalse(rl.AuditEnabled, "Audit is disabled by default.");

         rl.AuditEnabled = true;
         Assert.IsTrue(rl.AuditEnabled, "Audit is enabled.");
      }

      #endregion

      #region "test: add"

      [TestMethod(), Description("Asserts that it is possible to add single segment on a range line.")]
      public void TestRangeLine_AddSingle_Properties()
      {
         CRangeLine<decimal> rl = new CRangeLine<decimal>();
         rl.Add(1m, 2m);

         Assert.AreEqual<decimal>(1m, rl.Start);
         Assert.AreEqual<decimal>(2m, rl.End);
         Assert.AreEqual<int>(1, rl.Count, "Number of segments.");
         Assert.IsTrue(rl.Segments.Any());
      }

      [TestMethod(), Description("Asserts that it is possible to add single segment on a range line.")]
      public void TestRangeLine_AddSingle_Instance()
      {
         CRangeLine<DateTime> rl = new CRangeLine<DateTime>();
         CRange<DateTime> r = new CRange<DateTime>(DateTime.Now, DateTime.Now.AddDays(1));
         rl.Add(r);

         Assert.AreEqual<DateTime>(r.Start, rl.Start);
         Assert.AreEqual<DateTime>(r.End, rl.End);
         Assert.AreNotSame(r, rl.Segments.Single());
      }

      [TestMethod(), Description("Asserts that it is possible to add single segment on a range line.")]
      public void TestRangeLine_AddSingle_Audit()
      {
         CRangeLine<DateTime> rl = new CRangeLine<DateTime>();
         rl.AuditEnabled = true;
         CRange<DateTime> r = new CRange<DateTime>(DateTime.Now, DateTime.Now.AddDays(1));
         var result = rl.Add(r);

         Assert.AreEqual<int>(1, result.Count, "One change.");
         Assert.AreEqual<CRange<DateTime>>(null, result.First().Item1, "No segments exist before.");
         Assert.AreEqual<CRange<DateTime>>(r, result.First().Item2, "New segment replaced.");
      }

      [TestMethod(), Description("Asserts that it is possible to add multiple segments on a range line.")]
      public void TestRangeLine_AddMultiple_Count()
      {
         CRangeLine<decimal> rl = new CRangeLine<decimal>();
         rl.Add(1m, 2m);
         rl.Add(3m, 4m);

         Assert.AreEqual<decimal>(1m, rl.Start);
         Assert.AreEqual<decimal>(4m, rl.End);
         Assert.AreEqual<int>(2, rl.Count, "Number of segments.");
      }

      [TestMethod(), Description("Asserts that it is possible to add multiple segments on a range line.")]
      public void TestRangeLine_AddMultiple_Order()
      {
         CRangeLine<int> rl = new CRangeLine<int>();
         rl.Add(3, 4);
         rl.Add(1, 2);
         rl.Add(-3, -2);

         Assert.AreEqual<int>(-3, rl.Start);
         Assert.AreEqual<int>(4, rl.End);

         IRange<int> firstRange = rl.Segments.First();
         IRange<int> lastRange = rl.Segments.Last();

         Assert.AreEqual(-3, firstRange.Start);
         Assert.AreEqual(-2, firstRange.End);
         Assert.AreEqual(3, lastRange.Start);
         Assert.AreEqual(4, lastRange.End);
      }

      [TestMethod(), Description("Asserts that it is possible to add multiple segments on a range line.")]
      public void TestRangeLine_AddMultiple_Overlaps()
      {
         CRangeLine<decimal> rl = new CRangeLine<decimal>();
         rl.Add(1m, 2m);
         rl.Add(1.5m, 4m);

         Assert.AreEqual<decimal>(1m, rl.Start);
         Assert.AreEqual<decimal>(4m, rl.End);

         IRange<decimal> firstRange = rl.Segments.Single();
         Assert.AreEqual<decimal>(1m, firstRange.Start);
         Assert.AreEqual<decimal>(4m, firstRange.End);       
      }

      [TestMethod(), Description("Asserts that it is possible to add multiple segments on a range line.")]
      public void TestRangeLine_AddMultiple_Continuous()
      {
         CRangeLine<decimal> rl = new CRangeLine<decimal>();
         rl.Add(1m, 100m);
         rl.Add(100m, 1000m);

         Assert.AreEqual<decimal>(1m, rl.Start);
         Assert.AreEqual<decimal>(1000m, rl.End);

         IRange<decimal> firstRange = rl.Segments.Single();
         Assert.AreEqual<decimal>(1m, firstRange.Start);
         Assert.AreEqual<decimal>(1000m, firstRange.End);
      }

      #endregion
   }
}
