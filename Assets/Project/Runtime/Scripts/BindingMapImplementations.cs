using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameObject2GameObjectBindingMap : BindingMap<GameObject, GameObject> { }

[Serializable]
public class CellObjectBindingMap : BindingMap<Cell, CellObject> { }


//[Serializable]
//public class GameObject2GameObjectBindingMap : BindingMap_OLD<GameObject, GameObject> { }

//[Serializable]
//public class CellObjectBindingMap : BindingMap_OLD<Cell, CellObject> { }