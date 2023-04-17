using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// For image management
using System.Drawing;
using System.Diagnostics;

namespace Filters
{
    /// <summary>
    /// Gray scale methods
    /// </summary>
    public class Filters
    {
        /// <summary>
        /// Apply gray scale filter
        /// </summary>
        /// <param name="source">image location</param>
        /// <returns>Image in gray scale</returns>
        public static Bitmap GrayScale(Bitmap source)
        {
            Bitmap bm = new Bitmap(source.Width, source.Height);
            for (int y = 0; y < bm.Height; y++)
            {
                for (int x = 0; x < bm.Width; x++)
                {
                    Color c = source.GetPixel(x, y);
                    int average = (Convert.ToInt32(c.R) + Convert.ToInt32(c.G) + Convert.ToInt32(c.B)) / 3;
                    bm.SetPixel(x, y, Color.FromArgb(average, average, average));
                }
            }
            return bm;
        }

        public static Bitmap GrayScaleParallel4(Bitmap source)
        {
            Rectangle rect = new Rectangle(0, 0, source.Width, source.Height / 2);
            Bitmap firstHalf = source.Clone(rect, source.PixelFormat);

            rect = new Rectangle(0, source.Height / 2, source.Width, source.Height / 2);
            Bitmap secondHalf = source.Clone(rect, source.PixelFormat);

            Parallel.Invoke(
                () =>
                {
                    Parallel.For(0, firstHalf.Width, i => {
                        Parallel.For(0, firstHalf.Height, j => {
                            Color c = firstHalf.GetPixel(x, y);
                            int average = (Convert.ToInt32(c.R) + Convert.ToInt32(c.G) + Convert.ToInt32(c.B)) / 3;
                            firstHalf.SetPixel(x, y, Color.FromArgb(average, average, average));
                        });
                    });
                },
                () =>
                {
                    Parallel.For(0, secondHalf.Width, i => {
                        Parallel.For(0, secondHalf.Height, j => {
                            Color c = secondHalf.GetPixel(x, y);
                            int average = (Convert.ToInt32(c.R) + Convert.ToInt32(c.G) + Convert.ToInt32(c.B)) / 3;
                            secondHalf.SetPixel(x, y, Color.FromArgb(average, average, average));
                        });
                    });
                }
            );
            Bitmap bm = new Bitmap(firstHalf.Width, firstHalf.Height + firstHalf.Height);
            Graphics g = Graphics.FromImage(bm);
            g.DrawImageUnscaled(firstHalf, 0, 0);
            g.DrawImageUnscaled(secondHalf, 0, firstHalf.Height);

            return bm;
        }
    



        public static Bitmap GaussianBlur(Bitmap source)
            {
                int radius = 5;

                //Crea un nuevo bitmap con las dimensiones de la imagen proporcionada
                Bitmap newImage = new Bitmap(source.Width, source.Height);
            

                //Crea una copia de la imagen recibida.
                using (Graphics graphics = Graphics.FromImage(newImage))
                {
                    graphics.DrawImage(source, new Rectangle(0, 0, source.Width, source.Height),
                        new Rectangle(0, 0, source.Width, source.Height), GraphicsUnit.Pixel);
                }

                // Basicamente no está aplicando el filtro entonces se retorna la imagen
                if (radius < 1) return newImage;



                int size = radius * 2 + 1;

                //Matriz para guardar los coeficientes de la ecuación.
                double[,] gaussMatrix = new double[size, size];

                double sum = 0;


                //Recorre toda la matriz asignando valores a cada uno de los pixeles
            
                for (int y = -radius; y <= radius; y++)
                {
                    for (int x = -radius; x <= radius; x++)
                    {
                        //posicion entre el elemento actual y el centro de la matriz
                        double distance = Math.Sqrt(x * x + y * y);

                        //valor del elemento actual al aplicar la ecuacion guassiana
                        double weight = Math.Exp(-(distance * distance) / (2 * radius * radius));

                        //se almacena el valor 
                        gaussMatrix[y + radius, x + radius] = weight;

                        sum += weight;
                    }
                }


                //Empieza a recorrer por la imagen de manera horizontal bajando por cada fila

                for (int y = 0; y < newImage.Height; y++)
                {
                    for (int x = 0; x < newImage.Width; x++)
                    {
                        //se va obteniendo cada pixel dentro de la image
                        Color pixel = newImage.GetPixel(x, y);

                        double red = 0, green = 0, blue = 0;

                        // Se hace un recorrido por los pixeles adyacente al pixel actual que se esta trabajando
                        for (int gaussY = -radius; gaussY <= radius; gaussY++)
                        {
                            for (int gaussX = -radius; gaussX <= radius; gaussX++)
                            {
                                if (x + gaussX < 0 || x + gaussX >= newImage.Width || y + gaussY < 0 || y + gaussY >= newImage.Height) continue;

                                Color gaussPixel = newImage.GetPixel(x + gaussX, y + gaussY);
                                red += gaussPixel.R * gaussMatrix[gaussY + radius, gaussX + radius];
                                green += gaussPixel.G * gaussMatrix[gaussY + radius, gaussX + radius];
                                blue += gaussPixel.B * gaussMatrix[gaussY + radius, gaussX + radius];
                            }
                        }

                        red /= sum;
                        green /= sum;
                        blue /= sum;

                        newImage.SetPixel(x, y, Color.FromArgb(pixel.A, (int)red, (int)green, (int)blue));
                    }
                }


                return newImage;
            }



        public static Bitmap TwoPartsParallelGaussianBlur(Bitmap source)
        {
            int radius = 5;

            //Crea un nuevo bitmap con las dimensiones de la imagen proporcionada
            Bitmap newImage1 = new Bitmap(source.Width, source.Height);
            Bitmap newImage2 = new Bitmap(source.Width, source.Height);

            Bitmap newImage3 = new Bitmap(source.Width, source.Height);


            //Crea una copia de la imagen recibida.
            using (Graphics graphics = Graphics.FromImage(newImage1))
            {
                graphics.DrawImage(source, new Rectangle(0, 0, source.Width, source.Height),
                    new Rectangle(0, 0, source.Width, source.Height), GraphicsUnit.Pixel);
            }

            using (Graphics graphics = Graphics.FromImage(newImage2))
            {
                graphics.DrawImage(source, new Rectangle(0, 0, source.Width, source.Height),
                    new Rectangle(0, 0, source.Width, source.Height), GraphicsUnit.Pixel);
            }

            using (Graphics graphics = Graphics.FromImage(newImage3))
            {
                graphics.DrawImage(source, new Rectangle(0, 0, source.Width, source.Height),
                    new Rectangle(0, 0, source.Width, source.Height), GraphicsUnit.Pixel);
            }

            // Basicamente no está aplicando el filtro entonces se retorna la imagen
            if (radius < 1) return newImage3;

            int size = radius * 2 + 1;

            //Matriz para guardar los coeficientes de la ecuación.
            double[,] gaussMatrix = new double[size, size];

            double sum = 0;

            //Recorre toda la matriz asignando valores a cada uno de los pixeles
            for (int y = -radius; y <= radius; y++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    //posicion entre el elemento actual y el centro de la matriz
                    double distance = Math.Sqrt(x * x + y * y);

                    //valor del elemento actual al aplicar la ecuacion guassiana
                    double weight = Math.Exp(-(distance * distance) / (2 * radius * radius));

                    //se almacena el valor 
                    gaussMatrix[y + radius, x + radius] = weight;

                    sum += weight;
                }
            }

            //Divide the image into two sections
            int halfWidth1 = newImage1.Width / 2;
            int height1 = newImage1.Height;
            int width1 = newImage1.Width;

            int halfWidth2 = newImage2.Width / 2;
            int height2 = newImage2.Height;
            int width2 = newImage2.Width;

            //Apply the Gaussian blur filter to each section in parallel using Parallel.Invoke
            Parallel.Invoke(
                () =>
                {
                    //Apply the filter to the left section of the image
                    for (int y = 0; y < height1; y++)
                    {
                        for (int x = 0; x < halfWidth1; x++)
                        {
                            //se va obteniendo cada pixel dentro de la image
                            Color pixel = newImage1.GetPixel(x, y);

                            double red = 0, green = 0, blue = 0;

                            // Se hace un recorrido por los pixeles adyacente al pixel actual que se esta trabajando
                            for (int gaussY = -radius; gaussY <= radius; gaussY++)
                            {
                                for (int gaussX = -radius; gaussX <= radius; gaussX++)
                                {
                                    if (x + gaussX < 0 || x + gaussX >= halfWidth1 || y + gaussY < 0 || y + gaussY >= height1) continue;

                                    Color gaussPixel = newImage1.GetPixel(x + gaussX, y + gaussY);
                                    red += gaussPixel.R * gaussMatrix[gaussY + radius, gaussX + radius];
                                    green += gaussPixel.G * gaussMatrix[gaussY + radius, gaussX + radius];
                                    blue += gaussPixel.B * gaussMatrix[gaussY + radius, gaussX + radius];
                                }
                            }

                            red /= sum;
                            green /= sum;
                            blue /= sum;
                            lock (newImage3)
                            {
                                newImage3.SetPixel(x, y, Color.FromArgb(pixel.A, (int)red, (int)green, (int)blue));
                            }
                           
                        }
                    }
                },
                () =>
                {
                    for (int y = 0; y < height2; y++)
                    {
                        for (int x = halfWidth2; x < width2; x++)
                        {
                            //se va obteniendo cada pixel dentro de la image
                            Color pixel = newImage2.GetPixel(x, y);

                            double red = 0, green = 0, blue = 0;

                            // Se hace un recorrido por los pixeles adyacente al pixel actual que se esta trabajando
                            for (int gaussY = -radius; gaussY <= radius; gaussY++)
                            {
                                for (int gaussX = -radius; gaussX <= radius; gaussX++)
                                {
                                    if (x + gaussX < 0 || x + gaussX >= newImage2.Width || y + gaussY < 0 || y + gaussY >= height2) continue;

                                    Color gaussPixel = newImage2.GetPixel(x + gaussX, y + gaussY);
                                    red += gaussPixel.R * gaussMatrix[gaussY + radius, gaussX + radius];
                                    green += gaussPixel.G * gaussMatrix[gaussY + radius, gaussX + radius];
                                    blue += gaussPixel.B * gaussMatrix[gaussY + radius, gaussX + radius];
                                }
                            }

                            red /= sum;
                            green /= sum;
                            blue /= sum;

                            lock (newImage3)
                            {
                                newImage3.SetPixel(x, y, Color.FromArgb(pixel.A, (int)red, (int)green, (int)blue));
                            }
                        }
                    }

                });
            return newImage3;
        }



        public static Bitmap FourPartsParallelGaussianBlur(Bitmap source)
        {
            int radius = 5;

            //Crea un nuevo bitmap con las dimensiones de la imagen proporcionada
            Bitmap newImage1 = new Bitmap(source.Width, source.Height);
            Bitmap newImage2 = new Bitmap(source.Width, source.Height);
            Bitmap newImage3 = new Bitmap(source.Width, source.Height);
            Bitmap newImage4 = new Bitmap(source.Width, source.Height);

            Bitmap newImage5 = new Bitmap(source.Width, source.Height);


            //Crea una copia de la imagen recibida.
            using (Graphics graphics = Graphics.FromImage(newImage1))
            {
                graphics.DrawImage(source, new Rectangle(0, 0, source.Width, source.Height),
                    new Rectangle(0, 0, source.Width, source.Height), GraphicsUnit.Pixel);
            }

            using (Graphics graphics = Graphics.FromImage(newImage2))
            {
                graphics.DrawImage(source, new Rectangle(0, 0, source.Width, source.Height),
                    new Rectangle(0, 0, source.Width, source.Height), GraphicsUnit.Pixel);
            }

            using (Graphics graphics = Graphics.FromImage(newImage3))
            {
                graphics.DrawImage(source, new Rectangle(0, 0, source.Width, source.Height),
                    new Rectangle(0, 0, source.Width, source.Height), GraphicsUnit.Pixel);
            }

            using (Graphics graphics = Graphics.FromImage(newImage4))
            {
                graphics.DrawImage(source, new Rectangle(0, 0, source.Width, source.Height),
                    new Rectangle(0, 0, source.Width, source.Height), GraphicsUnit.Pixel);
            }
            using (Graphics graphics = Graphics.FromImage(newImage5))
            {
                graphics.DrawImage(source, new Rectangle(0, 0, source.Width, source.Height),
                    new Rectangle(0, 0, source.Width, source.Height), GraphicsUnit.Pixel);
            }


            // Basicamente no está aplicando el filtro entonces se retorna la imagen
            if (radius < 1) return newImage3;

            int size = radius * 2 + 1;

            //Matriz para guardar los coeficientes de la ecuación.
            double[,] gaussMatrix = new double[size, size];

            double sum = 0;

            //Recorre toda la matriz asignando valores a cada uno de los pixeles
            for (int y = -radius; y <= radius; y++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    //posicion entre el elemento actual y el centro de la matriz
                    double distance = Math.Sqrt(x * x + y * y);

                    //valor del elemento actual al aplicar la ecuacion guassiana
                    double weight = Math.Exp(-(distance * distance) / (2 * radius * radius));

                    //se almacena el valor 
                    gaussMatrix[y + radius, x + radius] = weight;

                    sum += weight;
                }
            }
            
            //Divide the image into two sections
            int halfWidth1 = newImage1.Width / 2;
            int width1 = newImage1.Width;
            int height1 = newImage1.Height;
            int halfHeight1 = newImage1.Height/2;

            int halfWidth2 = newImage1.Width / 2;
            int width2 = newImage1.Width;
            int height2 = newImage1.Height;
            int halfHeight2 = newImage1.Height / 2;
            

            int halfWidth3 = newImage1.Width / 2;
            int width3 = newImage1.Width;
            int height3 = newImage1.Height;
            int halfHeight3 = newImage1.Height / 2;


            int halfWidth4 = newImage1.Width / 2;
            int width4 = newImage1.Width;
            int height4 = newImage1.Height;
            int halfHeight4 = newImage1.Height / 2;


            

            //Apply the Gaussian blur filter to each section in parallel using Parallel.Invoke
            Parallel.Invoke(
                () =>
                {
                    //Apply the filter to the left section of the image
                    for (int y = 0; y < halfHeight1; y++)
                    {
                        for (int x = 0; x < halfWidth1; x++)
                        {
                            //se va obteniendo cada pixel dentro de la image
                            Color pixel = newImage1.GetPixel(x, y);

                            double red = 0, green = 0, blue = 0;

                            // Se hace un recorrido por los pixeles adyacente al pixel actual que se esta trabajando
                            for (int gaussY = -radius; gaussY <= radius; gaussY++)
                            {
                                for (int gaussX = -radius; gaussX <= radius; gaussX++)
                                {
                                    if (x + gaussX < 0 || x + gaussX >= halfWidth1 || y + gaussY < 0 || y + gaussY >= halfHeight1) continue;

                                    Color gaussPixel = newImage1.GetPixel(x + gaussX, y + gaussY);
                                    red += gaussPixel.R * gaussMatrix[gaussY + radius, gaussX + radius];
                                    green += gaussPixel.G * gaussMatrix[gaussY + radius, gaussX + radius];
                                    blue += gaussPixel.B * gaussMatrix[gaussY + radius, gaussX + radius];
                                }
                            }

                            red /= sum;
                            green /= sum;
                            blue /= sum;
                            lock (newImage5)
                            {
                                newImage5.SetPixel(x, y, Color.FromArgb(pixel.A, (int)red, (int)green, (int)blue));
                            }

                        }
                    }
                },
                () =>
                {
                    for (int y = halfHeight2; y < height2; y++)
                    {
                        for (int x = 0; x < halfWidth2; x++)
                        {
                            //se va obteniendo cada pixel dentro de la image
                            Color pixel = newImage2.GetPixel(x, y);

                            double red = 0, green = 0, blue = 0;

                            // Se hace un recorrido por los pixeles adyacente al pixel actual que se esta trabajando
                            for (int gaussY = -radius; gaussY <= radius; gaussY++)
                            {
                                for (int gaussX = -radius; gaussX <= radius; gaussX++)
                                {
                                    if (x + gaussX < 0 || x + gaussX >= halfWidth2 || y + gaussY < 0 || y + gaussY >= height2) continue;

                                    Color gaussPixel = newImage2.GetPixel(x + gaussX, y + gaussY);
                                    red += gaussPixel.R * gaussMatrix[gaussY + radius, gaussX + radius];
                                    green += gaussPixel.G * gaussMatrix[gaussY + radius, gaussX + radius];
                                    blue += gaussPixel.B * gaussMatrix[gaussY + radius, gaussX + radius];
                                }
                            }

                            red /= sum;
                            green /= sum;
                            blue /= sum;
                            lock (newImage5)
                            {
                                newImage5.SetPixel(x, y, Color.FromArgb(pixel.A, (int)red, (int)green, (int)blue));
                            }

                        }
                    }



                },
                 () =>
                 {
                     //Apply the filter to the left section of the image
                     for (int y = 0; y < halfHeight3; y++)
                     {
                         for (int x = halfWidth3; x < width3; x++)
                         {
                             //se va obteniendo cada pixel dentro de la image
                             Color pixel = newImage3.GetPixel(x, y);

                             double red = 0, green = 0, blue = 0;

                             // Se hace un recorrido por los pixeles adyacente al pixel actual que se esta trabajando
                             for (int gaussY = -radius; gaussY <= radius; gaussY++)
                             {
                                 for (int gaussX = -radius; gaussX <= radius; gaussX++)
                                 {
                                     if (x + gaussX < 0 || x + gaussX >= width3 || y + gaussY < 0 || y + gaussY >= halfHeight3) continue;

                                     Color gaussPixel = newImage3.GetPixel(x + gaussX, y + gaussY);
                                     red += gaussPixel.R * gaussMatrix[gaussY + radius, gaussX + radius];
                                     green += gaussPixel.G * gaussMatrix[gaussY + radius, gaussX + radius];
                                     blue += gaussPixel.B * gaussMatrix[gaussY + radius, gaussX + radius];
                                 }
                             }

                             red /= sum;
                             green /= sum;
                             blue /= sum;
                             lock (newImage5)
                             {
                                 newImage5.SetPixel(x, y, Color.FromArgb(pixel.A, (int)red, (int)green, (int)blue));
                             }

                         }
                     }
                 },



                  () =>
                  {
                      //Apply the filter to the left section of the image
                      for (int y = halfHeight4; y < height4; y++)
                      {
                          for (int x = halfWidth4; x < width4; x++)
                          {
                              //se va obteniendo cada pixel dentro de la image
                              Color pixel = newImage4.GetPixel(x, y);

                              double red = 0, green = 0, blue = 0;

                              // Se hace un recorrido por los pixeles adyacente al pixel actual que se esta trabajando
                              for (int gaussY = -radius; gaussY <= radius; gaussY++)
                              {
                                  for (int gaussX = -radius; gaussX <= radius; gaussX++)
                                  {
                                      if (x + gaussX < 0 || x + gaussX >= width4 || y + gaussY < 0 || y + gaussY >= height4) continue;

                                      Color gaussPixel = newImage4.GetPixel(x + gaussX, y + gaussY);
                                      red += gaussPixel.R * gaussMatrix[gaussY + radius, gaussX + radius];
                                      green += gaussPixel.G * gaussMatrix[gaussY + radius, gaussX + radius];
                                      blue += gaussPixel.B * gaussMatrix[gaussY + radius, gaussX + radius];
                                  }
                              }

                              red /= sum;
                              green /= sum;
                              blue /= sum;
                              lock (newImage5)
                              {
                                  newImage5.SetPixel(x, y, Color.FromArgb(pixel.A, (int)red, (int)green, (int)blue));
                              }

                          }
                      }
                  }


                );
            return newImage5;
        }





        public static Bitmap AntiAlias(Bitmap bitmap)
        {
            // Create a new bitmap to hold the anti-aliased image
            Bitmap antiAliasedBitmap = new Bitmap(bitmap.Width, bitmap.Height);

            // Apply the anti-aliasing algorithm to each pixel
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    Color color = GetAntiAliasingPixel(bitmap, x, y);
                    antiAliasedBitmap.SetPixel(x, y, color);
                }
            }

            // Return the anti-aliased bitmap
            return antiAliasedBitmap;
        }

        private static Color GetAntiAliasingPixel(Bitmap bitmap, int x, int y)
        {
            // Check if the pixel is at the edge of the bitmap
            if (x == 0 || y == 0 || x == bitmap.Width - 1 || y == bitmap.Height - 1)
            {
                // If so, return the pixel color unchanged
                return bitmap.GetPixel(x, y);
            }

            // Get the average color of the surrounding pixels using recursion
            Color c1 = GetAntiAliasingPixel(bitmap, x - 1, y - 1);
            Color c2 = GetAntiAliasingPixel(bitmap, x, y - 1);
            Color c3 = GetAntiAliasingPixel(bitmap, x + 1, y - 1);
            Color c4 = GetAntiAliasingPixel(bitmap, x - 1, y);
            Color c5 = GetAntiAliasingPixel(bitmap, x + 1, y);
            Color c6 = GetAntiAliasingPixel(bitmap, x - 1, y + 1);
            Color c7 = GetAntiAliasingPixel(bitmap, x, y + 1);
            Color c8 = GetAntiAliasingPixel(bitmap, x + 1, y + 1);

            int r = (c1.R + c2.R + c3.R + c4.R + c5.R + c6.R + c7.R + c8.R) / 8;
            int g = (c1.G + c2.G + c3.G + c4.G + c5.G + c6.G + c7.G + c8.G) / 8;
            int b = (c1.B + c2.B + c3.B + c4.B + c5.B + c6.B + c7.B + c8.B) / 8;

            return Color.FromArgb(r, g, b);
        }

    }
}

