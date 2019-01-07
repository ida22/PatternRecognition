using Accord.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Recognize.Models
{
    public class ART
    {
        public int neuronsCount = 0;
        public int patternsCount = 0;
        public int outputCount = 10;
        public double rho = 0.3;
        public int active = 0;

        public double[,] weights;
        public int[] output;

        public void Train(double[,] data)
        {
            patternsCount = data.Rows();
            neuronsCount = data.Columns();

            double[,] F1 = new double[neuronsCount, 1]; //warstwa porównawcza, zawiera wektory wejściowe (N)
            double[,] F2 = new double[outputCount, 1]; //warstwa rozpoznająca (M - liczba wyjściowych neuronów)
            F1.Set(1);
            F2.Set(1);

            double[,] W = new double[outputCount, neuronsCount];
            double[,] V = new double[neuronsCount, outputCount];
      
            // wypełnić macierz W i V randomowymi liczbami z przedzialu?
            Random r = new Random();
            W = W.Apply(x => r.NextDouble());
            V = V.Apply(x => r.NextDouble());

            for (int pattern = 0; pattern < patternsCount; pattern++)
            {
                double[,] x = data.Get(pattern, pattern + 1, 0, neuronsCount);
                F2 = W.Dot(x);

                var I = ToVector(F2).ArgSort(); //indeksy posortowanego F2

                foreach (int i in I)
                {
                    double d = (W.GetColumn(i).Multiply(ToVector(x))).Sum() / x.Sum();
                    if (d >= rho)
                    {
                        W.SetColumn(i, W.GetColumn(i).Dot(x));
                        V.SetRow(i, W.GetColumn(i).Divide((0.5 + W.GetColumn(i).Sum())));
                    }

                    if (active < F2.Length)
                    {
                        int j = active;
                        W.SetColumn(j, W.GetColumn(j).Dot(x));
                        V.SetRow(j, W.GetColumn(j).Divide((0.5 + W.GetColumn(i).Sum())));
                        active++;
                    }
                }
            }
        }

        public int[] Test(double[,] testdata)
        {
            return output;
        }

        public double[] ToVector(double[,] data)
        {
            double[] vector = new double[data.Rows() * data.Columns()];
            int k = 0;

            for (int i = 0; i < data.Rows(); i++)
            {
                for (int j = 0; j < data.Columns(); j++)
                {
                    vector[k] = data[i, j];
                    k++;
                }
            }

            return vector;
        }

        public double[,] ToMatrix(double[] data)
        {
            double rowCount = Math.Sqrt(data.Length);
            double[,] matrix = new double[(int)rowCount, (int)rowCount];
            int k = 0;

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < rowCount; j++)
                {
                    matrix[i, j] = data[k];
                    k++;
                }
            }

            return matrix;
        }
    }
}