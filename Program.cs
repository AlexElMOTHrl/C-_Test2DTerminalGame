using C__Test2DGame;
internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Main execution");
        Console.WriteLine(Game.deadthMsg);
        Console.WriteLine(Game.deadthMsgAnimation);
        Game.Run();
    }
}