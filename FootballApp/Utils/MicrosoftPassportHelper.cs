using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Windows.Storage.Streams;
using System.Security.Cryptography;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using UserAccountLibrary;
namespace FootballApp.Utils
{
    public static class MicrosoftPassportHelper
    {
        /// <summary>
        /// Checks to see if Passport is ready to be used.
        /// 
        /// Passport has dependencies on:
        ///     1. Having a connected Microsoft Account
        ///     2. Having a Windows PIN set up for that _account on the local machine
        /// </summary>
        public static async Task<bool> MicrosoftPassportAvailableCheckAsync()
        {
            bool keyCredentialAvailable = await KeyCredentialManager.IsSupportedAsync();
            if (keyCredentialAvailable == false)
            {
                // Key credential is not enabled yet as user 
                // needs to connect to a Microsoft Account and select a PIN in the connecting flow.
                Debug.WriteLine("Microsoft Passport is not setup!\nPlease go to Windows Settings and set up a PIN to use it.");
                return false;
            }

            return true;
        }
        public static async Task<bool> CreatePassportKeyAsync(string accountId, int userId)
        {
            KeyCredentialRetrievalResult keyCreationResult = await KeyCredentialManager.RequestCreateAsync(accountId, KeyCredentialCreationOption.ReplaceExisting);

            switch (keyCreationResult.Status)
            {
                case KeyCredentialStatus.Success:
                    Debug.WriteLine("Successfully made key");
                    KeyCredential userKey = keyCreationResult.Credential;
                    IBuffer pubKey = userKey.RetrievePublicKey(CryptographicPublicKeyBlobType.BCryptPublicKey);

                    HashAlgorithmProvider hashProvider = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);
                    IBuffer publicKeyHash = hashProvider.HashData(pubKey);
                    String pubKeyHint = CryptographicBuffer.EncodeToBase64String(publicKeyHash);
                    String deviceId = "Sam's Latptop";
                    UserAccount.InsertUserKeys(userId, pubKey, pubKeyHint, deviceId);
                    return true;
                case KeyCredentialStatus.UserCanceled:
                    Debug.WriteLine("User cancelled sign-in process.");
                    break;
                case KeyCredentialStatus.NotFound:
                    // User needs to setup Microsoft Passport
                    Debug.WriteLine("Microsoft Passport is not setup!\nPlease go to Windows Settings and set up a PIN to use it.");
                    break;
                default:
                    break;
            }

            return false;
        }

        public static async Task<bool> GetAuthenticationMessageAsync(User account)
        {
            var openKeyResult = await KeyCredentialManager.OpenAsync(account.username);
            
            if(openKeyResult.Status == KeyCredentialStatus.Success)
            {
                KeyCredential userKey = openKeyResult.Credential;
                IBuffer pubKey = userKey.RetrievePublicKey(CryptographicPublicKeyBlobType.BCryptPublicKey);

                HashAlgorithmProvider hashProvider = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);
                IBuffer publicKeyHash = hashProvider.HashData(pubKey);
                String pubKeyHint = CryptographicBuffer.EncodeToBase64String(publicKeyHash);

                IBuffer challenge = UserAccount.RequestChallenge(account.userId, pubKeyHint);
                Debug.WriteLine("Challenge sent by the server: " + challenge.ToString());
                var signResult = await userKey.RequestSignAsync(challenge);
                if (signResult.Status != KeyCredentialStatus.Success)
                {
                    // Launch app-specific flow to handle the scenario 
                    return false;
                }

                bool result = UserAccount.SubmitResponse(account.userId, pubKeyHint, signResult.Result);
                return result;

            }
            else if (openKeyResult.Status == KeyCredentialStatus.NotFound)
            {
                return false;
            }
            else
            {
                return false;
            }

        }

        public static async Task<bool> RemovePassportAccountAsync(User account)
        {
            // Open the account with Passport
            bool result1 = false;
            bool result2 = false;
            KeyCredentialRetrievalResult openKeyResult = await KeyCredentialManager.OpenAsync(account.username);

            if (openKeyResult.Status == KeyCredentialStatus.Success)
            {
                KeyCredential userKey = openKeyResult.Credential;
                IBuffer pubKey = userKey.RetrievePublicKey(CryptographicPublicKeyBlobType.BCryptPublicKey);

                HashAlgorithmProvider hashProvider = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);
                IBuffer publicKeyHash = hashProvider.HashData(pubKey);
                String pubKeyHint = CryptographicBuffer.EncodeToBase64String(publicKeyHash);

                UserAccount.DeleteUser(account.userId);
                UserAccount.DeleteUserKey(account.userId, pubKeyHint);


                result1 = UserAccount.ValidateKey(account.userId, pubKeyHint);
                result2 = UserAccount.ValidateAccount(account.username);
                // In the real world you would send key information to server to unregister
                //for example, RemovePassportAccountOnServer(account);
            }

            // Then delete the account from the machines list of Passport Accounts
            await KeyCredentialManager.DeleteAsync(account.username);

            return (!result1 && !result2);
        }
    }
}
