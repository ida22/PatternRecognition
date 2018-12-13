using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Recognize.Models
{
    public class SuperHopfield
    {
        private Matrix _weights;
        private double[] _pattern;

        private int _cellSize;

        Hopfield _network = new Hopfield();

        //public Hopfield()
        //{
        //    InitializeComponent();
        //}

        #region Public Properties

        public int Cols { set; get; }
        public int Rows { set; get; }

        public Matrix Pattern
        {
            get
            {
                return new Matrix(_pattern, true);
            }
        }
        public Matrix Weights
        {
            get
            {
                return _weights;
            }
        }
        public Matrix TrainingData
        {
            set
            {
                _weights = _network.Train(value);
            }
        }

        #endregion

        /// <summary>
        /// This method provides post processing of the output. 
        /// This is typically used to detect and correct inversions.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private double[] PostProcess(double[] data)
        {
            int size = data.Count();
            int numberSet = data.Count(i => i == 1);

            //if more than half of the elements are set then invert them
            if (numberSet > size / 2)
            {
                //loop through the x and y co-ordinates
                for (int y = 0; y < this.Rows; y++)
                {
                    for (int x = 0; x < this.Cols; x++)
                    {
                        int index = (y * this.Cols) + x;
                        if (_pattern[index] > 0)
                            _pattern[index] = 0;
                        else
                            _pattern[index] = 1.0;
                    }
                }
            }

            return data;
        }
    }
}