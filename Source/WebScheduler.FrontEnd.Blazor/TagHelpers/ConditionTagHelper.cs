namespace WebScheduler.FrontEnd.Blazor.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

/// <summary>
/// <see cref="ITagHelper"/> implementation targeting any element that include condition or
/// exclude-if elements.
/// </summary>
[HtmlTargetElement(Attributes = nameof(Condition))]
public class ConditionTagHelper : TagHelper
{
    public bool Condition { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (!this.Condition)
        {
            output.SuppressOutput();
        }
    }
}
