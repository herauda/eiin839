using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;

class Program
{
    static void Main(string[] args)
    {
        // creation serveur http        
            //if HttpListener is not supported by the Framework
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("A more recent Windows version is required to use the HttpListener class.");
                return;
            }
 
 
            // Create a listener.
            HttpListener listener = new HttpListener();

            // Add the prefixes.
            if (args.Length != 0)
            {
                foreach (string s in args)
                {
                    listener.Prefixes.Add(s);
                    // don't forget to authorize access to the TCP/IP addresses localhost:xxxx and localhost:yyyy 
                    // with netsh http add urlacl url=http://localhost:xxxx/ user="Tout le monde"
                    // and netsh http add urlacl url=http://localhost:yyyy/ user="Tout le monde"
                    // user="Tout le monde" is language dependent, use user=Everyone in english 

                }
            }
            else
            {
                Console.WriteLine("Syntax error: the call must contain at least one web server url as argument");
            }
            listener.Start();

            // get args 
            foreach (string s in args)
            {
                Console.WriteLine("Listening for connections on " + s);
            }

            // Trap Ctrl-C on console to exit 
            Console.CancelKeyPress += delegate {
                // call methods to close socket and exit
                listener.Stop();
                listener.Close();
                Environment.Exit(0);
            };


            while (true)
            {
                // Note: The GetContext method blocks while waiting for a request.
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;

                string documentContents;
                using (Stream receiveStream = request.InputStream)
                {
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        documentContents = readStream.ReadToEnd();
                    }
                }
                
                // get url 
                Console.WriteLine($"Received request for {request.Url}");

                //get url protocol
                Console.WriteLine(request.Url.Scheme);
                //get user in url
                Console.WriteLine(request.Url.UserInfo);
                //get host in url
                Console.WriteLine(request.Url.Host);
                //get port in url
                Console.WriteLine(request.Url.Port);
                //get path in url 
                Console.WriteLine(request.Url.LocalPath);

                // parse path in url 
                foreach (string str in request.Url.Segments)
                {
                    Console.WriteLine($"Segment: {str}");
                }

                //get params un url. After ? and between &

                Console.WriteLine(request.Url.Query);


                ///////////////////////////
                ///
                if (request.Url.Segments[1].Equals("incr"))
                {
                
                } 
                else
                {
                    //parse params in url
                    string[] parameters = new string[2] { HttpUtility.ParseQueryString(request.Url.Query).Get("param1"), HttpUtility.ParseQueryString(request.Url.Query).Get("param2") };
                    Console.WriteLine($"Parameters: {parameters[0]} - {parameters[1]}");
                    // reflection
                    Type type = typeof(Mymethods);
                    MethodInfo method = type.GetMethod(request.Url.Segments[1]);

                    if (method != null)
                    {
                        Mymethods c = new Mymethods();
                        string result = (string)method.Invoke(c, parameters);
                        Console.WriteLine(result);
                        Console.ReadLine();

                        //
                        Console.WriteLine(documentContents);
                    }
                    
                }
                
            }
            // Httplistener neither stop ... But Ctrl-C do that ...
            // listener.Stop();
    }

    public class Mymethods
    {
        public Mymethods()
        {
        }

        public String retHtml(String param1, String param2)
        {
            return $"<html><body> Hello {param1} et {param2} </body></html>";
        }

        // localhost:8080/refMethod1?param1=Prenom&param2=Nom
        public String refMethod1(String param1, String param2)
        {
            param1 = $"{param1} called through refMethod1";
            param2 = $"{param2} called through refMethod1";
            Console.WriteLine("Call refMethod1");
            return retHtml(param1, param2);
        }

        // localhost:8080/refMethod2?param1=Prenom&param2=Nom
        public String refMethod2(String param1, String param2)
        {
            param1 = $"{param1} called through refMethod2";
            param2 = $"{param2} called through refMethod2";
            Console.WriteLine("Call refMethod2");
            return retHtml(param1, param2);
        }

        // localhost:8080/callExt?param1=Prenom&param2=Nom
        public void refMethodExt(String param1, String param2)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = @"..\..\..\ExternalExe\bin\Debug\net6.0\ExternalExe.exe"; // Specify exe name.
            start.Arguments = $"{param1} {param2}";
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            //
            // Start the process.
            //
            using (Process process = Process.Start(start))
            {
                //
                // Read in all the text from the process with the StreamReader.
                //
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    Console.WriteLine(result);
                    Console.ReadLine();
                }
            }
        }

    }

    public class Incr
    {
        public int incr(int value)
        {
            return value++;
        }
    }

}