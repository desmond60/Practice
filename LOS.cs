namespace Practice;
public class LOS
{   
    private SLAU slau;      /// Структура СЛАУ

    private int maxIter;    /// Максимальное количество итераций
    private double EPS;     /// Точность 
    
// ************ Коструктор LOS ************ //
    public LOS(SLAU slau, int maxIter, double eps) { 
        this.slau    = slau; 
        this.maxIter = maxIter;
        this.EPS     = eps;    
    } 

    //* Решение СЛАУ
    public ComplexVector solve(bool isLog = true) {
        var r      = new ComplexVector(slau.N);
        var z      = new ComplexVector(slau.N);
        var multLr = new ComplexVector(slau.N);
        var Lr     = new ComplexVector(slau.N);
        var p      = new ComplexVector(slau.N);
        Complex alpha, betta;
        double Eps;
        int iter = 0;

        ComplexVector L = new ComplexVector(Enumerable.Range(0, slau.N).Select(i => new Complex(1, 0) / slau.di[i]).ToArray());

        ComplexVector multX = slau.mult(slau.q);
        for (int i = 0; i < r.Length; i++) {
            r[i] = L[i] * (slau.f[i] - multX[i]);
            z[i] = L[i] * r[i];
        }
        ComplexVector multZ = slau.mult(z);
        for (int i = 0; i < p.Length; i++)
            p[i] = L[i] * multZ[i];

        do {
            betta = Scalar(p, p);
            alpha = Scalar(p, r) / betta;
            for (int i = 0; i < slau.q.Length; i++) {
                slau.q[i]  += alpha * z[i];
                r[i]       -= alpha * p[i];
                Lr[i]       = L[i] * r[i];
            }

            multLr = slau.mult(Lr);
            for (int i = 0; i < Lr.Length; i++)
                multLr[i] = L[i] * multLr[i];
            betta = -Scalar(p, multLr) / betta;
            for (int i = 0; i < z.Length; i++) {
                z[i] = L[i] * r[i] + betta * z[i];
                p[i] = multLr[i] + betta * p[i];
            }
            Eps = Norm(Scalar(r, r));

            iter++;

            if (isLog) printLog(iter, Eps);
        } while (iter < maxIter  && 
                 Eps  > EPS);

        return slau.q;
    }

    //* Вывод невязки на определенной итерации
    private void printLog(int Iter, double Eps) {
        WriteLine($"Iteration = {Iter}\t\t" + 
                  $"Discrepancy = {Eps}");
    }
}