using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiger.AST
{
    public class Graph
    {
        public List<int>[] adjacents { get; private set; }
        public List<List<int>> cycle { get; private set; }
        public List<List<TypeDecNode>> circularity { get; private set; }
        public List<TypeDecNode> pending { get; private set; }

        public Graph(List<TypeDecNode> declarations)
        {
            adjacents = new List<int>[declarations.Count];
            Dictionary<string, int> mapper = new Dictionary<string, int>();
            cycle = new List<List<int>>();
            circularity = new List<List<TypeDecNode>>();

            for (int i = 0; i < declarations.Count; i++)
            {
                adjacents[i] = new List<int>();
                mapper[declarations[i].id.name] = i;//asociar a cada nombre su indice en la lista de adyacencia
            }

            for (int i = 0; i < declarations.Count; i++)
            {//no tener en cuenta los elementos que no se han declarado en este bloque
                if (declarations[i] is AliasDecNode)
                {
                    var alias = declarations[i] as AliasDecNode;
                    if (alias.aliasOf.name != alias.id.name && mapper.ContainsKey(alias.aliasOf.name))
                        adjacents[i].Add(mapper[alias.aliasOf.name]);
                }

                if (declarations[i] is ArrayDecNode)
                {
                    var array = declarations[i] as ArrayDecNode;
                    if (array.itemsType.name != array.id.name && mapper.ContainsKey(array.itemsType.name))
                        adjacents[i].Add(mapper[array.itemsType.name]);
                }
            }

            DFS();
            bool[] inCycle = new bool[declarations.Count];
            if (cycle.Count > 0)
            {
                for (int i = 0; i < cycle.Count; i++)
                {
                    var current = new List<TypeDecNode>();
                    for (int j = 0; j < cycle[i].Count; j++)
                    {
                        current.Add(declarations[cycle[i][j]]);
                        inCycle[cycle[i][j]] = true;
                    }
                    circularity.Add(current);
                }
            }

            pending = new List<TypeDecNode>();
            for (int i = 0; i < declarations.Count; i++)
            {
                if (!inCycle[i])
                    pending.Add(declarations[i]);
            }

        }

        private List<int> DFS_top_sort()
        {
            int[] colors = new int[adjacents.Length];
            List<int> top_sort = new List<int>();
            int[] phi = new int[adjacents.Length];
            for (int i = 0; i < phi.Length; i++)
            {
                phi[i] = -1;
            }

            for (int i = 0; i < colors.Length; i++)
            {
                if (colors[i] == 0)
                    DFS_visit(i, colors, phi, top_sort);
            }
            return top_sort;
        }

        private void DFS_visit(int u, int[] colors, int[] phi, List<int> top_sort)
        {
            colors[u] = 1;
            for (int i = 0; i < adjacents[u].Count; i++)
            {
                if (colors[adjacents[u][i]] == 0)
                {
                    phi[adjacents[u][i]] = u;
                    DFS_visit(adjacents[u][i], colors, phi, top_sort);
                }
            }
            colors[u] = 2;
            top_sort.Add(u);
        }
        public void DFS()
        {
            int[] colors = new int[adjacents.Length];
            int[] d = new int[adjacents.Length];
            int[] phi = new int[adjacents.Length];
            int time = 0;

            for (int i = 0; i < phi.Length; i++)
                phi[i] = -1;

            for (int i = 0; i < colors.Length; i++)
                if (colors[i] == 0)
                    DFS_visit(i, colors, d, phi, time);
        }

        private void DFS_visit(int u, int[] colors, int[] d, int[] phi, int time)
        {
            time++;
            d[u] = time;
            colors[u] = 1;
            for (int i = 0; i < adjacents[u].Count; i++)
            {
                if (colors[adjacents[u][i]] == 0)
                {
                    phi[adjacents[u][i]] = u;
                    DFS_visit(adjacents[u][i], colors, d, phi, time);
                }
                else if (colors[adjacents[u][i]] == 1) //arista de retroceso
                    FindCycle(u, adjacents[u][i], phi);
            }
            colors[u] = 2;
        }
        
        private void FindCycle(int u, int p, int[] phi)
        {
            List<int> _cycle = new List<int>();
            _cycle.Add(u);
            int current = u;
            while (phi[current] != p)
            {
                _cycle.Add(phi[current]);
                current = phi[current];
            }
            _cycle.Add(p);
            cycle.Add(_cycle);
        }
    }
}
