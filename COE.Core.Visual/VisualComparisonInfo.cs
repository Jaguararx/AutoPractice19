using System;
using System.Linq;

namespace COE.Core.Visual
{
    public class VisualComparisonInfo
    {
        public VisualComparisonInfo(byte[,] differenceMatrix) => DifferenceMatrix = differenceMatrix ?? throw new ArgumentNullException(nameof(differenceMatrix));
        public byte[,] DifferenceMatrix { get; }
        public float DifferencePercentage => DiffPixels / (float)DifferenceMatrix.Length;
        public int DiffPixels => DifferenceMatrix.Cast<byte>().Count(cellValue => cellValue > 0);
        public bool Match => DiffPixels == 0;
    }
}