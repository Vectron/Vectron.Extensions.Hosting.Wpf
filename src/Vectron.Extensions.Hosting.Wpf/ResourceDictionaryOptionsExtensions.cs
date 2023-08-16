using System.Windows;
using System.Windows.Controls;

namespace Vectron.Extensions.Hosting.Wpf;

/// <summary>
/// Extension methods for <see cref="ResourceDictionaryOptions"/>.
/// </summary>
public static class ResourceDictionaryOptionsExtensions
{
    /// <summary>
    /// Add a <see cref="DataTemplate"/> to the <see cref="ResourceDictionaryOptions"/>.
    /// </summary>
    /// <typeparam name="TData">The type of data.</typeparam>
    /// <typeparam name="TView">The view to use for the data.</typeparam>
    /// <param name="options">The <see cref="ResourceDictionaryOptions"/> to add the item to.</param>
    /// <returns>The updated <see cref="ResourceDictionaryOptions"/>.</returns>
    public static ResourceDictionaryOptions Add<TData, TView>(this ResourceDictionaryOptions options)
        where TData : class
        where TView : UserControl
    {
        var descriptor = new ResourceDictionaryOptions.DataTemplateDescriptor(typeof(TData), typeof(TView));
        return options.Add(descriptor);
    }

    /// <summary>
    /// Add a source to an other <see cref="ResourceDictionary"/> to include in this application.
    /// </summary>
    /// <param name="options">The <see cref="ResourceDictionaryOptions"/> to add the item to.</param>
    /// <param name="source">The <see cref="Uri"/> to the <see cref="ResourceDictionary"/> location.</param>
    /// <returns>The updated <see cref="ResourceDictionaryOptions"/>.</returns>
    public static ResourceDictionaryOptions Add(this ResourceDictionaryOptions options, string source)
    {
        options.Sources.Add(source);
        return options;
    }

    private static ResourceDictionaryOptions Add(this ResourceDictionaryOptions options, ResourceDictionaryOptions.DataTemplateDescriptor descriptor)
    {
        options.DataTemplates.Add(descriptor);
        return options;
    }
}
