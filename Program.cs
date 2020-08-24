using System;

namespace YelpJSON {


    class Program {

        static void Main() {
            UserParser.Parse();
            BusinessParser.Parse();
            TipParser.Parse();
            CheckinParser.Parse();

            Console.WriteLine($"{DateTime.Now} : * Import complete *");
            Console.Write("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
