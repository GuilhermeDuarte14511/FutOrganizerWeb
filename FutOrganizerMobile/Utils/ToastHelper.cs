using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;

public static class ToastHelper
{
    public static void mostrarToast(Layout layout, string mensagem, Color backgroundColor)
    {
        var toastLabel = new Label
        {
            Text = mensagem,
            TextColor = Colors.White,
            Padding = new Thickness(10, 4),
            FontAttributes = FontAttributes.Bold,
            FontSize = 13,
            HorizontalTextAlignment = TextAlignment.Center
        };

        var border = new Border
        {
            BackgroundColor = backgroundColor,
            StrokeThickness = 0,
            StrokeShape = new RoundRectangle { CornerRadius = 10 },
            Content = toastLabel,
            Opacity = 0,
            TranslationY = -40,
            Margin = new Thickness(0, 10, 0, 0),
            HorizontalOptions = LayoutOptions.Center,
            MinimumHeightRequest = 1,
            HeightRequest = 36
        };

        layout.Children.Add(border);

        border.FadeTo(1, 400, Easing.CubicOut);
        border.TranslateTo(0, 0, 300, Easing.SpringOut);

        Task.Run(async () =>
        {
            await Task.Delay(3000);
            await border.FadeTo(0, 300);
            MainThread.BeginInvokeOnMainThread(() => layout.Children.Remove(border));
        });
    }

    public static void mostrarToast(AbsoluteLayout toastContainer, string mensagem, Color backgroundColor)
    {
        var toastLabel = new Label
        {
            Text = mensagem,
            TextColor = Colors.White,
            Padding = new Thickness(12, 6),
            FontAttributes = FontAttributes.Bold,
            FontSize = 13,
            HorizontalTextAlignment = TextAlignment.Center
        };

        var border = new Border
        {
            BackgroundColor = backgroundColor,
            StrokeThickness = 0,
            StrokeShape = new RoundRectangle { CornerRadius = 10 },
            Content = toastLabel,
            Opacity = 0,
            WidthRequest = 300
        };

        AbsoluteLayout.SetLayoutFlags(border, AbsoluteLayoutFlags.PositionProportional);
        AbsoluteLayout.SetLayoutBounds(border, new Rect(0.5, 0, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));

        toastContainer.Children.Add(border);
        toastContainer.IsVisible = true;

        border.FadeTo(1, 400, Easing.CubicOut);

        Task.Run(async () =>
        {
            await Task.Delay(3000);
            await border.FadeTo(0, 300);
            MainThread.BeginInvokeOnMainThread(() =>
            {
                toastContainer.Children.Remove(border);
                toastContainer.IsVisible = false;
            });
        });
    }

    public static async Task mostrarToastAsync(AbsoluteLayout toastContainer, string mensagem, Color backgroundColor)
    {
        var toastLabel = new Label
        {
            Text = mensagem,
            TextColor = Colors.White,
            Padding = new Thickness(12, 6),
            FontAttributes = FontAttributes.Bold,
            FontSize = 13,
            HorizontalTextAlignment = TextAlignment.Center
        };

        var border = new Border
        {
            BackgroundColor = backgroundColor,
            StrokeThickness = 0,
            StrokeShape = new RoundRectangle { CornerRadius = 10 },
            Content = toastLabel,
            Opacity = 0,
            WidthRequest = 300
        };

        AbsoluteLayout.SetLayoutFlags(border, AbsoluteLayoutFlags.PositionProportional);
        AbsoluteLayout.SetLayoutBounds(border, new Rect(0.5, 0, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));

        toastContainer.Children.Add(border);
        toastContainer.IsVisible = true;

        await border.FadeTo(1, 400, Easing.CubicOut);
        await Task.Delay(3000);
        await border.FadeTo(0, 300);

        MainThread.BeginInvokeOnMainThread(() =>
        {
            toastContainer.Children.Remove(border);
            toastContainer.IsVisible = false;
        });
    }
}
