using System.Windows;
using System.Windows.Markup;
using Microsoft.Extensions.DependencyInjection;

namespace Vectron.Extensions.Hosting.Wpf;

/// <summary>
/// Extension methods for adding services to an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
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
        _ = services.AddTransient(WindowFactory<TWindow, TViewModel>);
        _ = services.AddTransient<TViewModel>();
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
        _ = services.AddTransient(WindowFactory<TWindow, TViewModel>);
        _ = services.AddTransient<TViewModel>();
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
