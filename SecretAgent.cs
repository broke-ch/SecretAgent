// Abstract base class representing an obstacle.
public abstract class Obstacle
{
    // HashSet storing the coordinates of the obstacle.
    public HashSet<Tuple<int, int>> Coordinates { get; protected set; } = new HashSet<Tuple<int, int>>();

    // Abstract method to add the obstacle. Each derived class must provide its own implementation.
    public abstract void Add();
}

// Derived class representing a Guard obstacle.
public class Guard : Obstacle
{
    // Implementation for adding a Guard's location.
    public override void Add()
    {
        Console.WriteLine("Enter the Guard's location (X,Y):");
        Coordinates.Add(Utility.ReadCoordinates());
    }
}

// Derived class representing a Fence obstacle.
public class Fence : Obstacle
{
    // Implementation for adding the start and end location of a fence.
    public override void Add()
    {
        Console.WriteLine("Enter the location where the fence starts (X,Y):");
        var startLocation = Utility.ReadCoordinates();

        Console.WriteLine("Enter the location where the fence ends (X,Y):");
        var endLocation = Utility.ReadCoordinates();

        // Validate if the fence is either horizontal or vertical.
        if (!Utility.IsValidFence(startLocation, endLocation))
        {
            Console.WriteLine("Fences must be horizontal or vertical.");
            Add();
            return;
        }

        // Generate and store the coordinates for the entire fence based on start and end locations.
        Coordinates.UnionWith(Utility.GenerateFenceCoordinates(startLocation, endLocation));
    }
}

// Derived class representing a Sensor obstacle.
public class Sensor : Obstacle
{
    // Implementation for adding a sensor's location and its range.
    public override void Add()
    {
        Console.WriteLine("Enter the sensor's location (X,Y):");
        var location = Utility.ReadCoordinates();

        float range = Utility.ReadPositiveFloat("Enter the sensor's range (in klicks):");
        // Generate and store the coordinates covered by the sensor's range.
        Coordinates.UnionWith(Utility.GenerateSensorRangeCoordinates(location, range));
    }
}

// Derived class representing a Camera obstacle.
public class Camera : Obstacle
{
    // Implementation for adding a camera's location and its viewing direction.
    public override void Add()
    {
        Console.WriteLine("Enter the camera's location (X,Y):");
        var location = Utility.ReadCoordinates();

        char direction = Utility.ReadDirection("Enter the direction the camera is facing (n, s, e, w):");
        // Generate and store the coordinates covered by the camera's field of vision.
        Coordinates.UnionWith(Utility.GenerateCameraVisionCoordinates(location, direction));
    }
}

// Utility class containing methods to assist with operations related to obstacles.
public static class Utility
{
    // Reads coordinates from the user in the format (X,Y) and validates the input.
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

    // Calculates the Euclidean distance between two points.
    public static double EuclideanDistance(int x1, int y1, int x2, int y2)
    {
        return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
    }

    // Checks if the given start and end points form a valid horizontal or vertical fence.
    public static bool IsValidFence(Tuple<int, int> start, Tuple<int, int> end)
    {
        return (start.Item1 == end.Item1 || start.Item2 == end.Item2) &&
               !(start.Item1 == end.Item1 && start.Item2 == end.Item2);
    }

    // Generates a list of coordinates for a fence based on its start and end points.
    public static List<Tuple<int, int>> GenerateFenceCoordinates(Tuple<int, int> start, Tuple<int, int> end)
    {
        var coordinates = new List<Tuple<int, int>>();

        // Check for vertical fence
        if (start.Item1 == end.Item1)
        {
            int startY = Math.Min(start.Item2, end.Item2);
            int endY = Math.Max(start.Item2, end.Item2);
            for (int y = startY; y <= endY; y++)
            {
                coordinates.Add(new Tuple<int, int>(start.Item1, y));
            }
        }
        // Check for horizontal fence
        else if (start.Item2 == end.Item2)
        {
            int startX = Math.Min(start.Item1, end.Item1);
            int endX = Math.Max(start.Item1, end.Item1);
            for (int x = startX; x <= endX; x++)
            {
                coordinates.Add(new Tuple<int, int>(x, start.Item2));
            }
        }

        return coordinates;
    }

    // Reads and validates a positive floating point value from the user.
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

    // Reads and validates a direction character (n, s, e, w) from the user.
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

    // Generates a list of coordinates covered by a sensor's range.
    public static List<Tuple<int, int>> GenerateSensorRangeCoordinates(Tuple<int, int> location, float range)
    {
        var coordinates = new List<Tuple<int, int>>();

        int min_x = (int)Math.Floor(location.Item1 - range);
        int max_x = (int)Math.Ceiling(location.Item1 + range);
        int min_y = (int)Math.Floor(location.Item2 - range);
        int max_y = (int)Math.Ceiling(location.Item2 + range);

        for (int x = min_x; x <= max_x; x++)
        {
            for (int y = min_y; y <= max_y; y++)
            {
                if (EuclideanDistance(location.Item1, location.Item2, x, y) <= range)
                {
                    coordinates.Add(new Tuple<int, int>(x, y));
                }
            }
        }

        return coordinates;
    }

    // Generates a set of coordinates based on a camera's location and its viewing direction.
    public static HashSet<Tuple<int, int>> GenerateCameraVisionCoordinates(Tuple<int, int> location, char direction)
    {
        var coordinates = new HashSet<Tuple<int, int>>();
        int maxX = 1000;
        int maxY = 1000;
        int minX = -1000;
        int minY = -1000;

        // Depending on the camera's direction, calculate its field of vision
        switch (direction)
        {
            case 'n':
                for (int y = location.Item2; y >= minY; y--)
                {
                    int offset = location.Item2 - y;
                    for (int x = location.Item1 - offset; x <= location.Item1 + offset; x++)
                    {
                        coordinates.Add(new Tuple<int, int>(x, y));
                    }
                }
                break;
            case 'e':
                for (int x = location.Item1; x <= maxX; x++)
                {
                    int offset = x - location.Item1;
                    for (int y = location.Item2 - offset; y <= location.Item2 + offset; y++)
                    {
                        coordinates.Add(new Tuple<int, int>(x, y));
                    }
                }
                break;
            case 's':
                for (int y = location.Item2; y <= maxY; y++)
                {
                    int offset = y - location.Item2;
                    for (int x = location.Item1 - offset; x <= location.Item1 + offset; x++)
                    {
                        coordinates.Add(new Tuple<int, int>(x, y));
                    }
                }
                break;
            case 'w':
                for (int x = location.Item1; x >= minX; x--)
                {
                    int offset = location.Item1 - x;
                    for (int y = location.Item2 - offset; y <= location.Item2 + offset; y++)
                    {
                        coordinates.Add(new Tuple<int, int>(x, y));
                    }
                }
                break;
        }

        return coordinates;
    }
}

// Manages the obstacles on a grid and provides utilities to navigate around them.
public class ObstacleManager
{
    // List of all obstacles on the grid
    private List<Obstacle> Obstacles { get; } = new List<Obstacle>();

    // Adds an obstacle to the grid.
    public void AddObstacle(Obstacle obstacle)
    {
        obstacle.Add();
        Obstacles.Add(obstacle);
    }

    // Checks if a specific coordinate is blocked by an obstacle.
    public bool IsCoordinateBlocked(Tuple<int, int> coordinate)
    {
        return Obstacles.Any(o => o.Coordinates.Contains(coordinate));
    }

    // Informs the user about safe directions to move in from their current location.
    public void ShowSafeDirections()
    {
        // Prompt the user for their current location
        Console.WriteLine("Enter your current location (X,Y):");
        var currentLocation = Utility.ReadCoordinates();

        // Check if the agent is on an obstacle
        bool onObstacle = IsCoordinateBlocked(currentLocation);
        if (onObstacle)
        {
            Console.WriteLine("Agent, your location is compromised. Abort mission.");
            return;
        }

        // Determine which directions are safe to move in
        List<string> safeDirections = new List<string> { "N", "S", "E", "W" };
        if (IsCoordinateBlocked(new Tuple<int, int>(currentLocation.Item1, currentLocation.Item2 - 1)))
            safeDirections.Remove("N");
        if (IsCoordinateBlocked(new Tuple<int, int>(currentLocation.Item1 + 1, currentLocation.Item2)))
            safeDirections.Remove("E");
        if (IsCoordinateBlocked(new Tuple<int, int>(currentLocation.Item1, currentLocation.Item2 + 1)))
            safeDirections.Remove("S");
        if (IsCoordinateBlocked(new Tuple<int, int>(currentLocation.Item1 - 1, currentLocation.Item2)))
            safeDirections.Remove("W");

        // Inform the user of the safe directions
        if (safeDirections.Count == 0)
        {
            Console.WriteLine("You cannot safely move in any direction. Abort mission.");
        }
        else
        {
            Console.WriteLine($"You can safely take any of the following directions: {string.Join("", safeDirections)}");
        }
    }

    // Displays a visual representation of the grid with obstacles.
    public void DisplayObstacleMap()
    {
        // Prompt the user for the boundaries of the map to be displayed
        Console.WriteLine("Enter the location of the top-left cell of the map (X,Y):");
        var topLeft = Utility.ReadCoordinates();
        Console.WriteLine("Enter the location of the bottom-right cell of the map (X,Y):");
        var bottomRight = Utility.ReadCoordinates();

        // Ensure that the specified boundaries are valid
        if (bottomRight.Item1 < topLeft.Item1 || bottomRight.Item2 < topLeft.Item2)
        {
            Console.WriteLine("Invalid map specification.");
            DisplayObstacleMap(); // Restart map display process.
            return;
        }

        // Display the grid with obstacle symbols
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
                        break;
                    }
                }
                Console.Write(symbol);
            }
            Console.WriteLine();
        }
    }

    // Returns the neighboring coordinates around a given location that are not blocked.
    public List<Tuple<int, int>> GetNeighbors(Tuple<int, int> location)
    {
        List<Tuple<int, int>> neighbors = new List<Tuple<int, int>>
        {
            new Tuple<int, int>(location.Item1, location.Item2 - 1), // North
            new Tuple<int, int>(location.Item1 + 1, location.Item2), // East
            new Tuple<int, int>(location.Item1, location.Item2 + 1), // South
            new Tuple<int, int>(location.Item1 - 1, location.Item2)  // West
        };

        return neighbors.Where(coord => !IsCoordinateBlocked(coord)).ToList();
    }

    // Finds and displays a safe path from the user's location to the mission objective.
    public void FindSafePath()
    {
        // Prompt the user for the start and end locations
        Console.WriteLine("Enter your current location (X,Y):");
        var start = Utility.ReadCoordinates();
        Console.WriteLine("Enter the location of the mission objective (X,Y):");
        var end = Utility.ReadCoordinates();

        // Check if the start and end locations are the same or if the objective is blocked
        if (start.Item1 == end.Item1 && start.Item2 == end.Item2)
        {
            Console.WriteLine("Agent, you are already at the objective.");
            return;
        }
        if (IsCoordinateBlocked(end))
        {
            Console.WriteLine("The objective is blocked by an obstacle and cannot be reached.");
            return;
        }

        // Try to find a path to the objective
        if (TryFindPath(start, end, out var cameFrom))
        {
            Console.WriteLine("The following path will take you to the objective:");
            List<string> path = ReconstructPath(cameFrom, start, end);
            Console.WriteLine(string.Join("", path));
        }
        else
        {
            Console.WriteLine("There is no safe path to the objective.");
        }
    }

    // Internal method to attempt to find a path using Breadth-First Search
    private bool TryFindPath(Tuple<int, int> start, Tuple<int, int> end, out Dictionary<Tuple<int, int>, Tuple<int, int>> cameFrom)
    {
        Queue<Tuple<int, int>> frontier = new Queue<Tuple<int, int>>();
        frontier.Enqueue(start);

        cameFrom = new Dictionary<Tuple<int, int>, Tuple<int, int>> { [start] = null };

        while (frontier.Any())
        {
            var current = frontier.Dequeue();
            if (current.Item1 == end.Item1 && current.Item2 == end.Item2)
            {
                return true;
            }

            foreach (var next in GetNeighbors(current))
            {
                if (!cameFrom.ContainsKey(next))
                {
                    frontier.Enqueue(next);
                    cameFrom[next] = current;
                }
            }
        }

        return false;
    }

    // Internal method to reconstruct the path from the cameFrom dictionary
    private List<string> ReconstructPath(Dictionary<Tuple<int, int>, Tuple<int, int>> cameFrom, Tuple<int, int> start, Tuple<int, int> end)
    {
        List<string> path = new List<string>();
        var current = end;

        while (current != start)
        {
            var previous = cameFrom[current];
            if (previous.Item1 == current.Item1)
            {
                path.Add(previous.Item2 < current.Item2 ? "S" : "N");
            }
            else if (previous.Item2 == current.Item2)
            {
                path.Add(previous.Item1 < current.Item1 ? "E" : "W");
            }

            current = previous;
        }

        path.Reverse();
        return path;
    }
}