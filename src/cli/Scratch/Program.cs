var simon = new Robot("Simon");
simon.Say("I have awaken.");
simon.Work();
simon.Say("I am returning to sleep.");

public class Robot(string name)
{
    public string Name { get; private set; } = name;

    public void Say(string something)
        => Console.WriteLine($"{DateTime.Now:HH:mm:ss} {Name} says: {something}");

    public async Task Work()
    {
        Say($"I am starting an important task. Your computer belongs to me.");
        Say("Hold your breath until my work is complete.");
        await Task.Delay(3000);
        Say($"I completed my work. Your computer is returned to you.");
        Say("You may breathe again.");
    }
}