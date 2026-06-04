
namespace UnityReplGs.Tools.Tests
{
    public class TestResultCollector
    {
        public string status = "NotStarted";
        public object response = new
        {
            summary = new { total = 0, passed = 0, failed = 0, skipped = 0 },
            results = new List<object>(),
            logs = new List<object>()
        };
    }
}
