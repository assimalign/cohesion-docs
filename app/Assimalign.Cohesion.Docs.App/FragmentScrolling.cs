using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;

namespace CohesionDocs;

internal static partial class FragmentScrolling
{
    [JSImport("globalThis.cohesionDocs.waitForHeading")]
    internal static partial Task<string?> WaitForHeadingAsync(string routePath, string fragment);
}
