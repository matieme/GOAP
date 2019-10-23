namespace GameUtils
{
    public class Tuple<T1, T2>
    {
        public T1 First { get; set; }
        public T2 Second { get; set; }
        internal Tuple(T1 first, T2 second)
        {
            First = first;
            Second = second;
        }
    }

    public class Triple<T1, T2, T3>
    {
        public T1 First { get; set; }
        public T2 Second { get; set; }
        public T3 Third { get; set; }

        internal Triple(T1 first, T2 second, T3 third)
        {
            First = first;
            Second = second;
            Third = third;
        }
    }

    public class Quadruple<T1, T2, T3, T4>
    {
        public T1 First { get; set; }
        public T2 Second { get; set; }
        public T3 Third { get; set; }
        public T4 Fourth { get; set; }

        internal Quadruple(T1 first, T2 second, T3 third, T4 fourth)
        {
            First = first;
            Second = second;
            Third = third;
            Fourth = fourth;
        }
    }


    public static class Tuple
    {
        public static Tuple<T1, T2> New<T1, T2>(T1 first, T2 second)
        {
            return new Tuple<T1, T2>(first, second); ;
        }

        public static Triple<T1, T2, T3> New<T1, T2, T3>(T1 first, T2 second, T3 third)
        {
            return new Triple<T1, T2, T3>(first, second, third);
        }

        public static Quadruple<T1, T2, T3, T4> New<T1, T2, T3, T4>(T1 first, T2 second, T3 third, T4 fourth)
        {
            return new Quadruple<T1, T2, T3, T4>(first, second, third, fourth);
        }
    }
}