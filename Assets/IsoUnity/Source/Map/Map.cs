using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Map : MonoBehaviour, ISerializationCallbackReceiver
{
    [Serializable]
    private class Neighbors
    {
        public Neighbors(Cell[] cells)
        {
            Cells = cells;
        }

        public Cell c0;
        public Cell c1;
        public Cell c2;
        public Cell c3;
        public Cell[] Cells
        {
            get { return new Cell[] { c0, c1, c2, c3 }; }
            set { c0 = value[0]; c1 = value[1]; c2 = value[2]; c3 = value[3]; }
        }
    }


	/*****************
	 * Attribute zone
	 * ***************/
    [SerializeField]
    private List<Vector2> cellsKeys;
    [SerializeField]
    private List<Cell> cellsValues;

    private Dictionary<Vector2, Cell> cellMap;

	[SerializeField]
	private float cellSize;

    [SerializeField]
    private List<Cell> vecinasKeys;
    [SerializeField]
    private List<Neighbors> vecinasValues;

	private Dictionary<Cell,Cell[]> vecinas;

	private Transform m_transform;

	private GameObject ghost;

	private int ghostType; //0 = ninguno; 1 = cell; 2 = Decoration;
	
	/*********************
	 * Constructor Zone
	 * ******************/

    void Awake(){

        
        int myCellCount = this.Cells.Length;

        if(cellMap == null)
            cellMap = new Dictionary<Vector2, Cell>();
        if(vecinas == null) 
            vecinas = new Dictionary<Cell, Cell[]>();


        if (cellMap.Count != myCellCount || vecinas.Count != myCellCount)
        {
            regenerateDictionaries();
            Debug.Log("Dictionaries regenerated in Awake!");
        }

        cellSize = 1;
    }

    public void OnBeforeSerialize()
    {
        if (cellsKeys == null)
            cellsKeys = new List<Vector2>();
        if (cellsValues == null)
            cellsValues = new List<Cell>();
        if (vecinasKeys == null)
            vecinasKeys = new List<Cell>();
        if (vecinasValues == null)
            vecinasValues = new List<Neighbors>();

        // Cell map serialization
        cellsKeys.Clear();
        cellsValues.Clear();
        foreach (var kvp in cellMap)
        {
            cellsKeys.Add(kvp.Key);
            cellsValues.Add(kvp.Value);
        }
        // Neighbourhood serialization
        vecinasKeys.Clear();
        vecinasValues.Clear();
        int i = 0;
        foreach (var kvp in vecinas)
        {
            vecinasKeys.Add(kvp.Key);
            vecinasValues.Add(new Neighbors(kvp.Value));
        }
    }

    public void OnAfterDeserialize()
    {
        // Cell map deserialization
        cellMap = new Dictionary<Vector2, Cell>();

        for (int i = 0; i != Math.Min(cellsKeys.Count, cellsValues.Count); i++)
        {
            cellMap.Add(cellsKeys[i], cellsValues[i]);
        }
        cellsKeys = null;
        cellsValues = null;


        // Neighbourhood deserialization
        vecinas = new Dictionary<Cell, Cell[]>();

        for (int i = 0; i != Math.Min(vecinasKeys.Count, vecinasValues.Count); i++)
        {
            vecinas.Add(vecinasKeys[i], vecinasValues[i].Cells);
        }

        vecinasKeys = null;
        vecinasValues = null;

    }

    private void regenerateDictionaries()
    {
        cellMap.Clear();
        vecinas.Clear();

        Cell[] cells = Cells;
        foreach(Cell c in cells){
            this.updateCell(c, true);
        }
    }

	/***********************
	 * Getter Zone
	 * ********************/

    Cell[] Cells
    {
        get { return this.GetComponentsInChildren<Cell>();  }
    }
	

	public void setCellSize(float cellSize){
		this.cellSize = cellSize;

        foreach (Cell c in Cells)
        {
			Vector2 coords = getCoords(c.gameObject);

			c.transform.position.Set(coords.x * cellSize,c.transform.position.y, coords.y * cellSize);
			c.Width = cellSize;
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
	
	public Vector3 getMousePositionOverMap(Ray ray, float height){
		checkTransform();
		
		// Saco el raton y proyecto un rayo con el
		Ray rayo = ray;
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

    private bool instanciatingGhost = false;
	public void ghostCell(Vector3 position, float intensity){
		checkTransform();
		
		if(ghost == null || ghostType != 1){
			removeGhost();
			ghostType = 1;
            instanciatingGhost = true;
			ghost = GameObject.Instantiate(IsoSettingsManager.getInstance().getIsoSettings().defaultCellPrefab) as GameObject;
			ghost.name = "Ghost";
			ghost.hideFlags = HideFlags.HideAndDontSave;
			ghost.GetComponent<Cell>().Map = this;
            instanciatingGhost = false;
		}
		
		// Getting the localPosition
		position = m_transform.InverseTransformPoint(position);
		
		Cell cell = ghost.GetComponent<Cell>();
		cell.Width = cellSize;	

		//La refresco
		Vector2 coords = getCoords(position);
		cell.Height = position.y;
		cell.transform.localPosition = new Vector3((coords.x+0.5f) * cellSize, 0, (coords.y+0.5f) * cellSize);

		Material mat = new Material(Shader.Find("Transparent/Diffuse"));
		mat.color = new Color(mat.color.r,mat.color.g,mat.color.b,intensity);
		ghost.GetComponent<Renderer>().sharedMaterial = mat;
	}

	public void ghostDecoration(Cell cs, Vector3 position, int angle, bool parallel, bool centered, IsoDecoration dec, float intensity){
		checkTransform();
		
		if(ghost == null || ghostType != 2){
			removeGhost();
			ghostType = 2;
			ghost = cs.addGhost(position, angle, parallel, centered, dec, intensity);
		}else{
			if (ghost.GetComponent<Decoration> ().Father != cs) {
				removeGhost();
				ghostType = 2;
				ghost = cs.addGhost(position, angle, parallel, centered, dec, intensity);
			}
		}
		Decoration der = ghost.GetComponent<Decoration>();
		der.IsoDec = dec;
		der.setParameters(position,angle,parallel,centered);

	}

	public void removeGhost(){
		if (ghost != null) {
			if(ghostType==2){
				(ghost.GetComponent<Decoration>().Father as Cell).removeGhost();
			}
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
#if UNITY_EDITOR
		GameObject go = UnityEditor.PrefabUtility.InstantiatePrefab(IsoSettingsManager.getInstance().getIsoSettings().defaultCellPrefab) as GameObject;
#else
        GameObject go = GameObject.Instantiate(IsoSettingsManager.getInstance().getIsoSettings().defaultCellPrefab);
#endif

        // Getting the localPosition
		position = m_transform.InverseTransformPoint(position);

		// Setting base properties
		Cell cell = go.GetComponent<Cell>();
		cell.Map = this;
		cell.transform.localPosition = new Vector3(position.x,0,position.z);
        cell.Height = position.y;
		cell.Width = cellSize;
        cell.forceRefresh();

		//Añado la celda al conjunto de celdas

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
            Cell other;
            cellMap.TryGetValue(coords, out other);

			//Si hay otra celda en mi posicion
			if(other != null && other != cell){
				// Quito la celda
				//vecinas.Remove(other);
                //celdas.Remove(other);
                //cellMap[coords] = cell;
                /**
                 * Inside the destroy, references must be destroyed, even the cellMap and the neightbors
                 * */
                Debug.Log("Solicitando destrucción: ");
                GameObject.DestroyImmediate(other.gameObject);
                Debug.Log("Debería estar destruido. ");
            }
            cellMap.Add(coords, cell);

			// Busco mis nuevos vecinos
			searchNeighborhood(cell);
			// Les informo de mi posicion
			updateReferences(cell, vecinas[cell]);	
		}
	}


	public void removeCell(Cell cell){
		this.removeReferences(cell);
	}

    public void registerCell(Cell cell){
        if (instanciatingGhost || cell.gameObject == ghost)
            return;

        //Añado la celda al conjunto de celdas
        if (!this.vecinas.ContainsKey(cell)) {
            this.updateCell(cell, true);
        }
    }


	/* ***************** *
	 * Neighborhood Zone *
	 * ***************** */

	private void searchNeighborhood(Cell cell){
		Cell[] v = new Cell[4];

		Vector2 current = getCoords(cell.gameObject);
        Debug.Log(current);

        // If all cells have registered
        if (cellMap != null){
            cellMap.TryGetValue(new Vector2(current.x, current.y + cellSize), out v[0]);
            cellMap.TryGetValue(new Vector2(current.x + cellSize, current.y), out v[1]);
            cellMap.TryGetValue(new Vector2(current.x, current.y - cellSize), out v[2]);
            cellMap.TryGetValue(new Vector2(current.x - cellSize, current.y), out v[3]);
        }else{
            // Code fragment in case registration is done wrong or something...
            Debug.Log("He tenido que buscar en el mapa completo :C");
            foreach (Cell c in Cells)
            {
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
        }

		if(!vecinas.ContainsKey(cell))
			vecinas.Add(cell, v);
		else
			vecinas[cell] = v;
	}

    // Guarantee opposite references
    // * If i'm your neighbor you're mine too *
	private void updateReferences(Cell cell, Cell[] cells){
		for(int i = 0; i<cells.Length; i++)
			if(cells[i]!=null)
                if (vecinas.ContainsKey(cells[i]))
				    vecinas[cells[i]][(i+2)%4] = cell;

	}

    private bool haveAnyNeighbor(Cell cell)
    {
        bool iAm = false;
        Cell[] v = null;
        vecinas.TryGetValue(cell, out v);
        if (v != null)
        {
            // v[0] es X = X, Y = Y + CellSize
            // v[1] es X = X + CellSize, Y = Y
            // v[2] es X = X, Y = Y - CellSize
            // v[3] es X = X - CellSize, Y = Y

            iAm = v[0] != null || v[1] != null || v[2] != null || v[3] != null;
        }

        return iAm;
    }

    private Vector2 getCoordsFromNeighbors(Cell cell)
    {
        Vector2 coords = new Vector2(0,0);
        Cell[] v = null;
        vecinas.TryGetValue(cell, out v);
        if (v != null)
        {
            // v[0] es X = X, Y = Y + CellSize
            // v[1] es X = X + CellSize, Y = Y
            // v[2] es X = X, Y = Y - CellSize
            // v[3] es X = X - CellSize, Y = Y

            if(v[0] != null) coords = getCoords(v[0].gameObject) + new Vector2(0, -1);
            else if(v[1] != null) coords = getCoords(v[1].gameObject) + new Vector2(-1, 0);
            else if(v[2] != null) coords = getCoords(v[2].gameObject) + new Vector2(0, 1);
            else if (v[3] != null) coords = getCoords(v[3].gameObject) + new Vector2(1, 0);
        }

        return coords;
    }

	private void removeReferences(Cell cell)
	{
        if (cell.gameObject == ghost && ghost != null)
            return;

        //Debug.Log("Borrando referencias para: " + cell.gameObject.name + " en " + getCoords(cell.gameObject));
            
        // Cell Map
        Vector2 coords = getCoords(cell.gameObject), oldCoords;
        Cell me = null;

        /**
         * Notice that all this steps are made to optimize inverse search.
         * 1st step (if) shouldnt work
         * 2nd step (if) is the most probably to happen
         * 3rd step (if) is the assurance step
         * */
        // First of all, i look for me in my current position
        cellMap.TryGetValue(coords, out me);
        if (me == cell)
            cellMap.Remove(coords);
        // Second, i look for me in my neighbors
        else if (haveAnyNeighbor(cell) && cellMap.TryGetValue(oldCoords = getCoordsFromNeighbors(cell), out me) && me == cell)
            cellMap.Remove(oldCoords);
        // If i dont find me, i look if i am actually in the map
        else if (cellMap.ContainsValue(cell))
        {
            // And in that case, i remove me
            Vector2 key = new Vector2(0,0);
            foreach (var kvp in cellMap)
                if (kvp.Value == cell)
                {
                    key = kvp.Key;
                    break;
                }

            cellMap.Remove(key);
        }

        // Neighborhood
		if(vecinas.ContainsKey(cell))
		{
			Cell[] bcVecinas = vecinas[cell];
            int vecinasLimpiadas = 0;
			for(int i = 0; i< bcVecinas.Length; i++)
                if (bcVecinas[i] != null) 
                    if(vecinas.ContainsKey(bcVecinas[i])){ 
					    vecinas[bcVecinas[i]][(i+2) % 4] = null;
                        vecinasLimpiadas++;
                     }
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
		foreach(Renderer r in this.transform.GetComponentsInChildren<Renderer>())
			r.enabled = visible;
	}

	public void broadcastEvent(GameEvent ge){
		foreach(Cell c in this.transform.GetComponentsInChildren<Cell>()){
			foreach(Entity e in c.getEntities()){
				e.eventHappened(ge);
			}
		}
	}

	private Entity[] entities;
	private List<Entity> toAdd;
	private int removed;

	public void OnEnable(){
		this.entities = this.gameObject.GetComponentsInChildren<Entity> ();
		toAdd = new List<Entity> ();
	}

	public void registerEntity(Entity entity){
		toAdd.Add (entity);
	}

	public void unRegisterEntity(Entity entity){
		bool r = false;
		for (int i = 0; i< this.entities.Length; i++) {
			if(entities[i] == entity) {
				entities[i] = null;
				r = true;
				break;
			}
		}
		if (r == false && toAdd.Contains (entity))
			r = toAdd.Remove (entity);

		if (r)
			removed++;
	}

	private void UpdateEntities(){
		Entity[] newEntities = new Entity[entities.Length - removed + toAdd.Count];
		int pos = 0;
		foreach(Entity e in this.entities)
		if(e != null) {
			newEntities[pos] = e; pos++;
		}
		foreach(Entity e in toAdd){
			newEntities[pos] = e; pos++;
		}
		removed = 0;
		toAdd.Clear();
		this.entities = newEntities;
	}

	public void tick(){

		if (removed > 0 || toAdd.Count > 0)
			UpdateEntities ();

		foreach (Entity e in this.entities)
			e.tick ();

	}

	/***************
	 * CONTROLLER ZONE
	 * **************/

	private void fillEntity(Entity e, ControllerEventArgs args){
		args.entity = e;
		args.cell = args.entity.Position;
		args.options = args.entity.getOptions();	
	}

	public void fillControllerEvent(ControllerEventArgs args){
		bool encontrado = false;
		if(args.isLeftDown){
			RaycastHit[] hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(args.mousePos));
			if(hits.Length>0){
				Heap<float> pqHits = new Heap<float>(hits.Length);
				for(int i = 0; i<hits.Length; i++){
					if(hits[i].collider.transform.IsChildOf(this.transform))
						pqHits.push(i+1, hits[i].distance);
				}
				while(!encontrado && !pqHits.isEmpty()){
					RaycastHit hit = hits[pqHits.top().elem-1];
					pqHits.pop();
					Cell c = hit.collider.GetComponent<Cell>();
					if(c!=null){
						args.cell = c;
						Entity[] entities = c.getEntities();
						if(entities.Length==1)
							fillEntity(entities[0], args);
						encontrado = true;
					}else{
						Entity e = hit.collider.GetComponent<Entity>();
						if(e!=null)
							fillEntity(e,args);
						encontrado=true;
					}
				}
			}
		}
		args.send = args.UP || args.DOWN || args.LEFT || args.RIGHT || encontrado;
	}

	public void Update(){

	}

}

