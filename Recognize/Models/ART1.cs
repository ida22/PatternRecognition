using Accord.Math;
using System;

namespace Recognize.Models
{
    public class ART1
    {
        public static int neuronsCount;
        public static int patternsCount;
        public static int trainingPatternsCount;

        public static double VIGILANCE = 0.5;

        public static double[,] bw;    //Bottom-up weights.
        public static double[,] tw;    //Top-down weights.

        public static double[] f1a;        //Input layer. powinno byc int
        public static double[] f1b;        //Interface layer. powinno byc int
        public static double[] f2;

        private int[] membership;

        public void Train(double[,] data)
        {
            int inputSum = 0;
            int activationSum = 0;
            int f2Max = 0;
            bool reset = true;
            patternsCount = 10; //przekazane jako arg funkcji
            int pupa2 = data.Rows();
            neuronsCount = data.Columns();

            bw = new double[patternsCount, neuronsCount];    //Bottom-up weights.
            tw = new double[patternsCount, neuronsCount];    //Top-down weights.

            f1b = new double[neuronsCount];        //Interface layer. powinno byc int
            f2 = new double[patternsCount];

            membership = new int[patternsCount];

            // Initialize top-down weight matrix filled by ones
            tw.Set(1);

            // Initialize bottom-up weight matrix.
            bw.Set(1.0 / (1.0 + neuronsCount));

            for (int row = 0; row < pupa2; row++)
            {
                // Initialize f2 layer activations to 0.0
                f2.Set(0, 0, patternsCount);

                // Input pattern() to F1 layer.
                f1a = data.GetRow(row);

                // Compute sum of input pattern.
                inputSum = (int)f1a.Sum();

                // Compute activations for each node in the F1 layer.
                // Send input signal from f1a to the f1b layer.
                f1a.CopyTo(f1b);

                // Compute net input for each node in the f2 layer.
                for (int i = 0; i < patternsCount; i++)
                {
                    for (int j = 0; j < neuronsCount; j++)
                    {
                        f2[i] += bw[i,j] * f1a[j];
                    }
                }

                reset = true;
                while (reset == true)
                {
                    // Determine the largest value of the f2 nodes.
                    f2Max = f2.IndexOf(f2.Max());

                    // Recompute the f1a to f1b activations (perform AND function).
                    for (int i = 0; i < neuronsCount; i++)
                    {
                        f1b[i] = f1a[i] * Math.Floor(tw[f2Max, i]);
                    }

                    // Compute sum of input pattern.
                    activationSum = (int)f1b.Sum();

                    reset = testForReset(activationSum, inputSum, f2Max);
                }

                // Only use number of TRAINING_PATTERNS for training, the rest are tests.
                if (row < patternsCount)
                {
                    updateWeights(activationSum, f2Max);
                }

                // Record which cluster the input vector is assigned to.
                membership[row] = f2Max;

            }
            return;

        }

        private static bool testForReset(int activationSum, int inputSum, int f2Max)
        {
            if (activationSum / inputSum >= VIGILANCE)
            {
                return false;     // Candidate is accepted.
            }
            else
            {
                f2[f2Max] = -1.0; // Inhibit.
                return true;      // Candidate is rejected.
            }
        }

        private static void updateWeights(int activationSum, int f2Max)
        {
            // Update bw(f2Max)
            for (int i = 0; i < neuronsCount; i++)
            {
                bw[f2Max, i] = (2.0 * f1b[i]) / (1.0 + activationSum);
            }

            // Update tw(f2Max)
            for (int i = 0; i < neuronsCount; i++)
            {
                tw[f2Max, i] = f1b[i];
            }
            return;
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
    }
}