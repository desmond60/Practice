namespace Practice;
public class FEM 
{
    private Node[]     Nodes;               /// Узлы
    private Elem[]     Elems;               /// КЭ
    private Kraev[]    Kraevs;              /// Краевые
    private SLAU       slau;                /// Структура СЛАУ
    private Matrix part_M;                  /// Неполная матрица масс (M)
    private Matrix part_G_left;             /// Неполная левая матрица жесткости (G)
    private Matrix part_G_right;            /// Неполная правая матрица жесткости (G)

    public string Path { get; set; }        /// Путь к задаче

    //* Конструктор
    public FEM(Grid grid, string path) {
        (Nodes, Elems, Kraevs) = grid;
        this.Path = path;

        part_G_left = new Matrix(new double[4, 4]{
            {2, -2, 1, -1},
            {-2, 2, -1, 1},
            {1, -1, 2, -2},
            {-1, 1, -2, 2}
        });

        part_G_right = new Matrix(new double[4, 4]{
            {2, 1, -2, -1},
            {1, 2, -1, -2},
            {-2, -1, 2, 1},
            {-1, -2, 1, 2}
        });

        part_M = new Matrix(new double[4, 4]{
            {4, 2, 2, 1},
            {2, 4, 1, 2},
            {2, 1, 4, 2},
            {1, 2, 2, 4}
        });
    }

    //* Основной метод решения
    public void solve() {
        portrait();                                               //? Составление портрета матрицы
        global();                                                 //? Составление глобальной матрицы
        LOS los = new(slau, 10000, 9e-30);                        //? Создание метода LOS
        los.solve(true);                                          //? Решение СЛАУ методом ЛОС (диагональный)
        AbsolutSolve();                                           //? Абсолютное решение СЛАУ
        WriteMatrix();                                            //? Запись матрицы и вектора решения СЛАУ 
        WriteTable();                                             //? Запись таблички с решением и погрешностью
    }

    //* Составление портрета матрицы (ig, jg, выделение памяти)
    private void portrait() {
        Portrait port = new Portrait(Nodes.Length);

        // Генерируем массивы ig и jg и размерность
        slau.N_el = port.GenPortrait(ref slau.ig, ref slau.jg, Elems);
        slau.N    = Nodes.Length;

        // Выделяем память
        slau.gg         = new ComplexVector(slau.N_el);
        slau.di         = new ComplexVector(slau.N);
        slau.f          = new ComplexVector(slau.N);
        slau.q          = new ComplexVector(slau.N);
        slau.q_absolut  = new ComplexVector(slau.N);
    }

    //* Построение глобальной матрицы
    private void global() {

        // Для каждого КЭ
        for (int index_fin_el = 0; index_fin_el < Elems.Length; index_fin_el++) {
            // Состовляем локальную матрицы и локальный вектор
            (ComplexMatrix loc_mat, ComplexVector local_f) = local(index_fin_el);

            // Занесение в глобальную
            EntryMatInGlobalMatrix(loc_mat, Elems[index_fin_el].Node);
            EntryVecInGlobalMatrix(local_f, Elems[index_fin_el].Node);
        }

        // Для каждого условия на границе
        for (int index_kraev_cond = 0; index_kraev_cond < Kraevs.Length; index_kraev_cond++) {
            Kraev cur_kraev = Kraevs[index_kraev_cond];
            if (cur_kraev.numKraev == 1)
                First_Kraev(cur_kraev, cur_kraev.numSide);
            // else if (cur_kraev.NumKraev == 2) {
            //     Vector corr_vec = Second_Kraev(cur_kraev, index_t);
            //     EntryVecInGlobalMatrix(corr_vec, Kraevs[index_kraev_cond].Node);
            // } else {
            //     (double[][] corr_mat, Vector corr_vec) = Third_Kraev(cur_kraev, index_t);
            //     EntryMatInGlobalMatrix(corr_mat, Kraevs[index_kraev_cond].Node);
            //     EntryVecInGlobalMatrix(corr_vec, Kraevs[index_kraev_cond].Node);
            // }
        }
    }

    //* Занесение матрицы в глоабальную матрицу
    private void EntryMatInGlobalMatrix(ComplexMatrix mat, int[] index) {
        for (int i = 0, h = 0; i < mat.Dim; i++) {
            int ibeg = index[i];
            for (int j = i + 1; j < mat.Dim; j++) {
                int iend = index[j];
                int temp = ibeg;

                if (temp < iend)
                    (iend, temp) = (temp, iend);

                h = slau.ig[temp];
                while (slau.jg[h++] - iend != 0);
                slau.gg[--h] += mat[i, j];
            }
            slau.di[ibeg] += mat[i, i];
        }
    }

    //* Занесение вектора в глолбальный вектор
    private void EntryVecInGlobalMatrix(ComplexVector vec, int[] index) {
        for (int i = 0; i < vec.Length; i++)
            slau.f[index[i]] += vec[i];
    }

    //* Построение локальной матрицы и вектора
    private (ComplexMatrix, ComplexVector) local(int index_fin_el) {
        // Подсчет компонент
        double hx   = Nodes[Elems[index_fin_el].Node[1]].x - Nodes[Elems[index_fin_el].Node[0]].x;
        double hy   = Nodes[Elems[index_fin_el].Node[2]].y - Nodes[Elems[index_fin_el].Node[0]].y;
        
        ComplexVector local_f      = build_F(index_fin_el, hx, hy);    // Построение локальной правой части
        ComplexMatrix M            = build_M(index_fin_el, hx, hy);    // Построение матрицы массы (M)
        Matrix G                   = build_G(index_fin_el, hx, hy);    // Построение матрицы жесткости (G)
        ComplexMatrix local_matrix = G + (new Complex(0, 1))*M;

        return (local_matrix, local_f);
    }

    //* Построение вектора правой части
    private ComplexVector build_F(int index_fin_el, double hx, double hy) {
        // Подсчет коэффициента
        double coef = (hx * hy) / 36.0;

        // Вычисление f - на узлах КЭ 
        var f = new ComplexVector(4);                               
        for (int i = 0; i < f.Length; i++) {
            Vector vec = new Vector(new double[]{ Nodes[Elems[index_fin_el].Node[i]].x, Nodes[Elems[index_fin_el].Node[i]].y});
            f[i] = F(vec);
        }

        // Вычисление локального вектора
        var local_f = part_M * (coef * f);

        return local_f;
    }

    //* Построение матрицы масс
    private ComplexMatrix build_M(int index_fin_el, double hx, double hy) {
        // Подсчет коэффициента
        Complex coef = (Function.gamma * hx * hy) / 36.0;
        
        // Матрица масс
        var M_matrix = coef * part_M;                                 

        return M_matrix;
    }

    //* Построение матрицы жесткости
    private Matrix build_G(int index_fin_el, double hx, double hy) {
        // Подсчет коэффициентов
        double coef_left  = (lambda * hy) / (6 * hx); 
        double coef_right = (lambda * hx) / (6 * hy);

        // Матрица жесткости
        var G_matrix = coef_left * part_G_left + coef_right * part_G_right;                            
        
        return G_matrix;
    }

    //* Учет первого краевого условия
    private void First_Kraev(Kraev kraev, int side) {
        // Ставим вместо диагонального эл. единицу
        slau.di[kraev.Node[0]] = new Complex(1, 0);
        slau.di[kraev.Node[1]] = new Complex(1, 0);

        // В вектор правой части ставим значение краевого условия
        Vector vec0 = new Vector(new double[] {Nodes[kraev.Node[0]].x, Nodes[kraev.Node[0]].y});
        Vector vec1 = new Vector(new double[] {Nodes[kraev.Node[1]].x, Nodes[kraev.Node[1]].y});
        slau.f[kraev.Node[0]] = Absolut(vec0);
        slau.f[kraev.Node[1]] = Absolut(vec1);

        // Зануляем в строке все стоящие элементы кроме диагонального и сразу делаем симметричной
        for (int k = 0; k < 2; k++) {

            // Зануление в нижнем треугольнике
            for (int i = slau.ig[kraev.Node[k]]; i < slau.ig[kraev.Node[k] + 1]; i++) {
                if (slau.di[slau.jg[i]] != 1)
                    slau.f[slau.jg[i]] -= slau.gg[i] * slau.f[kraev.Node[k]];
                slau.gg[i] = 0;
            }

            // Зануление в верхнем треугольнике, но т.к. делаем симметричную "зануление в нижнем"
            for (int i = kraev.Node[k] + 1; i < Nodes.Length; i++) {
                int lbeg = slau.ig[i];
                int lend = slau.ig[i + 1];
                for (int p = lbeg; p < lend; p++)
                    if (slau.jg[p] == kraev.Node[k])
                    {
                        if (slau.di[i] != 1)
                            slau.f[i] -= slau.gg[p] * slau.f[kraev.Node[k]];
                        slau.gg[p] = 0;
                    }
            }
        }
    }

    //* Расчет погрешности и нормы решения
    private (ComplexVector, double) Norm(ComplexVector x_abs, ComplexVector x) {
        ComplexVector norm_arr = new ComplexVector(x.Length);

        for (int i = 0; i < x.Length; i++) {
            norm_arr[i] = x_abs[i] - x[i];
            norm_arr[i] = new Complex(Abs(norm_arr[i].Real), Abs(norm_arr[i].Imaginary));
        }
        return (norm_arr, Helper.Norm(norm_arr));
    }

    //* Абсолютное решение СЛАУ
    private void AbsolutSolve() {
        for (int i = 0; i < Nodes.Length; i++) {
            Vector vec = new Vector(new double[] { Nodes[i].x, Nodes[i].y });
            slau.q_absolut[i] = Absolut(vec);
        }
    }

    //* Запись глобальной матрицы и решения
    private void WriteMatrix() {
        Directory.CreateDirectory(Path + "/matrix");
        Directory.CreateDirectory(Path + "/output");
        File.WriteAllText(Path + "/matrix/ig.txt", String.Join("\n", slau.ig));
        File.WriteAllText(Path + "/matrix/jg.txt", String.Join("\n", slau.jg));
        File.WriteAllText(Path + "/matrix/di.txt", String.Join("\n", slau.di));
        File.WriteAllText(Path + "/matrix/gg.txt", String.Join("\n", slau.gg));
        File.WriteAllText(Path + "/matrix/pr.txt", String.Join("\n", slau.f));
        File.WriteAllText(Path + "/output/x.txt",  String.Join("\n", slau.q));
        File.WriteAllText(Path + "/output/x_absolut.txt", String.Join("\n", slau.q_absolut));
    }

    //* Запись таблички с погрешностью
    private void WriteTable() {
        (ComplexVector SubX, double norma) = Norm(slau.q_absolut, slau.q);

        StringBuilder table = new StringBuilder();
        string margin = String.Join("", Enumerable.Repeat("-", 35));

        table.Append(String.Join("", Enumerable.Repeat("-", 145)) + "\n");
        table.Append($"|X`{" ",-32} | X{" ",-32} | |X` - X|{" ",-25} | ||X` - X||  {" ",-21} |\n");
        table.Append($"|" + margin + "|" + margin + "|" + margin + "|" + margin + "|\n");

        for (int i = 0; i < Nodes.Length; i++) {
            table.Append($"|{String.Format("{0,35}", slau.q_absolut[i].ToString("E4"))}" +
                            $"|{String.Format("{0,35}", slau.q[i].ToString("E4"))}" +
                            $"|{String.Format("{0,35}", SubX[i].ToString("E6"))}|");
            if (Nodes.Length / 2 == i)
                table.Append($"{String.Format("{0,35}", norma.ToString("E6"))}|");
            else
                table.Append($"{String.Format("{0,35}", " ")}|");
            table.Append("\n");
        }
        table.Append(String.Join("", Enumerable.Repeat("-", 145)) + "\n");
        File.WriteAllText(Path + "/output/table.txt", table.ToString());
    }

}