using System.Data;
using MySql.Data.MySqlClient;

namespace GhostIM.Utility;

public class UserData
{
    private const string connectCtx = @"server=localhost;userid=ghost;password=oe_FDC[]L0sYJ5eN;database=im";
    
    public static string GetScreenName(string username)
    {
        using(var conn = new MySqlConnection(connectCtx)) 
        {
            conn.Open(); 
            using (var cmd = new MySqlCommand("SELECT screenname FROM `users` WHERE `username` LIKE '" + username + "'", conn)) 
            {
                using(IDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read()) 
                    { 
                        return reader.GetString(reader.GetOrdinal("screenname"));
                    } 
                } 
            }
        }

        return null;
    }

    public static int GetUserId(string username)
    {
        using(var conn = new MySqlConnection(connectCtx)) 
        {
            conn.Open(); 
            using (var cmd = new MySqlCommand("SELECT id FROM `users` WHERE `username` LIKE '" + username + "'", conn)) 
            {
                using(IDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read()) 
                    { 
                        return reader.GetInt32(reader.GetOrdinal("id"));
                    } 
                } 
            }
        }

        return 0;
    }

    public static string GetPassword(string username)
    {
        using(var conn = new MySqlConnection(connectCtx)) 
        {
            conn.Open(); 
            using (var cmd = new MySqlCommand("SELECT password FROM `users` WHERE `username` LIKE '" + username + "'", conn)) 
            {
                using(IDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read()) 
                    { 
                        return reader.GetString(reader.GetOrdinal("password"));
                    } 
                } 
            }
        }

        return null;
    }
    
    public static string GeUserIdentificationNumber(string username)
    {
        using(var conn = new MySqlConnection(connectCtx)) 
        {
            conn.Open(); 
            using (var cmd = new MySqlCommand("SELECT uin FROM `users` WHERE `username` LIKE '" + username + "'", conn)) 
            {
                using(IDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read()) 
                    { 
                        return reader.GetString(reader.GetOrdinal("uin"));
                    } 
                } 
            }
        }

        return null;
    }
}