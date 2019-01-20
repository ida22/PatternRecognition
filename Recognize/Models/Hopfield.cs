using System;
using Accord.Math;

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

            for(int row = 0; row < patternCount; row++)
            {
                double[,] x = testData.Get(row, row + 1, 0, neuronCount).Transpose().Convert(i => (double)i);

                var x1 = W.Dot(x).Subtract(x);
                var x1t = x1.Transpose();

                var licznik = x1.Dot(x1t);
                double mianownik = x.TransposeAndDot(x).Subtract(x.Transpose().Dot(W).Dot(x))[0,0];

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
            int[] updateOrder = new int[neuronCount];
            for (int index = 0; index < neuronCount; index++)
            {
                updateOrder[index] = index;
            }
            updateOrder = Shuffle(updateOrder);

            output = testData;

            int updatedCount = 0;
            int iterationCount = 0;

            do
            {
                updatedCount = 0;

                for(int i=0; i<neuronCount; i++)
                {
                    int neuronId = updateOrder[i];
                    var Vin = weights.GetColumn(neuronId).Dot(output);
                    var V = (Vin >= 0 ? 1 : -1);
                    if(output[neuronId] != V)
                    {
                        updatedCount++;
                        output[neuronId] = V;
                    }
                }

                iterationCount++;
            } while (updatedCount != 0 && iterationCount < maxIterations);

            return output;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Simple Fisher-Yates to shuffle an array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static T[] Shuffle<T>(T[] array)
        {
            T[] retArray = new T[array.Length];
            array.CopyTo(retArray, 0);

            Random random = new Random();

            for (int i = 0; i < array.Length; i += 1)
            {
                int swapIndex = random.Next(i, array.Length);
                if (swapIndex != i)
                {
                    T temp = retArray[i];
                    retArray[i] = retArray[swapIndex];
                    retArray[swapIndex] = temp;
                }
            }

            return retArray;
        }

        #endregion
    }
}
