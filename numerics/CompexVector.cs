namespace Practice.numerics;
public class ComplexVector : IEnumerable
{
    public Complex[] vector;            /// Вектор
    public int Length { get; init; }    /// Размерность вектора

    //* Реализация IEnumerable
    public IEnumerator GetEnumerator() => vector.GetEnumerator();

    //* Перегрузка неявного преобразования
    public static explicit operator Complex[](ComplexVector vec) {
        return vec.vector;
    }

    //* Конструктор (с размерностью)
    public ComplexVector(int lenght) {
        vector = new Complex[lenght];
        this.Length = lenght;
    }

    //* Конструктор (с массивом)
    public ComplexVector(Complex[] array) {
        vector = new Complex[array.Length];
        this.Length = array.Length;
        Array.Copy(array, vector, array.Length);
    }

    //* Индексатор
    public Complex this[int index] {
        get => vector[index];
        set => vector[index] = value;
    }

    //* Перегрузка умножения (на  число)
    public static ComplexVector operator *(double Const, ComplexVector vector) {
        var result = new ComplexVector(vector.Length);
        for (int i = 0; i < vector.Length; i++)
            result[i] = Const * vector[i];
        return result;
    }
    public static ComplexVector operator *(ComplexVector vector, double Const) => Const * vector;

    //* Перегрузка умножения (на комплексное число)
    public static ComplexVector operator *(Complex Const, ComplexVector vector) {
        var result = new ComplexVector(vector.Length);
        for (int i = 0; i < vector.Length; i++)
            result[i] = Const * vector[i];
        return result;
    }
    public static ComplexVector operator *(ComplexVector vector, Complex Const) => Const * vector;

    //* Перегрузка деления
    public static ComplexVector operator /(ComplexVector vector, double Const) {
        var result = new ComplexVector(vector.Length);
        for (int i = 0; i < vector.Length; i++)
            result[i] = vector[i] / Const;
        return result;
    }

    //* Перегрузка сложения
    public static ComplexVector operator +(ComplexVector vec1, ComplexVector vec2) {
        var result = new ComplexVector(vec1.Length);
        for (int i = 0; i < vec1.Length; i++)
            result[i] = vec1[i] + vec2[i];
        return result;
    }

    //* Перегрузка вычитания
    public static ComplexVector operator -(ComplexVector vec1, ComplexVector vec2) {
        var result = new ComplexVector(vec1.Length);
        for (int i = 0; i < vec1.Length; i++)
            result[i] = vec1[i] - vec2[i];
        return result;
    }

    // //* Перегрузка тернарного минуса
    public static ComplexVector operator -(ComplexVector vector) {
        var result = new ComplexVector(vector.Length);
        for (int i = 0; i < vector.Length; i++)
            result[i] = vector[i] * (-1);
        return result;
    }

    //* Копирование вектора
    public static void Copy(ComplexVector source, ComplexVector dest) {
        for (int i = 0; i < source.Length; i++)
            dest[i] = source[i];
    }

    //* Очистка вектора
    public static void Clear(ComplexVector vector) {
        for (int i = 0; i < vector.Length; i++)
            vector[i] = 0;
    }

    //* Выделение памяти под вектор
    public static void Resize(ref ComplexVector vector, int lenght) {
        vector = new(lenght);
    }

    //* Строковое представление вектора
    public override string ToString() {
        StringBuilder vec = new StringBuilder();
        if (vector == null) return vec.ToString();

        for (int i = 0; i < Length; i++)
            vec.Append(vector[i].ToString("E3") + "\n");    
        
        return vec.ToString();
    }
}