using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BHTechTest.Domain.ShareKernel.Results
{
    /// <summary>
    /// Result class without type.
    /// </summary>
    public class Result
    {
        public ICollection<ResultMessage> Messages { get; private set; } = new Collection<ResultMessage>();

        public bool HasErrors { get => Has(ResultType.Error); }
        public bool Has(ResultType type) => Messages.Any(m => m.Type == type);


        #region Constructor & Factory Methods
        private Result() { }

        public static Result CreateDefault() => new();
        public static Result CreateError(Exception ex) => (new Result()).AddError(ex);
        #endregion

        #region Add
        public Result AddInfo(string text) => AddMessage(ResultType.Info, text);
        public Result AddWarning(string text) => AddMessage(ResultType.Warning, text);
        public Result AddError(string text) => AddMessage(ResultType.Error, text);
        public Result AddError(Exception ex)
        {
            if (ex == null)
                return AddError(new ArgumentNullException(nameof(ex)));

            if (ex.InnerException != null) AddError(ex.InnerException);

            return AddError(ex.Message);
        }

        private Result AddMessage(ResultType type, string text)
        {
            if (!Enum.IsDefined(typeof(ResultType), type)) AddError(new ArgumentOutOfRangeException(nameof(type)));
            if (string.IsNullOrWhiteSpace(text)) AddError(new ArgumentNullException(nameof(text)));
            if (HasErrors)
                return this;

            var createResult = ResultMessage.Create(type, text);
            AddTyped(createResult);
            if (HasErrors)
                return this;

            var resultMessage = createResult.Value;
            if(resultMessage!= null)
                Messages.Add(resultMessage);
            return this;
        }

        public Result Add(Result otherResult)
        {
            if (otherResult == null)
                return AddError(new ArgumentNullException(nameof(otherResult)));

            Messages = Messages.Union(otherResult.Messages).ToList();
            return this;
        }

        internal Result<T> AddTyped<T>(Result<T> tResult)
        {
            var value = default(T);

            if (tResult == null)
            {
                AddError(new ArgumentNullException(nameof(tResult)));
            }
            else
            {
                Messages = Messages.Union(tResult.Messages).ToList();
                value = tResult.Value;
            }

            return AddValue(value);
        }
        #endregion

        #region T
        public Result<T> AddValue<T>(T? value)
        {
            if (value == null)
                return AddError(new ArgumentNullException($"{nameof(value)} ({typeof(T).Name})")).ToDefaultValueResult<T>();

            if (HasErrors)
                return AddError(new Exception("Cannot add a value to a Result<T> with errors!")).ToDefaultValueResult<T>();

            return Result<T>.Create(this, value);
        }

        public Result<T> ToDefaultValueResult<T>() => Result<T>.CreateDefault(this);

        public static Result<T> CreateDefault<T>() => Result<T>.CreateDefault();

        public static Result<T> CreateError<T>(Exception ex) => Result<T>.CreateError(ex);
        #endregion

        public static Result operator +(Result leftResult, Result rigthResult) => leftResult.Add(rigthResult);
    }

    /// <summary>
    /// Clase Resultado con tipo. Mayormente para métodos asincronos que no permiten el uso de parámetros de salida.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Result<T>
    {
        private Result _result;

        public IEnumerable<ResultMessage> Messages { get => _result.Messages; }

        public bool HasErrors { get => Has(ResultType.Error); }
        public bool Has(ResultType type) => _result.Messages.Any(m => m.Type == type);

        public T? Value { get; private set; }

        #region Constructor & Factory Method
        private Result(Result result)
        {
            _result = result;
        }

        private Result(Result result, T value)
        {
            _result = result;
            Value = value;
        }

        internal static Result<T> CreateDefault()
        {
            var result = Result.CreateDefault(); 

            return new Result<T>(result);
        }

        internal static Result<T> CreateDefault(Result result)
        {
            if (result == null) result = Result.CreateError(new ArgumentNullException(nameof(result)));

            return new Result<T>(result);
        }

        internal static Result<T> Create(Result result, T value)
        {
            if (result == null) result = Result.CreateError(new ArgumentNullException(nameof(result)));

            return new Result<T>(result, value);
        }

        internal static Result<T> CreateError(Exception ex) => CreateDefault(Result.CreateError(ex));

        public Result<T> Add(Result tResult)
        {
            _result.Add(tResult);
            return this;
        }

        public Result<T> Add(Result<T> tResult)
        {
            _result.AddTyped(tResult);
            return this;
        }

        public Result<T> AddValue(T value)
        {
            _result.AddValue(value);
            return this;
        }
        #endregion

    }
}

