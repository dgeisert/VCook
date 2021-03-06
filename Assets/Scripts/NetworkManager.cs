﻿using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using VRTK;
using TMPro;
using UnityEngine.SceneManagement;

public enum InterpretationType {
	Ping = 0,
	Voice = 1,
	PlayerObjects = 2,
	CreateObject = 3,
	GrabObject = 4,
	ReleaseObject = 5,
	DestroyObject = 6,
	UpdateObjects = 7,
    SceneChange = 8,
    UseObject = 9,
}

public class NetworkManager : MonoBehaviour {

	protected Callback<GameOverlayActivated_t> Callback_GameOverlayActivated;
	protected Callback<LobbyCreated_t> Callback_lobbyCreated;
	protected Callback<LobbyMatchList_t> Callback_lobbyList;
	protected Callback<LobbyEnter_t> Callback_lobbyEnter;
	protected Callback<LobbyDataUpdate_t> Callback_lobbyInfo;
	protected Callback<GameLobbyJoinRequested_t> Callback_joinLobby;
	protected Callback<P2PSessionRequest_t> Callback_p2PSessionRequest;
	private List<ulong> ExpectingClient = new List<ulong>();
	private CSteamID lobbyID;
	private bool initialized = false;
	public Dictionary<string, ItemMachine> allObjects;
	public static NetworkManager instance;
	public CSteamID host;
    public bool isHost = false;
	public Dictionary<ulong, OtherPlayerObject> otherPlayers = new Dictionary<ulong, OtherPlayerObject>();
	public GameObject otherPlayerObject;
	float lazyUpdateTimer, objectUpdateTimer;
	Dictionary<string, float> timestamps = new Dictionary<string, float>();

	void Awake(){
		allObjects = new Dictionary<string, ItemMachine>();
		instance = this;
	}

	// Use this for initialization
	void Start () {
		if(SteamManager.Initialized && !initialized) {
			SteamAPI.Init ();
			Callback_GameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnGameOverlayActivated);
			Callback_lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
			Callback_lobbyList = Callback<LobbyMatchList_t>.Create(OnLobbyList);
			Callback_lobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEnter);
			Callback_lobbyInfo = Callback<LobbyDataUpdate_t>.Create(OnLobbyInfo);
			Callback_joinLobby = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
			Callback_p2PSessionRequest = Callback<P2PSessionRequest_t>.Create(OnP2PSessionRequest);
		}
		lazyUpdateTimer = Time.time;
		objectUpdateTimer = Time.time;
		SceneManager.sceneLoaded += InitObjectsInScene;
	}

	void InitObjectsInScene(Scene scene, LoadSceneMode mode){
		foreach(ItemMachine im in GameObject.FindObjectsOfType<ItemMachine>())
		{
			im.Init();
		}
	}

	public void Update(){
		if (SteamManager.Initialized) {
			SteamAPI.RunCallbacks ();
			if (Input.GetKeyDown (KeyCode.S)) {
				SetupServer ();
			}
			if (Input.GetKeyDown (KeyCode.I)) {
                InviteFriend();
			}
			ReadPackets ();
			SendMyPosition ();
			Talk ();
            if (IsHost())
			{
				if (objectUpdateTimer + 0.1f < Time.time) {
					objectUpdateTimer = Time.time;
					SendObjectUpdate();
				}
			}
			if (lazyUpdateTimer + 3 < Time.time) {
				lazyUpdateTimer = Time.time;
				CheckLobby ();
			}
		}
	}

	public void CheckLobby(){
		if (SteamManager.Initialized) {
			GameObject lobbyText = GameObject.Find ("Lobby");
			string lobbyPeople = "";
			List<ulong> inLobby = new List<ulong> ();
			int lobbyCount = SteamMatchmaking.GetNumLobbyMembers (lobbyID);
			for (int i = 0; i < lobbyCount; i++) {
				CSteamID csid = SteamFriends.GetFriendFromSourceByIndex (lobbyID, i);
				inLobby.Add (csid.m_SteamID);
				if (!ExpectingClient.Contains (csid.m_SteamID) && !(SteamUser.GetSteamID ().m_SteamID == csid.m_SteamID)) {
					NewConnection (csid.m_SteamID);
				}
				if (lobbyText != null) {
					TextMeshPro tmp = lobbyText.GetComponent<TextMeshPro> ();
					if (tmp != null) {
						lobbyPeople += SteamFriends.GetFriendPersonaName (csid) + '\n';
					}
				}
			}
			if (lobbyText != null) {
				TextMeshPro tmp = lobbyText.GetComponent<TextMeshPro> ();
				if (tmp != null) {
					tmp.text = lobbyPeople;
				}
			}
			List<ulong> endConnections = new List<ulong> ();
			for (int j = 0; j < ExpectingClient.Count; j++) {
				if (!inLobby.Contains (ExpectingClient [j])) {
					endConnections.Add (ExpectingClient [j]);
				}
			}
			foreach (ulong csid in endConnections) {
				EndConnection (csid);
			}
		}
	}

	public void ReadPackets(){
		uint size;
		while (SteamNetworking.IsP2PPacketAvailable(out size))
		{
			var buffer = new byte[size];
			uint bytesRead;
			CSteamID remoteId;
			if (SteamNetworking.ReadP2PPacket(buffer, size, out bytesRead, out remoteId))
			{
				byte[] timestampBytes = new byte[4];
				Array.Copy (buffer, 0, timestampBytes, 0, 4);
				float timestamp = ByteToFloatArray (timestampBytes) [0];
				byte[] dataIn = new byte[size - 4];
				Array.Copy (buffer, 4, dataIn, 0, size - 4);
				while(dataIn.Length > 2)
				{
					int dataType = (int)dataIn[0];
					byte[] dataIn2 = new byte[dataIn.Length - 1];
					Array.Copy(dataIn, 1, dataIn2, 0, dataIn2.Length);
					DeconstructPacket(dataIn2, timestamp, dataType, remoteId);
					dataIn = new byte[0];
				}
			}
		}
	}

	byte[] DeconstructPacket(byte[] dataIn, float timestamp, int dataType, CSteamID remoteId)
	{
		byte[] returnBytes = new byte[0];
		switch (dataType)
		{
		case 0://ping
			GetPing(timestamp, dataIn, remoteId);
			break;
		case 1://voice
			Listen(timestamp, dataIn, remoteId);
			break;
		case 2://person update, head and hands
			ParseUpdatePerson(timestamp, dataIn, remoteId);
			break;
		case 3://instantiate object
			ParseInstantiateObject(timestamp, dataIn, remoteId);
			break;
		case 4://other player grabs object
			ParseGrabObject(timestamp, dataIn, remoteId);
			break;
		case 5://other player releases object
			ParseReleaseObject(timestamp, dataIn, remoteId);
			break;
		case 6://destroy object
			ParseDestroyObject(timestamp, dataIn, remoteId);
			break;
		case 7://update objects
			ParseUpdateObjects(timestamp, dataIn, remoteId);
			break;
        case 8://scene change
            ParseSceneChange(timestamp, dataIn, remoteId);
            break;
        case 9://use object
            ParseUseObject(timestamp, dataIn, remoteId);
            break;
            default:
			Debug.Log("Unknown Data");
			break;
		}
		return returnBytes;
	}
	void GetPing(float timestamp, byte[] dataIn, CSteamID remoteId){
		Debug.Log("ping");
		if (!ExpectingClient.Contains(remoteId.m_SteamID))
		{
			ExpectingClient.Add(remoteId.m_SteamID);
		}
	}

	void Talk(){
		uint voiceBytes;
		uint voice2Bytes;
		EVoiceResult voiceResult = SteamUser.GetAvailableVoice (out voiceBytes, out voice2Bytes, 0);
		if (voiceBytes > 0) {
			byte[] voice = new byte[voiceBytes];
			byte[] voice2 = new byte[voiceBytes + 1];
			SteamUser.GetVoice (true, voice, voiceBytes, out voiceBytes, false, voice2, 0, out voice2Bytes, 0);
			voice.CopyTo (voice2, 1);
			voice2 [0] = (byte)1;
			SendBytes (voice2);
		}
	}
	void Listen(float timestamp, byte[] dataIn, CSteamID remoteId){
		if (otherPlayers.ContainsKey(remoteId.m_SteamID))
		{
			if (otherPlayers[remoteId.m_SteamID] == null)
			{
                CreateOtherPlayer(remoteId, timestamp);
            }
		}
		else
		{
            CreateOtherPlayer(remoteId, timestamp);
        }
        if (!timestamps.ContainsKey(remoteId.m_SteamID.ToString() + ((int)InterpretationType.PlayerObjects).ToString()))
        {
            timestamps.Add(remoteId.m_SteamID.ToString() + ((int)InterpretationType.PlayerObjects).ToString(), timestamp);
        }
        byte[] bufferOut = new byte[22050];
		uint bytesOut;
		EVoiceResult voiceOut = SteamUser.DecompressVoice(dataIn, (uint)dataIn.Length, bufferOut, 22050, out bytesOut, 11025);
		float[] audio = new float[Mathf.FloorToInt(bytesOut/2)];
		for (int i = 0; i < audio.Length; ++i)
		{
			audio[i] = (short)(bufferOut[i * 2] | bufferOut[i * 2 + 1] << 8) / 32768.0f;
		}
        otherPlayers[remoteId.m_SteamID].PlayAudio(audio, timestamp);
	}


	void SendMyPosition(){
        if(VRTK.VRTK_SDKManager.instance == null)
        {
            return;
        }
		byte[] bytes = ItemsPositionBytes(new List<Transform> { 
            VRTK.VRTK_SDKManager.instance.actualHeadset.transform,
            VRTK.VRTK_SDKManager.instance.actualRightController.transform,
            VRTK.VRTK_SDKManager.instance.actualLeftController.transform
		});
		byte[] bytes2 = new byte[bytes.Length + 1];
		Array.Copy (bytes, 0, bytes2, 1, bytes.Length);
		bytes2 [0] = (byte)InterpretationType.PlayerObjects;

		SendBytes (bytes2);
	}
	void ParseUpdatePerson(float timestamp, byte[] dataIn, CSteamID remoteId){
		if (timestamps.ContainsKey(remoteId.m_SteamID.ToString() + ((int)InterpretationType.PlayerObjects).ToString()))
		{
			if (timestamps[remoteId.m_SteamID.ToString() + ((int)InterpretationType.PlayerObjects).ToString()] > timestamp)
			{
				return;
			}
		}
		else
		{
			timestamps.Add(remoteId.m_SteamID.ToString() + ((int)InterpretationType.PlayerObjects).ToString(), timestamp);
		}
		if (otherPlayers.ContainsKey(remoteId.m_SteamID))
		{
			if (otherPlayers[remoteId.m_SteamID] == null)
			{
                CreateOtherPlayer(remoteId, timestamp);
            }
		}
		else
		{
			CreateOtherPlayer(remoteId, timestamp);
		}
		otherPlayers[remoteId.m_SteamID].InterpretLocation(ByteToFloatArray(dataIn));
	}

	void CreateOtherPlayer(CSteamID csid, float timestamp)
	{
		GameObject go = (GameObject)GameObject.Instantiate(otherPlayerObject);
		otherPlayers[csid.m_SteamID] = go.GetComponent<OtherPlayerObject>();
		go.GetComponent<OtherPlayerObject>().Init(timestamp);
	}

	public void SendInstantiateObject(ItemMachine im){
		byte[] rbBytes = RigidbodyBytes(new List<Rigidbody> { im.rb });
        byte[] nameBytes = StringToByte(im.itemName);
        byte[] idBytes = StringToByte(im.itemID);
        byte[] bytes = new byte[1 + rbBytes.Length + nameBytes.Length + idBytes.Length];
        bytes[0] = (byte)((int)InterpretationType.CreateObject);
        Array.Copy(idBytes, 0, bytes, 1, idBytes.Length);
        Array.Copy(rbBytes, 0, bytes, 1 + idBytes.Length, rbBytes.Length);
        Array.Copy(nameBytes, 0, bytes, 1 + idBytes.Length + rbBytes.Length, nameBytes.Length);
        SendBytesReliable(bytes);
	}
	void ParseInstantiateObject(float timestamp, byte[] dataIn, CSteamID remoteId)
    {//add rb parsing
        byte[] instantiateChar = new byte[10 * sizeof(char)];
        byte[] instantiateFloat = new byte[4 * 13];
        byte[] instantiateName = new byte[dataIn.Length - instantiateFloat.Length - instantiateChar.Length];
        Array.Copy(dataIn, 0, instantiateChar, 0, instantiateChar.Length);
        Array.Copy(dataIn, instantiateChar.Length, instantiateFloat, 0, instantiateFloat.Length);
		Array.Copy(dataIn, instantiateChar.Length + instantiateFloat.Length, instantiateName, 0, instantiateName.Length);
		string itemName = ByteToString(instantiateName);
		if (allObjects.ContainsKey (itemName)) {
			return;
		}
        float[] f = ByteToFloatArray(instantiateFloat);
		Vector3 pos = new Vector3(f[0], f[1], f[2]);
		Quaternion quat = new Quaternion(f[3], f[4], f[5], f[6]);
        if (RecipeManager.instance.itemList.ContainsKey(itemName))
        {
            PlayerMachine.instance.CreateItem(RecipeManager.instance.itemList[itemName].gameObject, pos, quat, false, null, ByteToString(instantiateChar), !IsHost());
        }
        else if(TransformationManager.instance.itemList.ContainsKey(itemName))
        {
            PlayerMachine.instance.CreateItem(TransformationManager.instance.itemList[itemName].gameObject, pos, quat, false, null, ByteToString(instantiateChar), !IsHost());
        }
		Debug.Log("Instantiate: " + itemName);
	}

	public void SendGrabObject(ItemMachine im, int hand = 1){
		byte[] stringBytes = StringToByte (im.itemID);
        Debug.Log("Sending Grab " + im.itemID);
		byte[] bytes = new byte[stringBytes.Length + 2];
		bytes[0] = (byte)((int)InterpretationType.GrabObject);
		bytes[1] = (byte)((int)hand);
		Array.Copy (stringBytes, 0, bytes, 2, stringBytes.Length);
		SendBytes (bytes);
	}
	void ParseGrabObject(float timestamp, byte[] dataIn, CSteamID remoteId){
		byte[] grabBytes = new byte[dataIn.Length - 1];
        Array.Copy(dataIn, 1, grabBytes, 0, grabBytes.Length);
        Debug.Log("Grab: " + ByteToString(grabBytes));
        otherPlayers[remoteId.m_SteamID].GrabObject(allObjects[ByteToString(grabBytes)], (int)dataIn[0]);
    }

    public void SendReleaseObject(ItemMachine im)
    {
        byte[] rbBytes = NetworkManager.instance.RigidbodyBytes(new List<Rigidbody> { im.rb });
        byte[] stringBytes = StringToByte(im.itemID);
        byte[] bytes = new byte[rbBytes.Length + stringBytes.Length + 1];
        bytes[0] = (byte)((int)InterpretationType.ReleaseObject);
        Array.Copy(stringBytes, 0, bytes, 1, stringBytes.Length);
        Array.Copy(rbBytes, 0, bytes, 1 + stringBytes.Length, rbBytes.Length);
        SendBytesReliable(bytes);
    }
    void ParseReleaseObject(float timestamp, byte[] dataIn, CSteamID remoteId)
    {
        byte[] releaseBytesID = new byte[sizeof(char) * 10];
        byte[] releaseFloatBytes = new byte[4 * 13];
        Array.Copy(dataIn, 0, releaseBytesID, 0, releaseBytesID.Length);
        Array.Copy(dataIn, releaseBytesID.Length, releaseFloatBytes, 0, releaseFloatBytes.Length);
        Debug.Log("Release: " + ByteToString(releaseBytesID));
        float[] releaseFloats = ByteToFloatArray(releaseFloatBytes);
        Vector3 relPos = new Vector3(releaseFloats[0], releaseFloats[1], releaseFloats[2]);
        Quaternion relRot = new Quaternion(releaseFloats[3], releaseFloats[4], releaseFloats[5], releaseFloats[6]);
        Vector3 relVel = new Vector3(releaseFloats[7], releaseFloats[8], releaseFloats[9]);
        Vector3 relAngVel = new Vector3(releaseFloats[10], releaseFloats[11], releaseFloats[12]);
        otherPlayers[remoteId.m_SteamID].ReleaseObject(allObjects[ByteToString(releaseBytesID)], relPos, relRot, relVel, relAngVel);
    }

    public void SendUseObject(ItemMachine im, int hand = 1)
    {
        byte[] stringBytes = StringToByte(im.itemID);
        Debug.Log("Sending Use " + im.itemID);
        byte[] bytes = new byte[stringBytes.Length + 1];
        bytes[0] = (byte)((int)InterpretationType.UseObject);
        Array.Copy(stringBytes, 0, bytes, 1, stringBytes.Length);
        SendBytes(bytes);
    }
    void ParseUseObject(float timestamp, byte[] dataIn, CSteamID remoteId)
    {
        Debug.Log("Use: " + ByteToString(dataIn));
        allObjects[ByteToString(dataIn)].Use();
    }

    public void DestroyObject(ItemMachine im)
    {
        byte[] stringBytes = StringToByte(im.itemID);
        byte[] bytes = new byte[stringBytes.Length + 1];
        Array.Copy(stringBytes, 0, bytes, 1, stringBytes.Length);
        bytes[0] = (byte)((int)InterpretationType.DestroyObject);
        SendBytesReliable(bytes);
        if (allObjects.ContainsKey(im.itemID))
        {
            allObjects.Remove(im.itemID);
        }
        Destroy(im.gameObject);
    }
	void ParseDestroyObject(float timestamp, byte[] dataIn, CSteamID remoteId){
		string itemID = ByteToString(dataIn);
        Debug.Log("Destroy: " + itemID);
        if (allObjects.ContainsKey(itemID))
        {
            Destroy(allObjects[itemID].gameObject);
            allObjects.Remove(itemID);
        }
	}

    public void SendObjectUpdate()
    {
        List<byte[]> rbsBytes = new List<byte[]>();
        List<string> removes = new List<string>();
        List<byte[]> strBytes = new List<byte[]>();
        List<byte> phaseBytes = new List<byte>();
        foreach (KeyValuePair<string, ItemMachine> kvp in allObjects)
        {
            if(kvp.Value != null)
            {
				rbsBytes.Add (RigidbodyBytes (new List<Rigidbody> { kvp.Value.rb }));
                strBytes.Add(StringToByte(kvp.Value.itemID));
                phaseBytes.Add((byte) kvp.Value.phase);
            }
            else
            {
                removes.Add(kvp.Key);
            }
        }
        foreach(string str in removes)
        {
            allObjects.Remove(str);
        }
        byte[] bytes = new byte[rbsBytes.Count * (sizeof(char) * 10 + 13 * 4 + 1) + 1];
        for(int i = 0; i < rbsBytes.Count; i++)
        {
            Array.Copy(strBytes[i], 0, bytes, 1 + i * (sizeof(char) * 10 + 13 * 4 + 1), sizeof(char) * 10);
            Array.Copy(rbsBytes[i], 0, bytes, 1 + i * (sizeof(char) * 10 + 13 * 4 + 1) + sizeof(char) * 10, 13 * 4);
            bytes[(i + 1) * (sizeof(char) * 10 + 13 * 4)] = phaseBytes[i];
        }
        bytes[0] = (byte)((int)InterpretationType.UpdateObjects);
        SendBytes(bytes);
    }
	void ParseUpdateObjects(float timestamp, byte[] dataIn, CSteamID remoteId){
		if (timestamps.ContainsKey(remoteId.m_SteamID.ToString() + ((int)InterpretationType.UpdateObjects).ToString()))
		{
			if (timestamps[remoteId.m_SteamID.ToString() + ((int)InterpretationType.UpdateObjects).ToString()] > timestamp)
			{
				return;
			}
		}
		else
		{
			timestamps.Add(remoteId.m_SteamID.ToString() + ((int)InterpretationType.UpdateObjects).ToString(), timestamp);
		}
        for(int i = 0; i < (dataIn.Length / (sizeof(char) * 10 + 13 * 4)); i++)
        {
            byte[] idBytes = new byte[sizeof(char) * 10];
            byte[] rbBytes = new byte[13 * 4];
            Array.Copy(dataIn, i * (sizeof(char) * 10 + 13 * 4 + 1), idBytes, 0, idBytes.Length);
            Array.Copy(dataIn, i * (sizeof(char) * 10 + 13 * 4 + 1) + sizeof(char) * 10, rbBytes, 0, rbBytes.Length);
            int phase = (int) dataIn[(i + 1) * (sizeof(char) * 10 + 13 * 4 + 1) - 1];
            string itemID = ByteToString(idBytes);
            if (allObjects.ContainsKey(itemID))
            {
                float[] f = ByteToFloatArray(rbBytes);
                if (allObjects[itemID] != null)
                {
					if (allObjects [itemID].GetComponentsInParent<OtherPlayerObject> () == null) {
						allObjects [itemID].SetRB (
							new Vector3 (f [0], f [1], f [2]),
							new Quaternion (f [3], f [4], f [5], f [6]),
							new Vector3 (f [7], f [8], f [9]),
							new Vector3 (f [10], f [11], f [12])
						);
					}
                }
            }
        }
	}

	public void SendSceneChange(string scene)
	{
		byte[] stringBytes = StringToByte(scene);
		byte[] bytes = new byte[stringBytes.Length + 1];
		Array.Copy(stringBytes, 0, bytes, 1, stringBytes.Length);
		bytes[0] = (byte)((int)InterpretationType.SceneChange);
		SendBytesReliable(bytes);
        SceneManager.LoadSceneAsync(scene);
    }
	void ParseSceneChange(float timestamp, byte[] dataIn, CSteamID remoteId){
		string scene = ByteToString(dataIn);
		Debug.Log("Load Scene: " + scene);
		SceneManager.LoadSceneAsync (scene);
	}


	/*
	 * This section is sending the bytes, either quickly or reliably
	 * 
	 */

	public void SendBytes(byte[] bytes){
		byte[] toSend = new byte[bytes.Length + 4];
		Array.Copy (bytes, 0, toSend, 4, bytes.Length);
		Array.Copy(FloatToByteArray(new float[] {Time.time}) , 0, toSend, 0, 4);
		foreach (ulong csid in ExpectingClient) {
			SteamNetworking.SendP2PPacket(new CSteamID(csid), toSend, (uint)toSend.Length, EP2PSend.k_EP2PSendUnreliableNoDelay);
		}
	}

	public void SendBytesReliable(byte[] bytes){
		byte[] toSend = new byte[bytes.Length + 4];
		Array.Copy (bytes, 0, toSend, 4, bytes.Length);
		Array.Copy(FloatToByteArray(new float[] {Time.time}) , 0, toSend, 0, 4);
		foreach (ulong csid in ExpectingClient) {
			SteamNetworking.SendP2PPacket(new CSteamID(csid), toSend, (uint)toSend.Length, EP2PSend.k_EP2PSendReliable);
		}
	}



	/*
	 * This section is managing the steam lobby
	 * 
	 */
	public void OnGameOverlayActivated(GameOverlayActivated_t overlay){

	}

	public void OnLobbyCreated(LobbyCreated_t lobbyCreated){
		host = SteamUser.GetSteamID ();
        isHost = true;
		lobbyID = new CSteamID (lobbyCreated.m_ulSteamIDLobby);
		Debug.Log ("Lobby Created");
	}
	public bool IsHost(){
        return isHost;
	}
	public bool IsInLobby(){
		return lobbyID.IsLobby();
	}

	public void OnLobbyList(LobbyMatchList_t lobbyList){
		Debug.Log ("Lobby List");
	}

	public void OnLobbyEnter(LobbyEnter_t lobbyEnter){
		Debug.Log ("Lobby Entered");
		lobbyID = new CSteamID (lobbyEnter.m_ulSteamIDLobby);
        SteamUser.StartVoiceRecording();
        CheckLobby ();
	}

	public void OnLobbyInfo(LobbyDataUpdate_t lobbyInfo){
		Debug.Log ("Lobby Info");
	}
	void NewConnection(ulong csid){
		ExpectingClient.Add (csid);
	}
	void EndConnection(ulong csid){
		ExpectingClient.Remove (csid);
		Destroy (otherPlayers [csid].gameObject);
		otherPlayers.Remove (csid);
	}

	public void OnJoinRequest(GameLobbyJoinRequested_t joinRequest){
		Debug.Log ("Requested to join lobby");
		JoinLobby (joinRequest.m_steamIDLobby);
	}

	void OnP2PSessionRequest(P2PSessionRequest_t request)
	{
		CSteamID clientId = request.m_steamIDRemote;
		if (ExpectingClient.Contains(clientId.m_SteamID))
		{
			SteamNetworking.AcceptP2PSessionWithUser(clientId);
		} else {
			Debug.LogWarning("Unexpected session request from " + clientId);
		}
	}

	public void SetupServer()
	{
		if (SteamManager.Initialized) {
			CreateLobby (ELobbyType.k_ELobbyTypeFriendsOnly, 2);
		}
	}

    public void InviteFriend()
    {
        SteamFriends.ActivateGameOverlayInviteDialog(lobbyID);
    }

	SteamAPICall_t CreateLobby (ELobbyType eLobbyType, int maxMembers){
		return SteamMatchmaking.CreateLobby (eLobbyType, maxMembers);
	}

	SteamAPICall_t JoinLobby (CSteamID lobby){
		return SteamMatchmaking.JoinLobby (lobby);
	}

	bool InviteUserToLobby(CSteamID lobby, CSteamID invitee){
		return false;
	}

	/*
	 * This section is for converting bytes and other primative types
	 * 
	 */
	public void SendStringFloat(string str, float[] floatArray, InterpretationType mType){
		byte[] bytes = StringToByte (str);
		byte[] bytes2 = FloatToByteArray (floatArray);
		byte[] bytes3 = new byte[bytes.Length + bytes2.Length + 1];
		Array.Copy (bytes, 0, bytes3, 1, bytes.Length);
		Array.Copy (bytes2, 0, bytes3, 1 + bytes.Length, bytes2.Length);
		bytes3 [0] = (byte)((int)mType);
		SendBytes (bytes3);
	}
	public void SendString(string str, InterpretationType mType){
		byte[] bytes = StringToByte (str);
		byte[] bytes2 = new byte[bytes.Length + 1];
		Array.Copy (bytes, 0, bytes2, 1, bytes.Length);
		bytes2 [0] = (byte)((int)mType);
		SendBytes (bytes2);
	}
	byte[] FloatToByteArray(float[] floatArray) {
		int len = floatArray.Length * 4;
		byte[] byteArray = new byte[len];
		int pos = 0;
		foreach (float f in floatArray) {
			byte[] data = System.BitConverter.GetBytes(f);
			System.Array.Copy(data, 0, byteArray, pos, 4);
			pos += 4;
		}
		return byteArray;
	}
	float[] ByteToFloatArray(byte[] byteArray) {
		int len = byteArray.Length / 4;
		float[] floatArray = new float[len];
		for (int i = 0; i < byteArray.Length; i+=4) {
			floatArray[i/4] = System.BitConverter.ToSingle(byteArray, i);
		}
		return floatArray;
	}
	string ByteToString(byte[] byteArray){
		char[] chars = new char[byteArray.Length / sizeof(char)];
		System.Buffer.BlockCopy(byteArray, 0, chars, 0, byteArray.Length);
		return new string(chars, 0, chars.Length);
	}
	byte[] StringToByte(string str){
		byte[] bytes = new byte[str.Length * sizeof(char)];
		System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
		return bytes;
	}
	public byte[] ItemsPositionBytes(List<Transform> trs){
		float[] f = new float[7*trs.Count];
		for (int i = 0; i < trs.Count; i++) {
            if (trs[i] != null)
            {
                f[0 + i * 7] = trs[i].position.x;
                f[1 + i * 7] = trs[i].position.y;
                f[2 + i * 7] = trs[i].position.z;
				f[3 + i * 7] = trs[i].rotation.x;
				f[4 + i * 7] = trs[i].rotation.y;
				f[5 + i * 7] = trs[i].rotation.z;
				f[6 + i * 7] = trs[i].rotation.w;
            }
            else
            {
                f[0 + i * 7] = 0;
                f[1 + i * 7] = 0;
                f[2 + i * 7] = 0;
                f[3 + i * 7] = 0;
                f[4 + i * 7] = 0;
                f[5 + i * 7] = 0;
                f[6 + i * 7] = 0;
            }
		}
		return FloatToByteArray (f);
	}
	public byte[] RigidbodyBytes(List<Rigidbody> rbs){
		float[] f = new float[13*rbs.Count];
		for (int i = 0; i < rbs.Count; i++) {
			f [0 + i*13] = rbs[i].position.x;
			f [1 + i*13] = rbs[i].position.y;
			f [2 + i*13] = rbs[i].position.z;
			f [3 + i*13] = rbs[i].rotation.x;
			f [4 + i*13] = rbs[i].rotation.y;
			f [5 + i*13] = rbs[i].rotation.z;
			f [6 + i*13] = rbs[i].rotation.w;
			f [7 + i*13] = rbs[i].velocity.x;
			f [8 + i*13] = rbs[i].velocity.y;
			f [9 + i*13] = rbs[i].velocity.z;
			f [10 + i*13] = rbs[i].angularVelocity.x;
			f [11 + i*13] = rbs[i].angularVelocity.y;
			f [12 + i*13] = rbs[i].angularVelocity.z;
		}
		return FloatToByteArray (f);
	}
}
