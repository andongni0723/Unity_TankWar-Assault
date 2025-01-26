using System;
using UnityEngine;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using UnityEngine.Events;
using UnityEngine.UI;

public class An_ConnectionManager : Singleton<An_ConnectionManager>
{
    [Header("Component")]
    public Button quickJoinButton;
    public Button leaveButton;
    
    [Header("Settings")]
    public int maxPlayers = 2;
    
    public UnityEvent JoiningSession = new();
    public UnityEvent<ISession> JoinedSession = new();
    public UnityEvent<SessionException> FailedToJoinSession = new();
    
    [Header("Debug")]
    private ISession _session;
    
    public override async void Awake()
    {
        base.Awake();
        await AwakeInitial();
    }
    
    private async Task AwakeInitial()
    {
        quickJoinButton.onClick.AddListener(CallQuickJoinSession);
        quickJoinButton.interactable = false;
        leaveButton.interactable = false;
        await UnityServices.InitializeAsync();
        quickJoinButton.interactable = true;
    }

    /// <summary>
    /// Match a session with the QuickJoinOptions. Call by Button Click.
    /// </summary>
    public async void CallQuickJoinSession()
    {
        quickJoinButton.interactable = false;
        JoiningSession?.Invoke();
        Debug.Log("Click Join Button");
        try
        {
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            
            var sessionOptions = new SessionOptions
            {
                IsLocked = false, IsPrivate = false,
                MaxPlayers = maxPlayers,
                Name = Guid.NewGuid().ToString()
            }.WithRelayNetwork();
            
            var quickJoinOptions = new QuickJoinOptions
            {
                CreateSession = true
            };

            _session = await MultiplayerService.Instance.MatchmakeSessionAsync(quickJoinOptions, sessionOptions);
            JoinedSession?.Invoke(_session);
            leaveButton.interactable = true;
        }
        catch (SessionException e)
        {
            FailedToJoinSession?.Invoke(e);
            Debug.LogException(e);
        } 
    }
    
    public async void LeaveSession()
    {
        if (_session != null)
            await _session.LeaveAsync();
    }
}
