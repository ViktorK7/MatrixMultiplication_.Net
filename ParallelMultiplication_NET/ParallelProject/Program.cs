using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MatrixMultiplication
{
    public class Matrix
    {
        public int Row { get; set; }
        public int Column { get; set; }
        double[,] matrix;
        public static Mutex mutex = new Mutex();

        Matrix() { }
        public Matrix(int row, int column)
        {
            Row = row;
            Column = column;
            matrix = new double[row, column];
        }
        public double[] GetColumn(int i)
        {
            double[] res = new double[Row];
            for (int j = 0; j < Row; j++)
                res[j] = matrix[j, i];
            return res;
        }
        public double[] GetRow(int i)
        {
            double[] res = new double[Column];
            for (int j = 0; j < Column; j++)
                res[j] = matrix[i, j];
            return res;
        }
        public double this[int i, int j]
        {
            get { return matrix[i, j]; }
            set { matrix[i, j] = value; }
        }
        public Matrix RandomValues()
        {
            Random rnd = new Random();
            for (int i = 0; i < Row; i++)
                for (int j = 0; j < Column; j++)
                    matrix[i, j] = rnd.Next(3);
            return this;
        }

        public void Print()
        {
            for (int i = 0; i < Row; i++)
            {
                for (int j = 0; j < Column; j++)
                    Console.Write(matrix[i, j] + " ");
                Console.WriteLine();
            }
        }
        public static Matrix operator *(Matrix one, Matrix two)
        {
            Matrix resultOfMultiplication = new Matrix(one.Row, two.Column);
            List<Thread> threads = new List<Thread>();
            for (int i = 0; i < one.Row; i++)
                for (int j = 0; j < two.Column; j++)
                {
                    int temporaryRow = i;
                    int temporaryColumn = j;
                    Thread thread = new Thread(() => VectorMult(temporaryRow, temporaryColumn, one, two, resultOfMultiplication));
                    thread.Start();
                    threads.Add(thread);
                }
            foreach (Thread t in threads)
                t.Join();
            return resultOfMultiplication;
        }

        public static void VectorMult(int temporaryRow, int temporaryColumn, Matrix one, Matrix two, Matrix resultOfMultiplication)
        {
            mutex.WaitOne();
            int i = temporaryRow;
            int j = temporaryColumn;
            double[] x = one.GetRow(i);
            double[] y = two.GetColumn(j);

            for (int k = 0; k < x.Length; k++)
                resultOfMultiplication[i, j] += x[k] * y[k];

            mutex.ReleaseMutex();
        }


        class Program
        {
            private static string endTime;

            static void Main(string[] args)
            {
                Console.Write($"Number of rows per matrixx 1 ");
                int n = int.Parse(Console.ReadLine());
                Console.Write("Number of colums per matrix 1 and rows per matrix 2 =");
                int m = int.Parse(Console.ReadLine());
                Console.Write("Number of colums per Matrix 2 =");
                int k = int.Parse(Console.ReadLine());
                Matrix One = new Matrix(n, m).RandomValues();
                Matrix Two = new Matrix(m, k).RandomValues();
                Console.WriteLine("Matrix 1 :");
                One.Print();
                Console.WriteLine("Matrix 2 :");
                Two.Print();
                Console.WriteLine("Result:");


                DateTime start = DateTime.Now;
                Matrix resultMatrix = One * Two;
                endTime = (DateTime.Now - start).TotalSeconds.ToString();
                resultMatrix.Print();
                Console.WriteLine("End Time" + endTime + "c\n");

                Console.ReadLine();
            }
        }

    }
}
