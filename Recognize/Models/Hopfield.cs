using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AForge;
using Accord.Math;
using Accord.Math.Decompositions;

namespace Recognize.Models
{
    public class Hopfield
    {
        #region Declarations

        private Matrix _weights;
        private Matrix _output;
        private Matrix _offsetNeuronWeights;
        private long _iterations = 0;
        private int _neuronCount = 0;
        private const double THRESHOLD_CONSTANT = 0.0;

        #endregion

        #region Constructor

        /// <summary>
        /// Method that accepts a pattern(s) from which to train. Returns the current Weight matrix. 
        /// Each column of the input matrix represents a pattern to be learnt. The number of neurons
        /// generated is determined by the number of rows in the matrix.
        /// </summary>
        /// <param name="trainingData">Matrix containing the training pattern(s).</param>
        public Matrix Train(Matrix trainingData)
        {
            //see if this is the first piece of training data
            if (_neuronCount == 0)
            {
                //get the size of the network, i.e. the number of neurons
                _neuronCount = trainingData.RowCount;

                //create the weights matrix
                _weights = new Matrix(_neuronCount, _neuronCount);

                //create the output vector
                _output = new Matrix(_neuronCount, 1);

                //create the offset Neuron weights (used to determine thresholds)
                _offsetNeuronWeights = new Matrix(_neuronCount, 1);

            }

            //set data to bipolar representation
            trainingData = trainingData.ToBiPolar();

            //transpose and multiply by self
            Matrix trainingDataT = Matrix.Transpose(trainingData);
            Matrix result = Matrix.Multiply(trainingData, trainingDataT);

            //neurons do not have connections to themselves so subtract a unit matrix (with the diagonal set
            //to the number of paterns)
            Matrix unit = new Matrix(trainingData.RowCount, 1.0);
            result = Matrix.Subtract(result, unit);

            //now we have the calculated pattern so add to the weights
            _weights = Matrix.Add(_weights, result);

            //now calculate the thresholds using a neuron with its output fixed at 1.
            //first create a vector of all 1's that is the same size as the number of patterns passed in
            //this allows us to calculate the offSetNeuron weights as we did above.            
            Matrix offsetNeuron = new Matrix(trainingData.ColumnCount, 1, 1.0);

            //multiply offsetNeuron with input pattern(s) and add to _offsetNeuronWeights.
            result = Matrix.Multiply(trainingData, offsetNeuron);
            _offsetNeuronWeights = Matrix.Add(_offsetNeuronWeights, result);

#if(TRACE)
            Console.WriteLine(string.Format("Weights: {0}", _weights.ToString("f0")));
#endif
            return _weights;
        }

        public void TrainByPseudoInverse(double[,] testData)
        {
            int rowCount = testData.Rows();
            int rowLength = testData.Columns();

            double[,] W = new double[rowLength, rowLength];

            for(int row = 0; row < rowCount; row++)
            {
                double[,] x = testData.Get(row, row + 1, 0, rowLength).Transpose();

                var x1 = W.Dot(x).Subtract(x);
                var x1t = x1.Transpose();

                var licznik = x1.Dot(x1t);
                double mianownik = x.TransposeAndDot(x).Subtract(x.Transpose().Dot(W).Dot(x))[0,0];

                W = W.Add(licznik.Divide(mianownik));
            }
            _weights = new Matrix(W);
        }

        /// <summary>
        /// Method that accepts a pattern for the network to test. Patterns are represented in columns.
        /// Returns an binary output matrix that matches the training matrix if the testdata
        /// was matched to the training data.
        /// </summary>
        public Matrix Test(Matrix testData)
        {
            //_neuronCount = 64; // TRZEBA TU COS Z TYM ZROBIC!!!!
            //_weights = new Matrix(_neuronCount, _neuronCount);
            //_output = new Matrix(_neuronCount, 1);
            //_offsetNeuronWeights = new Matrix(_neuronCount, 1);

            if (_neuronCount <= 0)
                throw new NetworkException(Resources.ErrorMessages.NetworkNotTrained);
            if (!testData.IsColumnVector)
                throw new ArgumentException(Resources.ErrorMessages.ArgumentMatrixisNotColumnVector);
            if (testData.RowCount != _neuronCount)
                throw new ArgumentException(Resources.ErrorMessages.ArgumentBadSize);

            //set the maximum number of iterations
            long maxIterations = Convert.ToInt64(System.Math.Pow((double)_neuronCount, 2));
            bool isStable = false;

            //create an ordered array and then shuffle it
            int[] updateOrder = new int[_neuronCount];
            for (int index = 0; index < _neuronCount; index++)
            {
                updateOrder[index] = index;
            }

            updateOrder = Shuffle(updateOrder);

            //set the output to the initial state
            _output = testData;
            _iterations = 0;

            //loop through the maximum possible times or until network is stable
            while (_iterations < maxIterations && isStable == false)
            {

                //assume stable
                isStable = true;

                //make the input the same as the output
                Matrix input = _output;

                //loop through each neuron, calculating the input and determining the output
                for (int currentNeuron = 0; currentNeuron < _neuronCount; currentNeuron++)
                {

                    //get the next neuron to update
                    int neuronToUpdate = updateOrder[currentNeuron];

                    //get the weights for the neuron
                    Matrix neuronWeights = _weights.GetRow(neuronToUpdate);

                    //==================================================================================================
                    // This code was originally used and was based entirely on Picton's description of the 
                    // process (2000, p75). It was later refactored to use the Matrix.DotProduct() method.
                    //==================================================================================================
                    //calculate wieght of the current neuron
                    //double neuronWeight = 0;
                    //for (int inputLine = 0; inputLine < input.RowCount; inputLine++)
                    //{
                    //    //dont do if the input is from the current neuron as neuron outputs
                    //    //do not connect to themselves
                    //    if (inputLine != currentNeuron)
                    //    {
                    //        //multiply by associated input element so that only the inputLine set to 1 adds to the total
                    //        neuronWeight += (input.GetElement(inputLine, 0) * neuronWeights.GetElement(0, inputLine));
                    //    }
                    //}
                    //==================================================================================================

                    //==================================================================================================
                    // Refactored alternative approach to code above, this works because each neurons connection to
                    // itself was 'zeroed out' during the training phase.
                    //==================================================================================================
                    double neuronWeight = Matrix.DotProduct(neuronWeights, input);
                    //==================================================================================================

                    //get the threshold for the current neuron
                    //the threshold is = the negated weight therefore multiply by -1
                    double threshold = _offsetNeuronWeights.GetElement(neuronToUpdate, 0) * -1.0;

                    //output 1 if the weight is above the threshold and 0 if the weight is below the threshold
                    //if the weight equals the threshold, make no change to whats already there.
                    //the THRESHOLD_CONSTANT is a value used in experiments and would typically be set to 0
                    if (neuronWeight > threshold + THRESHOLD_CONSTANT)
                    {
                        //set the output to 1 if it is not already
                        if (_output.GetElement(neuronToUpdate, 0) != 1)
                        {
                            _output.SetElement(neuronToUpdate, 0, 1);
                            isStable = false;
                        }
                    }
                    else if (neuronWeight < threshold)
                    {
                        //set the output to 0 if it is not already
                        if (_output.GetElement(neuronToUpdate, 0) != 0)
                        {
                            _output.SetElement(neuronToUpdate, 0, 0);
                            isStable = false;
                        }
                    }

                    //if state has changed then exit this loop and move to next state
                    if (!isStable)
                    {
                        break;
                    }
                    //else loop to next neuron
                }

                //keep a count of how many times we have been through the process
                _iterations++;

            }

#if(TRACE)
            Console.WriteLine(string.Format("Output: {0}, Iterations: {1}", _output.ToString("f0"), _iterations.ToString()));
#endif
            return _output;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns the number of neurons in the network.
        /// This property is updated after the Train() method has been invoked.
        /// </summary>
        public int NeuronCount
        {
            get { return _neuronCount; }
        }
        /// <summary>
        /// Returns the current weight values.
        /// This property is updated after the Train() method has been invoked.
        /// </summary>
        public Matrix Weights
        {
            get { return _weights; }
        }
        /// <summary>
        /// Returns the current output values.
        /// This property is updated after the Test() method has been invoked.
        /// </summary>
        public Matrix Output
        {
            get { return _output; }
        }
        /// <summary>
        /// Returns the number of iterations needed to identify the test data. 
        /// This property is updated after the Test() method has been invoked.
        /// </summary>
        public long Iterations
        {
            get { return _iterations; }
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