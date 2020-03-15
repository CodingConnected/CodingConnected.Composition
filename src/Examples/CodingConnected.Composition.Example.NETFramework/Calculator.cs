using System;
using System.Collections.Generic;
using System.Linq;
using CodingConnected.Composition.Annotations;
using CodingConnected.Composition.Example.Interfaces;

namespace CodingConnected.Composition.Example.NETFramework
{
    /// <summary>
    /// A rudimentary calculator, able to execute a single command
    /// with a single operator and two operands.
    /// This class implements the ICalculator interface, and is 
    /// exported as such, to be used in composition.
    /// </summary>
    [Export(typeof(ICalculator))]
    public class Calculator : ICalculator
    {
        [ImportMany(typeof(ICalculateCommand))]
        public IEnumerable<ICalculateCommand> Commands { get; set; }

        public double ExecuteCommand(double a, double b, string op)
        {
            var c = Commands.FirstOrDefault(x => x.Operator == op);
            if (c == null) throw new InvalidOperationException("No such command!");
            return c.Calculate(a, b);
        }
    }
}
