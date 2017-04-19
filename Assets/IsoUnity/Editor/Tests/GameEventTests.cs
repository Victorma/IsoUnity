using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using System.Collections.Generic;

public class GameEventTests {

	[Test]
	public void NameTest()
	{
		var lge = new List<IGameEvent> ();

		lge.Add (new GameEvent ());
		lge.Add (ScriptableObject.CreateInstance<SerializableGameEvent> ());


		foreach (var ge in lge) {

			ge.Name = "evento";
			Assert.AreEqual("evento", ge.Name);

			/*ge.name = "evento";
			Assert.AreEqual(ge.name, ge.Name);

			ge.Name = "eventito";

			Assert.AreEqual(ge.name, ge.Name);*/
		}
	}

    [Test]
    public void SetParameterTest()
    {
		var lge = new List<IGameEvent> ();

		lge.Add (new GameEvent ());
		lge.Add (ScriptableObject.CreateInstance<SerializableGameEvent> ());

		var par = new Dictionary<string, object> ();
		par.Add ("string", "t");
		par.Add ("int", 1);
		par.Add ("float", 2.5f);
		par.Add ("double", 3.25);
		par.Add ("char", 'c');
		par.Add ("bool", true);
		par.Add ("vector2", new Vector2(1,2));
		par.Add ("vector3", new Vector3(1,2,3));
		par.Add ("vector4", new Vector4(1,2,3,4));
		par.Add ("quaternion", new Quaternion(1,2,3,4));
		par.Add ("gameObject", new GameObject());
		par.Add ("null", null);

		foreach (var ge in lge) {

			var i = 0;
			foreach (var entry in par) {
				ge.setParameter (entry.Key, entry.Value);
				i++;

				Assert.AreEqual(ge.getParameter(entry.Key), entry.Value);
				Assert.AreEqual(ge.Params.Length, i);
			}
		}
    }

	[Test]
	public void RemoveParameterTest()
	{
		var lge = new List<IGameEvent> ();

		lge.Add (new GameEvent ());
		lge.Add (ScriptableObject.CreateInstance<SerializableGameEvent> ());

		foreach (var ge in lge) {

			ge.setParameter ("test", "t");

			Assert.AreEqual(ge.getParameter("test"), "t");
			Assert.AreEqual(ge.Params.Length, 1);
			Assert.AreEqual(ge.Params[0], "test");

			ge.removeParameter ("test");

			Assert.AreEqual(ge.getParameter("test"), null);
			Assert.AreEqual(ge.Params.Length, 0);
		}
	}

	[Test]
	public void EquallityTest()
	{
		var lge = new List<IGameEvent> ();

		lge.Add (new GameEvent ());
		lge.Add (new GameEvent ());
		lge.Add (ScriptableObject.CreateInstance<SerializableGameEvent> ());
		lge.Add (ScriptableObject.CreateInstance<SerializableGameEvent> ());

		var par = new Dictionary<string, object> ();
		par.Add ("string", "t");
		par.Add ("int", 1);
		par.Add ("float", 2.5f);
		par.Add ("double", 3.25);
		par.Add ("char", 'c');
		par.Add ("bool", true);
		par.Add ("vector2", new Vector2(1,2));
		par.Add ("vector3", new Vector3(1,2,3));
		par.Add ("vector4", new Vector4(1,2,3,4));
		par.Add ("quaternion", new Quaternion(1,2,3,4));
		par.Add ("gameObject", new GameObject());
		par.Add ("null", null);

		foreach (var ge in lge) {
			ge.Name = "evento";
			foreach (var entry in par) {
				ge.setParameter (entry.Key, entry.Value);
			}
		}

		foreach (var ge in lge) {
			foreach (var ge2 in lge) {
				Assert.True (GameEvent.CompareEvents(ge, ge2), "GameEvent equallity comparing: " + ge.GetType() + " with " + ge2.GetType());
				Assert.True (ge.Equals(ge2), "Equals method equallity comparing: " + ge.GetType() + " with " + ge2.GetType());

				if (ge is GameEvent) {
					var rge = ge as GameEvent;
					Assert.True (rge == ge2, "Operator equallity comparing: " + ge.GetType() + " with " + ge2.GetType());
				}else if (ge is SerializableGameEvent) {
					var sge = ge as SerializableGameEvent;
					Assert.True (sge == ge2, "Operator equallity comparing: " + ge.GetType() + " with " + ge2.GetType());
				}
			}
		}
	}
		
}
