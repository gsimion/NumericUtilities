using System;

namespace Numeric.Range
{
   /// <summary>
   /// Defines a generic range.
   /// </summary>
   public interface IRange<T> : ICloneable where T : IComparable
   {
      /// <summary>
      /// Represents the range opening point.
      /// </summary>
      T Start { get; }
      /// <summary>
      /// Represents the range closing point.
      /// </summary>
      T End { get; }
   }
}
