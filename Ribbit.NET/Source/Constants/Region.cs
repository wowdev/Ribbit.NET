namespace Ribbit.Constants
{
    public enum Region
    {
        EU, US, KR, CN, TW,
    }

    internal static class RegionExtensions
    {
        public static string GetName(this Region region)
        {
            switch (region)
            {
                case Region.EU: return "eu";
                case Region.US: return "us";
                case Region.KR: return "kr";
                case Region.CN: return "cn";
                case Region.TW: return "tw";
                default: throw new UnknownRegionException(region);
            }
        }

        public static string GetHostname(this Region region) => region.GetName() + ".version.battle.net";
    }

    public class UnknownRegionException : System.Exception
    {
        public Region region;

        public UnknownRegionException(Region region) : base("Unknown region provided")
        {
            this.region = region;
        }
    }
}
