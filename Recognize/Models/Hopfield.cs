using System;
using Accord.Math;
using System.Linq;

namespace Recognize.Models
{
    public class Hopfield
    {
        #region Declarations

        public double[,] weights;
        private int[] output;
        private int neuronCount = 0;
        private const double THRESHOLD_CONSTANT = 0.0;

        public bool trained = false;

        #endregion

        #region Constructor

        public void TrainByPseudoInverse(int[,] testData)
        {
            testData = testData.Multiply(2).Subtract(1); // macierz -1 i 1

            int patternCount = testData.Rows(); //liczba wzorców
            neuronCount = testData.Columns(); //liczba neuronów we wzorcu

            double[,] W = new double[neuronCount, neuronCount]; //inicjalizacja macierzy wag 64x64

            for (int row = 0; row < patternCount; row++)
            {
                double[,] x = testData.Get(row, row + 1, 0, neuronCount).Transpose().Convert(i => (double)i);

                var x1 = W.Dot(x).Subtract(x);
                var x1t = x1.Transpose();

                var licznik = x1.Dot(x1t);
                double mianownik = x.TransposeAndDot(x).Subtract(x.Transpose().Dot(W).Dot(x))[0, 0];

                W = W.Add(licznik.Divide(mianownik));
            }

            weights = W;

            trained = true;
        }

        public int[] Test(int[] testData)
        {
            testData = testData.Multiply(2).Subtract(1);

            long maxIterations = Convert.ToInt64(Math.Pow(neuronCount, 2));
            //bool isStable = false;

            //create an ordered array and then shuffle it
            //int[] updateOrder = new int[neuronCount];
            //for (int index = 0; index < neuronCount; index++)
            //{
            //    updateOrder[index] = index;
            //}
            //updateOrder = Shuffle(updateOrder);

            output = testData.Copy();
            int[] tempOutput = testData.Copy();

            int updatedCount = 0;
            int iterationCount = 0;

            do
            {
                updatedCount = 0;

                for (int i = 0; i < neuronCount; i++)
                {
                    // int neuronId = updateOrder[i];
                    var Vin = weights.GetColumn(i).Dot(output);
                    var V = (Vin >= 0 ? 1 : -1);
                    if (tempOutput[i] != V)
                    {
                        updatedCount++;
                        tempOutput[i] = V;
                    }
                }

                tempOutput.CopyTo(output);

                iterationCount++;
            } while (updatedCount != 0 && iterationCount < maxIterations);

            return output;
        }

        public int NeuronOutput(int[] hopfieldOutput, int[,] patterns)
        {
            int neuron = -1;
            int[] neuronSimilarityVector = new int[patterns.Rows()];

            hopfieldOutput = hopfieldOutput.Add(1).Divide(2).Select(c => Convert.ToInt32(c.ToString())).ToArray();

                for (int index = 0; index < patterns.Rows(); index++)
                {
                    int similarNeuronsCount = 0;

                    for (int k = 0; k < patterns.Columns(); k++)
                    {
                        if (patterns[index,k] == hopfieldOutput[k]) similarNeuronsCount++;
                    }

                    neuronSimilarityVector.Set(similarNeuronsCount, index);
                }
            neuron = neuronSimilarityVector.IndexOf(neuronSimilarityVector.Max());

            return neuron;
        }

        #endregion
    }
}
