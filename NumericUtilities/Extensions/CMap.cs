using System;
using System.Collections.Generic;
using System.Linq;

namespace Numeric.Extensions
{
   /// <summary>
   /// Interface defining a map direction.
   /// </summary>
   /// <typeparam name="T1">The type of the first collection.</typeparam>
   /// <typeparam name="T2"><The type of the second collection./typeparam>
   public interface IMapDirection<T1, T2>
   {
      /// <summary>
      /// Represents the map direction item.
      /// </summary>
      /// <param name="item">The item.</param>
      T2 this[T1 item] { get; set; }

      /// <summary>
      /// Gets whether the map direction contains a given item.
      /// </summary>
      /// <param name="item">The item.</param>
      /// <returns>The result.</returns>
      bool Contains(T1 item);
   }

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
      /// Creates a new instance of bidirectional mapping object.
      /// </summary>
      public CMap()
      {
         this.Forward = new CIndexer<T1, T2>(m_Forward);
         this.Reverse = new CIndexer<T2, T1>(m_Reverse);
      }

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

      private class CIndexer<T3, T4> : IMapDirection<T3, T4>
      {
         private Dictionary<T3, T4> m_Dictionary;
         public CIndexer(Dictionary<T3, T4> dictionary)
         {
            this.m_Dictionary = dictionary;
         }
         public T4 this[T3 item]
         {
            get { return m_Dictionary[item]; }
            set { m_Dictionary[item] = value; }
         }
         public bool Contains(T3 item)
         {
            return m_Dictionary.ContainsKey(item);
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
      public IMapDirection<T1, T2> Forward { get; private set; }
      
      /// <summary>
      /// Represents the map in its reverse direction.
      /// </summary>
      public IMapDirection<T2, T1> Reverse { get; private set; }
   }
}
