using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
namespace server
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
            //**********CREACIÓ DEL SERVER ********************/
            //Creem un IPEndPoint (IP + Port)
            //IPAddress ipadress = Dns.GetHostAddresses("localhost")[1];
            IPAddress MyIPAddress;
            MyIPAddress = IPAddress.Parse("127.0.0.1");
            int MyPort = 11000;
            IPEndPoint MyIPEndPoint = new IPEndPoint(MyIPAddress, MyPort);
            //Creem un objecte TcpListener que és l'encarregat de posar-se a l'escola de noves connexions de clients.
            TcpListener Server = new TcpListener(MyIPEndPoint);
            Console.WriteLine("Server creat");
            //Iniciem el servidor, el posem a l'escola de connexions
            Server.Start();
            Console.WriteLine("Servidor iniciat");
            /**********ACCEPTACIÓ DE CONNEXIONS **********/
            //Recuperem peticions de connexió per part de clients. En aquest punt el server es bloquejaria esperant connexions si no n'hi hagués cap.
            //Un cop tenim una connexió amb un client generem un TcpClient, que serà l'objecte encarregat de la transmissió de dades.
            TcpClient client = Server.AcceptTcpClient();
            Console.WriteLine("Connexió acceptada");
            //Obtenim el NetworkStream on realitzaremm l'intercanvi de dades entre client i servidor.
            NetworkStream MyNetworkStream = client.GetStream();
            /***********LECTURA DE DADES **************************/
            //Llegim dades enviades pel client. En aquest punt es  bloquejat si no hi ha dades per llegir.
            BytesRebuts = MyNetworkStream.Read(BufferLocal, 0,
            BufferLocal.Length);
            //Aquesta part la haurem de fer si la informació que ens envien a través del NetworkStream és més gran que el tamany del nostre buffer local.
             BufferAllData.Write(BufferLocal, 0, BufferLocal.Length);
            //Escrivim la informació llegida al Buffer on acumulem les dades.
            while (MyNetworkStream.DataAvailable) //Mentre hi hagi dades disponibles al NetworkStream
            {
                BytesRebuts += MyNetworkStream.Read(BufferLocal, 0,
               BufferLocal.Length); //Tornem a llegir del NetworkStream
                BufferAllData.Write(BufferLocal, 0, BufferLocal.Length);
                //Acumulem la informació al MemoryStream.
            }
            //Com que el missatge el rebem en bytes, l'hem de passar a string.
            missatge =
           System.Text.Encoding.Unicode.GetString(BufferAllData.ToArray(), 0, BytesRebuts);
            Console.WriteLine("missatge: {0}", missatge);
            Console.WriteLine("missatge rebut!");
            /***********************ENVIAMENT DE DADES*************************/
            //Reenvieme el missatge al client
            //Passem el missatge a bytes
            msg = Encoding.Unicode.GetBytes(missatge + "_server");
            //Enviem al client
            MyNetworkStream.Write(msg, 0, msg.Length);
            /**********TANCAMENT DE LA CONNXIO ******************/
            MyNetworkStream.Close();
            client.Close();
            Console.ReadLine();
        }
    }
}
