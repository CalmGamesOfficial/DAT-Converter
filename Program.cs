//DAT Converter (Version 1.3) by Calm Games for more information https://github.com/CalmGamesOfficial/DAT-Converter
using System.Runtime.InteropServices;
using System.Collections.Generic;
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
        //SO Info (Permite desarrollar este proyecto en otra plataforma sin apenas cambiar nada)
        public static bool isWindows = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        //Variables estaticas
        public static string file;
        public static string[] fileElements;
        public static string[] outputFileName;

        public static int fileCount;
        public static string fileName;
        public static string filePath;
        public static string outputPath;

        public static int fileLine = 4;
        public static string[] ignoreTags;
        public static List<int> tagsToIgnore = new List<int>();

        static void Main(string[] args)
        {
            //Obtener metodos de la clase strings
            Strings strings = new Strings();

            //Titulo
            Console.Clear();
            strings.Title();
            
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
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No se ha encontrado el archivo especificado, por favor vuelva a introducirla:");
                Console.ForegroundColor = ConsoleColor.Yellow;
                
                filePath = Console.ReadLine();
                Console.WriteLine("\n");
                Console.ResetColor();
            }
            
            //Una vez encontrado se guarda su nombre para su posterior uso
            string[] fileDir;
            if (isWindows)
            {
                fileDir = filePath.Split('\\');
                fileName = fileDir[fileDir.Length - 1];
                fileName = fileName.Split('.')[0];
            }
            else{
                fileDir = filePath.Split('/');
                fileName = fileDir[fileDir.Length - 1];
                fileName = fileName.Split('.')[0];
            }
            
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
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No se ha encontrado la ruta especificada, por favor vuelva a introducirla:");
                Console.ForegroundColor = ConsoleColor.Yellow;
                
                outputPath = Console.ReadLine();
                Console.WriteLine("\n");
                Console.ResetColor();
            }

            //Cargar archivo
            try
            {
                file = File.ReadAllText(filePath);
            }
            catch (IOException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("IO Error: {0}", e.GetType().Name);
                Console.ResetColor();
                return;
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Archivo cargado correctamente\n\n");
            Console.Title = "DAT_1 CONVERTER - " + fileName;
            Console.ResetColor();

            //Cargar ignoreTags
            try
            {
                ignoreTags = File.ReadAllLines("IgnoreTags.csv");
            }
            catch (IOException)
            {
                Console.ForegroundColor = ConsoleColor.Red; 
                Console.WriteLine("IO Error: 'IgnoreTags.csv' Se ha movido del directorio o modificado incorrectamente");
                Console.ResetColor();
                return;
            }

            //Confirmar si el usuario quiere convertir el archivo
            Console.ResetColor();
            Console.Write("Estas seguro que deseas continuar? ");
            
            //Mostrar colores en Si/No
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Si");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("/");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("No\n");

            Console.ForegroundColor = ConsoleColor.Yellow;
            string userKey = Console.ReadLine();
            Console.ResetColor();
            
            if (userKey == "N" || userKey == "n" || userKey == "No" || userKey == "NO")
            {
                //En caso de lo contrario se cancela la conversion y se cierra el programa
                Console.Clear();
                strings.Title();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("la conversion ha sido cancelada");
                Console.ResetColor();
                return;
            }
            if (userKey == "S" || userKey == "s" || userKey == "Si" || userKey == "SI")
            {
                strings.ClearConsole();

                //Guardar en memoria la hora en la que empieza a convertir el archivo
                DateTime startTime = DateTime.Now;
                
                //Copiar en memoria el archivo y formatear su informacion
                file = strings.AddSeparators(file);                        //<- Añade separadores para no romper el espaciado del archivo
                file = strings.DeleteStartCharacter(file);                 //<- Elimina el caracter inicial
                fileElements = strings.SplitFileLines(file);               //<- Divide el archivo en distintas partes para trabajar con el

                //Guarda el nombre del archivo
                outputFileName = fileName.Split('.');
                string result = String.Empty;

                ArrayToString(fileElements);

                TimeSpan time = DateTime.Now.Subtract(startTime);
                
                strings.ClearConsole();

                Console.Title = "DAT_1 CONVERTER";
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nConversion completa en " + time.Hours.ToString("00") + ":" + time.Minutes.ToString("00") + ":" + time.Seconds.ToString("00") + ", csv guardado en '" + outputPath + "'\n");

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("DAT Converter (Version 1.3) by Calm Games for more information: ");
                Console.ForegroundColor =ConsoleColor.Cyan;
                Console.Write("https://github.com/CalmGamesOfficial\n\n");
                
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Pulse Enter para salir...");
                Console.ReadKey();
                Console.ResetColor();
            }

        }

        //Array to string (Convierte la array de elementos a una string que se acaba copiando al documento)
        public static void ArrayToString(string[] file)
        {
            //Obtener metodos de la clase strings
            Strings strings = new Strings();

            List<string> result = new List<string>();
            string header = "@mode edit, t\n@table pisnap\n@istr tag, time, value\n";

            for (int i = 0; i < file.Length - 1; i++)
            {
                //Porcentaje
                if(i > 0) strings.ClearCurrentConsoleLine();
                Console.WriteLine("Dando formato a la informacion: " + MathF.Abs(i * 100 / fileElements.Length) + "%");
                Console.Title = "DAT_1 CONVERTER - " + fileName + " " + MathF.Abs(i * 100 / fileElements.Length) + "%";
                
                //Añadir los elementos a la lista
                result.Add(ArrayToStringSingleOperation(file[i], i));
            }
            Console.WriteLine("Formato completado");

            //Quitar el porcentaje del titulo mostrado anteriormente
            Console.Title = "DAT_1 CONVERTER - " + fileName;

            //Elimina las separaciones creadas anteriormente para evitar problemas con el espaciado
            string str = strings.CleanSeparators(string.Join("", result));

            //Crear archivo de cache de datos
            using (FileStream fileStream = File.Create("Data.cache"))
            {
                Console.WriteLine("Copiando informacion al archivo de cache...\n");

                byte[] info = new UTF8Encoding(true).GetBytes(header + str);

                fileStream.Write(info, 0, info.Length);
            }

            //Copiar datos de el cache al csv
            string line = string.Empty;
            int cacheFileLine = 0;
            int lineMultiplier = 0;
            bool ignoredLine = false;
            FileStream streamCache = File.OpenRead("Data.cache");
            FileStream streamFile = File.Create(outputPath + outputFileName[0] + ".csv");
            using(StreamReader reader = new StreamReader(streamCache))
            using(StreamWriter writer = new StreamWriter(streamFile, Encoding.UTF8))
            {
                //mientras el reader siga leyendo lineas se siguen escribiendo en el csv
                while((line = reader.ReadLine()) != null)
                {
                    cacheFileLine++;
                    
                    //Se comprueba si la linea que se esta escribiendo tiene que ser ignorada
                    for (int i = 0; i < tagsToIgnore.Count; i++)
                    {if(cacheFileLine == (tagsToIgnore[i])) ignoredLine = true;}
                    
                    //Se escriben las lineas en el archivo
                    if(!ignoredLine) {writer.WriteLine(line); } 
                    else {ignoredLine = false; lineMultiplier++;}    
                }
                //Cerrar los archivos    
                writer.Close();
                reader.Close();
                streamFile.Close();
                streamCache.Close();
            }
            strings.ClearCurrentConsoleLine();
            Console.WriteLine("\nInformacion copiada correctamente\n");

            //Por ultimo borramos el archivo de cache
            Console.WriteLine("Borrando el cache...\n");
            File.Delete("Data.cache");
            strings.ClearCurrentConsoleLine();
            Console.WriteLine("Borrado\n");
        }
        public static string ArrayToStringSingleOperation(string file, int i)
        {
            //Obtener metodos de la clase strings
            Strings strings = new Strings();

            char emptyChar = (char)0;
            string result = String.Empty;

            //TAG
            if (fileCount == 0 && i != 0) {
                //Si no es un tag devuelve una excepcion
                if(!file.Contains(emptyChar)){
                    Exception exp = new Exception("Error: tag expected in: " + file + "(Index: " + i + ")");
                    throw exp;
                }

                //Si esta en ignoredTags se guarda en una array para mas tarde remover las lineas
                if(isIgnoredTag(file)) tagsToIgnore.Add(fileLine);

                //Borrar los caracteres ascii del tag
                file = file.Remove(0, 5);
                
                result = file + ",";

            }else if(fileCount == 0){ result = file + ","; }

            //FECHA
            if (fileCount == 1) result = file + " ";

            //HORA
            if (fileCount == 2 && file.Length > 13)
            {
                //Si la hora tiene un formato distinto lo arregla
                file = file.Substring(0, 12) + "," + file.Substring(12, file.Length - 12);
                result = file + "\n";
                fileCount = -1;
            }
            if (fileCount == 2) result =  file + ",";

            //VALUE
            if(fileCount == 3 && (file.Contains(emptyChar) || file == "0" ))
            {
                //Si el final de linea no es correcto se corrige
                file = file.Remove(0, 5);
                
                result = "\n" + file + ",";
                fileCount = 0;
                fileLine++;
            }
            else if (fileCount == 3)
            {
                //Si es final de linea pasar a otra nueva

                //Covertir de exponencial a decimal el value
                file = strings.ExponentialToDecimal(file);
                result = file + "\n";
                fileCount = -1;
                fileLine++;
            }

            //Si no se comprueba si es el ultimo de la lista y se elimina
            if (fileElements[fileElements.Length - 1] == file)
            {
                file = string.Empty;
            }

            fileCount++;
            return result;
        }
        
        //Comprueba que no se cuelen tags antiguos en el pyconfig
        public static bool isIgnoredTag (string tag) {
            //remove nml
            tag = tag.Remove(0, 5); 
            //Check if the tag is on the ignored list
            for (int i = 1; i < ignoreTags.Length; i++)
            { if(tag == ignoreTags[i]) return true;}
            
            return false;
        }
    }

    public class Strings
    {   
        //Console methods
        public void ClearConsole() {Console.Clear(); Title();}
        public void Title() {
            Console.Title = "DAT_1 CONVERTER";
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine(" DAT_1 CONVERTER ".PadRight(Console.BufferWidth));
            Console.ResetColor();
        }
        public void ClearCurrentConsoleLine()
        {
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.Write(new string(' ', Console.BufferWidth));
            Console.SetCursorPosition(0, Console.CursorTop - 1);
        }
        
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
            
            ClearCurrentConsoleLine();
            Console.WriteLine("Ejecutando operaciones menores...(1/3)");
            while (file.Contains("N "))
            {
                string changedPart = file.Substring(file.IndexOf("N ", 0), 3);
                string partToChange = changedPart;

                changedPart = changedPart.Replace(' ', '-');

                file = file.Replace(partToChange, changedPart);
            }
            
            ClearCurrentConsoleLine();
            Console.WriteLine("Ejecutando operaciones menores...(2/3)");
            while (file.Contains("NO "))
            {
                string changedPart = file.Substring(file.IndexOf("NO ", 0), 4);
                string partToChange = changedPart;

                changedPart = changedPart.Replace(' ', '-');

                file = file.Replace(partToChange, changedPart);
            }
            
            ClearCurrentConsoleLine();
            Console.WriteLine("Ejecutando operaciones menores...(3/3)");
            while (file.Contains("PRUEBA "))
            {
                string changedPart = file.Substring(file.IndexOf("PRUEBA ", 0), 8);
                string partToChange = changedPart;

                changedPart = changedPart.Replace(' ', '-');

                file = file.Replace(partToChange, changedPart);
            }

            Console.WriteLine("completadas\n");
            return file;
        }
        public string CleanSeparators(string file)
        {
            Console.WriteLine("\nLimpiando el archivo...(1/3)");
            while (file.Contains("N-"))
            {
                string changedPart = file.Substring(file.IndexOf("N-", 0), 3);
                string partToChange = changedPart;

                changedPart = changedPart.Replace('-', ' ');

                file = file.Replace(partToChange, changedPart);
            }
            ClearCurrentConsoleLine();
            Console.WriteLine("Limpiando el archivo...(2/3)");
            while (file.Contains("NO-"))
            {
                string changedPart = file.Substring(file.IndexOf("NO-", 0), 4);
                string partToChange = changedPart;

                changedPart = changedPart.Replace('-', ' ');

                file = file.Replace(partToChange, changedPart);
            }
            ClearCurrentConsoleLine();
            Console.WriteLine("Limpiando el archivo...(3/3)");
            while (file.Contains("PRUEBA-"))
            {
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