using RestSharp;

namespace OrangeHRM.Tests.Helpers;

public static class TestDataCleanup
{
    public static void TryDelete(
        string entityName,
        int? entityId,
        Func<int, RestResponse> deleteAction)
    {
        if (!entityId.HasValue)
        {
            Console.WriteLine($"Cleanup skipped for {entityName}: no ID was captured.");

            return;
        }

        try
        {
            var response = deleteAction(entityId.Value);

            Console.WriteLine($"Delete {entityName} {entityId}: " + $"HTTP {(int)response.StatusCode}. Body: {response.Content}");

            if (!response.IsSuccessful)
            {
                Console.WriteLine($"WARNING: Could not delete {entityName} {entityId}.");
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine($"WARNING: Exception while deleting {entityName} {entityId}: " + exception.Message);
        }
    }
}