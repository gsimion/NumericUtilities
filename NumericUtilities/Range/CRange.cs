using System;

namespace Numeric.Range
{
   /// <summary>
   /// Class defining a generic range.
   /// </summary>
   public class CRange<T> : IRange<T> where T : struct, IComparable
   {
      private T m_Start;
      private T m_End;

      /// <summary>
      /// Represents the range opening point.
      /// </summary>
      /// <value>The opening point.</value>
      public T Start
      {
         get
         {
            return m_Start;
         }
         set
         {
            m_Start = value;
            StartChanged();
         }
      }

      /// <summary>
      /// Represents the range closing point.
      /// </summary>
      /// <value>The closing point.</value>
      public T End
      {
         get
         {
            return m_End;
         }
         set
         {
            this.m_End = value;
            EndChanged();
         }
      }

      /// <summary>
      /// Creates a new instance of range.
      /// </summary>
      /// <param name="start">The opening point.</param>
      /// <param name="end">The closing point.</param>
      public CRange(T start, T end)
         : this()
      {
         if (start.CompareTo(end) > 0)
            throw new ArgumentException("Start value cannot be greater than end value.", "(" + start.ToString() + "," + end.ToString() + ")");
         this.m_Start = start;
         this.m_End = end;
      }

      /// <summary>
      /// Creates a new instance range with its default values.
      /// </summary>
      protected CRange()
      {
      }

      /// <summary>
      /// Method containing the logic run after start value changes.
      /// </summary>
      protected virtual void StartChanged()
      {
         return;
      }

      /// <summary>
      /// Method containing the logic run after end value changes.
      /// </summary>
      protected virtual void EndChanged()
      {
         return;
      }

      /// <summary>
      /// Determines whether the provided value is inside the current instance of range.
      /// </summary>
      /// <param name="value">The value.</param>
      /// <returns><c>True</c> if <paramref name="value">vaue</paramref> falls inside the current range, <c>false</c> otherwise.</returns>
      protected internal bool Contains(T value)
      {
         return (m_Start.CompareTo(value) <= 0) && (value.CompareTo(m_End) <= 0);
      }

      /// <summary>
      /// Determines whether the provided range falls inside the current instance of range.
      /// </summary>
      /// <param name="range">The range.</param>
      /// <returns><c>True</c> if <paramref name="range">range</paramref> falls inside the current instance of range, <c>false</c> otherwise.</returns>
      protected internal bool Contains(CRange<T> range)
      {
         return Contains(range.m_Start) && Contains(range.m_End);
      }

      /// <summary>
      /// Determines whether the provided range shares any sub range with the current instance of range.
      /// </summary>
      /// <param name="range">The range.</param>
      /// <returns><c>True</c> if <paramref name="range">range</paramref> shares any sub range with the current instance of range, <c>false</c> otherwise.</returns>
      protected internal bool IsSharingRange(CRange<T> range)
      {
         return Contains(range.m_Start) || Contains(range.m_End) || range.Contains(this);
      }

      /// <summary>
      /// Equals method.
      /// </summary>
      /// <param name="obj">The object to compare.</param>
      /// <returns>The equality result.</returns>
      public override bool Equals(object obj)
      {
         if (obj == null || !(obj is CRange<T>))
         {
            return false;
         }
         if (object.ReferenceEquals(obj, this))
         {
            return true;
         }

         CRange<T> otherObj = (CRange<T>)obj;
         return m_Start.Equals(otherObj.m_Start) && m_End.Equals(otherObj.m_End);
      }

      /// <summary>
      /// Hash code implementation.
      /// </summary>
      public override int GetHashCode()
      {
         unchecked
         {
            int hash = 31;
            hash = (hash * 23) + m_Start.GetHashCode();
            hash = (hash * 23) + m_End.GetHashCode();
            return hash;
         }
      }

      /// <summary>
      /// Creates a new instance of range object with the same properties.
      /// </summary>
      /// <returns>The cloned instance.</returns>
      public virtual object Clone()
      {
         CRange<T> clone = new CRange<T>();
         clone.m_Start = m_Start;
         clone.m_End = m_End;
         return clone;
      }

      /// <summary>
      /// Performs a cast operation from the range inherited line. Skip the validations.
      /// </summary>
      /// <typeparam name="T1">Inherited type of range.</typeparam>
      /// <returns>The cast range.</returns>
      protected T1 Cast<T1>() where T1 : CRange<T>
      {
         CRange<T> cast = (CRange<T>)Activator.CreateInstance(typeof(T1));
         cast.m_End = m_End;
         cast.m_Start = m_Start;
         return (T1)cast;
      }

      /// <summary>
      /// Gets the string representation of the segment.
      /// </summary>
      /// <returns>1-dimensional segment representation as (a, b)</returns>
      public override string ToString()
      {
         return this.ToString("(a, b)");
      }

      /// <summary>
      /// Gets the string representation of the range [a, b).
      /// Any format can be specified as far as 'a' and 'b' are defined.
      /// </summary>
      /// <param name="format">The format, containing 'a' and/or 'b'</param>
      /// <returns>The 1-dimensional range representation of [a, b) with the specified format.</returns>
      public string ToString(string format)
      {
         return format.Replace("a", Start.ToString()).Replace("b", End.ToString());
      }
   }
}
