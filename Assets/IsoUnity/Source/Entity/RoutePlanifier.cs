using UnityEngine;
using System.Collections.Generic;
using IsoUnity;
using C5;
using System;

namespace IsoUnity.Entities
{
    public class RoutePlanifier
    {

        private static Dictionary<Mover, Stack<Cell>> routes = new Dictionary<Mover, Stack<Cell>>();

        public static bool planifyRoute(Mover mover, Cell destination)
        {
            return planifyRoute(mover, destination, 0);
        }

        public static bool hasRoute(Mover mover)
        {
            return routes.ContainsKey(mover);
        }

        public static bool planifyRoute(Mover mover, Cell destination, int distance)
        {
            Stack<Cell> ruta = null;
            if (routes.ContainsKey(mover))
            {
                ruta = routes[mover];
                var cellList = routes[mover].ToArray();
                if (cellList.Length == 0 || cellList[cellList.Length - 1] != destination)
                {
                    //return false;
                    ruta = calculateRoute(routes[mover].Peek(), destination, mover, distance);
                    //Stack<Cell> ruta = new Stack<Cell>();
                    //ruta.Push (destination);

                    if (ruta != null)
                    {
                        //ruta.Push(routes[mover].Peek());
                        routes[mover] = ruta;
                    }
                }
            }
            else
            {
                /*Stack<Cell> ruta = new Stack<Cell>();
                ruta.Push (destination);*/
                ruta = calculateRoute(mover.Entity.Position, destination, mover, distance);

                if (ruta != null)
                {
                    //ruta.Push(mover.Entity.Position);
                    //ruta.Pop(); //Quito en la que estoy
                    routes.Add(mover, ruta);
                }
            }

            return ruta != null;
        }

        public static Cell next(Mover mover)
        {
            Cell nextCell = null;

            if (routes.ContainsKey(mover))
            {
                if (routes[mover].Count == 0)
                    routes.Remove(mover);
                else
                {
                    if (routes[mover].Peek() != mover.Entity.Position)
                        routes.Remove(mover);
                    else
                    {
                        routes[mover].Pop();
                        if (routes[mover].Count == 0)
                            routes.Remove(mover);
                        else
                            nextCell = routes[mover].Peek();
                    }

                }
            }
            return nextCell;
        }

        public static void cancelRoute(Mover mover)
        {
            if(routes.ContainsKey(mover)){
                routes.Remove(mover);
            }
        }

        private static void reconstruyeCamino(Stack<Cell> route, Cell celda, Dictionary<Cell, Cell> anterior)
        {
            route.Push(celda);
            Cell posAnterior = anterior[celda];
            if (posAnterior != null)
                reconstruyeCamino(route, posAnterior, anterior);
        }

        private static List<Cell> GetSurroundCellsAtRadius(Cell to, int distance)
        {
            List<Cell> cells = new List<Cell>();
            GetSurroundCellsAtRadius(to, distance, cells);
            return cells;
        }

        private static void GetSurroundCellsAtRadius(Cell to, int distance, List<Cell> cells)
        {
            if (distance < 0)
                return;

            if (!cells.Contains(to))
            {
                cells.Add(to);
                foreach (Cell c in to.Map.getNeightbours(to))
                    if (c != null)
                        GetSurroundCellsAtRadius(c, distance - 1, cells);
            }
        }

        private class CellDistance : System.IComparable
        {
            public Cell cell;
            public float distance;

            public CellDistance(Cell cell, float distance)
            {
                this.cell = cell;
                this.distance = distance;
            }

            public int CompareTo(object obj)
            {
                if(obj is CellDistance)
                {
                    var cd = obj as CellDistance;
                    return distance.CompareTo(cd.distance);
                }
                return 0;
            }
        }

        private static Stack<Cell> calculateRoute(Cell from, Cell to, Mover mover, int distance)
        {

            Dictionary<Cell, int> cellToPos = new Dictionary<Cell, int>();

            IPriorityQueueHandle<CellDistance> handle = null;
            IPriorityQueue<CellDistance> abierta = new IntervalHeap<CellDistance>();

            Dictionary<Cell, bool> cerrada = new Dictionary<Cell, bool>();
            Dictionary<Cell, float> f = new Dictionary<Cell, float>();
            Dictionary<Cell, float> g = new Dictionary<Cell, float>();
            Dictionary<Cell, Cell> anterior = new Dictionary<Cell, Cell>();
            Dictionary<Cell, IPriorityQueueHandle<CellDistance>> handles = new Dictionary<Cell, IPriorityQueueHandle<CellDistance>>();

            Cell posInicial = from;

            g[posInicial] = 0;
            f[posInicial]= estimaMeta(from, to);
            anterior[posInicial] = null;
            abierta.Add(ref handle, new CellDistance(posInicial, f[posInicial]));
            handles.Add(posInicial, handle);
            
            List<Cell> ends = GetSurroundCellsAtRadius(to, distance);

            while (!abierta.IsEmpty)
            {
                CellDistance candidata = abierta.DeleteMin();
                Cell celdaCandidata = candidata.cell;
                handles.Remove(celdaCandidata);

                if (ends.Contains(celdaCandidata))
                {
                    Stack<Cell> ruta = new Stack<Cell>();
                    reconstruyeCamino(ruta, celdaCandidata, anterior);
                    return ruta;
                }

                Cell[] accesibles = getCeldasAccesibles(celdaCandidata, mover);
                cerrada[celdaCandidata] = true;
                foreach (Cell accesible in accesibles)
                {
                    float posibleG = g[celdaCandidata] + estimaAvance(celdaCandidata, accesible);
                    float posibleF = posibleG + estimaMeta(accesible, to);

                    if (cerrada.ContainsKey(accesible) && cerrada[accesible] && posibleF >= f[accesible])
                        continue;

                    if (!handles.ContainsKey(accesible) || posibleF < f[accesible])
                    {
                        anterior[accesible] = celdaCandidata;
                        g[accesible] = posibleG;
                        f[accesible] = posibleF;

                        // Modifica inserta si no esta dentro
                        var cd = new CellDistance(accesible, f[accesible]);
                        if (handles.ContainsKey(accesible))
                            abierta[handles[accesible]] = cd;
                        else
                        {
                            handle = null;
                            abierta.Add(ref handle, cd);
                            handles.Add(accesible, handle);
                        }
                    }
                }
            }

            return null;
        }

        private static Cell[] getCeldasAccesibles(Cell celda, Mover mover)
        {

            Cell[] vecinas = celda.Map.getNeightbours(celda);
            List<Cell> accesibles = new List<Cell>();
            foreach (Cell c in vecinas)
            {
                if (c != null)
                {
                    if (mover.CanMoveTo(celda, c))
                        accesibles.Add(c);
                }
            }

            return accesibles.ToArray() as Cell[];
        }



        private static float estimaAvance(Cell celdaCandidata, Cell accesible)
        {
            return Vector3.Distance(celdaCandidata.transform.position, accesible.transform.position);
        }

        private static float estimaMeta(Cell accesible, Cell to)
        {
            return Vector3.Distance(accesible.transform.position, to.transform.position);
        }
    }
}