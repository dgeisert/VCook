using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public enum InterpretationType {
	Voice = 1,
	PlayerObjects = 2,
	CreateObject = 3,
	GrabObject = 4,
	DestroyObject = 5,
	UpdateObjects = 6,
	Ping = 7
}

public class NetworkManager : MonoBehaviour {

	protected Callback<GameOverlayActivated_t> Callback_GameOverlayActivated;
	protected Callback<LobbyCreated_t> Callback_lobbyCreated;
	protected Callback<LobbyMatchList_t> Callback_lobbyList;
	protected Callback<LobbyEnter_t> Callback_lobbyEnter;
	protected Callback<LobbyDataUpdate_t> Callback_lobbyInfo;
	protected Callback<GameLobbyJoinRequested_t> Callback_joinLobby;
	protected Callback<P2PSessionRequest_t> Callback_p2PSessionRequest;
	private List<CSteamID> ExpectingClient = new List<CSteamID>();
	private CSteamID lobbyID;
	private bool initialized = false;
	public Dictionary<string, ItemMachine> allObjects;
	public static NetworkManager instance;
	public CSteamID host;
	public Dictionary<CSteamID, OtherPlayerObject> otherPlayers = new Dictionary<CSteamID, OtherPlayerObject>();
	public GameObject otherPlayerObject;

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
			f [0 + i*7] = trs[i].position.x;
			f [1 + i*7] = trs[i].position.y;
			f [2 + i*7] = trs[i].position.z;
			f [3 + i*7] = trs[i].rotation.w;
			f [4 + i*7] = trs[i].rotation.x;
			f [5 + i*7] = trs[i].rotation.y;
			f [6 + i*7] = trs[i].rotation.z;
		}
		return FloatToByteArray (f);
	}
	public byte[] RigidbodyBytes(List<Rigidbody> rbs){
		float[] f = new float[13*rbs.Count];
		for (int i = 0; i < rbs.Count; i++) {
			f [0 + i*13] = rbs[i].position.x;
			f [1 + i*13] = rbs[i].position.y;
			f [2 + i*13] = rbs[i].position.z;
			f [3 + i*13] = rbs[i].rotation.w;
			f [4 + i*13] = rbs[i].rotation.x;
			f [5 + i*13] = rbs[i].rotation.y;
			f [6 + i*13] = rbs[i].rotation.z;
			f [7 + i*13] = rbs[i].velocity.x;
			f [8 + i*13] = rbs[i].velocity.y;
			f [9 + i*13] = rbs[i].velocity.z;
			f [10 + i*13] = rbs[i].angularVelocity.x;
			f [11 + i*13] = rbs[i].angularVelocity.y;
			f [12 + i*13] = rbs[i].angularVelocity.z;
		}
		return FloatToByteArray (f);
	}

	public void SendBytes(byte[] bytes){
		foreach (CSteamID csid in ExpectingClient) {
			SteamNetworking.SendP2PPacket(csid, bytes, (uint) bytes.Length, EP2PSend.k_EP2PSendUnreliableNoDelay);
		}
	}

	public void SendBytesReliable(byte[] bytes){
		foreach (CSteamID csid in ExpectingClient) {
			Debug.Log("Sending to: " + SteamFriends.GetFriendPersonaName (csid));
			SteamNetworking.SendP2PPacket(csid, bytes, (uint) bytes.Length, EP2PSend.k_EP2PSendReliable);
		}
	}

	public void OnGameOverlayActivated(GameOverlayActivated_t overlay){

	}

	public void OnLobbyCreated(LobbyCreated_t lobbyCreated){
		host = SteamUser.GetSteamID ();
		lobbyID = new CSteamID (lobbyCreated.m_ulSteamIDLobby);
		Debug.Log ("Lobby Created");
	}
	public bool IsHost(){
		return SteamUser.GetSteamID () == host;
	}

	public void OnLobbyList(LobbyMatchList_t lobbyList){
		Debug.Log ("Lobby List");
	}

	public void OnLobbyEnter(LobbyEnter_t lobbyEnter){
		Debug.Log ("Lobby Entered");
		PlayerMachine.instance.chatAudio.clip = AudioClip.Create ("chat", 11025, 1, 11025, false, true);
		lobbyID = new CSteamID (lobbyEnter.m_ulSteamIDLobby);
		CheckLobby ();
	}

	public void OnLobbyInfo(LobbyDataUpdate_t lobbyInfo){
		CheckLobby ();
		Debug.Log ("Lobby Info");
		SteamUser.StartVoiceRecording ();
	}

	public void CheckLobby(){
		ExpectingClient = new List<CSteamID> ();
		int lobbyCount = SteamMatchmaking.GetNumLobbyMembers (lobbyID);
		for (int i = 0; i < lobbyCount; i++) {
			CSteamID csid = SteamFriends.GetFriendFromSourceByIndex (lobbyID, i);
			if (!ExpectingClient.Contains (csid) && !(SteamUser.GetSteamID ().m_SteamID == csid.m_SteamID)) {
				ExpectingClient.Add(SteamFriends.GetFriendFromSourceByIndex (lobbyID, i));
			}
		}
		foreach (CSteamID csid in ExpectingClient) {
			Debug.Log (SteamFriends.GetFriendPersonaName (csid));
		}
		SendBytesReliable (new byte[] { (byte)7});
	}

	public void OnJoinRequest(GameLobbyJoinRequested_t joinRequest){
		Debug.Log ("Requested to join lobby");
		JoinLobby (joinRequest.m_steamIDLobby);
	}

	void OnP2PSessionRequest(P2PSessionRequest_t request)
	{
		CSteamID clientId = request.m_steamIDRemote;
		if (ExpectingClient.Contains(clientId))
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

	SteamAPICall_t CreateLobby (ELobbyType eLobbyType, int maxMembers){
		return SteamMatchmaking.CreateLobby (eLobbyType, maxMembers);
	}

	SteamAPICall_t JoinLobby (CSteamID lobby){
		return SteamMatchmaking.JoinLobby (lobby);
	}

	bool InviteUserToLobby(CSteamID lobby, CSteamID invitee){
		return false;
	}

	public void Update(){
		SteamAPI.RunCallbacks ();
		if(Input.GetKeyDown(KeyCode.S)){
			SetupServer ();
		}
		if(Input.GetKeyDown(KeyCode.I)){
			SteamFriends.ActivateGameOverlayInviteDialog (lobbyID);
		}
		ReadPackets ();
		SendMyPosition ();
		Talk ();
	}

	void SendMyPosition(){
		byte[] bytes = ItemsPositionBytes(new List<Transform> {
			InputMachine.instance.Headset.transform
			, InputMachine.instance.Right.transform
			, InputMachine.instance.Left.transform
		});
		byte[] bytes2 = new byte[bytes.Length + 1];
		Array.Copy (bytes, 0, bytes2, 1, bytes.Length);
		bytes2 [0] = (byte)InterpretationType.PlayerObjects;

		SendBytes (bytes2);
	}
	public void SendGrab(string itemID, int hand){
		byte[] stringBytes = StringToByte (itemID);
		byte[] bytes = new byte[stringBytes.Length + 2];
		bytes[0] = (byte)InterpretationType.GrabObject;
		bytes[1] = (byte)hand;
		Array.Copy (stringBytes, 0, bytes, 2, stringBytes.Length);
		SendBytes (bytes);
	}

	public void ReadPackets(){
		uint size;
		while (SteamNetworking.IsP2PPacketAvailable(out size))
		{
			Debug.Log ("packet");
			var buffer = new byte[size];
			uint bytesRead;
			CSteamID remoteId;
			if (SteamNetworking.ReadP2PPacket(buffer, size, out bytesRead, out remoteId))
			{
				int dataType = (int)buffer [0];
				byte[] dataIn = new byte[size - 1];
				Array.Copy (buffer, 1, dataIn, 0, size - 1);
				switch (dataType) {
				case 7://ping
					Debug.Log("ping");
					if (!ExpectingClient.Contains (remoteId)) {
						ExpectingClient.Add (remoteId);
					}
					break;
				case 1://voice
					byte[] bufferOut = new byte[22050];
					uint bytesOut;
					EVoiceResult voiceOut = SteamUser.DecompressVoice (dataIn, (uint)dataIn.Length, bufferOut, 22050, out bytesOut, 11025);
					float[] test = new float[11025];
					for (int i = 0; i < test.Length; ++i) {
						test[i] = (short)(bufferOut[i * 2] | bufferOut[i * 2 + 1] << 8) / 32768.0f;
					}
					PlayerMachine.instance.chatAudio.clip.SetData (test, 0);
					PlayerMachine.instance.chatAudio.Play ();
					break;
				case 2://person update, head and hands
					if (otherPlayers.ContainsKey (remoteId)) {
						if (otherPlayers [remoteId] == null) {
							GameObject go = (GameObject)GameObject.Instantiate (otherPlayerObject);
							otherPlayers [remoteId] = go.GetComponent<OtherPlayerObject> ();
						}
					} else {
						GameObject go = (GameObject)GameObject.Instantiate (otherPlayerObject);
						otherPlayers.Add(remoteId, go.GetComponent<OtherPlayerObject> ());
					}
					otherPlayers [remoteId].InterpretLocation (ByteToFloatArray (dataIn));
					break;
				case 3://instantiate object
					byte[] instantiateChar = new byte[10 * sizeof(char)];
					byte[] instantiateFloat = new byte[7 * sizeof(float)];
					if (dataIn.Length == instantiateChar.Length + instantiateFloat.Length) {
						Array.Copy (dataIn, 0, instantiateChar, 0, instantiateChar.Length);
						Array.Copy (dataIn, instantiateChar.Length, instantiateFloat, 0, instantiateFloat.Length);
						Debug.Log("Instantiate: " + ByteToString(dataIn));
					}
					break;
				case 4://grab object
					Debug.Log("Grab");
					byte[] grabBytes = new byte[dataIn.Length - 1];
					Array.Copy (dataIn, 1, grabBytes, 0, grabBytes.Length);
					otherPlayers [remoteId].GrabObject (allObjects [ByteToString (grabBytes)], (Hand)dataIn [0]);
					break;
				case 5://update objects
					Debug.Log("Update Objects");

					break;
				case 6://destroy object
					Debug.Log("Destroy: " + ByteToString(dataIn));
					break;
				default:
					Debug.Log ("Unknown Data");
					break;
				}
			}
		}
	}
}
