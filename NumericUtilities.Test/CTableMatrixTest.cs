using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.Linq;
using Numeric.Vectors;

namespace Numeric.Test
{
   [TestClass]
   public class CTableMatrixTest
   {
      [TestMethod(), Description("Asserts that the default constructor returns an empty matrix with the right properties.")]
      public void TestTableMatrix_DefaultConstructor()
      {
         CTableMatrix<object> Matrix = new CTableMatrix<object>();

         Assert.IsNotNull(Matrix);
         Assert.AreEqual(0, Matrix.Columns.Count);
         Assert.AreEqual(0, Matrix.Rows.Count);
      }

      [TestMethod(), Description("Asserts that the default constructor returns an empty matrix with the right properties.")]
      public void TestTableMatrix_DefaultConstructor_EmptyTable()
      {
         DataTable t = new DataTable();
         CTableMatrix<object> Matrix = new CTableMatrix<object>(t);

         Assert.IsNotNull(Matrix);
         Assert.AreEqual(0, Matrix.Columns.Count);
         Assert.AreEqual(0, Matrix.Rows.Count);
      }

      [TestMethod(), Description("Asserts that the default constructor returns the right properties.")]
      public void TestTableMatrix_DefaultConstructor_Table()
      {
         DataTable Table = new DataTable();
         Table.Columns.Add("1", typeof(string));
         Table.Columns.Add("2", typeof(string));
         Table.Rows.Add("a", "b");
         Table.Rows.Add("c", "d");
         CTableMatrix<object> Matrix = new CTableMatrix<object>(Table);

         Assert.IsNotNull(Matrix);
         Assert.AreEqual(Table.Columns.Count, Matrix.Columns.Count);
         Assert.AreEqual(Table.Rows.Count, Matrix.Rows.Count);
      }

      [TestMethod(), Description("Asserts that the default constructor returns the correct values.")]
      public void TestTableMatrix_DefaultConstructor_StringTable_Values()
      {
         DataTable Table = new DataTable();
         Table.Columns.Add("1", typeof(string));
         Table.Columns.Add("2", typeof(string));
         Table.Rows.Add("a", "b");
         Table.Rows.Add("c", "d");
         CTableMatrix<object> Matrix = new CTableMatrix<object>(Table);

         Assert.AreEqual("a", Matrix[0, 0]);
         Assert.AreEqual("b", Matrix[0, 1]);
         Assert.AreEqual("c", Matrix[1, 0]);
         Assert.AreEqual("d", Matrix[1, 1]);
      }

      [TestMethod(), Description("Asserts that the default constructor returns the correct values.")]
      public void TestTableMatrix_DefaultConstructor_ObjectTable_Values()
      {
         DataTable Table = new DataTable();
         Table.Columns.Add("1", typeof(object));
         Table.Columns.Add("2", typeof(object));
         Table.Rows.Add(1, "2");
         Table.Rows.Add("3", 4m);
         CTableMatrix<object> Matrix = new CTableMatrix<object>(Table);

         Assert.AreEqual(1, Matrix[0, 0]);
         Assert.AreEqual("2", Matrix[0, 1]);
         Assert.AreEqual("3", Matrix[1, 0]);
         Assert.AreEqual(4m, Matrix[1, 1]);
      }

      #region "tests: hash indexing"

      [TestMethod(), Description("Asserts that an index cannot be added on a non existing column.")]
      [ExpectedException(typeof(ArgumentException), "There is no column defined for the passed index.")]
      public void TestResizableMatrix_AddIndex_NotExistingColumn()
      {
         CTableMatrix<object> Matrix = new CTableMatrix<object>();

         Matrix.AddHashIndex(10);
      }

      [TestMethod(), Description("Asserts that an index can be added on a column with empty values and is building the hashing correctly.")]
      public void TestResizableMatrix_AddIndex_EmptyValues_IndexValues()
      {
         CTableMatrix<object> Matrix = new CTableMatrix<object>();
         Matrix.AddColumn("column1");
         Matrix.AddColumn("column2");
         Matrix.AddRow();
         Matrix.AddRow();
         Matrix.AddRow();
         Matrix.AddHashIndex(0);

         Assert.AreEqual(CTableMatrix<object>.NullHashing, Matrix.GetIndexValue(0, 0));
         Assert.AreEqual(CTableMatrix<object>.NullHashing, Matrix.GetIndexValue(0, 1));
         Assert.AreEqual(CTableMatrix<object>.NullHashing, Matrix.GetIndexValue(0, 2));
      }

      [TestMethod(), Description("Asserts that an the hashing cannot be called without an index defined.")]
      [ExpectedException(typeof(ArgumentException), "There is no index defined for the column passed.")]
      public void TestResizableMatrix_GetIndexValue_NoIndex()
      {
         CTableMatrix<object> Matrix = new CTableMatrix<object>();
         Matrix.AddColumn("column1");
         Matrix.AddColumn("column2");
         Matrix.AddRow();
         Matrix.AddRow();
         Matrix.AddHashIndex(0);

         Matrix.GetIndexValue(1, 0);
      }

      [TestMethod(), Description("Asserts that an index can be added on a column with empty values and is building the hashing correctly.")]
      public void TestResizableMatrix_AddIndex_Values_IndexValues()
      {
         CTableMatrix<object> Matrix = new CTableMatrix<object>();
         Matrix.AddColumn("column1");
         Matrix.AddColumn("column2");
         Matrix.AddRow("same", 1);
         Matrix.AddRow("different", 3);
         Matrix.AddRow("same", 1);
         Matrix.AddHashIndex(0);
         Matrix.AddHashIndex(1);

         Assert.AreNotEqual(CTableMatrix<object>.NullHashing, Matrix.GetIndexValue(0, 0));
         Assert.AreNotEqual(CTableMatrix<object>.NullHashing, Matrix.GetIndexValue(0, 1));

         Assert.AreEqual(Matrix.GetIndexValue(0, 0), Matrix.GetIndexValue(0, 2));
         Assert.AreEqual(Matrix.GetIndexValue(1, 0), Matrix.GetIndexValue(1, 2));
         Assert.AreNotEqual(Matrix.GetIndexValue(0, 0), Matrix.GetIndexValue(0, 1));
         Assert.AreNotEqual(Matrix.GetIndexValue(1, 0), Matrix.GetIndexValue(1, 1));
      }

      #endregion

      #region "tests: add rows"

      [TestMethod(), Description("Asserts that matrix updates its properties when rows are added.")]
      public void TestResizableMatrix_AddRow_Properties()
      {
         CTableMatrix<object> Matrix = new CTableMatrix<object>();
         Matrix.AddColumn("column1");
         Matrix.AddColumn("column2");
         Matrix.AddRow(1, "2");
         Matrix.AddRow();
         Matrix.AddRow(1);
         Matrix.AddRow("2", null, "3");

         Assert.AreEqual(4, Matrix.Rows.Count);
         Assert.AreEqual(2, Matrix.Columns.Count);
      }

      [TestMethod(), Description("Asserts that matrix add rows return the index of the newly added rows.")]
      public void TestResizableMatrix_AddRow_Index()
      {
         CTableMatrix<object> Matrix = new CTableMatrix<object>();
         Matrix.AddColumn("column1");
         Matrix.AddColumn("column2");

         Assert.AreEqual(0, Matrix.AddRow(1, "2"));
         Assert.AreEqual(1, Matrix.AddRow());
         Assert.AreEqual(2, Matrix.AddRow(1));
      }

      [TestMethod(), Description("Asserts that matrix add rows return the index of the newly added rows for a non empty matrix.")]
      public void TestResizableMatrix_AddRow_NonEmptyInit_Index()
      {
         DataTable Table = new DataTable();
         Table.Columns.Add("1", typeof(int));
         Table.Columns.Add("2", typeof(int));
         Table.Rows.Add(1, 2);
         Table.Rows.Add(3, 4);
         CTableMatrix<object> Matrix = new CTableMatrix<object>(Table);

         Assert.AreEqual(2, Matrix.AddRow(1, "2"));
         Assert.AreEqual(3, Matrix.AddRow());
         Assert.AreEqual(4, Matrix.AddRow(1));
      }

      [TestMethod(), Description("Asserts that matrix cells are populated with the right values when row is added.")]
      public void TestResizableMatrix_AddRow_CorrectValues()
      {
         CTableMatrix<object> Matrix = new CTableMatrix<object>();
         Matrix.AddColumn("column1");
         Matrix.AddColumn("column2");
         Matrix.AddRow(1, "2");

         Assert.AreEqual(1, Matrix[0, "column1"]);
         Assert.AreEqual("2", Matrix[0, "column2"]);
      }

      #endregion

      #region "tests: remove row"

      [TestMethod(), Description("Asserts that matrix updates its properties when row is removed.")]
      public void TestResizableMatrix_RemoveRow_LastRow_Properties()
      {
         CTableMatrix<object> Matrix = new CTableMatrix<object>();
         Matrix.AddColumn("column1");
         Matrix.AddColumn("column2");
         Matrix.AddRow();
         Matrix.AddRow();
         int index = Matrix.AddRow();
         Matrix.RemoveAt(index);

         Assert.AreEqual(2, Matrix.Rows.Count);
         Assert.AreEqual(2, Matrix.Columns.Count);
      }

      [TestMethod(), Description("Asserts that matrix updates its values when first rows are removed.")]
      public void TestResizableMatrix_RemoveRow_FirstRows_Values()
      {
         CTableMatrix<object> Matrix = new CTableMatrix<object>();
         Matrix.AddColumn("column1");
         Matrix.AddColumn("column2");
         Matrix.AddRow();
         Matrix.AddRow("value", 1);
         Matrix.AddRow(1, "value");
         Matrix.RemoveAt(0);

         Assert.AreEqual("value", Matrix[0, 0]);
         Assert.AreEqual("value", Matrix[1, 1]);
         Assert.AreEqual(1, Matrix[0, 1]);
         Assert.AreEqual(1, Matrix[1, 0]);
      }

      #endregion

      #region "tests: extend j dimension"

      [TestMethod(), Description("Asserts that columns can be correctly added to the matrix.")]
      public void TestResizableMatrix_AddColumn_Count()
      {
         CTableMatrix<object> Matrix = new CTableMatrix<object>();
         Matrix.AddColumn("column1");
         Matrix.AddColumn("column2");

         Assert.AreEqual(2, Matrix.Columns.Count);
         Assert.AreEqual(0, Matrix.Rows.Count);
      }

      [TestMethod(), Description("Asserts that columns index can be retrieved correctly.")]
      public void TestResizableMatrix_IndexOfColumn_Properties()
      {
         CTableMatrix<object> Matrix = new CTableMatrix<object>();
         Matrix.AddColumn("column1");
         Matrix.AddColumn("column2");

         Assert.AreEqual(0, Matrix.IndexOf("column1"));
         Assert.AreEqual(1, Matrix.IndexOf("column2"));
         Assert.AreEqual(-1, Matrix.IndexOf("column3"));
      }

      [TestMethod(), Description("Asserts that a column can be correctly added to the matrix with the right properties.")]
      public void TestResizableMatrix_AddColumn_Properties()
      {
         CTableMatrix<object> Matrix = new CTableMatrix<object>();
         Matrix.AddColumn("column");

         Assert.AreEqual("column", Matrix.Columns.First().Name);
         Assert.AreEqual(0, Matrix.Columns.First().Ordinal);
      }

      [TestMethod(), Description("Asserts that a column can be correctly added to a non empty matrix.")]
      public void TestResizableMatrix_AddColumn_NonEmptyMatrix_Properties()
      {
         CTableMatrix<object> Matrix = new CTableMatrix<object>();
         Matrix.AddColumn("column1");
         Matrix.AddRow(1);
         //non empty
         Matrix.AddColumn("column2");

         Assert.AreEqual("column2", Matrix.Columns.Last().Name);
         Assert.AreEqual(1, Matrix.Columns.Last().Ordinal);
      }

      [TestMethod(), Description("Asserts that matrix cells are populated with the default values when columns are added.")]
      public void TestResizableMatrix_AddColumn_NonEmptyMatrix_DefaultValueTypeObject()
      {
         CTableMatrix<int> Matrix = new CTableMatrix<int>();
         Matrix.AddColumn("column1");
         Matrix.AddRow(1);
         //non empty
         Matrix.AddColumn("column2");
         Matrix.AddColumn("column3");

         Assert.AreEqual(new int(), Matrix[0, "column2"]);
         Assert.AreEqual(new int(), Matrix[0, "column3"]);
      }

      [TestMethod(), Description("Asserts that a column with the same name of an existing one cannot be added.")]
      [ExpectedException(typeof(ArgumentException),"Column name is already defined for this context.")]
      public void TestResizableMatrix_AddColumn_ExceptionSameName()
      {
         CTableMatrix<object> Matrix = new CTableMatrix<object>();
         Matrix.AddColumn("column");
         Matrix.AddColumn("column");
      }

      #endregion

   }
}
