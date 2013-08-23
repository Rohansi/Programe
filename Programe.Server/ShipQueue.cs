using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Programe.Server
{
    public class ShipQueue
    {
        private int maxCount;
        private LinkedList<Ship> queue;
        
        public int Count
        {
            get
            {
                lock (queue)
                    return queue.Count;
            }
        } 

        public ShipQueue(int maxCount)
        {
            this.maxCount = maxCount;
            queue = new LinkedList<Ship>();
        }

        public bool Enqueue(Ship ship, out string message)
        {
            lock (queue)
            {
                if (queue.Count >= maxCount)
                {
                    message = "The spawn queue is full.";
                    return false;
                }

                var removeList = queue.Where(i => i.Name == ship.Name).ToList();
                foreach (var remove in removeList)
                {
                    queue.Remove(remove);
                }

                queue.AddLast(ship);

                message = null;
                return true;
            }
        }

        public Ship Dequeue()
        {
            lock (queue)
            {
                if (Count == 0)
                    return null;

                var value = queue.First.Value;
                queue.RemoveFirst();
                return value;
            }
        }
    }
}
