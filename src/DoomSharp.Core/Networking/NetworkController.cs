namespace DoomSharp.Core.Networking;

public class NetworkController
{
    public DoomCommunication DoomCom { get; } = new();

    public void Initialize()
    {
        DoomCom.TicDup = 1;
        DoomCom.ExtraTics = 0;
        
        // Single player support only
        DoomGame.Instance.Game.NetGame = false;
        DoomCom.Id = Constants.DoomComId;
        DoomCom.NumPlayers = DoomCom.NumNodes = 1;
        DoomCom.DeathMatch = 0;
        DoomCom.ConsolePlayer = 0;
    }

    public void NetworkCommand()
    {

    }
}