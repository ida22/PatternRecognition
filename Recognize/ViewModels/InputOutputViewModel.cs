namespace Recognize.ViewModels
{
    public class InputOutputViewModel
    {
        public MatrixViewModel Input { get; set; }
        public MatrixViewModel HopfieldOutput { get; set; }
        public MatrixViewModel ARTOutput { get; set; }

        public double[,] Weights { get; set; }
    }
}