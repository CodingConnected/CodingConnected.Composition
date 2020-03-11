using CodingConnected.Composition.Annotations;
using CodingConnected.Composition.Example.Interfaces;

namespace CodingConnected.Composition.Example.NetStandardLib
{
    [Export(typeof(ICalculateCommand))]
    public class MulCommand : ICalculateCommand
    {
        public string Operator => "*";
        public double Calculate(double a, double b)
        {
            return a * b;
        }
    }
}
