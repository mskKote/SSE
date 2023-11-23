using task_3_5;

var server = new HttpFileServer();
server.Run("127.0.0.1", 3000);
Console.WriteLine("Listening ...");