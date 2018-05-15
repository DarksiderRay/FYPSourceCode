 using UnityEngine;
 
 [ExecuteInEditMode]
 public class NewBehaviourScript : MonoBehaviour 
 {
     public float BasemapDistance = 2000;
     private Terrain terrain;
 
     void OnEnable () 
     {
         terrain = GetComponent<Terrain> ();
 #if UNITY_EDITOR
         UnityEditor.EditorApplication.update += Set;
 #endif
     }
 
 #if !UNITY_EDITOR
     void Update () 
     {
         Set();
     }
 #endif
 
     void Set () 
     {
         if (terrain == null)
             terrain = GetComponent<Terrain> ();
         else if (terrain.basemapDistance != BasemapDistance)
             terrain.basemapDistance = BasemapDistance;
     }
 }