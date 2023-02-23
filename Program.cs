using C__Test2DGame;
internal class Program
{
    private static void Main(string[] args)
    {
        Console.Clear();
        Console.WriteLine("Main execution");
        Game.CalculateMapSize();
        Console.ReadLine();
        Game.Run();
    }
}