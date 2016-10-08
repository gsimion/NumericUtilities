using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Numeric.Extensions;

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

      private CMap<int, string> m_Columns;

      private List<Column> m_MaterializedView;

      /// <summary>
      /// Hash non-unique indexes per column, as a dictionary of column index, maps [row index, value].
      /// </summary>
      private readonly Dictionary<int, Dictionary<int, int>> m_ColumnIndexes;
      
      /// <summary>
      /// Structure representing a column of the 2-dimensional matrix.
      /// </summary>
      public class Column : Tuple<int, string>
      {
         /// <summary>
         /// Creates a new column within the current instance of 2-dimensional matrix.
         /// </summary>
         /// <param name="ordinal"></param>
         /// <param name="name"></param>
         /// <remarks></remarks>
         internal Column(int ordinal, string name)
            : base(ordinal, name)
         { }

         /// <summary>
         /// Represents the name of the column.
         /// </summary>
         public string Name { get { return Item2; } }

         /// <summary>
         /// Represents the ordinal of the column.
         /// </summary>
         public int Ordinal { get { return Item1; } }
      }

      /// <summary>
      /// Creates a new instance of 2-dimensional matrix with no structure or data.
      /// </summary>
      public CTableMatrix()
         : this(null, 0, 0)
      {
      }

      /// <summary>
      /// Internal constructor creating a new instance of 2-dimensional matrix.
      /// </summary>
      /// <param name="matrix">The matrix itself.</param>
      /// <param name="colCount">The columns count.</param>
      /// <param name="rowCount">The rows count.</param>
      private CTableMatrix(List<List<T>> matrix, int colCount, int rowCount)
         : base(rowCount, colCount)
      {
         this.m_Columns = new CMap<int, string>();
         this.m_ColumnIndexes = new Dictionary<int, Dictionary<int, int>>();
         if (matrix == null)
            return;
         for (int i = 0; i < RowsCount; i++)
         {
            var value_i = matrix[i];
            for (int j = 0; j < ColumnsCount; j++)
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
         : this(null, table.Columns.Count, table.Rows.Count)
      {
         foreach (DataColumn column in table.Columns)
         {
            m_Columns.Add(column.Ordinal, column.ColumnName);
         }
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
         get 
         {
            if (m_MaterializedView == null || m_MaterializedView.Count != m_Columns.Count)
            {
               m_MaterializedView = m_Columns.Values.Select(x => new Column(x.Item1, x.Item2)).ToList();
            }
            return m_MaterializedView;
         }
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
         if (!m_Columns.Forward.ContainsKey(column))
         {
            throw new ArgumentException("There is no column defined for the passed index.", Utilities.CUtility.GetParameters(System.Reflection.MethodInfo.GetCurrentMethod(), 1));
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
            throw new ArgumentException("There is no index defined for the column passed.", Utilities.CUtility.GetParameters(System.Reflection.MethodInfo.GetCurrentMethod(), 1));
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
         if ((m_Columns.Reverse.ContainsKey(name)))
         {
            throw new ArgumentException("Column name is already defined for this context.", Utilities.CUtility.GetParameters(System.Reflection.MethodInfo.GetCurrentMethod(), 1));
         }
         m_Columns.Add(newColumnIndex, name);
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
         int index;
         if (!m_Columns.Reverse.TryGetValue(column, out index))
         {
            return -1;
         }
         return index;
      }

      /// <summary>
      /// Parse the current instance of 2-dimensional matrix to a datatable.
      /// </summary>
      /// <returns>The datatable.</returns>
      public DataTable ToDataTable()
      {
         DataTable t = new DataTable();
         foreach (var col in m_Columns.Values.OrderBy(x => x.Item1))
         {
            t.Columns.Add(col.Item2, typeof(T));
         }
         foreach (List<T> row in Rows)
         {
            t.Rows.Add(row.Cast<object>().ToArray());
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
         CMap<int, string> clonedColumns = new CMap<int, string>();
         foreach(var map in m_Columns.Values)
         {
            clonedColumns.Add(map.Item1, map.Item2);
         }
         List<List<T>> clonedValues = new List<List<T>>();
         if (includeRows)
         {
            foreach (var row in Rows)
            {
               clonedValues.Add(new List<T>(row));
            }
         }
         int newRowCount = includeRows ? RowsCount : 0;
         CTableMatrix<T> clone = new CTableMatrix<T>(clonedValues, ColumnsCount, newRowCount);
         clone.m_Columns = clonedColumns;
         return clone;
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
         if (!m_Columns.Values.SequenceEqual(second.m_Columns.Values))
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
