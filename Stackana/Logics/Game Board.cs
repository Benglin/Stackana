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
using System.Collections.Generic;

namespace Xleeque.Stackana.Logics
{
    public class GameBlock
    {
        int type;
        Panel panel;
        public TextBlock text;
        public Rectangle rect;

        public GameBlock(Panel panel, int type, double size)
        {
            this.type = type;
            this.panel = panel;

            this.text = new TextBlock();
            this.text.Width = size;
            this.text.Height = size;
            this.text.TextAlignment = TextAlignment.Center;

            this.rect = new Rectangle();
            this.rect.Width = size;
            this.rect.Height = size;
            this.rect.Fill = new SolidColorBrush(Colors.Blue);
        }

        public void SetPosition(double x, double y)
        {
            this.text.SetValue(Canvas.LeftProperty, x);
            this.text.SetValue(Canvas.TopProperty, y);
            this.rect.SetValue(Canvas.LeftProperty, x);
            this.rect.SetValue(Canvas.TopProperty, y);
        }

        public void Nullify()
        {
            this.type = -1;
            this.text.Text = "";
            this.rect.Fill = new SolidColorBrush(Colors.Blue);
        }

        public void CopyFrom(GameBlock other)
        {
            if (null != other)
            {
                this.type = other.Type;
                this.rect.Fill = other.rect.Fill;
                this.text.Text = other.text.Text;
            }
        }

        public TextBlock Text
        {
            get { return text; }
        }

        public int Type
        {
            get { return this.type; }
            set { this.type = value; }
        }

        public bool Occupied
        {
            get { return (type != -1); }
        }
    }

    public class GameBoard : GameObject
    {
        // General configurations.
        double width, height, size;
        GameBlock[,] blocks;
        List<GameBlock> good_blocks;

        double drop_delta = 1000.0;
        double state_delta = 500.0;

        // Falling block properties.
        int x_fall, y_fall;
        double last_drop_time = 0.0;
        double last_state_time = 0.0;
        GameBlock falling = null;
        BoardState board_states;

        enum BoardState
        {
            None = 0x00,
            Highlight = 0x01,
            Holding = 0x02
        }

        public GameBoard(GameObject parentObject, Canvas parentCanvas)
            : base(parentObject, parentCanvas)
        {
            board_states = BoardState.None;
        }

        public void Initialize(int width, int height, int size)
        {
            if (null != blocks)
                CleanUpInternal();

            this.width = width;
            this.height = height;
            this.size = size;
            blocks = new GameBlock[height, width];
            good_blocks = new List<GameBlock>();

            double offset = size + Configuration.block_gap;
            double left_size = ParentCanvas.ActualWidth;
            left_size = (left_size - (width * offset)) * 0.5;
            double y_coord = ParentCanvas.ActualHeight - offset;
            for (int y = 0; y < height; y++)
            {
                double x_coord = left_size;
                for (int x = 0; x < width; x++)
                {
                    blocks[y, x] = new GameBlock(ParentCanvas, -1, this.size);
                    GameBlock block = blocks[y, x];
                    block.SetPosition(x_coord, y_coord);
                    ParentCanvas.Children.Add(block.rect);
                    ParentCanvas.Children.Add(block.text);
                    x_coord = x_coord + offset;
                }

                y_coord = y_coord - offset;
            }

            GenerateNextBlock(); // Get the block ready.
        }

        public override void StartUp()
        {
        }

        public override void ShutDown()
        {
            CleanUpInternal();
        }

        public override void UpdateFrame(double deltaTime)
        {
            if (board_states == BoardState.None)
            {
                last_drop_time += deltaTime;
                if (last_drop_time > drop_delta)
                {
                    if (!AdvanceBlock(true)) // If cannot advance anymore.
                    {
                        ClearGoodCombinations();
                        if (board_states != BoardState.Highlight)
                            GenerateNextBlock(); // Nothing changed.
                    }
                }
            }
            else
            {
                last_state_time += deltaTime;
                if (last_state_time < state_delta)
                    return;

                last_state_time = 0.0;
                if (board_states == BoardState.Holding)
                {
                    board_states = BoardState.None;
                    ClearGoodCombinations();
                    if (board_states == BoardState.None)
                        GenerateNextBlock();
                }
                else if (board_states == BoardState.Highlight)
                {
                    board_states = BoardState.None;
                    CompactGameBoard();
                }
            }
        }

        public override void ProcessKeyInput(Key key, bool keyDown)
        {
            bool block_position_updated = false;
            if (false == keyDown) // Key up event.
            {
                switch (key)
                {
                    case Key.Left:
                        x_fall = x_fall - 1;
                        if (x_fall < 0)
                            x_fall = 0;
                        else
                            block_position_updated = true;
                        break;

                    case Key.Right:
                        x_fall = x_fall + 1;
                        if (x_fall >= this.width)
                            x_fall = ((int)(this.width - 1.0));
                        else
                            block_position_updated = true;
                        break;

                    case Key.Down:
                        block_position_updated = AdvanceBlock(false);
                        break;
                }
            }

            if (block_position_updated)
                PositionBlock();
        }

        private void CleanUpInternal()
        {
            good_blocks.Clear();
            good_blocks = null;

            for (int y = 0; y < this.height; y++)
            {
                for (int x = 0; x < this.width; x++)
                {
                    ParentCanvas.Children.Remove(blocks[y, x].rect);
                    ParentCanvas.Children.Remove(blocks[y, x].text);
                    blocks[y, x].Nullify();
                    blocks[y, x].text = null;
                    blocks[y, x].rect = null;
                    blocks[y, x] = null;
                }
            }

            blocks = null; // Destroy blocks.
            falling = null;
        }

        private void GenerateNextBlock()
        {
            if (null != falling)
                return;

            Color[] predefined = new Color[13] { Colors.Black,
                Colors.Brown, Colors.Cyan, Colors.DarkGray, Colors.Gray,
                Colors.Green, Colors.LightGray, Colors.Magenta, Colors.Orange,
                Colors.Purple, Colors.Red, Colors.White, Colors.Yellow };

            x_fall = ((int)(this.width * 0.5));
            y_fall = ((int)(this.height - 1.0));
            falling = new GameBlock(ParentCanvas, Utils.RndGen(0, 12), this.size);
            falling.rect.Fill = new SolidColorBrush(predefined[falling.Type]);
            falling.text.Text = "AB";

            PositionBlock();
            ParentCanvas.Children.Add(falling.rect);
            ParentCanvas.Children.Add(falling.text);
            last_drop_time = 0.0;
        }

        private void PositionBlock()
        {
            double offset = size + Configuration.block_gap;
            double left_size = ParentCanvas.ActualWidth;
            left_size = (left_size - (width * offset)) * 0.5;

            double left = left_size + (x_fall * (this.size + Configuration.block_gap));
            double top = ParentCanvas.ActualHeight;

            top -= ((y_fall + 1) * (this.size + Configuration.block_gap));
            falling.SetPosition(left, top);
        }

        private bool AdvanceBlock(bool forceCommit)
        {
            bool dead_end = (y_fall == 0);
            if (false == dead_end)
            {
                int next_y = y_fall - 1;
                dead_end = (blocks[next_y, x_fall].Occupied);
            }

            if (false == forceCommit)
            {
                if (false == dead_end)
                    y_fall = y_fall - 1;

                last_drop_time = 0.0;
                return !dead_end;
            }

            if (false == dead_end)
            {
                y_fall = y_fall - 1;
                PositionBlock();
            }
            else // Reached a dead-end now.
            {
                blocks[y_fall, x_fall].CopyFrom(falling);
                ParentCanvas.Children.Remove(falling.rect);
                ParentCanvas.Children.Remove(falling.text);
                falling.Nullify();
                falling.rect = null;
                falling.text = null;
                falling = null;
            }

            last_drop_time = 0.0;
            return (!dead_end);
        }

        private void CompactGameBoard()
        {
            board_states = BoardState.None;
            foreach (GameBlock block in good_blocks)
                block.Nullify();

            for (int x = 0; x < this.width; x++)
            {
                for (int y = 0; y < this.height - 1; y++)
                {
                    if (blocks[y, x].Occupied)
                        continue;

                    blocks[y, x].CopyFrom(blocks[y + 1, x]);
                    blocks[y + 1, x].Nullify();
                }
            }

            good_blocks.Clear();
            board_states = BoardState.Holding;
        }

        private void ClearGoodCombinations()
        {
            last_state_time = 0.0;
            board_states = BoardState.None;

            if (!GetGoodBlocks())
                return; // No good combination.

            board_states = BoardState.Highlight;
            foreach (GameBlock block in good_blocks)
                block.rect.Fill = new SolidColorBrush(Colors.White);
        }

        private bool GetGoodBlocks()
        {
            int last = ((int)(this.width - 1));
            good_blocks.Clear(); // Empty out incoming list.

            for (int y = 0; y < this.height - 1; y++)
            {
                bool row_occupied = false;
                for (int x = 0; x < this.width - 1; x++)
                {
                    if (blocks[y, x].Occupied == false)
                        continue;

                    row_occupied = true; // Not an empty row...

                    // Check matching blocks horizontally.
                    if (blocks[y, x].Type == blocks[y, x + 1].Type)
                    {
                        good_blocks.Add(blocks[y, x]);
                        good_blocks.Add(blocks[y, x + 1]);
                    }

                    // Check matching blocks vertically.
                    if (blocks[y, x].Type == blocks[y + 1, x].Type)
                    {
                        good_blocks.Add(blocks[y, x]);
                        good_blocks.Add(blocks[y + 1, x]);
                    }
                }

                if (blocks[y, last].Type == blocks[y + 1, last].Type)
                {
                    if (blocks[y, last].Occupied)
                    {
                        good_blocks.Add(blocks[y, last]);
                        good_blocks.Add(blocks[y + 1, last]);
                    }
                }

                if (false == row_occupied)
                {
                    // If the row is not occupied so far, check the last block.
                    if (blocks[y, last].Occupied == false)
                        break; // No point checking anymore, nothing else on top.
                }
            }

            return (good_blocks.Count > 0);
        }
    }
}
