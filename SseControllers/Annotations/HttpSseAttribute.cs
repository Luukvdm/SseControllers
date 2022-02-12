using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Formatters.Internal;
using Microsoft.AspNetCore.Mvc.Routing;

namespace SseControllers.Annotations
{
    [AttributeUsage(AttributeTargets.Method)]
    public class HttpSseAttribute : HttpMethodAttribute, IResultFilter, IOrderedFilter, IApiResponseMetadataProvider
    {
        private static readonly IEnumerable<string> _supportedMethods = new[] {"GET"};

        /// <summary>
        /// Creates a new <see cref="HttpSseAttribute"/>.
        /// </summary>
        public HttpSseAttribute() : base(_supportedMethods)
        {
        }

        /// <summary>
        /// Creates a new <see cref="HttpSseAttribute"/> with the given route template.
        /// </summary>
        /// <param name="template">The route template. May not be null.</param>
        public HttpSseAttribute(string template)
            : base(_supportedMethods, template)
        {
            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }
        }

        public void SetContentTypes(MediaTypeCollection contentTypes)
        {
            contentTypes.Clear();
            contentTypes.Add(Constants.SseContentType);
        }

        /// <inheritdoc />
        public Type Type { get; }

        /// <inheritdoc />
        public int StatusCode { get; }

        /// <inheritdoc />
        public void OnResultExecuting(ResultExecutingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Result is not ObjectResult objectResult)
            {
                return;
            }

            foreach (var t in context.Filters)
            {
                var filter = t as IFormatFilter;

                if (filter?.GetFormat(context) != null)
                {
                    return;
                }
            }

            SetContentTypes(objectResult.ContentTypes);
        }

        /// <inheritdoc />
        public void OnResultExecuted(ResultExecutedContext context)
        {
        }
    }
}
