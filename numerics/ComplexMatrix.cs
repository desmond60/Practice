namespace Practice.numerics;
public class ComplexMatrix
{
    private Complex[,] matrix;               /// Матрица (квадратная)
    public int Dim { get; init; }            /// Размерность матрицы

    //* Конструктор (с размерностью)
    public ComplexMatrix(int _dim) {
        matrix = new Complex[_dim, _dim];
        this.Dim = _dim;
    }

    //* Конструктор (с двумерным массивом)
    public ComplexMatrix(Complex[,] _mat) {        
        Dim = _mat.GetUpperBound(0) + 1;
        matrix = new Complex[Dim, Dim];
        for (int i = 0; i < Dim; i++)
            for (int j = 0; j < Dim; j++)
                matrix[i, j] = _mat[i, j];           
    }
 
    //* Деконструктор
    public void Deconstruct(out Complex[,] mat) {
        mat = this.matrix;
    }

    //* Индексаторы
    public Complex this[int i, int j] {
        get { return matrix[i, j];  }
        set { matrix[i, j] = value; }
    }

    //* Перегрузка умножения
    public static ComplexVector operator *(ComplexMatrix mat, ComplexVector vec) {
        var result = new ComplexVector(vec.Length);
        for (int i = 0; i < vec.Length; i++)
            for (int j = 0; j < vec.Length; j++)
                result[i] += mat[i, j] * vec[j];

        return result;
    }

    //* Перегрузка умножение числа на матрицу
    public static ComplexMatrix operator *(double _const, ComplexMatrix mat) {
        var result = new ComplexMatrix(mat.Dim);
        for (int i = 0; i < result.Dim; i++)
            for (int j = 0; j < result.Dim; j++)
                result[i, j] = _const * mat[i, j];

        return result;
    }

    //* Перегрузка умножение Комплексного числа на матрицу
    public static ComplexMatrix operator *(Complex _const, ComplexMatrix mat) {
        var result = new ComplexMatrix(mat.Dim);
        for (int i = 0; i < result.Dim; i++)
            for (int j = 0; j < result.Dim; j++)
                result[i, j] = _const * mat[i, j];

        return result;
    }

    //* Перегрузка сложение матриц 
    public static ComplexMatrix operator +(ComplexMatrix mat1, ComplexMatrix mat2) {
        var result = new ComplexMatrix(mat1.Dim);
        for (int i = 0; i < result.Dim; i++)
            for (int j = 0; j < result.Dim; j++)
                result[i, j] = mat1[i, j] + mat2[i, j];

        return result;
    }

    //* Перегрузка сложение матриц 
    public static ComplexMatrix operator +(Matrix mat1, ComplexMatrix mat2) {
        var result = new ComplexMatrix(mat1.Dim);
        for (int i = 0; i < result.Dim; i++)
            for (int j = 0; j < result.Dim; j++)
                result[i, j] = mat1[i, j] + mat2[i, j];

        return result;
    }

    public static ComplexMatrix operator +(ComplexMatrix mat1, Matrix mat2) => mat2 + mat1;

    //* Копирование матриц
    public static void Copy(ComplexMatrix source, ComplexMatrix dest) {
        for (int i = 0; i < source.Dim; i++)
            for (int j = 0; j < source.Dim; j++)
                dest[i, j] = source[i, j];
    }

    //* Строковое представление матрицы
    public override string ToString() {
        StringBuilder mat = new StringBuilder();
        if (matrix == null) return mat.ToString();

        for (int i = 0; i < Dim; i++) {
            for (int j = 0; j < Dim; j++)
                mat.Append(matrix[i, j] + "\t");
            mat.Append("\n");
        }
        return mat.ToString();
    }

}