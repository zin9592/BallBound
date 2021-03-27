using System;
using System.Drawing;

namespace boundball
{
    class Ball
    {
        static public int FWIDTH, FHEIGHT;    //실제 크기(패널 크기)
        static public int MWIDTH, MHEIGHT;    //맵 크기(블럭으로 나눠진 크기)
        static public int blockSize;
        public bool live = true;
        public bool saved = false;
        public bool fly = false;
        private Point SavePoint;
        public bool victory = false;
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
        

        public int x { get; set; }
        public int y { get; set; }
        static public int map_x { get; set; }
        public int real_x { get; set; }
        public int radius { get; set; }
        public int speed { get; set; }
        public int rotation { get; set; }
        public int jumpSize { get; }
        public Image image { get; set; }

        public double x_ { get; set; }          // x축 속도
        public double y_ { get; set; }          // y축 속도
        public double accel_x { get; set; }     // x축 가속도
        public double accel_y { get; set; }     // y축 가속도
        public bool ballLeft { get; set; }
        public bool ballRight { get; set; }
        public bool ballUp { get; set; }
        public bool ballDown { get; set; }


        // ball의 생성자
        public Ball()
        {

        }
        public Ball(int _x, int _y, int _speed, int _radius, Image _image)
        {
            x = _x;
            y = _y;
            speed = _speed;
            radius = _radius;
            rotation = 0;
            jumpSize = 7;
            image = _image;
            accel_x = 0.2;
            accel_y = 0.2;
            SavePoint.X = _x;
            SavePoint.Y = _y;
        }

        // 공의 움직임
        // =================================================================================== 
        public void ball_move()
        {
            real_x = x + map_x;    //실제 x좌표
            Lball_move();
            Rball_move();
            exception_move();
            if (fly) fly_move();
        }
        private void exception_move()
        {
            //좌우키를 동시에 누름
            if (ballRight && ballLeft)
            {
                x_ = 0;
            }
            //좌우키 둘다 누르지 않음
            if (!ballRight && !ballLeft)
            {
                if (x_ > 0)
                    x_ -= accel_x;
                else if (x_ < 0)
                    x_ += accel_x;
            }
        }
        private void Lball_move()
        {
            if (ballLeft)
            {
                if (x_ > -speed)
                    x_ -= accel_x;
                //ball의 x위치가 패널 너비 4분의 1보다 클때 공을 움직임
                if (x > (FWIDTH / 4))
                    x += Convert.ToInt32(x_);
                //맵을 움직임
                else
                    map_x += Convert.ToInt32(x_);
            }
        }
        private void Rball_move()
        {
            if (ballRight)
            {
                if (x_ < speed)
                    x_ += accel_x;
                //ball의 x위치가 패널 너비 4분의 3보다 작을때 공을 움직임
                if (x < (FWIDTH / 4) * 3)
                    x += Convert.ToInt32(x_);
                //맵을 움직임
                else
                    map_x += Convert.ToInt32(x_);
            }
        }
        private void fly_move()
        {
            if (ballUp)
            {
                y -= 2;
            }
            if (ballDown)
            {
                y += 2;
            }
        }
        // =================================================================================== 

        // 공 그리기
        public void drawBall(ref BufferedGraphics graphics)
        {
            graphics.Graphics.FillEllipse(Brushes.Orange, x - radius, y - radius, radius * 2, radius * 2);
        }

        // 공의 점프
        public void Ball_Jump(int _jump = 7)
        {
            y_ = _jump;        //기본 점프 크기
            y_ *= -1;          //운동에너지 방향을 바꾼다.
        }

        //공이 죽었을경우 초기값으로 돌아감
        public void user_delay()
        {
            x = 3000;
            y = 10;
        }

        public void user_die(int blockSize)
        {
            Sound sound = new Sound();
            sound.SoundPlay((int)Sound.Index.Died);
            if(live == true) { 
            live = false;
            Delay(2000);
            x = blockSize * 5 - radius;
            y = SavePoint.Y - blockSize;//blockSize * 12 - radius;
            map_x = SavePoint.X - x;
            live = true;
            sound.SoundPlay((int)Sound.Index.Revive);
            }
            else
            {
                return;
            }
        }

        //공 위치 저장
        private void user_save(Block block)
        {
            SavePoint.X = block.getX + block.getSize / 2;
            SavePoint.Y = block.getY + block.getSize / 2;
            saved = true;
        }

        // 공 충돌 메소드
        // =================================================================================== 
        // 공 충돌검사
        public Point CheckCollosion(Block[,] block)
        {
            bool check = false;
            Point tempBlock;
            Point user_pos = new Point();
            Sound sound = new Sound();
            user_pos.X = (x + map_x) / blockSize;
            user_pos.Y = y / blockSize;
            checkSpikeCollision(user_pos, block);
            tempBlock = checkSideCollision(user_pos, block, sound);
            if(tempBlock == new Point(0,0)) { 
                tempBlock = checkEdgeCollision(user_pos, block, sound);
            }
            else if(tempBlock == new Point(-1,-1))
            {
                tempBlock = new Point(0, 0);
            }
            return tempBlock;
        }
        // 가시 검사
        private bool checkSpikeCollision(Point user_pos, Block[,] block)
        {
            int block_pos_y = user_pos.Y;
            int block_pos_x = user_pos.X;
            if (block_pos_y > -1 && block_pos_x > -1 && block_pos_y < MHEIGHT && block_pos_x < MWIDTH)
            {
                if (block[block_pos_y, block_pos_x].Shape == (int)Block.Type.UpSpike ||
                   block[block_pos_y, block_pos_x].Shape == (int)Block.Type.DownSpike ||
                   block[block_pos_y, block_pos_x].Shape == (int)Block.Type.LeftSpike ||
                   block[block_pos_y, block_pos_x].Shape == (int)Block.Type.RightSpike)
                {
                    switch (block[block_pos_y, block_pos_x].Shape)
                    {
                        case (int)Block.Type.UpSpike:
                            if (IsTriCollision(block[block_pos_y, block_pos_x], -map_x, (int)Block.Type.UpSpike)){
                                user_delay();
                                user_die(blockSize);
                                return true;
                            }
                            break;
                        case (int)Block.Type.DownSpike:
                            if (IsTriCollision(block[block_pos_y, block_pos_x], -map_x, (int)Block.Type.DownSpike))
                            {
                                user_delay();
                                user_die(blockSize);
                                return true;
                            }
                            break;
                        case (int)Block.Type.LeftSpike:
                            if (IsTriCollision(block[block_pos_y, block_pos_x], -map_x, (int)Block.Type.LeftSpike))
                            {
                                user_delay();
                                user_die(blockSize);
                                return true;
                            }
                            break;
                        case (int)Block.Type.RightSpike:
                            if (IsTriCollision(block[block_pos_y, block_pos_x], -map_x, (int)Block.Type.RightSpike))
                            {
                                user_delay();
                                user_die(blockSize);
                                return true;
                            }
                            break;
                    }
                }
            }
            return false;
        }
        // 상하좌우 검사
        private Point checkSideCollision(Point user_pos, Block[,] block, Sound sound)
        {
            //상하좌우 상대적 위치
            void collosionProcess(int i, int jump = 7)
            {
                switch (i)
                {
                    case 0:
                        if (y_ < 0)
                        {
                            y_ = -y_ % jumpSize;
                        }
                        y += 5;
                        break;
                    case 1:
                        if (fly) jump = 0;
                        Ball_Jump(jump);
                        y -= 5;
                        break;
                    case 2:
                        x_ = -3;
                        x -= 1;
                        break;
                    case 3:
                        x_ = 3;
                        x += 1;
                        break;
                }
            }

            int[] arr_y = { -1, 1, 0, 0 };
            int[] arr_x = { 0, 0, 1, -1 };
            for (int i = 0; i < 4; ++i)
            {
                int block_pos_y = user_pos.Y + arr_y[i];
                int block_pos_x = user_pos.X + arr_x[i];
                if (block_pos_y > -1 && block_pos_x > -1 && block_pos_y < MHEIGHT && block_pos_x < MWIDTH)
                {
                    if (block[block_pos_y, block_pos_x].Shape != (int)Block.Type.None &&
                        IsRectCollision(block[block_pos_y, block_pos_x], -map_x))
                    {

                        switch (block[block_pos_y, block_pos_x].Shape)
                        {
                            case (int)Block.Type.Block:
                                collosionProcess(i);
                                sound.SoundPlay((int)Sound.Index.Bound);
                                break;
                            case (int)Block.Type.BrokenBlock:
                                collosionProcess(i);
                                sound.SoundPlay((int)Sound.Index.Break);
                                block[block_pos_y, block_pos_x].Shape = 0;
                                block[block_pos_y, block_pos_x].RegenerationBlock();
                                return new Point(block_pos_x, block_pos_y);
                            case (int)Block.Type.JumpPad:
                                collosionProcess(i, 10);
                                sound.SoundPlay((int)Sound.Index.JumpPad);
                                break;
                            case (int)Block.Type.Flag:
                                if (!victory)
                                {
                                    victory = true;
                                    sound.SoundPlay((int)Sound.Index.Victory);
                                }
                                break;
                            case (int)Block.Type.Save:
                                collosionProcess(i);
                                user_save(block[block_pos_y, block_pos_x]);
                                sound.SoundPlay((int)Sound.Index.Save);
                                break;
                            case (int)Block.Type.CloseChest:
                                collosionProcess(i);
                                for (int index = 0; index < Item.itemList.Length; ++index)
                                {
                                    if (Item.itemList[index] == (int)Item.Type.None)
                                    {
                                        block[block_pos_y, block_pos_x].Shape = (int)Block.Type.OpenChest;
                                        sound.SoundPlay((int)Sound.Index.OpenChest);
                                        Random rand = new Random();
                                        Item.itemList[index] = rand.Next(1, 4);
                                        block[block_pos_y, block_pos_x].RegenerationBlock();
                                        return new Point(block_pos_x, block_pos_y);
                                    }
                                }
                                sound.SoundPlay((int)Sound.Index.Bound);
                                break;
                            case (int)Block.Type.OpenChest:
                                collosionProcess(i);
                                sound.SoundPlay((int)Sound.Index.Bound);
                                break;
                        }
                    
                        return new Point(-1,-1);
                    }
                }
            }
            return new Point(0,0);
        }

        // 모서리 검사
        private Point checkEdgeCollision(Point user_pos, Block[,] block, Sound sound)
        {
            void collosionProcess(int i, int jump = 7)
            {
                if (i == 0 || i == 1)
                {
                    if (y_ < 0)
                    {
                        y_ = -y_ % jumpSize;
                    }
                    y += 5;
                }
                if (i == 2 || i == 3)
                {
                    if (fly) jump = 0; 
                    Ball_Jump(jump);
                    y -= 5;
                }
            }

            //모서리들의 상대적 위치
            int[] arr_ey = { -1, -1, 1, 1 };
            int[] arr_ex = { -1, 1, -1, 1 };
            for (int i = 0; i < 4; ++i)
            {
                int block_pos_y = user_pos.Y + arr_ey[i];
                int block_pos_x = user_pos.X + arr_ex[i];
                if (block_pos_y > -1 && block_pos_x > -1 && block_pos_y < MHEIGHT && block_pos_x < MWIDTH)
                {
                    if (block[block_pos_y, block_pos_x].Shape != (int)Block.Type.None &&
                       IsRectCollision(block[block_pos_y, block_pos_x], -map_x))
                    {

                        switch (block[block_pos_y, block_pos_x].Shape)
                        {
                            case (int)Block.Type.Block:
                                collosionProcess(i);
                                sound.SoundPlay((int)Sound.Index.Bound);
                                break;
                            case (int)Block.Type.BrokenBlock:
                                collosionProcess(i);
                                sound.SoundPlay((int)Sound.Index.Break);
                                block[block_pos_y, block_pos_x].Shape = 0;
                                block[block_pos_y, block_pos_x].RegenerationBlock();
                                return new Point(block_pos_x, block_pos_y);
                            case (int)Block.Type.JumpPad:
                                collosionProcess(i, 10);
                                sound.SoundPlay((int)Sound.Index.JumpPad);
                                break;
                            case (int)Block.Type.Flag:
                                if (!victory)
                                {
                                    victory = true;
                                    sound.SoundPlay((int)Sound.Index.Victory);
                                }
                                break;
                            case (int)Block.Type.Save:
                                collosionProcess(i);
                                user_save(block[block_pos_y, block_pos_x]);
                                sound.SoundPlay((int)Sound.Index.Save);
                                break;
                            case (int)Block.Type.CloseChest:
                                collosionProcess(i);
                                for(int index=0;index<Item.itemList.Length;++index) {
                                    if (Item.itemList[index] == (int)Item.Type.None) {
                                        block[block_pos_y, block_pos_x].Shape = (int)Block.Type.OpenChest;
                                        sound.SoundPlay((int)Sound.Index.OpenChest);
                                        Random rand = new Random();
                                        Item.itemList[index] = rand.Next(1, 4);
                                        block[block_pos_y, block_pos_x].RegenerationBlock();
                                        return new Point(block_pos_x, block_pos_y);
                                    }
                                }
                                sound.SoundPlay((int)Sound.Index.Bound);
                                break;
                            case (int)Block.Type.OpenChest:
                                collosionProcess(i);
                                sound.SoundPlay((int)Sound.Index.Bound);
                                break;
                        }
                        return new Point(-1,-1);
                    }
                }
            }
            return new Point(0,0);
        }
        // 블록과의 충돌확인 함수
        public bool IsRectCollision(Block block, int setPos)
        {
            /*-------------
             *| 6 | 7 | 8 |         4 : 사각형
             *| 3 | 4 | 5 |         해결방법 : 1. 사각형을 반지름만큼 확장시켜서 상하좌우
             *| 0 | 1 | 2 |                       충돌여부를 체크한다.
             *-------------                    2. 대각선은 사각형의 꼭지점이 원 안에
             * *********                          들어왔는지 확인 후 충돌을 처리한다.
             */

            if (y > block.getY &&
                y < block.getY + block.getSize &&
                x > block.getX - radius + setPos &&
                x < block.getX + block.getSize + radius + setPos)
            {
                // case 3, 4, 5
                return true;
            }
            else if (x > block.getX + setPos &&
                    x < block.getX + block.getSize + setPos &&
                    y > block.getY - radius &&
                    y < block.getY + block.getSize + radius)
            {
                // case 1, 4, 7
                return true;
            }
            else
            {
                // case 0, 2, 6, 8
                double distZero = Math.Sqrt(Math.Pow((block.getX + setPos - x), 2) + Math.Pow((block.getY + block.getSize - y), 2));
                double distTwo = Math.Sqrt(Math.Pow((block.getX + setPos + block.getSize - x), 2) + Math.Pow((block.getY + block.getSize - y), 2));
                double distSix = Math.Sqrt(Math.Pow((block.getX + setPos - x), 2) + Math.Pow((block.getY - y), 2));
                double distEight = Math.Sqrt(Math.Pow((block.getX + setPos + block.getSize - x), 2) + Math.Pow((block.getY - y), 2));

                if (distZero < radius ||
                    distTwo < radius ||
                    distSix < radius ||
                    distEight < radius)
                {
                    return true;
                }
            }
            return false;
        }

        // 삼각형과 원과의 충돌
        public bool IsTriCollision(Block block, int setPos, int type)
        {
            Point centralPoint = new Point();
            Point p1 = new Point();
            Point p2 = new Point();
            Point p3 = new Point();
            int TriRadius = 0;
            string center = "";
            switch (type)
            {
                case (int)Block.Type.UpSpike:
                    p1.X = block.getX + setPos;                     // 왼쪽 아래 점
                    p1.Y = block.getY + block.getSize;
                    p2.X = block.getX + block.getSize + setPos;     // 오른쪽 아래 점
                    p2.Y = block.getY + block.getSize;
                    p3.X = block.getX + block.getSize / 2 + setPos; // 위쪽 중앙 점
                    p3.Y = block.getY;
                    TriRadius = block.getY + block.getSize;
                    center = "Y";
                    break;
                case (int)Block.Type.DownSpike:
                    p1.X = block.getX + setPos;                     // 왼쪽 위 점
                    p1.Y = block.getY;
                    p2.X = block.getX + block.getSize + setPos;     // 오른쪽 위 점
                    p2.Y = block.getY;
                    p3.X = block.getX + block.getSize / 2 + setPos; // 아래 중앙 점
                    p3.Y = block.getY + block.getSize;
                    TriRadius = block.getY;
                    center = "Y";
                    break;
                case (int)Block.Type.LeftSpike:
                    p1.X = block.getX + setPos;                     // 왼쪽 위 점
                    p1.Y = block.getY;
                    p2.X = block.getX + setPos;                     // 왼쪽 아래 점
                    p2.Y = block.getY + block.getSize;
                    p3.X = block.getX + block.getSize + setPos;     // 오른쪽 중앙 점
                    p3.Y = block.getY + block.getSize / 2;
                    TriRadius = block.getX;
                    center = "X";
                    break;
                case (int)Block.Type.RightSpike:
                    p1.X = block.getX + block.getSize + setPos;     // 오른쪽 위 점
                    p1.Y = block.getY;
                    p2.X = block.getX + block.getSize + setPos;     // 오른쪽 아래 점
                    p2.Y = block.getY + block.getSize;
                    p3.X = block.getX + setPos;                     // 왼쪽 중앙 점
                    p3.Y = block.getY + block.getSize / 2;
                    TriRadius = block.getX + block.getSize;
                    center = "X";
                    break;
            }
            centralPoint.X = (p1.X + p2.X + p3.X) / 3;
            centralPoint.Y = (p1.Y + p2.Y + p3.Y) / 3;
            if (center == "X")
            {
                TriRadius = Math.Abs(TriRadius - centralPoint.X);
            }else if(center == "Y")
            {
                TriRadius = Math.Abs(TriRadius - centralPoint.Y);
            }

            double distCentral = Math.Sqrt(Math.Pow(x - centralPoint.X,2) + Math.Pow(y - centralPoint.Y,2));

            if (distCentral < radius + TriRadius)
            {
                return true;
            }
            else
            {
                double distP1 = Math.Sqrt(Math.Pow(p1.X - x, 2) + Math.Pow(p1.Y - y, 2));
                double distP2 = Math.Sqrt(Math.Pow(p2.X - x, 2) + Math.Pow(p2.Y - y, 2));
                double distP3 = Math.Sqrt(Math.Pow(p3.X - x, 2) + Math.Pow(p3.Y - y, 2));

                if (distP1 < radius ||
                    distP2 < radius ||
                    distP3 < radius)
                {
                    return true;
                }
            }
            return false;
        }

        // =================================================================================== 

        // 승리조건
        // =================================================================================== 
        public int CheckVictory(FriendBall[] balls)
        {
            if (victory)
            {
                return 1;
            }
            else
            {
                foreach (FriendBall ball in balls)
                {
                    if(ball.victory)
                        return 2;
                }
            }
            return 0;
        }
        // =================================================================================== 
    }
}
