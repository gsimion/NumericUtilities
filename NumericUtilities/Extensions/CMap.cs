using System;
using System.Collections.Generic;
using System.Linq;

namespace Numeric.Extensions
{
   /// <summary>
   /// Class representing a bidirectional mapping object,
   /// accessible and indexed by both directions.
   /// </summary>
   /// <typeparam name="T1">The type of the first collection.</typeparam>
   /// <typeparam name="T2">The type of the second collection.</typeparam>
   public class CMap<T1, T2>
   {
      private Dictionary<T1, T2> m_Forward = new Dictionary<T1, T2>();
      private Dictionary<T2, T1> m_Reverse = new Dictionary<T2, T1>();

      /// <summary>
      /// Represents the collection of maps.
      /// </summary>
      public IReadOnlyCollection<Tuple<T1, T2>> Values
      {
         get
         {
            return m_Forward.Select(x => new Tuple<T1, T2>(x.Key, x.Value)).ToList();
         }
      }

      /// <summary>
      /// Represents the maps count.
      /// </summary>
      public int Count
      {
         get
         {
            return m_Forward.Count;
         }
      }

      /// <summary>
      /// Adds a map.
      /// </summary>
      /// <param name="first">The first item.</param>
      /// <param name="second">The second item.</param>
      public void Add(T1 first, T2 second)
      {
         m_Forward.Add(first, second);
         m_Reverse.Add(second, first);
      }

      /// <summary>
      /// Represents the map in its forward direction.
      /// </summary>
      public IReadOnlyDictionary<T1, T2> Forward { get { return m_Forward; } }
      
      /// <summary>
      /// Represents the map in its reverse direction.
      /// </summary>
      public IReadOnlyDictionary<T2, T1> Reverse { get { return m_Reverse; } }
   }
}
