using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numeric.Vectors;

namespace Numeric.Test
{
   [TestClass]
   public class CResizableMatrixTest
   {
      #region "test: constructor"

      [TestMethod(),  Description("Asserts that the default constructor returns an empty matrix with the right properties.")]
      public void TestResizableMatrix_DefaultConstructor()
      {
         CResizableMatrix<object> matrix = new CResizableMatrix<object>();

         Assert.IsNotNull(matrix);
         Assert.AreEqual(0, matrix.Rows.Count);
      }

      [TestMethod(), Description("Asserts that the constructor with size returns a matrix with the right properties.")]
      public void TestResizableMatrix_SizeConstructor()
      {
         CResizableMatrix<object> matrix = new CResizableMatrix<object>(2, 1);

         Assert.IsNotNull(matrix);
         Assert.AreEqual(2, matrix.Rows.Count);
         Assert.AreEqual(1, matrix.Rows[0].Count);
         Assert.AreEqual(1, matrix.Rows[1].Count);
      }

      [TestMethod(), Description("Asserts that the constructor with size returns a matrix with the default values for a reference type.")]
      public void TestResizableMatrix_SizeConstructor_DefaultValue_ReferenceType()
      {
         CResizableMatrix<object> matrix = new CResizableMatrix<object>(2, 2);

         Assert.AreEqual(default(object), matrix.Rows[0][0]);
         Assert.AreEqual(default(object), matrix.Rows[1][1]);
      }

      [TestMethod(), Description("Asserts that the constructor with size returns a matrix with the default values for a value type.")]
      public void TestResizableMatrix_SizeConstructor_DefaultValue_ValueType()
      {
         CResizableMatrix<int> matrix = new CResizableMatrix<int>(2, 2);

         Assert.AreEqual(default(int), matrix.Rows[0][0]);
         Assert.AreEqual(default(int), matrix.Rows[1][1]);
      }

      #endregion

      #region "tests: extend i dimension"

      [TestMethod(), Description("Asserts that it is possible to extend the i dimension of an existing matrix correctly.")]
      public void TestResizableMatrix_ExtendRows_Count()
      {
         CResizableMatrix<int> matrix = new CResizableMatrix<int>(2, 2);
         matrix.ExtendRows();
         matrix.ExtendRows();

         Assert.AreEqual(4, matrix.Rows.Count);
      }

      [TestMethod(), Description("Asserts that extending the i dimension of an existing matrix is populating the new row correctly.")]
      public void TestResizableMatrix_ExtendRows_Values()
      {
         CResizableMatrix<int> matrix = new CResizableMatrix<int>(2, 2);
         matrix.ExtendRows();

         Assert.AreEqual(default(int), matrix.Rows[2][0]);
         Assert.AreEqual(default(int), matrix.Rows[2][1]);
      }

      #endregion

      #region "tests: add row"

      [TestMethod(), Description("Asserts that matrix updates its properties when rows are added.")]
      public void TestResizableMatrix_AddRow_Properties()
      {
         CResizableMatrix<object> matrix = new CResizableMatrix<object>(0, 2);
         matrix.AddRow(1, "2");
         matrix.AddRow("2", null, "3");

         Assert.AreEqual(2, matrix.Rows.Count);
      }

      [TestMethod(), Description("Asserts that matrix add row return the index of the newly added rows.")]
      public void TestResizableMatrix_AddRow_Index()
      {
         CResizableMatrix<object> matrix = new CResizableMatrix<object>(0, 2);

         Assert.AreEqual(0, matrix.AddRow(1, "2"));
         Assert.AreEqual(1, matrix.AddRow());
         Assert.AreEqual(2, matrix.AddRow(1));
      }

      [TestMethod(), Description("Asserts that matrix add rows return the index of the newly added rows for a non empty matrix.")]
      public void TestResizableMatrix_AddRow_NonEmptyInit_Index()
      {
         CResizableMatrix<object> matrix = new CResizableMatrix<object>(2, 1);

         Assert.AreEqual(2, matrix.AddRow(1, "2"));
      }

      [TestMethod(), Description("Asserts that matrix cells are populated with the right values when row is added.")]
      public void TestResizableMatrix_AddRow_CorrectValues()
      {
         CResizableMatrix<object> matrix = new CResizableMatrix<object>(0, 2);
         matrix.AddRow(1, "2");

         Assert.AreEqual(1, matrix[0, 0]);
         Assert.AreEqual("2", matrix[0, 1]);
      }

      [TestMethod(), Description("Asserts that matrix cells are populated with the default values when row is added.")]
      public void TestResizableMatrix_AddRow_FillAllRowCells()
      {
         CResizableMatrix<object> matrix = new CResizableMatrix<object>(0, 2);
         matrix.AddRow(1);

         Assert.AreEqual(1, matrix[0, 0]);
         Assert.AreEqual(default(object), matrix[0, 1]);
      }

      #endregion

      #region "tests: remove row"

      [TestMethod(), Description("Asserts that matrix updates its properties when row is removed.")]
      public void TestResizableMatrix_RemoveRow_LastRow_Properties()
      {
         CResizableMatrix<object> matrix = new CResizableMatrix<object>(2, 2);
         int index = matrix.AddRow();
         matrix.RemoveAt(index);

         Assert.AreEqual(2, matrix.Rows.Count);
      }

      [TestMethod(), Description("Asserts that matrix updates its values when first rows are removed.")]
      public void TestResizableMatrix_RemoveRow_FirstRows_Values()
      {
         CResizableMatrix<object> matrix = new CResizableMatrix<object>(1, 2);
         matrix.AddRow("value", 1);
         matrix.AddRow(1, "value");
         matrix.RemoveAt(0);

         Assert.AreEqual("value", matrix[0, 0]);
         Assert.AreEqual("value", matrix[1, 1]);
         Assert.AreEqual(1, matrix[0, 1]);
         Assert.AreEqual(1, matrix[1, 0]);
      }

      [TestMethod(), Description("Asserts that matrix updates its properties when all rows are removed.")]
      public void TestResizableMatrix_RemoveRow_AllRows_Properties()
      {
         CResizableMatrix<object> matrix = new CResizableMatrix<object>(3, 2);
         matrix.RemoveAt(0);
         matrix.RemoveAt(0);
         matrix.RemoveAt(0);

         Assert.AreEqual(0, matrix.Rows.Count);
      }

      #endregion

      #region "tests: extend j dimension"

      [TestMethod(), Description("Asserts that columns can be correctly added to the matrix.")]
      public void TestResizableMatrix_AddColumn_Count()
      {
         CResizableMatrix<object> matrix = new CResizableMatrix<object>(1, 0);
         matrix.ExtendColumns();
         matrix.ExtendColumns();

         Assert.AreEqual(2, matrix.Rows[0].Count);
      }

      [TestMethod(), Description("Asserts that adding columns retrieve the right column index.")]
      public void TestResizableMatrix_AddColumn_Index()
      {
         CResizableMatrix<object> matrix = new CResizableMatrix<object>(0, 2);
         
         Assert.AreEqual(2, matrix.ExtendColumns());
         Assert.AreEqual(3, matrix.ExtendColumns());
      }

      [TestMethod(), Description("Asserts that matrix cells are populated with the default values when columns are added.")]
      public void TestResizableMatrix_AddColumn_NonEmptyMatrix_ReferenceType()
      {
         CResizableMatrix<object> matrix = new CResizableMatrix<object>();
         matrix.ExtendColumns();
         matrix.AddRow(1);
         //non empty
         matrix.ExtendColumns();
         matrix.ExtendColumns();

         Assert.AreEqual(1, matrix[0, 0]);
         Assert.AreEqual(default(object), matrix[0, 1]);
         Assert.AreEqual(default(object), matrix[0, 2]);
      }

      [TestMethod(), Description("Asserts that matrix cells are populated with the default values when columns are added.")]
      public void TestResizableMatrix_AddColumn_NonEmptyMatrix_ValueType()
      {
         CResizableMatrix<int> matrix = new CResizableMatrix<int>(0, 1);
         matrix.AddRow(1);
         matrix.ExtendColumns();

         Assert.AreEqual(new int(), matrix[0, 1]);
      }

      #endregion

      #region "tests: other"

      [TestMethod(), Description("Asserts that is default method returns the correct result.")]
      public void TestResizableMatrix_IsDefault_ValueType()
      {
         CResizableMatrix<int> matrix = new CResizableMatrix<int>(1, 5);
         matrix.AddRow(1, 2);
         matrix.AddRow();
         matrix.AddRow(0,0,0,0,0);
         matrix.AddRow(default(int), default(int), default(int), default(int), -1);

         Assert.IsTrue(matrix.IsDefault(0));
         Assert.IsFalse(matrix.IsDefault(1));
         Assert.IsTrue(matrix.IsDefault(2));
         Assert.IsTrue(matrix.IsDefault(3));
         Assert.IsFalse(matrix.IsDefault(4));
      }

      [TestMethod(), Description("Asserts that is default method returns the correct result.")]
      public void TestResizableMatrix_IsDefault_ReferenceType()
      {
         CResizableMatrix<object> matrix = new CResizableMatrix<object>(1, 5);
         matrix.AddRow(1, "");
         matrix.AddRow();
         matrix.AddRow(default(object), default(object), default(object), default(object), default(object));

         Assert.IsTrue(matrix.IsDefault(0));
         Assert.IsFalse(matrix.IsDefault(1));
         Assert.IsTrue(matrix.IsDefault(2));
         Assert.IsTrue(matrix.IsDefault(3));
      }

      [TestMethod(), Description("Asserts that same resizable matrices are equal.")]
      public void TestResizableMatrix_Equals_Reference()
      {
         CResizableMatrix<object> matrix = new CResizableMatrix<object>(1, 5);
         matrix.AddRow(1, "");
         CResizableMatrix<object> second = matrix;

         Assert.IsTrue(matrix.Equals(second));
      }

      [TestMethod(), Description("Asserts that different resizable matrices are equal if containing reference type.")]
      public void TestResizableMatrix_Equals_DifferentInstance_ReferenceType()
      {
         CResizableMatrix<object> matrix = new CResizableMatrix<object>(1, 5);
         matrix.AddRow(1, "");
         CResizableMatrix<object> second = new CResizableMatrix<object>(1, 5); ;
         second.AddRow(1, "");

         Assert.IsTrue(matrix.Equals(second));
      }

      [TestMethod(), Description("Asserts that different resizable matrices are not equal if different size.")]
      public void TestResizableMatrix_Equals_DifferentInstance_DifferentSize_ReferenceType()
      {
         CResizableMatrix<object> matrix = new CResizableMatrix<object>(1, 5);
         CResizableMatrix<object> second = new CResizableMatrix<object>(matrix.Rows.Count, matrix.Rows[0].Count + 1);
         CResizableMatrix<object> third = new CResizableMatrix<object>(matrix.Rows.Count + 1, matrix.Rows[0].Count);

         Assert.IsFalse(matrix.Equals(second));
         Assert.IsFalse(matrix.Equals(third));
      }

      [TestMethod(), Description("Asserts that different resizable matrices equal if same value type content.")]
      public void TestResizableMatrix_Equals_ValueType()
      {
         CResizableMatrix<int> matrix = new CResizableMatrix<int>(1, 2);
         matrix.AddRow(1, 2);
         CResizableMatrix<int> second = matrix.Copy();

         Assert.IsTrue(matrix.Equals(second));
      }

      [TestMethod(), Description("Asserts that it is possible to perform a simple transformation on a value type matrix.")]
      public void TestResizableMatrix_TransformSimple_ValueType()
      {
         CResizableMatrix<int> matrix = new CResizableMatrix<int>(0, 3);
         matrix.AddRow(1, 2, 3);
         matrix.AddRow(4, 5, 6);
         
         matrix.Transform((value) => 2 * value);

         Assert.AreEqual(2, matrix[0, 0]);
         Assert.AreEqual(4, matrix[0, 1]);
         Assert.AreEqual(6, matrix[0, 2]);
         Assert.AreEqual(8, matrix[1, 0]);
         Assert.AreEqual(10, matrix[1, 1]);
         Assert.AreEqual(12, matrix[1, 2]);
      }

      [TestMethod(), Description("Asserts that it is possible to perform a complex transformation on a value type matrix.")]
      public void TestResizableMatrix_TransformComplex_ValueType()
      {
         CResizableMatrix<int> matrix = new CResizableMatrix<int>(0, 3);
         matrix.AddRow(1, 2, 3);
         matrix.AddRow(4, 5, 6);

         matrix.Transform((i, j, value) => i == 1 ? 2 * value : 0);

         Assert.AreEqual(0, matrix[0, 0]);
         Assert.AreEqual(0, matrix[0, 1]);
         Assert.AreEqual(0, matrix[0, 2]);
         Assert.AreEqual(8, matrix[1, 0]);
         Assert.AreEqual(10, matrix[1, 1]);
         Assert.AreEqual(12, matrix[1, 2]);
      }

      [TestMethod(), Description("Asserts that it is possible to perform addition operator on value type when '+' is defined.")]
      public void TestResizableMatrix_Add_ValueType()
      {
         CResizableMatrix<decimal> a = new CResizableMatrix<decimal>(0, 3);
         a.AddRow(1m, 2m, 3m);
         a.AddRow(4m, 5m, 6m);

         CResizableMatrix<decimal> result = a + a.Copy();

         Assert.AreEqual(2m, result[0, 0]);
         Assert.AreEqual(4m, result[0, 1]);
         Assert.AreEqual(6m, result[0, 2]);
         Assert.AreEqual(8m, result[1, 0]);
         Assert.AreEqual(10m, result[1, 1]);
         Assert.AreEqual(12m, result[1, 2]);
      }

      [TestMethod(), Description("Asserts that it is not possible to perform addition operator on reference type when '+' is not defined.")]
      [ExpectedException(typeof(InvalidOperationException), "Addition operation cannot be applied.")]
      public void TestResizableMatrix_Add_ReferenceType_TrowsException()
      {
         CResizableMatrix<object> a = new CResizableMatrix<object>(0, 3);
         a.AddRow(1, 2, 3);
         a.AddRow(4, 5, 6);

         CResizableMatrix<object> result = a + a.Copy();
      }

      [TestMethod(), Description("Asserts that the copied matrix has the correct properties.")]
      public void TestResizableMatrix_Copy_NonEmptyMatrix_ValueType_Properties()
      {
         CResizableMatrix<int> matrix = new CResizableMatrix<int>(0, 2);
         matrix.AddRow(1, 2);
         matrix.AddRow(3, 4);

         CResizableMatrix<int> copy = matrix.Copy();

         Assert.AreNotSame(copy, matrix);
         Assert.AreEqual(matrix.Rows.Count, copy.Rows.Count);
      }

      [TestMethod(), Description("Asserts that the copied matrix has the correct values.")]
      public void TestResizableMatrix_Copy_NonEmptyMatrix_ValueType_Values()
      {
         CResizableMatrix<int> matrix = new CResizableMatrix<int>(0, 2);
         matrix.AddRow(1, 2);
         matrix.AddRow(3, 4);

         CResizableMatrix<int> copy = matrix.Copy();

         Assert.AreEqual(matrix.Rows[0][0], copy.Rows[0][0]);
         Assert.AreEqual(matrix.Rows[0][1], copy.Rows[0][1]);
         Assert.AreEqual(matrix.Rows[1][0], copy.Rows[1][0]);
         Assert.AreEqual(matrix.Rows[1][1], copy.Rows[1][1]);
      }

      [TestMethod(), Description("Asserts that the copied matrix has the correct values.")]
      public void TestResizableMatrix_Copy_NonEmptyMatrix_ReferenceType_Values()
      {
         CResizableMatrix<object> matrix = new CResizableMatrix<object>(0, 2);
         matrix.AddRow(1, null);
         matrix.AddRow("something");

         CResizableMatrix<object> copy = matrix.Copy();

         Assert.AreEqual(matrix.Rows[0][0], copy.Rows[0][0]);
         Assert.AreEqual(matrix.Rows[0][1], copy.Rows[0][1]);
         Assert.AreEqual(matrix.Rows[1][0], copy.Rows[1][0]);
         Assert.AreEqual(matrix.Rows[1][1], copy.Rows[1][1]);
      }

      [TestMethod(), Description("Asserts that matrix trasposed has the correct properties.")]
      public void TestResizableMatrix_Traspose_Properties()
      {
         CResizableMatrix<int> matrix = new CResizableMatrix<int>(0, 3);
         matrix.AddRow(1, 2, 3);
         matrix.AddRow(4, 5, 6);

         matrix.Traspose();

         Assert.AreEqual(3, matrix.Rows.Count);
         Assert.AreEqual(2, matrix.Rows[0].Count);
         Assert.AreEqual(2, matrix.Rows[1].Count);
      }

      [TestMethod(), Description("Asserts that matrix trasposed has the correct values.")]
      public void TestResizableMatrix_Traspose_Values()
      {
         CResizableMatrix<object> matrix = new CResizableMatrix<object>(0, 3);
         matrix.AddRow(1, null);
         matrix.AddRow("something", "else");

         matrix.Traspose();

         Assert.AreEqual(1, matrix.Rows[0][0]);
         Assert.AreEqual(null, matrix.Rows[1][0]);
         Assert.AreEqual(null, matrix.Rows[2][0]);
         Assert.AreEqual("something", matrix.Rows[0][1]);
         Assert.AreEqual("else", matrix.Rows[1][1]);
         Assert.AreEqual(null, matrix.Rows[2][1]);
      }

      #endregion
   }
}
