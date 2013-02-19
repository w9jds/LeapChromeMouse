using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leap;
using System.Runtime.InteropServices;

class LeapListener : Listener
{
    [DllImport("user32.dll", SetLastError = true)]
    static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

    public const int KEYEVENTF_EXTENDEDKEY = 0x01; //Key down flag
    public const int KEYEVENTF_KEYUP = 0x02; //Key up flag
    public const int VK_LCONTROL = 0xA2; //Left Control key code
    public const int VK_TAB = 0x09; //Tab key code
    public const int VK_SHIFT = 0x10; //Shift key code

    public override void OnInit(Controller controller) {
        Console.WriteLine("Initialized");
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
        Console.WriteLine("Frame id: " + frame.Id + ", timestamp: " + frame.Timestamp + ", hands: " + frame.Hands.Count + ", fingers: " + frame.Fingers.Count + ", tools: " + frame.Tools.Count);

        if (!frame.Hands.Empty) 
        {
            // Get the first hand
            Hand hand = frame.Hands[0];
            FingerList fingers = hand.Fingers;
            Vector handVector = hand.PalmNormal;
            if (!fingers.Empty)
            {
                var fingerVelocity = fingers.Average(f => f.TipVelocity.x);
                var fingerVelocityVariance = fingers.Sum(f => Math.Pow(f.TipVelocity.x - fingerVelocity, 2)) / fingers.Count();
                var fingerVelocityDeviation = (float)Math.Sqrt(fingerVelocityVariance);

                if ((handVector.Yaw * 57.295779513f) > 25 && fingerVelocity > 300 && fingerVelocityDeviation > 40)
                {
                    Console.WriteLine("swipe left");
                    keybd_event(VK_LCONTROL, 0, 0, 0);
                    keybd_event(VK_SHIFT, 0, 0, 0);
                    keybd_event(VK_TAB, 0, 0, 0);

                    keybd_event(VK_TAB, 0, KEYEVENTF_KEYUP, 0);
                    keybd_event(VK_SHIFT, 0, KEYEVENTF_KEYUP, 0);
                    keybd_event(VK_LCONTROL, 0, KEYEVENTF_KEYUP, 0);
                    System.Threading.Thread.Sleep(50);
                }

                else if ((handVector.Yaw * 57.295779513f) < -25 && fingerVelocity < -300 && fingerVelocityDeviation > 40)
                {
                    Console.WriteLine("swipe right");
                    keybd_event(VK_LCONTROL, 0, 0, 0);
                    keybd_event(VK_TAB, 0, 0, 0);

                    keybd_event(VK_TAB, 0, KEYEVENTF_KEYUP, 0);
                    keybd_event(VK_LCONTROL, 0, KEYEVENTF_KEYUP, 0);
                    System.Threading.Thread.Sleep(50);
                }
            }
                 
            // Check if the hand has any fingers
            /*
            FingerList fingers = hand.Fingers;
            if (!fingers.Empty) 
            {
                // Calculate the hand's average finger tip position
                Vector avgPos = new Vector();
                for (int i = 0; i < fingers.Count(); ++i)
                    avgPos += fingers[i].TipPosition;
                
                avgPos /= (float)fingers.Count;
                Console.WriteLine("Hand has " + fingers.Count + " fingers, average finger tip position" + avgPos);
            }
                
            // Get the hand's sphere radius and palm position
            Console.WriteLine("Hand sphere radius: " + hand.SphereRadius + " mm, palm position: " + hand.PalmPosition);

            // Get the hand's normal vector and direction
            Vector normal = hand.PalmNormal;
            Vector direction = hand.Direction;

            // Calculate the hand's pitch, roll, and yaw angles
            Console.WriteLine("Hand pitch: " + direction.Pitch * 57.295779513f + " degrees, roll: " + normal.Roll * 57.295779513f + " degrees, yaw: " + direction.Yaw * 57.295779513f + " degrees");
            */
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

