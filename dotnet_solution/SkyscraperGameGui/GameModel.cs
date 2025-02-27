namespace SkyscraperGameGui
{
    record class GameModel
    {
        public int Size { get; }
        public int CurrentDepth { get; set; } = 0;
        public bool IsSolved { get; set; } = false;
        public bool IsInfeasible { get; set; } = false;
        public int NumInserts { get; set; } = 0;
        public int NumChecks { get; set; } = 0;
        public int NumUnsets { get; set; } = 0;
        public (int, int) LastSetIndex { get; set; } = (-1, -1);
        public byte[] TopValues { get; set; }
        public byte[] BottomValues { get; set; }
        public byte[] LeftValues { get; set; }
        public byte[] RightValues { get; set; }
        public bool[] TopValuesCheckStatus { get; set; }
        public bool[] BottomValuesCheckStatus { get; set; }
        public bool[] LeftValuesCheckStatus { get; set; }
        public bool[] RightValuesCheckStatus { get; set; }
        public byte[,] GridValues { get; set; }
        public bool[,][] GridValueValidities { get; set; }

        public GameModel(int size)
        {
            if (size < 4 || size > 9)
                throw new ArgumentException("Size must be between 4 and 9");
            Size = size;
            TopValues = new byte[size];
            BottomValues = new byte[size];
            LeftValues = new byte[size];
            RightValues = new byte[size];

            TopValuesCheckStatus = new bool[size];
            BottomValuesCheckStatus = new bool[size];
            LeftValuesCheckStatus = new bool[size];
            RightValuesCheckStatus = new bool[size];

            GridValues = new byte[size, size];
            GridValueValidities = new bool[size, size][];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    GridValueValidities[i, j] = new bool[size];
                }
            }
        }
    }
}
