using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SQLite;

namespace TestTask
{
    // подключение к БД
    public static class Connection
    {
        public const string connectionString = "Data Source=TestTask.db;Version=3;";
    }
    // класс, описывающий коробки
    public class Box
    {
        public int id;
        public double width;
        public double height;
        public double depth;
        public double weight;
        public double volume;
        public string productionDate;
        public string expirationDate;

        public Box()
        {
        }

        public Box(int _id, double _width, double _height, double _depth, double _weight, double _volume, string _productionDate, string _expirationDate)
        {
            id = _id;
            width = _width;
            height = _height;
            depth = _depth;
            weight = _weight;
            volume = _volume;
            productionDate = _productionDate;
            expirationDate = _expirationDate;
        }

        public int Id 
        { 
            get { return id; } 
            set { id = value; } 
        }
        public double Width 
        { 
            get { return width; } 
            set { width = value; } 
        }
        public double Height 
        { 
            get { return height; } 
            set { height = value; } 
        }
        public double Depth 
        { 
            get { return depth; } 
            set { depth = value; } 
        }
        public double Weight 
        { 
            get { return weight; } 
            set { weight = value; } 
        }
        public double Volume
        {
            get { return volume; }
            set { volume = width*height*depth; }
        }
        public string ProductionDate
        { 
            get { return productionDate; } 
            set { productionDate = value; } 
        }
        public string ExpirationDate
        {
            get { return expirationDate; }
            set 
            {
                if (productionDate == "" || value != "")
                    expirationDate = value;
                //expirationDateBox = (Convert.ToDateTime(productionDateBox).AddDays(100)).ToString();
                else 
                    expirationDate = DateTime.Parse(productionDate).AddDays(100).ToShortDateString();
            }
        }

    }

    // класс, описывающий палетты
    public class Palette 
    {
        public int idPalette;
        public double widthPalette;
        public double heightPalette;
        public double depthPalette;
        public double weightPalette;
        public double volumePalette;
        public string expirationDatePalette;
        public List<Box> boxes;

        public Palette()
        {
        }

        public Palette(int _id, double _width, double _height, double _depth, double _weight, double _volume, string _expirationDate, List<Box> _boxes)
        {
            idPalette = _id;
            widthPalette = _width;
            heightPalette = _height;
            depthPalette = _depth;
            weightPalette = _weight;
            volumePalette = _volume;
            expirationDatePalette = _expirationDate;
            boxes = _boxes;
        }

        public int Id
        {
            get { return idPalette; }
            set { idPalette = value; }
        }
        public double Width
        {
            get { return widthPalette; }
            set { widthPalette = value; }
        }
        public double Height
        {
            get { return heightPalette; }
            set { heightPalette = value; }
        }
        public double Depth
        {
            get { return depthPalette; }
            set { depthPalette = value; }
        }
        public double Weight
        {
            get { return weightPalette; }
            set {
                if (boxes != null)
                {
                    foreach (var item in boxes)
                    {
                        weightPalette += item.weight;
                    }
                    weightPalette += 30;
                }
                else
                    weightPalette = 30.0; 
            }
        }
        public double Volume
        {
            get { return volumePalette; }
            set
            {
                if (boxes != null)
                {
                    foreach (var item in boxes)
                    {
                        volumePalette += item.volume;
                    }
                    volumePalette += widthPalette * heightPalette * depthPalette;
                }
                else
                    volumePalette = widthPalette * heightPalette *depthPalette; }
        }
        public string ExpirationDate
        {
            get { return expirationDatePalette; }
            set
            {
                if (boxes != null && boxes.Count > 0)
                {
                    DateTime min = DateTime.MaxValue;
                    foreach (var item in boxes)
                    {
                        if((DateTime.Parse(item.expirationDate)) <  min)
                        min = DateTime.Parse(item.expirationDate);
                    }
                    expirationDatePalette = min.ToShortDateString();
                }
                else
                    expirationDatePalette = DateTime.MinValue.ToShortDateString();
            }
        }
        public List<Box> Boxes
        {
            get { return boxes; }
            set { boxes = value; }
        }
    }

    // меню программы
    class Menu
    {
        private enum Options
        {
            input_rnd = 1,
            input_Keyboard,
            input_From_Database,
            go_out
        }
        static void Main(string[] args)
        {
            for (; ; )
            {
                Console.WriteLine("Выберите пункт меню:\n" +
                    "1. Сгенерировать данные\n" +
                    "2. Ввести данные с клавиатуры\n" +
                    "3. Ввести данные из базы данных\n" +
                    "4. Выйти из программы\n");
                string menu_str = Console.ReadLine();
                if (Checks.Menu(menu_str))
                {
                    var menu = int.Parse(menu_str);
                    List<Box> boxes = new List<Box>();
                    List<Palette> palettes = new List<Palette>();
                    switch (menu)
                    {
                        // ввод данных генерацией
                        case (int)Options.input_rnd:
                            boxes = FillData.FillDataRandom.Box_Data_Random();
                            palettes = FillData.FillDataRandom.Palette_Data_Random(boxes);
                            Output.FirstTask(palettes);
                            Output.SecondTask(palettes);
                            Console.WriteLine("Для продолжения нажмите любую клавишу... ");
                            Console.ReadKey();
                            Console.Clear();
                            break;
                        // ввод с клавиатуры
                        case (int)Options.input_Keyboard:
                            boxes = FillData.FillDataKeyboard.Box_Data_keyboard();
                            palettes = FillData.FillDataKeyboard.Palette_Data_keyboard(boxes);
                            Output.FirstTask(palettes);
                            Output.SecondTask(palettes);
                            Console.WriteLine("Для продолжения нажмите любую клавишу... ");
                            Console.ReadKey();
                            Console.Clear();
                            break;
                        // ввод из БД
                        case (int)Options.input_From_Database:
                            boxes = FillData.FillDataDatabase.Box_Data_Database();
                            palettes = FillData.FillDataDatabase.Palette_Data_Database();
                            Output.FirstTask(palettes);
                            Output.SecondTask(palettes);
                            Console.WriteLine("Для продолжения нажмите любую клавишу... ");
                            Console.ReadKey();
                            Console.Clear();
                            break;
                        // выход
                        case (int)Options.go_out:
                            return;
                    }
                }
            }
        }
    }
}
