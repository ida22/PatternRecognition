using Recognize.Models;
using Recognize.ViewModels;
using System.Web.Mvc;
using Accord.Math;
using System.Linq;
using System;

namespace Recognize.Controllers
{
    public class ManyPatternsController : Controller
    {
        static int[,] superPatterns;
        readonly int[,] patterns = {
            { 0,0,1,1,1,1,0,0,0,0,1,0,0,1,0,0,0,0,1,0,0,1,0,0,0,0,1,0,0,1,0,0,0,0,1,0,0,1,0,0,0,0,1,0,0,1,0,0,0,0,1,0,0,1,0,0,0,0,1,1,1,1,0,0 },
            { 0,0,0,0,0,1,0,0,0,0,0,0,1,1,0,0,0,0,0,1,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0 },
            { 0,0,1,1,1,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,1,1,1,1,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,1,1,1,0,0 },
            { 0,0,1,1,1,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,1,1,1,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,1,1,1,1,0,0 },
            { 0,0,1,0,0,1,0,0,0,0,1,0,0,1,0,0,0,0,1,0,0,1,0,0,0,0,1,1,1,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0 },
            { 0,0,1,1,1,1,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,1,1,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,1,1,1,1,0,0 },
            { 0,0,1,1,1,1,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,1,1,1,0,0,0,0,1,0,0,1,0,0,0,0,1,0,0,1,0,0,0,0,1,0,0,1,0,0,0,0,1,1,1,1,0,0 },
            { 0,0,1,1,1,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0 },
            { 0,0,1,1,1,1,0,0,0,0,1,0,0,1,0,0,0,0,1,0,0,1,0,0,0,0,1,1,1,1,0,0,0,0,1,0,0,1,0,0,0,0,1,0,0,1,0,0,0,0,1,0,0,1,0,0,0,0,1,1,1,1,0,0 },
            { 0,0,1,1,1,1,0,0,0,0,1,0,0,1,0,0,0,0,1,0,0,1,0,0,0,0,1,1,1,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,1,1,1,1,0,0 }};

        static Hopfield hp;
        static ART1 art;
        static ART art2;

        public ActionResult Index(bool? selection)
        {
            ViewBag.Message = "";
            var vm = new InputOutputViewModel();

            vm.Input = new MatrixViewModel(64);
            vm.HopfieldOutput = new MatrixViewModel(64);
            vm.ARTOutput = new MatrixViewModel(64);

            //vm.Weights = new double[64, 64];
            if (selection == true && selection != null)
            {
                vm.Input.neurons = Enumerable.Repeat(1, 64).ToArray();
            }

            vm.patterns = patterns;

            return View(vm);
        }

        [HttpPost]
        public ActionResult TestMany(InputOutputViewModel vm)
        {
            vm.HopfieldOutput = new MatrixViewModel(64);
            vm.ARTOutput = new MatrixViewModel(64);
            vm.patterns = patterns;

            var vector = vm.Input.neurons;
            var x = string.Join("", vector);
            var y = string.Join(",", vector);

            if (hp == null || art == null || hp.trained == false || art.trained == false)
            {
                ViewBag.Message = "You have to train neural networks before testing.";
                return View("Index", vm);
            }

            var outputMatrixHP = hp.Test(vm.Input.neurons);
            //var outputART = art.Test(vm.Input.neurons);
            var outputART = art2.Test(vm.Input.neurons);
            var outputHP = hp.NeuronOutput(outputMatrixHP, superPatterns);

            if (outputART >= 10) outputART = outputART % 10;
            if (outputHP >= 10) outputHP = outputHP % 10;

            //vm.HopfieldOutput.neurons = outputHP;

            if (outputHP == -1)
                ViewBag.Message = "Hopfield cannot recognize pattern.";
            else
                vm.HopfieldOutput.neurons = patterns.GetRow(outputHP);

            if (outputART == -1)
                ViewBag.Message = "ART cannot recognize pattern.";
            else
                vm.ARTOutput.neurons = patterns.GetRow(outputART);

            return View("Index", vm);
        }

        [HttpPost]
        public ActionResult TrainMany(InputOutputViewModel vm)
        {
            vm.HopfieldOutput = new MatrixViewModel(64);
            vm.ARTOutput = new MatrixViewModel(64);
            vm.patterns = patterns;

            var manyPatterns = System.IO.File.ReadAllLines(@"C:\Patterns\newPatterns012.txt");

            int[,] patternsNew = new int[manyPatterns.Count(), 64];

            int u = 0;
            foreach (string line in manyPatterns)
            {
                int[] lineArray = line.Select(c => Convert.ToInt32(c.ToString())).ToArray();
                patternsNew.SetRow(u, lineArray);
                u++;
            }

            superPatterns = patternsNew;


            // trenowanie Hopfielda
            hp = new Hopfield();

            hp.TrainByPseudoInverse(patterns);

            //trenowanie ART
            art = new ART1();
            art.Train(patternsNew);
            //art.Train(patterns);

            art2 = new ART();
            art2.Train(patternsNew);

            int[,] randomPatterns = new int[10, 64];
            int[] numbers = new int[9];
            foreach (int i in numbers)
            {
                var rnd = new Random();
                numbers[i] = rnd.Next(0, 9);
            }




            ViewBag.Message = "Neural networks were trained by patterns.";

            return View("Index", vm);
        }
    }
}
