
namespace UnityReplGs.Tools.Tests
{
    public class CombinedTestResultCollector
    {
        public readonly List<object> collectors = new List<object>();

        public object Combine()
        {
            return new { collectors = collectors };
        }
    }
}
