using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GritNetworking
{
    public struct netVector
    {
        public float x;
        public float y;

        public netVector(float X, float Y)
        {
            x = X;
            y = Y;
        }
        public netVector(string str)
        {
            string[] parts = str.Split('/');
            x = float.Parse(parts[0]);
            y = float.Parse(parts[1]);
        }

        public override string ToString()
        {
            return x + "/" + y;
        }
    }

    public class NetEntity
    {
        public string clientId;
        public netVector position = new netVector();
        public netVector velocity = new netVector();

        public NetEntity() { }
        public NetEntity(string ID)
        {
            clientId = ID;
        }
        public NetEntity(string ID, netVector pos, netVector vel) 
        {
            clientId = ID;
            position = pos;
            velocity = vel;
        }
        public NetEntity(netVector pos, netVector vel) 
        {
            position = pos;
            velocity = vel;
        }

        public void UpdateFromString(string str, bool updateClientId = false)
        {
            string[] parts = str.Split('=');
            if (updateClientId)
            {
                clientId = parts[0];
            }
            position = new netVector(parts[1]);
            velocity = new netVector(parts[2]);
        }

        public override string ToString()
        {
            return string.Format("{0}={1}={2}", clientId, position, velocity);
        }
    }
}