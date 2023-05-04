using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;

namespace Application.Core
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (!_validators.Any())
            {
                return await next();
            }

            var context = new ValidationContext<TRequest>(request);

            var errorsDictionary = _validators
                .Select(validator => validator.Validate(context))
                .SelectMany(validatorResult => validatorResult.Errors)
                .Where(validatorFailure => validatorFailure != null)
                .GroupBy(
                    validatorFailure => validatorFailure.PropertyName,
                    validatorFailure => validatorFailure.ErrorMessage,
                    (propertyName, errorMessages) => new
                    {
                        Key = propertyName,
                        Values = errorMessages.Distinct().ToArray()
                    })
                .ToDictionary(option => option.Key, option => option.Values);

            if (errorsDictionary.Any())
            {
                throw new ValidationException(errorsDictionary.ToString());
            }

            return await next();
        }
    }
}