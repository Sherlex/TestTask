using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;


namespace TestTask
{
    public class FillData
    {
        // заполнение с клавиатуры
        public class FillDataKeyboard
        {
            public static List<Box> Box_Data_keyboard()
            {
                Console.WriteLine("\nВведите количество коробок: ");
                int count = Checks.IsIdOk();
                List<Box> boxes = new List<Box>(count);

                for (int i = 0; i < count; i++)
                {
                    Box item = new Box();
                    Console.WriteLine("\nВведите ID коробки: ");
                    item.Id = Checks.IsIdOk();
                    Console.WriteLine("\nВведите ширину коробки, см: ");
                    item.Width = Checks.IsParameterOk();
                    Console.WriteLine("\nВведите высоту коробки, см: ");
                    item.Height = Checks.IsParameterOk();
                    Console.WriteLine("\nВведите глубину коробки, см: ");
                    item.Depth = Checks.IsParameterOk();
                    Console.WriteLine("\nВведите вес коробки, кг: ");
                    item.Weight = Checks.IsParameterOk();
                    Console.WriteLine("\nВведите дату производства коробки, дд.мм.гггг:" +
                        "\nЕсли дата производства отсутствует, оставьте поле пустым. ");
                    item.ProductionDate = Checks.IsDateOk();
                    Console.WriteLine("\nВведите срок годности коробки, дд.мм.гггг: " +
                        "\nЕсли срок годности отсутствует, но указана дата производcтва, оставьте поле пустым. ");
                    item.ExpirationDate = Checks.IsDateOk(item.ProductionDate);
                    double volume = 0;
                    item.Volume = volume;
                    boxes.Add(item);
                }
                return boxes;
            }

            public static List<Palette> Palette_Data_keyboard(List<Box> boxes)
            {
                Console.WriteLine("\nВведите количество палетт: ");
                int count = Checks.IsIdOk();
                List<Palette> palettes = new List<Palette>(count);
                var find = boxes.Select(x => x.Id).ToArray();
                for (int i = 0; i < count; i++)
                {
                    Console.WriteLine("\nВведите ID паллеты: ");
                    int id = Checks.IsIdOk(); ;
                    Console.WriteLine("\nВведите ширину паллеты, см: ");
                    double width = Checks.IsParameterOk();
                    Console.WriteLine("\nВведите высоту паллеты, см: ");
                    double height = Checks.IsParameterOk();
                    Console.WriteLine("\nВведите глубину паллеты, см: ");
                    double depth = Checks.IsParameterOk();
                    Console.WriteLine("\nВведите ID коробок, находящихся на паллете: ");
                    // выполняется пока не будут получены корректные id коробок, находящихся на палетте
                    while (true)
                    {
                        string[] indexesString = Console.ReadLine().Split(' ').ToArray();
                        if(Checks.IsIndexesOk(find, indexesString))
                        {
                            int[] indexes = Array.ConvertAll(indexesString, x => int.Parse(x));
                            List<Box> boxIn = new List<Box>();
                            double volume = 0;
                            double weight = 0;
                            string min = "";
                            for (int j = 0; j < indexes.Length; j++)
                            {
                                foreach (var item in boxes)
                                {
                                    if (indexes[j] == item.Id)
                                    {
                                        // проверка на соотвествие размеров коробки размерам паллеты
                                        if (width >= item.Width && depth >= item.Depth)
                                        {
                                            Box box = new Box
                                            {
                                                Id = item.Id,
                                                Width = item.Width,
                                                Height = item.Height,
                                                Depth = item.Depth,
                                                Weight = item.Weight,
                                                Volume = item.Volume,
                                                ProductionDate = item.ProductionDate,
                                                ExpirationDate = item.ExpirationDate
                                            };
                                            boxIn.Add(box);
                                        }
                                        else
                                            Console.WriteLine("\nШирина и/или глубина коробки № " + item.Id + "превышает ширину и/или глубину паллеты " + id + "!");
                                    }
                                }
                            }
                            if (boxIn.Count >= 1)
                            {
                                min = boxIn.Min(x => (DateTime.Parse(x.ExpirationDate))).ToShortDateString();
                            }
                            Palette palette = new Palette
                            {
                                Id = id,
                                Width = width,
                                Height = height,
                                Depth = depth,
                                Boxes = boxIn,
                                Weight = weight,
                                Volume = volume,
                                ExpirationDate = min
                            };
                            palettes.Add(palette);
                        }
                        break;
                    }
                }
                return palettes;
            }
        }

        // заполнение из БД
        public class FillDataDatabase
        {
            public static List<Box> Box_Data_Database()
            {
                // подключение к БД
                SQLiteConnection sqlconnection;
                SQLiteCommand command;
                sqlconnection = new SQLiteConnection(Connection.connectionString);
                sqlconnection.Open();
                command = new SQLiteCommand
                {
                    Connection = sqlconnection
                };
                // получение данных из таблицы, содержащей информацию о коробках
                command = new SQLiteCommand("SELECT * FROM [Box]", sqlconnection);
                SQLiteDataAdapter boxDataAdapter = new SQLiteDataAdapter(command);
                SQLiteDataReader reader = command.ExecuteReader();
                List<Box> boxes = new List<Box>();
                while (reader.Read())
                {
                    Box item = new Box();
                    item.Id = Convert.ToInt32(reader[0]);
                    item.Width = Convert.ToDouble(reader[1]);
                    item.Height = Convert.ToDouble(reader[2]);
                    item.Depth = Convert.ToDouble(reader[3]);
                    item.Weight = Convert.ToDouble(reader[4]);
                    item.ProductionDate = reader[5].ToString();
                    item.ExpirationDate = reader[6].ToString();
                    double volume = 0;
                    item.Volume = volume;
                    boxes.Add(item);
                }
                return boxes;
            }

            public static List<Palette> Palette_Data_Database()
            {
                SQLiteConnection sqlconnection;
                SQLiteCommand command;
                sqlconnection = new SQLiteConnection(Connection.connectionString);
                sqlconnection.Open();
                command = new SQLiteCommand
                {
                    Connection = sqlconnection
                };

                // получение данных из таблицы, содержащей информацию о палеттах
                command = new SQLiteCommand("SELECT * FROM [Palette]", sqlconnection);
                SQLiteDataAdapter paletteDataAdapter = new SQLiteDataAdapter(command);
                SQLiteDataReader reader = command.ExecuteReader();
                List<Palette> palettes = new List<Palette>();
                while (reader.Read())
                {
                    double volume = 0;
                    double weight = 0;
                    string min = "";
                    Palette item = new Palette();
                    item.Id = Convert.ToInt32(reader[0]);
                    item.Width = Convert.ToDouble(reader[1]);
                    item.Height = Convert.ToDouble(reader[2]);
                    item.Depth = Convert.ToDouble(reader[3]);
                    command = new SQLiteCommand("SELECT * FROM [Box] WHERE IdPalette = @par;", sqlconnection);
                    command.Parameters.AddWithValue("par", item.Id);
                    SQLiteDataAdapter boxDataAdapter = new SQLiteDataAdapter(command);
                    SQLiteDataReader boxReader = command.ExecuteReader();
                    List<Box> boxIn = new List<Box>();
                    while (boxReader.Read())
                    {
                        // проверка на соотвествие размеров коробки размерам паллеты
                        if (item.Width >= (Convert.ToDouble(boxReader[1])) && item.Depth >= (Convert.ToDouble(boxReader[3])))
                        {
                            Box box = new Box()
                            {
                                Id = Convert.ToInt32(boxReader[0]),
                                Width = Convert.ToDouble(boxReader[1]),
                                Height = Convert.ToDouble(boxReader[2]),
                                Depth = Convert.ToDouble(boxReader[3]),
                                Weight = Convert.ToDouble(boxReader[4]),
                                Volume = volume,
                                ProductionDate = boxReader[5].ToString(),
                                ExpirationDate = boxReader[6].ToString()
                            };
                            boxIn.Add(box);
                        }
                        else
                            Console.WriteLine("\nШирина и/или глубина коробки превышает ширину и/или глубину паллеты!");
                    }
                    // если на паллете есть коробки
                    if (boxIn.Count != 0)
                    {
                        min = boxIn.Min(x => (DateTime.Parse(x.ExpirationDate))).ToShortDateString();
                    }
                    item.Boxes = boxIn;
                    item.Volume = volume;
                    item.Weight = weight;
                    item.ExpirationDate = min;
                    palettes.Add(item);
                }
                return palettes;
            }
        }

        // заполнение генерацией
        public class FillDataRandom
        {
            public static List<Box> Box_Data_Random()
            {
                Console.WriteLine("\nВведите количество коробок: ");
                int count = Checks.IsIdOk();
                List<Box> boxes = new List<Box>();
                // генерация случайных значений для коробок
                Random rnd = new Random();
                for (int i = 0; i < count; i++)
                {
                    Box item = new Box();
                    item.Id = rnd.Next(0, 100);
                    item.Width = rnd.Next(0, 100) + rnd.NextDouble();
                    item.Height = rnd.Next(0, 100) + rnd.NextDouble();
                    item.Depth = rnd.Next(0, 100) + rnd.NextDouble();
                    item.Weight = rnd.Next(0, 100) + rnd.NextDouble();
                    item.ProductionDate = RandomDate();
                    item.ExpirationDate = RandomDate();
                    double volume = 0;
                    item.Volume = volume;
                    boxes.Add(item);
                }
                return boxes;
            }

            public static List<Palette> Palette_Data_Random(List<Box> boxes)
            {
                List<Palette> palettes = new List<Palette>();
                // генерация случайных значений для паллет
                Random rnd = new Random();
                Console.WriteLine("\nВведите количество палетт: ");
                int count = Checks.IsIdOk();
                for (int i = 0; i < count; i++)
                {
                    double volume = 0;
                    double weight = 0;
                    string min = "";
                    Palette palette = new Palette();
                    palette.Id = rnd.Next(0, 100);
                    palette.Width = rnd.Next(0, 100) + rnd.NextDouble();
                    palette.Height = rnd.Next(0, 100) + rnd.NextDouble();
                    palette.Depth = rnd.Next(0, 100) + rnd.NextDouble();
                    List<Box> boxIn = new List<Box>();
                    int[] indexes = boxes.Select(x => x.Id).ToArray();
                    int[] boxIndexes = RandomIndexes(indexes);
                    for (int j = 0; j < boxIndexes.Length; j++)
                    {
                        foreach (var item in boxes)
                        {
                            if (boxIndexes[j] == item.Id)
                            {
                                if (palette.Width >= item.Width && palette.Depth >= item.Depth)
                                {
                                    Box box = new Box
                                    {
                                        Id = item.Id,
                                        Width = item.Width,
                                        Height = item.Height,
                                        Depth = item.Depth,
                                        Weight = item.Weight,
                                        Volume = item.Volume,
                                        ProductionDate = item.ProductionDate,
                                        ExpirationDate = item.ExpirationDate
                                    };
                                    boxIn.Add(box);
                                }
                                else
                                    Console.WriteLine("\nШирина и/или глубина коробки № " + item.Id + " превышает ширину и/или глубину паллеты " + palette.Id + "!");
                            }
                        }
                    }
                    if (boxIn.Count >= 1)
                    {
                        min = boxIn.Min(x => (DateTime.Parse(x.ExpirationDate))).ToShortDateString();
                    }
                    palette.Boxes = boxIn;
                    palette.Weight = weight;
                    palette.Volume = volume;
                    palette.ExpirationDate = min;
                    palettes.Add(palette);
                }

                return palettes;
            }

            // генерация массива коробок, находящихся на паллете
            public static int[] RandomIndexes(int[] id)
            {
                Random random = new Random();
                var quantity = random.Next(1, 6);
                var min = id.Min();
                var max = id.Max();
                int[] result = new int[quantity];
                for(int i = 0; i < quantity; i++)
                {
                    result[i] = random.Next(min, max);
                }
                return result;
            }
        }

        // генерация случайной даты формата дд.мм.гггг
        public static string RandomDate()
        {
            var startDate = DateTime.Now.AddYears(-15);
            var endDate = DateTime.Now.AddYears(15);
            Random random = new Random();
            var randomYear = random.Next(startDate.Year, endDate.Year);
            var randomMonth = random.Next(1, 12);
            var randomDay = random.Next(1, DateTime.DaysInMonth(randomYear, randomMonth));
            DateTime randomDate = new DateTime(randomYear, randomMonth, randomDay);
            return randomDate.ToShortDateString();
        }
    }

    public class Output
    {
        // вывод на экран результатов выполнения первого пункта задания
        public static List<IGrouping<string, Palette>> FirstTask(List<Palette> palettes)
        {
            // сортировка и последующая группировка по сроку годности
            var groups = palettes.OrderBy(palette => DateTime.Parse(palette.ExpirationDate))
                             .ThenBy(palette => palette.Weight)
                             .GroupBy(palette => palette.ExpirationDate).ToList();
            Console.WriteLine("\nРезультат - Паллеты сгруппированы по сроку годности, полученные группы отсортированы по возрастанию срока годности,"+
                "в каждой группе палетты отсортированы по весу: \n");
            foreach (var group in groups)
            {
                Console.WriteLine("Срок годности: "+ group.Key);
                foreach (var item in group)
                {
                    Console.WriteLine("Id: " + item.Id + "\nШирина: " + item.Width + "\nВысота: " + item.Height + "\nГлубина: " + item.Depth 
                        + "\nВес: "+ item.Weight + "\nОбъем: " + item.Volume + "\n");
                }
            }
            return groups;
        }

        // вывод на экран результатов выполнения второго пункта задания
        public static void SecondTask(List<Palette> palettes)
        {
            // выбор 3-х паллет с наиольшия сроком годности коробок
            var top3Palette = palettes.OrderByDescending(palette => DateTime.Parse(palette.ExpirationDate))
           .Take(3);
            
            var sortBoxes = top3Palette.Select(palette => palette.Boxes);
            Console.WriteLine("\nРезультат - 3 паллеты, которые содержат коробки с наибольшим сроком годности," 
                + "отсортированные по возрастанию объема: ");
            int i = 0;
            foreach (var palette in top3Palette)
            {
                int number = 1;
                Console.WriteLine("\nId палетты: " + palette.Id + "\nСрок годности: " + palette.ExpirationDate);
                Console.WriteLine("Объем коробок: ");
                var item = sortBoxes.ElementAt(i);
                // сортировка коробок по возрастанию объема
                var sort = item.OrderBy(x => x.Volume);
                foreach (var element in sort)
                {
                    Console.WriteLine(number +". " + "Объем: " + element.Volume + " Id коробки: " + element.Id);
                    number++;
                }
                i++;
            }
        }                
    }

    public class Checks
    {
        public enum OptionsSubMenu
        {
            Yes = 1,
            No
        }

        // проверка выбранного пункта меню на корректность
        public static bool Menu(string menu)
        {
            if (menu != "1" && menu != "2" && menu != "3" && menu != "4")
            {
                Console.Clear();
                Console.WriteLine("\nНекорректный пункт меню! Попробуйте снова\n");
                return false;
            }
            else
            {
                return true;
            }
        }

        // проверка целого числа на корректность
        public static int IsIdOk()
        {
            int id;
            while(!int.TryParse(Console.ReadLine(), out id))
            {
                Console.WriteLine("Ошибка! Введите целое число: ");
            }
            return id;
        }

        // проверка десятичного числа на корректность
        public static double IsParameterOk()
        {
            double element;
            while (!double.TryParse(Console.ReadLine(), out element))
            {
                Console.WriteLine("Ошибка! Введите десятичное число: ");
            }
            return element;
        }

        // проверка даты на корректность 
        public static string IsDateOk()
        {
            DateTime element;
            
            while (true)
            {
                string check = Console.ReadLine();
                bool result = DateTime.TryParse(check, out element);
                if (!result && check != "")
                {
                    Console.WriteLine("Ошибка! Введите дату в указанном формате: ");
                }
                else if (check == "")
                {
                    return check;
                }
                else
                    return element.ToShortDateString();
            }
        }

        public static string IsDateOk(string date)
        {
            DateTime element;
            while (true)
            {
                string check = Console.ReadLine();
                bool result = DateTime.TryParse(check, out element);
                if (!result && check != "")
                {
                    Console.WriteLine("Ошибка! Введите дату в указанном формате: ");
                }
                else if (check == "" && date == "")
                {
                    Console.WriteLine("Ошибка! Дата не может быть пустой.: ");
                }
                else if (check == "")
                    return check;
                else
                    return element.ToShortDateString();
            }
        }

        // проверка выбранных id на корректность (существуют ли корjбки с даннымb id)
        public static bool IsIndexesOk(int[] id, string[] indexes)
        {
            int[] element = new int[indexes.Count()];
            int i = 0;
            while (true)
            {
                foreach(var item in indexes)
                {
                    if (!(int.TryParse(item, out element[i])) && (id.ToString()).Contains(item))
                    {
                        Console.WriteLine("Ошибка! Коробки с данными Id не найдены. ");
                        return false;
                    }
                    return true;
                }
               
            }
        }  
    }
}
