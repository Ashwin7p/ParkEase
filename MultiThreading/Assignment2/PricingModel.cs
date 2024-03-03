using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment2
{
    static class PricingModel
    {   /*
         Static class to return random values for a parking structure
         */
        private static Random random = new Random();

        // return a random price from 10 to 40
        public static double CalculatePrice()
        {
            return random.Next(10, 41);
        }
    }
}
