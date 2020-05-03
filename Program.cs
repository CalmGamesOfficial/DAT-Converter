//DAT1 Converter (Version 1.1) by Calm Games for more information https://github.com/CalmGamesOfficial
using System.Threading.Tasks;
using System.Globalization;
using System.Threading;
using System.Text;
using System.Linq;
using System.IO;
using System;

namespace DAT_1_Converter
{
    public class Program
    {
        //Variables estaticas
        public static string file;
        public static string[] fileElements;

        public static string fileName;
        public static string filePath;
        public static string outputPath;

        public static string emptyValue = string.Empty;

        //Variables Estaticas de procesamiento de arrays de alto nivel
        public static int fileCount;
        public static string[] outputFileName;

        static void Main(string[] args)
        {
            Strings strings = new Strings();

            //Titulo
            Console.Title = "DAT_1 CONVERTER";
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine(" DAT_1 CONVERTER                                                                                                        \n");
            Console.ResetColor();
            
            //Preguntar archivo de entrada
            Console.WriteLine("Introduzca la ruta de el archivo a convertir (Ejemplo: C:\\Users\\...\\Archivo.DAT_1):");
            Console.ForegroundColor = ConsoleColor.Yellow;
            filePath = Console.ReadLine();
            Console.WriteLine("\n");
            Console.ResetColor();
            
            //Si no existe see inicia un bucle hasta que encuentre un archivo
            while (!File.Exists(filePath))
            {
                Console.ResetColor();
                Console.WriteLine("No se ha encontrado el archivo especificado, por favor vuelva a introducirla:");
                Console.ForegroundColor = ConsoleColor.Yellow;
                filePath = Console.ReadLine();
                Console.WriteLine("\n");
            }
            
            //Una vez encontrado se guarda su nombre para su posterior uso
            string[] fileDir = filePath.Split('\\');
            fileName = fileDir[fileDir.Length - 1];
            
            //Preguntar ruta de salida
            Console.ResetColor();
            Console.WriteLine("Introduzca la ruta de el archivo a exportar (Ejemplo: C:\\Users\\...\\):");
            Console.ForegroundColor = ConsoleColor.Yellow;
            outputPath = Console.ReadLine();
            Console.WriteLine("\n");
            
            //Si no existe see inicia un bucle hasta que encuentre una directorio de salida
            while (!Directory.Exists(outputPath))
            {
                Console.ResetColor();
                Console.WriteLine("No se ha encontrado la ruta especificada, por favor vuelva a introducirla:");
                Console.ForegroundColor = ConsoleColor.Yellow;
                outputPath = Console.ReadLine();
                Console.WriteLine("\n");
            }

            //Cargar archivo
            try
            {
                file = File.ReadAllText(filePath);
            }
            catch (IOException e)
            {
                Console.WriteLine("IO Error: {0}", e.GetType().Name);
                return;
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Archivo cargado correctamente\n\n");
            Console.Title = "DAT_1 CONVERTER - " + fileName;
            Console.ResetColor();

            //Confirmar si el usuario quiere convertir el archivo
            Console.ResetColor();
            Console.WriteLine("Estas seguro que deseas continuar? Si/No");
            Console.ForegroundColor = ConsoleColor.Yellow;
            string userKey = Console.ReadLine();
            Console.ResetColor();
            
            if (userKey == "N" || userKey == "n" || userKey == "No" || userKey == "NO")
            {
                //En caso de lo contrario se cancela la conversion y se cierra el programa
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("la conversion ha sido cancelada");
                Console.ResetColor();
                return;
            }
            if (userKey == "S" || userKey == "s" || userKey == "Si" || userKey == "SI")
            {
                //Guardar en memoria la hora en la que empieza a convertir el archivo
                DateTime startTime = DateTime.Now;
                
                //Copiar en memoria el archivo y formatear su informacion
                file = strings.AddSeparators(file);                        //<- Añade separadores para no romper el espaciado del archivo
                file = strings.DeleteStartCharacter(file);                 //<- Elimina el caracter inicial
                fileElements = strings.SplitFileLines(file);               //<- Divide el archivo en distintas partes para trabajar con el

                //Guarda el nombre del archivo
                outputFileName = fileName.Split('.');
                string result = String.Empty;
                
                //Decide que metodo le interesa mas usar para aumentar el rendimiento y comienza la conversion
                if (file.Length > 1000000) ArrayToStringHighFileMode(fileElements);
                else
                {
                    result = ArrayToStringLowFileMode(fileElements);
                    //Elimina las separaciones creadas anteriormente para evitar problemas con el espaciado
                    result = strings.CleanSeparators(result);
                    //Crear csv
                    using (FileStream fileStream = File.Create(outputPath + outputFileName[0] + ".csv"))
                    {
                        Console.WriteLine("\ncsv creado correctamente\n");

                        byte[] info = new UTF8Encoding(true).GetBytes(result);

                        Console.WriteLine("Copiando informacion al csv...\n");

                        fileStream.Write(info, 0, info.Length);
                    }
                }

                TimeSpan time = DateTime.Now.Subtract(startTime);
                
                Console.Title = "DAT_1 CONVERTER";
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Conversion completa en " + time.Hours.ToString("00") + ":" + time.Minutes.ToString("00") + ":" + time.Seconds.ToString("00") + ", csv guardado en '" + outputPath + "'\n");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Pulse Enter para salir...");
                Console.ReadKey();
                Console.ResetColor();
            }

        }

        //Array to string

        public static string ArrayToStringLowFileMode(string[] file)
        {
            int count = 0;
            string result = "@mode edit, t\n@table pisnap\n@istr tag, time, value\n";
            string[] fileArray = new string[file.Length];
            for (int i = 0; i < file.Length - 1; i++)
            {
                //Porcentaje
                Console.WriteLine("Dando formato a la informacion: " + MathF.Abs(i * 100 / fileElements.Length) + "%");
                Console.Title = "DAT_1 CONVERTER - " + fileName + ": " + MathF.Abs(i * 100 / fileElements.Length) + "%";
                if (count == 0) fileArray[i] = file[i] + ",";
                if (count == 1) fileArray[i] = file[i] + " ";

                //Si la hora tiene un formato distinto lo arregla
                if (count == 2 && file[i].Length > 13)
                {
                    file[i] = file[i].Substring(0, 12) + "," + file[i].Substring(12, file[i].Length - 12);
                    fileArray[i] = file[i] + ",";
                    count = -1;
                }
                if (count == 2) fileArray[i] = file[i] + ",";

                if (count == 3)
                {
                    fileArray[i] = file[i] + "\n";
                    count = -1;
                }
                count++;
            }
            Console.WriteLine("Formato completado\n");
            //Elimina la separacion ascii que tiene el tag
            int e = 0;
            Parallel.For(1, fileArray.Length - 1, i =>
            {
                Console.WriteLine("Finalizando archivo: " + MathF.Abs((e * 100 / fileArray.Length)) + "%");

                char emptyChar = (char)0;
                string[] endLine = new string[2];
                if (fileArray[i].Contains(emptyChar)) endLine = fileArray[i].Split(emptyChar);

                //Si el elemento tag esta correcto se copia a file
                if (endLine[1] != null) fileArray[i] = endLine[1];
                i++;
                e++;
            });

            result = result + string.Join("", fileArray);
            return result;
        }
        public static void ArrayToStringHighFileMode(string[] file)
        {
            Strings strings = new Strings();
            string header = "@mode edit, t\n@table pisnap\n@istr tag, time, value\n";
            string[] result = new string[file.Length];
            for (int i = 0; i < file.Length - 1; i++)
            {
                //Porcentaje
                Console.WriteLine("Dando formato a la informacion: " + MathF.Abs(i * 100 / fileElements.Length) + "%");
                Console.Title = "DAT_1 CONVERTER - " + fileName + ": " + MathF.Abs(i * 100 / fileElements.Length) + "%";

                result[i] = ArrayToStringSingleOperation(file[i], i);
            }
            Console.WriteLine("Formato completado\n");

            //Quitar el porcentaje del titulo mostrado anteriormente
            Console.Title = "DAT_1 CONVERTER - " + fileName;

            //Elimina las separaciones creadas anteriormente para evitar problemas con el espaciado
            string str = strings.CleanSeparators(string.Join("", result));

            //Crear csv
            using (FileStream fileStream = File.Create(outputPath + outputFileName[0] + ".csv"))
            {
                Console.WriteLine("\ncsv creado correctamente\n");
                Console.WriteLine("Copiando informacion al csv...\n");

                byte[] info = new UTF8Encoding(true).GetBytes(header + str);

                fileStream.Write(info, 0, info.Length);
            }
            Console.WriteLine("Informacion copiada correctamente\n");
        }

        static int e = 0;
        public static string ArrayToStringSingleOperation(string file, int i)
        {
            Strings strings = new Strings();

            char emptyChar = (char)0;
            string result = String.Empty;

            if (fileCount == 0 && i != 0) {
                if(!file.Contains(emptyChar)){
                    Exception exp = new Exception("Error: tag expected in: " + file);
                    throw exp;
                }

                file = file.Remove(0, 5);
                result = file + ",";
            }else if(fileCount == 0){result = file + ",";}


            if (fileCount == 1) result = file + " ";

            //Si la hora tiene un formato distinto lo arregla
            if (fileCount == 2 && file.Length > 13)
            {
                file = file.Substring(0, 12) + "," + file.Substring(12, file.Length - 12);
                result = file + "\n";
                fileCount = -1;
            }
            if (fileCount == 2) result =  file + ",";

            //Si el final de linea no es correcto se corrige
            if(fileCount == 3 && (file.Contains(emptyChar) || file == "0" ))
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(file + " Found in line: " + i/3);
                Console.ResetColor();
                
                if(!file.Contains(emptyChar)){
                    Exception exp = new Exception("Error: tag expected in: " + file);
                    throw exp;
                }

                Console.WriteLine(file + ": Length = " + file.Length);
                file = file.Remove(0, 5);
                Console.WriteLine(file);
                
                result = "\n" + file + ",";
                fileCount = 0;
                e++;
            }
            //Si es final de linea pasar a otra nueva
            else if (fileCount == 3)
            {
                file = strings.ExponentialToDecimal(file);
                result = file + "\n";
                fileCount = -1;
            }

            //Si no se comprueba si es el ultimo de la lista y se elimina
            if (fileElements[fileElements.Length - 1] == file)
            {
                file = string.Empty;
            }

            fileCount++;
            return result;
        }
        
    }

    public class Strings
    {
        //String methods
        public string DeleteStartCharacter(string file)
        {
            //Comprueba si comienza con el caracter inicial
            char emptyChar = (char)0;
            if (file.StartsWith("0" + emptyChar))
            {
                file = file.Remove(0, 2);
                return file;
            }
            return file;
        }
        public string[] SplitFileLines(string file)
        {
            //Separa todos los elementos de file
            string[] fileElements = file.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            return fileElements;
        }
        public string ExponentialToDecimal(string file)
        {
            char emptyChar = (char)0;
            
            //Forzando a usar "." como separador decimal
            string CultureName = Thread.CurrentThread.CurrentCulture.Name;
            CultureInfo ci = new CultureInfo(CultureName);
            if (ci.NumberFormat.NumberDecimalSeparator != ".")
            {
                ci.NumberFormat.NumberDecimalSeparator = ".";
                Thread.CurrentThread.CurrentCulture = ci;
            }
            
            //Comprueba si la variable a convertir es un numero
            bool containsNumber = file.Any(char.IsDigit);
            if (containsNumber && (!file.Contains(emptyChar) && !file.Contains(':')))
            {
                decimal result;
                Decimal.TryParse(file, NumberStyles.AllowExponent | NumberStyles.Float, ci, out result);
                file = result.ToString();

            }

            return file;
        }
        public string AddSeparators(string file)
        {
            Console.WriteLine("\nEjecutando operaciones menores...");
            while (file.Contains("N "))
            {
                Console.WriteLine("Ejecutando operaciones menores...(1/3)");
                string changedPart = file.Substring(file.IndexOf("N ", 0), 3);
                string partToChange = changedPart;

                changedPart = changedPart.Replace(' ', '-');

                file = file.Replace(partToChange, changedPart);
            }
            while (file.Contains("NO "))
            {
                Console.WriteLine("Ejecutando operaciones menores...(2/3)");
                string changedPart = file.Substring(file.IndexOf("NO ", 0), 4);
                string partToChange = changedPart;

                changedPart = changedPart.Replace(' ', '-');

                file = file.Replace(partToChange, changedPart);
            }
            while (file.Contains("PRUEBA "))
            {
                Console.WriteLine("Ejecutando operaciones menores...(3/3)");
                string changedPart = file.Substring(file.IndexOf("PRUEBA ", 0), 8);
                string partToChange = changedPart;

                changedPart = changedPart.Replace(' ', '-');

                file = file.Replace(partToChange, changedPart);
            }

            Console.WriteLine("\ncompletadas\n");
            return file;
        }
        public string CleanSeparators(string file)
        {
            Console.WriteLine("\nLimpiando el archivo...");
            while (file.Contains("N-"))
            {
                Console.WriteLine("Limpiando el archivo...(1/3)");
                string changedPart = file.Substring(file.IndexOf("N-", 0), 3);
                string partToChange = changedPart;

                changedPart = changedPart.Replace('-', ' ');

                file = file.Replace(partToChange, changedPart);
            }
            while (file.Contains("NO-"))
            {
                Console.WriteLine("Limpiando el archivo...(2/3)");
                string changedPart = file.Substring(file.IndexOf("NO-", 0), 4);
                string partToChange = changedPart;

                changedPart = changedPart.Replace('-', ' ');

                file = file.Replace(partToChange, changedPart);
            }
            while (file.Contains("PRUEBA-"))
            {
                Console.WriteLine("Limpiando el archivo...(3/3)");
                string changedPart = file.Substring(file.IndexOf("PRUEBA-", 0), 8);
                string partToChange = changedPart;

                changedPart = changedPart.Replace('-', ' ');

                file = file.Replace(partToChange, changedPart);
            }

            Console.WriteLine("Limpiado\n");

            return file;
        }

    }

}