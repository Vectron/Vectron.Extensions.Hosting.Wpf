using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Vectron.Extensions.Hosting.Wpf;

/// <summary>
/// Extension methods for adding services to an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add a <see cref="DataTemplate"/> to the <see cref="ResourceDictionary"/>.
    /// </summary>
    /// <typeparam name="TData">The type of the view model.</typeparam>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddDataTemplateResource<TData, TView>(this IServiceCollection services)
        where TData : class
        where TView : UserControl
    {
        _ = services.ConfigureResourceDictionary(options => options.Add<TData, TView>());
        return services;
    }

    /// <summary>
    /// Add a <see cref="ResourceDictionary"/> to the application.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="uri">The location of the resource dictionary to add.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddResourceDictionary(this IServiceCollection services, string uri)
    {
        var creationOptions = new UriCreationOptions()
        {
            DangerousDisablePathAndQueryCanonicalization = false,
        };

        if (Uri.TryCreate(uri, creationOptions, out var result))
        {
            _ = services.ConfigureResourceDictionary(options => options.Add(result));
        }

        return services;
    }

    /// <summary>
    /// Adds a scoped WPF window service of the type specified in <typeparamref name="TWindow"/> to
    /// the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TWindow">The type of the window service to add.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model service to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddWindowScoped<TWindow, TViewModel>(this IServiceCollection services)
        where TWindow : Window, IComponentConnector
        where TViewModel : class
    {
        _ = services.AddScoped(WindowFactory<TWindow, TViewModel>);
        _ = services.AddScoped<TViewModel>();
        return services;
    }

    /// <summary>
    /// Adds a singleton WPF window service of the type specified in <typeparamref name="TWindow"/>
    /// to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TWindow">The type of the window service to add.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model service to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddWindowSingleton<TWindow, TViewModel>(this IServiceCollection services)
        where TWindow : Window, IComponentConnector
        where TViewModel : class
    {
        _ = services.AddSingleton(WindowFactory<TWindow, TViewModel>);
        _ = services.AddSingleton<TViewModel>();
        return services;
    }

    /// <summary>
    /// Adds a transient WPF window service of the type specified in <typeparamref name="TWindow"/>
    /// to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TWindow">The type of the window service to add.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model service to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddWindowTransient<TWindow, TViewModel>(this IServiceCollection services)
        where TWindow : Window, IComponentConnector
        where TViewModel : class
    {
        _ = services.AddTransient(WindowFactory<TWindow, TViewModel>);
        _ = services.AddTransient<TViewModel>();
        return services;
    }

    /// <summary>
    /// Adds services required for using wpf.
    /// </summary>
    /// <typeparam name="TApplication">The main WPF entry point.</typeparam>
    /// <typeparam name="TMainWindow">The main window to show.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddWpf<TApplication, TMainWindow>(this IServiceCollection services)
        where TApplication : Application, IComponentConnector
        where TMainWindow : Window, IComponentConnector
    {
        _ = services.AddHostedService<WpfHostedService<TApplication, TMainWindow>>();
        services.TryAddSingleton<TApplication>();
        services.TryAddSingleton<TMainWindow>();
        return services;
    }

    /// <summary>
    /// Adds services required for using wpf.
    /// </summary>
    /// <typeparam name="TApplication">The main WPF entry point.</typeparam>
    /// <typeparam name="TMainWindow">The main window to show.</typeparam>
    /// <typeparam name="TMainWindowViewModel">The main window view model.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddWpf<TApplication, TMainWindow, TMainWindowViewModel>(this IServiceCollection services)
        where TApplication : Application, IComponentConnector
        where TMainWindow : Window, IComponentConnector
        where TMainWindowViewModel : class
    {
        _ = services.AddHostedService<WpfHostedService<TApplication, TMainWindow>>();
        services.TryAddSingleton<TApplication>();
        _ = services.AddWindowSingleton<TMainWindow, TMainWindowViewModel>();
        return services;
    }

    private static IServiceCollection ConfigureResourceDictionary(this IServiceCollection services, Action<ResourceDictionaryOptions> configureOptions)
    {
        _ = services.Configure(configureOptions);
        return services;
    }

    private static TWindow WindowFactory<TWindow, TViewModel>(IServiceProvider serviceProvider)
        where TWindow : Window, IComponentConnector
        where TViewModel : class
    {
        var window = ActivatorUtilities.CreateInstance<TWindow>(serviceProvider);
        window.DataContext = serviceProvider.GetRequiredService<TViewModel>();
        window.InitializeComponent();
        return window;
    }
}
