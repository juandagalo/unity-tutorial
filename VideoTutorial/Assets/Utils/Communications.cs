using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class Communications : MonoBehaviour
{
	public static event Action OnCompleteQueue
	{
		add { instance.onCompleteQueue += value; }
		remove { instance.onCompleteQueue -= value; }
	}

	private event Action onCompleteQueue;

	private static Communications instance;

	private Thread receiveThread;

	private UdpClient sendClient;
	private UdpClient receiveClient;

	private IPEndPoint remoteEndPoint;
	private IPEndPoint receiveEndPoint;

	public string ip = "127.0.0.1";
	public int port = 32000;
	public int receivePort = 32002;

	private bool isInitialized;

	private Dictionary<string, Action<float>> floatSubscribers;

	private Dictionary<string, Action<string>> stringSubscribers;

	private Queue<string> sendQueue;

	[SerializeField]
	private List<string> logException;

	private bool isSendingQueue;

	private void Awake()
	{
		Initialize();
	}

	private void Initialize()
	{
		instance = this;

		floatSubscribers = new Dictionary<string, Action<float>>();
		stringSubscribers = new Dictionary<string, Action<string>>();
		sendQueue = new Queue<string>();

		remoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
		receiveEndPoint = new IPEndPoint(IPAddress.Parse(ip), receivePort);

		sendClient = new UdpClient();
		receiveClient = new UdpClient(receivePort);

		receiveThread = new Thread(new ThreadStart(ReceiveDataListener));
		receiveThread.IsBackground = true;
		receiveThread.Start();

		isInitialized = true;
	}

	public static void Receive(string key, Action<float> callback)
	{
		if (instance.floatSubscribers.ContainsKey(key))
			instance.floatSubscribers[key] = callback;
		else
			instance.floatSubscribers.Add(key, callback);
	}

	public static void Receive(string key, Action<string> callback)
	{
		if (instance.floatSubscribers.ContainsKey(key))
			instance.stringSubscribers[key] = callback;
		else
			instance.stringSubscribers.Add(key, callback);
	}

	public static void Remove(string key, Action<string> callback)
	{
		if (instance.stringSubscribers.ContainsKey(key))
			instance.stringSubscribers.Remove(key);
		else
			DreamLogs.LogWarning("<color=#FFFF00>There's no callback subscribed to '{0}'</color>", key);
	}

	public static void Remove(string key, Action<float> callback)
	{
		if (instance.floatSubscribers.ContainsKey(key))
			instance.floatSubscribers.Remove(key);
		else
			DreamLogs.LogWarning("<color=#FFFF00>There's no callback subscribed to '{0}'</color>", key);
	}

	public static void SendImmediatly(string key, float value, int port = 0)
	{
		string message = string.Format("{0} {1}", key, value);
		instance.SendStringImmediatly(message, port);
	}

	public static void Send(string key, float value)
	{
		string message = string.Format("{0} {1}", key, value);
		instance.SendString(message);
	}

	public static void Send(string key, string value)
	{
		string message = string.Format("{0} {1}", key, value);
		instance.SendString(message);
	}

	private void SendString(string message)
	{
		sendQueue.Enqueue(message);
		if (!isSendingQueue)
			StartCoroutine(SendQueue());
	}

	private void SendStringImmediatly(string message, int port)
	{
		try
		{
			DreamLogs.Log("<color=red>Sending data immediatly: {0}</color>", message);
			byte[] data = Encoding.UTF8.GetBytes(message);

			if (port > 0)
				sendClient.Send(data, data.Length, new IPEndPoint(IPAddress.Parse(ip), port));
			else
				sendClient.Send(data, data.Length, remoteEndPoint);
		}
		catch (Exception err)
		{
			Debug.LogWarning(err.ToString());
		}
	}

	private IEnumerator SendQueue()
	{
		isSendingQueue = true;
		while (sendQueue.Count > 0)
		{
			string message = sendQueue.Dequeue();
			try
			{
				DreamLogs.Log("<color=blue>Sending data by queue: {0}</color>", message);
				byte[] data = Encoding.UTF8.GetBytes(message);

				sendClient.Send(data, data.Length, remoteEndPoint);
			}
			catch (Exception err)
			{
				Debug.LogWarning(err.ToString());
			}
			yield return new WaitForSeconds(0.05f);
		}

		isSendingQueue = false;
		if (onCompleteQueue != null)
			onCompleteQueue();
	}

	private void ReceiveDataListener()
	{
		while (true)
		{
			try
			{
				byte[] data = receiveClient.Receive(ref receiveEndPoint);
				string text = Encoding.UTF8.GetString(data);

				SerializeMessage(text);
			}
			catch (Exception ex)
			{
				DreamLogs.LogWarning(ex.ToString());
			}
		}
	}

	private void SerializeMessage(string message)
	{
		try
		{
			string[] chain = message.Split(' ');
			string key = chain[0];
			float value = 0;

			if (float.TryParse(chain[1], out value))
			{
				Action<float> callback = null;
				if (floatSubscribers.TryGetValue(key, out callback))
					callback(value);
				else
				{
					if (!logException.Contains(key))
						DreamLogs.LogWarning("<color=#FFFF00>There's no callback subscribed to '{0}'</color>", key);
				}
			}
			else
			{
				Action<string> callback = null;
				if (stringSubscribers.TryGetValue(key, out callback))
					callback(chain[1]);
				else
				{
					if (!logException.Contains(key))
						DreamLogs.LogWarning("<color=#FFFF00>There's no callback subscribed to '{0}'</color>", key);
				}
			}

			if (!logException.Contains(key))
				DreamLogs.Log("<color=green>Receive data: {0}</color>", message);
		}
		catch (Exception e)
		{
			DreamLogs.LogWarning("<color=#FFFF00>Cannot deserialize message '{0}', reason: {1}</color>", message, e.Message);
		}
	}

	private void OnDestroy()
	{
		TryKillThread();
	}

	private void OnApplicationQuit()
	{
		TryKillThread();
	}

	private void TryKillThread()
	{
		if (isInitialized)
		{
			receiveThread.Abort();
			receiveThread = null;

			sendClient.Close();
			sendClient = null;

			receiveClient.Close();
			receiveClient = null;

			DreamLogs.Log("Thread killed: {0}", receiveThread);

			onCompleteQueue = null;
			isInitialized = false;
		}
	}
}