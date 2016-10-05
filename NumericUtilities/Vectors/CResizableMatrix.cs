using System;
using System.Collections.Generic;
using System.Linq;
using Numeric.Extensions;

namespace Numeric.Vectors
{
   /// <summary>
   /// Represents a 2-dimensional matrix, 
   /// where the generic element is mapped as [<c>i</c>, <c>j</c>], where <c>i</c> is the row index and <c>j</c> is the column index.
   /// </summary>
   /// <typeparam name="T">The type objects to store in the 2-dimensional matrix.</typeparam>
   public class CResizableMatrix<T>: IEquatable<CResizableMatrix<T>>
   {
      private List<List<T>> m_Matrix;
      private CDefaultRowBuilder m_DefaultBuilder;

      /// <summary>
      /// Represents the columns count of the j-dimension.
      /// </summary>
      protected int ColumnsCount { get; private set; }
      
      /// <summary>
      /// Represents the rows count of the i-dimension.
      /// </summary>
      protected int RowsCount { get; private set; }

      /// <summary>
      /// Creates a new instance of 2-dimensional matrix with no data.
      /// </summary>
      public CResizableMatrix()
         : this(new List<List<T>>(), 0, 0)
      {
      }

      /// <summary>
      /// Creates a new instance of 2-dimensional matrix with default data.
      /// </summary>
      /// <param name="rows">The number of rows (<c>i</c> dimension).</param>
      /// <param name="columns">The number of columns (<c>j</c>  dimension).</param>
      public CResizableMatrix(int rows, int columns)
         : this(new List<List<T>>(), columns, rows)
      {
         for (int i = 0; i<rows; i++)
         {
            m_Matrix.Add(m_DefaultBuilder.GetRow(ColumnsCount));
         }
      }

      /// <summary>
      /// Creates a new instance of 2-dimensional matrix of identity type,
      /// where elements on index [i,i] are set to the same value.
      /// </summary>
      /// <param name="rows">The number of rows (<c>i</c> dimension).</param>
      /// <param name="identity">The elemtnt staying on the [i,i] index.</param>
      public CResizableMatrix(T identity, int rows)
         : this(new List<List<T>>(), rows, rows)
      {
         for (int i = 0; i < rows; i++)
         {
            m_Matrix.Add(m_DefaultBuilder.GetRow(ColumnsCount));
            m_Matrix[i][i] = identity;
         }
      }

      /// <summary>
      /// Internal constructor creating a new instance of 2-dimensional matrix.
      /// </summary>
      /// <param name="matrix">The matrix itself.</param>
      /// <param name="colCount">The columns count.</param>
      /// <param name="rowCount">The rows count.</param>
      private CResizableMatrix(List<List<T>> matrix, int colCount, int rowCount)
      {
         this.m_DefaultBuilder = new CDefaultRowBuilder() ;
         this.m_Matrix = matrix;
         this.ColumnsCount = colCount;
         this.RowsCount = rowCount;
      }

      /// <summary>
      /// Represents the value stored for a given matrix cell.
      /// </summary>
      /// <param name="row">The row index.</param>
      /// <param name="column">The column index.</param>
      public T this[int row, int column]
      {
         get { return m_Matrix[row][column]; }
         set { m_Matrix[row][column] = value; }
      }

      /// <summary>
      /// Represents the collection of rows belonging to the current instance of the 2-dimensional matrix.
      /// </summary>
      public IReadOnlyList<IReadOnlyList<T>> Rows
      {
         get { return m_Matrix; }
      }

      /// <summary>
      /// Adds a new column to the 2-dimensional matrix.
      /// </summary>
      /// <returns>The column index of the newly added columm.</returns>
      public int ExtendColumns()
      {
         ColumnsCount++;
         foreach (List<T> row in m_Matrix)
         {
            row.Add(default(T));
         }
         return ColumnsCount - 1;
      }

      /// <summary>
      /// Adds an empry row to the 2-dimensional matrix.
      /// </summary>
      /// <returns>The index of the newly added empty row.</returns>
      public int ExtendRows()
      {
         List<T> row = m_DefaultBuilder.GetRow(ColumnsCount);
         m_Matrix.Add(row);
         RowsCount ++;
         return RowsCount - 1;
      }

      /// <summary>
      /// Adds a row to the 2-dimensional matrix.
      /// </summary>
      /// <param name="args">The row content.</param>
      /// <returns>The index of the newly added row.</returns>
      private int AddRow(params object[] args)
      {
         return AddRow(args.Select(x => (T)x).ToArray());
      }

      /// <summary>
      /// Adds a row to the 2-dimensional matrix.
      /// </summary>
      /// <param name="args">The row content.</param>
      /// <returns>The index of the newly added row.</returns>
      public int AddRow(params T[] args)
      {
         List<T> row = m_DefaultBuilder.GetRow(ColumnsCount);
         int idx = 0;
         while (idx < args.Length && idx < ColumnsCount)
         {
            row[idx] = args[idx];
            idx += 1;
         }
         m_Matrix.Add(row);
         RowsCount += 1;
         return RowsCount - 1;
      }

      /// <summary>
      /// Gets whether the whole list of values in a row are set to their default.
      /// </summary>
      /// <param name="row">The row index.</param>
      /// <returns><c>True</c> if all values are set to their default, <c>false</c> otherwise.</returns>
      public bool RowIsDefault(int row)
      {
         bool isDefaultNull = (default(T) == null);
         Func<T, bool> cellCondition = (cellValue=> (cellValue == null && isDefaultNull) || (cellValue != null && cellValue.Equals(default(T))));
         return RowIs(row, cellCondition); 
      }

      /// <summary>
      /// Gets whether the whole list of values in a row fulfil a certain condition.
      /// </summary>
      /// <param name="row">The row index.</param>
      /// <param name="cellCondition">The single cell conditon as a lambda function.</param>
      /// <returns><c>True</c> if all values fulfill the condition, <c>false</c> otherwise.</returns>
      public bool RowIs(int row, Func<T, bool> cellCondition)
      {
         IReadOnlyList<T> content = m_Matrix[row];
         for (int j = 0; j < ColumnsCount; j++)
         {
            if (!cellCondition(content[j]))
            {
               return false;
            }

         }
         return true;
      }

      /// <summary>
      /// Removes a row at a given index.
      /// </summary>
      /// <param name="index">The index.</param>
      public virtual void RemoveAt(int index)
      {
         List<T> remove = m_Matrix[index];
         m_Matrix.RemoveAt(index);
         RowsCount -= 1;
         remove.Clear();
      }

      /// <summary>
      /// Clears all the rows.
      /// </summary>
      protected void ClearAllRows()
      {
         m_Matrix.Clear();
         RowsCount = 0;
      }

      /// <summary>
      /// Clears all the columns.
      /// </summary>
      protected void ClearAllColumns()
      {
         m_Matrix.ForEach(x => x.Clear());
         ColumnsCount = 0;
      }

      #region "operators"

      /// <summary>
      /// Trasposes the 2-dimensional matrix, by turning rows into columns and vice versa.
      /// </summary>
      public void Traspose()
      {
         List<List<T>> newMatrix = new List<List<T>>();
         for (int j =0; j < ColumnsCount; j++)
         {
            List<T> newRow = new List<T>();
            for (int i = 0; i < RowsCount; i++)
            {
               newRow.Add(this[i, j]);
            }
            newMatrix.Add(newRow);
         }
         m_Matrix = newMatrix;
         int rows = RowsCount;
         RowsCount = ColumnsCount;
         ColumnsCount = rows;
      }

      /// <summary>
      /// Copies the current instance of the 2-dimensional matrix, with dimenstions and data.
      /// </summary>
      /// <returns>New instance of the current 2-dimensional matrix containing same dimenstions and data.</returns>
      public virtual CResizableMatrix<T> Copy()
      {
         List<List<T>> clonedValues = new List<List<T>>();
         foreach (List<T> row in m_Matrix)
         {
            clonedValues.Add(new List<T>(row));
         }
         return new CResizableMatrix<T>(clonedValues, ColumnsCount, RowsCount);
      }

      /// <summary>
      /// Determines whetehr the passed 2-dimensional matrix is equal to the current instance of 2-dimensional matrix.
      /// </summary>
      /// <param name="second">The passed 2-dimensional matrix.</param>
      /// <returns>The equality result, <c>true</c> if satisfied, <c>false</c> otherwise.</returns>
      public virtual bool Equals(CResizableMatrix<T> second)
      {
         if (second == null || this == null)
         {
            return false;
         }
         if (object.ReferenceEquals(second, this))
         {
            return true;
         }
         if (!this.SameSizeOf(second))
         {
            return false;
         }
         if (this.RowsCount == 0 && this.ColumnsCount == 0)
         {
            return true;
         }
         for (int i = 0; i < RowsCount; i++)
         {
            for (int j = 0; j < ColumnsCount; j++)
            {
               object current = this[i, j];
               object other = second[i, j];
               if (current == null || other == null)
               {
                  if (current == null && other == null)
                  {
                     continue;
                  }
                  return false;
               }
               if (!current.Equals(other))
               {
                  return false;
               }
            }
         }
         return true;
      }

      /// <summary>
      /// Transforms the elements contained within a 2-dimensional matrix 
      /// considering their values.
      /// </summary>
      /// <param name="lambda">The lambda function to perform the transformation. 
      /// Takes parameters [<c>original value</c>].</param>
      public void Transform(Func<T, T> lambda)
      {
         Transform((i, j, value) => lambda(value));
      }

      /// <summary>
      /// Transforms the elements contained within a 2-dimensional matrix 
      /// considering their position [<c>i</c>, <c>j</c>] and values.
      /// </summary>
      /// <param name="lambda">The lambda function to perform the transformation. 
      /// Takes parameters [<c>i</c>, <c>j</c>, <c>original value</c>].</param>
      public void Transform(Func<int, int, T, T> lambda)
      {
         for (int i = 0; i < RowsCount; i++)
         {
            for (int j = 0; j < ColumnsCount; j++)
            {
               this[i, j] = lambda(i, j, this[i, j]);
            }
         }
      }

      /// <summary>
      /// Implements the addition binary operator as defined on the 2-dimensional matrices.
      /// </summary>
      /// <param name="a">The first addend.</param>
      /// <param name="b">The second addend.</param>
      /// <returns>The addition result.</returns>
      public static CResizableMatrix<T> operator +(CResizableMatrix<T> a, CResizableMatrix<T> b)
      {        
         if (!a.SameSizeOf(b) || !CNumericTypeExtension.SupportsAddition<T>())
         {
            throw new InvalidOperationException("Addition operation cannot be applied.");
         }
         CResizableMatrix<T> res = new CResizableMatrix<T>(a.RowsCount, a.ColumnsCount);
         res.Transform((i, j, value) => CNumericTypeExtension.Add<T>(a[i, j], b[i, j]));
         return res;
      }

      /// <summary>
      /// Determines whether as instance of 2-dimensional matrix has the same size of the current instance.
      /// </summary>
      /// <param name="second">The instance of a 2-dimensional matrix.</param>
      /// <returns><c>True</c> if i and j sizes are matching, <c>false</c> otherwise.</returns>
      public bool SameSizeOf(CResizableMatrix<T> second)
      {
         return (this.RowsCount == second.RowsCount && this.ColumnsCount == second.ColumnsCount);
      }

      #endregion

      #region "helpers"

      /// <summary>
      /// Class getting the default row object from a single instance of list.
      /// </summary>
      private class CDefaultRowBuilder
      {
         private List<T> m_DefaultRow = new List<T>();
         int m_TempColumnsCount = 0;

         public List<T> GetRow(int columnsCount)
         {
            if (columnsCount == 0)
            {
               m_DefaultRow.Clear();
            }
            else if (columnsCount > m_TempColumnsCount)
            {
               m_DefaultRow.AddRange(Enumerable.Repeat<T>(default(T), columnsCount - m_TempColumnsCount));
            }
            else
            {
               m_DefaultRow.RemoveRange(0, m_TempColumnsCount - columnsCount);               
            }
           m_TempColumnsCount= columnsCount;
           return new List<T>(m_DefaultRow);
         }
      }

      #endregion
   }
}
