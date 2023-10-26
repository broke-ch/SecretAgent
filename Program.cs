using System;
using System.Collections.Generic;
using System.Linq;

public class Program
{
    private static ObstacleManager manager = new ObstacleManager();

    public static void Main()
    {
        while (true)
        {
            DisplayMenu();
            HandleMenuChoice(Console.ReadLine().ToLower());
        }
    }

    private static void DisplayMenu()
    {
        Console.WriteLine("Select one of the following options:");
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

    private static void HandleMenuChoice(string choice)
    {
        switch (choice)
        {
            case "g":
                manager.AddObstacle(new Guard());
                break;
            case "f":
                manager.AddObstacle(new Fence());
                break;
            case "s":
                manager.AddObstacle(new Sensor());
                break;
            case "c":
                manager.AddObstacle(new Camera());
                break;
            case "d":
                manager.ShowSafeDirections();
                break;
            case "m":
                manager.DisplayObstacleMap();
                break;
            case "p":
                manager.FindSafePath();
                break;
            case "x":
                Environment.Exit(0);
                break;
            default:
                Console.WriteLine("Invalid choice. Please choose again.");
                break;
        }
    }
}