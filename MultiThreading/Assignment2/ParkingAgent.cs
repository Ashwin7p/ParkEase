using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Assignment2
{
    class ParkingAgent
    {
        private string name;
        //private double currentPrice;
        //private double previousPrice;
        private ArrayList currentPrice = new ArrayList();//store the prices of different parking-structures to avoid overwriting
        private ArrayList previousPrice = new ArrayList();//store the prices of different parking-structures to avoid overwriting
        private static bool parkingStructuresActive = true;
        private bool baseOrder = true;
        private bool ispricecutEvent = false;
        int K;

        private string parkingStructureId;

        public ParkingAgent(int K)
        {
            this.K = K;
            for (int i = 0; i < K; i++)
            { //initialize curent and previous cost of parking structures
                currentPrice.Add(0);
                previousPrice.Add(0);
            }

        }
        public static bool ParkingStructuresActive
        {
            get { return parkingStructuresActive; }
            set { parkingStructuresActive = value; }
        }


        public int CalculateQuantityToOrder()
        { /*
             Determine the quantity to order based on the change done by price-cut-event
          */
            char lastchar = parkingStructureId[parkingStructureId.Length - 1];
            int i = (int)Char.GetNumericValue(lastchar);
            if ((double)previousPrice[i] - (double)currentPrice[i] >= 5)
            {
                //order 10
                return 10;

            }
            else
            {
                //order 5 default
                return 5;
            }
        }


        // ParkingAgent thread logic 
        public void Start()
        {


            while (parkingStructuresActive)
            {

                if (ispricecutEvent)// act as a price-cut-event handler
                {
                    CreatePriceCutEventOrder();

                }
                else if (baseOrder)
                {
                    if (!ispricecutEvent)
                    {
                        CreateBaseOrder();
                    }
                }
                else
                {
                    // No orders are needed so sleep the thread for some time
                    Console.WriteLine("WAITING: Parking Agent Thread ({0})", Thread.CurrentThread.Name);
                    Thread.Sleep(1000);
                    baseOrder = true;
                }

            }

            Console.WriteLine("CLOSING: Parking Agent Thread ({0})", Thread.CurrentThread.Name);

        }

        // Event handler for price-cut event
        public void PriceCutEventHandler(PriceCutEventArgs e)
        {
            /*
              Set the values received from the parking structures
             */
            char lastchar = e.Id[e.Id.Length - 1];
            int i = (int)Char.GetNumericValue(lastchar);
            ispricecutEvent = true;
            currentPrice[i] = e.CurrentPrice;
            previousPrice[i] = e.Previousprice;
            parkingStructureId = e.Id;
        }
        public void Subscribe(ParkingStructure parkingStructure, int index) //call this main class
        {
            Console.WriteLine("SUBSCRIBING: Price Cut Event");
            parkingStructure.PriceCut += PriceCutEventHandler;//add the handlers to be inviked by every parking structure
            currentPrice[index] = parkingStructure.CurrentPrice;
            previousPrice[index] = parkingStructure.PreviousPrice;
            // ispricecutEvent = false;
        }
        private void CreateBaseOrder()
        {
            var random = new Random();
            int check = random.Next(0, 5);
            if (check == 1)//create baseorder randomly decreasing the mcb filling up quickly and no space for price event orders
            {
                Console.WriteLine("CREATING: Base Order ({0})", Thread.CurrentThread.Name);

                int i = 0;
                if (parkingStructureId != null)
                {
                    char lastchar = parkingStructureId[parkingStructureId.Length - 1];
                    i = (int)Char.GetNumericValue(lastchar);
                }
                else
                {
                    Random rnd = new Random();
                    i = rnd.Next(0, K);
                    parkingStructureId = "ParkingStructure-Thread-" + i;
                }

                baseOrder = false;
                OrderClass order = new OrderClass(
                    senderId: Thread.CurrentThread.Name,
                    recieverId: parkingStructureId,
                    cardNo: random.Next(5000, 8000),
                    quantity: 1,
                    unitPrice: (double)previousPrice[i], //order of previous price itself, not updating with price cut price
                    eventType: "Base Order"

                );

                Program.mb.SetOneCell(order);
            }
            //baseOrder = false;

        }
        private void CreatePriceCutEventOrder()
        {
            var random = new Random();
            char lastchar = parkingStructureId[parkingStructureId.Length - 1];
            int i = (int)Char.GetNumericValue(lastchar);
            
            Console.WriteLine("CREATING: Price cut Event ({0})", Thread.CurrentThread.Name);


            int quantityToOrder = CalculateQuantityToOrder();

            // Create an order

            if (quantityToOrder == 10)//logic for not creating Price cut orders on every price-cut
            {
                OrderClass order = new OrderClass(
                  senderId: Thread.CurrentThread.Name,
                  recieverId: parkingStructureId,
                  cardNo: random.Next(5000, 7001),
                  quantity: quantityToOrder,
                  unitPrice: (double)currentPrice[i],
                  eventType: "Price Cut Order"
                );
                // Send the order to the MultiCellBuffer
                // parkingStructure.SendOrder(order);
                Program.mb.SetOneCell(order);

            }
            else
            {
                Console.WriteLine("SKIPPING: Order on price cut for " + parkingStructureId + " by " + Thread.CurrentThread.Name);
            }


            // baseOrder = false;
            Thread.Sleep(1000);
            ispricecutEvent = false;
            //}
        }
    }
}
