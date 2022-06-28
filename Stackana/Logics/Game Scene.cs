using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Xleeque.Stackana.Logics
{
    public class GameScene : GameObject
    {
        private GameBoard game_board;

        public GameScene(Canvas parentCanvas)
            : base(null, parentCanvas)
        {
        }

        public override void StartUp()
        {
            game_board = new GameBoard(this, this.ParentCanvas);
            game_board.StartUp();
            game_board.Initialize(8, 10, 20);
            this.Children.Add(game_board);
        }

        public override void ShutDown()
        {
            this.Children.Remove(game_board);
            game_board.ShutDown();
            game_board = null;
        }

        public override void UpdateFrame(double deltaTime)
        {
            if (null != game_board)
                game_board.UpdateFrame(deltaTime);
        }

        public override void ProcessKeyInput(Key key, bool keyDown)
        {
            if (null != game_board)
                game_board.ProcessKeyInput(key, keyDown);
        }
    }
}
