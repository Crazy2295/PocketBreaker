//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Quobject.SocketIoClientDotNet.Client;
//
//public class SocketManager : MonoBehaviour
//{
//    Socket socket = IO.Socket("http://127.0.0.1:5000/sessions/test");
//    
//    // Start is called before the first frame update
//    void Start()
//    {
//        Debug.Log("start");
//
//        socket.On(Socket.EVENT_CONNECT, () =>
//        {
//            Debug.Log("emit");
//
//            socket.Emit("my event", "{data: 'some data'}");
//        });
//
//        socket.On("my response", (data) =>
//        {
//            Debug.Log("my response function");
//
//            Debug.Log(data);
//
//            if (data.ToString() != "my response")
//            {
//                socket.Disconnect();
//                Debug.Log("Disconnect. Socket: " + socket);
//            }
//            
//        });
//
//
//    }
//
//    // Update is called once per frame
//    void Update()
//    {
//    }
//}