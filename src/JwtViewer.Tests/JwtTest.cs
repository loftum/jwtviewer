using System.Text;
using System.Text.Json;
using JwtViewer.Tests.Assertions;
using JwtViewer.ViewModels.Core;
using Xunit;
using Xunit.Abstractions;

namespace JwtViewer.Tests;

public class JwtTest
{
    protected readonly ITestOutputHelper Output;
    private const string Raw =
        "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkthbXVmIExhcnNlbiIsImlhdCI6MTUxNjIzOTAyMn0.QTrmCF1_LqZiqm567jj_gagJgjGziqELmwBrHR8-PKE";
    
    public JwtTest(ITestOutputHelper output)
    {
        Output = output;
    }

    [Fact]
    public void Parse_Succeeds()
    {
        Verify.That(Jwt.TryParse(Raw, out _));
    }

    [Fact]
    public void ToString_ReturnsRaw()
    {
        Verify.That(Jwt.TryParse(Raw, out var jwt));
        Verify.That(jwt.ToString().IsStringEqualTo(Raw));
    }

    [Fact]
    public void Position_IsZero()
    {
        Verify.That(Jwt.TryParse(Raw, out var jwt));
        Verify.That(jwt.Position.IsEqualTo(0));
    }

    [Fact]
    public void HeaderPosition_IsZero()
    {
        Verify.That(Jwt.TryParse(Raw, out var jwt));
        Verify.That(jwt.Header.RawPosition.IsEqualTo(0));
        Verify.That(jwt.Header.JsonStartPosition.IsEqualTo(0));
    }

    [Fact]
    public void PayloadPosition_IsSet()
    {
        Verify.That(Jwt.TryParse(Raw, out var jwt));
        Verify.That(jwt.Payload.RawPosition.IsEqualTo(37));
        Verify.That(jwt.Payload.JsonStartPosition.IsEqualTo(37));
    }

    [Fact]
    public void Json()
    {
        var json =
"""
{
  "name": "value"
}
""";
        var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(json));
        while (reader.Read())
        {
            Output.WriteLine($"{reader.TokenType} {reader.TokenStartIndex}");
        }
    }
}