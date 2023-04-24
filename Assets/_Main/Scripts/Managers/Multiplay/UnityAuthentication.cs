using System;
using Arikan;
using Cysharp.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Matchmaker;
using Unity.Services.Matchmaker.Models;
using Unity.Services.Multiplay;
using Unity.Services.Multiplay.Models;
using UnityEngine;

public class UnityAuthentication : SingletonBehaviour<UnityAuthentication>
{
    private string backfillTicketId;
    private float acceptBackfillTicketTimer;

    public async UniTask Initialize()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            InitializationOptions options = new();
            await UnityServices.InitializeAsync(options);

            MultiplayEventCallbacks multiplayCallbacks = new();
            multiplayCallbacks.Allocate += MultiplayCallbacks_Allocate;
            multiplayCallbacks.Deallocate += MultiplayCallbacks_Deallocate;
            multiplayCallbacks.SubscriptionStateChanged += MultiplayCallbacks_SubscriptionStateChanged;
            multiplayCallbacks.Error += MultiplayCallbacks_Error;
            IServerEvents serverEvents = await MultiplayService.Instance.SubscribeToServerEventsAsync(multiplayCallbacks);

            var serverQuery = await MultiplayService.Instance.StartServerQueryHandlerAsync(
                3,
                "MyServer",
                "GameType",
                "1.0.0",
                "Default"
            );

            Debug.Log("SERVER initialized");
        }
        else
        {
            Debug.Log("SERVER already initialized");

            var serverConfig = MultiplayService.Instance.ServerConfig;
            if (!string.IsNullOrEmpty(serverConfig.AllocationId))
            {
                MultiplayCallbacks_Allocate(new("", serverConfig.ServerId, serverConfig.AllocationId));
            }
        }
    }

    private void MultiplayCallbacks_Error(MultiplayError obj)
    {
        Debug.LogError(obj.Reason);
        Debug.LogError(obj.Detail);
    }

    private void MultiplayCallbacks_SubscriptionStateChanged(MultiplayServerSubscriptionState obj)
    {
    }

    private void MultiplayCallbacks_Deallocate(MultiplayDeallocation obj)
    {
    }

    private void MultiplayCallbacks_Allocate(MultiplayAllocation obj)
    {
        var serverConfig = MultiplayService.Instance.ServerConfig;
        Debug.Log($"{serverConfig.IpAddress} {serverConfig.Port} {serverConfig.QueryPort} {serverConfig.ServerId}");

        // StartServer

        SetupBackfillTickets();
    }

    private async void SetupBackfillTickets()
    {
        var payloadAllocations = await MultiplayService.Instance.GetPayloadAllocationFromJsonAs<MatchmakingResults>();

        var backfillTicketId = payloadAllocations.BackfillTicketId;

    }

    private async void HandleBackfillTickets()
    {
        // if enough slot available

        var ticket = await MatchmakerService.Instance.ApproveBackfillTicketAsync(backfillTicketId);
        backfillTicketId = ticket.Id;
    }
}