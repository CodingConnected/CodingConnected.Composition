using System;
using System.Text.RegularExpressions;
using CodingConnected.Composition.Annotations;
using CodingConnected.Composition.Example.Interfaces;

namespace CodingConnected.Composition.Example.NETFramework
{
    class PolishNotationComponent
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

    class MainComponent
    {
        [Import]
        public ICalculator Calculator { get; set; }

        public PolishNotationComponent MyPolishNotationComponent { get; set; }

        public double ParseCommand(string command)
        {
            var commandData = Regex.Match(command, @"\s*(?<a>[0-9\.]+)(?<op>[^0-9\s]+)(?<b>[0-9\.]+)");
            if (!commandData.Success)
            {
                try
                {
                    return MyPolishNotationComponent.ProcessCommand(command);
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

        public MainComponent()
        {
            MyPolishNotationComponent = new PolishNotationComponent();
        }
    }
}