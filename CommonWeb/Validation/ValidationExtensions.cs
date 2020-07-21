using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace HanumanInstitute.CommonWeb.Validation
{
    /// <summary>
    /// Provides methods to validate objects based on DataAnnotations.
    /// </summary>
    public static class ValidationExtensions
    {
        /// <summary>
        /// Returns a configuration object if it passes validation defined by DataAnnotations, or throws ValidationException if validation fails.
        /// </summary>
        /// <typeparam name="T">The type of object to validate against and return.</typeparam>
        /// <param name="configuration">The configuration section to parse.</param>
        /// <returns>The configuration object.</returns>
        /// <exception cref="Services.ComponentModel.DataAnnotations.ValidationException">Occurs when validation fails.</exception>
        public static T GetValid<T>(this IConfiguration configuration) where T : class, new()
        {
            var obj = configuration.Get<T>() ?? new T();
            Validator.ValidateObject(obj, new ValidationContext(obj), true);
            return obj;
        }

        /// <summary>
        /// Validates an object based on its DataAnnotations and throws an exception if the object is not valid.
        /// </summary>
        /// <param name="obj">The object to validate.</param>
        public static T ValidateAndThrow<T>(this T obj)
        {
            Validator.ValidateObject(obj, new ValidationContext(obj), true);
            return obj;
        }

        /// <summary>
        /// Validates an object based on its DataAnnotations and returns a list of validation errors.
        /// </summary>
        /// <param name="obj">The object to validate.</param>
        /// <returns>A list of validation errors.</returns>
        public static ICollection<ValidationResult>? Validate<T>(this T obj)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(obj);
            if (!Validator.TryValidateObject(obj, context, results, true))
            {
                return results;
            }
            return null;
        }

        public static void ValidateToModelState(this PageModel model, object? objectToValidate = null)
        {
            model.CheckNotNull(nameof(model));

            var errors = objectToValidate != null ? objectToValidate.Validate() : model.Validate();
            if (errors != null)
            {
                foreach (var item in errors)
                {
                    model.ModelState.AddModelError(item.MemberNames.ToString(), item.ErrorMessage);
                }
            }
        }
    }
}
