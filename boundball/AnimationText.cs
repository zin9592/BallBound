using System;
using System.Drawing;
using System.Windows.Forms;

namespace boundball
{
    class AnimationText
    {
        enum Index : int { Victory, Defeat, Save };
        static string Path = "resource\\Image\\";
        static Image[] TextImage = new Image[] {
            Image.FromFile(Path+"Victory.png"),
            Image.FromFile(Path+"Defeat.png"),
            Image.FromFile(Path+"SaveTitle.png")
        };
        public static string[] loadingText = new string[]
        {
            ".",
            "..",
            "...",
            "....",
            "....."
        };
        Label loading;
        int FWIDTH, FHEIGHT;
        public int title_pos_x;
        public int title_pos_y;
        public int count;

        public AnimationText(int _FWIDTH, int _FHEIGHT)
        {
            FWIDTH = _FWIDTH;
            FHEIGHT = _FHEIGHT;
            count = 0;
        }
        private static DateTime Delay(int MS)
        {
            DateTime ThisMoment = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, MS);
            DateTime AfterWards = ThisMoment.Add(duration);

            while (AfterWards >= ThisMoment)
            {
                System.Windows.Forms.Application.DoEvents();
                ThisMoment = DateTime.Now;
            }

            return DateTime.Now;
        }


        public void drawSaveText(ref BufferedGraphics graphics, Ball ball, ref bool saveTimer)
        {
            if (ball.saved)
            {
                // 타이머 값 초기화
                if (saveTimer == false)
                {
                    saveTimer = true;
                    title_pos_x = FWIDTH;
                    title_pos_y = FHEIGHT / 4;
                    count = 30;
                }
                graphics.Graphics.DrawImage(TextImage[(int)Index.Save], title_pos_x, title_pos_y, 300, 100);
            }
        }
        public void drawVictoryText(ref BufferedGraphics graphics, Ball ball, FriendBall[] friend)
        {
            int victory = ball.CheckVictory(friend);
            if (victory == 1)
            {
                graphics.Graphics.DrawImage(TextImage[(int)Index.Victory], FWIDTH / 2 - 150, FHEIGHT / 4, 300, 100);
            }
            else if (victory == 2)
            {
                graphics.Graphics.DrawImage(TextImage[(int)Index.Defeat], FWIDTH / 2 - 150, FHEIGHT / 4, 300, 100);
            }
        }
        public void drawConnectWindow(Panel connect_panel, Label ConnectText, string text)
        {
            if (text != "연결시도중..")
            {
                loading.Visible = false;
                loadingTimer.Enabled = false;
            }


            connect_panel.Location = new Point(FWIDTH / 2 - connect_panel.Width / 2, FHEIGHT / 2 - connect_panel.Height / 2);
            ConnectText.Text = text;
            ConnectText.Location = new Point(FWIDTH / 2 - ConnectText.Width / 2, FHEIGHT / 3 + ConnectText.Height / 2);
            connect_panel.Refresh();
            ConnectText.Visible = true;
            connect_panel.Visible = true;

            Delay(3000);
        }


        System.Windows.Forms.Timer loadingTimer;
        public void Loading(Label b)
        {
            loading = b;
            loadingTimer = new System.Windows.Forms.Timer();
            loadingTimer.Enabled = true;
            loadingTimer.Tick += new EventHandler(Load_Tick);
            loadingTimer.Interval = 100;
            loading.Visible = true;
        }
        void Load_Tick(object sender, EventArgs e)
        {
            loading.Text = loadingText[count];
            count = (count + 1) % 5;
            loading.Location = new Point(FWIDTH / 2 - loading.Width / 2, 3 * FHEIGHT / 5 - loading.Height / 2);

        }
    }
}
