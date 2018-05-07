using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract
{
    public interface IContract
    {
        void Connect();
        void Disconnect();
        void SelectARoom(int index);
        void MoveLeft();
        void MoveRight();
        void BallCoordinatesProcesing();
        void LoadGame();
        void CloseGame();
        void Exit();
        void SendMessage(string message);
    }

    public enum Commands
    {
        Connect,
        Disconnect,
        SelectARoom,
        Left,
        Right,
        BallCoordinatesProcesing,
        LoadGame,
        CloseGame,
        Exit,
        SendMessage,
        Paint,
        Goal
    }
}
