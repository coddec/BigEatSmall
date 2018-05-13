using System;

public static class Function
{
    public static int RdnInclusive(int min, int max)
    {
        Random rnd = new Random();
        int tmp = rnd.Next(min, max + 1);
        return tmp;
    }
}


