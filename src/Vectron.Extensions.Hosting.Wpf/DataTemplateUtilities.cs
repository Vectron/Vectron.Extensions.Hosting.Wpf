using System.Windows;
using System.Windows.Markup;

namespace Vectron.Extensions.Hosting.Wpf;

/// <summary>
/// Helper class for creating <see cref="DataTemplate"/>.
/// </summary>
internal static class DataTemplateUtilities
{
    /// <summary>
    /// Create a data template from a Data type and a view type.
    /// </summary>
    /// <param name="dataType">The type of the DataType key.</param>
    /// <param name="viewType">The type of the view to use.</param>
    /// <returns>The constructed <see cref="DataTemplate"/>.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "MA0101:String contains an implicit end of line character", Justification = "Idea is to use new lines.")]
    public static DataTemplate CreateDataTemplate(Type dataType, Type viewType)
    {
        var xaml = $$"""
            <DataTemplate DataType="{x:Type vm:{{dataType.Name}}}">
                <v:{{viewType.Name}} />
            </DataTemplate>
            """;

        var context = new ParserContext
        {
            XamlTypeMapper = new XamlTypeMapper(Array.Empty<string>()),
        };
        context.XamlTypeMapper.AddMappingProcessingInstruction("vm", dataType.Namespace, dataType.Assembly.FullName);
        context.XamlTypeMapper.AddMappingProcessingInstruction("v", viewType.Namespace, viewType.Assembly.FullName);

        context.XmlnsDictionary.Add(string.Empty, "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
        context.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");
        context.XmlnsDictionary.Add("vm", "vm");
        context.XmlnsDictionary.Add("v", "v");

        var template = (DataTemplate)XamlReader.Parse(xaml, context);
        return template;
    }
}
