using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace FS.Email.Templates;

public class HtmlEmailRenderer(HtmlRenderer renderer)
{
    public Task<string> RenderAsync<TComponent>(
        IDictionary<string, object?> parameters,
        CancellationToken ct)
        where TComponent : IComponent
    {
        ct.ThrowIfCancellationRequested();

        return renderer.Dispatcher.InvokeAsync(async () =>
        {
            var output = await renderer.RenderComponentAsync<TComponent>(
                ParameterView.FromDictionary(parameters));

            ct.ThrowIfCancellationRequested();
            return output.ToHtmlString();
        });
    }
}
