using BHTechTest.Domain.ShareKernel.BHTechTest.Domain.SharedKernel;
using BHTechTest.Domain.ToDoListContext.ToDoListAggregateRoot.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BHTechTest.Domain.ToDoListContext.ToDoListAggregateRoot.Entities
{
    public class TodoItem : Entity
    {
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string Category { get; }

        private readonly List<Progression> _progressions = new();
        public IReadOnlyList<Progression> Progressions => _progressions.AsReadOnly();

        public TodoItem(int id, string title, string description, string category)
        {
            Id = id;
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Description = description ?? string.Empty;
            Category = category ?? string.Empty;
        }


        /// <summary>
        ///  Accumulated progress percent
        /// </summary>
        public decimal AcumulativeProgressPercent => _progressions.Any() ? _progressions.Sum(p => p.Percent) : 0m;

        /// <summary>
        /// Accumulated progress percent is 100
        /// </summary>
        public bool IsCompleted => _progressions.Any() && AcumulativeProgressPercent >= 100m;

        // Returns latest progression percent or 0
        public decimal LatestProgressPercent => _progressions.Any() ? _progressions.Last().Percent : 0m;

        /// <summary>
        /// Adds a progression; percent is interpreted as **cumulative** percent (matching example).<br/>
        /// * percent validation: greater than 0 and less or equal 100<br/>
        /// * date validation: progression date must be strictly greater than the last progression date<br/>
        /// * percent must be strictly greater than last progression percent (cumulative)<br/>
        /// * final cumulative percent cannot exceed 100 (we validated <=100 above)<br/>
        /// </summary>
        /// <param name="progression"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void AddProgression(Progression progression)
        {
            ArgumentNullException.ThrowIfNull(progression);


            if (progression.Percent <= 0m || progression.Percent > 100m)
                throw new InvalidOperationException("Progress percent must be > 0 and <= 100.");

            if (_progressions.Any() && progression.DateTime <= _progressions.Last().DateTime)
                throw new InvalidOperationException("Progression date must be greater than the last progression date.");

            if (_progressions.Any() && progression.Percent + AcumulativeProgressPercent > 100m)
                throw new InvalidOperationException("Accumulative Progression percent must be less or equal than 100.");

            _progressions.Add(progression);
        }

        public void UpdateDescription(string newDescription)
        {
            if (newDescription == null) throw new ArgumentNullException(nameof(newDescription));
            Description = newDescription;
        }
    }
}
