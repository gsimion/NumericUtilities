﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace Numeric.Vectors
{
   /// <summary>
   /// Represents a 2-dimensional matrix storing data and allowing reading data on a similar way they are read for the .net datatable.
   /// </summary>
   /// <typeparam name="T">The type objects to store in the 2-dimensional matrix.</typeparam>
   public sealed class CTableMatrix<T> : CResizableMatrix<T>, ICloneable
   {
      /// <summary>
      /// Represents the default value assigned to the index when matrix value is <c>null</c>.
      /// </summary>
      public const int NullHashing = int.MinValue;

      private readonly List<Column> m_Columns;

      /// <summary>
      /// Hash non-unique indexes per column, as a dictionary of column index, maps [row index, value].
      /// </summary>
      private readonly Dictionary<int, Dictionary<int, int>> m_ColumnIndexes;
      
      /// <summary>
      /// Structure representing a column of the 2-dimensional matrix.
      /// </summary>
      public struct Column
      {
         private readonly KeyValuePair<int, string> m_Info;
         /// <summary>
         /// Creates a new column within the current instance of 2-dimensional matrix.
         /// </summary>
         /// <param name="iOrdinal"></param>
         /// <param name="sName"></param>
         /// <remarks></remarks>
         internal Column(int iOrdinal, string sName)
         {
            m_Info = new KeyValuePair<int, string>(iOrdinal, sName);
         }

         /// <summary>
         /// Represents the name of the column.
         /// </summary>
         public string Name
         {
            get { return m_Info.Value; }
         }

         /// <summary>
         /// Represents the ordinal of the column.
         /// </summary>
         public int Ordinal
         {
            get { return m_Info.Key; }
         }
      }

      /// <summary>
      /// Creates a new instance of 2-dimensional matrix with no structure or data.
      /// </summary>
      public CTableMatrix()
         : this(new List<Column>(), null, 0, 0)
      {
      }

      /// <summary>
      /// Internal constructor creating a new instance of 2-dimensional matrix.
      /// </summary>
      /// <param name="columns">The collection containing the columns.</param>
      /// <param name="matrix">The matrix itself.</param>
      /// <param name="colCount">The columns count.</param>
      /// <param name="rowCount">The rows count.</param>
      private CTableMatrix(List<Column> columns, List<List<T>> matrix, int colCount, int rowCount)
         : base(rowCount, colCount)
      {
         this.m_Columns = columns;
         this.m_ColumnIndexes = new Dictionary<int, Dictionary<int, int>>();
         if (matrix == null)
            return;
         for (int i = 0; i < rowCount;i++ )
         {
            var value_i = matrix[i];
            for (int j = 0; j < rowCount; j++)
            {
               this[i, j] = value_i[j];
            }         
         }       
      }

      /// <summary>
      /// Creates a new instance of 2-dimensional matrix from a datatable.
      /// </summary>
      /// <param name="table">The datatable to create the instance from.</param>
      public CTableMatrix(DataTable table)
         : this(null, null, table.Columns.Count, table.Rows.Count)
      {
         this.m_Columns = new List<Column>(table.Columns.Cast<DataColumn>().Select(x=>new Column(x.Ordinal, x.ColumnName)));
         for (int i = 0; i < RowsCount; i++)
         {
            var value_i = table.Rows[i];
            for (int j = 0; j < ColumnsCount; j++)
            {
               this[i, j] = (T)value_i[j];
            }
         }    
      }

      /// <summary>
      /// Represents the value stored for a given matrix cell.
      /// </summary>
      /// <param name="row">The row index.</param>
      /// <param name="column">The column name.</param>
      public T this[int row, string column]
      {
         get { return this[row, IndexOf(column)]; }
         set { this[row, IndexOf(column)] = value; }
      }

      /// <summary>
      /// Represents the list of the column belonging to the current instance of the 2-dimensional matrix.
      /// </summary>
      public ICollection<Column> Columns
      {
         get { return m_Columns; }
      }

      /// <summary>
      /// Adds an hash non unique index within the matrix context on a column level, using the default hash function.
      /// </summary>
      /// <param name="column">The column name to add the index to.</param>
      public void AddHashIndex(string column)
      {
         AddHashIndex(IndexOf(column));
      }

      /// <summary>
      /// Adds an hash non unique index within the matrix context on a column level, using the default hash function.
      /// </summary>
      /// <param name="column">The column index to add the index to.</param>
      public void AddHashIndex(int column)
      {
         if (!m_Columns.Any(x => x.Ordinal == column))
         {
            throw new ArgumentException("There is no column defined for the passed index.");
         }
         m_ColumnIndexes[column] = new Dictionary<int, int>();
         for (int i = 0; i < RowsCount; i++)
         {
            m_ColumnIndexes[column].Add(i, GetHash(column, i));
         }
      }

      /// <summary>
      /// Retrieves the value of the underlying hashing column index for a given row index.
      /// </summary>
      /// <param name="column">The column index.</param>
      /// <param name="row">The row index.</param>
      /// <returns>The hash indexed value.</returns>
      public int GetIndexValue(int column, int row)
      {
         Dictionary<int, int> currentHashingDictionary = null;
         if (!m_ColumnIndexes.TryGetValue(column, out currentHashingDictionary))
         {
            throw new ArgumentException("There is no index defined for the column passed.");
         }
         return currentHashingDictionary[row];
      }

      /// <summary>
      /// Gets the hash of a matrix value.
      /// </summary>
      /// <param name="column">The column index.</param>
      /// <param name="row">The row index.</param>
      /// <returns>The default .net hash if the value is not <c>null</c>, default otherwise. <see cref="NullHashing" /></returns>
      private int GetHash(int column, int row)
      {
         object value = this[row, column];
         return (value == null) ? NullHashing : value.GetHashCode();
      }

      /// <summary>
      /// Adds a new column to the 2-dimensional matrix.
      /// </summary>
      /// <param name="name">The column name to map.</param>
      /// <returns>The column index of the newly added columm.</returns>
      public int AddColumn(string name)
      {
         int newColumnIndex = ExtendColumns();
         if ((m_Columns.Any(x => x.Name.Equals(name))))
         {
            throw new ArgumentException("Column name is already defined for this context.");
         }
         m_Columns.Add(new Column(newColumnIndex, name));
         return newColumnIndex;
      }

      /// <summary>
      /// Adds an empry row to the 2-dimensional matrix.
      /// </summary>
      /// <returns>The index of the newly added empty row.</returns>
      public int AddRow()
      {
         int newRowIndex = ExtendRows();
         foreach (int columnIndexed in m_ColumnIndexes.Keys)
         {
            m_ColumnIndexes[columnIndexed].Add(newRowIndex, GetHash(newRowIndex, columnIndexed));
         }
         return newRowIndex;
      }

      /// <summary>
      /// Removes a row at a given index.
      /// </summary>
      /// <param name="index">The index.</param>
      public override void RemoveAt(int index)
      {
         base.RemoveAt(index);
         //remove all the indexes of the specific removed row and update existing
         foreach (int columnIndex in m_ColumnIndexes.Keys)
         {
            bool bExists = false;
            HashSet<int> updates = new HashSet<int>();
            Dictionary<int, int> currentDictionary = m_ColumnIndexes[columnIndex];
            foreach (int iRowIndexed in currentDictionary.Keys)
            {
               if (iRowIndexed == index)
               {
                  bExists = true;
               }
               if (iRowIndexed > index)
               {
                  updates.Add(iRowIndexed);
               }
            }
            if (bExists)
            {
               currentDictionary.Remove(index);
            }
            if (updates.Any())
            {
               foreach (int indexToUpdate in updates.OrderBy(x => x))
               {
                  int value = currentDictionary[indexToUpdate];
                  currentDictionary.Remove(indexToUpdate);
                  currentDictionary.Add(indexToUpdate - 1, value);
               }
            }
         }
      }

      /// <summary>
      /// Finds the index, if any, of the column name.
      /// </summary>
      /// <param name="column">The column name to look for the index.</param>
      /// <returns>Index as a positive integer if found, <c>-1</c> otherwise.</returns>
      public int IndexOf(string column)
      {
         Column col = m_Columns.SingleOrDefault(x => x.Name.Equals(column));
         if (col.Equals(new Column()))
         {
            return -1;
         }
         else
         {
            return col.Ordinal;
         }
      }

      /// <summary>
      /// Parse the current instance of 2-dimensional matrix to a datatable.
      /// </summary>
      /// <returns>The datatable.</returns>
      public DataTable ToDataTable()
      {
         DataTable t = new DataTable();
         foreach (Column col in m_Columns)
         {
            t.Columns.Add(col.Name, typeof(T));
         }
         foreach (List<T> row in Rows)
         {
            t.Rows.Add(row.ToArray());
         }
         return t;
      }

      /// <summary>
      /// Clones the current instance of the 2-dimensional matrix.
      /// </summary>
      /// <returns>New instance of the current 2-dimensional matrix.</returns>
      public object Clone()
      {
         return Clone(false);
      }

      /// <summary>
      /// Clones the current instance of the 2-dimensional matrix.
      /// </summary>
      /// <param name="includeRows">Defines whether to include the rows.</param>
      /// <returns>New instance of the current 2-dimensional matrix.</returns>
      public CTableMatrix<T> Clone(bool includeRows)
      {
         List<Column> clonedColumns = m_Columns.Select(x => x).ToList();
         List<List<T>> clonedValues = new List<List<T>>();
         if (includeRows)
         {
            foreach (var row in Rows)
            {
               clonedValues.Add(new List<T>(row));
            }
         }
         int newRowCount = includeRows ? RowsCount : 0;
         return new CTableMatrix<T>(clonedColumns, clonedValues, ColumnsCount, newRowCount);
      }

      /// <summary>
      /// Clears all the data contained in the current instance of the 2-dimensional matrix.
      /// </summary>
      public void Clear()
      {
         ClearAllRows();
      }

      /// <summary>
      /// Merges another instance of a 2-dimensional matrix containing the same type of objects to the current instance.
      /// </summary>
      /// <param name="second">The instance of 2-dimensional matrix to merge to the current.</param>
      public void Merge(CTableMatrix<T> second)
      {
         if (!m_Columns.SequenceEqual(second.m_Columns))
         {
            throw new InvalidOperationException("Cannot merge different structures.");
         }

         foreach (List<T> row in second.Rows)
         {
            AddRow(row.ToArray());
         }
      }
   }
}
