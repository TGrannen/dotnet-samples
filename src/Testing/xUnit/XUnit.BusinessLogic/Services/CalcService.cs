using System;
using XUnit.BusinessLogic.Interfaces;

namespace XUnit.BusinessLogic.Services;

public class CalcService : ICalculatorService
{
    public double PerformOperation(double num1, double num2, string op)
    {
        var result = double.NaN; // Default value is "not-a-number" which we use if an operation, such as division, could result in an error.

        // Use a switch statement to do the math.
        switch (op)
        {
            case "a":
                result = num1 + num2;
                break;

            case "s":
                result = num1 - num2; //  - 12
                break;

            case "m":
                result = num1 * num2; //  * 0.1
                break;

            case "d":
                // Ask the user to enter a non-zero divisor.
                if (Math.Abs(num2) > 0.0001)
                {
                    result = num1 / num2;
                }
                break;

            case "n":
                // Will this work??
                return -1;
            // Return text for an incorrect option entry.
            default:
                break;
        }
        return result;
    }
}