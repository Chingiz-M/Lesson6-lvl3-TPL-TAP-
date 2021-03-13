using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MultiplicationMatrix
{
    class Program
    {
        public static List<int[,]> ListMatrix = new List<int[,]>();
        static void Main(string[] args)
        {
            int[,] Matrix1 = GetMatrix(10, 10, 10);
            int[,] Matrix2 = GetMatrix(10, 10, 10);

            MultiMatrix(Matrix1, Matrix2);
            DisplayMatrixAsync(ListMatrix);

            Console.ReadKey(true);
        }

        public static int[,] MultiMatrix(int[,] Matrix1, int[,] Matrix2)
        {
            int[,] ResultMultiMatrix = GetMatrix(Matrix1.GetLength(0), Matrix2.GetLength(1), 0);
            Parallel.For(0, ResultMultiMatrix.GetLength(0), resultrow =>
            {
                for (int i = 0; i < ResultMultiMatrix.GetLength(0); i++)
                    for (int j = 0; j < ResultMultiMatrix.GetLength(1); j++)
                        ResultMultiMatrix[resultrow,i] += Matrix1[resultrow, j] * Matrix2[j, i];
            });
            return ResultMultiMatrix;
        }

        private static int[,] GetMatrix(int cols, int rows, int maxrandom)
        {
            int[,] Matrix = new int[cols,rows];
            Random random = new Random();
            for (int i = 0; i < Matrix.GetLength(0); i++)
                for (int j = 0; j < Matrix.GetLength(1); j++)
                    Matrix[i, j] = random.Next(0, maxrandom);
            ListMatrix.Add(Matrix);
            return Matrix;
        }

        public static async void DisplayMatrixAsync(List<int[,]> list)
        {
            foreach (var matrix in list)
                await Task.Run(() => DisplayMatrix(matrix));
        }

        private static void DisplayMatrix(int[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                    Console.Write($"{matrix[i, j]}\t");
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
}
