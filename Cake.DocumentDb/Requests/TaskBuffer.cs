using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cake.DocumentDb.Requests
{
    public class TaskBuffer
    {
        private readonly int count;
        private readonly List<Task> buffer;

        public TaskBuffer(int count)
        {
            this.count = count;
            buffer = new List<Task>(count);
        }

        public bool IsEmpty()
        {
            return buffer.Count == 0;
        }

        public bool IsFull()
        {
            return buffer.Count >= count;
        }

        public void Add(Task task)
        {
            if (IsFull())
                throw new InvalidOperationException("Buffer is full");

            buffer.Add(task);
        }

        public async Task ExecuteInParallel()
        {
            if (IsEmpty())
                return;

            await Task.WhenAll(buffer);
            buffer.Clear();
        }
    }
}