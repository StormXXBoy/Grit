using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GritNetworking
{
    public class NetVector
    {
        public float X;
        public float Y;

        public NetVector() { }
        public NetVector(float x, float y)
        {
            X = x;
            Y = y;
        }
        public NetVector(string str)
        {
            string[] parts = str.Split('/');
            X = float.Parse(parts[0]);
            Y = float.Parse(parts[1]);
        }

        public void UpdateFromString(string str)
        {
            string[] parts = str.Split('/');
            X = float.Parse(parts[0]);
            Y = float.Parse(parts[1]);
        }

        public override string ToString()
        {
            return X + "/" + Y;
        }
    }

    public class NetEntity
    {
        public string clientId;
        public NetVector position = new NetVector();
        public NetVector velocity = new NetVector();

        public NetEntity() { }
        public NetEntity(string ID)
        {
            clientId = ID;
        }
        public NetEntity(string ID, NetVector pos, NetVector vel) 
        {
            clientId = ID;
            position = pos;
            velocity = vel;
        }
        public NetEntity(NetVector pos, NetVector vel) 
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
            position = new NetVector(parts[1]);
            velocity = new NetVector(parts[2]);
        }

        public override string ToString()
        {
            return string.Format("{0}={1}={2}", clientId, position, velocity);
        }
    }
}