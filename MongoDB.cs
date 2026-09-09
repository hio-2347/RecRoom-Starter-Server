using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using RecRoom.Data;
using System.Text.Json.Serialization;

namespace RecRoom
{
    public class MongoDB
    {
        private static readonly MongoClient mongoClient = new("uh add mongoclient");
        private static readonly IMongoDatabase mongoDatabase = mongoClient.GetDatabase("RevivalDB");
        public static readonly IMongoCollection<User> usersCollection = mongoDatabase.GetCollection<User>("Users");
        public static readonly IMongoCollection<RoomDetailsMongoDB> roomsCollection = mongoDatabase.GetCollection<RoomDetailsMongoDB>("Rooms");

        public class User
        {
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)]
            public string Id { get; set; }

            [BsonElement("accountId")]
            public long AccountId { get; set; }
            
            [BsonElement("username")]
            public string? Username { get; set; }
            
            [BsonElement("bio")]
            public string? Bio { get; set; }
            
            [BsonElement("displayName")]
            public string? DisplayName { get; set; }
            
            [BsonElement("profileImage")]
            public string? ProfileImage { get; set; }
            
            [BsonElement("email")]
            public string? Email { get; set; }
            
            [BsonElement("password")]
            public string? Password { get; set; }
            
            [BsonElement("dateOfBirth")]
            public DateTime DateOfBirth { get; set; }
            
            [BsonElement("createdAt")]
            public DateTime CreatedAt { get; set; }
            
            [BsonElement("isJunior")]
            public bool IsJunior { get; set; }
            
            [BsonElement("platform")]
            public PlatformType Platform { get; set; }
            
            [BsonElement("platformId")]
            public ulong PlatformId { get; set; }
            
            [BsonElement("otherData")]
            public OtherData OtherData { get; set; } = new OtherData();
            
            [BsonElement("isDeveloper")]
            public bool IsDeveloper { get; set; }
        }

        public class OtherData
        {
            [BsonElement("roomInstance")]
            public RoomInstance? RoomInstance { get; set; }

            [BsonElement("avatar")]
            public AvatarDTO? Avatar { get; set; }
            
            [BsonElement("avatarItems")]
            public List<UnlockedAvatarItem>? AvatarItems { get; set; }
            
            [BsonElement("loginLockCode")]
            public string? LoginLockCode { get; set; }
            
            [BsonElement("outfits")]
            public Outfits? Outfits { get; set; }
            
            [BsonElement("dormRoomId")]
            public long? DormRoomId { get; set; }

            [BsonElement("settings")]
            public List<SettingDTO>? Settings { get; set; }

            [BsonElement("relationships")]
            public List<RelationshipDTO>? Relationships { get; set; }
        }

        public class Outfits
        {
            [BsonElement("avatar1")]
            public AvatarDTO? Avatar1 { get; set; }
            
            [BsonElement("avatar2")]
            public AvatarDTO? Avatar2 { get; set; }
            
            [BsonElement("avatar3")]
            public AvatarDTO? Avatar3 { get; set; }
            
            [BsonElement("avatar4")]
            public AvatarDTO? Avatar4 { get; set; }
            
            [BsonElement("avatar5")]
            public AvatarDTO? Avatar5 { get; set; }
            
            [BsonElement("avatar6")]
            public AvatarDTO? Avatar6 { get; set; }
            
            [BsonElement("avatar7")]
            public AvatarDTO? Avatar7 { get; set; }
            
            [BsonElement("avatar8")]
            public AvatarDTO? Avatar8 { get; set; }
            
            [BsonElement("avatar9")]
            public AvatarDTO? Avatar9 { get; set; }
        }

        public class RoomDetailsMongoDB
        {
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)]
            public string Id { get; set; }

            public RoomDTO Room { get; set; } = new ();

            public List<SceneDTO>? Scenes { get; set; }

            public List<int>? CoOwners { get; set; }

            public List<int>? InvitedCoOwners { get; set; }

            public List<int>? Moderators { get; set; }

            public List<int>? InvitedModerators { get; set; }

            public List<int>? Hosts { get; set; }

            public List<int>? InvitedHosts { get; set; }

            public int CheerCount { get; set; }

            public int FavoriteCount { get; set; }

            public int VisitCount { get; set; }

            public List<TagDTO>? Tags { get; set; }
        }
    }
}
