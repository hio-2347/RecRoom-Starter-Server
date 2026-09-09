using RecRoom.Important;

namespace RecRoom.Data.Stuff
{
    public class VersionCheck
    {
        [ServerAPI.GET("/api/versioncheck/v2")]
        public VersionCheckV2ResponseDTO ReturnVersionCheckV2(HttpContext ctx)
        {
            string? v = ctx.Request.Query["v"];

            VersionCheckV2ResponseDTO response = new()
            {
                ValidVersion = false
            };

            if (v != BuildVersion.Version && v != BuildConstants.PCVersion)
            {
                response = new VersionCheckV2ResponseDTO
                {
                    ValidVersion = false
                };
            }

            return response;
        }

        [ServerAPI.GET("/api/versioncheck/v3")]
        public VersionCheckV3ResponseDTO ReturnVersionCheckV3(HttpContext ctx)
        {
            string? v = ctx.Request.Query["v"];

            VersionCheckV3ResponseDTO response = new()
            {
                ValidVersion = false
            };

            if (v != BuildVersion.Version && v != BuildConstants.PCVersion)
            {
                response = new VersionCheckV3ResponseDTO
                {
                    ValidVersion = false
                };
            }

            return response;
        }

        [ServerAPI.GET("/api/versioncheck/v4")]
        public VersionCheckV4ResponseDTO ReturnVersionCheckV4(HttpContext ctx)
        {
            string? v = ctx.Request.Query["v"];

            VersionCheckV4ResponseDTO response = new()
            {
                VersionStatus = VersionStatus.ValidForPlay
            };

            if (v != BuildVersion.Version && v != BuildConstants.PCVersion)
            {
                response = new VersionCheckV4ResponseDTO
                {
                    VersionStatus = VersionStatus.ValidForMenu
                };
            }

            return response;
        }
    }
}
