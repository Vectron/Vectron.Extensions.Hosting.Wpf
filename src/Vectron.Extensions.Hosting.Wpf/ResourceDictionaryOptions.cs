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
        DataTemplates = [];
        Sources = [];
    }

    /// <summary>
    /// Gets the resources to add.
    /// </summary>
    public IList<DataTemplateDescriptor> DataTemplates
    {
        get;
        init;
    }

    /// <summary>
    /// Gets external resources to merge.
    /// </summary>
    public IList<string> Sources
    {
        get;
        init;
    }

    /// <summary>
    /// Describes how a <see cref="DataTemplate"/> should be constructed.
    /// </summary>
    /// <param name="DataType">The type of the data object.</param>
    /// <param name="ViewType">The type of the view to display this template.</param>
    public record DataTemplateDescriptor(Type DataType, Type ViewType);
}
