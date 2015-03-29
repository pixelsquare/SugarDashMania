using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class ConnectionTest : MonoBehaviour {

	private static string fileName = "cmd.exe";
	private static string arguments = "/c ping 74.125.239.116"; //replace localhost with masterserver ip.
	private static ProcessStartInfo cmdping;

	private static Ping connectionPing = new Ping("74.125.239.116");
	public static Ping ConnectionPing {
		get { return connectionPing; }
	}

	//private static bool donePinging = false;
	//public static bool DonePinging {
	//    get {
	//        if (connectionPing != null) {
	//            //UnityEngine.Debug.Log(connectionPing.time);
	//            donePinging = connectionPing.isDone;
	//        }

	//        return donePinging;
	//    }
	//}

	private static bool hasInternetConnection = false;
	public static bool HasInternetConnection {
		get {
			if (connectionPing != null && connectionPing.time > 0)
				hasInternetConnection = true;
			else
				hasInternetConnection = false;

			return hasInternetConnection;	
		}
	}

	public static void InitConnectionTest() {
		cmdping = new ProcessStartInfo(fileName, arguments);
		Process.Start(cmdping);
	}

	//public static IEnumerator GetConnectionTest() {
	//    while (!connectionPing.isDone)
	//        yield return null;

	//    if (connectionPing.time > 0)
	//        hasInternetConnection = true;
	//}

	//private void Awake() {
	//    cmdping = new ProcessStartInfo(fileName, arguments);
	//    Process.Start(cmdping);
	//    ping = new Ping("74.125.239.116");
	//    //ping = new Ping("127.0.0.1");
	//    //Put your code for connecting from within unity here, after the cmd opens and pings. 
	//}

	//private IEnumerator Start() {
	//    while (!ping.isDone)
	//        yield return null;

	//    if (ping.time > 0)
	//        hasInternetConnection = true;

	//    //UnityEngine.Debug.Log(ping.time.ToString());
	//}
}
