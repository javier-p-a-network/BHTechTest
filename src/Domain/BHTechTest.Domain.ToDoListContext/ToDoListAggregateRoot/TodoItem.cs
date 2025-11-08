using System;
using System.Collections.Generic;
using System.Linq;

namespace BHTechTest.Domain.ToDoListContext.ToDoListAggregateRoot
{
    public class TodoItem
    {
        public int Id { get; }
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

        // Interpreting IsCompleted as "latest progression percent == 100"
        public bool IsCompleted => _progressions.Any() && _progressions.Last().Percent >= 100m;

        // Returns latest progression percent or 0
        public decimal LatestProgressPercent => _progressions.Any() ? _progressions.Last().Percent : 0m;

        // Adds a progression; percent is interpreted as **cumulative** percent (matching example).
        public void AddProgression(Progression progression)
        {
            if (progression == null) throw new ArgumentNullException(nameof(progression));

            // percent validation: greater than 0 and less or equal 100
            if (progression.Percent <= 0m || progression.Percent > 100m)
                throw new InvalidOperationException("Progress percent must be > 0 and <= 100.");

            // date validation: progression date must be strictly greater than the last progression date
            if (_progressions.Any() && progression.DateTime <= _progressions.Last().DateTime)
                throw new InvalidOperationException("Progression date must be greater than the last progression date.");

            // percent must be strictly greater than last progression percent (cumulative)
            if (_progressions.Any() && progression.Percent <= _progressions.Last().Percent)
                throw new InvalidOperationException("Progression percent must be greater than the last progression percent.");

            // final cumulative percent cannot exceed 100 (we validated <=100 above)
            // add
            _progressions.Add(progression);
        }

        public void UpdateDescription(string newDescription)
        {
            if (newDescription == null) throw new ArgumentNullException(nameof(newDescription));
            Description = newDescription;
        }
    }
}
