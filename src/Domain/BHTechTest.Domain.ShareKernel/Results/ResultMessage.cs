using System;

namespace BHTechTest.Domain.ShareKernel.Results
{
    /// <summary>
    /// Result Message;
    /// </summary>
    public class ResultMessage
    {
        public ResultType Type { get; private set; }
        public string Text { get; private set; }

        private ResultMessage(ResultType type, string text)
        {
            Type = type;
            Text = text;
        }

        private static ResultMessage CreateDefault() =>
            new ResultMessage(ResultType.None, string.Empty);

        public static Result<ResultMessage> Create(ResultType type, string text)
        {
            var result = Result<ResultMessage>.CreateDefault();
            var resultMessage = CreateDefault();

            var canCreateResult = CanCreate(type, text);
            result.Add(canCreateResult);
            if (canCreateResult.HasErrors)
                return result;

            resultMessage = new ResultMessage(type, text);
            return result;
        }

        private static Result CanCreate(ResultType type, string text)
        {
            var result = Result.CreateDefault();

            if (!Enum.IsDefined(typeof(ResultType), type)) result.AddError(new ArgumentNullException(nameof(type)));
            if (string.IsNullOrWhiteSpace(text)) result.AddError(new ArgumentNullException(nameof(text)));

            return result;
        }
    }
}
