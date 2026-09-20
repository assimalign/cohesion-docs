using Assimalign.Cohesion.Viu.Markdown;
using Assimalign.Viu.Components;

namespace CohesionDocs;

internal static class CohesionDocsComponentCatalog
{
    internal static ComponentFactory CreateFactory()
    {
        ComponentFactory factory = new();
        GeneratedViuComponents.Register(factory);
        MarkdownComponents.Register(factory);
        factory.Register(DocsPageView.Registration);
        factory.Register(new ComponentRegistration(
            ComponentReference.ForName(nameof(DocsPageView)),
            DocsPageView.Registration.Contract,
            DocsPageView.Registration.Activator));
        return factory;
    }
}
