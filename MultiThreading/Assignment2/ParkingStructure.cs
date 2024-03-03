using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Assignment2
{
    public class PriceCutEventArgs : EventArgs
    {
        /*
         The class serves as Price cut notifying middleware, where the parameters are sent to the respective price-cut-handlers
         
         */
        private double previousprice;
        private double currentPrice;
        private string id;

        public PriceCutEventArgs(string id, double previousprice, double currentPrice)
        {
            this.Id = id;
            this.previousprice = previousprice;
            this.currentPrice = currentPrice;
        }

        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        public double Previousprice
        {
            get { return previousprice; }
            private set { previousprice = value; }
        }
        public double CurrentPrice
        {
            get { return currentPrice; }
            private set { currentPrice = value; }
        }
    }


    // Define the ParkingStructure class
    class ParkingStructure
    {
        //private PricingModel pricingModel;
        private int priceCutCount = 1;
        private double currentPrice = 0.0; // Current Unit Price for the rooms
        private double previousPrice = 0.0; // Previous Unit Price for the rooms

        private ArrayList processingThreads = new ArrayList();

        /*Create the delegatee function for the parking agents to add their respective pric-cut-handlers*/
        public delegate void PriceCutHandler(PriceCutEventArgs e);
        public event PriceCutHandler PriceCut;


        public double CurrentPrice { get; private set; }
        public double PreviousPrice { get; private set; }

        private void PriceCutEvent()
        {
            if (PriceCut != null)
            {
                Console.WriteLine("\nEVENT: Performing Price Cut Event (#{0}) ({1})\n", priceCutCount, Thread.CurrentThread.Name);
                priceCutCount++;
                PriceCut(new PriceCutEventArgs(Thread.CurrentThread.Name, previousPrice, currentPrice));//pass pricing
            }
            else
            {
                Console.WriteLine("ERROR: No PriceCut event subscribers");
            }
        }
        private void ProcessOrders(OrderClass order)
        {
            // Retrieve orders from the MultiCellBuffer and start OrderProcessing threads
            // check if the receiver is intended or just skip the order(ignore)
            if (order.RecieverId == Thread.CurrentThread.Name || order.RecieverId == null)
            {
                // Process the order based on the current price
                Console.WriteLine("RECEIVING: Order for Parking Structure ({0})", Thread.CurrentThread.Name);
                OrderProcessing orderProcessor = new OrderProcessing(order);
                Thread processingThread = new Thread(new ThreadStart(orderProcessor.ProcessOrder));
                processingThreads.Add(processingThread);
                processingThread.Name = "Processor_" + Thread.CurrentThread.Name;
                processingThread.Start();

            }
            else
            {
                // Console.WriteLine("SKIPPING: Order not for Parking Structure ({0})," + "ORDER\\n\\t{ID: \" + order.SenderId\r\n                + \"}\\n\\t{RECIEVER_ID: \" + order.recieverId\r\n                + \"}\\n\\t{CARD_NO: \" + order.CardNo\r\n                + \"}\\n\\t{Quantity: \" + order.Quantity\r\n                + \"}\\n\\t{Event Type Order: \" + order.EventType\r\n                + \"}\\n\\t{UnitPrice: \" + order.UnitPrice + \"}\"", Thread.CurrentThread.Name);
                Console.WriteLine("SKIPPING: Order not for Parking Structure - "+ Thread.CurrentThread.Name + " \nORDER\n\t{ID: " + order.SenderId
                + "}\n\t{RECIEVER_ID: " + order.RecieverId
                + "}\n\t{CARD_NO: " + order.CardNo
                + "}\n\t{Quantity: " + order.Quantity
                + "}\n\t{Event Type Order: " + order.EventType
                + "}\n\t{UnitPrice: " + order.UnitPrice + "}");
            }
        }

        // Method to start the ParkingStructure thread
        public void Start()
        {
            while (priceCutCount <= 15) // Terminate after 15 price cuts
            {
                if (Program.DEBUG) Console.WriteLine("PRICING: ({0}) Starting Calculation", Thread.CurrentThread.Name);
                previousPrice = currentPrice;
                currentPrice = PricingModel.CalculatePrice();

                if (Program.DEBUG)
                {
                    Console.WriteLine("PRICING: ({0}) Price Finalized ({1})",
                        Thread.CurrentThread.Name,
                        currentPrice.ToString("C"));
                }

                if (Program.DEBUG)
                {
                    Console.WriteLine("CHECKING: ({0}) Price Comparison ({1} to {2})",
                        Thread.CurrentThread.Name,
                        previousPrice.ToString("C"),
                        currentPrice.ToString("C")
                    );
                }

                // Check for price cut and emit event
                if (currentPrice < previousPrice)
                {
                    PriceCutEvent();
                    // Thread.Sleep(500);
                }

                previousPrice = currentPrice;
                //Thread.Sleep(100);
                ProcessOrders(Program.mb.GetOneCell());//process the orders in the MCB 

                // Simulate some delay
                //Thread.Sleep(1000); // Adjust the delay as needed

            }

            foreach (Thread item in processingThreads)
            {//wait for processing threads to complete
                while (item.IsAlive) ;
            }

            Console.WriteLine("CLOSING: Parking structure Thread ({0})", Thread.CurrentThread.Name);
        }


    }
}
