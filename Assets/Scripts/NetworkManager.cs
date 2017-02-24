using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

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

	void SendString(string str){
		byte[] bytes = new byte[str.Length * sizeof(char)];
		System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
		byte[] bytes2 = new byte[bytes.Length + 1];
		Array.Copy (bytes, 0, bytes2, 1, bytes.Length);
		bytes2 [0] = (byte)2;
		SendBytes (bytes);
	}

	void SendClaimObject(string objID){
		byte[] bytes = new byte[objID.Length * sizeof(char)];
		System.Buffer.BlockCopy(objID.ToCharArray(), 0, bytes, 0, bytes.Length);
		byte[] bytes2 = new byte[bytes.Length + 1];
		Array.Copy (bytes, 0, bytes2, 1, bytes.Length);
		bytes2 [0] = (byte)4;
		SendBytes (bytes);
	}

	void SendReleaseObject(string objID){
		byte[] bytes = new byte[objID.Length * sizeof(char)];
		System.Buffer.BlockCopy(objID.ToCharArray(), 0, bytes, 0, bytes.Length);
		byte[] bytes2 = new byte[bytes.Length + 1];
		Array.Copy (bytes, 0, bytes2, 1, bytes.Length);
		bytes2 [0] = (byte)5;
		SendBytes (bytes);
	}

	public void SendBytes(byte[] bytes){
		foreach (CSteamID csid in ExpectingClient) {
			SteamNetworking.SendP2PPacket(csid, bytes, (uint) bytes.Length, EP2PSend.k_EP2PSendReliable);
		}
	}

	public void OnGameOverlayActivated(GameOverlayActivated_t overlay){

	}

	public void OnLobbyCreated(LobbyCreated_t lobbyCreated){
		lobbyID = new CSteamID (lobbyCreated.m_ulSteamIDLobby);
		SteamFriends.ActivateGameOverlayInviteDialog (lobbyID);
		Debug.Log ("Lobby Created");
	}

	public void OnLobbyList(LobbyMatchList_t lobbyList){
		Debug.Log ("Lobby List");
	}

	public void OnLobbyEnter(LobbyEnter_t lobbyEnter){
		Debug.Log ("Lobby Entered");
		lobbyID = new CSteamID (lobbyEnter.m_ulSteamIDLobby);
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
		SendBytes (new byte[] { (byte)3 });
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

	public void Update(){
		SteamAPI.RunCallbacks ();
		if(Input.GetKeyDown(KeyCode.S)){
			SetupServer ();
		}
		if(Input.GetKeyDown(KeyCode.I)){
			SteamFriends.ActivateGameOverlayInviteDialog (lobbyID);
		}
		if(Input.GetKeyDown(KeyCode.C)){
			SendString ("HI!");
		}
		Talk ();
		ReadPackets ();
	}
	public void ReadPackets(){
		uint size;
		while (SteamNetworking.IsP2PPacketAvailable(out size))
		{
			Debug.Log (size);
			var buffer = new byte[size];
			uint bytesRead;
			CSteamID remoteId;

			if (SteamNetworking.ReadP2PPacket(buffer, size, out bytesRead, out remoteId))
			{
				int dataType = (int)buffer [0];
				byte[] dataIn = new byte[size - 1];
				Array.Copy (buffer, 1, dataIn, 0, size - 1);
				switch (dataType) {
				case 1://voice
					byte[] bufferOut = new byte[22050];
					uint bytesOut;
					EVoiceResult voiceOut = SteamUser.DecompressVoice (dataIn, (uint)dataIn.Length, bufferOut, 22050, out bytesOut, 11025);
					PlayerMachine.instance.chatAudio.clip = AudioClip.Create ("chat", 11025, 1, 11025, false, false);
					float[] test = new float[11025];
					for (int i = 0; i < test.Length; ++i) {
						test[i] = (short)(bufferOut[i * 2] | bufferOut[i * 2 + 1] << 8) / 32768.0f;
					}
					PlayerMachine.instance.chatAudio.clip.SetData (test, 0);
					PlayerMachine.instance.chatAudio.Play ();
					break;
				case 2://string
					char[] chars = new char[bytesRead / sizeof(char)];
					System.Buffer.BlockCopy(dataIn, 0, chars, 0, dataIn.Length);

					string message = new string(chars, 0, chars.Length);
					Debug.Log("Received a message: " + message);
					break;
				case 3://ping
					if (!ExpectingClient.Contains (remoteId)) {
						ExpectingClient.Add (remoteId);
					}
					break;
				case 4://claim object
					char[] claimChars = new char[bytesRead / sizeof(char)];
					System.Buffer.BlockCopy(dataIn, 0, claimChars, 0, dataIn.Length);
					Debug.Log("Claimed: " + new string(claimChars, 0, claimChars.Length));
					break;
				case 5://release object claim
					char[] releaseChars = new char[bytesRead / sizeof(char)];
					System.Buffer.BlockCopy(dataIn, 0, releaseChars, 0, dataIn.Length);
					Debug.Log("Released: " + new string(releaseChars, 0, releaseChars.Length));
					break;
				case 6://object update
					char[] objectUpdateChars = new char[bytesRead / sizeof(char)];
					System.Buffer.BlockCopy(dataIn, 0, objectUpdateChars, 0, dataIn.Length);
					Debug.Log("Updated: " + new string(objectUpdateChars, 0, objectUpdateChars.Length));
					break;
				default:
					Debug.Log ("Unknown Data");
					break;
				}
			}
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
}
