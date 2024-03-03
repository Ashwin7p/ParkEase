using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Assignment2
{
    class MultiCellBuffer
    {
        /*
         * Semaphores(read and write) to allow simultaneous access to buffer
         * With two standard methods to set and get value
         */

        // Semaphores to control read/write access
        Semaphore write = new Semaphore(3, 3);
        Semaphore read = new Semaphore(2, 2);

        // To get Read and write pointers in the buffer
        int head = 0;
        int tail = 0;
        int toatalElements = 0;

        object[] cells = new object[3];


        public void SetOneCell(OrderClass order)
        {// put an oredr to Multi-cell-buffer
            write.WaitOne();//get semaphore
            Console.WriteLine("THREAD: " + Thread.CurrentThread.Name + " Entered Write");
            lock (this)//lock object
            {

                do
                {
                    if (toatalElements == 3)
                    {
                        if (Program.DEBUG) Console.WriteLine("MONITOR: Write Waiting {0}", Thread.CurrentThread.Name);
                        Monitor.Wait(this);
                    }
                    else { break; }
                } while (true);

                cells[tail] = order;
                tail = (tail + 1) % 3;

                Console.WriteLine("WRITING: ({0}) Multi-Cell Buffer\n\n{1}\n, Elements: {2}\n",
                    Thread.CurrentThread.Name,
                    order,
                    toatalElements
                );

                toatalElements++;
                Console.WriteLine("THREAD: ({0}) Leaving Write", Thread.CurrentThread.Name);
                // Access for others
                write.Release();
                //notify others
                Monitor.Pulse(this);
            }
        }

        public OrderClass GetOneCell()
        {// get an oredr to Multi-cell - buffer
            read.WaitOne();//get semaphore
            Console.WriteLine("THREAD: " + Thread.CurrentThread.Name + " Entered Read");
            lock (this)//lock object
            {
                OrderClass order;
                do {
                    if(toatalElements == 0){
                        if (Program.DEBUG) Console.WriteLine("MONITOR: Read Waiting {0}", Thread.CurrentThread.Name);
                        //obtain only when size >0
                        Monitor.Wait(this);
                    }
                    else { break; }
                } while (true);

                order = (OrderClass)cells[head];

                head = (head + 1) % 3;
                toatalElements--;
                Console.WriteLine("READING: ({0}) Multi-Cell Buffer\n{1}\n, Elements: {2}\n",
                    Thread.CurrentThread.Name,
                    order,
                    toatalElements
                );

                Console.WriteLine("THREAD: ({0}) Leaving Read", Thread.CurrentThread.Name);
                //Access for others
                read.Release();
                //Notify others
                Monitor.Pulse(this);
                return order;
            }

        }
    }
}
