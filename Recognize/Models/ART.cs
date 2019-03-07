using Accord.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Recognize.Models
{
    public class ART
    {
        public int neuronsCount = 64;
        public int patternsCount = 10;
        public int outputCount = 10;

        public double TRAIN_VIGILANCE = 0.99;
        public double TEST_VIGILANCE = 0.8;

        public int resetLimit = 100;

        int[,] F1; //warstwa porównawcza, zawiera wektory wejściowe (N)
        double[,] F2; //warstwa rozpoznająca (M - liczba wyjściowych neuronów)  F2 = y

        double[,] W;
        double[,] V;

        public bool trained = false;

        public void Train(int[,] data)
        {
            //patternsCount = data.Rows();
            //neuronsCount = data.Columns();

            F1 = new int[neuronsCount, 1]; //warstwa porównawcza, zawiera wektory wejściowe (N)
            F2 = new double[patternsCount, 1]; //warstwa rozpoznająca (M - liczba wyjściowych neuronów)  F2 = y

            W = new double[patternsCount, neuronsCount];
            V = new double[patternsCount, neuronsCount];

            W.Set(1.0 / (1.0 + neuronsCount)); //krok 1
            V.Set(1);

            for (int pattern = 0; pattern < patternsCount; pattern++)
            { 
                MagicIda(data.Get(pattern, pattern + 1, 0, neuronsCount), true);
            }

            trained = true;
        }

        public int Test(int[] data)
        {
            int[,] newData = new int[1, neuronsCount];
            newData.SetRow(0, data);

            return MagicIda(newData, false);
        }

        public int MagicIda(int[,] data, bool train)
        {
            int m = 0;
            bool condition = false;
            int limit = resetLimit;

            F1 = data;

            F2.Set(0);

            for (int pattern = 0; pattern < patternsCount; pattern++)
            {
                F2[pattern, 0] = W.GetRow(pattern).DotWithTransposed(F1)[0]; //krok 2
            }

            while (!condition)
            {
                m = F2.ArgMax().Item1; //krok 3

                double licznik = V.GetRow(m).DotWithTransposed(data)[0];
                int mianownik = data.Sum();

                condition = TestProbability(licznik, mianownik, m, train);

                if (--limit == 0)
                    return -1;
            }

            //if (train) UpdateWeights(F1, m);


            return m;
        }

        private bool TestProbability(double licznik, int mianownik, int m, bool train)
        {
            if (licznik / mianownik >= (train ? TRAIN_VIGILANCE : TEST_VIGILANCE))
            {
                UpdateWeights(F1, m);
                return true;     // Candidate is accepted.
            }
            else
            {
                if (F2.Max() != 0) F2[m, 0] = -1.0; // wyłączenie m-tego neuronu w górnej warstwie
                else m = m == F2.ArgMax().Item1 ? F2.ArgMax().Item1 + 1 : F2.ArgMax().Item1;
                return false;      // Candidate is rejected.
            }
        }

        private void UpdateWeights(int[,] F1, int m)
        {
            double[,] tempV = new double[patternsCount, neuronsCount];
            double[,] tempW = new double[patternsCount, neuronsCount];
            V.CopyTo(tempV);

            var row = tempV.GetRow(m).Multiply(F1.GetRow(0));
            V.SetRow(m, row);

            var row2 = row.Divide(0.5 + tempV.GetRow(m).DotWithTransposed(F1)[0]);
            W.SetRow(m, row2);

            return;
        }
    }
}