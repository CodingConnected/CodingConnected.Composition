using System;
using System.Text.RegularExpressions;
using CodingConnected.Composition.Annotations;
using CodingConnected.Composition.Example.Interfaces;

namespace CodingConnected.Composition.Example.NETFramework
{
    class MainComponent
    {
        [Import]
        public ICalculator Calculator { get; set; }

        public double ParseCommand(string command)
        {
            var commandData = Regex.Match(command, @"\s*(?<a>[0-9\.]+)(?<op>[^0-9]+)(?<b>[0-9\.]+)");
            if (!commandData.Success)
            {
                throw new InvalidOperationException("Invalid command");
            }
            else
            {
                var a = double.Parse(commandData.Groups["a"].Value);
                var b = double.Parse(commandData.Groups["b"].Value);
                var op = commandData.Groups["op"].Value;
                return Calculator.ExecuteCommand(a, b, op);
            }
        }
    }
}