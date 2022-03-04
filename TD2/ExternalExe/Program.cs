var arguments = Environment.GetCommandLineArgs();

foreach (String arg in arguments)
{
    Console.WriteLine(arg);
}

Console.WriteLine($"<html><body> Hello {arguments[1]} {arguments[2]} : you called an external executable!!</body></html>");
