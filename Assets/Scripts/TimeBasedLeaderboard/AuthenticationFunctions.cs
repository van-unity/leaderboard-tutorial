using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;

namespace TimeBasedLeaderboard {
    public static class AuthenticationFunctions {
        public static async Task<Result> Authenticate() {
            var initializationTask = UnityServices.InitializeAsync();

            try {
                await initializationTask;
            }
            catch (Exception exception) {
                return Result.Failure(exception.Message ?? string.Empty);
            }

            var signInTask = AuthenticationService.Instance.SignInAnonymouslyAsync();

            try {
                await signInTask;
                if (AuthenticationService.Instance.IsSignedIn) {
                    return Result.Success();
                }
            }
            catch (Exception exception) {
                return Result.Failure(exception.Message ?? string.Empty);
            }

            return Result.Failure(string.Empty);
        }

        public static void SignOut() {
            AuthenticationService.Instance.SignOut(true);
        }
    }
}