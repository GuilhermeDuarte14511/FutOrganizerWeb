using Microsoft.Maui;
using Microsoft.Extensions.DependencyInjection;

namespace FutOrganizerMobile.Utils
{
    public static class ServiceHelper
    {
        public static IServiceProvider Services { get; private set; }

        public static void Init(IServiceProvider serviceProvider)
        {
            Services = serviceProvider;
        }

        public static T GetService<T>() where T : class
        {
            return Services.GetService<T>();
        }
    }
}
