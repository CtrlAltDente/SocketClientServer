using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Network.Managers;
using Network.Senders;

public class ServerSenderManager : MonoBehaviour
{
    [SerializeField] private ServerManager _serverMaanger = default;

    [SerializeField] private StringToDataSender _dataSender = default;
    [SerializeField] private TMP_InputField _inputField = default;

    public void SendMessageToClients()
    {
        _dataSender.SendString(_inputField.text);
    }
}
