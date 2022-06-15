using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace AlgorytmDijkstry
{
    internal class Program
    {
        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("\tImplementacja algorytmu Dijkstry\n");
            Console.Write("Zacznijmy od utworzenia grafu.\n" +
                "\nProgram najpierw zapyta o liczbę wszystkich wierzchołków w grafie oraz o ich nazwy.\n" +
                "Następnie program dla każdego wierzchołka zapyta o liczbę sąsiadów wierzchołka," +
                "\no ich nazwy i odległości dzielące te wierzchołki.\n" +
                "\nPo utworzeniu grafu wyświetlona zostanie lista najkrótszych ścieżek.\n" +
                "\nUWAGA! Pierwszy podany wierzchołek traktowany będzie jako źródło i to od niego wyznaczane będą ścieżki!\n" +
                "\nWciśnij dowolny klawisz, aby kontynuować: ");
            Console.ReadKey();

            Console.WriteLine("\n\n----------------------------------------------------------------------");
            int allVertices = GetNumberInRange("\nPodaj liczbę wszystkich wierzchołków w grafie", 3, 50);

            List<Vertex> vertices = new();
            GetVertices(vertices, allVertices);
            CreateGraph(vertices);
            CalculateShortestPaths(vertices[0]);
            PrintPaths(vertices);
            Console.Write("\nWciśnij dowolny klawisz, aby zakończyć program: ");
            Console.ReadKey();
        }
        private static void GetVertices(List<Vertex> vertices, int allVertices)
        {
            Console.WriteLine();
            for (int i = 0; i < allVertices; i++)
            {
                if (i == 0)
                {
                    Console.Write("Podaj nazwę wierzchołka źródłowego: ");
                }
                else
                {
                    Console.Write("Podaj nazwę {0} wierzchołka: ", i + 1);
                }
                vertices.Add(new Vertex(Console.ReadLine()));
            }
            Console.WriteLine();
        }
        private static void CreateGraph(List<Vertex> vertices)
        {
            List<string> verticesNames = new();
            foreach (Vertex v in vertices)
            {
                verticesNames.Add(v.Name);
            }
            foreach (Vertex v in vertices)
            {
                Vertex neighbour;
                string neighbourName;
                Console.WriteLine("----------------------------------------------------------------------");
                int neighbours = GetNumberInRange("Podaj liczbę sąsiadujących wierzchołków z wierzchołkiem "
                    + v.Name, 1, vertices.Count - 1);
                for (int j = 0; j < neighbours; j++)
                {
                    bool isVertexFound = false;
                    do
                    {
                        Console.Write("Podaj nazwę sąsiadującego wierzchołka ({0} z {1}): ", j + 1, neighbours);
                        neighbourName = Console.ReadLine();
                        if (!verticesNames.Contains(neighbourName))
                        {
                            Console.WriteLine("Nie znaleziono wierzchołka o podanej nazwie.");
                        }
                        else if (neighbourName == v.Name)
                        {
                            Console.WriteLine("Wierzchołek nie może sąsiadować sam ze sobą.");
                        }
                        else
                        {
                            isVertexFound = true;
                        }
                    } while (!isVertexFound);
                    neighbour = vertices.Find(x => x.Name == neighbourName);
                    int weight;
                    if (neighbour.GetAdjacentVertices.ContainsKey(v))
                    {
                        weight = neighbour.GetAdjacentVertices.GetValueOrDefault(v);
                        Console.WriteLine("Te wierzchołki są już powiązane ze sobą. " +
                            "Dodano powiązanie zwrotne o tej samej odległości ({0})", weight);
                    }
                    else
                    {
                        weight = GetNumberInRange("Podaj dystans od " + v.Name + " do " + neighbourName, 1, 100000);
                    }
                    v.AddAdjacentVertex(neighbour, weight);
                    Console.WriteLine();
                }
            }
        }
        private static void DisplayAdjacencyMatrix(List<Vertex> vertices)
        {
            int dimension = vertices.Count + 1;
            string[,] matrix = new string[dimension, dimension];
            matrix[0, 0] = "\\";
            for (int i = 1; i < dimension; i++)
            {
                matrix[i, 0] = vertices[i - 1].Name;
                matrix[0, i] = vertices[i - 1].Name;
            }
            Vertex checkedVertex;
            for (int i = 1; i < dimension; i++)
            {
                checkedVertex = vertices[i - 1];
                for (int j = 1; j < dimension; j++)
                {
                    if (checkedVertex.GetAdjacentVertices.ContainsKey(vertices[j - 1]))
                    {
                        matrix[i, j] = checkedVertex.GetAdjacentVertices.GetValueOrDefault(vertices[j - 1]).ToString();
                    }
                    else
                    {
                        matrix[i, j] = "0";
                    }
                }
            }
            Console.WriteLine("\n----------------------------------------------------------------------");
            Console.WriteLine("Tablica sąsiedztwa wierzchołków\n");
            for (int i = 0; i < dimension; i++)
            {
                Console.Write(" ");
                for (int j = 0; j < dimension; j++)
                {
                    Console.Write(string.Format("{0} ", matrix[i, j]));
                }
                Console.Write("\n");
            }
            Console.Write("\nWciśnij dowolny klawisz, aby wyświetlić najkrótsze ścieżki: ");
            Console.ReadKey();
        }
        private static void CalculateShortestPaths(Vertex source)
        {
            source.SetDistance(0);
            HashSet<Vertex> settledVertices = new();
            PriorityQueue<Vertex, int> unsettledVertices = new();

            unsettledVertices.Enqueue(source, source.Distance);
            while (unsettledVertices.Count > 0)
            {
                Vertex currentVertex = unsettledVertices.Dequeue();
                foreach (KeyValuePair<Vertex, int> adjacentVertex in currentVertex.GetAdjacentVertices)
                {
                    if (!settledVertices.Contains(adjacentVertex.Key))
                    {
                        EvaluateDistanceAndPath(adjacentVertex.Key, adjacentVertex.Value, currentVertex);
                        unsettledVertices.Enqueue(adjacentVertex.Key, adjacentVertex.Value);
                    }
                    settledVertices.Add(currentVertex);
                }
            }
        }
        private static void EvaluateDistanceAndPath(Vertex adjacentVertex, int distance, Vertex sourceVertex)
        {
            int newDistance = sourceVertex.Distance + distance;
            if (newDistance < adjacentVertex.Distance)
            {
                adjacentVertex.SetDistance(newDistance);
                adjacentVertex.AddToShortestPath(sourceVertex);
            }
        }
        private static void PrintPaths(List<Vertex> vertices)
        {
            Console.WriteLine("\n----------------------------------------------------------------------"+
                "\nLista najkrótszych ścieżek od wierzchołka {0} do wszystkich innych wierzchołków:\n", vertices[0].Name);
            string fullPath;
            List<string> path = new();
            foreach (Vertex vertex in vertices)
            {
                path.Clear();
                fullPath = "";
                Vertex currentVertex = vertex;
                while (currentVertex.ShortestPath.Count > 0)
                {
                    path.Add(currentVertex.ShortestPath.First.Value.Name);
                    currentVertex = currentVertex.ShortestPath.First.Value;
                }
                path.Reverse();
                if (path.Count > 0)
                {
                    path.Add(vertex.Name);
                    string[] arr = path.ToArray();
                    fullPath = arr[0];
                    for (int i = 1; i < arr.Length; i++)
                    {
                        fullPath = string.Format("{0} -> {1}", fullPath, arr[i]);
                    }
                    Console.WriteLine("{0} : {1} ", fullPath, vertex.Distance);
                }
            }
        }
        private static int GetNumberInRange(String message, int lowBound, int upBound)
        {
            int temp;
            do
            {
                Console.Write("{0} (zakres {1}-{2}): ", message, lowBound, upBound);
                try
                {
                    int.TryParse(Console.ReadLine(), out temp);
                }
                catch
                {
                    temp = lowBound - 1;
                }

            } while (temp < lowBound || temp > upBound);
            return temp;
        }
    }
    internal class Vertex : IComparable<Vertex>
    {
        private string name;
        private Int32 distance;
        private LinkedList<Vertex> shortestPath;
        private Dictionary<Vertex, Int32> adjacentVertices;

        public Vertex()
        {
            distance = Int32.MaxValue;
        }
        public Vertex(string name)
        {
            this.name = name;
            distance = Int32.MaxValue;
            shortestPath = new LinkedList<Vertex>();
            adjacentVertices = new Dictionary<Vertex, Int32>();
        }

        public string Name
        {
            get { return name; }
        }

        public Dictionary<Vertex, Int32> GetAdjacentVertices
        {
            get { return adjacentVertices; }
        }

        public void SetDistance(int distance)
        {
            this.distance = distance;
        }

        public Int32 Distance
        {
            get { return distance; }
        }

        public void AddAdjacentVertex(Vertex vertex, int weight)
        {
            adjacentVertices.Add(vertex, weight);
        }

        public void AddToShortestPath(Vertex vertex)
        {
            shortestPath.AddFirst(vertex);
        }

        public LinkedList<Vertex> ShortestPath
        {
            get { return shortestPath; }
        }

        //override
        public int CompareTo(Vertex other)
        {
            return this.distance.CompareTo(other.distance);
        }
    }
}
