using System;
using System.Diagnostics;
using System.IO;

namespace DirSearch{
    public class Program {
        public static async Task Search(string url,string[]directories){
            List<string> valid_directories = new List<string>();
            ProcessStartInfo startInfo = new ProcessStartInfo() { FileName = "/bin/bash", Arguments = "./fig.sh", }; 
            Process proc = new Process() { StartInfo = startInfo, };
            proc.Start();
            Console.WriteLine("Created by -Bafedile");
            Console.WriteLine("==============================================================\n"+
            "Starting...");

             try{

                HttpClientHandler handler = new HttpClientHandler();
                
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(60);

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get,url);
                HttpResponseMessage response = await client.SendAsync(request);

                //check if the website is available
                if((int)response.StatusCode ==200){
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Status Code: {(int)response.StatusCode} Successful\nReason Phrase: {response.ReasonPhrase}");
                    // check for available directories 
                    
                    Parallel.For(0,directories.Length,i =>{
                        var line = directories[i];
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine($"Testing {url+line}");
                        request = new HttpRequestMessage(HttpMethod.Get,url+line);
                        response =  client.Send(request);
                        if((int)response.StatusCode == 200){
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"[+] Found a directory : {url+line}");
                            valid_directories.Add($"[+] Found a directory : {url+line}");
                        }
                    });
                    // display the found directories 
                    Console.WriteLine("Directory Search Results\n================================================================================================\n");
                    foreach(string dir in valid_directories){
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(dir);
                    }

                                        
                }else{
                    Console.WriteLine($"Website is unavailable: {response.ReasonPhrase}");
                }
            }catch(HttpRequestException hre){
                Console.WriteLine($"An exception thrown: {hre.Message}");
            }
        }
        public static void Main(string[]args){
            // declare the variables
            string[] directories ;
            string url = "http://127.0.0.1";
            int port;
            try{
                if(args.Length ==4){
                // check if the url is valid 
                    if(args[1].Contains("http")){
                        // check if the port is a digit 
                        if(args[2].All(char.IsDigit)){
                            // get the url 
                            url = args[1];
                            port = Convert.ToInt32(args[2]); 
                            string full_url = url+":"+port+"/";

                            // check if the wordlist is a text file 
                            if(args[3].EndsWith(".txt")){
                                // assign the wordlist text file 
                                directories = File.ReadAllLines(args[3]);
                                Task task = Task.Run(()=>Search(full_url,directories));
                                task.Wait();
                            }else if(args[3] == null){
                                // use the default wordlist text file 
                                directories  = File.ReadAllLines("big.txt");    
                                Task task = Task.Run(()=>Search(full_url,directories));
                                task.Wait();
                            }
                            
                            else{
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Wordlist should be a text file\nE.g wordlist.txt");
                            }
                        }else{
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("A port should consist of only digits\nE.g 443,80");
                        }
                        
                    }else{
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Use a valid url link\n"+
                        "E.g http://foo.com or https://foo.com");
                    }
                
                }else{
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("The script should be in the format\n"+
                    "dotnet run dirsearch.cs [url] [port] [wordlist]\n"+
                "E.g dotnet run dirsearch.cs http://localhost 443 wordlist.txt");
                }
            
            }catch(AggregateException ae){
                foreach(Exception e in ae.InnerExceptions){
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Exception thrown: {e.Message}");
                }
            }
            
           
        }
    }
}