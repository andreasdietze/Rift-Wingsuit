using UnityEngine;
using XInputDotNetPure; //Required

public class XBoxController : Controller {

    PlayerIndex playerIndex;
    GamePadState state;
    GamePadState prevState;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        if (!FindController())
        {
            return;
        }
        prevState = state;
        state = GamePad.GetState(playerIndex);

	}

    public bool FindController()
    {
        if (!prevState.IsConnected)
        {
            for (int i = 0; i < 4; ++i)
            {
                PlayerIndex testPlayerIndex = (PlayerIndex)i;
                GamePadState testState = GamePad.GetState(testPlayerIndex);
                if (testState.IsConnected)
                {
                    playerIndex = testPlayerIndex;
                    return true;
                }
            }
        }
        return prevState.IsConnected;
    }

    public override Vector3 GetDir()
    {
        Vector3 dir = new Vector3();			// create (0,0,0)
        dir.x = state.ThumbSticks.Left.X;
        dir.y += state.Triggers.Left;
        dir.y -= state.Triggers.Right;
        dir.z = state.ThumbSticks.Left.Y;
        dir.z += 1.0f;

        dir.Normalize();

        return dir;
    }

    public override Vector3 CalculateViewport(bool inverted)
    {
        Vector3 currentPosition = new Vector3(state.ThumbSticks.Right.X * 128, state.ThumbSticks.Right.Y * 128, 0);

        lastViewport = currentPosition - lastViewport;
        if (!inverted) lastViewport.y = -lastViewport.y;
        lastViewport *= sensitivity;
        lastViewport = new Vector3(transform.eulerAngles.x + lastViewport.y,
                                 transform.eulerAngles.y + lastViewport.x, 0);
        transform.eulerAngles = lastViewport;
        lastViewport = currentPosition;
        return lastViewport;
    }
}
