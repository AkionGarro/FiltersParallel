using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Namespace for get some statistics 
using System.Diagnostics;
// Allow us to access into HD/SSD
using System.IO;
// For image management
using System.Drawing;



namespace ImagesConcurrentExample
{
    class Program
    {
        #region Global variables
        //The arroba at the beginning is for indicate to .NET that backslash character belongs to string
        static string imagePath = @"D:\FiltrosImages\Old";
        // Path for "filtering" result (new images)
        static string newImagePath = @"D:\FiltrosImages\New";
        //For get the execution time
        static Stopwatch stopWatch = new Stopwatch();
        #endregion Global variables
        static void Main(string[] args)
        {
            /*for(int i = 1; i < 7; i++)
            {
                ParallelMethod(i);
            }*/
            for (int i = 1; i < 7; i++)
            {
                SecuencialMethod(i);
            }
            Console.Read();

        }

            /*
            static void Main(string[] args)
            {

                int type;
                int option;
                Console.WriteLine("Seleccione una opción:");

                Console.WriteLine("1: Secuencial");
                Console.WriteLine("2: Paralelo");
                Console.WriteLine("3: Salir");
                Console.Write("Ingrese su opción: ");

                type = Convert.ToInt32(Console.ReadLine());

                if (type == 3)
                {

                    System.Environment.Exit(0);
                }


                Console.Write("\n");
                Console.Write("\n");
                Console.WriteLine("Seleccione una opción:");
                Console.WriteLine("1: GrayScale");
                Console.WriteLine("2: Gaussian Blur");
                Console.WriteLine("3: Anti aliasing");
                Console.WriteLine("4: GrayScaleParallel");
                Console.WriteLine("5: Gaussian Blur Parallel");
                Console.WriteLine("6: Anti aliasing Parallel");
                Console.Write("Ingrese su opción: ");
                option = Convert.ToInt32(Console.ReadLine());

                switch (type)
                {
                    //Sequential images
                    case 1:
                        if(option > 0 && option<7)
                        {
                            SecuencialMethod(option);
                        }
                        else
                        {
                            Console.WriteLine("Error ingrese una opcion valida");
                            break;
                        }

                        break;

                    //Parallel Images
                    case 2:

                        if (option > 0 && option < 7)
                        {
                            ParallelMethod(option);
                        }
                        else
                        {
                            Console.WriteLine("Error ingrese una opcion valida");
                            break;
                        }

                        break;

                    case 3:
                        break;

                    default:
                        Console.WriteLine("Opción inválida");
                        break;
                }




            }
            */

            /// <summary>
            /// Apply the gray scale filter in secuencial way
            /// </summary>
            static void SecuencialMethod(int option)
        {
            //Start to count time

            stopWatch.Start();
            //Secuencial method that apply grayscale filter to whole folder (images)
            foreach (string file in Directory.GetFiles(imagePath))
            {
                Bitmap bitmap = (Bitmap)Image.FromFile(file);
                if (option == 1)
                {
                    bitmap = Filters.Filters.GrayScale(bitmap);
                    bitmap.Save(newImagePath + "\\Sequential\\GrayScale\\" + Path.GetFileNameWithoutExtension(file) + Guid.NewGuid().ToString() + ".jpg");
                    Console.WriteLine("Finish Sequential GrayScale");
                }
                else if(option == 2)
                {
                    bitmap = Filters.Filters.GaussianBlur(bitmap);
                    bitmap.Save(newImagePath + "\\Sequential\\GaussianBlur\\" + Path.GetFileNameWithoutExtension(file) + Guid.NewGuid().ToString() + ".jpg");
                    Console.WriteLine("Finish Sequential GaussianBlur");
                }
                else if (option == 3)
                {
                    bitmap = Filters.Filters.AntiAlias(bitmap);
                    bitmap.Save(newImagePath + "\\Sequential\\AntiAlias\\" + Path.GetFileNameWithoutExtension(file) + Guid.NewGuid().ToString() + ".jpg");
                    Console.WriteLine("Finish Sequential AntiAlias");

                }
                else if (option == 4)
                {
                    bitmap = Filters.Filters.GrayScaleParallel(bitmap);
                    bitmap.Save(newImagePath + "\\Sequential\\GrayScaleParallel\\" + Path.GetFileNameWithoutExtension(file) + Guid.NewGuid().ToString() + ".jpg");
                    Console.WriteLine("Finish Sequential GrayScaleParallel");
                }

                else if (option == 5)
                {
                    bitmap = Filters.Filters.FourPartsParallelGaussianBlur(bitmap);
                    bitmap.Save(newImagePath + "\\Sequential\\GaussianBlurParallel\\" + Path.GetFileNameWithoutExtension(file) + Guid.NewGuid().ToString() + ".jpg");
                    Console.WriteLine("Finish Sequential GaussianBlurParallel");
                }


                else if (option == 6)
                {
                    bitmap = Filters.Filters.AntiAliasParallel(bitmap);
                    bitmap.Save(newImagePath + "\\Sequential\\AntiAliasParallel\\" + Path.GetFileNameWithoutExtension(file) + Guid.NewGuid().ToString() + ".jpg");
                    Console.WriteLine("Finish Sequential AntiAliasParallel");
                }



            }
            Console.WriteLine("Execution time = {0} seconds\n", stopWatch.Elapsed.TotalSeconds);
            stopWatch.Reset();

        }

        /// <summary>
        /// Apply the gray scale filter in secuencial way
        /// </summary>
        static void ParallelMethod(int option)
        {
            stopWatch.Start();

            //Secuencial method that apply grayscale filter to whole folder (images)
            Parallel.ForEach(Directory.GetFiles(imagePath), file =>
            {
                Bitmap bitmap = (Bitmap)Image.FromFile(file);
                if (option == 1)
                {
                    bitmap = Filters.Filters.GrayScale(bitmap);
                    bitmap.Save(newImagePath + "\\Parallel\\GrayScale\\" + Path.GetFileNameWithoutExtension(file) + Guid.NewGuid().ToString() + ".jpg");
                    Console.WriteLine("Finish Parallel GrayScale");
                }
                else if (option == 2)
                {
                    bitmap = Filters.Filters.GaussianBlur(bitmap);
                    bitmap.Save(newImagePath + "\\Parallel\\GaussianBlur\\" + Path.GetFileNameWithoutExtension(file) + Guid.NewGuid().ToString() + ".jpg");
                    Console.WriteLine("Finish Parallel GaussianBlur");
                }
                else if (option == 3)
                {
                    bitmap = Filters.Filters.AntiAlias(bitmap);
                    bitmap.Save(newImagePath + "\\Parallel\\Antialias\\" + Path.GetFileNameWithoutExtension(file) + Guid.NewGuid().ToString() + ".jpg");
                    Console.WriteLine("Finish Parallel Antialias");
                }
                else if (option == 4)
                {
                    bitmap = Filters.Filters.GrayScaleParallel(bitmap);
                    bitmap.Save(newImagePath + "\\Parallel\\GrayScaleParallel\\" + Path.GetFileNameWithoutExtension(file) + Guid.NewGuid().ToString() + ".jpg");
                    Console.WriteLine("Finish Parallel GrayScalePararell");
                }

                else if (option == 5)
                {
                    bitmap = Filters.Filters.FourPartsParallelGaussianBlur(bitmap);
                    bitmap.Save(newImagePath + "\\Parallel\\GaussianBlurParallel\\" + Path.GetFileNameWithoutExtension(file) + Guid.NewGuid().ToString() + ".jpg");
                    Console.WriteLine("Finish Parallel GaussianBlurParallel");
                }


                else if (option == 6)
                {
                    bitmap = Filters.Filters.AntiAliasParallel(bitmap);
                    bitmap.Save(newImagePath + "\\Parallel\\AntiAliasParallel\\" + Path.GetFileNameWithoutExtension(file) + Guid.NewGuid().ToString() + ".jpg");
                    Console.WriteLine("Finish Parallel AntiAliasParallel");
                }
            });
            Console.WriteLine("Execution time = {0} seconds\n", stopWatch.Elapsed.TotalSeconds);
            stopWatch.Reset();
        }
    }
}
