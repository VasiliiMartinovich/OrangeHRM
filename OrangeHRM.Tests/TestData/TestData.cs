namespace OrangeHRM.Tests.TestData;

public static class CandidateTestData
{
    public static string UniqueId => Guid.NewGuid().ToString("N");

    public static string FirstName => $"Auto{UniqueId[..8]}";

    public static string LastName => $"Test{UniqueId[..8]}";

    public static string Email => $"auto{UniqueId}@example.com";
}