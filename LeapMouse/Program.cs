using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leap;
using System.Runtime.InteropServices;

class LeapListener : Listener
{
    private Frame lastFrame = new Frame();

    [DllImport("user32.dll", SetLastError = true)]
    static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

    private const int KEYEVENTF_EXTENDEDKEY = 0x01; //Key down flag
    private const int KEYEVENTF_KEYUP = 0x02; //Key up flag
    private const int VK_LCONTROL = 0xA2; //Left Control key code
    private const int VK_TAB = 0x09; //Tab key code
    private const int VK_SHIFT = 0x10; //Shift key code

    public override void OnInit(Controller controller) {
        Console.WriteLine("Initialized");
        controller.EnableGesture(Gesture.GestureType.TYPESWIPE);
    }

    public override void OnConnect(Controller controller) {
        Console.WriteLine("Connected");
    }

    public override void OnDisconnect(Controller controller) {
        Console.WriteLine("Disconnected");
    }

    public override void OnExit(Controller controller) {
        Console.WriteLine("Exited");
    }

    public override void OnFrame(Controller controller)
    {
        //System.Threading.Thread.Sleep(50);
        Frame frame = controller.Frame();
        
        //Console.WriteLine("Frame id: " + frame.Id + ", timestamp: " + frame.Timestamp + ", hands: " + frame.Hands.Count + ", fingers: " + frame.Fingers.Count + ", tools: " + frame.Tools.Count);

        if (lastFrame == frame)
            return;

        GestureList Gestures =  (lastFrame.IsValid == true) ? frame.Gestures(lastFrame) : frame.Gestures();

        lastFrame = frame;

        for (int i = 0; i < Gestures.Count; i++)
        {
            if (Gestures[i].Type == Gesture.GestureType.TYPESWIPE && Gestures[i].State == Gesture.GestureState.STATESTOP)
            {
                SwipeGesture Swipe = new SwipeGesture(Gestures[i]);
                if (Swipe.Direction.x < (Vector.Right.x + .1) && Swipe.Direction.x > (Vector.Right.x - .1))
                {
                    Console.WriteLine("Swipe Right");
                    keybd_event(VK_LCONTROL, 0, 0, 0);
                    keybd_event(VK_SHIFT, 0, 0, 0);
                    keybd_event(VK_TAB, 0, 0, 0);

                    keybd_event(VK_TAB, 0, KEYEVENTF_KEYUP, 0);
                    keybd_event(VK_SHIFT, 0, KEYEVENTF_KEYUP, 0);
                    keybd_event(VK_LCONTROL, 0, KEYEVENTF_KEYUP, 0);
                    System.Threading.Thread.Sleep(50);

                }
                if (Swipe.Direction.x < (Vector.Left.x + .1) && Swipe.Direction.x > (Vector.Left.x - .1))
                {
                    Console.WriteLine("Swipe Left");
                    keybd_event(VK_LCONTROL, 0, 0, 0);
                    keybd_event(VK_TAB, 0, 0, 0);

                    keybd_event(VK_TAB, 0, KEYEVENTF_KEYUP, 0);
                    keybd_event(VK_LCONTROL, 0, KEYEVENTF_KEYUP, 0);
                    System.Threading.Thread.Sleep(50);

                }
            }
        }
    }
}

class LeapMouse
{
    public static void Main()
    {
        LeapListener listener = new LeapListener();
        Controller controller = new Controller();
        controller.AddListener(listener);

        Console.WriteLine("Press Enter to quit...");
        Console.ReadLine();

        controller.RemoveListener(listener);
    }
}

