using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class Map : MonoBehaviour
{
	/*****************
	 * Attribute zone
	 * ***************/

	[SerializeField]
	private List<Cell> celdas;

	[SerializeField]
	private float cellSize;

	[SerializeField]
	private Dictionary<Cell,Cell[]> vecinas;

	private Transform m_transform;

	private GameObject ghost;

	private int ghostType; //0 = ninguno; 1 = cell; 2 = Decoration;
	
	/*********************
	 * Constructor Zone
	 * ******************/

	public Map(){
		celdas = new List<Cell>();
		vecinas = new Dictionary<Cell,Cell[]>();
		cellSize = 1;
	}

	/***********************
	 * Getter Zone
	 * ********************/
	

	public void setCellSize(float cellSize){
		this.cellSize = cellSize;

		foreach(Cell c in celdas){
			Vector2 coords = getCoords(c.gameObject);

			c.transform.position.Set(coords.x * cellSize,c.transform.position.y, coords.y * cellSize);
			c.setCellWidth(cellSize);
		}
	}

	/* ************ *
	 * Utility Zone *
	 * ************ */

	private void checkTransform(){
		if(m_transform == null)
			m_transform = this.transform;
	}
	
	public Vector2 getCoords(GameObject go){return getCoords(go.transform.localPosition);}
	public Vector2 getCoords(Vector3 localPosition){
		Vector3 position = localPosition;
		float xTile = Mathf.Floor(position.x / cellSize);
		float zTile = Mathf.Floor(position.z / cellSize);
		return new Vector2(xTile,zTile);
	}

	/* ************** *
	 *   Editor Zone  *
	 * ************** */
	
	public Vector3 getMousePositionOverMap(float height){
		checkTransform();
		
		// Saco el raton y proyecto un rayo con el
		Ray rayo = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
		// Calculo las veces que tengo que multiplicar el vector para cortar el plano
		float a = ((m_transform.position.y + height) - rayo.origin.y) / rayo.direction.y;
		// Calculo la posicion absoluta del punto de corte
		Vector3 puntoDeCorte = rayo.origin + a*rayo.direction;
		
		// Lo paso a cordenadas relativas para calcular las coordenadas 
		// con respecto a este mapa y lo vuelvo a cordenadas absolutas
		puntoDeCorte = m_transform.InverseTransformPoint(puntoDeCorte);
		Vector2 coords = getCoords(puntoDeCorte);
		return m_transform.TransformPoint((coords.x+0.5f) * cellSize, 0, (coords.y+0.5f) * cellSize) + new Vector3(0,height,0);
	}
	
	public void ghostCell(Vector3 position, float intensity){
		checkTransform();
		
		if(ghost == null || ghostType != 1){
			removeGhost();
			ghostType = 1;
			ghost = GameObject.Instantiate(IsoSettingsManager.getInstance().getIsoSettings().defaultCellPrefab) as GameObject;
			ghost.name = "Ghost";
			ghost.GetComponent<Cell>().Map = this;
			ghost.renderer.material.color = new Color(ghost.renderer.material.color.r,ghost.renderer.material.color.g,ghost.renderer.material.color.b,intensity);
			ghost.renderer.material.shader = Shader.Find("Transparent/Diffuse");
		}
		
		// Getting the localPosition
		position = m_transform.InverseTransformPoint(position);
		
		Cell cell = ghost.GetComponent<Cell>();
		cell.setCellWidth(cellSize);	

		//La refresco
		Vector2 coords = getCoords(position);
		cell.Height = position.y;
		cell.transform.localPosition = new Vector3((coords.x+0.5f) * cellSize, 0, (coords.y+0.5f) * cellSize);
	}

	public void ghostDecoration(Vector3 position, float intensity, IsoDecoration dec, Boolean centered){
		checkTransform();
		
		if(ghost == null || ghostType != 2){
			removeGhost();
			ghostType = 2;
			ghost = GameObject.Instantiate(IsoSettingsManager.getInstance().getIsoSettings().defaultDecorationPrefab) as GameObject;
			ghost.name = "GhostDer";
			ghost.transform.parent = transform;
			ghost.renderer.material.color = new Color(ghost.renderer.material.color.r,ghost.renderer.material.color.g,ghost.renderer.material.color.b,intensity);
			ghost.renderer.material.shader = Shader.Find("Transparent/Diffuse");
		}
		
		// Getting the localPosition
		position = m_transform.InverseTransformPoint(position);
		
		Decoration der = ghost.GetComponent<Decoration>();
		der.IsoDec = dec;
		
		//La refresco
		if (centered) {
			Vector2 coords = getCoords (position);
			der.setPosition (new Vector3 ((coords.x + 0.5f) * cellSize, position.y, (coords.y + 0.5f) * cellSize));
		} else {
			der.setPosition (position);
		}

		//La refresco

		der.refresh ();
	}

	public void removeGhost(){
		if (ghost != null) {
			GameObject.DestroyImmediate (ghost);
			ghostType = 0;
		}
	}

	/* ********** *
	 * CRUD Zone  *
	 * ********** */

	public GameObject addCell(Vector3 position){
		checkTransform();

		// Creating the gameObject
		GameObject go = GameObject.Instantiate(IsoSettingsManager.getInstance().getIsoSettings().defaultCellPrefab) as GameObject;

		// Getting the localPosition
		position = m_transform.InverseTransformPoint(position);

		// Setting base properties
		Cell cell = go.GetComponent<Cell>();
		cell.Map = this;
		cell.transform.localPosition = position;
		cell.setCellWidth(cellSize);	

		//Añado la celda al conjunto de celdas
		vecinas.Add(cell, new Cell[4]);
		this.celdas.Add(cell);

		//La refresco
		updateCell(cell, true);

		return go;
	}

	public void updateCell(Cell cell, bool finalPos){
		
		// Actualizo la posicion con respecto a la grilla
		Vector2 coords = getCoords(cell.gameObject);
		cell.transform.localPosition = new Vector3((coords.x+0.5f) * cellSize, cell.transform.localPosition.y, (coords.y+0.5f) * cellSize);
		
		// Si esta posicion sera la posicion final
		if(finalPos){
			// Borro las referencias de mis vecinos hacia mi
			removeReferences(cell);
			// Busco si he ocupado el lugar de alguna celda
			for(int i = 0; i<celdas.Count; i++){
				Cell other = celdas[i];
				Vector2 goCoords = getCoords (other.gameObject);
				//Si hay otra celda en mi posicion
				if(other != cell && coords.x == goCoords.x && coords.y == goCoords.y){
					// Quito la celda
					vecinas.Remove(other);
					celdas.RemoveAt(i);
					SceneView.DestroyImmediate(other.gameObject);
					break;
				}
			}
			// Busco mis nuevos vecinos
			searchNeighborhood(cell);
			// Les informo de mi posicion
			updateReferences(cell, vecinas[cell]);	
		}
	}


	public void removeCell(Cell cell){
		this.removeReferences(cell);
		this.celdas.Remove(cell);
	}


	/* ***************** *
	 * Neighborhood Zone *
	 * ***************** */

	private void searchNeighborhood(Cell cell){
		Cell[] v = new Cell[4];

		Vector2 current = getCoords(cell.gameObject);

		foreach(Cell c in celdas){
			Vector2 other = getCoords(c.gameObject);
			if(other.x == current.x){
				if(other.y == current.y + cellSize)
					v[0] = c;
				else if(other.y == current.y - cellSize)
					v[2] = c;
			}else if(other.y == current.y){
				if(other.x == current.x + cellSize)
					v[1] = c;
				else if(other.x == current.x - cellSize)
					v[3] = c;
			}
		}

		if(!vecinas.ContainsKey(cell))
			vecinas.Add(cell, v);
		else
			vecinas[cell] = v;
	}

	private void updateReferences(Cell cell, Cell[] cells){
		for(int i = 0; i<cells.Length; i++)
			if(cells[i]!=null)
				vecinas[cells[i]][(i+2)%4] = cell;

	}

	private void removeReferences(Cell cell)
	{
		if(vecinas.ContainsKey(cell))
		{
			Cell[] bcVecinas = vecinas[cell];
			for(int i = 0; i< bcVecinas.Length; i++)
				if(bcVecinas[i] != null)
					if(vecinas.ContainsKey(bcVecinas[i]))
					   vecinas[bcVecinas[i]][(i+2) % 4] = null;
			vecinas.Remove(cell);
		}
	}

	public Cell[] getNeightbours(Cell c){
		if(!vecinas.ContainsKey(c))
			searchNeighborhood(c);
		return vecinas[c];
	}


	/***********
	 * GO! Zone
	 * **********/

	public void setVisible(bool visible){
		foreach(Cell cell in this.transform.GetComponentsInChildren<Cell>())
			cell.renderer.enabled = visible;
	}

	public void broadcastEvent(GameEvent ge){
		foreach(Cell c in this.transform.GetComponentsInChildren<Cell>()){
			foreach(Entity e in c.getEntities()){
				e.eventHappened(ge);
			}
		}
	}

	public void tick(){
		foreach(Cell c in this.transform.GetComponentsInChildren<Cell>())
			c.tick();
	}
	
	public void Start(){

	}

	public void Update(){

	}

}

