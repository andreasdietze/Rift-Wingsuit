using UnityEngine;
using System;

//Singleton class in order to avoid possible instantiation problems.
public class Fader : MonoBehaviour
{
    private static Fader actualInstance;

    //Accessor
    private static Fader instance
    {
        get
        {
            if (actualInstance == null)
            {
                //Try to find another instance first.
                actualInstance = (Fader)GameObject.FindObjectOfType(typeof(Fader));
                if (actualInstance == null)
                {
                    actualInstance = new GameObject("Fader").AddComponent<Fader>();
                }
            }
            return actualInstance;
        }
    }
    //Enables the fading
    private Texture2D fadeTexture;

    //Enables the override of the normal background drawing
    private GUIStyle backgroundStyle = new GUIStyle();

    //Current color of the fading
    private Color currentFadingColor = new Color(0, 0, 0, 0);
    private Color targetFadingColor = new Color(0, 0, 0, 0);

    //"Delta" to be applied for the currentFadingColor each second
    private Color deltaFadingColor = new Color(0, 0, 0, 0);

    //In order to avoid problems with OnGUI() as it may be called several times at once
    private bool usedFrame = false;

    //Duration of the fade out (in seconds)
    public float fadeOutDuration = 5.0f;

    //Sets the given color to the fadingTexture and applies it immediately.
    private static void SetScreenColor(Color newColor)
    {
        instance.currentFadingColor = newColor;
        instance.fadeTexture.SetPixel(0, 0, instance.currentFadingColor);
        instance.fadeTexture.Apply();
    }

    //Interface for the user
    public static void StartFade(Color fadingColor, float duration, Action onFinish)
    {
        instance.fadeOutDuration = duration;

        //No duration at all, immediately 'fade'
        if (instance.fadeOutDuration <= 0.0f)
        {
            SetScreenColor(fadingColor);
        }
        else
        {
            instance.targetFadingColor = fadingColor;
            SetScreenColor(new Color(instance.targetFadingColor.r, instance.targetFadingColor.g, instance.targetFadingColor.b, 0));
        }

        //Calculate the change of color per second
        instance.deltaFadingColor = (instance.targetFadingColor - instance.currentFadingColor) / instance.fadeOutDuration;
    }


    //More detailled delegate
    public static void StartFade(Color fadingColor, float duration)
    {
        StartFade(fadingColor, duration, null);
    }

    //Delegate 
    public static void StartFade(Color fadingColor)
    {
        StartFade(fadingColor, instance.fadeOutDuration);
    }

    //Init instance upon loading
    void Awake()
    {
        instance.init();
    }

    void Update() 
    {
        usedFrame = false;
    }

    //Dirty, dirtier, this piece of code.
    public bool HasNextFrame() 
    {
        return !usedFrame;
    }

    //Is regulary called for rendering/handling GUIEvents
    void OnGUI()
    {
        //Keep fading in case the target color is not yet matched; also, check if another frame has elapsed
        //in order to avoid unncessary redrawings/calculations
        if (instance.currentFadingColor != instance.targetFadingColor && instance.HasNextFrame())
        {
            instance.usedFrame = true;
            //We're finished in case the distance between the Alphas is smaller than the passed time * alpha_PerTick
            if (Mathf.Abs(instance.currentFadingColor.a - instance.targetFadingColor.a) < Mathf.Abs(instance.deltaFadingColor.a) * Time.deltaTime)
            {
                instance.currentFadingColor = instance.targetFadingColor;
                SetScreenColor(instance.targetFadingColor);
                instance.deltaFadingColor = new Color(0, 0, 0, 0);

                //Reload the level after the fading is finished
                Stop();
                Application.LoadLevel(Application.loadedLevel);
            }
            else
            {
                Color currentColorWithDelta = new Color( instance.targetFadingColor.r, instance.targetFadingColor.g, instance.targetFadingColor.b, instance.currentFadingColor.a + instance.deltaFadingColor.a * Time.deltaTime);
                SetScreenColor(currentColorWithDelta);
            }
        }
        //Start drawing the fading texture onto the entire screen if it's visible (alpha>0)
        if (instance.currentFadingColor.a > 0.0f)
        {
            //In order to make it draw on top of all other objects
            GUI.depth = -10000;
            GUI.Label(new Rect(-1.0f, -1.0f, Screen.width + 1.0f, Screen.height + 1.0f), instance.fadeTexture, instance.backgroundStyle);
        }
    }

    public void init()
    {
        instance.fadeTexture = new Texture2D(1, 1);

        //Override the normal background style with our fading one.
        instance.backgroundStyle.normal.background = instance.fadeTexture;
    }

    void Stop()
    {
        actualInstance = null;
        Destroy(gameObject);
    }

    void OnApplicationQuit()
    {
        Stop();
    }
};