using System;
using Newtonsoft.Json;
using socket.io;
using UnityEngine;

public class BattleHandlers: MonoBehaviour
{
    public Action<MoveResult> MoveGotResult { get; set; }
    public void GetMoveResult(Socket socket)
    {
        socket.On("move result", 
            json => { MoveGotResult(JsonConvert.DeserializeObject<MoveResult>(json)); });   
    }
    
    public Action<UnitModel> BattleInvited { get; set; }

    public void GetBattleInvite(Socket socket)
    {
        socket.On("battle invited",
            json => BattleInvited(JsonConvert.DeserializeObject<UnitModel>(json)));
    }
    
    public Action<MoveResult> EndBattle { get; set; }

    public void BattleEnded(Socket socket)
    {
        socket.On("end battle", 
            json => EndBattle(JsonConvert.DeserializeObject<MoveResult>(json)));
    }

    public void ActivateAll(Socket socket)
    {
        GetMoveResult(socket);
        GetBattleInvite(socket);
        BattleEnded(socket);
    }
}