namespace IntroToDtataStructures
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int[] lockerRoom = new int[10];


            Console.WriteLine("=============Fixed Size Below ==============");
            Console.WriteLine($"This array has the size of {lockerRoom.Length} slots.");

            Console.WriteLine();


            //populate our arrays
            Console.WriteLine("===========Populating the Array===========");

            for (int i = 0;i < lockerRoom.Length; i++)
                {
                lockerRoom[i] = i * 100;
                
            }

            for (int i = 0; i < lockerRoom.Length; i++)
            {
                Console.WriteLine($"Locker[{i}] contains the value: {lockerRoom[i]}");   
            }

            Console.WriteLine();

            //check how memory is allocated 

            Console.WriteLine("===========Memory Layout===========");
            Console.WriteLine($"sizeof(int) = {sizeof(int)} bytes ");
            Console.WriteLine();



            //less work or touch our memory
            unsafe
            {
                fixed (int* ptr = lockerRoom)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        int* elementsPointer = ptr + i;
                        long address = (long)elementsPointer;
                    }
                }
            }
        }
    }
}
