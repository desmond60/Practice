namespace Practice;
public class Data
{
    //* Данные для генерации сетки
    public double[] start { get; set; }       /// Начальная точка
    public double[] end   { get; set; }       /// Конечная точка
    public double   hx    { get; set; }       /// Шаг по Оси X
    public double   hy    { get; set; }       /// Шаг по Оси Y
    public double   kx    { get; set; }       /// Коэффициент разрядки по Оси X  
    public double   ky    { get; set; }       /// Коэффициент разрядки по Оси Y  
    public uint     N     { get; set; }       /// Номер функции

    //* Деконструктор
    public void Deconstruct(out Vector<double>   Start, 
                            out Vector<double>   End,
                            out double           Hx, 
                            out double           Hy, 
                            out double           Kx,
                            out double           Ky) 
    {
        Start = new Vector<double>(start);
        End   = new Vector<double>(end);
        Hx    = hx;
        Hy    = hy;
        Kx    = kx;
        Ky    = ky;
    }

    //* Проверка входных данных
    public bool Incorrect(out string mes) {
        StringBuilder errorStr = new StringBuilder("");
        
        if (start[0] > end[0])
            errorStr.Append($"Incorrect data (start[0] > end[0]): {start[0]} > {end[0]}\n");

        if (start[1] > end[1])
            errorStr.Append($"Incorrect data (start[1] > end[1]): {start[1]} > {end[1]}\n");

        if (hx <= 0)
            errorStr.Append($"Incorrect data (hx <= 0): {hx} <= {0}\n");

        if (hy <= 0)
            errorStr.Append($"Incorrect data (hy <= 0): {hy} <= {0}\n");

        if (hx == hy)
            errorStr.Append($"Incorrect data (hx == hy): {hx} == {hy}\n");

        if (kx < 1)
            errorStr.Append($"Incorrect data (kx < 1): {kx} < {1}\n");

        if (ky < 1)
            errorStr.Append($"Incorrect data (ky < 1): {ky} < {1}\n");

        if (!errorStr.ToString().Equals("")) {
            mes = errorStr.ToString();
            return false;
        }

        mes = errorStr.ToString();
        return true;
    }
}