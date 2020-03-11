namespace CodingConnected.Composition.Example.Interfaces
{
    public interface ICalculateCommand
    {
        string Operator { get; }
        double Calculate(double a, double b);
    }
}