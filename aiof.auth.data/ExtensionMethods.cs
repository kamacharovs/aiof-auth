using System;
using System.Threading.Tasks;

using FluentValidation;
using FluentValidation.Results;

namespace aiof.auth.data
{
    public static class ExtensionMethods
    {
        public static async Task ValidateAndThrowAsync<T>(
            this IValidator<T> validator, 
            T instance,
            string ruleSet)
        {
            var result = await validator.ValidateAsync(instance, o => o.IncludeRuleSets(ruleSet));

            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }
        }

        public static async Task ValidateAndThrowAsync<T>(
            this IValidator<T> validator,
            T instance)
        {
            var result = await validator.ValidateAsync(instance, o => o.IncludeAllRuleSets());
            
            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }
        }
    }
}