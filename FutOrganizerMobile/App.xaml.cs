using FutOrganizerMobile.Application.Interfaces.Services;
using FutOrganizerMobile.Pages;

namespace FutOrganizerMobile;

public partial class App : IApplication // <-- alterado de IApplication para Application
{
    public App(ILoginService loginService)
    {
        InitializeComponent();

        MainPage = new NavigationPage(new LoginPage(loginService))
        {
            BarBackgroundColor = Color.FromArgb("#121212"),
            BarTextColor = Colors.White
        };
    }
}
