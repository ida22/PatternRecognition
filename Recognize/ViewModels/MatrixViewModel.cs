using Recognize.Enums;
using Recognize.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Recognize.ViewModels
{
    public class MatrixViewModel
    {
        public int[] neurons { get; set; }

		public MatrixViewModel()
		{
			NumbersList = new List<SelectListItem>();
		}

		public MatrixViewModel(int size)
        {
            NumbersList = new List<SelectListItem>();
			neurons = new int[size];
        }

        public IEnumerable<SelectListItem> NumbersList { get; set; }

        public string Number { get; set; }

        public int I { get; set; }
        public int II { get; set; }
        public int III { get; set; }
        public string IV { get; set; }
        public bool V { get; set; }
        public string VI { get; set; }
    }
}
