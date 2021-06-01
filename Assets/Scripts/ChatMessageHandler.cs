using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine;

public class ChatMessageHandler : MonoBehaviourPun
{
    public Text messageReceived;

    enum EventCodes
    {
        chatmessage = 0,
            
    }
    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived  += OnEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        object content = photonEvent.CustomData;
        EventCodes code = (EventCodes)eventCode;

        if (code == EventCodes.chatmessage)
        {
            object[] datas = content as object[];
            messageReceived.text = (string)datas[0];
        }
      
    }

    public void SendMessage(string message)
    {
        object[] datas = new object[] { message };
        //cache options
        RaiseEventOptions options = new RaiseEventOptions {

            CachingOption = EventCaching.DoNotCache,
            Receivers = ReceiverGroup.All
        };

        SendOptions sendOptions = new SendOptions();
        sendOptions.Reliability = true;

        PhotonNetwork.RaiseEvent((byte) EventCodes.chatmessage, datas, options,sendOptions);
    }
}
