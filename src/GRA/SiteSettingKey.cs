
namespace GRA
{
    public struct SiteSettingKey
    {
        public struct SecretCode
        {
            // Currently: if this is set (i.e. not null), hide the secret code entry
            // TODO make this truly disable secret codes for the site
            public const string Disable = "SecretCode.Disable";
        }
    }
}
