using System;
using System.Drawing;
using System.Windows.Forms;

namespace boundball
{
    public partial class Form1 : Form
    {
        private const int MAX_USER = 8;
        private int FWIDTH, FHEIGHT;    //실제 크기(패널 크기)
        private int MWIDTH, MHEIGHT;    //맵 크기(블럭으로 나눠진 크기)
        private int blockSize, ballSize;
        private int transparent;
        private Block[,] block;
        private Ball ball;
        private AnimationText text;
        private FriendBall[] friend = new FriendBall[MAX_USER];
        private ClientSocket client;
        private Item item;

        BufferedGraphicsContext context;

        Image[] mainImage = new Image[5];

        // 생성자
        public Form1()
        {
            InitializeComponent();
            FWIDTH = game_panel.Width;
            FHEIGHT = game_panel.Height;
            MHEIGHT = 15;
            MWIDTH = 100;
            blockSize = FHEIGHT / 15;
            ballSize = FHEIGHT / 40;

            //더블 버퍼링
            context = BufferedGraphicsManager.Current; // 참조
            context.MaximumBuffer = game_panel.Size;

            //초기화
            Map map = new Map(MWIDTH, MHEIGHT, blockSize);
            text = new AnimationText(FWIDTH, FHEIGHT);
            block = map.InitializeMap(block);
            InitTitlePanel();
            InitializeBall();
            InitializeTimer();
            item = new Item(FWIDTH, FHEIGHT);

            //통신
            client = new ClientSocket(friend, ref block, game_panel);
            KeyPreview = true;
            transparent = 30;
            //이미지
            string Path = "resource\\Image\\";
            mainImage[0] = Image.FromFile(Path+"title.png");
            mainImage[1] = Image.FromFile(Path + "solo_basic.png");
            mainImage[2] = Image.FromFile(Path + "friend_basic.png");
            mainImage[3] = Image.FromFile(Path + "solo_hover.png");
            mainImage[4] = Image.FromFile(Path + "friend_hover.png");


        }
        // 시작화면
        // ===============================================================================
        int ButtonWidth, ButtonHeight;
        Point sButton, fButton;
        bool sHover, fHover;
        private void InitTitlePanel()
        {
            title_panel.Dock = DockStyle.Fill;
            ButtonWidth = FWIDTH / 6;
            ButtonHeight = FHEIGHT / 6;
            sButton = new Point(FWIDTH / 2 - ButtonWidth / 2, FHEIGHT / 10 * 5);
            fButton = new Point(FWIDTH / 2 - ButtonWidth / 2, FHEIGHT / 10 * 7);
            sHover = fHover = false;
        }
        private void title_panel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Image block_image = Image.FromFile("resource\\Image\\block.png");
            for (int y = 0; y < MHEIGHT; ++y)
            {
                for (int x = 0; x < MWIDTH; ++x)
                {
                    if (block[y, x].Shape == 1)
                    {
                        g.DrawImage(block_image, x * blockSize - Ball.map_x, y * blockSize, blockSize, blockSize);
                    }
                }
            }
            Image sImage, fImage;
            g.DrawImage(mainImage[0], FWIDTH / 3, FHEIGHT / 10, FWIDTH / 3, FHEIGHT / 3);
            sImage = sHover ? mainImage[3] : mainImage[1];
            fImage = fHover ? mainImage[4] : mainImage[2];
            g.DrawImage(sImage, sButton.X, sButton.Y, ButtonWidth, ButtonHeight);
            g.DrawImage(fImage, fButton.X, fButton.Y, ButtonWidth, ButtonHeight);
        }
        private bool soloButtonArea(MouseEventArgs e)
        {
            Point Mouse = new Point(e.X, e.Y);
            if (Mouse.X > sButton.X && Mouse.X < sButton.X + ButtonWidth
                && Mouse.Y > sButton.Y && Mouse.Y < sButton.Y + ButtonHeight)
                return true;
            return false;
        }
        private bool friendButtonArea(MouseEventArgs e)
        {
            Point Mouse = new Point(e.X, e.Y);
            if (Mouse.X > sButton.X && Mouse.X < sButton.X + ButtonWidth
                && Mouse.Y > fButton.Y && Mouse.Y < fButton.Y + ButtonHeight)
                return true;
            return false;
        }
        private void title_panel_MouseMove(object sender, MouseEventArgs e)
        {
            //마우스가 버튼 위에 오버되었을때
            Point Mouse = new Point(e.X, e.Y);
            sHover = soloButtonArea(e) ? true : false;      //혼자하기
            fHover = friendButtonArea(e) ? true : false;    //같이하기
            Refresh();
        }
        private void title_panel_MouseClick(object sender, MouseEventArgs e)
        {
            Point Mouse = new Point(e.X, e.Y);
            //혼자하기 버튼
            if (soloButtonArea(e))
            {
                title_panel.Visible = false;
                tm.Enabled = true;
            }
            //같이하기 버튼
            else if (friendButtonArea(e))
            {

                try
                {
                    text.Loading(load);
                    text.drawConnectWindow(connect_panel, ConnectText, "연결시도중..");
                    client.StartConnection();
                    text.drawConnectWindow(connect_panel, ConnectText, "연결완료");
                    title_panel.Visible = false;
                    ConnectText.Visible = false;
                    connect_panel.Visible = false;
                    tm.Enabled = true;

                }
                catch (Exception connectError)
                {
                    text.drawConnectWindow(connect_panel, ConnectText, "서버와 연결할 수 없습니다.");
                    ConnectText.Visible = false;
                    connect_panel.Visible = false;
                }
            }
        }
        private void ConnectPanelPaint(object sender, PaintEventArgs e)
        {
            BufferedGraphics graphics;
            graphics = context.Allocate(CreateGraphics(), new Rectangle(0, 0, connect_panel.Width, connect_panel.Height));
            graphics.Graphics.Clear(Color.FromArgb(255, Color.Ivory));   // 버퍼 지우기
            graphics.Graphics.DrawImage(Image.FromFile("resource\\Image\\wait.png"), new Rectangle(0, 0, connect_panel.Width, connect_panel.Height));
            graphics.Render(e.Graphics);
        }
    
        // ===============================================================================



        // 공 초기화
        private void InitializeBall()
        {
            Ball.FHEIGHT = FHEIGHT;
            Ball.FWIDTH = FWIDTH;
            Ball.MHEIGHT = MHEIGHT;
            Ball.MWIDTH = MWIDTH;
            Ball.blockSize = blockSize;
            Ball.map_x = 0;
            FriendBall.MAX_USER = MAX_USER;

            int initBallX = blockSize * 5 - ballSize / 2;
            int initBallY = blockSize * 12 - ballSize / 2;
            int ballSpeed = 3;
            ball = new Ball(initBallX, initBallY, ballSpeed, ballSize / 2, null);
            for (int i = 0; i < MAX_USER; ++i)
            {
                friend[i] = new FriendBall(-100, -100, ballSize/2, null);
            }
            ball.real_x = ball.x;
        }

        // 그리기
        private void panel1_Paint_1(object sender, PaintEventArgs e)
        {
            BufferedGraphics graphics;
            graphics = context.Allocate(CreateGraphics(), new Rectangle(0, 0, FWIDTH, FHEIGHT));
            graphics.Graphics.Clear(Color.FromArgb(255,Color.Ivory));   // 버퍼 지우기

            // 버퍼에 그림 그리기
            Block.drawMap(ref graphics, block, MHEIGHT, MWIDTH, Ball.map_x);
            ball.drawBall(ref graphics);
            friend[0].drawBall(ref graphics, friend, block);
            text.drawSaveText(ref graphics, ball, ref saveTimer);
            text.drawVictoryText(ref graphics, ball, friend);
            item.drawItemSlot(ref graphics);
            if (ball.CheckVictory(friend) != 0){
                ball.ballLeft = false;
                ball.ballRight = false;
                ball.ballUp = false;
                ball.ballDown = false;
            }
            if (!ball.live)
            {
                ball.ballLeft = false;
                ball.ballRight = false;
                ball.ballUp = false;
                ball.ballDown = false;
                graphics.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(transparent, Color.Black)), new Rectangle(0, 0, FWIDTH, FHEIGHT));
            }

            graphics.Render(e.Graphics);    // 버퍼에 있는걸 화면에 출력
        }



        //키 입력 처리
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!ball.live || ball.CheckVictory(friend) != 0) return;
            if (e.KeyCode.Equals(Keys.Right))
            {
                ball.ballRight = true;
            }
            else if (e.KeyCode.Equals(Keys.Left))
            {
                ball.ballLeft = true;
            }
            else if (e.KeyCode.Equals(Keys.Up) && ball.fly)
            {
                ball.ballUp = true;
            }
            else if (e.KeyCode.Equals(Keys.Down) && ball.fly)
            {
                ball.ballDown = true;
            }

            if (e.KeyCode.Equals(Keys.X))  // 아이템 선택바꾸기
            {
                item.select = (item.select + 1) % 2;
            }
            if (e.KeyCode.Equals(Keys.Z)) // 아이템 사용
            {
                int itemType = item.UseItem();
                switch (itemType)
                {
                    case 0:
                        break;
                    case 1:
                        item.BoostItem(ref ball);
                        break;
                    case 2:
                        item.DoubleJumpItem(ref ball);
                        break;
                    case 3:
                        item.FlyItem(ref ball);
                        break;
                }
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Right))
                ball.ballRight = false;
            else if (e.KeyCode.Equals(Keys.Left))
                ball.ballLeft = false;
            else if (e.KeyCode.Equals(Keys.Up))
                ball.ballUp = false;
            else if (e.KeyCode.Equals(Keys.Down))
                ball.ballDown = false;
        }


        //공 튀기기 모션
        // ===============================================================================
        System.Windows.Forms.Timer tm;
        System.Windows.Forms.Timer testTimer;
        bool saveTimer;

        void InitializeTimer()
        {
            tm = new System.Windows.Forms.Timer();
            tm.Enabled = false;
            tm.Tick += new EventHandler(tm_Tick);
            tm.Interval = 15;
            saveTimer = false;
        }
        void turnTransParent()
        {
            transparent = (transparent + 1)%255; 
        }
         void saveTitle()
        {
            if (text.title_pos_x > FWIDTH / 2 - 150)
            {
                text.title_pos_x -= 30;
            }
            else if (text.count > 0)
            {
                text.title_pos_x = FWIDTH / 2 - 150;
                --text.count;
            }
            else if (text.count <= 0)
            {
                ball.saved = false;
                saveTimer = false;
            }
        }
        int T = 10;
        Point blockPoint = new Point(0, 0);
        Point p = new Point(0, 0);
        void tm_Tick(object sender, EventArgs e)
        {
            ball.ball_move();
            if (ball.live) { 
            ball.y += Convert.ToInt32(ball.y_); // 가속도 + 위치
            if (ball.y_ < ball.jumpSize)              // 가속도가 매우 커질 시 벽을 뚫고 들어감
                ball.y_ += ball.accel_y;              // 가속도를 증가
            }
            if (ball.y > FHEIGHT)
            {
                ball.x = 3000;
                ball.y = 10;
                ball.user_die(blockSize);
            }
            blockPoint = ball.CheckCollosion(block);
            if (blockPoint.X > 0)
            {
                p = blockPoint;
            }

            if (p.X > 0 && T > 0)
            {
                client.sendProcessing(ball, p);
                T--;
            }
            else
            {
                client.sendProcessing(ball, new Point(0, 0));
                T = 20;
                p = new Point(0, 0);
            }
            //죽을때 투명도 처리
            if (!ball.live)
            {
                turnTransParent();
            }
            else if(transparent != 0)
            {
                transparent = 0;
            }
            ///////////////////
            //세이브타이머 처리
            if(saveTimer) { 
                saveTitle();
            }
            Refresh();
        }
        // ===============================================================================
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            client.AbortSocket();
            
        }
        private void panel1_SizeChanged(object sender, EventArgs e)
        {
            FHEIGHT = this.Height;
            FWIDTH = this.Width;
            blockSize = FHEIGHT / 15;
            this.Invalidate();
        }
    }
    // 더블버퍼링
    public class DoubleBufPanel : Panel
    {
        public DoubleBufPanel()
        {
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.UpdateStyles();
        }
    }
}


