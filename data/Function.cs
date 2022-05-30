namespace Practice;
public static class Function
{
    public static uint      numberFunc;     /// Номер задачи
    public static double    lambda;         /// Значение Lambda
    public static Complex   gamma;          /// Значение gamma
    public static double    betta;          /// Значение betta

    //* Инициализация констант
    public static void Init(uint numF) {
        numberFunc = numF;

        switch(numberFunc) {
            case 1:                                         /// Полином первой степени
                lambda = 2;          
                gamma  = new Complex(1, 2);
            break;

            case 2:                                         /// Полином второй степени
                lambda = 2;          
                gamma  = new Complex(1, 2);
            break;

            case 3:                                         /// Полином третьей степени
                lambda = 2;          
                gamma  = new Complex(1, 2);
            break;

            case 4:                                         /// Полином четвертой степени
                lambda = 2;          
                gamma  = new Complex(1, 2);
            break;

            case 5:                                         /// Не полином
                lambda = 2;          
                gamma  = new Complex(1, 2);
            break;

        }
    }

    //* Абсолютное значение U-функции
    public static Complex Absolut(Vector<double> vec) {
        (double x, double y) = (vec[0], vec[1]);
        return numberFunc switch 
        {
            1 => new Complex(x + y, 2*x + y),                                       /// Полином первой степени 
            2 => new Complex(x*x + y*y, 2*x*x + y*y),                               /// Полином второй степени 
            3 => new Complex(Pow(x, 3) + Pow(y, 3), 2*Pow(x, 3) + Pow(y, 3)),       /// Полином третьей степени 
            4 => new Complex(Pow(x, 4) + Pow(y, 4), 2*Pow(x, 4) + Pow(y, 4)),       /// Полином четвертой степени 
            5 => new Complex(Sin(x + y), Cos(2*x + y)),                             /// Не полином

            _ => 0,
        };
    }

    //* Значения F-функции
    public static Complex F(Vector<double> vec) {
        (double x, double y) = (vec[0], vec[1]);
        return numberFunc switch 
        {
            1 => gamma * (new Complex(-2*x - y, x + y)),                                                                                    /// Полином первой степени
            2 => gamma * (new Complex(-2*x*x - y*y, x*x + y*y)) - new Complex(8, 12),                                                       /// Полином второй степени
            3 => gamma * (new Complex(-2*Pow(x, 3) - Pow(y, 3), Pow(x, 3) + Pow(y, 3))) - new Complex(12*x + 12*y, 24*x + 12*y),            /// Полином третьей степени
            4 => gamma * (new Complex(-2*Pow(x, 4) - Pow(y, 4), Pow(x, 4) + Pow(y, 4))) - new Complex(24*x*x + 24*y*y, 48*x*x + 24*y*y),    /// Полином четвертой степени
            5 => gamma * (new Complex(-Cos(2*x + y), Sin(x + y))) - new Complex(-4*Sin(x + y), -10*Cos(2*x + y)),                           /// Не полином

            _ => 0,
        };
    }


}