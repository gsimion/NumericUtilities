using System;
using System.Collections.Generic;
using System.Linq;

namespace Numeric.Range
{
   /// <summary>
   /// A range line essentially is a sorted list of non-overlapping ranges.
   /// </summary>
   public sealed class CRangeLine<T> : IRange<T> where T : struct, IComparable
   {
      private bool m_EnableAudit;
      private readonly SortedList<T, CRange<T>> m_Segments;

      private T m_CoveredRangeStart;
      private T m_CoveredRangeEnd;

      /// <summary>
      /// The segment line constructor.
      /// </summary>
      public CRangeLine()
         : this(new SortedList<T, CRange<T>>())
      {
      }

      private CRangeLine(IDictionary<T, CRange<T>> segments)
      {
         this.m_Segments = new SortedList<T, CRange<T>>(segments);
         this.m_EnableAudit = false; // by default audit is not enabled
         GatherStats();
      }

      /// <summary>
      /// Represents whether the audit for add and remove operations is enabled.
      /// </summary>
      public bool AuditEnabled
      {
         set
         {
            m_EnableAudit = value;
         }
         get
         {
            return m_EnableAudit;
         }
      }

      #region IRange

      /// <summary>
      /// Represents the range line coverage opening point.
      /// </summary>
      public T Start
      {
         get
         {
            return m_CoveredRangeStart;
         }
      }
      /// <summary>
      /// Represents the range line coverage closing point.
      /// </summary>
      public T End
      {
         get
         {
            return m_CoveredRangeEnd;
         }
      }

      /// <summary>
      /// Gets a clone of the range line.
      /// </summary>
      /// <returns>The clone range line.</returns>
      public object Clone()
      {
         return new CRangeLine<T>(m_Segments);
      }

      #endregion

      /// <summary>
      /// The segments in the line.
      /// </summary>
      public IReadOnlyList<IRange<T>> Segments
      {
         get
         {
            return m_Segments.Values.ToList();
         }
      }

      /// <summary>
      /// Gets the number of distinct segments stored in the range line.
      /// </summary>
      public int Count
      {
         get
         {
            return m_Segments.Count;
         }
      }

      /// <summary>
      /// Clears all the existing segments from the line.
      /// </summary>
      public void Clear()
      {
         m_Segments.Clear();
         GatherStats();
      }

      /// <summary>
      /// Gets whether a value falls within the range line domain, according to the default definition.
      /// </summary>
      /// <param name="value">The value.</param>
      /// <returns>The result of the default inclusion definition for the value within the range line.</returns>
      public bool IsIncluded(T value)
      {
         if (value.CompareTo(m_CoveredRangeStart) >= 0 && value.CompareTo(m_CoveredRangeEnd) <= 0 &&
            m_Segments.Any(x => x.Value.Contains(value)))
         {
            return true;
         }
         else
         {
            return false;
         }
      }

      /// <summary>
      /// Gets whether a value falls within the range line domain, according to the passed definition.
      /// </summary>
      /// <param name="value">The value.</param>
      /// <param name="inclusionDefinition">The inclusion definition.</param>
      /// <returns>The result of the passed inclusion definition for the value within the range line.</returns>
      public bool IsIncluded(T value, Func<IRange<T>, bool> inclusionDefinition)
      {
         return m_Segments.Any(x => inclusionDefinition(x.Value));
      }

      /// <summary>
      /// Adds a segment overriding existing one(s).
      /// </summary>
      /// <param name="start">The start of the segment to add.</param>
      /// <param name="end">The end of the segment to add.</param>
      /// <returns>The collection of status changes in a form of tuples containing current segment and new segment when audit is enabled, <c>null</c> otherwise. <see cref="AuditEnabled"/></returns>
      public ICollection<Tuple<CRange<T>, CRange<T>>> Add(T start, T end)
      {
         CAudit changes = new CAudit(m_EnableAudit);
         List<Tuple<CRange<T>, OverlapFlag>> affectedRanges = GetAllOverlappingSegments(start, end);
         T newStart = start;
         T newEnd = end;

         foreach (Tuple<CRange<T>, OverlapFlag> r in affectedRanges)
         {
            switch (r.Item2)
            {
               case OverlapFlag.Full:
                  return changes.ToList();
               case OverlapFlag.Overlap:
                  changes.Add(() => (CRange<T>)r.Item1.Clone(), () => null);
                  m_Segments.Remove(r.Item1.Start);
                  break;
               case OverlapFlag.Start:
               case OverlapFlag.StartIncluded:
                  newStart = r.Item1.Start;
                  changes.Add(() => (CRange<T>)r.Item1.Clone(), () => null);
                  m_Segments.Remove(r.Item1.Start);
                  break;
               case OverlapFlag.End:
               case OverlapFlag.EndIncluded:
                  newEnd = r.Item1.End;
                  changes.Add(() => (CRange<T>)r.Item1.Clone(), () => null);
                  m_Segments.Remove(r.Item1.Start);
                  break;
            }
         }
         CRange<T> addedRange = new CRange<T>(newStart, newEnd);
         changes.Add(() => null, () => (CRange<T>)addedRange.Clone());
         m_Segments.Add(addedRange.Start, addedRange);

         GatherStats();

         return changes.ToList();
      }

      /// <summary>
      /// Adds a segment overriding existing one(s).
      /// </summary>
      /// <param name="range">The segment to add.</param>
      /// <returns>The collection of status changes in a form of tuples containing current segment and new segment when audit is enabled, <c>null</c> otherwise. <see cref="AuditEnabled"/></returns>
      public ICollection<Tuple<CRange<T>, CRange<T>>> Add(IRange<T> range)
      {
         return Add(range.Start, range.End);
      }

      /// <summary>
      /// Removes a segment overriding existing one(s).
      /// </summary>
      /// <param name="start">The start of the segment to remove.</param>
      /// <param name="end">The end of the segment to remove.</param>
      /// <returns>The collection of status changes in a form of tuples containing current segment and new segment when audit is enabled, <c>null</c> otherwise. <see cref="AuditEnabled"/></returns>
      public ICollection<Tuple<CRange<T>, CRange<T>>> Remove(T start, T end)
      {
         return Remove(start, end, up => up, down => down);
      }

      /// <summary>
      /// Removes a segment overriding existing one(s).
      /// </summary>
      /// <param name="start">The start of the segment to remove.</param>
      /// <param name="end">The end of the segment to remove.</param>
      /// <param name="roundExactOverlapUp">The function rounding exact overlaps up. 
      /// When dealing with inclusion or discrete range domain, this call allows to handle the case when a line containing a range (b,c) is required to substract a range (a,b); 
      /// (b,c) is going to be modified to (f(b), c).</param>
      /// <param name="roundExactOverlapDown">The function rounding exact overlaps down. 
      /// When dealing with inclusion or discrete range domain, this call allows to handle the case when a line containing a range (a,b) is required to substract a range (b,c); 
      /// (a,b) is going to be modified to (a, f(b)).</param>
      /// <returns>The collection of status changes in a form of tuples containing current segment and new segment when audit is enabled, <c>null</c> otherwise. <see cref="AuditEnabled"/></returns>
      public ICollection<Tuple<CRange<T>, CRange<T>>> Remove(T start, T end, Func<T, T> roundExactOverlapUp, Func<T, T> roundExactOverlapDown)
      {
         CAudit changes = new CAudit(m_EnableAudit);
         List<Tuple<CRange<T>, OverlapFlag>> affectedRanges = GetAllOverlappingSegments(start, end);

         if (affectedRanges.Count > 0)
         {
            T up;
            T down;
            try { up = roundExactOverlapUp(end); }
            catch { up = end; }
            try { down = roundExactOverlapDown(start); }
            catch { down = start; }

            CRange<T> split = null;
            foreach (Tuple<CRange<T>, OverlapFlag> r in affectedRanges)
            {
               switch (r.Item2)
               {
                  case OverlapFlag.Overlap:
                     changes.Add(() => (CRange<T>)r.Item1.Clone(), () => null);
                     m_Segments.Remove(r.Item1.Start);
                     break;
                  case OverlapFlag.Start:
                  case OverlapFlag.StartIncluded:
                     if (r.Item1.Start.CompareTo(down) <= 0)
                     {
                        changes.Add(() => (CRange<T>)r.Item1.Clone(), () => new CRange<T>(r.Item1.Start, down));
                        m_Segments[r.Item1.Start].End = down;
                     }
                     break;
                  case OverlapFlag.Full:
                     //split
                     if (r.Item1.End.CompareTo(end) > 0 && up.CompareTo(r.Item1.End) <= 0)
                     {
                        split = new CRange<T>(up, r.Item1.End);
                        changes.Add(() => null, () => (CRange<T>)split.Clone());
                     }
                     if (r.Item1.Start.CompareTo(down) <= 0)
                     {
                        changes.Add(() => (CRange<T>)r.Item1.Clone(), () => new CRange<T>(r.Item1.Start, down));
                        m_Segments[r.Item1.Start].End = down;
                     }
                     break;
                  case OverlapFlag.End:
                  case OverlapFlag.EndIncluded:
                     if (up.CompareTo(r.Item1.End) <= 0)
                     {
                        CRange<T> clone = (CRange<T>)m_Segments[r.Item1.Start].Clone();
                        clone.Start = up;
                        changes.Add(() => (CRange<T>)r.Item1.Clone(), () => (CRange<T>)clone.Clone());
                        m_Segments.Remove(r.Item1.Start);
                        m_Segments.Add(clone.Start, clone);
                     }
                     break;
               }
            }
            if (split != null)
            {
               m_Segments.Add(split.Start, split);
            }
            GatherStats();
         }

         return changes.ToList();
      }

      /// <summary>
      /// Removes a segment overriding existing one(s).
      /// </summary>
      /// <param name="range">The segment to remove.</param>
      /// <returns>The collection of status changes in a form of tuples containing current segment and new segment.</returns>
      public ICollection<Tuple<CRange<T>, CRange<T>>> Remove(IRange<T> range)
      {
         return Remove(range.Start, range.End);
      }

      /// <summary>
      /// Updates aggregate coverage values for range line start and end points.
      /// </summary>
      private void GatherStats()
      {
         if (m_Segments.Count == 0)
         {
            m_CoveredRangeStart = default(T);
            m_CoveredRangeStart = default(T);
         }
         else
         {
            m_CoveredRangeStart = m_Segments.First().Key;
            m_CoveredRangeEnd = m_Segments.Last().Value.End;
         }
      }

      /// <summary>
      /// Gets all the overlapping segments within the line, according to the overlap definition.
      /// </summary>
      /// <param name="start">The range start.</param>
      /// <param name="end">The range end.</param>
      /// <returns>The list containing all the overlapping segments within the context of the passed paraemters.</returns>
      private List<Tuple<CRange<T>, OverlapFlag>> GetAllOverlappingSegments(T start, T end)
      {
         List<Tuple<CRange<T>, OverlapFlag>> res = new List<Tuple<CRange<T>, OverlapFlag>>();
         if (end.CompareTo(m_CoveredRangeStart) < 0 || start.CompareTo(m_CoveredRangeEnd) > 0)
            return res;

         foreach (CRange<T> r in m_Segments.Values)
         {
            if (r.End.CompareTo(start) < 0)
               continue;
            if (r.Start.CompareTo(end) > 0)
               break;
            res.Add(new Tuple<CRange<T>, OverlapFlag>(r, Classify(start, end, r.Start, r.End)));
         }

         return res;
      }

      /// <summary>
      /// Classifies a segment against another.
      /// </summary>
      /// <param name="a">The segment to classify start.</param>
      /// <param name="b">The segment to classify end.</param>
      /// <param name="aHat">The segment to classify against start.</param>
      /// <param name="bHat">The segment to classify against end.</param>
      /// <returns>The internal classification.</returns>
      private static OverlapFlag Classify(T a, T b, T aHat, T bHat)
      {
         if (a.CompareTo(bHat) > 0 || b.CompareTo(aHat) < 0)
            return OverlapFlag.Out;
         else if (a.CompareTo(aHat) <= 0 && b.CompareTo(bHat) >= 0)
            return OverlapFlag.Overlap;
         else if (a.CompareTo(aHat) > 0 && a.CompareTo(bHat) < 0 && b.CompareTo(bHat) > 0)
            return OverlapFlag.Start;
         else if (a.CompareTo(aHat) > 0 && a.CompareTo(bHat) == 0)
            return OverlapFlag.StartIncluded;
         else if (a.CompareTo(aHat) < 0 && b.CompareTo(aHat) > 0 && b.CompareTo(bHat) < 0)
            return OverlapFlag.End;
         else if (a.CompareTo(aHat) < 0 && b.CompareTo(aHat) == 0)
            return OverlapFlag.EndIncluded;
         else if (a.CompareTo(aHat) > 0 || b.CompareTo(bHat) < 0)
            return OverlapFlag.Full;
         else
            throw new InvalidOperationException();
      }

      /// <summary>
      /// Given a, b |------|, this set contains how the segment is classified within the line context.
      /// </summary>
      private enum OverlapFlag
      {
         /// <summary>
         /// a, b |------|
         ///        |--|
         ///      |--|
         ///         |---|
         ///      |------|
         /// </summary>
         Overlap,
         /// <summary>
         /// a, b |------|
         ///     |--------|
         /// </summary>
         Full,
         /// <summary>
         /// a, b |------|
         ///    |-----|
         /// </summary>
         Start,
         /// <summary>
         /// a, b |------|
         ///  |---|
         /// </summary>
         StartIncluded,
         /// <summary>
         /// a, b |------|
         ///         |-----|
         /// </summary>
         End,
         /// <summary>
         /// a, b |------|
         ///             |---|
         /// </summary>
         EndIncluded,
         /// <summary>
         /// a, b |------|
         ///  |--|
         ///              |--|
         /// </summary>
         Out
      }

      /// <summary>
      /// Class wrapping the audit operations.
      /// </summary>
      private class CAudit
      {
         private readonly List<Tuple<CRange<T>, CRange<T>>> m_List;
         private readonly bool m_Enabled;

         public CAudit(bool enabled)
         {
            this.m_Enabled = enabled;
            if (enabled)
               this.m_List = new List<Tuple<CRange<T>, CRange<T>>>();
         }

         public void Add(Func<CRange<T>> previous, Func<CRange<T>> current)
         {
            if (m_Enabled)
               m_List.Add(new Tuple<CRange<T>, CRange<T>>(previous(), current()));
         }

         public List<Tuple<CRange<T>, CRange<T>>> ToList()
         {
            return m_List;
         }
      }
   }
}
