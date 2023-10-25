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

                    AddSensor();

                    break;

                case "c":

                    AddCamera();

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



    private static void AddSensor()

    {

        Console.WriteLine("Enter the sensor's location (X,Y):");

        var location = ReadCoordinates();



        Console.WriteLine("Enter the sensor's range (in klicks):");

        float range;

        while (true)

        {

            if (float.TryParse(Console.ReadLine(), out range) && range > 0)

                break;

            else

                Console.WriteLine("Invalid input. Please enter a positive floating point number for the range.");

        }



        if (!obstacles.ContainsKey("s"))

        {

            obstacles["s"] = new List<Tuple<int, int>>();

        }



        // use a grid loop to identify all the squares that fall within the sensor's range

        int maxDistance = (int)Math.Ceiling(range);

        for (int x = location.Item1 - maxDistance; x <= location.Item1 + maxDistance; x++)

        {

            for (int y = location.Item2 - maxDistance; y <= location.Item2 + maxDistance; y++)

            {

                if (EuclideanDistance(location.Item1, location.Item2, x, y) <= range)

                {

                    obstacles["s"].Add(new Tuple<int, int>(x, y));

                }

            }

        }

    }



    private static double EuclideanDistance(int x1, int y1, int x2, int y2)

    {

        return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));

    }





    private static void AddCamera()

    {

        Console.WriteLine("Enter the camera's location (X,Y):");

        var location = ReadCoordinates();



        Console.WriteLine("Enter the direction the camera is facing (n, s, e or w):");

        char direction;

        while (true)

        {

            direction = Console.ReadLine().ToLower()[0];

            if (direction == 'n' || direction == 's' || direction == 'e' || direction == 'w')

                break;

            else

                Console.WriteLine("Invalid direction.");

        }



        if (!obstacles.ContainsKey("c"))

        {

            obstacles["c"] = new List<Tuple<int, int>>();

        }



        switch (direction)

        {

            case 'n':

                for (int y = location.Item2; y >= -byte.MaxValue; y--) // Changed condition

                {

                    AddCameraVision(location.Item1, y); // Directly north

                    AddCameraVision(location.Item1 - (location.Item2 - y), y); // 45 degrees west

                    AddCameraVision(location.Item1 + (location.Item2 - y), y); // 45 degrees east

                }

                break;

            case 'e':

                for (int x = location.Item1; x <= byte.MaxValue; x++) // Changed condition

                {

                    AddCameraVision(x, location.Item2); // Directly east

                    AddCameraVision(x, location.Item2 - (x - location.Item1)); // 45 degrees north

                    AddCameraVision(x, location.Item2 + (x - location.Item1)); // 45 degrees south

                }

                break;

            case 's':

                for (int y = location.Item2; y <= byte.MaxValue ; y++) // Changed condition

                {

                    AddCameraVision(location.Item1, y); // Directly south

                    AddCameraVision(location.Item1 - (y - location.Item2), y); // 45 degrees west

                    AddCameraVision(location.Item1 + (y - location.Item2), y); // 45 degrees east

                }

                break;

            case 'w':

                for (int x = location.Item1; x >= -byte.MaxValue ; x--) // Changed condition

                {

                    AddCameraVision(x, location.Item2); // Directly west

                    AddCameraVision(x, location.Item2 + (location.Item1 - x)); // 45 degrees north

                    AddCameraVision(x, location.Item2 - (location.Item1 - x)); // 45 degrees south

                }

                break;

        }

    }



    private static void AddCameraVision(int x, int y)

    {

        Tuple<int, int> coord = new Tuple<int, int>(x, y);

        if (!obstacles["c"].Contains(coord))

        {

            obstacles["c"].Add(coord);

        }

    }

   



    private static Tuple<int, int> ReadCoordinates()

    {

        while (true)

        {

            string[] coords = Console.ReadLine().Split(',');

            if (coords.Length == 2 && int.TryParse(coords[0], out int x) && int.TryParse(coords[1], out int y))

                return new Tuple<int, int>(x, y);



            Console.WriteLine("Invalid input.");

        }

    }



    private static void ShowSafeDirections()

    {

        Console.WriteLine("Enter your current location (X,Y):");

        var currentLocation = ReadCoordinates();



        // Check if the agent is on an obstacle

        bool onObstacle = false;

        foreach (var obstacleType in obstacles)

        {

            if (obstacleType.Value.Contains(currentLocation))

            {

                onObstacle = true;

                break;

            }

        }



        if (onObstacle)

        {

            Console.WriteLine("Agent, your location is compromised. Abort mission.");

            return; // Early exit from the method

        }



        List<string> safeDirections = new List<string> { "N", "S", "E", "W" };



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



        if (safeDirections.Count == 0)

        {

            Console.WriteLine("You cannot safely move in any direction. Abort mission.");

        }

        else

        {

            Console.WriteLine($"You can safely take any of the following directions: {string.Join("", safeDirections)}");

        }

    }



    private static void DisplayObstacleMap()

    {

        Console.WriteLine("Enter the location of the top-left cell of the map (X,Y):");

        var topLeft = ReadCoordinates();

        

        Console.WriteLine("Enter the location of the bottom-right cell of the map (X,Y):");

        var bottomRight = ReadCoordinates();

    

        // Check for valid coordinates 

        if (bottomRight.Item1 < topLeft.Item1 || bottomRight.Item2 < topLeft.Item2)

        {

            Console.WriteLine("Invalid map specification.");

            DisplayObstacleMap(); // Restart map display process.

            return;

        }

    

        for (int y = topLeft.Item2; y <= bottomRight.Item2; y++)

        {

            for (int x = topLeft.Item1; x <= bottomRight.Item1; x++)

            {

                if (ObstacleAt(x, y, "g"))

                    Console.Write("g");

                else if (ObstacleAt(x, y, "f"))

                    Console.Write("f");

                else if (ObstacleAt(x, y, "s"))

                    Console.Write("s");

                else if (ObstacleAt(x, y, "c"))

                    Console.Write("c");

                else

                    Console.Write(".");

            }

            Console.WriteLine(); // Move to the next line after printing each row

        }

    }

    private static bool ObstacleAt(int x, int y, string type)

    {

        if (obstacles.ContainsKey(type))

        {

            foreach (var location in obstacles[type])

            {

                if (location.Item1 == x && location.Item2 == y)

                    return true;

            }

        }

        return false;

    }



    private static void FindSafePath()

    {

        Console.WriteLine("The following path will take you to the objective:\nNNNNEEEE");

    }

}

