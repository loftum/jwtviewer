using Newtonsoft.Json.Bson;
using Xunit.Sdk;

namespace JwtViewer.Tests.Assertions;

public delegate bool TestDelegateWithMessage(out string error);

public static class Verify
{
    public static void That(TestDelegateWithMessage del)
    {
        if (!del(out var error))
        {
            throw FailException.ForFailure(error);
        }
    }

    public static void That(bool expected)
    {
        if (!expected)
        {
            throw FailException.ForFailure($"Expected true, but got false");
        }
    }
}

public static class Verifications
{
    public static TestDelegateWithMessage IsStringEqualTo(this string actual, string expected, StringComparison comparison = default)
    {
        return IsStringEqualWithMessage;
        bool IsStringEqualWithMessage(out string error)
        {
            if (!string.Equals(actual, expected, comparison))
            {
                error = $"Expected '{expected}'\nbut got\n'{actual}'";
                return false;
            }

            error = default;
            return true;
        }
    }

    public static TestDelegateWithMessage IsEqualTo(this int actual, int expected)
    {
        return IsIntEqualWithMessage;
        bool IsIntEqualWithMessage(out string error)
        {
            if (actual != expected)
            {
                error = $"Expected '{expected}'\nbut got\n'{actual}'";
                return false;
            }

            error = default;
            return true;
        }
    }
    
    public static TestDelegateWithMessage IsEqualTo(this long actual, long expected)
    {
        return IsIntEqualWithMessage;
        bool IsIntEqualWithMessage(out string error)
        {
            if (actual != expected)
            {
                error = $"Expected '{expected}'\nbut got\n'{actual}'";
                return false;
            }

            error = default;
            return true;
        }
    }
}

