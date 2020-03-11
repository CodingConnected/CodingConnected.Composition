using CodingConnected.Composition.Annotations;
using CodingConnected.Composition.Example.Interfaces;

namespace CodingConnected.Composition.Example.NETFramework
{
    [Export(typeof(ICalculateCommand))]
    public class AddCommand : ICalculateCommand
    {
        public string Operator => "+";
        public double Calculate(double a, double b)
        {
            return a + b;
        }
    }
}