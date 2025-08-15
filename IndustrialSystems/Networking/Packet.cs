using ProtoBuf;
using System.Collections.Generic;
using VRageMath;
using VRage.Serialization;

namespace IndustrialSystems.Networking
{

    [ProtoInclude(1000, typeof(SyncRequestPacket))]
    [ProtoContract]
    public abstract class Packet
    {

        public Packet()
        {
        }
    }

    [ProtoContract]
    public class SyncRequestPacket : Packet
    {
        // DJ if you're reading this ik theres a mild amount of position exploitation here, I do not care
        [ProtoMember(1)] public Vector3D PlayerPos;
        public SyncRequestPacket()
        {
        }

        public SyncRequestPacket(Vector3D pos)
        {
            PlayerPos = pos;
        }
    }
}
