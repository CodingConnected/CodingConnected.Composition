using System;
using System.Text.RegularExpressions;
using CodingConnected.Composition.Annotations;
using CodingConnected.Composition.Example.Interfaces;

namespace CodingConnected.Composition.Example.NETFramework
{
    /// <summary>
    /// A very rudimentary implementation of a Polish notation calculator,
    /// able only to process one operator with two operands
    /// </summary>
    class ReversePolishNotationCalculationService
    {
        [Import]
        public ICalculator Calculator { get; set; }

        public double ProcessCommand(string command)
        {
            var specialCommandData = Regex.Match(command, @"\s*(?<op>[^0-9])\s+(?<a>[0-9\.]+)\s+(?<b>[0-9\.]+)");
            if (!specialCommandData.Success)
            {
                throw new InvalidOperationException("Incorrect command parameters for polish notation");
            }
            var a = double.Parse(specialCommandData.Groups["a"].Value);
            var b = double.Parse(specialCommandData.Groups["b"].Value);
            var op = specialCommandData.Groups["op"].Value;
            return Calculator.ExecuteCommand(a, b, op);
        }
    }
}