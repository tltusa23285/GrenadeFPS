using UnityEngine;
using FishNet;

namespace Game
{
    public class QuickConnectMenu : MonoBehaviour
    {
        public void StartServer()
        {
            InstanceFinder.ServerManager.StartConnection();
        }
        public void StartHost()
        {
            InstanceFinder.ServerManager.StartConnection();
            InstanceFinder.ClientManager.StartConnection();
        }
        public void StartClient()
        {
            InstanceFinder.ClientManager.StartConnection();
        }
    }
}
