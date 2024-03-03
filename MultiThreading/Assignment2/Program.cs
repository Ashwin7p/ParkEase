using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Assignment2
{
    internal class Program
    {
        /*
         Intialize and start Parking structure threads and Parking agent threads
         */
        public const bool DEBUG = true;
        // Number of parking structures
        private const int K = 1;
        // Number of parking agents
        private const int N = 1; 

        private static Thread[] parkingAgentThreads = new Thread[N];
        private static Thread[] parkingStructureThreads = new Thread[K];
        private static ParkingStructure[] parkingStructureDelegatees = new ParkingStructure[K];

        public static MultiCellBuffer mb = new MultiCellBuffer();

        static void Main(string[] args)
        {
            for (int i = 0; i < K; ++i)
            {
                ParkingStructure parkingStructure = new ParkingStructure();
                parkingStructureDelegatees[i] = parkingStructure;
                parkingStructureThreads[i] = new Thread(parkingStructure.Start);
                parkingStructureThreads[i].Name = "ParkingStructure-Thread-" + i;
                parkingStructureThreads[i].Start();
                while (!parkingStructureThreads[i].IsAlive) ;
            }

            for (int i = 0; i < N; ++i)
            {
                ParkingAgent parkingAgent = new ParkingAgent(K);

                // Loop through the Parking Structures and Subscribe to the Price Cut event
                for (int j = 0; j < K; ++j)
                {
                    parkingAgent.Subscribe(parkingStructureDelegatees[j],j);

                }

                parkingAgentThreads[i] = new Thread(parkingAgent.Start);
                parkingAgentThreads[i].Name = "ParkingAgent-Thread-" + i;
                parkingAgentThreads[i].Start();
                while (!parkingAgentThreads[i].IsAlive) ;
            }


            for (int i = 0; i < K; ++i)
            {
                while (parkingStructureThreads[i].IsAlive) ;
            }

            for (int i = 0; i < N; ++i)
            {
                //kill the agents
                ParkingAgent.ParkingStructuresActive = false;
            }

            for (int i = 0; i < N; ++i)
            {
                while (parkingAgentThreads[i].IsAlive) ;
            }

            Console.WriteLine("\n\nPROGRAM COMPLETED");
            Console.ReadLine();

        }

    }
}
            



