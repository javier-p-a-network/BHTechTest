using BHTechTest.Domain.ShareKernel.BHTechTest.Domain.SharedKernel;
using System;
using System.Collections.Generic;

namespace BHTechTest.Domain.ToDoListContext.ToDoListAggregateRoot.ValueObjects
{
    public class Progression : ValueObject
    {
        public DateTime DateTime { get; }
        public decimal Percent { get; }

        public Progression(DateTime dateTime, decimal percent)
        {
            DateTime = dateTime;
            Percent = percent;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return DateTime;
            yield return Percent;
        }
    }
}
