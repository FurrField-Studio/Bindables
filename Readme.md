# Bindables

Bindables is a lightweight Unity editor extension designed to automate reference assignments in your MonoBehaviour scripts, cutting down manual Inspector setup and reducing errors.

# Table of Contents

- [Installation](#installation)
- [Setup](#setup)
- [Attributes](#attributes)
    - [Bind](#bind)
    - [BindSelf](#bindself)
    - [BindComponent](#bindcomponent)
    - [BindGameobject](#bindgameobject)
    - [BindScriptableObject](#bindscriptableobject)
- [Roadmap](#roadmap)

# Installation

- open <kbd>Window/Package Manager</kbd>
- click <kbd>+</kbd>
- click <kbd>Add package from git URL</kbd>
- add `https://github.com/FurrField-Studio/Bindables.git` in Package Manager

# Setup

There is not much to setup, but you can go to ``Project Settings/Bindables`` to add suffixes that you want to remove when searching for names using variable name as input (so without providing name in attribute argument).

# Attributes

Here is little nifty overview of features every attribute provides:

| Bindable Type        |        Grabs data from        |             Supported types             | Supports collections | Supports name filtering | Supports collection filtering | Supports path |
|:---------------------|:-----------------------------:|:---------------------------------------:|:-------------------:|:-----------------------:|:-----------------------------:|:-------------:|
| Bind                 | Self, Children, AssetDatabase | Component, GameObject, ScriptableObject |         No          |           Yes           |              No               |      No       |
| BindSelf             |             Self              |                Component                |         Yes         |      No (no need)       |              No               |      No       |
| BindComponent        |           Children            |                Component                |         Yes         |           Yes           |              Yes              |      No       |
| BindGameObject       |           Children            |               GameObject                |         Yes         |           Yes           |              Yes              |      No       |
| BindScriptableObject |         AssetDatabase         |            ScriptableObject             |         Yes         |           Yes           |              Yes              |      Yes      |

## Bind

This is basically what AutoHook achieves or BindWidget from Unreal, but with much greater range of supported types. It doesnt support collections for the sake of simplicity and doesnt contain features available in more specialized attributes.

```csharp
[Bind]
public GameObject bindGo; //will assign gameobject with the name "Bind"
[Bind]
public Transform bindTransform; //will assign transform with the gameobject name "Bind"
[Bind]
public TestElement0 bind; //will try to find gameobject with that specific name and grab component from it, if its not found it will grab component from this gameobject
[Bind]
public TestSO bindSo; //will try to find ScriptableObject with name "Bind" and matching type

//you can also override the name used for searching

[Bind("Bind")]
public GameObject bindYes; //will assign gameobject with the name "Bind"
[Bind("Bind")]
public Transform bindNo; //will assign transform with the gameobject name "Bind"
[Bind("Bind")]
public TestElement0 bindMaybe; //will try to find gameobject with that specific name and grab component from it, if its not found it will grab component from this gameobject
[Bind("Bind")]
public TestSO bindPlease; //will try to find ScriptableObject with name "Bind" and matching type
```

## BindSelf

This attribute only binds components that are found on the gameobject. It supports arrays but it doesnt support array filtering.

```csharp
[BindSelf]
public Transform bindTransform; //will assign this gameobject transform (I know its weird but it uses the same way as in assinging normal components)
[BindSelf]
public TestElement0 bind; //will assign component from this gameobject

//Also supports arrays and lists but due to unity way of drawing lists, you need to click + to populate it
//after clicking + to autopopulate you can freely add more entries, becouse the autopopulate only work when array is empty
        
[BindSelf]
public TestElement0[] bindArray;
[BindSelf]
public List<TestElement0> bindList;
```

## BindComponent

This is specialised attribute for binding components it supports name filtering, arrays and array filtering.

```csharp
[BindComponent("BindComponent")]
public TestElement0 testElement0;

// supports array binding

[BindComponent]
public TestElement0[] testElement0Array;

[BindComponent(filterMethod: nameof(FilterBind))] //and also collection filtering
public List<TestElement0> testElement0List;

private ICollection<TestElement0> FilterBind(ICollection<TestElement0> collection)
{
    Debug.Log($"collection: {collection.Count}");

    return collection;
}
```

## BindGameobject

This is specialised attribute for binding gameobjects it supports name filtering, arrays and array filtering.

```csharp
[BindGameobject("BindGameobject")]
public GameObject gameobjectBind;

// supports array binding

[BindGameobject]
public GameObject[] gameobjectArray;

[BindGameobject(filterMethod: nameof(FilterBind))] //and also collection filtering
public List<GameObject> gameobjectList;

private ICollection<GameObject> FilterBind(ICollection<GameObject> collection)
{
    Debug.Log($"collection: {collection.Count}");

    return collection;
}
```

## BindScriptableObject

This is specialised attribute for binding scriptable objects it supports name filtering, arrays and array filtering.

```csharp
[BindScriptableObject]
public TestSO testSo;

[BindScriptableObject("TestSO")]
public TestSO testSoNamed;

// supports array binding

[BindScriptableObject(filterMethod: nameof(FilterBind))] // supports array filtering
public TestSO[] testSoFilter;

private ICollection<TestSO> FilterBind(ICollection<TestSO> collection)
{
    Debug.Log($"collection: {collection.Count}");

    return collection;
}

// allows to set prefix for scriptable array search so it will populate with scriptable objects that are prefixed with that name
[BindScriptableObject(prefix: "Test")]
public TestSO[] testSoPrefix;
        
// allows to set path from where scriptable objects will be queried
[BindScriptableObject(path: "Assets/Scripts")]
public TestSO[] testSoPath;
        
// you can combine prefix with a path
[BindScriptableObject(prefix: "Test", path: "Assets/Scripts")]
public TestSO[] testSoPathPrefix;
```

# Roadmap

- [ ] Add "search option" to tell code if it needs to search in parents, children or self. This will allow to consolidate attributes.
