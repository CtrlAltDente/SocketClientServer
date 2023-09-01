using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Network.Managers;

public class ClientConnectionManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputField = default;
    [SerializeField] private TextMeshProUGUI _textMeshProUGUI = default;

    [SerializeField] private ClientManager _clientManager = default;

    public void ConnectToServer()
    {
        FixInputField();
        _clientManager.Initialize();
        _clientManager.ConnectClientToIPAddress(_inputField.text);
    }

    public void ShowReceivedMessage(string message)
    {
        _textMeshProUGUI.text = message;
    }

    private void FixInputField()
    {
        _inputField.text = _inputField.text.Replace(',', '.');
    }
}
