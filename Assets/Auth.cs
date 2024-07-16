using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
#if UNITY_EDITOR
using ParrelSync;
#endif
public class Auth : MonoBehaviour
{
    public static string _playerId;
    // Start is called before the first frame update
    void Awake()
    {
        Authenticate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private async Task Authenticate()
    {
        var options = new InitializationOptions();

#if UNITY_EDITOR
        // Remove this if you don't have ParrelSync installed. 
        // It's used to differentiate the clients, otherwise lobby will count them as the same
        options.SetProfile(ClonesManager.IsClone() ? ClonesManager.GetArgument() : "Primary");
#endif

        await UnityServices.InitializeAsync(options);

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        _playerId = AuthenticationService.Instance.PlayerId;
    }
}
