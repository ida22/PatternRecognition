using Accord.Math;
using System;

namespace Recognize.Models
{
	public class ART1
	{
		public int neuronsCount;
		public int patternsCount;

		public double TRAIN_VIGILANCE = 1;
		public double TEST_VIGILANCE = 0.5;

		public int resetLimit = 100;

		public double[,] bw;    //Bottom-up weights.
		public double[,] tw;    //Top-down weights.

		public int[] f1a;
		public int[] f1b;
		public double[] f2;

		public bool trained = false;

		public void Train(int[,] data)
		{
			patternsCount = data.Rows();
			neuronsCount = data.Columns();

			bw = new double[patternsCount, neuronsCount];    //Bottom-up weights. w
			tw = new double[patternsCount, neuronsCount];    //Top-down weights. v

			f1a = new int[neuronsCount];
			f1b = new int[neuronsCount];
			f2 = new double[patternsCount];

			// Initialize top-down weight matrix filled by ones V
			tw.Set(1); 

			// Initialize bottom-up weight matrix. W
			bw.Set(1.0 / (1.0 + neuronsCount)); 

			for (int row = 0; row < patternsCount; row++)
			{
				Console.Write("{0} ", Magic(data.GetRow(row), true));
			}

			trained = true;
		}

		public int Test(int[] data)
		{
			return Magic(data, false);
		}

		private int Magic(int[] pattern, bool train)
		{
			int inputSum = 0;
			int activationSum = 0;
			int f2Max = 0;

			// Initialize f2 layer activations to 0.0
			f2.Set(0, 0, patternsCount);

			// Input pattern() to F1 layer.
			f1a = pattern;

			// Compute sum of input pattern.
			inputSum = f1a.Sum();

			// Compute activations for each node in the F1 layer.
			// Send input signal from f1a to the f1b layer.
			f1a.CopyTo(f1b);

			// Compute net input for each node in the f2 layer.
			for (int i = 0; i < patternsCount; i++)
			{
				for (int j = 0; j < neuronsCount; j++)
				{
					f2[i] += bw[i, j] * f1a[j];
				}
			}

			bool reset = true;
			int limit = resetLimit;
			//Console.Write("sss");
			while (reset == true)
			{
				// Determine the largest value of the f2 nodes.
				f2Max = f2.IndexOf(f2.Max());
				//Console.Write(f2Max);

				// Recompute the f1a to f1b activations (perform AND function).
				for (int i = 0; i < neuronsCount; i++)
				{
					f1b[i] = f1a[i] * (int)Math.Floor(tw[f2Max, i]);
				}

				// Compute sum of input pattern.
				activationSum = f1b.Sum();

				reset = TestForReset(activationSum, inputSum, f2Max, train);

				if (--limit == 0)
					return -1;
			}
			//Console.WriteLine("");


			if (train) UpdateWeights(activationSum, f2Max);

			return f2Max;
		}

		private bool TestForReset(int activationSum, int inputSum, int f2Max, bool train)
		{
			if ((double)activationSum / (double)inputSum >= (train ? TRAIN_VIGILANCE : TEST_VIGILANCE))
			{
				return false;     // Candidate is accepted.
			}
			else
			{
				f2[f2Max] = -1.0; // Inhibit.
				return true;      // Candidate is rejected.
			}
		}

		private void UpdateWeights(int activationSum, int f2Max)
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
	}
}
