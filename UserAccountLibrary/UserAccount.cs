using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Windows.Storage.Streams;
using Windows.Security.Cryptography;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Asn1.X509;
using Windows.Security.Credentials;
using Windows.Storage;
using System.Diagnostics;
namespace UserAccountLibrary
{

    public class User
    {
        public int userId { get; }
        public String username { get; }
        public String access { get; }
        public String email { get; }
        public String firstName { get; }
        public String lastName { get; }

        public User( int userId, String username, String access, String email, String firstName, String lastName)
        {
            this.userId = userId;
            this.username = username;
            this.access = access;
            this.email = email;
            this.firstName = firstName;
            this.lastName = lastName;
        }
    }
     
    public class UserKey
    {
        public int keyId { get; }
        public int userId { get; }
        public IBuffer publicKey { get; }
        public String pubKeyHash { get; }
        public IBuffer activeChallenge { get; }
        public String deviceId { get; }

        public UserKey(int KeyId, int userId, IBuffer publicKey, String pubKeyHash, IBuffer activeChallenge, String deviceId)
        {
            this.keyId = keyId;
            this.userId = userId;
            this.publicKey = publicKey;
            this.pubKeyHash = pubKeyHash;
            this.activeChallenge = activeChallenge;
            this.deviceId = deviceId;
        }
    
    }

    public class UserAccount
    {
        public async static Task InitializeDatabase()
        {
            await ApplicationData.Current.LocalFolder.CreateFileAsync("Accounts.db", CreationCollisionOption.OpenIfExists);
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Accounts.db");
            using (SqliteConnection db = 
                new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();
                SqliteCommand initCommand = new SqliteCommand();
                initCommand.Connection = db;

                initCommand.CommandText = "CREATE TABLE IF NOT " +
                    "EXISTS Users (ID INTEGER PRIMARY KEY, " + " username VARCHAR(255), " + "access VARCHAR(255), " +
                    "email VARCHAR(255), " + "firstName VARCHAR(255), " + "lastName VARCHAR(255));";

                initCommand.ExecuteReader();

                SqliteCommand initCommand2 = new SqliteCommand();
                initCommand2.Connection = db;

                initCommand2.CommandText = "CREATE TABLE IF NOT " +
                    "EXISTS UserKeys (ID INTEGER PRIMARY KEY, " + "userId INTEGER, " + "publicKey MEDIUMBLOB, " +
                   "publicKeyHash MEDIUMTEXT, "+ "activeChallenge MEDIUMBLOB, " + "deviceId TINYTEXT); ";

                initCommand2.ExecuteReader();

                db.Close();

            }
        }

        public static int InsertUser( String username, String access, String email, String firstName, String lastName)
        {
            int retval = 0;
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Accounts.db");
            using (SqliteConnection db = 
                new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();
                SqliteCommand insertCommand = new SqliteCommand();
                insertCommand.Connection = db;

                insertCommand.CommandText = "INSERT INTO Users(username, access, email, firstName, lastName) " +
                    "VALUES(@Entry1, @Entry2, @Entry3, @Entry4, @Entry5);";
                insertCommand.Parameters.AddWithValue("@Entry1", username);
                insertCommand.Parameters.AddWithValue("@Entry2", access);
                insertCommand.Parameters.AddWithValue("@Entry3", email);
                insertCommand.Parameters.AddWithValue("@Entry4", firstName);
                insertCommand.Parameters.AddWithValue("@Entry5", lastName);

                SqliteDataReader query = insertCommand.ExecuteReader();
                retval = query.RecordsAffected;

                db.Close();
            }
            return retval;
        }

        public static int InsertUserKeys(int userId, IBuffer publicKey, String pubKeyHash, String deviceId)
        {
            int retval = 0;
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Accounts.db");
            using (SqliteConnection db = 
                new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();
                SqliteCommand insertCommand = new SqliteCommand();
                insertCommand.Connection = db;
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                byte[] buffer = new byte[16];
                rng.GetBytes(buffer);
                IBuffer challenge = buffer.AsBuffer();

                insertCommand.CommandText = "INSERT INTO UserKeys(userId, publicKey, publicKeyHash, activeChallenge, deviceId)" +
                    " VALUES(@Entry1, @Entry2, @Entry3, @Entry4, @Entry5);";
                insertCommand.Parameters.AddWithValue("@Entry1", userId);
                insertCommand.Parameters.AddWithValue("@Entry2", publicKey.ToArray());
                insertCommand.Parameters.AddWithValue("@Entry3", pubKeyHash);
                insertCommand.Parameters.AddWithValue("@Entry4", challenge.ToArray());
                insertCommand.Parameters.AddWithValue("@Entry5", deviceId);

                SqliteDataReader query = insertCommand.ExecuteReader();
                retval = query.RecordsAffected;

                db.Close();
            }
            return retval;
        }

        public static User GetUser(String username)
        {
            User entry = null;
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Accounts.db");
            using (SqliteConnection db = 
                new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand();
                selectCommand.Connection = db;

                selectCommand.CommandText = "SELECT * FROM Users WHERE username=@Entry;";
                selectCommand.Parameters.AddWithValue("@Entry", username);

                SqliteDataReader query =  selectCommand.ExecuteReader();
                if (query.FieldCount == 6 && query.Read())
                {
                    Debug.WriteLine(query.FieldCount);
                    entry = new User(query.GetInt32(0), query.GetString(1), query.GetString(2), query.GetString(3), query.GetString(4), query.GetString(5));

                }
                db.Close();
            }
            return entry;

        }

        public static User GetUserByName(String firstName, String lastName)
        {
            User entry = null;
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Accounts.db");
            using (SqliteConnection db =
                new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand();
                selectCommand.Connection = db;

                selectCommand.CommandText = "SELECT * FROM Users WHERE firstName=@Entry1 AND lastName=@Entry2;";
                selectCommand.Parameters.AddWithValue("@Entry1", firstName);
                selectCommand.Parameters.AddWithValue("@Entry2", lastName);
                

                SqliteDataReader query = selectCommand.ExecuteReader();
                if (query.FieldCount == 6 && query.Read())
                {
                    Debug.WriteLine(query.FieldCount);
                    entry = new User(query.GetInt32(0), query.GetString(1), query.GetString(2), query.GetString(3), query.GetString(4), query.GetString(5));

                }
                db.Close();
            }
            return entry;


        }

        public static IBuffer RequestChallenge(int userId, String publicKeyHash)
        {
            IBuffer challengeBuffer = null;
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Accounts.db");
            using (SqliteConnection db = 
                new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand();
                selectCommand.Connection = db;

                selectCommand.CommandText = "SELECT * FROM UserKeys WHERE userId=@Entry1 and publicKeyHash=@Entry2;";
                selectCommand.Parameters.AddWithValue("@Entry1", userId);
                selectCommand.Parameters.AddWithValue("@Entry2", publicKeyHash);

                SqliteDataReader query = selectCommand.ExecuteReader();

                if (query.Read())
                {
                    int row_id = query.GetInt32(0);

                    SqliteBlob challengeBlob = new SqliteBlob(db, "UserKeys", "activeChallenge", row_id);
                    byte[] bytes = new byte[challengeBlob.Length];
                    challengeBlob.Read(bytes, 0, (int)challengeBlob.Length);
                    challengeBuffer = bytes.AsBuffer();
                    challengeBlob.Close();

                }



                db.Close();
            }
            return challengeBuffer;

        }

        public static Boolean SubmitResponse(int userId, String publicKeyHash, IBuffer signatureBuffer)
        {
            bool retval = false;
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Accounts.db");
            using (SqliteConnection db = 
                new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand();
                selectCommand.Connection = db;

                selectCommand.CommandText = "SELECT * FROM UserKeys WHERE userId=@Entry1 and publicKeyHash=@Entry2;";
                selectCommand.Parameters.AddWithValue("@Entry1", userId);
                selectCommand.Parameters.AddWithValue("@Entry2", publicKeyHash);

                SqliteDataReader query = selectCommand.ExecuteReader();

                if (query.Read())
                {
                    int rowId = query.GetInt32(0);

                    SqliteBlob challengeBlob = new SqliteBlob(db, "UserKeys", "activeChallenge", rowId);
                    byte[] challengeBuffer = new byte[challengeBlob.Length];
                    challengeBlob.Read(challengeBuffer, 0, (int)challengeBlob.Length);
                    challengeBlob.Close();

                    SqliteBlob keyBlob = new SqliteBlob(db, "UserKeys", "publicKey", rowId);
                    byte[] pubKeyBuffer = new byte[keyBlob.Length];
                    keyBlob.Read(pubKeyBuffer, 0, (int)keyBlob.Length);
                    keyBlob.Close();


                    /// BIGGG FUCK IT MOMENT......XD

                    CngKey pubCngKey = CngKey.Import(pubKeyBuffer, CngKeyBlobFormat.GenericPublicBlob);
                    String signatureString = CryptographicBuffer.EncodeToBase64String(signatureBuffer);
                    using (RSACng pubKey = new RSACng(pubCngKey))
                    {
                        byte[] signature = Convert.FromBase64String(signatureString);
                        retval = pubKey.VerifyData(challengeBuffer, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                    }

                }
                db.Close();
            }
            return retval;

        }

        public static Boolean ValidateKey(int userId, string publicKeyHash)
        {
            bool retval = false;
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Accounts.db");
            using (SqliteConnection db =
                new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();
                SqliteCommand verifyCommand = new SqliteCommand();
                verifyCommand.Connection = db;

                verifyCommand.CommandText = "SELECT * FROM UserKeys WHERE userId=@Entry1 and publicKeyHash=@Entry2;";
                verifyCommand.Parameters.AddWithValue("@Entry1", userId);
                verifyCommand.Parameters.AddWithValue("@Entry2", publicKeyHash);

                SqliteDataReader query = verifyCommand.ExecuteReader();
                retval = query.HasRows;

                db.Close();

            }
            return retval;
        }

        public static Boolean ValidateAccount(String username)
        {
            bool retval = false;
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Accounts.db");
            using (SqliteConnection db = 
                new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();
                SqliteCommand verifyCommand = new SqliteCommand();
                verifyCommand.Connection = db;

                verifyCommand.CommandText = "SELECT * FROM Users WHERE username=@Entry;";
                verifyCommand.Parameters.AddWithValue("@Entry", username);

                SqliteDataReader query = verifyCommand.ExecuteReader();
                retval = query.HasRows;

                db.Close();
                
            }
            return retval;
        }

        public static int DeleteUser( int userId)
        {
            int retval = 0;
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Accounts.db");
            using (SqliteConnection db = 
                new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();
                SqliteCommand deleteCommand = new SqliteCommand();
                deleteCommand.Connection = db;

                deleteCommand.CommandText = "DELETE FROM Users WHERE ID=@Entry;";
                deleteCommand.Parameters.AddWithValue("@Entry", userId);

                SqliteDataReader query = deleteCommand.ExecuteReader();
                retval = query.RecordsAffected;
                

                db.Close();
            }
            return retval;
        }
        public static int DeleteAllUsers()
        {
            int delNum = 0;
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Accounts.db");
            using (SqliteConnection db =
                new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();
                SqliteCommand deleteCommand = new SqliteCommand();
                deleteCommand.Connection = db;

                deleteCommand.CommandText = "DELETE FROM Users;";

                SqliteDataReader query = deleteCommand.ExecuteReader();

                delNum = query.RecordsAffected;
                db.Close();
            }
            return delNum;
        }

        public static int DeleteUserKey(int userId, String pubKeyHash)
        {
            int retval = 0;
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Accounts.db");
            using (SqliteConnection db = 
                new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();
                SqliteCommand deleteCommand = new SqliteCommand();
                deleteCommand.Connection = db;

                deleteCommand.CommandText = "DELETE FROM UserKeys WHERE userId=@Entry and publicKeyHash=@Entry2;";
                deleteCommand.Parameters.AddWithValue("@Entry", userId);
                deleteCommand.Parameters.AddWithValue("@Entry2", pubKeyHash);

                SqliteDataReader query = deleteCommand.ExecuteReader();

                retval = query.RecordsAffected;

                db.Close();

            }
            return retval;
        }
        public static int DeleteAllKeys()
        {
            int delNum = 0;
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Accounts.db");
            using (SqliteConnection db =
                new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();
                SqliteCommand deleteCommand = new SqliteCommand();
                deleteCommand.Connection = db;

                deleteCommand.CommandText = "DELETE FROM UserKeys;";

                SqliteDataReader query = deleteCommand.ExecuteReader();

                delNum = query.RecordsAffected;
                db.Close();
            }
            return delNum;

        }
        //This method is for testing purposes. 
        private static void CreateTestTable()
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Accounts.db");
            using (SqliteConnection db =
                new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();
                SqliteCommand initCommand = new SqliteCommand();
                initCommand.Connection = db;

                initCommand.CommandText = "CREATE TABLE IF NOT " +
                    "EXISTS Test (ID INTEGER PRIMARY KEY, " + " username VARCHAR(255) );";

                initCommand.ExecuteReader();

                db.Close();

            }

        }

        public static void DeleteTable(String table)
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Accounts.db");
            using (SqliteConnection db = 
                new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();
                SqliteCommand deleteCommand = new SqliteCommand();
                deleteCommand.Connection = db;

                if (table.Equals("Users"))
                {
                    deleteCommand.CommandText = "DROP TABLE IF EXISTS Users;";

                }
                else
                {
                    deleteCommand.CommandText = "DROP TABLE IF EXISTS UserKeys;";

                }


                deleteCommand.ExecuteReader();

                db.Close();
            }
        }

       
    }
}
