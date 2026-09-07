using System;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using ChessLogic.Enum;


namespace ChessLogic
{
	public class ChessServer
	{
        private readonly TcpListener listener;
        private readonly Dictionary<Player, TcpClient> playerPool = new();
        private readonly Dictionary<Player, StreamWriter> playerWriters = new();
        private readonly GameState gameState;
        private readonly object stateLock = new();

        public ChessServer(int port)
		{
            listener = new TcpListener(IPAddress.Any, port);
            gameState = new GameState(Player.White, Board.Initial());
        }

        public void Start()
        {
            listener.Start();
            new Thread(AcceptLoop) { IsBackground = true }.Start();
        }
        private void AcceptLoop()
        {
            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                Player assigned = Player.None;

                    // Check the player pool and assign a player
                    lock (stateLock)
                    {
                        if(!playerPool.ContainsKey(Player.White)) assigned = Player.White;
                        else if (!playerPool.ContainsKey(Player.Black)) assigned = Player.Black;

                        if (assigned == Player.None) { client.Close(); continue; }
                        playerPool[assigned] = client;
                    }

                new Thread(() => HandleClient(client, assigned)) {  IsBackground = true }.Start();
            }
        }

        private void HandleClient(TcpClient client, Player assignedPlayer)
        {
            NetworkStream stream = client.GetStream();
            StreamReader reader = new StreamReader(stream);
            StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };

            lock (stateLock)
            {
                playerWriters[assignedPlayer] = writer;
                Send(writer, new NetworkMessage { Type = "hello", Payload = assignedPlayer.ToString() });
                BroadcastStateInternal();
            }

            try
            {
                // Check if connection is established
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    NetworkMessage msg = JsonSerializer.Deserialize<NetworkMessage>(line);
                    if (msg.Type == "move")
                    {
                        HandleMove(msg, assignedPlayer);
                    }
                }
            }
            catch (IOException) { /* client dropped */ }
            finally
            {
                // Clean up on disconnect
                lock (stateLock)
                {
                    playerPool.Remove(assignedPlayer);
                    playerWriters.Remove(assignedPlayer);
                }
                client.Close();
            }
        }

        private void HandleMove(NetworkMessage msg, Player sender)
        {
            MoveRequestDto movedRequest = JsonSerializer.Deserialize<MoveRequestDto>(msg.Payload);

            lock (stateLock)
            {
                // Check the steps of the game are correctly followed, game state is valid
                if (gameState.CurrentPlayer != sender)
                {
                    Send(playerWriters[sender], new NetworkMessage { Type = "reject", Payload = "Not your turn" });
                    return;
                }

                Position from = new Position(movedRequest.FromRow, movedRequest.FromCol);
                Position to = new Position(movedRequest.ToRow, movedRequest.ToCol);

                Move legalMove = gameState.LegalMovesForPiece(from).FirstOrDefault(m => m.ToPos == to);
                if (legalMove == null)
                {
                    Send(playerWriters[sender], new NetworkMessage { Type = "reject", Payload = "Illegal move" });
                    return;
                }

                gameState.MakeMove(legalMove);
                BroadcastStateInternal();
            }
        }

        private void BroadcastState()
        {
            lock (stateLock)
            {
                BroadcastStateInternal();
            }
        }

        private void BroadcastStateInternal()
        {
            GameStateDto dto = BuildStateDto();
            NetworkMessage msg = new NetworkMessage { Type = "state", Payload = JsonSerializer.Serialize(dto) };

            //List<TcpClient> clients;
            //lock (stateLock)
            //    clients = playerPool.Values.ToList();
            
            //foreach (TcpClient c in clients)
            //{
            //    Send(new StreamWriter(c.GetStream()) { AutoFlush = true }, msg);
            //}
            foreach (StreamWriter writer in playerWriters.Values)
            {
                Send(writer, msg);
            }
        }

        // Helper methods
        private static void Send(StreamWriter writer, NetworkMessage msg) => writer.WriteLine(JsonSerializer.Serialize(msg));
        private GameStateDto BuildStateDto()
        {
            String[][] rows = new string[8][];
            for (int r = 0; r < 8; r++)
            {
                rows[r] = new string[8];
                for (int c = 0; c < 8; c++)
                {
                    Piece piece = gameState.Board[r, c];
                    rows[r][c] = piece == null ? null : $"{piece.Color}_{piece.Type}";
                }
            }
            bool over = gameState.IsGameOver();
            return new GameStateDto
            {
                Board = rows,
                CurrentPlayer = gameState.CurrentPlayer.ToString(),
                IsGameOver = over,
                Winner = over ? gameState.Result.Winner.ToString() : null,
                Reason = over ? gameState.Result.Reason.ToString(): null
            };
        }
    }

    public class NetworkMessage 
    { 
        public string Type { get; set; } 
        public string Payload { get; set; }
    }
    public class MoveRequestDto 
    { 
        public int FromRow { get; set; } public int FromCol { get; set; } public int ToRow { get; set; } public int ToCol { get; set; } 
    }
    public class GameStateDto 
    { 
        public string[][] Board { get; set; } public string CurrentPlayer { get; set; } public bool IsGameOver { get; set; } public string Winner { get; set; } public string Reason { get; set; } 
    }
}
