using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Newtonsoft.Json; // Install Newtonsoft.Json from NuGet for JSON serialization

public class GameSessionManager : MonoBehaviour
{
    // Import JavaScript function for sending game session events
    [DllImport("__Internal")]
    private static extern void SendGameSessionEvent(string eventName, string payload);

    void Start()
    {
        Debug.Log("Starting Game Session...");

        // Trigger `game_start` event with an empty payload
        SendGameSessionEvent("game_start", SerializePayload(new Dictionary<string, object>()));

        // Trigger `game_fail` event with a score in the payload
        var failPayload = new Dictionary<string, object>
        {
            { "score", 1500 }
        };
        SendGameSessionEvent("game_fail", SerializePayload(failPayload));

        // Trigger `game_retry` event with an empty payload
        SendGameSessionEvent("game_retry", SerializePayload(new Dictionary<string, object>()));

        // Trigger `game_exit` event with additional metadata
        var exitPayload = new Dictionary<string, object>
        {
            { "reason", "user_quit" },
            { "duration", 3600 } // in seconds
        };
        SendGameSessionEvent("game_exit", SerializePayload(exitPayload));
    }

    // Helper method to serialize the payload to JSON
    private string SerializePayload(Dictionary<string, object> payload)
    {
        return JsonConvert.SerializeObject(payload);
    }
}
