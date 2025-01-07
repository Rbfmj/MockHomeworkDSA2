using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MockHomeworkDSA2
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            string dotGraph = "graph {\n" +
                              "    A -- B [label = \"5\"];\n" +
                              "    A -- C [label = \"3\"];\n" +
                              "    B -- C [label = \"2\"];\n" +
                              "    B -- D [label = \"4\"];\n" +
                              "    C -- D [label = \"11\"];\n" +
                              "    C -- E [label = \"12\"];\n" +
                              "    D -- E [label = \"1\"];\n" +
                              "}";

            var graph = DotParser.Parse(dotGraph);

            string source = "B";

            Console.WriteLine("Task 1: ABV Implementation\n");
            DijkstraABV.Dijkstra(graph, source);

            Console.WriteLine("\nTask 2: MinHeap Implementation\n");
            DijkstraMinHeap.Dijkstra(graph, source);
        }
    }

    public static class DotParser
    {
        public static Dictionary<string, List<(string, int)>> Parse(string dot)
        {
            var graph = new Dictionary<string, List<(string, int)>>();
            var edgePattern = new Regex(@"(\w+)\s*--\s*(\w+)\s*\[label\s*=\s*""(\d+)""\]");


            foreach (Match match in edgePattern.Matches(dot))
            {
                string node1 = match.Groups[1].Value;
                string node2 = match.Groups[2].Value;
                int weight = int.Parse(match.Groups[3].Value);

                if (!graph.ContainsKey(node1)) graph[node1] = new List<(string, int)>();
                if (!graph.ContainsKey(node2)) graph[node2] = new List<(string, int)>();

                graph[node1].Add((node2, weight));
                graph[node2].Add((node1, weight));
            }

            return graph;
        }
    }

    public class DijkstraABV
    {
        public static void Dijkstra(Dictionary<string, List<(string, int)>> graph, string source)
        {
            var dist = new Dictionary<string, int>();
            var prev = new Dictionary<string, string>();

            foreach (var vertex in graph.Keys)
            {
                dist[vertex] = int.MaxValue;
                prev[vertex] = null;
            }

            dist[source] = 0;
            var queue = new List<string>(graph.Keys);

            while (queue.Count > 0)
            {
                string u = null;
                int minDist = int.MaxValue;

                foreach (var vertex in queue)
                {
                    if (dist[vertex] < minDist)
                    {
                        minDist = dist[vertex];
                        u = vertex;
                    }
                }

                if (u == null) break;

                queue.Remove(u);

                foreach (var (neighbor, weight) in graph[u])
                {
                    int alt = dist[u] + weight;
                    if (alt < dist[neighbor])
                    {
                        dist[neighbor] = alt;
                        prev[neighbor] = u;
                    }
                }
            }

            Console.WriteLine("Vertex\tDistance from Source");
            foreach (var vertex in graph.Keys)
            {
                Console.WriteLine($"{vertex}\t{dist[vertex]}");
            }
        }
    }

    public class MinHeap
    {
        private List<(string, int)> _elements = new List<(string, int)>();

        public int Count => _elements.Count;

        public void Add(string vertex, int priority)
        {
            _elements.Add((vertex, priority));
            HeapifyUp(_elements.Count - 1);
        }

        public (string, int) ExtractMin()
        {
            if (_elements.Count == 0)
                throw new InvalidOperationException("Heap is empty.");

            var min = _elements[0];
            _elements[0] = _elements[_elements.Count - 1];
            _elements.RemoveAt(_elements.Count - 1);

            HeapifyDown(0);
            return min;
        }

        public void Update(string vertex, int newPriority)
        {
            int index = _elements.FindIndex(e => e.Item1 == vertex);
            if (index != -1)
            {
                _elements[index] = (vertex, newPriority);
                HeapifyUp(index);
                HeapifyDown(index);
            }
        }

        public bool Contains(string vertex)
        {
            return _elements.Any(e => e.Item1 == vertex);
        }

        private void HeapifyUp(int index)
        {
            while (index > 0)
            {
                int parent = (index - 1) / 2;
                if (_elements[index].Item2 >= _elements[parent].Item2)
                    break;

                Swap(index, parent);
                index = parent;
            }
        }

        private void HeapifyDown(int index)
        {
            while (true)
            {
                int left = 2 * index + 1;
                int right = 2 * index + 2;
                int smallest = index;

                if (left < _elements.Count && _elements[left].Item2 < _elements[smallest].Item2)
                    smallest = left;

                if (right < _elements.Count && _elements[right].Item2 < _elements[smallest].Item2)
                    smallest = right;

                if (smallest == index)
                    break;

                Swap(index, smallest);
                index = smallest;
            }
        }

        private void Swap(int i, int j)
        {
            var temp = _elements[i];
            _elements[i] = _elements[j];
            _elements[j] = temp;
        }
    }

    public class DijkstraMinHeap
    {
        public static void Dijkstra(Dictionary<string, List<(string, int)>> graph, string source)
        {
            var dist = new Dictionary<string, int>();
            var prev = new Dictionary<string, string>();
            var heap = new MinHeap();

            foreach (var vertex in graph.Keys)
            {
                dist[vertex] = int.MaxValue;
                prev[vertex] = null;
                heap.Add(vertex, int.MaxValue);
            }

            dist[source] = 0;
            heap.Update(source, 0);

            while (heap.Count > 0)
            {
                var (u, _) = heap.ExtractMin();

                foreach (var (neighbor, weight) in graph[u])
                {
                    int alt = dist[u] + weight;
                    if (alt < dist[neighbor])
                    {
                        dist[neighbor] = alt;
                        prev[neighbor] = u;
                        heap.Update(neighbor, alt);
                    }
                }
            }

            Console.WriteLine("Vertex\tDistance from Source");
            foreach (var vertex in graph.Keys)
            {
                Console.WriteLine($"{vertex}\t{dist[vertex]}");
            }
        }
    }
}




