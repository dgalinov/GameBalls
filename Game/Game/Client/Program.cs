using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] BufferLocal = new Byte[4]; //El definim de 4 bytes per forçar que els missatges superin el tamany del buffer.En una aplicació real es podria definir més gran fins un màxim de 8192 bytes(8K)
            byte[] msg;
            int BytesRebuts;
            string missatge;
            MemoryStream BufferAllData = new MemoryStream();
            /*********CONNEXIÓ AMB EL SERVER *********************/
            //Creem un IPEndPoint amb l'adreça del server IP + Port
            IPAddress ServerIP = IPAddress.Parse("127.0.0.1");
            int MyPort = 11000;
            IPEndPoint ServerEndPoint = new IPEndPoint(ServerIP, MyPort);
            //Cree un TcpClient per crear la connexió amb el server
            TcpClient Client = new TcpClient();
            //Connectem amb el Server
            Client.Connect(ServerEndPoint);
            if (Client.Connected)
                Console.WriteLine("Client connectat!");
            /***********ENVIAMENT DE DADES *************************/
            Console.WriteLine("Entra un text a enviar al server:");
            missatge = Console.ReadLine();
            //Obtenim l'Stream d'intercanvi de dades
            NetworkStream MyNetworkStream = Client.GetStream();
            //Passem el missatge a bytes
            msg = Encoding.Unicode.GetBytes(missatge);
            //Enviem al server
            MyNetworkStream.Write(msg, 0, msg.Length);
            /*******************LECTURA DE DADES **********************/
            //Llegim dades enviades pel client. En aquest punt es quedaria bloquejat si no hi ha dades per llegir.
            BytesRebuts = MyNetworkStream.Read(BufferLocal, 0,
            BufferLocal.Length);
            //Aquesta part l’haurem de fer si la informació que ens envien a través del NetworkStream és més gran que el tamany del nostre buffer local.
            BufferAllData.Write(BufferLocal, 0, BufferLocal.Length);
            //Escrivim la informació llegida al Buffer on acumulem les dades.
            while (MyNetworkStream.DataAvailable) //Mentre hi hagi dades disponibles al NetworkStream
            {
                BytesRebuts += MyNetworkStream.Read(BufferLocal, 0,
                BufferLocal.Length); //Tornem a llegir del NetworkStream
                BufferAllData.Write(BufferLocal, 0, BufferLocal.Length); //Acumulem la informació al MemoryStream.
            }
            //Com que el missatge el rebem en bytes, l'hem de passar a string.
            missatge = System.Text.Encoding.Unicode.GetString(BufferAllData.ToArray(), 0, BytesRebuts);
            Console.WriteLine("missatge: {0}", missatge);
            Console.WriteLine("missatge rebut!");
            /*****************TANCAMENT DE LA CONNEXIÓ****************/
            MyNetworkStream.Close();
            Client.Close();
            Console.ReadLine();
        }
    }
}