# Quackmageddon: The Duckie Hunting Game
> The main goal of the project is to develop a **shooting survival game prototype** using **Unity Engine** and the **Google Cardboard SDK**.

![popUpShort](https://user-images.githubusercontent.com/1534654/121075266-4389d500-c7d5-11eb-9998-3153f64728de.gif)

## Table of contents
* [Technologies Used](#technologies-used)
* [Features](#features)
* [OOP Principles and Software Design Patterns](#oop-principles-and-software-design-patterns)
* [Setup](#setup)
* [Inspirations](#inspirations)
* [Contact](#contact)

## Technologies Used
Project is developed with:
* Google VR SDK
* Unity Engine version 2018.4.35f1
* Low weight render pipeline
* Unity Shader Graph
* Unity UI & Animator
* TextMesh Pro
* Adobe Photoshop CS

## Features

* ### Custom Water Surface Shader
![AnimatedWaterSurfaceShader](https://user-images.githubusercontent.com/1534654/121162352-247b5980-c84e-11eb-9cac-6b823371dd99.gif)

> * Implemented using Unity Shader Graph.

* ### Custom HP Bar with his simplified variant
![Health Bar](https://user-images.githubusercontent.com/1534654/121071427-39b1a300-c7d0-11eb-8737-0c4da76c286b.gif)

> * To find in `Assets/Prefabs/UI` directory.

![Health Bar prefabs](https://user-images.githubusercontent.com/1534654/121075941-138f0180-c7d6-11eb-8fb6-1ad3391fef11.png)

* ### Custom Particle Systems
> * Particle prefabs are placed in `Assets/Prefabs/Effects`.
> * Prefab's hierarchy contains a few nodes with added Particle System component, an addictional point light and also root node with controller script.

![Particle prefab hierarchy](https://user-images.githubusercontent.com/1534654/121151286-833bd580-c844-11eb-9a69-04d4f5775a2d.png)

![Explosion Controller Script](https://user-images.githubusercontent.com/1534654/121151426-a23a6780-c844-11eb-8697-71ae4f8fddf4.png)

* ### Beakshot mechanic

![BeakshotShort](https://user-images.githubusercontent.com/1534654/121171392-bf783180-c856-11eb-9883-3d04d1cd9069.gif)

> * The equivalent of a headshot for duckies, but much more difficult to achieve.

![Beakshot score!](https://user-images.githubusercontent.com/1534654/121077544-083cd580-c7d8-11eb-9457-6921a8536db2.png)

> * The possibility of taking a beakshot is indicated by the change of crosshair color to red. It is also rewarded with extra score.

## OOP Principles and Software Design Patterns

* ### MonoSingleton
> * Thread-safe implementation of **Singleton Pattern** for MonoBehaviour.
> * Based on dictionary instead of using FindObjectsOfType method or creating GameObject during the game, which are very inefficient ways of implementation this pattern.
   
```cs
public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    #region Static fields
    protected static bool Quitting 
    { 
         get; 
         private set; 
    }

    private static readonly object Lock = new object();
    private static Dictionary<System.Type, MonoSingleton<T>> instancesDictionary;
    #endregion

    #region Static instance getter
    public static T Instance
    {
        get
        {
            lock (Lock)
            {
                if (instancesDictionary == null)
                {
                    instancesDictionary = new Dictionary<System.Type, MonoSingleton<T>>();
                }

                if (instancesDictionary.ContainsKey(typeof(T)))
                {
                    return (T)instancesDictionary[typeof(T)];
                }
                else
                {
                    return null;
                }
            }
        }
    }
    #endregion

    #region MonoBehaviour's inherited methods
    private void OnEnable()
    {
        lock (Lock)
        {
            if (instancesDictionary == null)
            {
                instancesDictionary = new Dictionary<System.Type, MonoSingleton<T>>();
            }

            if (instancesDictionary.ContainsKey(this.GetType()))
            {
                Destroy(this.gameObject);
            }
            else
            {
                instancesDictionary.Add(this.GetType(), this);

                DontDestroyOnLoad(gameObject);
            }
        }
    }
    #endregion
}
```

* ### Gameplay Event Manager
> * Implements **Observer Pattern**.
> * Using implementation of Singleton Pattern extending MonoBehaviour.
> * Bases on Dictionary containining list of Actions.

```cs 
public class GameplayEventsManager : MonoSingleton<GameplayEventsManager>
{
     private Dictionary<string, List<Action<short>>> listenersDictionary;
```

* ### Universal Object Pooler
![image](https://user-images.githubusercontent.com/1534654/121073987-8b0f6180-c7d3-11eb-80b2-7079f553a98b.png)

> * Implements **Pool Pattern**. 
> * Calls OnSpawn method of **IPooledObject interface**. 
> * Listening for Pause event to disable all pooled objects.
> * Uses Pool serializable structs to store data.

```cs 
[System.Serializable]
public struct Pool
{
   public string tag;
   public GameObject prefab;
   public short poolSize;
}
```

* ### Enemy Spawner
![Enemy Spawner Inspector](https://user-images.githubusercontent.com/1534654/121076435-b778ad00-c7d6-11eb-8126-ac3f14d4b647.png)

> * Spawns enemies using ObjectPooler. 
> * Listens to Pause and Resume events.

* ### Gun controller script
![Gun controllers inspector](https://user-images.githubusercontent.com/1534654/121089059-df700c80-c7e6-11eb-8e64-1622dd4064be.png)

> * Uses raycasting technique to aiming targets and shooting.
> * Also uses tags to detect specified targets.

* ### Health Manager

![Health manager](https://user-images.githubusercontent.com/1534654/121090763-54dcdc80-c7e9-11eb-93ad-cfc28d2e762b.png)

> * Manages health points and also dispatches GameplayEventType.HealthUpdate event using GameplayEventManager. 
> * Includes auto-healing mechanism with cooldown.

![Health Manager in hierarchy](https://user-images.githubusercontent.com/1534654/121090824-6a520680-c7e9-11eb-93b5-ef8e6c2fab6d.png)

* ### Sound Manager
![Sound manager inspector](https://user-images.githubusercontent.com/1534654/121155120-cf3c4980-c847-11eb-9999-019515959c6a.png)

> * Extends custom implementation of **Singleton Pattern** for MonoBehaviour objects.
> * Uses objects of Sound serializable class to store data and help to display its content in a Inspector

```cs  
 [System.Serializable]
 public class Sound
 {
     public string name;
     public AudioClip soundClip;

     [Range(0f, 1f)]
     public float volume = .5f;

     [Range(.1f, 3f)]
     public float pitch = 1f;

     [HideInInspector]
     public AudioSource source;
 }
```
> * Contains method for playing SFX with randomized pitch and volume values.
```cs
   public void PlaySound(Sound soundToPlay, bool isRandomVolumeAndPitch = false, AudioSource source = null )
   {
      if (source != null)
      {
          soundToPlay.source = source;
      }
      soundToPlay.source.Stop();
      soundToPlay.source.volume = !isRandomVolumeAndPitch ? soundToPlay.volume : Random.Range(minimumVolume, soundToPlay.volume);
      soundToPlay.source.pitch = !isRandomVolumeAndPitch ? soundToPlay.pitch : Random.Range(minimumPitch, maximumPitch);

      soundToPlay.source.Play();
   }
```

* ### Enemy controller
> * Extends MonoBehaviour and also implements **IPooledObject interface**
```cs 
public class Enemy : MonoBehaviour, IPooledObject
```
> * Controls owned simplified version of custom HP bar placed on an own Canvas (billboard)
> * Also controls explosion effect which is a set of Particles Systems triggered by a script.

![Enemy controller Inspector](https://user-images.githubusercontent.com/1534654/121157611-01e74180-c84a-11eb-89b7-1e407e1b2ce0.png)


## Setup

* ### Unity Editor

![Assets/Scenes/Gameplay.unity](https://user-images.githubusercontent.com/1534654/121062616-952a6380-c7c5-11eb-9881-2a5bb5898dab.png)

To run the game in the Unity Editor:
1. Select `Assets/Scenes` directory in the **Project** window.
2. Launch a scene named **Gameplay**.
3. Click **Play** button placed on the top of an Editor to start the gameplay.

![Gameplay Scene Hierarchy](https://user-images.githubusercontent.com/1534654/121090426-e13acf80-c7e8-11eb-9a62-457e79c29046.png)

* ### How to build the game

![Build Settings](https://user-images.githubusercontent.com/1534654/121181539-6b734a00-c862-11eb-9e86-e3ff4011af72.png)

1. In the Unity Editor go to **Build Settings** by clicking on `File > Build Settings...` (or alternatively press `Ctrl+Shift+B` shortkey).
2. Make sure **Android** is set as the target platform. If not then select **Android** and click the **Switch Platform** button.
3. Connect your phone device to your computer using **USB cable**.
4. Click `File > Build and Run` and Unity will build the project into an **Android APK**, installs it on the device, and launches it.
5. Put the phone in your **Cardboard viewer** device and try out the gameplay.

## Inspirations 
The idea for a gameplay setting is based on a real event ...

> *On 10 January 1992, during a storm in the North Pacific Ocean close to the International Date Line twelve 40-foot (12-m) intermodal containers were washed overboard. One of these containers held **28,800 Floatees** - that is a plastic rubber ducks. Some of the **toys landed along Pacific Ocean shores**, such as Hawaii.*

... but also on the typical programmer's everyday experience. 

> *In software engineering, <b>rubber duck debugging</b> is a method of debugging code. The name is a reference to a story in the book The Pragmatic Programmer in which a programmer would carry around a rubber duck and debug their code by forcing themselves to explain it, line-by-line, to the duck.*

![Rubber duck assisting with debugging](https://user-images.githubusercontent.com/1534654/121182082-02d89d00-c863-11eb-8046-2b4afdccba62.png)

So the idea for a backstory was that rubber ducklings, driven to the extreme due to fact that for long years they had to listen about the problems of software developers. Developers, who did not even expect that they understood the words addressed to them. So it was a form of torture, as a result of which they gained some kind of the self-awareness, but also began to cause them a desire to pay back for those decades of suffering of their species. 

Finally the high seas accident involving thousands of duckies was not an accident at all, but just the beginning of **the Quackmageddon**!

Sources: [Friendly Floatees - Wikipedia](https://en.wikipedia.org/wiki/Friendly_Floatees), [Rubber duck debugging - Wikipedia](https://en.wikipedia.org/wiki/Rubber_duck_debugging)

## Contact
*Created by* **Damian Śremski** - <d.sremski@gmail.com> - [linkedin.com/in/damianśremski](https://linkedin.com/in/damianśremski)

