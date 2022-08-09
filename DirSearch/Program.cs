using System;
using System.IO;

namespace DirSearch{
    public class Program {
        public static async Task Main(string[]args){
            // declare the variables
            string[] directories  = File.ReadAllLines("big.txt");
            
            
            string url = "http://127.0.0.1/";
            try{
                var client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get,url);
                HttpResponseMessage response = await client.SendAsync(request);

                //check if the website is available
                if((int)response.StatusCode ==200){
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{response.Headers}\n{response.StatusCode}\n{response.ReasonPhrase}");
                    // check for available directories 
                    
                    Parallel.For(0,directories.Length,i =>{
                        var line = directories[i];
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine($"Tesing {url+line}");
                        request = new HttpRequestMessage(HttpMethod.Get,url+line);
                        response =  client.Send(request);
                        if((int)response.StatusCode == 200){
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"[+] Found a directory : {url+line}");
                        }
                                           });
                }else{
                    Console.WriteLine($"Website is unavailable: {response.ReasonPhrase}");
                }
            }catch(HttpRequestException hre){
                Console.WriteLine($"An exception thrown: {hre.Message}");
            }
        }
    }
}