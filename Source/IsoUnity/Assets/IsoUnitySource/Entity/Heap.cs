using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Heap<T>
	where T : IComparable<T>
{
	
	public Heap(int tam)
	{
		inicia(tam);            
	}
	
	public struct Par
	{
		public int elem;
		public T prioridad;
	}
	
	
	public void push(int e, T p) {
		if (posiciones[e] != 0) throw new Exception("Elemento ya encolado");
		else if (numElems == posiciones.GetLength(0)) throw new Exception("Cola llena");
		else {
			numElems++;
			v[numElems].elem = e; v[numElems].prioridad = p;
			posiciones[e] = numElems;
			flotar(numElems);
		}
	}
	
	public void modify(int e, T p) {
		int i = posiciones[e];
		if (i == 0) // el elemento e se inserta por primera vez
			push(e, p);
		else {
			v[i].prioridad = p;
			if (i != 1 && v[i].prioridad.CompareTo(v[i/2].prioridad) < 0)
				flotar(i);
			else // puede hacer falta hundir a e
				hundir(i);
		}
	}
	
	public T getPrio(int e) {
		return v[posiciones[e]].prioridad;
	}
	
	public bool contains(int e){
		return posiciones[e] != 0;
	}
	
	public bool isEmpty() {
		return (numElems == 0);
	}
	
	public Par top() {
		if (numElems == 0) throw new Exception("Cola vacía: No se puede consultar el primero");
		else return v[1];
	}
	
	public void pop() {
		if (numElems == 0) throw new Exception("Cola vacía: Imposible eliminar primero");
		else {
			posiciones[v[1].elem] = 0; // para indicar que no esta
			v[1] = v[numElems];
			posiciones[v[1].elem] = 1;
			numElems--;
			hundir(1);
		}
	}
	
	
	
	private void inicia(int tam)
	{
		v = new Par[tam+1];
		posiciones = new int[tam + 1];
		numElems = 0;
	}
	
	private void flotar(int n)
	{
		int i = n;
		Par parmov = v[i];
		while ((i != 1) && ( parmov.prioridad.CompareTo(v[i / 2].prioridad) < 0 ))
		{
			v[i] = v[i / 2]; posiciones[v[i].elem] = i;
			i = i / 2;
		}
		v[i] = parmov; posiciones[v[i].elem] = i;
	}
	
	private void hundir(int n)
	{
		int i = n;
		Par parmov = v[i];
		int m = 2*i; // hijo izquierdo de i, si existe
		while (m <= numElems)  {
			// cambiar al hijo derecho de i si existe y va antes que el izquierdo
			if ((m < numElems) && ( (v[m + 1].prioridad.CompareTo(v[m].prioridad) < 0)))
				m = m + 1;
			// flotar el hijo m si va antes que el elemento hundiendose
			if (v[m].prioridad.CompareTo(parmov.prioridad) < 0) {
				v[i] = v[m]; posiciones[v[i].elem] = i;
				i = m; m = 2*i;
			}
			else break;
		}
		v[i] = parmov; posiciones[v[i].elem] = i;
	}
	
	/** Puntero al array que contiene los datos (pares < elem, prio >). */
	private Par[] v;
	
	/** Puntero al array que contiene las posiciones en v de los elementos. */
	private int[] posiciones;
	
	/** Numero de elementos reales guardados. */
	private int numElems;
}