namespace TestDiagnostics
{
    public class FirstClass
    {
        public static void Foo() { }
    }

    // This should trigger SPY01 because SecondClass must be moved to a separate file.
    public class SecondClass
    {
        public static void Bar() { }
    }
}