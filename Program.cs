using System;

namespace YelpJSON {


    class Program {

        static void Main() {
            User.Parse();
            Business.Parse();
            Tip.Parse();
            Checkin.Parse();

            Console.WriteLine($"{DateTime.Now} : * Import complete *");
            Console.Write("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
