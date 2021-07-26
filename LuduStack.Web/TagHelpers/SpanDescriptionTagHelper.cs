﻿using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Threading.Tasks;

namespace LuduStack.Web.TagHelpers
{
    /// <summary>
    /// <see cref="ITagHelper"/> implementation targeting &lt;span&gt; elements with an <c>asp-description-for</c> attribute.
    /// Adds an <c>id</c> attribute and sets the content of the &lt;span&gt; with the Description property from the model data annotation DisplayAttribute.
    /// </summary>
    [HtmlTargetElement("span", Attributes = DescriptionForAttributeName)]
    public class SpanDescriptionTagHelper : TagHelper
    {
        private const string DescriptionForAttributeName = "asp-description-for";

        /// <summary>
        /// Creates a new <see cref="SpanDescriptionTagHelper"/>.
        /// </summary>
        /// <param name="generator">The <see cref="IHtmlGenerator"/>.</param>
        public SpanDescriptionTagHelper(IHtmlGenerator generator)
        {
            Generator = generator;
        }

        /// <inheritdoc />
        public override int Order => -1000;

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        protected IHtmlGenerator Generator { get; }

        /// <summary>
        /// An expression to be evaluated against the current model.
        /// </summary>
        [HtmlAttributeName(DescriptionForAttributeName)]
        public ModelExpression DescriptionFor { get; set; }

        /// <inheritdoc />
        /// <remarks>Does nothing if <see cref="DescriptionFor"/> is <c>null</c>.</remarks>
        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            Microsoft.AspNetCore.Mvc.ModelBinding.ModelMetadata metadata = DescriptionFor.Metadata;

            if (metadata == null)
            {
                throw new InvalidOperationException(string.Format("No provided metadata ({0})", DescriptionForAttributeName));
            }

            output.Attributes.SetAttribute("id", metadata.PropertyName + "-description");
            output.TagMode = TagMode.StartTagAndEndTag;

            if (!string.IsNullOrWhiteSpace(metadata.Description))
            {
                string content = string.Format("<i class=\"fas fa-exclamation-circle\" data-container=\"body\" data-toggle=\"popover\" data-trigger=\"hover\" data-placement=\"top\" data-html=\"true\" data-content=\"{0}\" aria-hidden=\"true\"></i>", metadata.Description);

                output.Content.SetHtmlContent(content);
            }

            return Task.CompletedTask;
        }
    }
}