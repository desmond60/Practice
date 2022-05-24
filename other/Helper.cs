using Microsoft.Win32.SafeHandles;
namespace Practice.other;

public struct Grid    /// Структура сетки
{
    public int Count_Node  { get; set; }    /// Общее количество узлов
    public int Count_Elem  { get; set; }    /// Общее количество КЭ
    public int Count_Kraev { get; set; }    /// Количество краевых

    public Node[]  Nodes;      /// Узлы
    public Elem[]  Elems;      /// КЭ
    public Kraev[] Kraevs;     /// Краевые           

    public Grid(int count_node, int count_elem, int count_kraev, Node[] nodes, Elem[] elem, Kraev[] kraevs) {
        this.Count_Node  = count_node;
        this.Count_Elem  = count_elem;
        this.Count_Kraev = count_kraev;
        this.Nodes       = nodes;
        this.Elems       = elem;
        this.Kraevs      = kraevs;
    }

    public void Deconstruct(out Node[]  nodes,
                            out Elem[]  elems,
                            out Kraev[] kraevs) {
        nodes  = this.Nodes;
        elems  = this.Elems;
        kraevs = this.Kraevs;
    }
}

public struct Node     /// Структура Узла
{
    public double x { get; set; }  /// Координата X 
    public double y { get; set; }  /// Координата Y 

    public Node(double _x, double _y) {
        x = _x; y = _y;
    }

    public void Deconstruct(out double x, 
                              out double y) 
    {
        x = this.x;
        y = this.y;
    }

    public void Deconstruct(out double[] param) {
        param = new double[]{this.x, this.y};
    } 

    public override string ToString() => $"{x,20} {y,24}";
}

public struct Elem     /// Структура КЭ
{
    public int[] Node;  /// Узлы КЭ

    public Elem(params int[] node) { this.Node = node; }

    public void Deconstruct(out int[] nodes) { nodes = this.Node; }

    public override string ToString() {
        StringBuilder str_elem = new StringBuilder();
        str_elem.Append($"{Node[0],5}");
        for (int i = 1; i < Node.Count(); i++)
            str_elem.Append($"{Node[i],8}");
        return str_elem.ToString();
    }
}

public struct Kraev    /// Структура краевого
{
    public int[]    Node;        /// Узлы краевого (ребро)
    public int      numKraev;    /// Номер краевого
    public int      numSide;     /// Номер стороны на котором задано краевое
    

    public Kraev(int num, int side, params int[] node) { 
        this.numKraev = num;
        this.numSide  = side;
        this.Node     = node; 
    }

    public void Deconstruct(out int[] nodes, out int num, out int side) { 
        nodes = this.Node;
        num   = this.numKraev;
        side  = this.numSide; 
    }

    public override string ToString() {
        StringBuilder str_elem = new StringBuilder();
        str_elem.Append($"{numKraev} {numSide,5} {Node[0],5}");
        for (int i = 1; i < Node.Count(); i++)
            str_elem.Append($"{Node[i],8}");
        return str_elem.ToString();
    }
}

public struct SLAU     /// Структура СЛАУ
{
    public ComplexVector di, gg;         /// Матрица
    public int[] ig, jg;                 /// Массивы с индексами
    public ComplexVector f, q;           /// Правая часть и решение
    public ComplexVector q_absolut;      /// Абсолютные значения U-функции
    public int N;                        /// Размерность матрицы
    public int N_el;                     /// Размерность gl и gu

    //* Умножение матрицы на вектор
    public ComplexVector mult(ComplexVector x) {
        var y = new ComplexVector(x.Length);

        for (int i = 0, jj = 0; i < x.Length; i++) {
            y[i] = di[i] * x[i];

            for (int j = ig[i]; j < ig[i + 1]; j++, jj++) {
            y[i]      += gg[jj] * x[jg[jj]];
            y[jg[jj]] += gg[jj] * x[i];
            }
        }
        return y;
    }
}

public static class Helper
{

    //* Скалярное произведение векторов
    public static Complex Scalar(ComplexVector frst, ComplexVector scnd) {
        Complex res = 0;
        for (int i = 0; i < frst.Length; i++)
            res += frst[i]*scnd[i];
        return res;
    }

    //* Модуль комплексного вектора
    public static double Norm(ComplexVector vec) {
        double norm = 0;
        for (int i = 0; i < vec.Length; i++)
            norm += vec[i].Real*vec[i].Real + vec[i].Imaginary*vec[i].Imaginary;
        return Sqrt(norm);
    }

    //* Модуль комплексного числа
    public static double Norm(Complex ch) {
        return Sqrt(ch.Real*ch.Real + ch.Imaginary*ch.Imaginary);
    }

    //* Окно помощи при запуске (если нет аргументов или по команде)
    public static void ShowHelp() {
        WriteLine("----Команды----                        \n" + 
        "-help             - показать справку             \n" + 
        "-i                - входной файл                 \n");
    }
}