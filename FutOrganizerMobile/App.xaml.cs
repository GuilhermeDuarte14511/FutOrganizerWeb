using FutOrganizerMobile.Application.Interfaces.Services;
using FutOrganizerMobile.Pages;

namespace FutOrganizerMobile;

public partial class App : IApplication
{
    public App(ILoginService loginService)
    {
        InitializeComponent();

        MainPage = new LoginPage(loginService);
    }
}
