using Microsoft.VisualStudio.Utilities;

namespace OneCode.VsExtension.Utilities
{
    public static class PropertyCollectionExtensions
    {
        public static T GetOrDefault<T>(this PropertyCollection collection, object key)
        {
            return collection.TryGetProperty<T>(key, out var result) ? result : default!;
        }
    }
}
