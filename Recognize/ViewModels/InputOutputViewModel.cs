namespace Recognize.ViewModels
{
    public class InputOutputViewModel
    {
        public MatrixViewModel Input { get; set; }
        public MatrixViewModel Output { get; set; }

        public double[,] Weights { get; set; }
    }
}