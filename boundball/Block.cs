using System;
using System.Drawing;
using System.Windows.Forms;

namespace boundball
{
    class Block
    {
        public enum Type : int { None, Block, BrokenBlock, JumpPad, UpSpike, DownSpike, LeftSpike, RightSpike, Flag, Save, CloseChest, OpenChest };

        int x, y, size, shape, returnShape;
        static string Path = "resource\\Image\\";
        static Image[] blockImage = new Image[]
        {
            Image.FromFile(Path+"block.png"),
            Image.FromFile(Path+"Broken Block.png"),
            Image.FromFile(Path+"Jump Block.png"),
            Image.FromFile(Path+"UpSpike.png"),
            Image.FromFile(Path+"DownSpike.png"),
            Image.FromFile(Path+"LeftSpike.png"),
            Image.FromFile(Path+"RightSpike.png"),
            Image.FromFile(Path+"Flag.png"),
            Image.FromFile(Path+"Save.png"),
            Image.FromFile(Path+"chest_closed.png"),
            Image.FromFile(Path+"chest_opened.png")
        };

        public int getX
        {
            get { return x; }
        }
        public int getY
        {
            get { return y; }
        }
        public int getSize
        {
            get { return size; }
        }
        public int Shape
        {
            get { return shape; }
            set { shape = value; }
        }

        

        public Block()
        {
            x = 0;
            y = 0;
            size = 0;
            shape = 0;
        }

        // 블록 수정함수
        public void setBlock(int _x, int _y, int _size, int _shape)
        {
            x = _x;
            y = _y;
            size = _size;
            shape = _shape;
            if (_shape == (int)Block.Type.BrokenBlock || 
                _shape == (int)Block.Type.CloseChest)
            {
                returnShape = _shape;
                RegenTimer();
            }
        }
        // 맵 그리기
        static public void drawMap(ref BufferedGraphics graphics, Block[,] block, int MHEIGHT, int MWIDTH, int map_x)
        {
            for (int y = 0; y < MHEIGHT; ++y)
            {
                for (int x = 0; x < MWIDTH; ++x)
                {
                    if (block[y, x].Shape != 0)
                    {
                        int Size = block[y, x].getSize;
                        graphics.Graphics.DrawImage(blockImage[block[y, x].Shape - 1], x * Size - map_x, y * Size, Size, Size);
                    }
                }
            }
        }

        // 부서지는 블록 재생
        System.Timers.Timer regenTimer;
        public void RegenTimer()
        {
            regenTimer = new System.Timers.Timer();
            regenTimer.Enabled = false;
            switch (returnShape)
            {
                case (int)Block.Type.BrokenBlock:
                    regenTimer.Interval = 3000;
                    break;
                case (int)Block.Type.CloseChest:
                    regenTimer.Interval = 5000;
                    break;
            }

            
            regenTimer.Elapsed += Regen_Tick;
        }
        public void RegenerationBlock()
        {
            if(regenTimer.Enabled == false) { 
                regenTimer.Enabled = true;
            }
        }
        private void Regen_Tick(object sender, EventArgs e)
        {
            shape = returnShape;
            regenTimer.Enabled = false;
        }
    }
}
