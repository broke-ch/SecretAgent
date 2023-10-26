public abstract class Obstacle

{

    public HashSet<Tuple<int, int>> Coordinates { get; protected set; } = new HashSet<Tuple<int, int>>();

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



        Coordinates.UnionWith(Utility.GenerateFenceCoordinates(startLocation, endLocation));

    }

}



public class Sensor : Obstacle

{

    public override void Add()

    {

        Console.WriteLine("Enter the sensor's location (X,Y):");

        var location = Utility.ReadCoordinates();



        float range = Utility.ReadPositiveFloat("Enter the sensor's range (in klicks):");

        Coordinates.UnionWith(Utility.GenerateSensorRangeCoordinates(location, range));

    }

}



public class Camera : Obstacle

{

    public override void Add()

    {

        Console.WriteLine("Enter the camera's location (X,Y):");

        var location = Utility.ReadCoordinates();



        char direction = Utility.ReadDirection("Enter the direction the camera is facing (n, s, e, w):");

        Coordinates.UnionWith(Utility.GenerateCameraVisionCoordinates(location, direction));

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



    public static HashSet<Tuple<int, int>> GenerateCameraVisionCoordinates(Tuple<int, int> location, char direction)

    {

        var coordinates = new HashSet<Tuple<int, int>>();



        int maxX = 1000;

        int maxY = 1000;

        int minX = -1000;

        int minY = -1000;



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

    



    // Gets the neighbouring coordinates of the given location in North, East, South, and West directions.

    public List<Tuple<int, int>> GetNeighbors(Tuple<int, int> location)

    {

        // Define the neighboring coordinates

        List<Tuple<int, int>> neighbors = new List<Tuple<int, int>>

        {

            new Tuple<int, int>(location.Item1, location.Item2 - 1), // North

            new Tuple<int, int>(location.Item1 + 1, location.Item2), // East

            new Tuple<int, int>(location.Item1, location.Item2 + 1), // South

            new Tuple<int, int>(location.Item1 - 1, location.Item2)  // West

        };



        // Return only the neighbors that are not blocked

        return neighbors.Where(coord => !IsCoordinateBlocked(coord)).ToList();

    }



    // Guides the user to find a safe path from their current location to an objective.

    public void FindSafePath()

    {

        Console.WriteLine("Enter your current location (X,Y):");

        var start = Utility.ReadCoordinates();



        Console.WriteLine("Enter the location of the mission objective (X,Y):");

        var end = Utility.ReadCoordinates();



        // Check if the user is already at the objective

        if (start.Item1 == end.Item1 && start.Item2 == end.Item2)

        {

            Console.WriteLine("Agent, you are already at the objective.");

            return;

        }



        // Check if the objective is blocked

        if (IsCoordinateBlocked(end))

        {

            Console.WriteLine("The objective is blocked by an obstacle and cannot be reached.");

            return;

        }



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



    // Uses Breadth-First Search (BFS) to determine if a path exists between the start and end coordinates.

    private bool TryFindPath(Tuple<int, int> start, Tuple<int, int> end, out Dictionary<Tuple<int, int>, Tuple<int, int>> cameFrom)

    {

        Queue<Tuple<int, int>> frontier = new Queue<Tuple<int, int>>();

        frontier.Enqueue(start);



        cameFrom = new Dictionary<Tuple<int, int>, Tuple<int, int>> { [start] = null };



        while (frontier.Any())

        {

            var current = frontier.Dequeue();



            // Check if the current location is the end location

            if (current.Item1 == end.Item1 && current.Item2 == end.Item2)

            {

                return true;

            }



            // Explore neighbors

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



    // Reconstructs the path from the end coordinate to the start coordinate using the 'cameFrom' map.

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
