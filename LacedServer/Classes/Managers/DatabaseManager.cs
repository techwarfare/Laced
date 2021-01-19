using CitizenFX.Core;
using LacedServer.Models;
using LacedShared.Classes;
using LacedShared.Enums;
using LacedShared.Libs;
using LacedShared.Models;
using LiteDB;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LacedServer.Classes.Managers
{
    public class DatabaseManager
    {
        private static string ConnectionString = ConfigManager.ServerConfig.DBConnectionString;

        //Used for selecting/reading data from the SQL database
        protected static async Task<User> SelectUserQueryDatabase(string _query)
        {
            User foundUser = null;
            using (var conn = new MySqlConnection(ConnectionString))
            {

                await conn.OpenAsync();

                using (var cmd = new MySqlCommand(_query, conn))
                {

                    using (DbDataReader dataReader = await cmd.ExecuteReaderAsync())
                    {
                        while (await dataReader.ReadAsync())
                        {
                            foundUser = new User();
                            foundUser.Id = dataReader.GetInt32(0);
                            foundUser.Name = dataReader.GetString(1);
                            foundUser.SteamIdentifier = dataReader.GetString(2);
                            foundUser.LicenseIdentifier = dataReader.GetString(3);
                            foundUser.GroupPerms = (Permission)dataReader.GetInt32(4);
                            foundUser.IsWhitelisted = dataReader.GetBoolean(5);
                            int banDataID = dataReader.GetInt32(6);

                            if (banDataID != 0)
                            {
                                foundUser.BanData = new BanStatus(banDataID);
                            }

                            return foundUser;

                        }
                    }
                }
            }

            return foundUser;
        }

        protected static async Task<int> InsertIntoDatabase(string _query)
        {
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(_query, conn))
                {
                    await conn.OpenAsync();
                    return await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        protected static List<Character> GetDBCharacters([FromSource] Player _player, User _user)
        {
            try
            {
                using (LiteDatabase db = new LiteDatabase(@"./resources/[local]/laced/data/laced.db"))
                {
                    LiteCollection<Character> characters = (LiteCollection<Character>)db.GetCollection<Character>("characters");
                    IEnumerable<Character> usersCharacters = characters.Find(charc => charc.UserId == _user.Id);
                    return usersCharacters.ToList();
                }
            }
            catch (Exception _ex)
            {
                Utils.Throw(_ex);
                return null;
            }
        }

        public static Tuple<bool,List<Character>> CreateDBCharacter([FromSource] Player _player, User _user, string _firstName, string _lastName, Genders _gender)
        {
            using (LiteDatabase db = new LiteDatabase(@"./resources/[local]/laced/data/laced.db"))
            {
                LiteCollection<Character> characters = (LiteCollection<Character>)db.GetCollection<Character>("characters");
                int _charID = characters.Find(charc => charc.UserId == _user.Id).Count() + 1;
                Utils.DebugLine(_charID.ToString(), "SDatabaseManager");

                try
                {
                    characters.Insert(_charID, new Character(_user.Id, _firstName, _lastName, _gender, 0, 0, 0, new List<GarageItem>(), null));
                    Utils.DebugLine($"Inserted character [{characters.Count()}] [{db.GetCollection<Character>("characters").Find(c => c.UserId == _user.Id).ToList()[0].FirstName}]", "SDatabaseManager");
                }
                catch (Exception _ex)
                {
                    Utils.Throw(_ex);
                }

                IEnumerable<Character> usersCharacters = characters.Find(charc => charc.UserId == _user.Id);
                Utils.DebugLine(usersCharacters.Count().ToString(), "SDatabaseManager");
                Utils.DebugLine(usersCharacters.ToList()[0].FirstName, "SDatabaseManager");
                return Tuple.Create(true, usersCharacters.ToList());
            }
        }

        protected static Character SelectDBCharacter([FromSource] Player _player, User _user, int _charID)
        {
            try
            {
                using (LiteDatabase db = new LiteDatabase(@"./resources/[local]/laced/data/laced.db"))
                {
                    LiteCollection<Character> characters = (LiteCollection<Character>)db.GetCollection<Character>("characters");
                    return characters.FindOne(charc => charc.Id == _charID);
                }
            }
            catch (Exception _ex)
            {
                Utils.Throw(_ex);
                return null;
            }
        }

        protected static List<Character> DeleteDBCharacter([FromSource] Player _player, User _user, int _charID)
        {
            try
            {
                using (LiteDatabase db = new LiteDatabase(@"./resources/[local]/laced/data/laced.db"))
                {
                    LiteCollection<Character> characters = (LiteCollection<Character>)db.GetCollection<Character>("characters");
                    characters.Delete(_charID);
                    IEnumerable<Character> userCharacters = characters.Find(charc => charc.UserId == _user.Id);
                    return userCharacters.ToList();
                }
            }
            catch (Exception _ex)
            {
                Utils.Throw(_ex);
                return null;
            }
        }
        protected static List<Character> UpdateDBCharacter([FromSource] Player _player, User _user, int _charID)
        {
            try
            {
                using (LiteDatabase db = new LiteDatabase(@"./resources/[local]/laced/data/laced.db"))
                {
                    LiteCollection<Character> characters = (LiteCollection<Character>)db.GetCollection<Character>("characters");
                    characters.Update(SessionManager.Sessions.Find(s => s.Player.Handle == _player.Handle).selectedCharacter);
                    IEnumerable<Character> userCharacters = characters.Find(charc => charc.UserId == _user.Id);
                    return userCharacters.ToList();
                }
            }
            catch (Exception _ex)
            {
                Utils.Throw(_ex);
                return null;
            }
        }
    }

    public class PlayerDBManager : DatabaseManager
    {
        public static async Task<User> GetPlayerData([FromSource] Player _player)
        {
            Utils.DebugLine("Getting Player Data!", "SDBManager");
            string query = "SELECT * FROM player_data";
            //Check if the player has steam or not
            if (_player.Identifiers["steam"] != null) { 
                query += " WHERE player_steam='"+_player.Identifiers["steam"]+"'";
            }
            else
            {
                query += " WHERE player_license='"+_player.Identifiers["license"]+"'";
            }

            return await SelectUserQueryDatabase(query);
        }

        public static async Task<int> CreatePlayerData([FromSource] Player _player)
        {
            Utils.DebugLine("Creating Player Data!", "SDBManager");
            string query = $"INSERT INTO player_data(`player_name`, `player_steam`, `player_license`, `player_whitelisted`, `player_banned`) VALUES('{_player.Name}', '{_player.Identifiers["steam"]}', '{_player.Identifiers["license"]}', '0', '0')";

            return await InsertIntoDatabase(query);
        }
    }

    public class CharacterDBManager : DatabaseManager
    {
        public static List<Character> GetCharacters([FromSource] Player _player, User _user)
        {
            Utils.DebugLine($"Getting {_player.Name} characters!", "SDatabaseManager");

            return GetDBCharacters(_player, _user);
        }

        public static List<Character> CreateCharacter([FromSource] Player _player, User _user, string _firstName, string _lastName, Genders _gender)
        {
            Utils.DebugLine($"Creating character for {_player.Name} [{_firstName} {_lastName}]", "SDatabaseManager");

            Tuple<bool, List<Character>> _chars = CreateDBCharacter(_player, _user, _firstName, _lastName, _gender);

            Utils.WriteLine($"Created character! [{_chars.Item1}]");
            return _chars.Item2;
        }

        public static Character SelectCharacter([FromSource] Player _player, User _user, int _charID)
        {
            Utils.DebugLine($"{_player.Name} is selecting a character [{_charID}]", "SDatabaseManager");

            return SelectDBCharacter(_player, _user, _charID);
        }

        public static List<Character> DeleteCharacter([FromSource] Player _player, User _user, int _charID)
        {
            Utils.DebugLine($"{_player.Name} is deleting character[{_charID}]", "SDatabaseManager");

            return DeleteDBCharacter(_player, _user, _charID);
        }

        public static List<Character> UpdateCharacter([FromSource] Player _player, User _user, int _charID)
        {
            Utils.DebugLine($"{_player.Name} is updating character[{_charID}]", "SDatabaseManager");

            return UpdateDBCharacter(_player, _user, _charID);
        }
    }
}
