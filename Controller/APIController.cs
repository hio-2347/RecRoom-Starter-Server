using Microsoft.AspNetCore.Mvc;
using RecRoom.Config
using System.Text.Json;

namespace RecRoom.Controllers
{
    [ApiController]
    public class APIController : ControllerBase
    {
        [HttpGet("/")]
        public IActionResult GetNS()
        {
            string url = ServerConfig.BaseURL;
            return Ok(new
            {
                Accounts = url + "/acc",
                API = url,
                Auth = url + "/auth",
                BugReporting = url,
                Cards = url,
                CDN = url + "/cdn",
                Chat = url,
                Clubs = url,
                CMS = url,
                Commerce = url,
                Data = url,
                DataCollection = url,
                Discovery = url,
                Econ = url,
                GameLogs = url,
                Geo = url,
                Images = url + "/imageserver",
                Leaderboard = url,
                Link = url,
                Lists = url,
                Matchmaking = url + "/match",
                Moderation = url,
                Notifications = url + "/noti",
                PlatformNotifications = url,
                PlayerSettings = url,
                RoomComments = url,
                Rooms = url + "/roomserver",
                Storage = url,
                Strings = url,
                StringsCDN = url,
                Studio = url,
                Thorn = url,
                Videos = url,
                WWW = url
            });
        }

        [HttpGet("api/versioncheck/v4")]
        public IActionResult VersionCheck()
        {
            return Ok(new
            {
                ValidVersion = 0,
                VersionStatus = 0,
                UpdateNotificationStage = 0,
                IsVersionIslanded = false,
                IsCrossPlayDisabled = false
            });
        }

        [HttpGet("api/gameconfigs/v1/all")]
        public IActionResult GetGameConfigs()
        {
            string path = Path.Combine(Program.dataDir, "APIS", "GameConfigs.json");
            return System.IO.File.Exists(path) ? Content(System.IO.File.ReadAllText(path), "application/json") : NotFound();
        }

        [HttpGet("api/config/v1/amplitude")]
        public IActionResult GetAmplitude()
        {
            return Ok(new
            {
                AmplitudeKey = "cb2fb2ecb9953512c29af5bca58f2b4a",
                UseRudderStack = true,
                RudderStackKey = "23NiJHIgu3koaGNCZIiuYvIQNCu",
                UseStatSig = true,
                StatSigKey = "client-SBZkOrjD3r1Cat3f3W8K6sBd11WKlXZXIlCWj6l4Aje",
                StatSigEnvironment = 0
            });
        }

        [HttpGet("api/avatar/v1/defaultunlocked")]
        public IActionResult GetDefaultUnlocked()
        {
            return Ok(ServerConfig.Bracket);
        }

        [HttpGet("api/avatar/v1/defaultbaseavataritems")]
        public IActionResult GetDefaultBaseAvatarItems()
        {
            return Ok(ServerConfig.Bracket);
        }

		[HttpGet("api/avatar/v1/realitems")]
        public IActionResult GetRealItems()
        {
            return Ok("[FemRec]");
        }
        
        [HttpGet("/api/objectives/v1/myprogress")]
        public IActionResult GetMyObjectiveProgress()
        {
        	var id = AuthStuff.GetPlayerId(Request);
            if (id == null) 
            	return Unauthorized();
                
            return Ok(new 
            {
            	Objectives = new List<object>(),
                ObjectiveGroups = new List<object>()
            });
        }
        
        [HttpGet("/api/avatar/v2")]
        public IActionResult GetMyAvatar()
        {
            var player = AuthStuff.GetCurrentPlayer(Request);
            if (player == null)
            	return Unauthorized();

            return Ok(player.Player.PlayerExtra.Avatar);
        }
        
        [HttpPost("/api/avatar/v2/set")]
        public IActionResult SetMyAvatar([FromBody] PlayerDBClasses.Avatar request)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null) 
            	return Unauthorized();

            return Ok(PlayerDB.SetAvatar((long)id, request));
        }
        
        [HttpGet("/api/avatar/v4/items")]
        public IActionResult GetMyAvatarItems()
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null) 
            	return Unauthorized();

            string path = Path.Combine(Program.dataDir, "APIS", "Items", "AvatarItems.json");
            return System.IO.File.Exists(path) ? Content(System.IO.File.ReadAllText(path), "application/json") : NotFound();
        }
        
        [HttpGet("/api/PlayerReporting/v1/moderationBlockDetails")]
        public IActionResult GetMyModerationBlockDetails()
        {
            var player = AuthStuff.GetCurrentPlayer(Request);
            if (player == null)
            	return Unauthorized();

            return Ok(player.Player.PlayerExtra.ModerationBlockDetails);
        }
        
        [HttpGet("/api/relationships/v2/get")]
        public IActionResult GetMyRelationships()
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null) 
            	return Unauthorized();

            return Ok(ServerConfig.Bracket);
        }
        
        [HttpGet("/api/messages/v2/get")]
        public IActionResult GetMyMessages()
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null) 
            	return Unauthorized();

            return Ok(ServerConfig.Bracket);
        }
        
        [HttpGet("/playersettings")]
        public IActionResult GetMySettings()
        {
            var player = AuthStuff.GetCurrentPlayer(Request);
            if (player == null)
            	return Unauthorized();

            return Ok(player.Player.PlayerExtra.Settings);
        }
        
        [HttpPut("/playersettings")]
        public IActionResult SetMySettings([FromForm] string key, [FromForm] string value)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null) 
            	return Unauthorized();
                
			PlayerDB.SetPlayerSetting(key, value ?? "", (long)id);
            
            return NoContent();
        }
        
        [HttpGet("/econ/customAvatarItems/v1/owned")]
        public IActionResult GetMyOwnedCustomAvatarItems([FromQuery] int skip, [FromQuery] int take)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null) 
            	return Unauthorized();
                
            return Ok(new 
            {
            	Results = Array.Empty<object>(),
                TotalResults = 0
            });
        }
        
        [HttpGet("/api/checklist/v1/current")]
        public IActionResult GetMyChecklist()
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null) 
            	return Unauthorized();

            return Ok(ServerConfig.Bracket);
        }
        
        [HttpGet("/api/players/v2/progression/bulk")]
        public IActionResult GetProgressionForPlayers([FromQuery] List<long> id)
        {
            var authId = AuthStuff.GetPlayerId(Request);
            if (authId == null) 
                return Unauthorized();

            if (id == null || id.Count == 0)
                return Ok(new List<PlayerDBClasses.PlayerProgressionDTO>());

            var progressions = PlayerDB.GetProgressionBulk(id);

            return Ok(progressions);
        }
        
        [HttpGet("/api/playerReputation/v2/bulk")]
        public IActionResult GetReputationBulk([FromQuery] List<long> id)
        {
            var authId = AuthStuff.GetPlayerId(Request);
            if (authId == null) 
                return Unauthorized();

            if (id == null || id.Count == 0)
                return Ok(new List<PlayerDBClasses.Reputation>());

            var results = PlayerDB.GetReputationBulk(id);
            return Ok(results);
        }
        
        [HttpPost("/api/PlayerReporting/v1/hile")] // todo log to channel
        public IActionResult PlayerReportingHile()
        {
            var authId = AuthStuff.GetPlayerId(Request);
            if (authId == null) 
                return Unauthorized();
                
            return Ok(false);
        }
        
        [HttpGet("api/config/v2")]
        public IActionResult GetConfigV2()
        {
            string path = Path.Combine(Program.dataDir, "APIS", "ConfigV2.json");
            return System.IO.File.Exists(path) ? Content(System.IO.File.ReadAllText(path), "application/json") : NotFound();
        }
        
        [HttpGet("/api/announcement/v1/get")]
        [HttpGet("/api/PlayerReporting/v1/voteToKickReasons")]
        [HttpGet("/api/avatar/v3/saved")]
        [HttpGet("/api/equipment/v2/getUnlocked")]
        [HttpGet("/api/consumables/v2/getUnlocked")]
        [HttpGet("/api/images/v2/named")]
        [HttpGet("/api/avatar/v2/gifts")]
        [HttpGet("/api/gamerewards/v1/pending")]
        [HttpGet("/api/roomkeys/v1/mine")]
        [HttpGet("/cdn/config/LoadingScreenTipData")]
        [HttpGet("/api/roomcurrencies/v1/currencies")]
        [HttpGet("/api/inventions/v2/mine")]
        public IActionResult TodoImplement()
        {
            return Ok(ServerConfig.Bracket);
        }
        
        [HttpGet("/api/playerevents/v1/all")]
        public IActionResult GetAllPlayerEvents()
        {
            return Ok(new 
            {
            	Created = Array.Empty<object>(),
                Responses = Array.Empty<object>()
            });
        }
        
        [HttpGet("/api/customAvatarItems/v1/isCreationAllowedForAccount")]
        public IActionResult GetCustomAvatarItemsIsCreationAllowedForAccount()
        {
            return Ok(new
            {
            	success = true,
                value = (object?)null
            });
        }
        
        [HttpGet("/api/customAvatarItems/v1/isCreationEnabled")]
        [HttpGet("/api/customAvatarItems/v1/isRenderingEnabled")]
        public IActionResult CustomAvatarItemsIsEnabled()
        {
            return Ok(true);
        }
        
        [HttpGet("/api/roomconsumables/v1/roomConsumable/room/{roomId}")]
        public IActionResult GetRoomConsumablesForRoom(long roomId)
        {
        	return Ok(ServerConfig.Bracket);
        }
        
        [HttpPost("/api/sanitize/v1")]
        public IActionResult SanitizeV1([FromBody] SanitizeRequest request)
        {
        	return Ok(JsonSerializer.Serialize(request.Value));
        }
        
        [HttpPost("/api/sanitize/v1/isPure")]
        public IActionResult SanitizeV1IsPure()
        {
        	return Ok(new
            {
                IsPure = true
            });
        }
        
        public class SanitizeRequest
        {
            public string Value { get; set; } = string.Empty;
        }
    }
}
