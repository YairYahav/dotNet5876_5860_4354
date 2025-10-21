using System;


namespace Targil0
{
    partial class Program // Chapter 9
    {
        static void Main(string[] arg)
        {
            Welcome5860();
            Welcome4354();

            Console.ReadKey();
        }

        // I'm using Google Translate to translate some of the comments, I hope there are no mistakes.
        // The file asks why the method is static, and I'll briefly explain that because main is static,
        // the methods that call it must also be static, otherwise we'll have to create an instance of the class.
        private static void Welcome5860()
        {
            Console.WriteLine("Start program...");
            Console.WriteLine("{0}", "");
            Console.Write("Enter your name: "); // I didn't write "Line" in the command, so it won't go down a line.
            string name = Console.ReadLine();
            Console.WriteLine("Hi {0}! Welcome to my first application", name);
        }

        static partial void Welcome4354();
    }
}

