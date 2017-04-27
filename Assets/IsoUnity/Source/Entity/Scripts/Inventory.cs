using UnityEngine;
using System.Collections.Generic;
using IsoUnity;

namespace IsoUnity.Entities
{

    public class Inventory : EventedEntityScript
    {

        [GameEvent]
        public void OpenInventory()
        {
            InventoryGUI gui = ScriptableObject.CreateInstance<InventoryGUI>();
            gui.init(this);
            GUIManager.addGUI(gui, 2);
        }

        [GameEvent]
        public void AddItem(Item item)
        {
            items.Add(item);
        }


        [GameEvent]
        public void RemoveItem(Item item)
        {
            items.Remove(item);
        }


        [GameEvent]
        public void UseItem(Item item)
        {
            if (items.Contains(item)) item.use();
        }

        public override Option[] getOptions()
        {
            if (this.Entity.GetComponent<Player>() != null)
            {
                GameEvent ge = new GameEvent();
                ge.Name = "open inventory";
                ge.setParameter("Inventory", this);
                Option option = new Option("Inventory", ge, false, 0);
                return new Option[] { option };
            }
            else
                return null;
        }

        public override void Update() { }

        [SerializeField]
        private List<Item> items = new List<Item>();
        public Item[] Items
        {
            get { return items.ToArray() as Item[]; }
        }

    }
}