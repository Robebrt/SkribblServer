using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkribblClient
{
    public class Player
    {
        public string username;
        public Boolean isDrawing = false;
        public int score = 0;
        public int roomId;
        public string avatar;
        public string word;
        
        public Player(string username, string avatar, bool isDrawing, string word, int score)
        {
            this.username = username;
            this.avatar = avatar;
            this.isDrawing = isDrawing;
            this.word = word;
            this.score = score;
        }
        public void AddScore(int points)
        {
            this.score += points;
        }
        public void SetDrawing()
        {
            this.isDrawing = true;
        }
        public int GetRoom()
        {
            return this.roomId;
        }
        
    }
}
