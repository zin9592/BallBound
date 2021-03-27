using System.Drawing;

namespace boundball
{
    class FriendBall : Ball
    {
        static public int MAX_USER;

        public FriendBall(int _x, int _y,int _radius, Image _image)
        {
            x = _x;
            y = _y;
            radius = _radius;
            image = _image;
            
        }

        public void drawBall(ref BufferedGraphics graphics, FriendBall[] ball, Block[,] block)
        {
            Point[] friend_pos = new Point[MAX_USER];
            for (int i = 0; i < MAX_USER; ++i)
            {
                graphics.Graphics.FillEllipse(Brushes.Red, ball[i].x - map_x - ball[i].radius, ball[i].y - ball[i].radius, ball[i].radius * 2, ball[i].radius * 2);
                CheckCollosion(friend_pos, ball, block, i);
            }
        }
        public void CheckCollosion(Point[] friend_pos, FriendBall[] ball, Block[,] block, int i)
        {
            friend_pos[i].X = ball[i].x / blockSize;
            friend_pos[i].Y = ball[i].y / blockSize;

            int[] arr_y = { -1, 0, 0, 1, -1, -1, 1, 1 };
            int[] arr_x = { 0, -1, 1, 0, -1, 1, -1, 1 };
            for (int k = 0; k < MAX_USER; ++k)
            {
                int block_pos_y = friend_pos[i].Y + arr_y[k];
                int block_pos_x = friend_pos[i].X + arr_x[k];
                if (block_pos_y > -1 && block_pos_x > -1 && block_pos_y < MHEIGHT && block_pos_x < MWIDTH)
                {
                    if (ball[i].IsRectCollision(block[block_pos_y, block_pos_x], 0))
                    {
                        if (block[block_pos_y, block_pos_x].Shape == (int)Block.Type.BrokenBlock)
                        {
                            block[block_pos_y, block_pos_x].Shape = 0;
                            block[block_pos_y, block_pos_x].RegenerationBlock();
                            break;
                        }
                        else if (block[block_pos_y, block_pos_x].Shape == (int)Block.Type.Flag)
                        {
                            if (!victory)
                            {
                                victory = true;
                                Sound sound = new Sound();
                                sound.SoundPlay((int)Sound.Index.Defeat);
                            }
                            break;
                        }
                    }
                }
            }
        }
    }
}
