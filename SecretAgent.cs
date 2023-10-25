public abstract class Obstacle
{
    public List<Tuple<int, int>> Coordinates { get; protected set; } = new List<Tuple<int, int>>();

    public abstract void Add();
}

public class Guard : Obstacle
{
    public override void Add()
    {
        Console.WriteLine("Enter the Guard's location (X,Y):");
        Coordinates.Add(Utility.ReadCoordinates());
    }
}

public class Fence : Obstacle
{
    public override void Add()
    {
        Console.WriteLine("Enter the location where the fence starts (X,Y):");
        var startLocation = Utility.ReadCoordinates();

        Console.WriteLine("Enter the location where the fence ends (X,Y):");
        var endLocation = Utility.ReadCoordinates();

        if (!Utility.IsValidFence(startLocation, endLocation))
        {
            Console.WriteLine("Fences must be horizontal or vertical.");
            Add();
            return;
        }

        Coordinates.AddRange(Utility.GenerateFenceCoordinates(startLocation, endLocation));
    }
}

public class Sensor : Obstacle
{
    public override void Add()
    {
        Console.WriteLine("Enter the sensor's location (X,Y):");
        var location = Utility.ReadCoordinates();

        float range = Utility.ReadPositiveFloat("Enter the sensor's range (in klicks):");
        Coordinates.AddRange(Utility.GenerateSensorRangeCoordinates(location, range));
    }
}

public class Camera : Obstacle
{
    public override void Add()
    {
        Console.WriteLine("Enter the camera's location (X,Y):");
        var location = Utility.ReadCoordinates();

        char direction = Utility.ReadDirection("Enter the direction the camera is facing (n, s, e, w):");
        Coordinates.AddRange(Utility.GenerateCameraVisionCoordinates(location, direction));
    }
}

public static class Utility
{
    public static Tuple<int, int> ReadCoordinates()
    {
        while (true)
        {
            string[] coords = Console.ReadLine().Split(',');
            if (coords.Length == 2 && int.TryParse(coords[0], out int x) && int.TryParse(coords[1], out int y))
                return new Tuple<int, int>(x, y);

            Console.WriteLine("Invalid input.");
        }
    }

    public static double EuclideanDistance(int x1, int y1, int x2, int y2)
    {
        return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
    }

    // Added utility methods for cleaner code

    public static bool IsValidFence(Tuple<int, int> start, Tuple<int, int> end)
    {
        return (start.Item1 == end.Item1 || start.Item2 == end.Item2) && 
               !(start.Item1 == end.Item1 && start.Item2 == end.Item2);
    }

public static List<Tuple<int, int>> GenerateFenceCoordinates(Tuple<int, int> start, Tuple<int, int> end)
{
        var coordinates = new List<Tuple<int, int>>();

        if (start.Item1 == end.Item1)
        {
            // Vertical fence
            int startY = Math.Min(start.Item2, end.Item2);
            int endY = Math.Max(start.Item2, end.Item2);
            for (int y = startY; y <= endY; y++)
            {
                coordinates.Add(new Tuple<int, int>(start.Item1, y));
            }
        }
        else if (start.Item2 == end.Item2)
        {
            // Horizontal fence
            int startX = Math.Min(start.Item1, end.Item1);
            int endX = Math.Max(start.Item1, end.Item1);
            for (int x = startX; x <= endX; x++)
            {
                coordinates.Add(new Tuple<int, int>(x, start.Item2));
            }
        }

        return coordinates;
    }

        public static float ReadPositiveFloat(string prompt)
        {
            Console.WriteLine(prompt);
            while (true) 
            {
                if (float.TryParse(Console.ReadLine(), out float result) && result > 0)
                    return result; 
                Console.WriteLine("Invalid input. Please enter a positive floating point number.");
            }
        }


        public static char ReadDirection(string prompt)
        {
            Console.WriteLine(prompt);
            while (true)
            {
                char direction = Console.ReadLine().ToLower()[0];
                if (new[] { 'n', 's', 'e', 'w' }.Contains(direction)) return direction;

                Console.WriteLine("Invalid direction.");
            }
        }

    public static List<Tuple<int, int>> GenerateSensorRangeCoordinates(Tuple<int, int> location, float range)
    {
        var coordinates = new List<Tuple<int, int>>();

        for (int x = (int)(location.Item1 - range); x <= location.Item1 + range; x++)
        {
            for (int y = (int)(location.Item2 - range); y <= location.Item2 + range; y++)
            {
                if (Utility.EuclideanDistance(location.Item1, location.Item2, x, y) <= range)
                {
                    coordinates.Add(new Tuple<int, int>(x, y));
                }
            }
        }
        return coordinates;
    }
    public static List<Tuple<int, int>> GenerateCameraVisionCoordinates(Tuple<int, int> location, char direction)
    {
        var coordinates = new List<Tuple<int, int>>();
    
        int maxX = 1000;
        int maxY = 1000;
        int minX = 0;
        int minY = 0;
    
        switch (direction)
        {
            case 'n':
                for (int y = location.Item2; y >= minY; y--)
                {
                    for (int offsetX = 0; offsetX <= location.Item2 - y; offsetX++)
                    {
                        coordinates.Add(new Tuple<int, int>(location.Item1 + offsetX, y)); // right diagonal
                        coordinates.Add(new Tuple<int, int>(location.Item1 - offsetX, y)); // left diagonal
                    }
                }
                break;
            case 'e':
                for (int x = location.Item1; x <= maxX; x++)
                {
                    for (int offsetY = 0; offsetY <= x - location.Item1; offsetY++)
                    {
                        coordinates.Add(new Tuple<int, int>(x, location.Item2 + offsetY)); // up diagonal
                        coordinates.Add(new Tuple<int, int>(x, location.Item2 - offsetY)); // down diagonal
                    }
                }
                break;
            case 's':
                for (int y = location.Item2; y <= maxY; y++)
                {
                    for (int offsetX = 0; offsetX <= y - location.Item2; offsetX++)
                    {
                        coordinates.Add(new Tuple<int, int>(location.Item1 + offsetX, y)); // right diagonal
                        coordinates.Add(new Tuple<int, int>(location.Item1 - offsetX, y)); // left diagonal
                    }
                }
                break;
            case 'w':
                for (int x = location.Item1; x >= minX; x--)
                {
                    for (int offsetY = 0; offsetY <= location.Item1 - x; offsetY++)
                    {
                        coordinates.Add(new Tuple<int, int>(x, location.Item2 + offsetY)); // up diagonal
                        coordinates.Add(new Tuple<int, int>(x, location.Item2 - offsetY)); // down diagonal
                    }
                }
                break;
        }
    
        return coordinates;
    } 
}

public class ObstacleManager
{
    private List<Obstacle> Obstacles { get; } = new List<Obstacle>();

    public void AddObstacle(Obstacle obstacle)
    {
        obstacle.Add();
        Obstacles.Add(obstacle);
    }

    public bool IsCoordinateBlocked(Tuple<int, int> coordinate)
    {
        return Obstacles.Any(o => o.Coordinates.Contains(coordinate));
    }

    public void ShowSafeDirections()
    {
        Console.WriteLine("Enter your current location (X,Y):");
        var currentLocation = Utility.ReadCoordinates();

        // Check if the agent is on an obstacle
        bool onObstacle = IsCoordinateBlocked(currentLocation);
        if (onObstacle)
        {
            Console.WriteLine("Agent, your location is compromised. Abort mission.");
            return;
        }

        List<string> safeDirections = new List<string> { "N", "S", "E", "W" };

        if (IsCoordinateBlocked(new Tuple<int, int>(currentLocation.Item1, currentLocation.Item2 - 1)))
            safeDirections.Remove("N");

        if (IsCoordinateBlocked(new Tuple<int, int>(currentLocation.Item1 + 1, currentLocation.Item2)))
            safeDirections.Remove("E");

        if (IsCoordinateBlocked(new Tuple<int, int>(currentLocation.Item1, currentLocation.Item2 + 1)))
            safeDirections.Remove("S");

        if (IsCoordinateBlocked(new Tuple<int, int>(currentLocation.Item1 - 1, currentLocation.Item2)))
            safeDirections.Remove("W");

        if (safeDirections.Count == 0)
        {
            Console.WriteLine("You cannot safely move in any direction. Abort mission.");
        }
        else
        {
            Console.WriteLine($"You can safely take any of the following directions: {string.Join("", safeDirections)}");
        }
    }

    public void DisplayObstacleMap()
    {
        Console.WriteLine("Enter the location of the top-left cell of the map (X,Y):");
        var topLeft = Utility.ReadCoordinates();

        Console.WriteLine("Enter the location of the bottom-right cell of the map (X,Y):");
        var bottomRight = Utility.ReadCoordinates();

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
                char symbol = '.';

                foreach (var obstacle in Obstacles)
                {
                    if (obstacle.Coordinates.Contains(new Tuple<int, int>(x, y)))
                    {
                        if (obstacle is Guard) symbol = 'g';
                        else if (obstacle is Fence) symbol = 'f';
                        else if (obstacle is Camera) symbol = 'c';
                        else if (obstacle is Sensor) symbol = 's';
                        break; // Exit the loop as soon as we find an obstacle at this coordinate.
                    }
                }

                Console.Write(symbol);
            }

            Console.WriteLine();
        }
    }
}
