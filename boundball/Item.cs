
using System;
using System.Drawing;

namespace boundball
{
    class Item
    {
        public enum Type : int{ None, Boost, DoubleJump, Fly};
        static string Path = "resource\\Image\\";

        static Image[] Slot = new Image[]
        {
            Image.FromFile(Path+"ItemSlot.png"),
            Image.FromFile(Path+"ItemSelect.png")
        };
        static Image[] ItemType = new Image[]
        {
            Image.FromFile(Path+"Boost.png"),
            Image.FromFile(Path+"DoubleJump.png"),
            Image.FromFile(Path+"Fly.png")
        };
        public static int[] itemList;

        static int FWIDTH;
        static int FHEIGHT;
        bool[] used = new bool[3];
        Ball ball;

        public int select = 0;
        public Item(int _FWIDTH, int _FHEIGHT)
        {
            itemList = new int[] { (int)Type.None, (int)Type.None};
            FWIDTH = _FWIDTH;
            FHEIGHT = _FHEIGHT;
        }

        public void drawItemSlot(ref BufferedGraphics graphics)
        {
            graphics.Graphics.DrawImage(Slot[0], FWIDTH - 150, FHEIGHT - 100, 50, 50);
            graphics.Graphics.DrawImage(Slot[0], FWIDTH - 100, FHEIGHT - 100, 50, 50);
            graphics.Graphics.DrawImage(Slot[1], FWIDTH - 150 + (select * 50), FHEIGHT - 100, 50, 50);
            for (int i = 0; i < itemList.Length; ++i)
            {
                if (itemList[i] != (int)Type.None)
                {
                    graphics.Graphics.DrawImage(ItemType[itemList[i] - 1], FWIDTH - 140 + (i * 50), FHEIGHT - 90, 30, 30);
                }
            }
        }
        public int UseItem()       // 아이템 사용
        {
            return itemList[select];
        }
        private void UsedItem()
        {
            itemList[select] = (int)Type.None;
        }
        public void DeleteEffect()
        {
            if (BoostTimer != null) BoostTimer.Interval = 1;
            if (flyEndTimer != null) flyEndTimer.Interval = 1;
        }

        // Boost Item
        System.Windows.Forms.Timer BoostTimer;
        public void BoostItem(ref Ball _ball)
        {
            if (used[(int)Item.Type.Boost - 1]) return;
            used[(int)Item.Type.Boost - 1] = true;
            ball = _ball;
            Sound sound = new Sound();
            sound.SoundPlay((int)Sound.Index.Boost);
            ball.speed = 5;
            UsedItem();

            BoostTimer = new System.Windows.Forms.Timer();
            BoostTimer.Enabled = true;
            BoostTimer.Interval = 8000;
            BoostTimer.Tick += new EventHandler(Boost_Tick);
        }
        private void Boost_Tick(object sender, EventArgs e)
        {
            BoostTimer.Enabled = false;
            used[(int)Item.Type.Boost - 1] = false;
            ball.speed = 3;
        }


        // DoubleJump Item
        public void DoubleJumpItem(ref Ball ball)
        {
            Sound sound = new Sound();
            sound.SoundPlay((int)Sound.Index.JumpPad);
            ball.Ball_Jump();
            UsedItem();
        }



        //  Fly Item
        System.Windows.Forms.Timer flyTimer;
        System.Windows.Forms.Timer flyEndTimer;
        public void FlyItem(ref Ball _ball)
        {
            if (used[(int)Item.Type.Fly - 1]) return;
            used[(int)Item.Type.Fly - 1] = true;

            ball = _ball;
            ball.y_ = 0;
            ball.accel_y = 0;
            ball.fly = true;

            flyTimer = new System.Windows.Forms.Timer();
            flyTimer.Enabled = true;
            flyTimer.Interval = 1000;
            flyTimer.Tick += new EventHandler(fly_Tick);

            flyEndTimer = new System.Windows.Forms.Timer();
            flyEndTimer.Enabled = true;
            flyEndTimer.Interval = 5000;
            flyEndTimer.Tick += new EventHandler(flyEnd_Tick);

            Sound sound = new Sound();
            sound.SoundPlay((int)Sound.Index.Fly);
            UsedItem();
        }
        private void fly_Tick(object sender, EventArgs e)
        {
            Sound sound = new Sound();
            sound.SoundPlay((int)Sound.Index.Fly);
        }
        private void flyEnd_Tick(object sender, EventArgs e)
        {
            ball.accel_y = 0.2;
            used[(int)Item.Type.Fly - 1] = false;
            ball.fly = false;
            flyTimer.Enabled = false;
            flyEndTimer.Enabled = false;
        }

    }
}
