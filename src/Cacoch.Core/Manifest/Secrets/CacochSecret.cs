namespace Cacoch.Core.Manifest.Secrets
{
    public class CacochSecret
    {
        public CacochSecret(string s)
        {
            Secret = s;
        }

        public string Secret { get; }
    }
}