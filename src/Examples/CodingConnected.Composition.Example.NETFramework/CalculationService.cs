using System;
using System.Text.RegularExpressions;
using CodingConnected.Composition.Annotations;
using CodingConnected.Composition.Example.Interfaces;

namespace CodingConnected.Composition.Example.NETFramework
{
    /// <summary>
    /// A simple service able to process calculation commands with
    /// two operands and a single operator in between.
    /// If processing a command fails, the class will try processing
    /// the command with its personal assistent, the RPN service.
    /// Note that the RPN service internally uses the same instance
    /// of ICalculator, provided through composition.
    /// </summary>
    class CalculationService
    {
        /// <summary>
        /// This property will be set through composition
        /// </summary>
        [Import]
        public ICalculator Calculator { get; set; }

        /// <summary>
        /// The RPN service has its own ICalculator property, which, through
        /// composition, is set to the same instance as the Calculator
        /// property of this class.
        /// </summary>
        public ReversePolishNotationCalculationService RPNCalculationService { get; set; }

        public double ParseCommand(string command)
        {
            var commandData = Regex.Match(command, @"\s*(?<a>[0-9\.]+)(?<op>[^0-9\s]+)(?<b>[0-9\.]+)");
            if (!commandData.Success)
            {
                try
                {
                    return RPNCalculationService.ProcessCommand(command);
                }
                catch
                {
                    throw new InvalidOperationException("Incorrect command parameters");
                }
            }
            else
            {
                var a = double.Parse(commandData.Groups["a"].Value);
                var b = double.Parse(commandData.Groups["b"].Value);
                var op = commandData.Groups["op"].Value;
                return Calculator.ExecuteCommand(a, b, op);
            }
        }

        public CalculationService()
        {
            RPNCalculationService = new ReversePolishNotationCalculationService();
        }
    }
}