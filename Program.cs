using System;
using System.Collections.Generic;
using System.Linq;

public class Program
{
    // create an instance of ObstacleManager to manage obstacles and navigation.
    private static ObstacleManager manager = new ObstacleManager();

    public static void Main()
    {
        // infinite loop to repeatedly display the menu and handle user's choices.
        while (true)
        {
            DisplayMenu();
            HandleMenuChoice(Console.ReadLine().ToLower());
        }
    }

    // display the main menu with the available operations.
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

    // Handles the user's choice from the main menu.
    private static void HandleMenuChoice(string choice)
    {
        switch (choice)
        {
            case "g":
                // add a Guard obstacle.
                manager.AddObstacle(new Guard());
                break;
            case "f":
                // add a Fence obstacle.
                manager.AddObstacle(new Fence());
                break;
            case "s":
                // add a Sensor obstacle.
                manager.AddObstacle(new Sensor());
                break;
            case "c":
                // add a Camera obstacle.
                manager.AddObstacle(new Camera());
                break;
            case "d":
                // show directions that are safe to move in.
                manager.ShowSafeDirections();
                break;
            case "m":
                // display a map with all the obstacles.
                manager.DisplayObstacleMap();
                break;
            case "p":
                // find a safe path from a starting point to a destination.
                manager.FindSafePath();
                break;
            case "x":
                // exit the program.
                Environment.Exit(0);
                break;
            default:
                // handle an invalid menu choice.
                Console.WriteLine("Invalid choice. Please choose again.");
                break;
        }
    }
}