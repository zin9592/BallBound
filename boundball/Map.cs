using System;

namespace boundball
{
    class Map
    {
        static string Path = "resource\\Map\\";
        int MWIDTH;
        int MHEIGHT;
        int blockSize;

        // 맵 생성
        // ===============================================================================
        // 블록 초기화
        public Map(int _MWIDTH, int _MHEIGHT, int _blockSize)
        {
            MWIDTH = _MWIDTH;
            MHEIGHT = _MHEIGHT;
            blockSize = _blockSize;
        }
        public Block[,] InitializeMap(Block[,] block)
        {

            block = new Block[MHEIGHT, MWIDTH];
            for (int y = 0; y < MHEIGHT; ++y)
            {
                for (int x = 0; x < MWIDTH; ++x)
                {
                    block[y, x] = new Block();
                }
            }
            return LoadMap(block);
        }
        // 맵 텍스트 파일 불러오기
        private Block[,] LoadMap(Block[,] block)
        {
            string line;
            int y = 0;
            System.IO.StreamReader file =
                new System.IO.StreamReader(Path+"map1.txt");
            while ((line = file.ReadLine()) != null)
            {
                int len = line.Length;
                for (int x = 0; x < len; ++x)
                {
                    string str = Convert.ToString(line[x]);
                    int shape = 0;
                    switch (str)
                    {
                        case "A":
                            shape = 10;
                            break;
                        default:
                            shape = Convert.ToInt32(str);
                            break;
                    }
                    //line[x] - '0'
                    block[y, x].setBlock(x * blockSize, y * blockSize, blockSize, shape);
                }
                if (y < MHEIGHT - 1)
                {
                    y++;
                }
            }
            file.Close();
            return block;
        }
        // ===============================================================================
    }
}
