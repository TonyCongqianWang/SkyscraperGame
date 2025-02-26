using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SkyscraperGameGui
{
    record class GameModel
    {
        public int Size { get; }
        public int IsSolved { get; set; } = 0;
        public int IsInfeasible { get; set; } = 0;
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
        public HashSet<byte>[,] PossibleValues { get; set; }

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
            PossibleValues = new HashSet<byte>[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    PossibleValues[i, j] = new HashSet<byte>();
                }
            }
        }
    }
}
