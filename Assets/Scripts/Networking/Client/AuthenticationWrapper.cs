using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public static class AuthenticationWrapper
{
    public static AuthState AuthState { get; private set; } = AuthState.NotAuthenticated;

    public static async Task<AuthState> DoAuth(int maxTries = 5)
    {
        if (AuthState == AuthState.Authenticated)
        {
            return AuthState;
        }

        if (AuthState == AuthState.Authenticating)
        {
            Debug.Log("Already Authenticating");
            await Authenticating();
            return AuthState;
        }

        await SignInAnonymuslyAsync(maxTries);
 
        return AuthState;
    }

    private static async Task<AuthState> Authenticating()
    {
        while(AuthState == AuthState.Authenticating || AuthState == AuthState.NotAuthenticated)
        {
            await Task.Delay(200);
        }

        return AuthState;
    }

    private static async Task SignInAnonymuslyAsync(int maxTries)
    {
        AuthState = AuthState.Authenticating;

        int retries = 0;

        while (AuthState == AuthState.Authenticating && retries <= maxTries)
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
                {
                    AuthState = AuthState.Authenticated;
                    break;
                }
            }
            catch(AuthenticationException ex)
            {
                AuthState = AuthState.Error;
                Debug.LogError(ex);
            }
            catch(RequestFailedException exception)
            {
                AuthState = AuthState.Error;
                Debug.LogError(exception);
            }
            retries++;
            await Task.Delay(1000);
        }

    }
} 

public enum AuthState
{
    NotAuthenticated,
    Authenticating,
    Authenticated,
    Error,
    TimeOut
}
