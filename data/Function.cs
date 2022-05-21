namespace Practice;
public static class Function
{
    public static uint      numberFunc;     /// Номер задачи
    public static double    lambda;         /// Значение Lambda
    public static double    gamma;          /// Значение gamma
    public static double    betta;          /// Значение betta

    //* Инициализация констант
    public static void Init(uint numF) {
        numberFunc = numF;

        switch(numberFunc) {
            case 1:                         /// Полином первой степени
                lambda = 1;          
                gamma  = 2;
            break;

        }
    }

    //* Абсолютное значение U-функции
    public static double Absolut(Vector vec) {
        (double x, double y) = vec;
        return numberFunc switch 
        {
            1 => x + y,                     /// Полином первой степени 

            _ => 0,
        };
    }

    //* Значения F-функции
    public static double F(Vector vec) {
        (double x, double y) = vec;
        return numberFunc switch 
        {
            1 => 2*x + 2*y,                  /// Полином первой степени

            _ => 0,
        };
    }


}