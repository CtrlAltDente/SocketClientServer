using Network.Data;
using Network.Enums;
using Network.TCP;
using Network.UnityComponents;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Network.Samples.GroupChat
{
    public class ChatInfoSharer : MonoBehaviour
    {
        [SerializeField]
        private ChatSystem _chatSystem;

        private Coroutine ChatSharingCoroutine;

        public void UnityNetworkManagerInitialized(UnityNetworkManager unityNetworkManager)
        {
            if (unityNetworkManager.NetworkRole == NetworkRole.Server)
            {
                Debug.Log("Sharing started!!");
                KillCoroutine(ChatSharingCoroutine);
                ChatSharingCoroutine = StartCoroutine(ShareChat(unityNetworkManager));
            }
        }

        private IEnumerator ShareChat(UnityNetworkManager unityNetworkManager)
        {
            while (unityNetworkManager)
            {
                unityNetworkManager.SendDataPackage(CreateChatData());
                Debug.Log("Shared!");
                yield return new WaitForSeconds(0.5f);
            }
        }

        private DataPackage CreateChatData()
        {
            string jsonChatData = JsonUtility.ToJson(_chatSystem.ChatInfo);
            byte[] byteChatData = Encoding.UTF8.GetBytes(jsonChatData);

            DataPackage dataPackage = new DataPackage(byteChatData, DataType.Text);
            return dataPackage;
        }

        private void KillCoroutine(Coroutine coroutine)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }
    }
}