using System;
using System.Collections.Generic;

public class Program
{
    // Use a dictionary to store multiple coordinates for each type of obstacle.
    static Dictionary<string, List<Tuple<int, int>>> obstacles = new Dictionary<string, List<Tuple<int, int>>>();

    public static void Main()
    {
        bool exit = false;

        while (!exit)
        {
            DisplayMenu();
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "g":
                    AddGuard();
                    break;
                case "f":
                    AddFence();
                    break;
                case "s":
                    //AddSensor();
                    break;
                case "c":
                    //AddCamera();
                    break;
                case "d":
                    ShowSafeDirections();
                    break;
                case "m":
                    DisplayObstacleMap();
                    break;
                case "p":
                    FindSafePath();
                    break;
                case "x":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
    }

    private static void DisplayMenu()
    {
        Console.WriteLine("Select one of the following options");
        Console.WriteLine("g) Add 'Guard' obstacle");
        Console.WriteLine("f) Add 'Fence' obstacle");
        Console.WriteLine("s) Add 'Sensor' obstacle");
        Console.WriteLine("c) Add 'Camera' obstacle");
        Console.WriteLine("d) Show safe directions");
        Console.WriteLine("m) Display obstacle map");
        Console.WriteLine("p) Find safe path");
        Console.WriteLine("x) Exit");
        Console.Write("Enter code:\n");
    }

    private static void AddGuard()
    {
        Console.WriteLine("Enter the Guard's location (X,Y):");
        var location = ReadCoordinates();

        if (!obstacles.ContainsKey("g"))
        {
            obstacles["g"] = new List<Tuple<int, int>>();
        }

        obstacles["g"].Add(location);
    }

    private static void AddFence()
    {
        Console.WriteLine("Enter the location where the fence starts (X,Y):");
        var startLocation = ReadCoordinates();
        Console.WriteLine("Enter the location where the fence ends (X,Y):");
        var endLocation = ReadCoordinates();

        if ((startLocation.Item1 != endLocation.Item1 && startLocation.Item2 != endLocation.Item2) ||
            (startLocation.Item1 == endLocation.Item1 && startLocation.Item2 == endLocation.Item2))
        {
            Console.WriteLine("Fences must be horizontal or vertical.");
            AddFence(); // Restart fence addition process.
            return;
        }

        if (!obstacles.ContainsKey("f"))
        {
            obstacles["f"] = new List<Tuple<int, int>>();
        }

        if (startLocation.Item1 == endLocation.Item1) // vertical fence
        {
            int startY = Math.Min(startLocation.Item2, endLocation.Item2);
            int endY = Math.Max(startLocation.Item2, endLocation.Item2);
            for (int y = startY; y <= endY; y++)
            {
                obstacles["f"].Add(new Tuple<int, int>(startLocation.Item1, y));
            }
        }
        else // horizontal fence
        {
            int startX = Math.Min(startLocation.Item1, endLocation.Item1);
            int endX = Math.Max(startLocation.Item1, endLocation.Item1);
            for (int x = startX; x <= endX; x++)
            {
                obstacles["f"].Add(new Tuple<int, int>(x, startLocation.Item2));
            }
        }
    }

    private static Tuple<int, int> ReadCoordinates()
    {
        while (true)
        {
            string[] parts = Console.ReadLine().Split(',');
            if (parts.Length == 2 && int.TryParse(parts[0], out int x) && int.TryParse(parts[1], out int y))
                return new Tuple<int, int>(x, y);

            Console.WriteLine("Invalid input.");
        }
    }

    private static void ShowSafeDirections()
    {
        Console.WriteLine("Enter your current location (X,Y):");
        var currentLocation = ReadCoordinates();

        List<string> safeDirections = new List<string> { "N", "E", "S", "W" };

        foreach (var obstacleType in obstacles)
        {
            foreach (var obstacleLocation in obstacleType.Value)
            {
                if (currentLocation.Item1 == obstacleLocation.Item1 && currentLocation.Item2 - 1 == obstacleLocation.Item2)
                    safeDirections.Remove("N");

                if (currentLocation.Item1 + 1 == obstacleLocation.Item1 && currentLocation.Item2 == obstacleLocation.Item2)
                    safeDirections.Remove("E");


                if (currentLocation.Item1 == obstacleLocation.Item1 && currentLocation.Item2 + 1 == obstacleLocation.Item2)
                    safeDirections.Remove("S");

                if (currentLocation.Item1 - 1 == obstacleLocation.Item1 && currentLocation.Item2 == obstacleLocation.Item2)
                    safeDirections.Remove("W");
            }
        }

        Console.WriteLine($"You can safely take any of the following directions: {string.Join("", safeDirections)}");
    }

    private static void DisplayObstacleMap()
    {
        Console.WriteLine("........");
    }

    private static void FindSafePath()
    {
        Console.WriteLine("The following path will take you to the objective:\nNNNNEEEE");
    }
}
