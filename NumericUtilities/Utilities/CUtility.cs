using System;
using System.Text;

namespace Numeric.Utilities
{
   /// <summary>
   /// Static library internal class with some utilities.
   /// </summary>
   internal static class CUtility
   {
      private const int Prime = 17;

      private const int AntherPrime = 23;

      /// <summary>
      /// Generates safe hascode from a sequence of objects.
      /// </summary>
      /// <param name="Args">Object sequence to generate hashcode from.</param>
      /// <returns>Hashcode.</returns>
      public static int GenerateHashCode(params Object[] Args)
      {
         long lHashCode = Prime;
         foreach (Object Thing in Args)
            lHashCode = (lHashCode * AntherPrime) + Thing.GetHashCode();
         return (int)(lHashCode & 0x7fffffffL);
      }

      /// <summary>
      /// Gets a string containg the parameters as '[type] [name]' of a method using reflection.
      /// </summary>
      /// <param name="Method">Method to get the parameters from.</param>
      /// <param name="iTake">Number specifying how many parameters should be returned in the result, should be greater than <c>0</c>.</param>
      /// <returns>String containing '[type] [name]' for as many parameters as specified in <paramref name="iTake">take</paramref> argument.</returns>
      public static string GetParameters(System.Reflection.MethodBase Method, int iTake)
      {
         StringBuilder sb = new StringBuilder();
         int iStopper = 0;
         foreach (System.Reflection.ParameterInfo parameter in Method.GetParameters())
         {
            sb.AppendFormat("{0} {1}",
               parameter.ParameterType,
               parameter.Name);
            iStopper++;
            if (iStopper >= iTake)
               break;
            sb.Append(' ');
         }
         return sb.ToString();
      }
   }
}
