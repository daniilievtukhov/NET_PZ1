using System;
using System.Collections.Generic;
using System.Threading;

namespace MatrixMultiplication
{
    public class Matrix
    {
        private double[,] _data;
        private int _rows, _cols;

        public Matrix(double[,] data)
        {
            _data = data;
            _rows = data.GetLength(0);
            _cols = data.GetLength(1);
        }

        public double[] MultiplyByVectorLeft(double[] vector, int numberOfThreads)
        {
            if (vector.Length != _rows)
            {
                throw new ArgumentException("Vector size must match the number of matrix rows");
            }

            double[] result = new double[_cols];
            List<Thread> threads = new List<Thread>();

 
            for (int i = 0; i < numberOfThreads; i++)
            {
                int threadIndex = i;
                Thread t = new Thread(() => MultiplyColumns(vector, result, threadIndex, numberOfThreads));
                threads.Add(t);
                t.Start();
            }

            foreach (Thread t in threads)
            {
                t.Join();
            }

            return result;
        }

        private void MultiplyColumns(double[] vector, double[] result, int threadIndex, int numberOfThreads)
        {
            for (int col = threadIndex; col < _cols; col += numberOfThreads)
            {
                double sum = 0;
                for (int row = 0; row < _rows; row++)
                {
                    sum += vector[row] * _data[row, col];
                }
                result[col] = sum;
            }
        }

        public void PrintMatrix()
        {
            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _cols; j++)
                {
                    Console.Write($"{_data[i, j]} ");
                }
                Console.WriteLine();
            }
        }

        public static double[,] Generate(int rows, int cols)
        {
            var rand = new Random();
            double[,] matrix = new double[rows, cols];
            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < cols; ++j)
                {
                    matrix[i, j] = rand.Next(11);
                }
            }
            return matrix;
        }

        public static double[] GenerateVector(int size)
        {
            var rand = new Random();
            double[] vector = new double[size];
            for (int i = 0; i < size; ++i)
            {
                vector[i] = rand.Next(11);
            }
            return vector;
        }

        public static void PrintVector(double[] vector)
        {
            foreach (var value in vector)
            {
                Console.Write($"{value} ");
            }
            Console.WriteLine();
        }
    }

    internal class Program
    {
        public static void Main()
        {
            int rows = 6, cols = 6; // розмір матриці
            double[,] matrixData = Matrix.Generate(rows, cols);
            double[] vector = Matrix.GenerateVector(rows); // розмір вектора= число строк

            Matrix matrix = new Matrix(matrixData);

            Console.WriteLine("Matrix:");
            matrix.PrintMatrix();

            Console.WriteLine("\nVector:");
            Matrix.PrintVector(vector);

            // однопоточне множення
            Console.WriteLine("\nSingle-threaded multiplication (left multiplication):");
            var result = matrix.MultiplyByVectorLeft(vector, 1);
            Matrix.PrintVector(result);

            // багатопоточне множення 
            int numberOfThreads = 4;
            Console.WriteLine($"\nMulti-threaded multiplication (left multiplication) with {numberOfThreads} threads:");
            result = matrix.MultiplyByVectorLeft(vector, numberOfThreads);
            Matrix.PrintVector(result);
        }
    }
}
