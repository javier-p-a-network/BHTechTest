using System;

namespace BHTechTest.Domain.ToDoListContext.ToDoListAggregateRoot
{
    public class Progression
    {
        public DateTime DateTime { get; }
        public decimal Percent { get; }

        public Progression(DateTime dateTime, decimal percent)
        {
            DateTime = dateTime;
            Percent = percent;
        }
    }
}
