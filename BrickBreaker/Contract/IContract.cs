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
        void LoadBricks();
        void CloseGame();
        void Exit();
        void SendMessage(string nickname, string message);
        void PaintPlayer(int color_id);
        void Pause();
    }

    public enum Commands
    {
        Connect,
        Disconnect,
        Pause,
        SelectARoom,
        Left,
        Right,
        BallCoordinatesProcesing,
        LoadGame,
        LoadBricks,
        CloseGame,
        Exit,
        SendMessage,
        Paint,
        Goal,
        RemoveBrick,
        PaintPlayer
    }
}
