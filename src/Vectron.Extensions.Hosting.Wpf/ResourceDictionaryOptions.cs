using System.Windows;

namespace Vectron.Extensions.Hosting.Wpf;

/// <summary>
/// Options for configuring the runtime <see cref="ResourceDictionary"/>.
/// </summary>
public class ResourceDictionaryOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ResourceDictionaryOptions"/> class.
    /// </summary>
    public ResourceDictionaryOptions()
    {
        Entries = new Dictionary<object, object>();
        Sources = new List<Uri>();
    }

    /// <summary>
    /// Gets the resources to add.
    /// </summary>
    public IDictionary<object, object> Entries
    {
        get;
        init;
    }

    /// <summary>
    /// Gets external resources to merge.
    /// </summary>
    public IList<Uri> Sources
    {
        get;
        init;
    }
}
