using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment2
{  /*
    * To place and process orders by parking structures and parking agents
    */
    class OrderClass
    {
        private string senderId;
        private string recieverId;// for approriate reciever processing and validation
        private int cardNo;
        private int quantity;
        private double unitPrice;
        private string eventType;

        public OrderClass(string senderId, string recieverId, int cardNo, int quantity, double unitPrice, string eventType)
        {
            this.senderId = senderId;
            this.recieverId = recieverId;
            this.cardNo = cardNo;
            this.quantity = quantity;
            this.unitPrice = unitPrice;
            this.eventType   = eventType;
        }

        public override string ToString()
        {
            return "ORDER\n\t{ID: " + SenderId
                + "}\n\t{RECIEVER_ID: " + RecieverId
                + "}\n\t{CARD_NO: " + CardNo
                + "}\n\t{Quantity: " + Quantity
                + "}\n\t{Event Type Order: " + EventType
                + "}\n\t{UnitPrice: " + UnitPrice + "}";
        }

        public string EventType
        {
            get
            {
                return eventType;
            }
            set
            {
                eventType = value;
            }
        }

        public string SenderId
        {
            get
            {
                return senderId;
            }
            set
            {
                senderId = value;
            }
        }

        public string RecieverId
        {
            get
            {
                return recieverId;
            }
            set
            {
                recieverId = value;
            }
        }

        public int CardNo
        {
            get
            {
                return cardNo;
            }
            set
            {
                cardNo = value;
            }
        }

        public int Quantity
        {
            get
            {
                return quantity;
            }
            set
            {
                quantity = value;
            }
        }

        public double UnitPrice
        {
            get
            {
                return unitPrice;
            }
            set
            {
                unitPrice = value;
            }
        }
    }
}
