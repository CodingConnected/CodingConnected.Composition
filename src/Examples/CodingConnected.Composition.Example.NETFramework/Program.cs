using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CodingConnected.Composition.Example.NETFramework
{
    class Program
    {
        static void Main(string[] args)
        {
            Composer.LoadExports(Assembly.GetEntryAssembly());
            if (File.Exists("..\\..\\..\\CodingConnected.Composition.Example.NetStandardLib\\bin\\Debug\\netstandard2.0\\CodingConnected.Composition.Example.NetStandardLib.dll"))
            {
                Composer.LoadExports(Assembly.LoadFrom("..\\..\\..\\CodingConnected.Composition.Example.NetStandardLib\\bin\\Debug\\netstandard2.0\\CodingConnected.Composition.Example.NetStandardLib.dll"));
            }

            var main = new CalculationService();
            Composer.Compose(main);

            Console.WriteLine($"Loaded {((Calculator)main.Calculator).Commands.Count()} commands");
            Console.WriteLine("Enter command (eg. \"4+6\") followed by the enter key; enter \"exit\" to exit:");

            var command = "";
            while (command != "exit")
            {
                command = Console.ReadLine();
                if (command == "exit") break;
                try
                {
                    Console.WriteLine("Result: " + main.ParseCommand(command));
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occured: " + ex);
                }
            }
        }
    }
}
