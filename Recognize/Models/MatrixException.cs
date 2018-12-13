using System;

namespace Recognize.Models
{
    public class MatrixException : Exception
    {
        public MatrixException(string message)
           : base(message)
        {
        }
    }
}