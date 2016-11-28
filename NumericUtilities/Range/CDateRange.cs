using System;

namespace Numeric.Range
{
   /// <summary>
   /// Class defining a date range, where day is the least important unit.
   /// </summary>
   public class CDateRange : CRange<DateTime>
   {
      /// <summary>
      /// Creates a new date range.
      /// </summary>
      /// <param name="start">Start</param>
      /// <param name="end">End</param>
      public CDateRange(DateTime start, DateTime end)
         : base(start.Date, end.Date)
      {
      }

      /// <summary>
      /// Creates a new date range covering the full available date range.
      /// </summary>
      public CDateRange()
         : base(DateTime.MinValue.Date, DateTime.MaxValue.Date)
      {
      }

      /// <summary>
      /// Gets the string representation of the date range. <see cref="CDateRange"/>
      /// </summary>
      /// <returns>The string representation of the date range.</returns>
      public override string ToString()
      {
         return string.Format("({0}, {1})", Start.ToShortDateString(), End.ToShortDateString());
      }

      /// <summary>
      /// Creates a new instance of date range object with the same properties.
      /// </summary>
      /// <returns>The cloned instance.</returns>
      public override object Clone()
      {
         return base.Cast<CDateRange>();
      }
   }
}
