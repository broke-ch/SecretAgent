/// <summary>
/// Represents an abstract obstacle class with a set of coordinates.
/// </summary>
public abstract class Obstacle
{
    // set to store obstacle's coordinates.
    private HashSet<Tuple<int, int>> _coordinates = new HashSet<Tuple<int, int>>();

    // property to expose the obstacle's coordinates.
    public IEnumerable<Tuple<int, int>> Coordinates => _coordinates;

    // method to be implemented by specific obstacle types for adding coordinates.
    public abstract void Add();

    // method to add a single coordinate to the set.
    protected void AddCoordinate(Tuple<int, int> coordinate)
    {
        _coordinates.Add(coordinate);
    }

    // method to add a range of coordinates to the set.
    protected void AddRangeOfCoordinates(IEnumerable<Tuple<int, int>> coordinates)
    {
        foreach (var coord in coordinates)
        {
            _coordinates.Add(coord);
        }
    }
}

// Guard obstacle.
public class Guard : Obstacle
{
    // Overrides the Add method to get and store the Guard's location.
    public override void Add()
    {
        try
        {
            Console.WriteLine("Enter the Guard's location (X,Y):");
            AddCoordinate(Utility.ReadCoordinates());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}

// Fence obstacle.
public class Fence : Obstacle
{
    // Overrides the Add method to get and store the Fence's start and end locations.
    public override void Add()
    {
        try
        {
            Console.WriteLine("Enter the location where the fence starts (X,Y):");
            var startLocation = Utility.ReadCoordinates();
            Console.WriteLine("Enter the location where the fence ends (X,Y):");
            var endLocation = Utility.ReadCoordinates();

            if (!Utility.IsValidFence(startLocation, endLocation))
            {
                Console.WriteLine("Fences must be horizontal or vertical.");
                Add();
            }
            else
            {
                AddRangeOfCoordinates(Utility.GenerateFenceCoordinates(startLocation, endLocation));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}

// Sensor obstacle.
public class Sensor : Obstacle
{
    // Overrides the Add method to get and store the Sensor's location and range.
    public override void Add()
    {
        try
        {
            Console.WriteLine("Enter the sensor's location (X,Y):");
            var location = Utility.ReadCoordinates();
            float range = Utility.ReadPositiveFloat("Enter the sensor's range (in klicks):");
            AddRangeOfCoordinates(Utility.GenerateSensorRangeCoordinates(location, range));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}

// Camera obstacle.
public class Camera : Obstacle
{
    // Overrides the Add method to get and store the Camera's location and facing direction.
    public override void Add()
    {
        try
        {
            Console.WriteLine("Enter the camera's location (X,Y):");
            var location = Utility.ReadCoordinates();
            char direction = Utility.ReadDirection("Enter the direction the camera is facing (n, s, e, w):");
            AddRangeOfCoordinates(Utility.GenerateCameraVisionCoordinates(location, direction));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}

// Nanobot obstacle.
public class Nanobot : Obstacle
{
    // Overrides the Add method to get and store the Nanobot field's top-left, bottom-right locations and number of nanobots.
    public override void Add()
    {
        try
        {
            Console.WriteLine("Enter the location of the top-left cell of the nanobot field (X,Y):");
            var topLeft = Utility.ReadCoordinates();
            Console.WriteLine("Enter the location of the bottom-right cell of the nanobot field (X,Y):");
            var bottomRight = Utility.ReadCoordinates();

            if (bottomRight.Item1 < topLeft.Item1 || bottomRight.Item2 < topLeft.Item2)
            {
                Console.WriteLine("Invalid field specification.");
                return;
            }

            int numberOfBots = Utility.ReadPositiveInt("How many nanobots are there?");
            AddRangeOfCoordinates(Utility.GenerateNanobotCoordinates(topLeft, bottomRight, numberOfBots));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}

/// <summary>
/// Contains utility methods to assist with operations related to obstacles.
/// </summary>
public class Utility
{
    private static Random _random = new Random(); // made _random static
    // Reads coordinates from the user in the format (X,Y) and validates the input.
      public static Tuple<int, int> ReadCoordinates()
    {
        while (true)
        {
            try
            {
                string[] coords = Console.ReadLine().Split(',');
                if (coords.Length == 2 && int.TryParse(coords[0], out int x) && int.TryParse(coords[1], out int y))
                    return new Tuple<int, int>(x, y);
                throw new Exception("Invalid input format for coordinates. Expected format: X,Y");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    } 

    // calculate the Euclidean distance between two points.
    public static double EuclideanDistance(int x1, int y1, int x2, int y2)
    {
        return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
    }

    // check if the given start and end points form a valid horizontal or vertical fence.
    public static bool IsValidFence(Tuple<int, int> start, Tuple<int, int> end)
    {
        return (start.Item1 == end.Item1 || start.Item2 == end.Item2) &&
               !(start.Item1 == end.Item1 && start.Item2 == end.Item2);
    }

    // create a list of coordinates for a fence based on its start and end points.
    public static List<Tuple<int, int>> GenerateFenceCoordinates(Tuple<int, int> start, Tuple<int, int> end)
    {
        var coordinates = new List<Tuple<int, int>>();

        // check for vertical fence
        if (start.Item1 == end.Item1)
        {
            int startY = Math.Min(start.Item2, end.Item2);
            int endY = Math.Max(start.Item2, end.Item2);
            for (int y = startY; y <= endY; y++)
            {
                coordinates.Add(new Tuple<int, int>(start.Item1, y));
            }
        }
        // check for horizontal fence
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

    // read and validate a positive floating point value from the user for sensor range
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

    public static int ReadPositiveInt(string prompt)
    {
        Console.WriteLine(prompt);
        while (true)
        {
            if (int.TryParse(Console.ReadLine(), out int result) && result > 0)
                return result;
            Console.WriteLine("Invalid input. Please enter a positive integer.");
        }
    }

    // reads and validates a direction character (n, s, e, w) from the user.
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

    // create a list of coordinates covered by a sensor's range.
    public static HashSet<Tuple<int, int>> GenerateSensorRangeCoordinates(Tuple<int, int> location, float range)
    {
        var coordinates = new HashSet<Tuple<int, int>>();

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
    public static HashSet<Tuple<int, int>> GenerateNanobotCoordinates(Tuple<int, int> topLeft, Tuple<int, int> bottomRight, int numberOfBots)
    {
        var coordinates = new HashSet<Tuple<int, int>>();

        while (coordinates.Count < numberOfBots)
        {
            int x = _random.Next(topLeft.Item1, bottomRight.Item1 + 1);
            int y = _random.Next(topLeft.Item2, bottomRight.Item2 + 1);
            coordinates.Add(new Tuple<int, int>(x, y));
        }

        return coordinates;
    }

    // create a set of coordinates based on a camera's location and its viewing direction.
    public static HashSet<Tuple<int, int>> GenerateCameraVisionCoordinates(Tuple<int, int> location, char direction)
    {
        var coordinates = new HashSet<Tuple<int, int>>();
        int maxX = 1000;
        int maxY = 1000;
        int minX = -1000;
        int minY = -1000;

        // calculate its field of vision based on where the camera is facing
        switch (direction)
        {
            case 'n': // north
                for (int y = location.Item2; y >= minY; y--)
                {
                    int offset = location.Item2 - y;
                    for (int x = location.Item1 - offset; x <= location.Item1 + offset; x++)
                    {
                        coordinates.Add(new Tuple<int, int>(x, y));
                    }
                }
                break;
            case 'e': // east
                for (int x = location.Item1; x <= maxX; x++)
                {
                    int offset = x - location.Item1;
                    for (int y = location.Item2 - offset; y <= location.Item2 + offset; y++)
                    {
                        coordinates.Add(new Tuple<int, int>(x, y));
                    }
                }
                break;
            case 's': // south
                for (int y = location.Item2; y <= maxY; y++)
                {
                    int offset = y - location.Item2;
                    for (int x = location.Item1 - offset; x <= location.Item1 + offset; x++)
                    {
                        coordinates.Add(new Tuple<int, int>(x, y));
                    }
                }
                break;
            case 'w': // west
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

/// <summary>
/// Manages obstacles on a grid and provides utilities to navigate around them.
/// </summary>
public class ObstacleManager
{
    // list of all obstacles on the grid
    private List<Obstacle> Obstacles { get; } = new List<Obstacle>();

    // adds an obstacle to the grid.
    public void AddObstacle(Obstacle obstacle)
    {
        obstacle.Add();
        Obstacles.Add(obstacle);
    }

    // check if a specific coordinate is blocked by an obstacle.
    public bool IsCoordinateBlocked(Tuple<int, int> coordinate)
    {
        return Obstacles.Any(o => o.Coordinates.Contains(coordinate));
    }

    // inform the user about safe directions to move in from their current location.
    public void ShowSafeDirections()
    {
        // prompt the user for their current location
        Console.WriteLine("Enter your current location (X,Y):");
        var currentLocation = Utility.ReadCoordinates();

        // check if the agent is on an obstacle
        bool onObstacle = IsCoordinateBlocked(currentLocation);
        if (onObstacle)
        {
            Console.WriteLine("Agent, your location is compromised. Abort mission.");
            return;
        }

        // determine which directions are safe to move in
        List<string> safeDirections = new List<string> { "N", "S", "E", "W" };
        if (IsCoordinateBlocked(new Tuple<int, int>(currentLocation.Item1, currentLocation.Item2 - 1)))
            safeDirections.Remove("N");
        if (IsCoordinateBlocked(new Tuple<int, int>(currentLocation.Item1 + 1, currentLocation.Item2)))
            safeDirections.Remove("E");
        if (IsCoordinateBlocked(new Tuple<int, int>(currentLocation.Item1, currentLocation.Item2 + 1)))
            safeDirections.Remove("S");
        if (IsCoordinateBlocked(new Tuple<int, int>(currentLocation.Item1 - 1, currentLocation.Item2)))
            safeDirections.Remove("W");

        // inform the user of the safe directions
        if (safeDirections.Count == 0)
        {
            Console.WriteLine("You cannot safely move in any direction. Abort mission.");
        }
        else
        {
            Console.WriteLine($"You can safely take any of the following directions: {string.Join("", safeDirections)}");
        }
    }

    // display a visual representation of the grid with obstacles.
    public void DisplayObstacleMap()
    {
        // prompt the user for the boundaries of the map to be displayed
        Console.WriteLine("Enter the location of the top-left cell of the map (X,Y):");
        var topLeft = Utility.ReadCoordinates();
        Console.WriteLine("Enter the location of the bottom-right cell of the map (X,Y):");
        var bottomRight = Utility.ReadCoordinates();

        // ensure that the specified boundaries are valid
        if (bottomRight.Item1 < topLeft.Item1 || bottomRight.Item2 < topLeft.Item2)
        {
            Console.WriteLine("Invalid map specification.");
            DisplayObstacleMap(); // restart map display process.
            return;
        }

        // display the grid with obstacle symbols
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
                        if (obstacle is Fence) symbol = 'f';
                        if (obstacle is Camera) symbol = 'c';
                        if (obstacle is Sensor) symbol = 's';
                        if (obstacle is Nanobot) symbol = 'n';

                        break;
                    }
                }
                Console.Write(symbol);
            }
            Console.WriteLine();
        }
    }

    // return the neighbouring coordinates around a given location that are not blocked.
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

    // find and display a safe path from the user's location to the mission objective.
    public void FindSafePath()
    {
        // prompt the user for the start and end locations
        Console.WriteLine("Enter your current location (X,Y):");
        var start = Utility.ReadCoordinates();
        Console.WriteLine("Enter the location of the mission objective (X,Y):");
        var end = Utility.ReadCoordinates();

        // check if the start and end locations are the same or if the objective is blocked
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

        // try to find a path to the objective
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

    // internal method to attempt to find a path using Breadth-First Search
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

    // internal method to reconstruct the path from the cameFrom dictionary
    private static List<string> ReconstructPath(Dictionary<Tuple<int, int>, Tuple<int, int>> cameFrom, Tuple<int, int> start, Tuple<int, int> end)
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