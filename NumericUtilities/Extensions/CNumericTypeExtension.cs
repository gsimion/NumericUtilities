using System;
using System.Linq.Expressions;

namespace Numeric.Extensions
{
   /// <summary>
   /// Static class providing helpers methods for numeric operation within generic a generic type.
   /// </summary>
   public static class CNumericTypeExtension
   {
      /// <summary>
      /// Performs the addition operation.
      /// </summary>
      /// <typeparam name="T">The type.</typeparam>
      /// <param name="a">The first operand.</param>
      /// <param name="b">The second operand.</param>
      /// <returns>The result of the addition operation.</returns>
      public static T Add<T>(T a, T b)
      {
         var e = Expression.Add(GetConstant<T>(a), GetConstant<T>(b));
         return Expression.Lambda<Func<T>>(e).Compile()();
      }

      /// <summary>
      /// Performs the multiplication operation.
      /// </summary>
      /// <typeparam name="T">The type.</typeparam>
      /// <param name="a">The first multiplicand.</param>
      /// <param name="b">The second multiplicand.</param>
      /// <returns>The result of the multiplication operation.</returns>
      public static T Multiply<T>(T a, T b)
      {
         var e = Expression.Multiply(GetConstant<T>(a), GetConstant<T>(b));
         return Expression.Lambda<Func<T>>(e).Compile()();
      }

      /// <summary>
      /// Determines whether a type supporst addition operation.
      /// </summary>
      /// <typeparam name="T">The type.</typeparam>
      /// <returns><c>True</c> if the type supports addition, <c>false</c> otherwise.</returns>
      public static bool SupportsAddition<T>()
      {
         try
         {
            Expression.Add(GetConstant<T>(), GetConstant<T>());
            return true;
         }
         catch 
         {
            return false;
         }
      }

      /// <summary>
      /// Determines whether a type supporst multiplication operation.
      /// </summary>
      /// <typeparam name="T">The type.</typeparam>
      /// <returns><c>True</c> if the type supports multiplication, <c>false</c> otherwise.</returns>
      public static bool SupportsMultipication<T>()
      {
         try
         {
            Expression.Multiply(GetConstant<T>(), GetConstant<T>());
            return true;
         }
         catch
         {
            return false;
         }
      }

      /// <summary>
      /// Gets the constant expression of a type as its default value.
      /// </summary>
      /// <typeparam name="T">The type.</typeparam>
      /// <returns>The constant expression.</returns>
      public static ConstantExpression GetConstant<T>()
      {
         return Expression.Constant(default(T), typeof(T));
      }

      /// <summary>
      /// Gets the constant expression of a type for the passed value.
      /// </summary>
      /// <typeparam name="T">The type.</typeparam>
      /// <param name="k">The value to get the constant expression from.</param>
      /// <returns>The constant expression.</returns>
      public static ConstantExpression GetConstant<T>(T k)
      {
         return Expression.Constant(k, typeof(T));
      }

   }
}
