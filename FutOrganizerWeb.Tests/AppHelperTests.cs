using FutOrganizerWeb.Domain.Helpers;
using Xunit;

namespace FutOrganizerWeb.Tests;

public class AppHelperTests
{
    [Fact]
    public void NormalizeLocation_ParsesFourPartInput()
    {
        var result = AppHelper.NormalizeLocation("48,85,46,33");
        Assert.Equal("48.85,46.33", result);
    }

    [Fact]
    public void NormalizeLocation_ReturnsZeroForEmptyString()
    {
        var result = AppHelper.NormalizeLocation(string.Empty);
        Assert.Equal("0.0,0.0", result);
    }

    [Fact]
    public void NormalizeLocation_ReturnsZeroForMalformedString()
    {
        var result = AppHelper.NormalizeLocation("invalid,input");
        Assert.Equal("0.0,0.0", result);
    }
}
