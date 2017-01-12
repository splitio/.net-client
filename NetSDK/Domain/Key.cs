
namespace Splitio.Domain
{
    public class Key
    {
        public string matchingKey { get; private set; }
        public string bucketingKey { get; private set; }

        public Key(string matchingKey, string bucketingKey)
        {
            this.matchingKey = matchingKey;
            this.bucketingKey = bucketingKey ?? matchingKey;
        }
    }
}
