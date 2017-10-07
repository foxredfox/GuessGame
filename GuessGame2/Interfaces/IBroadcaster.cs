using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessGame2
{
    public interface IBroadcaster
    {
        void Broadcast(string v);
        void BroadcastExcept(string v, Client clientExcept);
    }
}
