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
        static int[,] teachingSet;
        static int[,] slightlyDisturbed;
        static int[,] heavilyDisturbed;
        static int[,] negative;

        static int[,] disturbed;

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

        readonly int[,] patternsChangeOrder = {
            { 0,0,0,0,0,1,0,0,0,0,0,0,1,1,0,0,0,0,0,1,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0 },
            { 0,0,1,1,1,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,1,1,1,1,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,1,1,1,0,0 },
            { 0,0,1,0,0,1,0,0,0,0,1,0,0,1,0,0,0,0,1,0,0,1,0,0,0,0,1,1,1,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0 },
            { 0,0,1,1,1,1,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,1,1,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,1,1,1,1,0,0 },
            { 0,0,1,1,1,1,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,1,1,1,0,0,0,0,1,0,0,1,0,0,0,0,1,0,0,1,0,0,0,0,1,0,0,1,0,0,0,0,1,1,1,1,0,0 },
            { 0,0,1,1,1,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0 },
            { 0,0,1,1,1,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,1,1,1,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,1,1,1,1,0,0 },
            { 0,0,1,1,1,1,0,0,0,0,1,0,0,1,0,0,0,0,1,0,0,1,0,0,0,0,1,1,1,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,1,1,1,1,0,0 },
            { 0,0,1,1,1,1,0,0,0,0,1,0,0,1,0,0,0,0,1,0,0,1,0,0,0,0,1,0,0,1,0,0,0,0,1,0,0,1,0,0,0,0,1,0,0,1,0,0,0,0,1,0,0,1,0,0,0,0,1,1,1,1,0,0 },
            { 0,0,1,1,1,1,0,0,0,0,1,0,0,1,0,0,0,0,1,0,0,1,0,0,0,0,1,1,1,1,0,0,0,0,1,0,0,1,0,0,0,0,1,0,0,1,0,0,0,0,1,0,0,1,0,0,0,0,1,1,1,1,0,0 }};

        readonly int[] patternsOrder = { 1, 2, 4, 5, 6, 7, 3, 9, 0, 8 };

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

            int[,] newDisturbed = patterns;
            string[] stringDisturbed = new string[patterns.Length / 64]; ;

            for (int i = 0; i < patterns.Length / 64; i++)
            {
                Random rnd = new Random();
                int r1 = rnd.Next(1, 63);
                int r2 = rnd.Next(1, 63);
                //int r3 = rnd.Next(1, 63);
                //int r4 = rnd.Next(1, 63);
                //int r5 = rnd.Next(1, 63);
                //int r6 = rnd.Next(1, 63);
                //int r7 = rnd.Next(1, 63);
                //int r8 = rnd.Next(1, 63);
                //int r9 = rnd.Next(1, 63);
                //int r10 = rnd.Next(1, 63);
                //int r11 = rnd.Next(1, 63);
                //int r12 = rnd.Next(1, 63);
                //int r13 = rnd.Next(1, 63);
                //int r14 = rnd.Next(1, 63);
                //int r15 = rnd.Next(1, 63);
                //int r16 = rnd.Next(1, 63);
                //int r17 = rnd.Next(1, 63);
                //int r18 = rnd.Next(1, 63);
                //int r19 = rnd.Next(1, 63);
                //int r20 = rnd.Next(1, 63);
                //int r21 = rnd.Next(1, 63);
                //int r22 = rnd.Next(1, 63);
                //int r23 = rnd.Next(1, 63);
                //int r24 = rnd.Next(1, 63);
                //int r25 = rnd.Next(1, 63);

                newDisturbed[i, r1] = patterns[i, r1] == 0 ? 1 : 0;
                newDisturbed[i, r2] = patterns[i, r2] == 0 ? 1 : 0;
                //newDisturbed[i, r3] = patterns[i, r3] == 0 ? 1 : 0;
                //newDisturbed[i, r4] = patterns[i, r4] == 0 ? 1 : 0;
                //newDisturbed[i, r5] = patterns[i, r5] == 0 ? 1 : 0;
                //newDisturbed[i, r6] = patterns[i, r6] == 0 ? 1 : 0;
                //newDisturbed[i, r7] = patterns[i, r7] == 0 ? 1 : 0;
                //newDisturbed[i, r8] = patterns[i, r8] == 0 ? 1 : 0;
                //newDisturbed[i, r9] = patterns[i, r9] == 0 ? 1 : 0;
                //newDisturbed[i, r10] = patterns[i, r10] == 0 ? 1 : 0;
                //newDisturbed[i, r11] = patterns[i, r11] == 0 ? 1 : 0;
                //newDisturbed[i, r12] = patterns[i, r12] == 0 ? 1 : 0;
                //newDisturbed[i, r13] = patterns[i, r13] == 0 ? 1 : 0;
                //newDisturbed[i, r14] = patterns[i, r14] == 0 ? 1 : 0;
                //newDisturbed[i, r15] = patterns[i, r15] == 0 ? 1 : 0;
                //newDisturbed[i, r16] = patterns[i, r16] == 0 ? 1 : 0;
                //newDisturbed[i, r17] = patterns[i, r17] == 0 ? 1 : 0;
                //newDisturbed[i, r18] = patterns[i, r18] == 0 ? 1 : 0;
                //newDisturbed[i, r19] = patterns[i, r19] == 0 ? 1 : 0;
                //newDisturbed[i, r20] = patterns[i, r20] == 0 ? 1 : 0;
                //newDisturbed[i, r21] = patterns[i, r21] == 0 ? 1 : 0;
                //newDisturbed[i, r22] = patterns[i, r22] == 0 ? 1 : 0;
                //newDisturbed[i, r23] = patterns[i, r23] == 0 ? 1 : 0;
                //newDisturbed[i, r24] = patterns[i, r24] == 0 ? 1 : 0;
                //newDisturbed[i, r25] = patterns[i, r25] == 0 ? 1 : 0;
                //newDisturbed.SetValue(patterns[i, random] == 0 ? 1 : 0, i, random);



                stringDisturbed[i] = string.Join("", newDisturbed.GetRow(i));
            }


            System.IO.File.WriteAllLines(@"C:\Patterns\symulacje\disturbed.txt", stringDisturbed);
            disturbed = newDisturbed;
            vm.patterns = disturbed;

            var manyPatterns = System.IO.File.ReadAllLines(@"C:\Patterns\newPatterns012zmiany.txt");

            int[,] patternsNew = new int[manyPatterns.Count(), 64];

            int u = 0;
            foreach (string line in manyPatterns)
            {
                int[] lineArray = line.Select(c => Convert.ToInt32(c.ToString())).ToArray();
                patternsNew.SetRow(u, lineArray);
                u++;
            }

            teachingSet = patternsNew;

            var slightlyDisturbedPatterns = System.IO.File.ReadAllLines(@"C:\Patterns\orderedLekko2.txt");

            int[,] newPatterns = new int[slightlyDisturbedPatterns.Count(), 64];

            int w = 0;
            foreach (string line in slightlyDisturbedPatterns)
            {
                int[] lineArray = line.Select(c => Convert.ToInt32(c.ToString())).ToArray();
                newPatterns.SetRow(w, lineArray);
                w++;
            }

            slightlyDisturbed = newPatterns;

            // vm.patterns = slightlyDisturbed;

            var heavilyDisturbedPatterns = System.IO.File.ReadAllLines(@"C:\Patterns\orderedMocno.txt");

            int[,] newPatterns2 = new int[heavilyDisturbedPatterns.Count(), 64];

            int x = 0;
            foreach (string line in heavilyDisturbedPatterns)
            {
                int[] lineArray = line.Select(c => Convert.ToInt32(c.ToString())).ToArray();
                newPatterns2.SetRow(x, lineArray);
                x++;
            }

            heavilyDisturbed = newPatterns2;

            //vm.patterns = heavilyDisturbed;

            var negatives = System.IO.File.ReadAllLines(@"C:\Patterns\newPatternsNegative.txt");

            int[,] negativePatterns = new int[negatives.Count(), 64];

            int v = 0;
            foreach (string line in negatives)
            {
                int[] lineArray = line.Select(c => Convert.ToInt32(c.ToString())).ToArray();
                negativePatterns.SetRow(v, lineArray);
                v++;
            }

            negative = negativePatterns;

            //vm.patterns = negative;

            return View(vm);
        }

        [HttpPost]
        public ActionResult TestMany(InputOutputViewModel vm)
        {
            vm.HopfieldOutput = new MatrixViewModel(64);
            vm.ARTOutput = new MatrixViewModel(64);
            vm.patterns = disturbed;

            var testPatterns = disturbed;

            var vector = vm.Input.neurons;
            var x = string.Join("", vector);
            var y = string.Join(",", vector);
            int failCountHP = 0;
            int failCountART = 0;

            var outputVectorHP = new int[testPatterns.Length / 64];
            var outputVectorART = new int[testPatterns.Length / 64];
            var outputVectorART2 = new int[testPatterns.Length / 64];

            if (hp == null || art == null || hp.trained == false || art.trained == false)
            {
                ViewBag.Message = "You have to train neural networks before testing.";
                return View("Index", vm);
            }

            string[] stringOupputART;
            for (int i = 0; i < testPatterns.Length / 64; i++)
            {
                var outMatrixHP = hp.Test(testPatterns.GetRow(i));
                var outART = art.Test(testPatterns.GetRow(i));
                var outART2 = art2.Test(testPatterns.GetRow(i));
                var outHP = hp.NeuronOutput(outMatrixHP, patterns);

                if (outART >= 10) outART = outART % 10;
                if (outHP >= 10) outHP = outHP % 10;
                if (outART2 >= 10) outART2 = outART2 % 10;

                //outHP = patternsOrder.IndexOf(outHP);
                outART = outART == -1 ? -1 : patternsOrder[outART];
                outART2 = outART2 == -1 ? -1 : patternsOrder[outART2];

                //vm.HopfieldOutput.neurons = outputHP;
                outputVectorHP[i] = outHP;
                outputVectorART[i] = outART;
                outputVectorART2[i] = outART2;

                if (outHP != i) failCountHP++;
                if (outART != i) failCountART++;
            }

            string stringHP = string.Join("", outputVectorHP);
            string stringART = string.Join("", outputVectorART);
            string stringFailCount = $"HP: {failCountHP}, ART: {failCountART}";

            System.IO.File.WriteAllText(@"C:\Patterns\responseHP.txt", stringHP);
            System.IO.File.WriteAllText(@"C:\Patterns\responseART.txt", stringART);

            System.IO.File.WriteAllText(@"C:\Patterns\symulacje\responseHP.txt", stringHP);
            System.IO.File.WriteAllText(@"C:\Patterns\symulacje\responseART.txt", stringART);

            System.IO.File.WriteAllText(@"C:\Patterns\symulacje\bledy.txt", stringFailCount);

            var outputMatrixHP = hp.Test(vm.Input.neurons);
            var outputART = art.Test(vm.Input.neurons);
            //var outputART = art2.Test(vm.Input.neurons);
            var outputHP = hp.NeuronOutput(outputMatrixHP, teachingSet);

            if (outputART >= 10) outputART = outputART % 10;
            if (outputHP >= 10) outputHP = outputHP % 10;

            if (outputHP == -1)
                ViewBag.Message = "Hopfield cannot recognize pattern.";
            else
                vm.HopfieldOutput.neurons = patternsChangeOrder.GetRow(outputHP);

            if (outputART == -1)
                ViewBag.Message = "ART cannot recognize pattern.";
            else
                vm.ARTOutput.neurons = patternsChangeOrder.GetRow(outputART);

            return View("Index", vm);
        }

        [HttpPost]
        public ActionResult TrainMany(InputOutputViewModel vm)
        {
            vm.HopfieldOutput = new MatrixViewModel(64);
            vm.ARTOutput = new MatrixViewModel(64);
            vm.patterns = patterns;


            // trenowanie Hopfielda
            hp = new Hopfield();

            hp.TrainByPseudoInverse(patterns);

            //trenowanie ART
            art = new ART1();
            art.Train(teachingSet);
            //art.Train(patterns);

            art2 = new ART();
            art2.Train(teachingSet);

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
