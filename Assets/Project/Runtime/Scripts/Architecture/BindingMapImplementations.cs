using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameObject2GameObjectBindingMap : BindingMap<GameObject, GameObject> { }

[Serializable]
public class CellObjectBindingMap : BindingMap<Cell_OLD, CellObject> { }