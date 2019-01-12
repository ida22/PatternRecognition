namespace Recognize.ViewModels
{
    public class InputOutputViewModel
    {
        public MatrixViewModel Input { get; set; }
        public MatrixViewModel HopfieldOutput { get; set; }
        public MatrixViewModel ARTOutput { get; set; }

        public int[,] patterns { get; set; }
        public int[] One { get; set; }
        public int[] Two { get; set; }
        public int[] Three { get; set; }
        public int[] Four { get; set; }
        public int[] Five { get; set; }
        public int[] Six { get; set; }
        public int[] Seven { get; set; }
        public int[] Eight { get; set; }
        public int[] Nine { get; set; }

        public double[,] Weights { get; set; }
    }
}