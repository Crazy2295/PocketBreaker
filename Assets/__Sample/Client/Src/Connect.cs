using UnityEngine;
using socket.io;

namespace Sample {

    /// <summary>
    /// The basic sample to show how to connect to a server
    /// </summary>
    public class Connect : MonoBehaviour
    {
        private GlobalStore _globalStore;

        private void Awake()
        {
            _globalStore = GameObject.FindObjectOfType<GlobalStore>();
        }

//        void Start() {
//            var serverUrl = "http://localhost:5000/";
//            socket = Socket.Connect(serverUrl);
//            
//            socket.On("boop", (string data) => {
//                Debug.Log("BOOP! " + data);
//                socket.Emit("beep", "sosi");
//            });
//            
//            socket.On("takeIt", (string data) =>
//            {
//                Debug.Log("take this! " + data);
//            });
//            
//            socket.On(SystemEvents.connect, () => {
//                Debug.Log("Hello, Socket.io~");
//            });
//
//            socket.On(SystemEvents.reconnect, (int reconnectAttempt) => {
//                Debug.Log("Hello, Again! " + reconnectAttempt);
//            });
//
//            socket.On(SystemEvents.disconnect, () => {
//                Debug.Log("Bye~");
//            });
//        }

        private void Update()
        {
            if (_globalStore.SocketSet)
            {
                _globalStore.socket.Emit("beep", "sosi");
            }
        }
    }

}
